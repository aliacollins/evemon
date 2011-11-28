using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common
{
    /// <summary>
    /// Provides SMTP based e-mail services taylored to Skill Completion.
    /// </summary>
    public static class Emailer
    {
        private static SmtpClient s_smtpClient;

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
        public static bool SendTestMail(NotificationSettings settings)
        {
            return SendMail(settings, "EVEMon Test Mail", "This is a test email sent by EVEMon");
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
            if (queueList == null)
                throw new ArgumentNullException("queueList");

            if (skill == null)
                throw new ArgumentNullException("skill");

            CCPCharacter ccpCharacter = character as CCPCharacter;

            // Current character isn't a CCP character, so can't have a Queue.
            if (ccpCharacter == null)
                return;

            string charName = character.Name;
            string skillName = skill.SkillName;
            string skillLevelString = Skill.GetRomanFromInt(skill.Level);

            DateTime skillQueueEndTime = ccpCharacter.SkillQueue.EndTime;
            bool freeTime = skillQueueEndTime < DateTime.UtcNow.AddHours(24);
            TimeSpan timeLeft = DateTime.UtcNow.AddHours(24).Subtract(skillQueueEndTime);
            string timeLeftText = timeLeft.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas, false);

            // Message's first line
            StringBuilder body = new StringBuilder();
            body.AppendFormat(CultureConstants.DefaultCulture, "{0} has finished training {1} {2}.{3}{3}", charName, skillName,
                              skillLevelString, Environment.NewLine);

            // Next skills in queue
            if (queueList[0] != null)
            {
                body.AppendFormat(CultureConstants.DefaultCulture, "Next skill{0} in queue:{1}",
                                  (queueList.Count > 1 ? "s" : String.Empty), Environment.NewLine);
                foreach (QueuedSkill qskill in queueList)
                {
                    body.AppendFormat(CultureConstants.DefaultCulture, "- {0}{1}", qskill, Environment.NewLine);
                }
                body.AppendLine();
            }
            else
                body.AppendFormat(CultureConstants.DefaultCulture, "Character is not training.{0}{0}", Environment.NewLine);

            // Free room in skill queue
            if (freeTime)
            {
                body.AppendFormat(CultureConstants.DefaultCulture, "There is also {0} free room in skill queue.{1}", timeLeftText,
                                  Environment.NewLine);
            }

            // Short format (also for SMS)
            if (Settings.Notifications.UseEmailShortFormat)
            {
                SendMail(Settings.Notifications,
                         String.Format(CultureConstants.DefaultCulture, "[STC] {0} :: {1} {2}", charName, skillName,
                                       skillLevelString), body.ToString());
                return;
            }

            // Long format
            if (character.Plans.Count > 0)
                body.AppendFormat(CultureConstants.DefaultCulture, "Next skills listed in plans:{0}{0}", Environment.NewLine);

            foreach (Plan plan in character.Plans)
            {
                if (plan.Count <= 0)
                    continue;

                // Print plan name
                CharacterScratchpad scratchpad = new CharacterScratchpad(character);
                body.AppendFormat(CultureConstants.DefaultCulture, "{0}:{1}", plan.Name, Environment.NewLine);

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
                        {
                            body.AppendFormat(CultureConstants.DefaultCulture, "{1}Longer skills from {0}:{1}", plan.Name,
                                              Environment.NewLine);
                        }

                        minDays = trainTime.Days + minDays;
                    }
                    body.AppendFormat(CultureConstants.DefaultCulture, "\t{0}", entry);

                    // Notes
                    if (!string.IsNullOrEmpty(entry.Notes))
                        body.AppendFormat(CultureConstants.DefaultCulture, " ({0})", entry.Notes);

                    // Training time
                    if (trainTime.Days > 0)
                        body.AppendFormat(CultureConstants.DefaultCulture, " - {0}d, {1}", trainTime.Days, trainTime);
                    else
                        body.AppendFormat(CultureConstants.DefaultCulture, " - {0}", trainTime);

                    body.AppendLine();
                }
                body.AppendLine();
            }

            string subject = String.Format(CultureConstants.DefaultCulture, "{0} has finished training {1} {2}", charName,
                                           skillName, skillLevelString);

            SendMail(Settings.Notifications, subject, body.ToString());
        }

        /// <summary>
        /// Triggers on when a SMTP client has finished (success or failure)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
                EveMonClient.Trace("Emailer.SendCompleted - The last message was cancelled");
            else if (e.Error != null)
            {
                EveMonClient.Trace("Emailer.SendCompleted - An error occurred");
                ExceptionHandler.LogException(e.Error, false);
                MessageBox.Show(e.Error.Message, "EVEMon Emailer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                EveMonClient.Trace("Emailer.SendCompleted - Message sent.");
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
        private static bool SendMail(NotificationSettings settings, string subject, string body)
        {
            // Trace something to the logs so we can identify the time the message was sent
            EveMonClient.Trace("Emailer.SendMail: Subject - {0}; Server - {1}:{2}",
                               subject,
                               settings.EmailSmtpServerAddress,
                               settings.EmailPortNumber);

            string sender = String.IsNullOrEmpty(settings.EmailFromAddress)
                                ? "evemonclient@battleclinic.com"
                                : settings.EmailFromAddress;

            List<string> toAddresses = settings.EmailToAddress.Split(
                new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            try
            {
                // Set up message
                MailMessage msg = new MailMessage();
                toAddresses.ForEach(address => msg.To.Add(address.Trim()));
                msg.From = new MailAddress(sender);
                msg.Subject = subject;
                msg.Body = body;

                // Set up client
                s_smtpClient = new SmtpClient();

                s_smtpClient.SendCompleted += SendCompleted;
                s_smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                s_smtpClient.Timeout = (int)TimeSpan.FromSeconds(Settings.Updates.HttpTimeout).TotalMilliseconds;
                
                // Host and port
                s_smtpClient.Host = settings.EmailSmtpServerAddress;
                s_smtpClient.Port = settings.EmailPortNumber;

                // SSL
                s_smtpClient.EnableSsl = settings.EmailServerRequiresSsl;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                // Credentials
                if (settings.EmailAuthenticationRequired)
                {
                    s_smtpClient.UseDefaultCredentials = false;
                    s_smtpClient.Credentials = new NetworkCredential(settings.EmailAuthenticationUserName,
                                                                     Util.Decrypt(settings.EmailAuthenticationPassword,
                                                                     settings.EmailAuthenticationUserName));
                }

                // Send message
                s_smtpClient.SendAsync(msg, null);

                return true;
            }
            catch (InvalidOperationException e)
            {
                ExceptionHandler.LogException(e, true);
                return false;
            }
            catch (SmtpException e)
            {
                ExceptionHandler.LogException(e, true);
                return false;
            }
        }
    }
}