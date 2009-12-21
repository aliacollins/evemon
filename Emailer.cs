using System;
using System.Net;
using System.Net.Mail;
using EVEMon.Common;
using System.Text;

namespace EVEMon
{
	internal static class Emailer
	{
		public static bool SendTestMail(Settings settings)
		{
			return SendMail(settings,
							"EVE Character Monitor Test Mail",
							"This is a test email sent by EVE Character Monitor");
		}

		public static bool SendAlertMail(Settings settings, int SkillLevel, string skillName, CharacterInfo characterInfo)
		{
			string charName = characterInfo.Name;
			bool useShortFormat = settings.EmailUseShortFormat;
			StringBuilder messageText = new StringBuilder();
			messageText.Append(charName + " has finished training " + skillName + " ");
			string skillLevelString = Skill.GetRomanForInt(SkillLevel);

			if (!useShortFormat)
			{
				messageText.Append("\r\n\r\nNext skills listed in plans:\r\n\r\n");

				foreach (string planName in settings.GetPlansForCharacter(charName))
				{

					Plan p = settings.GetPlanByName(charName, characterInfo, planName);
					if (p.Entries.Count > 0)
					{
						EveAttributeScratchpad scratchpad = new EveAttributeScratchpad();
						messageText.Append(planName + ":\r\n");
						int i = 0;
						int minDays = 1;
						foreach (Plan.Entry entry in p.Entries)
						{

							if (entry.Level > entry.Skill.LastConfirmedLvl)
							{
								TimeSpan trainTime = entry.Skill.GetTrainingTimeOfLevelOnly(entry.Level, true, scratchpad);
								//show 5 skills + day epoch skills
								if (++i <= 3 || trainTime.Days > minDays)
								{
									if (i > 3)
									{
										//print long message once
										if (minDays == 1)
										{
											messageText.Append("\r\n" + "Longer skills from " + planName + ":\r\n");
										}

										minDays = trainTime.Days + minDays;
									}
									messageText.Append("\t" + entry.SkillName + " " + entry.Level);

									if (entry.Notes != null && entry.Notes.Length > 0)
									{
										messageText.Append(" (" + entry.Notes + ")");
									}

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
						}
						messageText.Append("\r\n");
					}
				}
				return SendMail(settings, charName + " has finished training " + skillName + " " + skillLevelString, messageText.ToString());
			}

			// Shorter subject also for SMS Format
			return SendMail(settings, "[STC] " + charName + " :: " + skillName + " " + skillLevelString, messageText.ToString());
			//return SendMail(settings, charName + " skill " + skillName + " " + skillLevelString + " complete", messageText.ToString());
		}

		private static bool SendMail(Settings settings, string subject, string body)
		{
			try
			{
				MailMessage msg = new MailMessage(settings.EmailFromAddress, settings.EmailToAddress, subject, body);
				SmtpClient cli = new SmtpClient(settings.EmailServer);
				if (settings.PortNumber > 0)
				{
					cli.Port = settings.PortNumber;
				}

				if (settings.EmailAuthRequired)
				{
					cli.Credentials = new NetworkCredential(settings.EmailAuthUsername, settings.EmailAuthPassword);
				}
				cli.EnableSsl = settings.EmailServerRequiresSsl;
				cli.Send(msg);
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
