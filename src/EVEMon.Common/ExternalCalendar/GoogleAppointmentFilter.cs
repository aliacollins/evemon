using System;
using System.Collections;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common.Constants;
using Google.GData.Calendar;
using Google.GData.Client;
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
            m_service = new CalendarService(EveMonClient.FileVersionInfo.ProductName);

            UserName = String.Empty;
            Password = String.Empty;
            Uri = new Uri(NetworkConstants.GoogleCalendarURL);
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
        internal Uri Uri { private get; set; }

        /// <summary>
        /// Gets or sets Reminder Method.
        /// </summary>
        internal int ReminderMethod { private get; set; }

        #endregion


        #region Public Methods

        /// <summary>
        /// Add a new appointment or Update the appropriate appointment in the calendar.
        /// </summary>
        /// <param name="appointmentExists">if set to <c>true</c> the appointment exists.</param>
        /// <param name="queuePosition">The queue position.</param>
        /// <param name="lastSkillInQueue">if set to <c>true</c> skill is the last in queue.</param>
        internal override void AddOrUpdateAppointment(bool appointmentExists, int queuePosition, bool lastSkillInQueue)
        {
            EventEntry appointmentItem = appointmentExists
                                             ? (EventEntry)AppointmentArray[0]
                                             : new EventEntry();

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
                m_service.Insert(Uri, appointmentItem);
            }
        }

        /// <summary>
        /// Get the relevant Google appointment.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if an appointment is found, <c>false</c> otherwise.
        /// </returns>
        internal override bool Appointment
        {
            get
            {
                if (AppointmentArray.Count < 1)
                    return false;

                EventEntry appointmentItem = (EventEntry)AppointmentArray[0];
                if (appointmentItem.Times.Count < 1)
                    return false;

                StartDate = appointmentItem.Times[0].StartTime;
                EndDate = appointmentItem.Times[0].EndTime;
                Subject = appointmentItem.Title.Text;
                EntryId = appointmentItem.EventId;

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
        }

        /// <summary>
        /// Read the Google appointments.
        /// </summary>
        internal override void ReadAppointments()
        {
            EventQuery myQuery = new EventQuery(Uri.AbsoluteUri)
                                     {
                                         StartTime = StartDate,
                                         EndTime = EndDate,
                                         Query = Subject
                                     };

            AppointmentArray.Clear();

            try
            {
                EventFeed myResultsFeed = m_service.Query(myQuery);
                foreach (EventEntry eventEntry in myResultsFeed.Entries.OfType<EventEntry>()
                    .Where(entry => entry.Title.Text == Subject))
                {
                    AppointmentArray.Add(eventEntry);
                }
            }
            catch(GDataRequestException ex)
            {
                MessageBox.Show(ex.Message, "Google says:");
            }
        }

        /// <summary>
        /// Delete the relevant appointment.
        /// </summary>
        /// <param name="appointmentIndex">The appointment index.</param>
        internal override void DeleteAppointment(int appointmentIndex)
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
            m_service.RequestFactory.UseSSL = Uri.Scheme == Uri.UriSchemeHttps;
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