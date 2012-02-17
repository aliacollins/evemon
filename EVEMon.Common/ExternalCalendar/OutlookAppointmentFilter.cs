using System;
using System.Collections;
using Microsoft.Office.Interop.Outlook;

namespace EVEMon.Common.ExternalCalendar
{
    /// <summary>
    /// Appointment filter for MS Outlook.
    /// </summary>
    public sealed class OutlookAppointmentFilter : AppointmentFilter
    {
        #region Private Internals

        /// <summary>
        /// Outlook Application.
        /// </summary>
        private static Application s_outlookApplication;

        #endregion


        #region Private Properties

        /// <summary>
        /// Gets the Outlook application.
        /// </summary>
        /// <value>The outlook application.</value>
        private static Application OutlookApplication
        {
            get { return s_outlookApplication ?? (s_outlookApplication = new Application()); }
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Add a new appointment or Update the appropriate appointment in the calendar.
        /// </summary>
        /// <param name="appointmentExists">if set to <c>true</c> the appointment exists.</param>
        /// <param name="queuePosition">The queue position.</param>
        /// <param name="lastSkillInQueue">if set to <c>true</c> skill is the last in queue.</param>
        public override void AddOrUpdateAppointment(bool appointmentExists, int queuePosition, bool lastSkillInQueue)
        {
            AppointmentItem appointmentItem = (appointmentExists
                                                   ? (AppointmentItem)AppointmentArray[0]
                                                   : (AppointmentItem)
                                                     OutlookApplication.CreateItem(OlItemType.olAppointmentItem));

            appointmentItem.Subject = Subject;
            appointmentItem.Start = StartDate;
            appointmentItem.End = EndDate;

            string queuePositionText = lastSkillInQueue ? "End Of Queue" : queuePosition.ToString(CultureConstants.DefaultCulture);

            appointmentItem.Body = appointmentExists
                                        ? String.Format(CultureConstants.DefaultCulture,
                                                        "{0} {3}Updated: {1} Queue Position: {2}",
                                                        appointmentItem.Body, DateTime.Now,
                                                        queuePositionText,
                                                        Environment.NewLine)
                                        : String.Format(CultureConstants.DefaultCulture,
                                                        "Added: {0} Queue Position: {1}",
                                                        DateTime.Now,
                                                        queuePositionText);

            appointmentItem.ReminderSet = ItemReminder || AlternateReminder;
            appointmentItem.BusyStatus = OlBusyStatus.olBusy;
            appointmentItem.AllDayEvent = false;
            appointmentItem.Location = String.Empty;

            if (AlternateReminder)
            {
                EarlyReminder = new DateTime(StartDate.Year,
                                             StartDate.Month,
                                             StartDate.Day,
                                             EarlyReminder.Hour,
                                             EarlyReminder.Minute,
                                             EarlyReminder.Second);

                LateReminder = new DateTime(StartDate.Year,
                                            StartDate.Month,
                                            StartDate.Day,
                                            LateReminder.Hour,
                                            LateReminder.Minute,
                                            LateReminder.Second);

                DateTime dateTimeAlternateReminder = WorkOutAlternateReminders();

                // Subtract the reminder time from the appointment time
                TimeSpan timeSpan = appointmentItem.Start.Subtract(dateTimeAlternateReminder);
                appointmentItem.ReminderMinutesBeforeStart = Math.Abs((timeSpan.Hours * 60) + timeSpan.Minutes);
                Minutes = appointmentItem.ReminderMinutesBeforeStart;
            }

            appointmentItem.ReminderMinutesBeforeStart = (appointmentItem.ReminderSet ? Minutes : 0);
            appointmentItem.Save();
        }

        /// <summary>
        /// Get the relevant appointment item and populate the details.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if an appointment is found, <c>false</c> otherwise.
        /// </returns>
        public override bool Appointment
        {
            get
            {
                if (AppointmentArray.Count < 1)
                    return false;

                AppointmentItem appointmentItem = (AppointmentItem)AppointmentArray[0];
                StartDate = appointmentItem.Start;
                EndDate = appointmentItem.End;
                Subject = appointmentItem.Subject;
                ItemReminder = appointmentItem.ReminderSet;
                Minutes = appointmentItem.ReminderMinutesBeforeStart;
                EntryId = appointmentItem.EntryID;
                return true;
            }
        }

        /// <summary>
        /// Pull all the appointments and populate the appointment array.
        /// </summary>
        public override void ReadAppointments()
        {
            // Appointment Filter class that will handle any data attached
            // to the appointment with which we are currently dealing
            AppointmentArray.Clear();
            AppointmentArray.AddRange(RecurringItems());
        }

        /// <summary>
        /// Delete the specified appointment.
        /// </summary>
        /// <param name="appointmentIndex">The appointment index.</param>
        public override void DeleteAppointment(int appointmentIndex)
        {
            ((AppointmentItem)AppointmentArray[appointmentIndex]).Delete();
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Get appointments matching the subject.
        /// </summary>
        /// <returns>
        /// Array of relevant appointments or empty array.
        /// </returns>
        private ArrayList RecurringItems()
        {
            // Filter all the objects we are looking for
            MAPIFolder folder = OutlookApplication.Session.GetDefaultFolder(OlDefaultFolders.olFolderCalendar);

            // Use a Jet Query to filter the details we need initially between the two specified dates
            string dateFilter = String.Format(
                CultureConstants.DefaultCulture,
                "[Start] >= '{0:g}' and [End] <= '{1:g}'",
                StartDate,
                EndDate);
            Items calendarItems = folder.Items.Restrict(dateFilter);
            calendarItems.Sort("[Start]", Type.Missing);
            calendarItems.IncludeRecurrences = true;

            // Must use 'like' comparison for Find/FindNext
            string subjectFilter = (!String.IsNullOrEmpty(Subject)
                                        ? String.Format(CultureConstants.InvariantCulture,
                                                        "@SQL=\"urn:schemas:httpmail:subject\" like '%{0}%'", Subject)
                                        : "@SQL=\"urn:schemas:httpmail:subject\" <> '!@#'");

            // Use Find and FindNext methods to get all the items
            ArrayList resultArray = new ArrayList();
            AppointmentItem appointmentItem = calendarItems.Find(subjectFilter) as AppointmentItem;
            while (appointmentItem != null)
            {
                resultArray.Add(appointmentItem);

                // Find the next appointment
                appointmentItem = calendarItems.FindNext() as AppointmentItem;
            }

            return resultArray;
        }

        #endregion
    }
}