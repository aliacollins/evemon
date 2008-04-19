using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.ExternalCalendar
{
    public partial class ExternalCalendar : Form
    {
        // All the required info we need to set the appointment.
        private CharacterInfo m_grandCharacterInfo;
        private Settings m_tempSettings;
        private string m_skillTrainingName;
        private DateTime m_estimatedCompletion;

        public CharacterInfo GrandCharacterInfo
        {
            set { m_grandCharacterInfo = value; }
        }
        public Settings ApplicationSettings
        {
            set { m_tempSettings = value; }
        }
        public string SkillTrainingName
        {
            set { m_skillTrainingName = value; }
        }
        public DateTime EstimatedCompletion
        {
            set { m_estimatedCompletion = value; }
        }

        public ExternalCalendar()
        {
            InitializeComponent();
        }

        public void DoAppointment()
        {
            // Initialise the fields.
            if (m_tempSettings.CalendarOption == 0)
                rbMSOutlook.Checked = true;
            else
                rbGoogle.Checked = true;
            tbGoogleEmail.Text = m_tempSettings.GoogleEmail;
            tbGooglePassword.Text = m_tempSettings.GooglePassword;
            tbGoogleURI.Text = m_tempSettings.GoogleURI;
            cbGoogleReminder.SelectedIndex = m_tempSettings.GoogleReminder;
            cbSetReminder.Checked = m_tempSettings.SetReminder;
            tbReminder.Text = m_tempSettings.ReminderMinutes.ToString();
            cbUseAlterateReminder.Checked = m_tempSettings.UseAlternateReminder;
            dtpEarlyReminder.Value = m_tempSettings.EarlyReminder;
            dtpLateReminder.Value = m_tempSettings.LateReminder;
            this.ShowDialog();
        }

        private void rbMSOutlook_Click(object sender, EventArgs e)
        {
            gbGoogle.Enabled = rbGoogle.Checked;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            AssignTemporarySettings();

            if (rbMSOutlook.Checked)
                DoOutlookAppointment();
            else
            {
                try
                {
                    DoGoogleAppointment();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            this.Close();
        }

        private void AssignTemporarySettings()
        {
            // Save temporary settings for this appointment.
            if (rbMSOutlook.Checked)
                m_tempSettings.CalendarOption = 0;  // MS Outlook
            else
                m_tempSettings.CalendarOption = 1;  // Google Calendar
            m_tempSettings.GoogleEmail = tbGoogleEmail.Text;
            m_tempSettings.GooglePassword = tbGooglePassword.Text;
            m_tempSettings.GoogleURI = tbGoogleURI.Text;
            m_tempSettings.GoogleReminder = cbGoogleReminder.SelectedIndex;
            m_tempSettings.SetReminder = cbSetReminder.Checked;
            try
            {
                m_tempSettings.ReminderMinutes = Int32.Parse(tbReminder.Text);
            }
            catch (Exception)
            {
                m_tempSettings.ReminderMinutes = 5;
                tbReminder.Text = "5";
            }
            m_tempSettings.UseAlternateReminder = cbUseAlterateReminder.Checked;
            m_tempSettings.EarlyReminder = dtpEarlyReminder.Value;
            m_tempSettings.LateReminder = dtpLateReminder.Value;
        }

        private void DoOutlookAppointment()
        {
            try
            {
                SNCalendar.OutlookAppointmentFilter outlookAppointmentFilter = new SNCalendar.OutlookAppointmentFilter();

                // Check whether this is a new appointment or an update.
                outlookAppointmentFilter.StartDate = DateTime.Now.AddDays(-40);
                outlookAppointmentFilter.EndDate = DateTime.Now.AddDays(100);
                // Set the subject to the character name and the skill being trained for uniqueness sakes.
                outlookAppointmentFilter.Subject = m_grandCharacterInfo.Name + " - " + m_skillTrainingName;
                // Pull the list of appointments. Hopefully we should either get 1 or none back.
                outlookAppointmentFilter.ReadAppointments();

                // If there is an appointment, get the first one.
                if (outlookAppointmentFilter.ItemCount > 0)
                    outlookAppointmentFilter.GetAppointment(0);

                // Update the appointment we may have pulled or the new one.
                // Set the appoinment length to 5 minutes, starting at the estimated completion date and time.
                outlookAppointmentFilter.StartDate = m_estimatedCompletion;
                outlookAppointmentFilter.EndDate = m_estimatedCompletion.AddMinutes(5);
                outlookAppointmentFilter.ItemReminder = m_tempSettings.SetReminder;
                outlookAppointmentFilter.AlternateReminder = m_tempSettings.UseAlternateReminder;
                outlookAppointmentFilter.EarlyReminder = m_tempSettings.EarlyReminder;
                outlookAppointmentFilter.LateReminder = m_tempSettings.LateReminder;
                outlookAppointmentFilter.Minutes = m_tempSettings.ReminderMinutes;
                if (outlookAppointmentFilter.ItemCount > 0)
                    outlookAppointmentFilter.UpdateAppointment(0);
                else
                    outlookAppointmentFilter.AddAppointment();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0} {1}{1} {2}", "There was a problem accessing the Outlook Libraries, is Outlook installed on this machine?", Environment.NewLine, ex.Message), "Problem intergrating to Outlook");
                return;
            }
        }

        private void DoGoogleAppointment()
        {
            try
            {
                SNCalendar.GoogleAppointmentFilter googleAppointmentFilter = new SNCalendar.GoogleAppointmentFilter("serviceName");

                googleAppointmentFilter.UserName = m_tempSettings.GoogleEmail;
                googleAppointmentFilter.Password = m_tempSettings.GooglePassword;
                googleAppointmentFilter.URI = m_tempSettings.GoogleURI;
                googleAppointmentFilter.Logon();

                // Check whether this is a new appointment or an update.
                googleAppointmentFilter.StartDate = DateTime.Now.AddDays(-40);
                googleAppointmentFilter.EndDate = DateTime.Now.AddDays(100);
                // Set the subject to the chanracter name and the skill being trained for uniqueness sakes.
                googleAppointmentFilter.Subject = m_grandCharacterInfo.Name + " - " + m_skillTrainingName;
                // Pull the list of appointments. Hopefully we should either get 1 or none back.
                googleAppointmentFilter.ReadAppointments();

                // If there is are appointments, see if any match the subject.
                bool foundAppointment = false;
                if (googleAppointmentFilter.ItemCount > 0)
                    if (googleAppointmentFilter.GetAppointment(-1))
                        foundAppointment = true;

                // Update the appointment we may have pulled or the new one.
                // Set the appointment length to 5 minutes, starting at the estimated completion date and time.
                googleAppointmentFilter.StartDate = m_estimatedCompletion;
                googleAppointmentFilter.EndDate = m_estimatedCompletion.AddMinutes(5);
                googleAppointmentFilter.ItemReminder = m_tempSettings.SetReminder;
                googleAppointmentFilter.AlternateReminder = m_tempSettings.UseAlternateReminder;
                googleAppointmentFilter.EarlyReminder = m_tempSettings.EarlyReminder;
                googleAppointmentFilter.LateReminder = m_tempSettings.LateReminder;
                googleAppointmentFilter.Minutes = m_tempSettings.ReminderMinutes;
                googleAppointmentFilter.ReminderMethod = m_tempSettings.GoogleReminder;
                if (foundAppointment)
                {
                    try
                    {
                        googleAppointmentFilter.UpdateAppointment(0);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    try
                    {
                        googleAppointmentFilter.AddAppointment();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
