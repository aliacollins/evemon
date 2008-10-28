using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using EVEMon.Common;
using EVEMon.Common.Net;
using EVEMon.WindowRelocator;
using Microsoft.Win32;
using System.IO;
using System.Collections.Generic;

namespace EVEMon
{
    public partial class SettingsForm : EVEMonForm
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        public SettingsForm(Settings s)
            : this()
        {
            m_settings = s;
        }

        private Settings m_settings;

        // APIDelay strings are     
        //    1 Minute 5 Minutes 10 Minutes 15 Minutes 30 Minutes 1 Hour 2 Hours 3 Hours 6 Hours
        private int[] m_apiUpdateLookup = {60,5*60,10*60,15*60,30*60,60*60,2*60*60,3*60*60,6*60*60};

        private string[] m_trayPopupStyle = { "Popup Form", "Windows Tooltip" };

        private TrayPopupConfig m_trayPopupConfig;

        private String m_tooltipString;

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ApplyToSettings(Settings s)
        {
            // Look and feel options
            s.SkillPlannerHighlightPrerequisites = cbHighlightPrerequisites.Checked;
            s.SkillPlannerHighlightPlannedSkills = cbHighlightPlannedSkills.Checked;
            s.SkillPlannerHighlightConflicts = cbHighlightConflicts.Checked;
            s.SkillPlannerHighlightPartialSkills = cbHighlightPartialSkills.Checked;

            if (rbSystemTrayOptionsNever.Checked)
            {
                s.SystemTrayOptions = SystemTrayDisplayOptions.Never;
            }
            else if (rbSystemTrayOptionsMinimized.Checked)
            {
                s.SystemTrayOptions = SystemTrayDisplayOptions.Minimized;
            }
            else if (rbSystemTrayOptionsAlways.Checked)
            {
                s.SystemTrayOptions = SystemTrayDisplayOptions.Always;
            }

            s.CloseToTray = cbCloseToTray.Checked;
            s.TitleToTime = cbTitleToTime.Checked;
            s.TitleToTimeLayout = cbWindowsTitleList.SelectedIndex + 1;
            s.TitleToTimeSkill = cbSkillInTitle.Checked;
            s.WorksafeMode = cbWorksafeMode.Checked;
            s.RelocateEveWindow = cbRelocateEveWindow.Checked;
            s.RelocateTargetScreen = cbScreenList.SelectedIndex;
            s.SkillIconGroup = cbSkillIconSet.SelectedIndex + 1;
            s.EnableBalloonTips = cbShowBalloonTips.Checked;
            s.NotificationOffset = tbNotificationOffset.Value;
            lblNotificationOffset.Text = s.NotificationOffset.ToString() + " sec";
            s.PlaySoundOnSkillComplete = cbPlaySoundOnSkillComplete.Checked;
            s.EnableSkillCompleteDialog = cbShowCompletedSkillsDialog.Checked;
            s.UseLogitechG15Display = cbUseLogitechG15Display.Checked;
            s.G15ACycle = cbG15ACycle.Checked;
            s.G15ACycleint = (int)ACycleInterval.Value;
            s.G15ShowTime = cbG15ShowTime.Checked;
            if (G15Handler.LCD != null)
            {
                G15Handler.LCD.cycle = cbG15ACycle.Checked;
                G15Handler.LCD.cycleint = (int)ACycleInterval.Value;
                G15Handler.LCD.showtime = cbG15ShowTime.Checked;
            }
// 947 - Start
            s.ShowAllPublicSkills = cbShowAllPublicSkills.Checked;
            s.ShowNonPublicSkills = cbShowNonPublicSkills.Checked;
// 947 - End

            // Email Options
            s.EnableEmailAlert = cbSendEmail.Checked;
            s.EmailServer = tbMailServer.Text;
            try
            {
                s.PortNumber = Convert.ToInt16(tbPortNumber.Text);
            }
            catch (FormatException e)
            {
                ExceptionHandler.LogException(e, true);
                tbPortNumber.Text = "25";
                s.PortNumber = 25;
            }
            s.EmailServerRequiresSsl = cbEmailServerRequireSsl.Checked;
            s.EmailUseShortFormat = cbEmailUseShortFormat.Checked;
            s.EmailAuthRequired = cbEmailAuthRequired.Checked;
            s.EmailAuthUsername = tbEmailUsername.Text;
            s.EmailAuthPassword = tbEmailPassword.Text;
            s.EmailFromAddress = tbFromAddress.Text;
            s.EmailToAddress = tbToAddress.Text;

            //IGB Options
            s.RunIGBServer = cbRunIGBServer.Checked;
            s.IGBServerPublic = cbIGBPublic.Checked;
            s.IGBServerPort = Int32.Parse(tb_IgbPort.Text);

            // Tray Icon Popup
            s.TrayPopupConfig = m_trayPopupConfig;
            s.TrayPopupStyle = (TrayPopupStyles)cbTrayPopupStyle.SelectedIndex;
            s.TooltipString = m_tooltipString;

            s.UseCustomProxySettings = rbCustomProxy.Checked;
            s.DisableRequestsOnAuthenticationFailure = cbDisableOnAuthFailure.Checked;
            ProxySetting httpSetting = ((ProxySetting)btnProxyHttpAuth.Tag).Clone();
            httpSetting.Host = tbProxyHttpHost.Text;
            try
            {
                httpSetting.Port = Convert.ToInt32(tbProxyHttpPort.Text);
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, true);
                httpSetting.Port = 0;
            }
            s.HttpProxy = httpSetting;
            s.CustomTQAddress = tbTQServerAddress.Text;
            s.CustomTQPort = tbTQServerPort.Text;
            s.UseCustomTQCheckSettings = cbCustomTQSettings.Checked;
            s.ShowTQBalloon = cbShowTQBalloon.Checked;

