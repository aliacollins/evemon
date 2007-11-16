using System;
using System.Net;
using System.Net.Mail;
using EVEMon.Common;
using System.Text;

namespace EVEMon
{
    internal class Emailer
    {
        private Emailer()
        {
        }

        private string m_server;
        private ICredentialsByHost m_serverCredentials;
        private bool m_serverRequiresSsl = false;
        private string m_fromAddr;
        private string m_toAddr;
        private string m_subject;
        private string m_body;
        private int m_portNumber;

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
            else
            {
                // Shorter subject also for SMS Format
                return SendMail(settings, "[STC] " + charName + " :: " + skillName + " " + skillLevelString, messageText.ToString());
            }
            //return SendMail(settings, charName + " skill " + skillName + " " + skillLevelString + " complete", messageText.ToString());
        }

        private static bool SendMail(Settings settings, string subject, string body)
        {
            Emailer m = new Emailer();
            m.m_server = settings.EmailServer;
            m.m_fromAddr = settings.EmailFromAddress;
            m.m_toAddr = settings.EmailToAddress;
            m.m_subject = subject;
            m.m_body = body;
            m.m_portNumber = settings.PortNumber;
            if (settings.EmailAuthRequired)
            {
                m.m_serverCredentials = new NetworkCredential(settings.EmailAuthUsername, settings.EmailAuthPassword);
            }
            m.m_serverRequiresSsl = settings.EmailServerRequiresSsl;
            if (settings.PortNumber > 0)
            {
                return m.Send(settings.PortNumber);
            }
            else
            {
                return m.Send();
            }
        }

        private bool Send()
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(m_fromAddr);
                msg.To.Add(m_toAddr);
                msg.Subject = m_subject;
                msg.Body = m_body;
                SmtpClient cli = new SmtpClient(m_server);
                if (m_serverCredentials != null)
                {
                    cli.Credentials = m_serverCredentials;
                }
                cli.EnableSsl = m_serverRequiresSsl;
                cli.Send(msg);
                return true;
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, true);
                return false;
            }
        }

        private bool Send(int PortNumber)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(m_fromAddr);
                msg.To.Add(m_toAddr);
                msg.Subject = m_subject;
                msg.Body = m_body;
                SmtpClient client = new SmtpClient(m_server, m_portNumber);

                if (m_serverCredentials != null)
                {
                    client.Credentials = m_serverCredentials;
                }
                client.EnableSsl = m_serverRequiresSsl;
                client.Send(msg);
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
