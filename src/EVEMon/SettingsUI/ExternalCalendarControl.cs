using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Constants;
using EVEMon.Common.ExternalCalendar;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.SettingsUI
{
    public partial class ExternalCalendarControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalCalendarControl"/> class.
        /// </summary>
        internal ExternalCalendarControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the external calendar.
        /// </summary>
        /// <param name="settings">The settings.</param>
        internal void SetExternalCalendar(SerializableSettings settings)
        {
            // Google calendar reminder method
            InitilizeGoogleCalendarReminderDropDown();

            rbMSOutlook.Checked = settings.Calendar.Enabled && settings.Calendar.Provider == CalendarProvider.Outlook &&
                                  ExternalCalendar.OutlookInstalled;
            rbGoogle.Checked = !rbMSOutlook.Checked;

            rbDefaultCalendar.Checked = settings.Calendar.UseOutlookDefaultCalendar;
            rbCustomCalendar.Checked = !rbDefaultCalendar.Checked;
            tbCalendarPath.Text = settings.Calendar.OutlookCustomCalendarPath;

            tbGoogleEmail.Text = settings.Calendar.GoogleEmail;
            tbGooglePassword.Text = Util.Decrypt(settings.Calendar.GooglePassword, settings.Calendar.GoogleEmail);
            tbGoogleURI.Text = settings.Calendar.GoogleAddress;
            cbGoogleReminder.SelectedIndex = (int)settings.Calendar.GoogleReminder;
            cbSetReminder.Checked = settings.Calendar.UseReminding;
            tbReminder.Text = settings.Calendar.RemindingInterval.ToString(CultureConstants.DefaultCulture);
            cbUseAlterateReminder.Checked = settings.Calendar.UseRemindingRange;
            dtpEarlyReminder.Value = settings.Calendar.EarlyReminding;
            dtpLateReminder.Value = settings.Calendar.LateReminding;
            cbLastQueuedSkillOnly.Checked = settings.Calendar.LastQueuedSkillOnly;
            
            UpdateControlsVisibility();
        }

        /// <summary>
        /// Applies the external calendar settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        internal void ApplyExternalCalendarSettings(SerializableSettings settings)
        {
            settings.Calendar.Provider = (rbMSOutlook.Checked ? CalendarProvider.Outlook : CalendarProvider.Google);

            settings.Calendar.UseOutlookDefaultCalendar = rbDefaultCalendar.Checked;
            settings.Calendar.OutlookCustomCalendarPath = tbCalendarPath.Text.Trim();

            settings.Calendar.GoogleEmail = tbGoogleEmail.Text.Trim();
            settings.Calendar.GooglePassword = Util.Encrypt(tbGooglePassword.Text.Trim(), tbGoogleEmail.Text.Trim());
            settings.Calendar.GoogleAddress = tbGoogleURI.Text.Trim();
            settings.Calendar.GoogleReminder = cbGoogleReminder.SelectedIndex != -1
                ? (GoogleCalendarReminder)cbGoogleReminder.SelectedIndex
                : GoogleCalendarReminder.None;

            settings.Calendar.UseReminding = cbSetReminder.Checked;
            settings.Calendar.RemindingInterval = Int32.Parse(tbReminder.Text, CultureConstants.DefaultCulture);
            settings.Calendar.UseRemindingRange = cbUseAlterateReminder.Checked;
            settings.Calendar.EarlyReminding = dtpEarlyReminder.Value;
            settings.Calendar.LateReminding = dtpLateReminder.Value;
            settings.Calendar.LastQueuedSkillOnly = cbLastQueuedSkillOnly.Checked;
        }

        /// <summary>
        /// Updates the controls visibility.
        /// </summary>
        private void UpdateControlsVisibility()
        {
            rbMSOutlook.Enabled = Enabled && ExternalCalendar.OutlookInstalled;
            gbMSOutlook.Visible = rbMSOutlook.Checked;
            gbGoogle.Visible = rbGoogle.Checked;
            calendarPathLabel.Enabled = tbCalendarPath.Enabled = rbCustomCalendar.Checked;
            calendarPathExampleLabel.Visible = rbCustomCalendar.Checked;
            cbSetReminder.Enabled = lblMinutes.Enabled = !cbUseAlterateReminder.Checked;
            tbReminder.Enabled = cbSetReminder.Checked;
            cbUseAlterateReminder.Enabled = lblEarlyReminder.Enabled = lblLateReminder.Enabled = !cbSetReminder.Checked;
            dtpEarlyReminder.Enabled = cbUseAlterateReminder.Checked;
            dtpLateReminder.Enabled = cbUseAlterateReminder.Checked;

            if (rbCustomCalendar.Checked)
                tbCalendarPath.Focus();
        }

        /// <summary>
        /// Initilizes the google calendar reminder drop down.
        /// </summary>
        private void InitilizeGoogleCalendarReminderDropDown()
        {
            cbGoogleReminder.Items.Clear();
            foreach (string text in GoogleAppointmentFilter.ReminderMethods.Cast<Enum>().Select(item => item.ToString()))
            {
                cbGoogleReminder.Items.Add(char.ToUpper(text[0], CultureConstants.DefaultCulture) + text.Substring(1));
            }
        }

        /// <summary>
        /// This handler occurs when the radio buttons check changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMustEnableOrDisable(object sender, EventArgs e)
        {
            UpdateControlsVisibility();
        }

        /// <summary>
        /// Outlook custom calendar path validation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void tbCalendarPath_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = Enabled && rbMSOutlook.Checked && rbCustomCalendar.Checked &&
                       (tbCalendarPath.Text.Any(x => Path.GetInvalidPathChars().Contains(x)) ||
                        String.IsNullOrWhiteSpace(tbCalendarPath.Text.Trim()) ||
                        !ExternalCalendar.OutlookCalendarExist(rbDefaultCalendar.Checked, tbCalendarPath.Text));

            if (e.Cancel)
            {
                MessageBox.Show(@"A calendar at that path could not be found.", @"MS Outlook",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Reminder value validation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void tbReminder_Validating(object sender, CancelEventArgs e)
        {
            int value;
            e.Cancel = !Int32.TryParse(tbReminder.Text, out value) && value > 0;

            if (e.Cancel)
            {
                MessageBox.Show(@"The reminder interval must be a strictly positive integer.", @"Reminder interval",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExternalCalendarControl_EnabledChanged(object sender, EventArgs e)
        {
            UpdateControlsVisibility();
        }
    }
}