            s.CheckTranquilityStatus = cbCheckTranquilityStatus.Checked;
            s.StatusUpdateInterval = (int)numericStatusInterval.Value;
            s.ConnectivityURL = tbConnectivityURL.Text;

            s.DisableXMLAutoUpdate = cbAutomaticEOSkillUpdate.Checked;
            s.APIUpdateDelay = m_apiUpdateLookup[cmbAPIUpdateDelay.SelectedIndex];

            s.KeepCharacterPlans = cbKeepCharacterPlans.Checked;

            s.DisableEVEMonVersionCheck = cbAutomaticallySearchForNewVersions.Checked;
            s.CheckTimeOnStartup = cbCheckTimeOnStartup.Checked;

            s.EnableSkillCompleteDialog = cbShowCompletedSkillsDialog.Checked;

            // Save Calendar Colors
            s.CalendarBlockingColor = panelColorBlocking.BackColor;
            s.CalendarRecurring1 = panelColorRecurring1.BackColor;
            s.CalendarRecurring2 = panelColorRecurring2.BackColor;
            s.CalendarSingle1 = panelColorSingle1.BackColor;
            s.CalendarSingle2 = panelColorSingle2.BackColor;
            s.CalendarTextColor = panelColorText.BackColor;

// #735 - Start
            // Save External Calendar Settings
            s.UseExternalCalendar = cbUseExternalCalendar.Checked;
            if (rbMSOutlook.Checked)
                s.CalendarOption = 0;  // MS Outlook
            else
                s.CalendarOption = 1;  // Google Calendar
            s.GoogleEmail = tbGoogleEmail.Text;
            s.GooglePassword = tbGooglePassword.Text;
            s.GoogleURI = tbGoogleURI.Text;
            s.GoogleReminder = cbGoogleReminder.SelectedIndex;
            s.SetReminder = cbSetReminder.Checked;
            try
            {
                s.ReminderMinutes = Int32.Parse(tbReminder.Text);
            }
            catch (Exception)
            {
                s.ReminderMinutes = 5;
                tbReminder.Text = "5";
            }
            s.UseAlternateReminder = cbUseAlterateReminder.Checked;
            s.EarlyReminder = dtpEarlyReminder.Value;
            s.LateReminder = dtpLateReminder.Value;
// #735 - End
        }

        private void ShowErrorMessage(string caption, string message)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool ValidateTQSettings()
        {
            int tmp = 0;
            string portError = string.Format("Custom TQ port value must be a number between {0} and {1}", System.Net.IPEndPoint.MinPort, System.Net.IPEndPoint.MaxPort);

            // Check port is 1) an int and 2) a valid tcp port
            if (!int.TryParse(tbTQServerPort.Text, out tmp))
            {
                ShowErrorMessage("Non numeric value", portError);
                return false;
            }
            else if (tmp < System.Net.IPEndPoint.MinPort || tmp > System.Net.IPEndPoint.MaxPort)
            {
                ShowErrorMessage("Invalid Custom TQ port", portError);
                return false;
            }

            System.Net.IPAddress addy;

            if (!System.Net.IPAddress.TryParse(tbTQServerAddress.Text, out addy))
            {
                ShowErrorMessage("Invalid custom TQ IP address", "Custom TQ address value must be a valid IP address. Example: 87.237.38.200");
                return false;
            }

            return true;
        }

        private bool ValidateIGBSettings()
        {
            int l_igbPort = -1;
            try
            {
                l_igbPort = Int32.Parse(tb_IgbPort.Text);
            }
            catch (FormatException) { }

            if ((l_igbPort < System.Net.IPEndPoint.MinPort) || (l_igbPort > System.Net.IPEndPoint.MaxPort))
            {
                MessageBox.Show(string.Format("IGB port value must be between {0} and {1}",
                    System.Net.IPEndPoint.MinPort, System.Net.IPEndPoint.MaxPort),
                    "Invalid IGB Port",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                tb_IgbPort.Focus();
                tb_IgbPort.SelectAll();
                return false;
            }
            return true;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!ValidateIGBSettings() || !ValidateTQSettings()) return;
            ApplyToSettings(m_settings);
            m_settings.Save();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            cbScreenList.Items.Clear();
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                cbScreenList.Items.Add(String.Format("Screen {0}", i + 1));
            }

            cbSkillIconSet.Items.Clear();
            for (int i = 1; i < EVEMon.Resources.icons.Skill_Select.IconSettings.Default.Properties.Count; i++)
            {
                cbSkillIconSet.Items.Add(EVEMon.Resources.icons.Skill_Select.IconSettings.Default.Properties["Group" + i].DefaultValue.ToString());
            }

            int maxWidth = 0;
            int maxHeight = 0;
            foreach (TabPage tp in tabControl1.TabPages)
            {
                tabControl1.SelectedTab = tp;
                Size prefSize = tp.Controls[0].GetPreferredSize(new Size(Int32.MaxValue, Int32.MaxValue));
                prefSize.Width += tp.Padding.Left + tp.Padding.Right;
                prefSize.Height += tp.Padding.Top + tp.Padding.Bottom;
                maxWidth = Math.Max(maxWidth, prefSize.Width);
                maxHeight = Math.Max(maxHeight, prefSize.Height);
            }
            int extraHeight = tabControl1.ClientSize.Height - tabControl1.TabPages[0].Height;
            int extraWidth = tabControl1.ClientSize.Width - tabControl1.TabPages[0].Width;
            tabControl1.ClientSize = new Size(maxWidth + extraWidth, maxHeight + extraHeight);
            tabControl1.SelectedTab = tabControl1.TabPages[0];

            // General options.
			// this groupbox uses WinAPI, recheck this once #894 is resolved
			groupBox1.Enabled = Environment.OSVersion.Platform != PlatformID.Unix;
            rbSystemTrayOptionsNever.Checked = m_settings.SystemTrayOptionsIsNever;
            rbSystemTrayOptionsMinimized.Checked = m_settings.SystemTrayOptionsIsMinimized;
            rbSystemTrayOptionsAlways.Checked = m_settings.SystemTrayOptionsIsAlways;
            cbCloseToTray.Checked = m_settings.CloseToTray;

			// binary assemblies won't work on *nix
			groupBox5.Enabled = Environment.OSVersion.Platform != PlatformID.Unix;
            cbRelocateEveWindow.Checked = m_settings.RelocateEveWindow;
            if (m_settings.RelocateTargetScreen < cbScreenList.Items.Count)
            {
                cbScreenList.SelectedIndex = m_settings.RelocateTargetScreen;
            }
            else
            {
                cbScreenList.SelectedIndex = 0;
            }

			// mixed mode assemblies don't work with mono
			groupBox12.Enabled = Type.GetType("Mono.Runtime") == null;
            cbG15ACycle.Checked = m_settings.G15ACycle;
            cbUseLogitechG15Display.Checked = m_settings.UseLogitechG15Display;
            cbG15ShowTime.Checked = m_settings.G15ShowTime;
            ACycleInterval.Value = m_settings.G15ACycleint;
