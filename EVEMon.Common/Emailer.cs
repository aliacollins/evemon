using System;
using System.Net;
using System.Net.Mail;
using EVEMon.Common;
using System.Text;
using EVEMon.Common.Net;
using EVEMon.Common.SettingsObjects;
using System.ComponentModel;

namespace EVEMon.Common
{
	public static class Emailer
	{
        /// <summary>
        /// Sends a test mail
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
		public static bool SendTestMail(NotificationSettings settings)
		{
			return SendMail(settings, "EVE Character Monitor Test Mail", "This is a test email sent by EVE Character Monitor");
		}

        /// <summary>
        /// Sens a mail alert for a skill completion
        /// </summary>
        /// <param name="skillLevel"></param>
        /// <param name="skillName"></param>
        /// <param name="character"></param>
        /// <returns></returns>
		public static bool SendSkillCompletionMail(QueuedSkill skill, Character character)
		{
            string charName = character.Name;
            string skillName = skill.SkillName;
            string skillLevelString = Skill.GetRomanForInt(skill.Level);
			
            CCPCharacter CCPCharacter = character as CCPCharacter;
            bool freeTime = CCPCharacter.SkillQueue.EndTime < DateTime.UtcNow.AddHours(24);
            TimeSpan timeLeft = DateTime.UtcNow.AddHours(24) - CCPCharacter.SkillQueue.EndTime;
            string timeLeftText = Skill.TimeSpanToDescriptiveText(timeLeft, DescriptiveTextOptions.IncludeCommas, false);

            // Message's first line
            StringBuilder messageText = new StringBuilder();
            messageText.AppendFormat("{0} has finished training {1} {2}",charName, skillName, skillLevelString).AppendLine();

            if (freeTime) messageText.AppendFormat("There is also {0} free room in skill queue", timeLeftText).AppendLine(); 
            
            // Short format (also for SMS)
            if (Settings.Notifications.UseEmailShortFormat)
            {
                return SendMail(Settings.Notifications, String.Format("[STC] {0} :: {1} {2}",charName, skillName, skillLevelString), messageText.ToString());
            }

            // Long format
			messageText.Append("Next skills listed in plans:\r\n\r\n");

            foreach (var plan in character.Plans)
			{
				if (plan.Count > 0)
				{
                    // Print plan name
                    CharacterScratchpad scratchpad = new CharacterScratchpad(character);
					messageText.Append(plan.Name + ":\r\n");

                    // Scroll through entries
					int i = 0;
					int minDays = 1;
					foreach (PlanEntry entry in plan)
					{
						TimeSpan trainTime = scratchpad.GetTrainingTime(entry.Skill, entry.Level, TrainingOrigin.FromPreviousLevelOrCurrent);

						// Only print the first three skills, and the very long skills (first limit is one day, then we add skills duration
						if (++i <= 3 || trainTime.Days > minDays)
						{
							if (i > 3)
							{
								// Print long message once
								if (minDays == 1)
								{
									messageText.Append("\r\n" + "Longer skills from " + plan.Name + ":\r\n");
								}

								minDays = trainTime.Days + minDays;
							}
							messageText.Append("\t" + entry.ToString());

                            // Notes
							if (entry.Notes != null && entry.Notes.Length > 0)
							{
								messageText.Append(" (" + entry.Notes + ")");
							}

                            // Training time
							string timeText = String.Format("{0:00}:{1:00}:{2:00}", trainTime.Hours, trainTime.Minutes, trainTime.Seconds);
							if (trainTime.Days > 0)
							{
								messageText.Append(" - " + trainTime.Days + "d, " + timeText);
							}
							else
							{
								messageText.Append(" - " + timeText);
							}

							messageText.Append("\r\n");
						}
					}
					messageText.Append("\r\n");
				}
			}

			return SendMail(Settings.Notifications, charName + " has finished training " + skillName + " " + skillLevelString, messageText.ToString());
		}

        /// <summary>
        /// Triggers on when a SMTP client has finished (sucess or failure)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                EveClient.Trace("Emailer.SendCompleted - The last message was cancelled");
            }
            if (e.Error != null)
            {
                EveClient.Trace("Emailer.SendCompleted - An error occured");
                ExceptionHandler.LogException(e.Error, false);
            }
            else
            {
                Console.WriteLine("Emailer.SendCompleted - Message sent.");
            }
        }

        /// <summary>
        /// Performs the sending fo the mail
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
		private static bool SendMail(NotificationSettings settings, string subject, string body)
		{
			try
			{
                // Set up message
                MailMessage msg = new MailMessage(settings.EmailFromAddress, settings.EmailToAddress, subject, body);

                // Set up client
                SmtpClient client = new SmtpClient(settings.EmailSmtpServer);
                if (settings.EmailPortNumber > 0)
				{
                    client.Port = settings.EmailPortNumber;
				}

                // Enter crendtials
                if (settings.EmailAuthenticationRequired)
				{
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(
                        settings.EmailAuthenticationUserName,
                        settings.EmailAuthenticationPassword);
				}

                // SSL
                client.EnableSsl = settings.EmailServerRequiresSSL;
                client.SendCompleted += new SendCompletedEventHandler(SendCompleted);

                // Send message
				client.SendAsync(msg, null);
				return true;
			}
			catch (Exception e)
			{
				ExceptionHandler.LogException(e, true);
				return false;
			}
		}
	}
}
