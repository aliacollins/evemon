using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EVEMon.Common.SettingsObjects;
using Google.GData.Client;

namespace EVEMon.Common.ExternalCalendar
{
    public static class ExternalCalendar
    {
        /// <summary>
        /// Process the selected character skill queue into the selected calendar.
        /// </summary>
        /// <param name="character">The character.</param>
        public static void UpdateCalendar(CCPCharacter character)
        {
            if (character == null)
                throw new ArgumentNullException("character");

            SkillQueue skillQueue = character.SkillQueue;

            foreach (QueuedSkill queuedSkill in skillQueue)
            {
                bool lastSkillInQueue = queuedSkill == skillQueue.Last();
                int queuePosition = skillQueue.IndexOf(queuedSkill) + 1;

                // Continue if user has selected 'Last Queued Skill Only'
                if (Settings.Calendar.LastQueuedSkillOnly && !lastSkillInQueue)
                    continue;

                try
                {
                    if (Settings.Calendar.Provider == CalendarProvider.Outlook)
                        DoOutlookAppointment(queuedSkill, queuePosition, lastSkillInQueue);

                    if (Settings.Calendar.Provider == CalendarProvider.Google)
                        DoGoogleAppointment(queuedSkill, queuePosition, lastSkillInQueue);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogRethrowException(ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Process the queue into MS Outlook.
        /// </summary>
        /// <param name="queuedSkill">The queued skill.</param>
        /// <param name="queuePosition">The queue position.</param>
        /// <param name="lastSkillInQueue">if set to <c>true</c> skill is the last in queue.</param>
        private static void DoOutlookAppointment(QueuedSkill queuedSkill, int queuePosition, bool lastSkillInQueue)
        {
            try
            {
                // Set the subject to the character name and the skill and level in queue for uniqueness sake
                OutlookAppointmentFilter outlookAppointmentFilter = new OutlookAppointmentFilter
                                                                        {
                                                                            StartDate = DateTime.Now.AddDays(-40),
                                                                            EndDate = DateTime.Now.AddDays(100),
                                                                            Subject = String.Format(
                                                                                CultureConstants.DefaultCulture,
                                                                                "{0} - {1} {2}",
                                                                                queuedSkill.Owner.Name,
                                                                                queuedSkill.SkillName,
                                                                                Skill.GetRomanFromInt(queuedSkill.Level))
                                                                        };

                // Pull the list of appointments, hopefully we should either get 1 or none back
                outlookAppointmentFilter.ReadAppointments();

                // If there is an appointment, get the first one
                bool foundAppointment = false;
                if (outlookAppointmentFilter.ItemCount > 0)
                    foundAppointment = outlookAppointmentFilter.Appointment;

                // Update the appointment we may have pulled or the new one
                // Set the appointment length to 5 minutes, starting at the estimated completion date and time
                // Reminder value was already validated
                // Use the values from the screen as these may differ what the user has set for defaults
                outlookAppointmentFilter.StartDate = queuedSkill.EndTime.ToLocalTime();
                outlookAppointmentFilter.EndDate = queuedSkill.EndTime.ToLocalTime().AddMinutes(5);
                outlookAppointmentFilter.ItemReminder = Settings.Calendar.UseReminding;
                outlookAppointmentFilter.AlternateReminder = Settings.Calendar.UseRemindingRange;
                outlookAppointmentFilter.EarlyReminder = Settings.Calendar.EarlyReminding;
                outlookAppointmentFilter.LateReminder = Settings.Calendar.LateReminding;
                outlookAppointmentFilter.Minutes = Settings.Calendar.RemindingInterval;

                try
                {
                    outlookAppointmentFilter.AddOrUpdateAppointment(foundAppointment, queuePosition, lastSkillInQueue);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogRethrowException(ex);
                    throw;
                }
            }
            catch (COMException ex)
            {
                ExceptionHandler.LogException(ex, true);
                Settings.Calendar.Enabled = false;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogRethrowException(ex);
                throw;
            }
        }

        /// <summary>
        /// Process the queue into Google calendar.
        /// </summary>
        /// <param name="queuedSkill">The queued skill.</param>
        /// <param name="queuePosition">The queue position.</param>
        /// <param name="lastSkillInQueue">if set to <c>true</c> skill is the last in queue.</param>
        private static void DoGoogleAppointment(QueuedSkill queuedSkill, int queuePosition, bool lastSkillInQueue)
        {
            try
            {
                // Set the subject to the character name and the skill and level in queue for uniqueness sakes
                GoogleAppointmentFilter googleAppointmentFilter = new GoogleAppointmentFilter
                                                                      {
                                                                          UserName = Settings.Calendar.GoogleEmail,
                                                                          Password =
                                                                              Util.Decrypt(Settings.Calendar.GooglePassword,
                                                                                           Settings.Calendar.GoogleEmail),
                                                                          Uri = new Uri(Settings.Calendar.GoogleAddress),
                                                                          StartDate = DateTime.Now.AddDays(-40),
                                                                          EndDate = DateTime.Now.AddDays(100),
                                                                          Subject = String.Format(
                                                                              CultureConstants.DefaultCulture,
                                                                              "{0} - {1} {2}",
                                                                              queuedSkill.Owner.Name,
                                                                              queuedSkill.SkillName,
                                                                              Skill.GetRomanFromInt(queuedSkill.Level))
                                                                      };

                // Log on to google
                googleAppointmentFilter.Logon();

                // Pull the list of appointments, hopefully we should either get 1 or none back
                googleAppointmentFilter.ReadAppointments();

                // If there is are appointments, see if any match the subject
                bool foundAppointment = false;
                if (googleAppointmentFilter.ItemCount > 0)
                    foundAppointment = googleAppointmentFilter.Appointment;

                // Update the appointment we may have pulled or the new one
                // Set the appointment length to 5 minutes, starting at the estimated completion date and time
                // Reminder interval was already validated
                // Use the values from the screen as these may differ what the user has set for defaults
                googleAppointmentFilter.StartDate = queuedSkill.EndTime.ToLocalTime();
                googleAppointmentFilter.EndDate = queuedSkill.EndTime.ToLocalTime().AddMinutes(5);
                googleAppointmentFilter.ItemReminder = Settings.Calendar.UseReminding;
                googleAppointmentFilter.AlternateReminder = Settings.Calendar.UseRemindingRange;
                googleAppointmentFilter.EarlyReminder = Settings.Calendar.EarlyReminding;
                googleAppointmentFilter.LateReminder = Settings.Calendar.LateReminding;
                googleAppointmentFilter.Minutes = Settings.Calendar.RemindingInterval;
                googleAppointmentFilter.ReminderMethod = (int)Settings.Calendar.GoogleReminder;

                googleAppointmentFilter.AddOrUpdateAppointment(foundAppointment, queuePosition, lastSkillInQueue);
            }
            catch (GDataRequestException ex)
            {
                MessageBox.Show(ex.Message, "Google says:");
            }
            catch (InvalidCredentialsException ex)
            {
                MessageBox.Show(ex.Message, "Google says:");
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogRethrowException(ex);
                throw;
            }
        }
    }
}