// 947 - Start
            cbShowAllPublicSkills.Checked = m_settings.ShowAllPublicSkills;
            cbShowNonPublicSkills.Checked = m_settings.ShowNonPublicSkills;
// 947 - End

            // Look and feel options
            cbWorksafeMode.Checked = m_settings.WorksafeMode;
            cbTitleToTime.Checked = m_settings.TitleToTime;
            cbWindowsTitleList.SelectedIndex = m_settings.TitleToTimeLayout - 1;
            cbSkillInTitle.Checked = m_settings.TitleToTimeSkill;
            gbSkillPlannerHighlighting.Enabled = !cbWorksafeMode.Checked;
            cbRunIGBServer.Checked = m_settings.RunIGBServer;
            cbIGBPublic.Checked = m_settings.IGBServerPublic;
            tb_IgbPort.Text = m_settings.IGBServerPort.ToString();

            // Skill Icon Set
            if (m_settings.SkillIconGroup <= cbSkillIconSet.Items.Count && m_settings.SkillIconGroup > 0)
            {
                cbSkillIconSet.SelectedIndex = m_settings.SkillIconGroup - 1;
            }
            else
            {
                cbSkillIconSet.SelectedIndex = 0;
            }

            cbShowBalloonTips.Checked = m_settings.EnableBalloonTips;
            cbPlaySoundOnSkillComplete.Checked = m_settings.PlaySoundOnSkillComplete;
            cbShowCompletedSkillsDialog.Checked = m_settings.EnableSkillCompleteDialog;

            if (m_settings.NotificationOffset > 600)
            {
                m_settings.NotificationOffset = 600;
            }
            tbNotificationOffset.Value = m_settings.NotificationOffset;
            lblNotificationOffset.Text = FormatOffset(m_settings.NotificationOffset);

            cbSendEmail.Checked = m_settings.EnableEmailAlert;
            tbMailServer.Text = m_settings.EmailServer;
            tbPortNumber.Text = m_settings.PortNumber.ToString();
            cbEmailServerRequireSsl.Checked = m_settings.EmailServerRequiresSsl;
            cbEmailUseShortFormat.Checked = m_settings.EmailUseShortFormat;
            cbEmailAuthRequired.Checked = m_settings.EmailAuthRequired;
            tbEmailUsername.Text = m_settings.EmailAuthUsername;
            tbEmailPassword.Text = m_settings.EmailAuthPassword;
            tbFromAddress.Text = m_settings.EmailFromAddress;
            tbToAddress.Text = m_settings.EmailToAddress;

            // Proxy settings
            cbDisableOnAuthFailure.Checked = m_settings.DisableRequestsOnAuthenticationFailure;
            rbDefaultProxy.Checked = (m_settings.UseCustomProxySettings == false);
            rbCustomProxy.Checked = (m_settings.UseCustomProxySettings);
            tbProxyHttpHost.Text = m_settings.HttpProxy.Host;
            tbProxyHttpPort.Text = m_settings.HttpProxy.Port.ToString();
            btnProxyHttpAuth.Tag = m_settings.HttpProxy.Clone();

            cbCheckTranquilityStatus.Checked = m_settings.CheckTranquilityStatus;
            numericStatusInterval.Value = m_settings.StatusUpdateInterval;

            cbAutomaticallySearchForNewVersions.Checked = m_settings.DisableEVEMonVersionCheck;
            cbCheckTimeOnStartup.Checked = m_settings.CheckTimeOnStartup;
            cbAutomaticEOSkillUpdate.Checked = m_settings.DisableXMLAutoUpdate;

            int j;
            for (j = 0; j < m_apiUpdateLookup.Length; j++)
            {
                if (m_apiUpdateLookup[j] == m_settings.APIUpdateDelay) break;
            }
            if (j == m_apiUpdateLookup.Length) j = 2;

            cmbAPIUpdateDelay.SelectedIndex = j;

            cbKeepCharacterPlans.Checked = m_settings.KeepCharacterPlans;

            cbHighlightPlannedSkills.Checked = m_settings.SkillPlannerHighlightPlannedSkills;
            cbHighlightPrerequisites.Checked = m_settings.SkillPlannerHighlightPrerequisites;
            cbHighlightConflicts.Checked = m_settings.SkillPlannerHighlightConflicts;
            cbHighlightPartialSkills.Checked = m_settings.SkillPlannerHighlightPartialSkills;

            // Copy the current popup settings so we don't change m_settings in the config dialogs
            m_tooltipString = m_settings.TooltipString;
            m_trayPopupConfig = m_settings.TrayPopupConfig.Copy();
            cbTrayPopupStyle.Items.AddRange(m_trayPopupStyle);
            cbTrayPopupStyle.SelectedIndex = (int)m_settings.TrayPopupStyle;

            // Load Calendar Settings
            panelColorBlocking.BackColor = m_settings.CalendarBlockingColor;
            panelColorRecurring1.BackColor = m_settings.CalendarRecurring1;
            panelColorRecurring2.BackColor = m_settings.CalendarRecurring2;
            panelColorSingle1.BackColor = m_settings.CalendarSingle1;
            panelColorSingle2.BackColor = m_settings.CalendarSingle2;
            panelColorText.BackColor = m_settings.CalendarTextColor;

