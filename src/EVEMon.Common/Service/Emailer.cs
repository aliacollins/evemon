using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Helpers;
using EVEMon.Common.Models;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common.Service
{
    /// <summary>
    /// Provides SMTP based e-mail services taylored to Skill Completion.
    /// </summary>
    public static class Emailer
    {
        private static SmtpClient s_smtpClient;
        private static MailMessage s_mailMessage;
        private static bool s_isTestMail;

        /// <summary>
        /// Sends a test mail
        /// </summary>
        /// <param name="settings">NotificationSettings object</param>
        /// <remarks>
        /// A notification settings object is required, as this function
        /// is called from the Settings Window, and assumibly the user
        /// is changing settings.
        /// </remarks>
        /// <returns>False if an exception was thrown, otherwise True.</returns>
        public static void SendTestMail(NotificationSettings settings)
        {
            s_isTestMail = true;
            SendMail(settings, "EVEMon Test Mail", "This is a test email sent by EVEMon");
        }

        /// <summary>
        /// Sends a mail alert for a skill completion
        /// </summary>
        /// <param name="queueList">Current Skill Queue</param>
        /// <param name="skill">Skill that has just completed</param>
        /// <param name="character">Character affected</param>
        /// <returns></returns>
        public static void SendSkillCompletionMail(IList<QueuedSkill> queueList, QueuedSkill skill, Character character)
        {
            s_isTestMail = false;

            if (queueList == null)
                throw new ArgumentNullException(nameof(queueList));

            if (skill == null)
                throw new ArgumentNullException(nameof(skill));

            CCPCharacter ccpCharacter = character as CCPCharacter;

            // Current character isn't a CCP character, so can't have a Queue.
            if (ccpCharacter == null)
                return;

            string charName = character.Name;
            string skillName = skill.SkillName;
            string skillLevelString = Skill.GetRomanFromInt(skill.Level);

            // Message's first line
            StringBuilder body = new StringBuilder();
            body.Append($"{charName} has finished training {skillName} {skillLevelString}.")
                .AppendLine()
                .AppendLine();

            // Next skills in queue
            if (queueList[0] != null)
            {
                var plural = queueList.Count > 1 ? "s" : String.Empty;
                body.Append($"Next skill{plural} in queue:")
                    .AppendLine();

                foreach (QueuedSkill qskill in queueList)
                {
                    body.AppendFormat($"- {qskill}").AppendLine();
                }
                body.AppendLine();
            }
            else
                body.Append("Character is not training.")
                    .AppendLine()
                    .AppendLine();

            // Free room in skill queue
            DateTime skillQueueEndTime = ccpCharacter.SkillQueue.EndTime;
            if (skillQueueEndTime < DateTime.UtcNow.AddHours(EveConstants.SkillQueueDuration))
            {
                TimeSpan timeLeft = DateTime.UtcNow.AddHours(EveConstants.SkillQueueDuration).Subtract(skillQueueEndTime);
                string timeLeftText = timeLeft.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas, false);
                body.Append($"There is also {timeLeftText} free room in skill queue.")
                    .AppendLine();
            }

            // Short format (also for SMS)
            if (Settings.Notifications.UseEmailShortFormat)
            {
                SendMail(Settings.Notifications,
                    $"[STC] {charName} :: {skillName} {skillLevelString}",
                    body.ToString());

                return;
            }

            // Long format
            if (character.Plans.Count > 0)
            {
                body.Append("Next skills listed in plans:")
                    .AppendLine()
                    .AppendLine();
            }

            foreach (Plan plan in character.Plans)
            {
                if (plan.Count <= 0)
                    continue;

                // Print plan name
                CharacterScratchpad scratchpad = new CharacterScratchpad(character);
                body.Append($"{plan.Name}:").AppendLine();

                // Scroll through entries
                int i = 0;
                int minDays = 1;
                foreach (PlanEntry entry in plan)
                {
                    TimeSpan trainTime = scratchpad.GetTrainingTime(entry.Skill, entry.Level,
                        TrainingOrigin.FromPreviousLevelOrCurrent);

                    // Only print the first three skills, and the very long skills
                    // (first limit is one day, then we add skills duration)
                    if (++i > 3 && trainTime.Days <= minDays)
                        continue;

                    if (i > 3)
                    {
                        // Print long message once
                        if (minDays == 1)
                            body.AppendLine().Append($"Longer skills from {plan.Name}:").AppendLine();

                        minDays = trainTime.Days + minDays;
                    }
                    body.Append($"\t{entry}");

                    // Notes
                    if (!string.IsNullOrEmpty(entry.Notes))
                        body.Append($" ({entry.Notes})");

                    // Training time
                    body.Append(trainTime.Days > 0 ? $" - {trainTime.Days}d, {trainTime}" : $" - {trainTime}");

                    body.AppendLine();
                }
                body.AppendLine();
            }

            SendMail(Settings.Notifications,
                $"{charName} has finished training {charName} {skillLevelString}",
                body.ToString());
        }

        /// <summary>
        /// Triggers on when a SMTP client has finished (success or failure)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                EveMonClient.Trace("The last message was cancelled");
            }
            else if (e.Error != null)
            {
                EveMonClient.Trace("An error occurred");
                ExceptionHandler.LogException(e.Error, true);
                MessageBox.Show(e.Error.Message, @"EVEMon Emailer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (s_isTestMail)
                {
                    MessageBox.Show(@"The message sent successfully. Please verify that the message was received.",
                        @"EVEMon Emailer Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                EveMonClient.Trace("Message sent.");
            }

            s_mailMessage.Dispose();
            s_smtpClient.Dispose();
        }

        /// <summary>
        /// Performs the sending of the mail
        /// </summary>
        /// <param name="settings">Settings object to use when sending</param>
        /// <param name="subject">Subject of the message</param>
        /// <param name="body">Body of the message</param>
        /// <returns>True if no exceptions thrown, otherwise false</returns>
        /// <remarks>
        /// NotificationSettings object is required to support
        /// alternative settings from Tools -> Options. Use
        /// Settings.Notifications unless using an alternative
        /// configuration.
        /// </remarks>
        private static void SendMail(NotificationSettings settings, string subject, string body)
        {
            // Trace something to the logs so we can identify the time the message was sent
            EveMonClient.Trace($"(Subject - {subject}; Server - {settings.EmailSmtpServerAddress}:{settings.EmailPortNumber})");

            string sender = String.IsNullOrEmpty(settings.EmailFromAddress)
                ? "no-reply@evemon.net"
                : settings.EmailFromAddress;

            List<string> toAddresses = settings.EmailToAddress.Split(
                new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            try
            {
                // Set up message
                s_mailMessage = new MailMessage();
                toAddresses.ForEach(address => s_mailMessage.To.Add(address.Trim()));
                s_mailMessage.From = new MailAddress(sender);
                s_mailMessage.Subject = subject;
                s_mailMessage.Body = body;

                // Set up client
                s_smtpClient = GetClient(settings);

                // Send message
                s_smtpClient.SendAsync(s_mailMessage, null);
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, true);
                MessageBox.Show(@"The message failed to send.", @"EVEMon Emailer Failure", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        private static SmtpClient GetClient(NotificationSettings settings)
        {
            SmtpClient client = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = (int)TimeSpan.FromSeconds(Settings.Updates.HttpTimeout).TotalMilliseconds,

                // Host and port
                Host = settings.EmailSmtpServerAddress,
                Port = settings.EmailPortNumber,

                // SSL
                EnableSsl = settings.EmailServerRequiresSsl
            };

            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

            client.SendCompleted += SendCompleted;

            if (!settings.EmailAuthenticationRequired)
                return client;

            // Credentials
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(settings.EmailAuthenticationUserName,
                Util.Decrypt(settings.EmailAuthenticationPassword,
                    settings.EmailAuthenticationUserName));

            return client;
        }
    }
}