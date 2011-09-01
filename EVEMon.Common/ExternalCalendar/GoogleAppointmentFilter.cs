using System;
using System.Collections;
using System.Linq;
using Google.GData.Calendar;
using Google.GData.Extensions;

namespace EVEMon.Common.ExternalCalendar
{
    /// <summary>
    /// Class for handling Google calendar.
    /// </summary>
    public sealed class GoogleAppointmentFilter : AppointmentFilter
    {
        #region Private Variables

        /// <summary>
        /// The maximum of minutes google accepts.
        /// </summary>
        private const int MaxGoogleMinutes = 40320;

        /// <summary>
        /// The type of Google reminder required (SMS, EMail etc).
        /// </summary>
        private readonly ArrayList m_googleReminders = new ArrayList();

        /// <summary>
        /// The Goolge Calendar service.
        /// </summary>
        private readonly CalendarService m_service;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleAppointmentFilter"/> class.
        /// </summary>
        internal GoogleAppointmentFilter()
        {
            m_service = new CalendarService("serviceName");

            UserName = String.Empty;
            Password = String.Empty;
            Uri = NetworkConstants.GoogleCalendarURL;
            PopulateGoogleReminders();
        }

        #endregion


        #region Public Static Properties

        /// <summary>
        /// Gets the google reminder methods.
        /// </summary>
        /// <value>The reminder methods.</value>
        public static IEnumerable ReminderMethods
        {
            get { return Enum.GetValues(typeof(Reminder.ReminderMethod)); }
        }

        #endregion


        #region Internal Properties

        /// <summary>
        /// Gets or sets User Name used for Google logon.
        /// </summary>
        internal string UserName { private get; set; }

        /// <summary>
        /// Gets or sets Password used for Google logon.
        /// </summary>
        internal string Password { private get; set; }

        /// <summary>
        /// Gets or sets URI used for Google calendar.
        /// </summary>
        internal string Uri { private get; set; }

        /// <summary>
        /// Gets or sets Reminder Method.
        /// </summary>
        internal int ReminderMethod { private get; set; }

        #endregion


        #region Public Methods

        /// <summary>
        /// Add a new appointment or Update the appropriate appointment in the calendar.
        /// </summary>
        /// <param name="appointmentExists">if set to <c>true</c> [appointment exists].</param>
        /// <param name="queuePosition">The queue position.</param>
        public override void AddOrUpdateAppointment(bool appointmentExists, int queuePosition)
        {
            Exception googleProblem = null;

            EventEntry appointmentItem = (appointmentExists
                                              ? (EventEntry)AppointmentArray[0]
                                              : new EventEntry());

            if (appointmentItem.Times.Count == 0)
                appointmentItem.Times.Add(new When());

            // Set the title and content of the entry
            appointmentItem.Title.Text = Subject;
            appointmentItem.Times[0].StartTime = StartDate;
            appointmentItem.Times[0].EndTime = EndDate;

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

                // Subtract the reminder time from the appointment time
                DateTime dateTimeAlternateReminder = WorkOutAlternateReminders();
                TimeSpan timeSpan = appointmentItem.Times[0].StartTime.Subtract(dateTimeAlternateReminder);
                Minutes = Math.Abs((timeSpan.Hours * 60) + timeSpan.Minutes);

                SetGoogleReminder(appointmentItem);
            }
            else if (ItemReminder)
                SetGoogleReminder(appointmentItem);
            else
                appointmentItem.Reminder = null;

            if (appointmentExists)
            {
                // Update the appointment
                appointmentItem.Update();
            }
            else
            {
                // Send the request and receive the response
                Uri postUri = new Uri(Uri);
                m_service.Insert(postUri, appointmentItem);
            }

            if (googleProblem != null)
                throw googleProblem;
        }

        /// <summary>
        /// Get the relevant Google appointment.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if an appointment is found, <c>false</c> otherwise.
        /// </returns>
        public override bool GetAppointment()
        {
            if (AppointmentArray.Count < 1)
                return false;

            EventEntry appointmentItem = (EventEntry)AppointmentArray[0];
            StartDate = appointmentItem.Times[0].StartTime;
            EndDate = appointmentItem.Times[0].EndTime;
            Subject = appointmentItem.Title.Text;
            EntryId = appointmentItem.Id.Uri.Content;

            if (appointmentItem.Reminder != null)
            {
                Reminder.ReminderMethod reminderMethod = appointmentItem.Reminder.Method;
                ItemReminder = reminderMethod != Reminder.ReminderMethod.none;

                ReminderMethod = m_googleReminders.IndexOf(reminderMethod);
                Minutes = appointmentItem.Reminder.Minutes;
            }
            else
            {
                ItemReminder = false;
                Minutes = 5;
            }

            return true;
        }

        /// <summary>
        /// Read the Google appointments.
        /// </summary>
        public override void ReadAppointments()
        {
            EventQuery myQuery = new EventQuery(Uri)
                                     {
                                         StartTime = StartDate,
                                         EndTime = EndDate,
                                         Query = Subject
                                     };

            AppointmentArray.Clear();

            EventFeed myResultsFeed = m_service.Query(myQuery);
            foreach (EventEntry eventEntry in myResultsFeed.Entries.OfType<EventEntry>())
            {
                AppointmentArray.Add(eventEntry);
            }
        }

        /// <summary>
        /// Delete the relevant appointment.
        /// </summary>
        /// <param name="appointmentIndex">The appointment index.</param>
        public override void DeleteAppointment(int appointmentIndex)
        {
            ((EventEntry)AppointmentArray[appointmentIndex]).Delete();
        }

        #endregion


        #region Internal Methods

        /// <summary>
        /// Logon to Google.
        /// </summary>
        internal void Logon()
        {
            m_service.setUserCredentials(UserName, Password);
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Sets the goolge reminder.
        /// </summary>
        /// <param name="appointmentItem">The appointment item.</param>
        private void SetGoogleReminder(EventEntry appointmentItem)
        {
            appointmentItem.Reminder = (appointmentItem.Reminder ?? new Reminder());
            appointmentItem.Reminder.Minutes = Math.Min(Minutes, MaxGoogleMinutes);
            appointmentItem.Reminder.Method = (Reminder.ReminderMethod)m_googleReminders[ReminderMethod];
        }

        /// <summary>
        /// Populate the Google reminders types.
        /// </summary>
        private void PopulateGoogleReminders()
        {
            m_googleReminders.Add(Reminder.ReminderMethod.alert);
            m_googleReminders.Add(Reminder.ReminderMethod.all);
            m_googleReminders.Add(Reminder.ReminderMethod.email);
            m_googleReminders.Add(Reminder.ReminderMethod.none);
            m_googleReminders.Add(Reminder.ReminderMethod.sms);
            m_googleReminders.Add(Reminder.ReminderMethod.unspecified);
        }

        #endregion
    }
}