// #735 - Start
            // Load External Calendar settings.
            try
            {
                cbUseExternalCalendar.Checked = m_settings.UseExternalCalendar;
                if (m_settings.CalendarOption == 0)
                    rbMSOutlook.Checked = true;
                else
                    rbGoogle.Checked = true;
                cbUseExternalCalendar_CheckedChanged(cbUseExternalCalendar, e);
                tbGoogleEmail.Text = m_settings.GoogleEmail;
                tbGooglePassword.Text = m_settings.GooglePassword;
                tbGoogleURI.Text = m_settings.GoogleURI;
                cbGoogleReminder.SelectedIndex = m_settings.GoogleReminder;
                cbSetReminder.Checked = m_settings.SetReminder;
                tbReminder.Text = m_settings.ReminderMinutes.ToString();
                cbUseAlterateReminder.Checked = m_settings.UseAlternateReminder;
                dtpEarlyReminder.Value = m_settings.EarlyReminder;
                dtpLateReminder.Value = m_settings.LateReminder;
            }
            catch (Exception) { }
// #735 - End

            // New bits to allow custom server/port options when checking the server status
            tbTQServerAddress.Text = m_settings.CustomTQAddress != "" ? m_settings.CustomTQAddress : "87.237.38.200";
            tbTQServerPort.Text = m_settings.CustomTQPort != "" ? m_settings.CustomTQPort : "26000";
            cbCustomTQSettings.Checked = m_settings.UseCustomTQCheckSettings;

            // If we're using custom TQ settings enable the groupbox and hence the options
            tlpCustomTQSettings.Enabled = cbCustomTQSettings.Checked;
            cbShowTQBalloon.Checked = m_settings.ShowTQBalloon;
            tbConnectivityURL.Text  = m_settings.ConnectivityURL;

            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            if (rk != null && rk.GetValue("EVEMon") == null)
            {
                cbRunAtStartup.Checked = false;
            }
            else
            {
                cbRunAtStartup.Checked = true;
            }
            // API Configuration
            gbAPIConfiguration.Enabled = gbAPIConfiguration.Visible = Singleton.Instance<APIState>().DebugMode;
            InitialiseAPIConfigDropDown();
            UpdateDisables();
        }

        private string FormatOffset(int offset)
        {
            int m, s;
            m = offset / 60;
            s = offset - (m * 60);
            return String.Format("{0}m, {1}s", m, s);
        }

        private void cbRunAtStartup_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            CheckBox cbSender = sender as CheckBox;

            //it's possible to get to this point without write access, check for nulls just in case
            if (rk == null)
            {
                cbSender.Checked = false;
                return;
            }
            if (cbSender.Checked)
            {
                rk.SetValue("EVEMon", String.Format("\"{0}\" {1}", Application.ExecutablePath.ToString(), "-startMinimized"));
            }
            else
            {
                rk.DeleteValue("EVEMon", false);
            }
        }

        private void cbWorksafeMode_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDisables();
        }

        private void cbSendEmail_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDisables();
        }

        private void UpdateDisables()
        {
            tlpEmailSettings.Enabled = cbSendEmail.Checked;
            btnTestEmail.Enabled = cbSendEmail.Checked;
            tlpEmailAuthTable.Enabled = cbEmailAuthRequired.Checked;

            bool isValid = true;
            isValid = isValid && ValidateProxySetting(tbProxyHttpHost.Text, tbProxyHttpPort.Text);

            btnOk.Enabled = isValid;
            numericStatusInterval.Enabled = cbCheckTranquilityStatus.Checked;
            cbShowTQBalloon.Enabled = cbCheckTranquilityStatus.Checked;
            cbCustomTQSettings.Enabled = cbCheckTranquilityStatus.Checked;
            tbConnectivityURL.Enabled = cbCheckTranquilityStatus.Checked;
            cbWindowsTitleList.Enabled = cbTitleToTime.Checked;
            cbSkillInTitle.Enabled = cbTitleToTime.Checked;
            gbSkillPlannerHighlighting.Enabled = !cbWorksafeMode.Checked;
            btnEditAPIServer.Enabled = btnDeleteAPIServer.Enabled = cbAPIServer.SelectedItem == null ? false : !((APIConfiguration) cbAPIServer.SelectedItem).IsDefault;
        }

        private bool ValidateProxySetting(string host, string port)
        {
            if (rbCustomProxy.Checked)
            {
                if (String.IsNullOrEmpty(host))
                {
                    return false;
                }

                int junk;
                if (!Int32.TryParse(port, out junk) || junk < 0)
                {
                    return false;
                }
            }
            return true;
        }

        private void btnTestEmail_Click(object sender, EventArgs e)
        {
            Settings ts = new Settings();
            ts.NeverSave();
            ApplyToSettings(ts);
            if (!Emailer.SendTestMail(ts))
            {
                MessageBox.Show("The message failed to send.", "Mail Failure", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("The message sent successfully. Please verify that the message was received.",
                                "Mail Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cbEmailAuthRequired_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDisables();
        }

        private void btnIdentifyScreens_Click(object sender, EventArgs e)
        {
            IdentifyScreenForm.Display();
        }

        private void cbRelocateEveWindow_CheckedChanged(object sender, EventArgs e)
        {
            flpScreenSelect.Enabled = cbRelocateEveWindow.Checked;
        }

        private void rbCustomProxy_CheckedChanged(object sender, EventArgs e)
        {
            vfpCustomProxySettings.Enabled = rbCustomProxy.Checked;
            UpdateDisables();
        }

        private void tbProxyHttpPort_TextChanged(object sender, EventArgs e)
        {
            UpdateDisables();
        }

        private void btnProxyHttpAuth_Click(object sender, EventArgs e)
        {
            ProxySetting ps = ((ProxySetting)btnProxyHttpAuth.Tag).Clone();
            using (ProxyAuthenticationWindow f = new ProxyAuthenticationWindow())
            {
                f.ProxySetting = ps;
                DialogResult dr = f.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    btnProxyHttpAuth.Tag = f.ProxySetting.Clone();
                }
            }
        }

        private void cbCheckTranquilityStatus_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDisables();
        }

        private void cbAutomaticallySearchForNewVersions_CheckedChanged(object sender, EventArgs e)
        {
            UpdateManager um = UpdateManager.GetInstance();
            if (cbAutomaticallySearchForNewVersions.Checked == false)
            {
                um.UpdateAvailable += new UpdateAvailableHandler(um_UpdateAvailable);
                um.Start();
            }
            else
            {
                try
                {
                    um.UpdateAvailable -= new UpdateAvailableHandler(um_UpdateAvailable);
                    um.Stop();
                }
                catch
                {
                    //We ignore this since the form may be starting up and thus 
                    //UpdateManager has no running start method.
                }
            }
            UpdateDisables();
        }

        private bool m_updateShowing = false;

        private void um_UpdateAvailable(object sender, UpdateAvailableEventArgs e)
        {
            if (e.NewestVersion <= new Version(m_settings.IgnoreUpdateVersion))
            {
                return;
            }

            this.Invoke(new MethodInvoker(delegate
            {
                if (!m_updateShowing)
                {
                    m_updateShowing = true;
                    using (UpdateNotifyForm f = new UpdateNotifyForm(m_settings, e))
                    {
                        f.ShowDialog();
                        if (f.DialogResult == DialogResult.OK)
                        {
                            this.Close();
                        }
                    }
                    m_updateShowing = false;
                }
            }));
        }

        private void cbSkillIconSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            ImageList def = new ImageList();
            def.ColorDepth = ColorDepth.Depth32Bit;
            string groupname = null;
            if (cbSkillIconSet.SelectedIndex >= 0 && cbSkillIconSet.SelectedIndex < EVEMon.Resources.icons.Skill_Select.IconSettings.Default.Properties.Count - 1)
            {
                groupname = EVEMon.Resources.icons.Skill_Select.IconSettings.Default.Properties["Group" + (cbSkillIconSet.SelectedIndex + 1)].DefaultValue.ToString();
            }
            if ((groupname != null
                && !System.IO.File.Exists(
                    String.Format(
                        "{1}Resources{0}icons{0}Skill_Select{0}Group{2}{0}{3}.resources",
                        Path.DirectorySeparatorChar,
                        System.AppDomain.CurrentDomain.BaseDirectory,
                        (cbSkillIconSet.SelectedIndex + 1),
                        groupname)))
                || !System.IO.File.Exists(
                    String.Format(
                        "{1}Resources{0}icons{0}Skill_Select{0}Group0{0}Default.resources",
                        Path.DirectorySeparatorChar,
                        System.AppDomain.CurrentDomain.BaseDirectory)))
            {
                groupname = null;
            }
            if (groupname != null)
            {
                System.Resources.IResourceReader basic = new System.Resources.ResourceReader(
                    String.Format(
                        "{1}Resources{0}icons{0}Skill_Select{0}Group0{0}Default.resources",
                        Path.DirectorySeparatorChar,
                        System.AppDomain.CurrentDomain.BaseDirectory));
                System.Collections.IDictionaryEnumerator basicx = basic.GetEnumerator();
                while (basicx.MoveNext())
                {
                    def.Images.Add(basicx.Key.ToString(), (System.Drawing.Icon)basicx.Value);
                }
                basic.Close();
                basic = new System.Resources.ResourceReader(
                    String.Format(
                        "{1}Resources{0}icons{0}Skill_Select{0}Group{2}{0}{3}.resources",
                        Path.DirectorySeparatorChar,
                        System.AppDomain.CurrentDomain.BaseDirectory,
                        (cbSkillIconSet.SelectedIndex + 1),
                        groupname));
                basicx = basic.GetEnumerator();
                while (basicx.MoveNext())
                {
                    if (def.Images.ContainsKey(basicx.Key.ToString()))
                    {
                        def.Images.RemoveByKey(basicx.Key.ToString());
                    }
                    def.Images.Add(basicx.Key.ToString(), (System.Drawing.Icon)basicx.Value);
                }
                basic.Close();
            }
            tvlist.Nodes.Clear();
            tvlist.ImageList = def;
            tvlist.ImageList.ColorDepth = ColorDepth.Depth32Bit;
            TreeNode gtn = new TreeNode("Book", tvlist.ImageList.Images.IndexOfKey("book"), tvlist.ImageList.Images.IndexOfKey("book"));
            gtn.Nodes.Add(new TreeNode("Pre-Reqs NOT met (Rank)", tvlist.ImageList.Images.IndexOfKey("PrereqsNOTMet"), tvlist.ImageList.Images.IndexOfKey("PrereqsNOTMet")));
            gtn.Nodes.Add(new TreeNode("Pre-Reqs met (Rank)", tvlist.ImageList.Images.IndexOfKey("PrereqsMet"), tvlist.ImageList.Images.IndexOfKey("PrereqsMet")));
            for (int i = 0; i < 6; i++)
            {
                gtn.Nodes.Add(new TreeNode("Level " + i + " (Rank)", tvlist.ImageList.Images.IndexOfKey("lvl" + i), tvlist.ImageList.Images.IndexOfKey("lvl" + i)));
            }
            gtn.Expand();
            tvlist.Nodes.Add(gtn);
        }

        private void rbSystemTrayOptionsNever_CheckedChanged(object sender, EventArgs e)
        {
            cbCloseToTray.Enabled = !rbSystemTrayOptionsNever.Checked;
            gboxTrayIconPopUpOptions.Enabled = !rbSystemTrayOptionsNever.Checked;
        }

        private void tbNotificationOffset_ValueChanged(object sender, EventArgs e)
        {
            lblNotificationOffset.Text = FormatOffset(tbNotificationOffset.Value);
        }

        private void cbCustomTQSettings_CheckedChanged(object sender, EventArgs e)
        {
            tlpCustomTQSettings.Enabled = cbCustomTQSettings.Checked;

            // Make sure we reset the defaults when turned off
            if (!cbCustomTQSettings.Checked)
            {
                tbTQServerPort.Text = "26000";
                tbTQServerAddress.Text = "87.237.38.200";
            }
        }

        private void cbTitleToTime_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDisables();
        }

        private void colorPanel_Click(object sender, EventArgs e)
        {
            Panel color = (Panel)sender;
            colorDialog.Color = color.BackColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                color.BackColor = colorDialog.Color;
            }
        }

        /// <summary>
        /// Instantiates the appropriate configuration dialog for the Tray Tooltip & Popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfigureTrayPopUp_Click(object sender, EventArgs e)
        {
            // Get the selected display style
            TrayPopupStyles selectedStyle = (TrayPopupStyles)cbTrayPopupStyle.SelectedIndex;
            switch (selectedStyle)
            {
                case TrayPopupStyles.PopupForm:
                    using (TrayPopupConfigForm f = new TrayPopupConfigForm())
                    {
                        // Edit a copy of the current settings
                        f.Config = m_trayPopupConfig.Copy();
                        f.ShowDialog();
                        if (f.DialogResult == DialogResult.OK)
                        {
                            // Save changes in local copy
                            m_trayPopupConfig = f.Config;
                        }
                    }
                    break;
                case TrayPopupStyles.WindowsTooltip:
                    using (TrayTooltipConfigForm f = new TrayTooltipConfigForm())
                    {
                        // Set current tooltip string
                        f.TooltipString = m_tooltipString;
                        f.ShowDialog();
                        if (f.DialogResult == DialogResult.OK)
                        {
                            // Save changes in local copy
                            m_tooltipString = f.TooltipString;
                        }
                    }
                    break;
            }
        }

        private void rbMSOutlook_Click(object sender, EventArgs e)
        {
            gbGoogle.Enabled = rbGoogle.Checked;
        }

        private void cbUseExternalCalendar_CheckedChanged(object sender, EventArgs e)
        {
            // Enable or disable the relevant fields.
            rbMSOutlook.Enabled = cbUseExternalCalendar.Checked;
            rbGoogle.Enabled = cbUseExternalCalendar.Checked;
            if (cbUseExternalCalendar.Checked)
            {
                if (rbGoogle.Checked)
                    gbGoogle.Enabled = true;
                else
                    gbGoogle.Enabled = false;
            }
            else
                gbGoogle.Enabled = false;
            cbSetReminder.Enabled = cbUseExternalCalendar.Checked;
            tbReminder.Enabled = cbUseExternalCalendar.Checked;
            cbUseAlterateReminder.Enabled = cbUseExternalCalendar.Checked;
            dtpEarlyReminder.Enabled = cbUseExternalCalendar.Checked;
            dtpLateReminder.Enabled = cbUseExternalCalendar.Checked;
        }


        private void cbShowAllPublicSkills_CheckedChanged(object sender, EventArgs e)
        {
            if (cbShowAllPublicSkills.Checked)
            {
                cbShowNonPublicSkills.Enabled = true;
                cbShowNonPublicSkills.Checked = m_settings.ShowNonPublicSkills;
            }
            else
            {
                cbShowNonPublicSkills.Checked = false;
                cbShowNonPublicSkills.Enabled = false;
            }
        }

        private void btnAddAPIServer_Click(object sender, EventArgs e)
        {
            APIConfiguration newConfiguration = new APIConfiguration();
            newConfiguration.Methods = APIConfiguration.DefaultMethods;
            using (APISettingsForm apiForm = new APISettingsForm(newConfiguration))
            {
                DialogResult result = apiForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    m_settings.APIConfigurations.Add(newConfiguration);
                    m_settings.CustomAPIConfiguration = newConfiguration.Name;
                    InitialiseAPIConfigDropDown();
                }
            }
        }

        private void btnEditAPIServer_Click(object sender, EventArgs e)
        {
            using (APISettingsForm apiForm = new APISettingsForm(cbAPIServer.SelectedItem as APIConfiguration))
            {
                apiForm.ShowDialog();
            }
        }

        private void btnDeleteAPIServer_Click(object sender, EventArgs e)
        {
            DialogResult deleteServer =
                MessageBox.Show(
                    string.Format("Delete API Server configuration \"{0}\"?",
                                  ((APIConfiguration) cbAPIServer.SelectedItem).Name),
                    "Delete API Server?", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);
            if (deleteServer == DialogResult.Yes)
            {
                m_settings.APIConfigurations.Remove((APIConfiguration) cbAPIServer.SelectedItem);
                m_settings.CustomAPIConfiguration = m_settings.APIConfigurations.Count > 0
                                                        ? m_settings.APIConfigurations[0].Name
                                                        : APIConfiguration.DefaultConfiguration.Name;
                InitialiseAPIConfigDropDown();
            }
        }

        private void cbAPIServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_settings.CustomAPIConfiguration = ((APIConfiguration) cbAPIServer.SelectedItem).Name;
            UpdateDisables();
        }

        private void InitialiseAPIConfigDropDown()
        {
            List<APIConfiguration> configurations = new List<APIConfiguration>();
            configurations.AddRange(m_settings.APIConfigurations);
            configurations.Add(APIConfiguration.DefaultConfiguration);
            cbAPIServer.Items.Clear();
            foreach (APIConfiguration configuration in configurations)
            {
                cbAPIServer.Items.Add(configuration);
                if (configuration.Name == m_settings.CustomAPIConfiguration)
                    cbAPIServer.SelectedItem = configuration;
            }
        }
    }
}
