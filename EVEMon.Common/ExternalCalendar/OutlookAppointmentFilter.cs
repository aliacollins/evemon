using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Outlook;

namespace EVEMon.Common.ExternalCalendar
{
    /// <summary>
    /// Appointment filter for MS Outlook.
    /// </summary>
    public sealed class OutlookAppointmentFilter : AppointmentFilter
    {
        private static Application s_outlookApplication;
        private static MAPIFolder s_mapiFolder;


        #region Properties

        /// <summary>
        /// Gets the Outlook application.
        /// </summary>
        /// <value>The outlook application.</value>
        internal static Application OutlookApplication
        {
            get
            {
                try
                {
                    if (s_outlookApplication == null)
                    {
                        s_outlookApplication = new Application();
                        ((ApplicationEvents_11_Event)s_outlookApplication).Quit += Outlook_Quit;
                    }
                }
                catch (COMException ex)
                {
                    ExceptionHandler.LogException(ex, true);
                }
                return s_outlookApplication;
            }
        }

        #endregion


        #region Event Hanlders

        /// <summary>
        /// Occurs when Outlook quits.
        /// </summary>
        private static void Outlook_Quit()
        {
            ((ApplicationEvents_11_Event)s_outlookApplication).Quit -= Outlook_Quit;
            s_outlookApplication = null;
            s_mapiFolder = null;
        }


        #endregion


        #region Internal Methods

        /// <summary>
        /// Add a new appointment or Update the appropriate appointment in the calendar.
        /// </summary>
        /// <param name="appointmentExists">if set to <c>true</c> the appointment exists.</param>
        /// <param name="queuePosition">The queue position.</param>
        /// <param name="lastSkillInQueue">if set to <c>true</c> skill is the last in queue.</param>
        internal override void AddOrUpdateAppointment(bool appointmentExists, int queuePosition, bool lastSkillInQueue)
        {
            AppointmentItem appointmentItem = appointmentExists
                                                  ? (AppointmentItem)AppointmentArray[0]
                                                  : (AppointmentItem)s_mapiFolder.Items.Add(OlItemType.olAppointmentItem);

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
        internal override bool Appointment
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
        internal override void ReadAppointments()
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
        internal override void DeleteAppointment(int appointmentIndex)
        {
            ((AppointmentItem)AppointmentArray[appointmentIndex]).Delete();
        }

        /// <summary>
        /// Gets true if the Outlook calendar exist.
        /// </summary>
        /// <param name="useDefaultCalendar">if set to <c>true</c> [use default calendar].</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        internal static bool OutlookCalendarExist(bool useDefaultCalendar, string path = null)
        {
            s_mapiFolder = null;
            return GetMapiFolder(useDefaultCalendar, OutlookApplication.Session.Folders, path);
        }

        /// <summary>
        /// Gets the mapi folder.
        /// </summary>
        /// <param name="useDefaultCalendar">if set to <c>true</c> [use default calendar].</param>
        /// <param name="path">The path.</param>
        /// <param name="folders">The folders.</param>
        /// <returns></returns>
        private static bool GetMapiFolder(bool useDefaultCalendar, IEnumerable folders, string path = null)
        {
            if (useDefaultCalendar)
            {
                s_mapiFolder = OutlookApplication.Session.GetDefaultFolder(OlDefaultFolders.olFolderCalendar);
                return s_mapiFolder != null;
            }

            if (String.IsNullOrWhiteSpace(path))
                path = Settings.Calendar.OutlookCustomCalendarPath;

            if (!path.StartsWith(@"\\", StringComparison.Ordinal))
                return s_mapiFolder != null;

            string pathRoot = GetFolderPathRoot(path);

            foreach (Folder folder in folders.Cast<Folder>().TakeWhile(
                folder => s_mapiFolder == null).Select(
                    folder => new { folder, folderRoot = GetFolderPathRoot(folder.FolderPath) }).Where(
                        folder => folder.folderRoot == pathRoot).Select(folder => folder.folder))
            {
                if (folder.DefaultItemType == OlItemType.olAppointmentItem && folder.FolderPath == path)
                {
                    s_mapiFolder = folder;
                    break;
                }

                if (folder.Folders.Cast<Folder>().Any())
                    GetMapiFolder(false, folder.Folders, path);
            }

            return s_mapiFolder != null && s_mapiFolder.FolderPath == path;
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
            // Use a Jet Query to filter the details we need initially between the two specified dates
            string dateFilter = String.Format(CultureConstants.DefaultCulture, "[Start] >= '{0:g}' and [End] <= '{1:g}'",
                                              StartDate, EndDate);
            Items calendarItems = s_mapiFolder.Items.Restrict(dateFilter);
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

        /// <summary>
        /// Gets the folder path root.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        /// <returns></returns>
        private static string GetFolderPathRoot(string folderPath)
        {
            // Strip header directory seperator characters
            folderPath = folderPath.Remove(0, 2);

            // Find the index of a directory seperator character
            int index = folderPath.IndexOf(Path.DirectorySeparatorChar, 0);

            // Reconstruct the root path according to the index found
            return String.Format(@"\\{0}", index > 0 ? folderPath.Substring(0, index) : folderPath);
        }

        #endregion

    }
}