using System;
using System.Collections;

namespace EVEMon.Common.ExternalCalendar
{
    /// <summary>
    /// Common Appointment Class used for both instances of the calendar.
    /// </summary>
    public abstract class AppointmentFilter
    {
        private readonly ArrayList m_arrayList;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AppointmentFilter"/> class.
        /// </summary>
        protected AppointmentFilter()
        {
            StartDate = DateTime.Now;
            EndDate = DateTime.Now.AddDays(1);
            Subject = String.Empty;
            ItemReminder = true;
            Minutes = 5;
            EntryId = String.Empty;
            AlternateReminder = false;
            EarlyReminder = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
            LateReminder = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 0, 0);
            m_arrayList = new ArrayList();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets Start Date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets End Date.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets Subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the appointment requires a reminder.
        /// </summary>
        public bool ItemReminder { get; set; }

        /// <summary>
        /// Gets or sets Minutes.
        /// </summary>
        public int Minutes { get; set; }

        /// <summary>
        /// Gets or sets Entry Id.
        /// </summary>
        public string EntryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the appointment uses the Alternate Reminder.
        /// </summary>
        public bool AlternateReminder { get; set; }

        /// <summary>
        /// Gets or sets Early Reminder for alternate reminders.
        /// </summary>
        public DateTime EarlyReminder { get; set; }

        /// <summary>
        /// Gets or sets LateReminder for alternate reminders.
        /// </summary>
        public DateTime LateReminder { get; set; }

        /// <summary>
        /// Gets the number of appointments.
        /// </summary>
        public int ItemCount
        {
            get { return AppointmentArray.Count; }
        }

        /// <summary>
        /// Gets or sets Appointment Array.
        /// </summary>
        protected ArrayList AppointmentArray
        {
            get { return m_arrayList; }
        }

        #endregion


        #region Abstract Methods

        /// <summary>
        /// Method to search for any existing appointments for this skill.
        /// </summary>
        internal abstract void ReadAppointments();

        /// <summary>
        /// Method to get the relevant appointment item and populate the details.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if an appointment is found, <c>false</c> otherwise.
        /// </returns>
        internal abstract bool Appointment { get; }

        /// <summary>
        /// Add a new appointment or Update the appropriate appointment in the calendar.
        /// </summary>
        /// <param name="appointmentExists">if set to <c>true</c> the appointment exists.</param>
        /// <param name="queuePosition">The queue position.</param>
        /// <param name="lastSkillInQueue">if set to <c>true</c> skill is the last in queue.</param>
        internal abstract void AddOrUpdateAppointment(bool appointmentExists, int queuePosition, bool lastSkillInQueue);

        /// <summary>
        /// Delete the appropriate appointment.
        /// </summary>
        /// <param name="appointmentIndex">The index of the appointment.</param>
        internal abstract void DeleteAppointment(int appointmentIndex);

        #endregion


        #region Public Methods

        /// <summary>
        /// Figure out whether the appointment falls within the times set that the users will
        /// be away from the PC and return the times they have specified.
        /// </summary>
        /// <returns>
        /// The date and time of the relevant reminder.
        /// </returns>
        protected DateTime WorkOutAlternateReminders()
        {
            // See whether the appointment falls within the middle of the two, it it does, set the early reminder
            if ((StartDate >= EarlyReminder) && (StartDate <= LateReminder))
                return EarlyReminder;

            // If the appointment falls outside the two reminders: if the appointment is before the early reminder, set 
            // the late reminder to the day before....
            if (StartDate < EarlyReminder)
            {
                DateTime lateRem = LateReminder;
                lateRem = lateRem.AddDays(-1);
                return lateRem;
            }

            return LateReminder;
        }

        #endregion
    }
}