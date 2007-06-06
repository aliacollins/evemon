using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using EVEMon.Common;
using EVEMon.WindowRelocator;
using Microsoft.Win32;

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
            s.ShowLoginName = cbShowLoginName.Checked;
            s.UseLogitechG15Display = cbUseLogitechG15Display.Checked;
            s.G15ACycle = cbG15ACycle.Checked;
            s.G15ACycleint = (int)ACycleInterval.Value;
            if (G15Handler.LCD != null)
            {
                G15Handler.LCD.cycle = cbG15ACycle.Checked;
                G15Handler.LCD.cycleint = (int)ACycleInterval.Value;
            }

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

            s.TooltipString = tbTooltipString.Text;

            s.UseCustomProxySettings = rbCustomProxy.Checked;
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

            m_settings.CheckTranquilityStatus = cbCheckTranquilityStatus.Checked;
            m_settings.StatusUpdateInterval = (int)numericStatusInterval.Value;

            m_settings.DisableXMLAutoUpdate = cbAutomaticEOSkillUpdate.Checked;

            m_settings.DeleteCharacterSilently = cbDeleteCharactersSilently.Checked;
            m_settings.KeepCharacterPlans = cbKeepCharacterPlans.Checked;

            m_settings.DisableEVEMonVersionCheck = cbAutomaticallySearchForNewVersions.Checked;

            m_settings.EnableSkillCompleteDialog = cbShowCompletedSkillsDialog.Checked;
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

            // Look and feel options

            rbSystemTrayOptionsNever.Checked = m_settings.SystemTrayOptionsIsNever;
            rbSystemTrayOptionsMinimized.Checked = m_settings.SystemTrayOptionsIsMinimized;
            rbSystemTrayOptionsAlways.Checked = m_settings.SystemTrayOptionsIsAlways;

            cbCloseToTray.Checked = m_settings.CloseToTray;
            cbTitleToTime.Checked = m_settings.TitleToTime;
            cbWindowsTitleList.SelectedIndex = m_settings.TitleToTimeLayout - 1;
            cbSkillInTitle.Checked = m_settings.TitleToTimeSkill;
            cbWorksafeMode.Checked = m_settings.WorksafeMode;
            gbSkillPlannerHighlighting.Enabled = !cbWorksafeMode.Checked;
            cbRunIGBServer.Checked = m_settings.RunIGBServer;
            cbIGBPublic.Checked = m_settings.IGBServerPublic;
            tb_IgbPort.Text = m_settings.IGBServerPort.ToString();
            cbRelocateEveWindow.Checked = m_settings.RelocateEveWindow;
            cbG15ACycle.Checked = m_settings.G15ACycle;
            cbUseLogitechG15Display.Checked = m_settings.UseLogitechG15Display;
            ACycleInterval.Value = m_settings.G15ACycleint;
            cbShowLoginName.Checked = m_settings.ShowLoginName;

            if (m_settings.RelocateTargetScreen < cbScreenList.Items.Count)
            {
                cbScreenList.SelectedIndex = m_settings.RelocateTargetScreen;
            }
            else
            {
                cbScreenList.SelectedIndex = 0;
            }

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

            rbDefaultProxy.Checked = (m_settings.UseCustomProxySettings == false);
            rbCustomProxy.Checked = (m_settings.UseCustomProxySettings);
            tbProxyHttpHost.Text = m_settings.HttpProxy.Host;
            tbProxyHttpPort.Text = m_settings.HttpProxy.Port.ToString();
            btnProxyHttpAuth.Tag = m_settings.HttpProxy.Clone();

            cbCheckTranquilityStatus.Checked = m_settings.CheckTranquilityStatus;
            numericStatusInterval.Value = m_settings.StatusUpdateInterval;

            cbAutomaticallySearchForNewVersions.Checked = m_settings.DisableEVEMonVersionCheck;
            cbAutomaticEOSkillUpdate.Checked = m_settings.DisableXMLAutoUpdate;

            cbDeleteCharactersSilently.Checked = m_settings.DeleteCharacterSilently;
            cbKeepCharacterPlans.Checked = m_settings.KeepCharacterPlans;

            cbHighlightPlannedSkills.Checked = m_settings.SkillPlannerHighlightPlannedSkills;
            cbHighlightPrerequisites.Checked = m_settings.SkillPlannerHighlightPrerequisites;
            cbHighlightConflicts.Checked = m_settings.SkillPlannerHighlightConflicts;

            cbTooltipDisplay.Items.Clear();
            for (int i = 0; i < tooltipCodes.Length; i++)
            {
                cbTooltipDisplay.Items.Add(FormatExampleTooltipText(tooltipCodes[i]));
            }
            cbTooltipDisplay.Items.Add(" -- Custom -- ");

            tbTooltipString.Text = m_settings.TooltipString;

            // New bits to allow custom server/port options when checking the server status
            tbTQServerAddress.Text = m_settings.CustomTQAddress != "" ? m_settings.CustomTQAddress : "87.237.38.200";
            tbTQServerPort.Text = m_settings.CustomTQPort != "" ? m_settings.CustomTQPort : "26000";
            cbCustomTQSettings.Checked = m_settings.UseCustomTQCheckSettings;

            // If we're using custom TQ settings enable the groupbox and hence the options
            tlpCustomTQSettings.Enabled = cbCustomTQSettings.Checked;
            cbShowTQBalloon.Checked = m_settings.ShowTQBalloon;

            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            if (rk != null && rk.GetValue("EVEMon") == null)
            {
                cbRunAtStartup.Checked = false;
            }
            else
            {
                cbRunAtStartup.Checked = true;
            }
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
            gbSkillPlannerHighlighting.Enabled = !cbWorksafeMode.Checked;
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
            cbWindowsTitleList.Enabled = cbTitleToTime.Checked;
            cbSkillInTitle.Enabled = cbTitleToTime.Checked;
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
            if ((groupname != null && !System.IO.File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\icons\\Skill_Select\\Group" + (cbSkillIconSet.SelectedIndex + 1) + "\\" + groupname + ".resources"))
                || !System.IO.File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\icons\\Skill_Select\\Group0\\Default.resources"))
            {
                groupname = null;
            }
            if (groupname != null)
            {
                System.Resources.IResourceReader basic = new System.Resources.ResourceReader(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\icons\\Skill_Select\\Group0\\Default.resources");
                System.Collections.IDictionaryEnumerator basicx = basic.GetEnumerator();
                while (basicx.MoveNext())
                {
                    def.Images.Add(basicx.Key.ToString(), (System.Drawing.Icon)basicx.Value);
                }
                basic.Close();
                basic = new System.Resources.ResourceReader(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\icons\\Skill_Select\\Group" + (cbSkillIconSet.SelectedIndex + 1) + "\\" + groupname + ".resources");
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

        // Array containing the example tooltip formats that are populated into the dropdown box.
        static string[] tooltipCodes = {
            "%n - %s %tr - %r",
            "%n - %s [%cr->%tr]: %r",
            "%n : %s - %d : %b isk",
            "%s %ci to %ti, %r left"
        };

        private void tbTooltipString_TextChanged(object sender, EventArgs e)
        {
            tbTooltipTestDisplay.Text = FormatExampleTooltipText(tbTooltipString.Text);

            if (cbTooltipDisplay.SelectedIndex == -1)
            {
                int index = tooltipCodes.Length;

                for (int i = 0; i < tooltipCodes.Length; i++)
                {
                    if (tooltipCodes[i].Equals(tbTooltipString.Text))
                    {
                        index = i;
                    }
                }

                cbTooltipDisplay.SelectedIndex = index;
                DisplayCustomControls(index == tooltipCodes.Length);
            }
        }

        // Formats the argument format string with hardcoded exampe values.  Works basically the
        // same as MainWindow.FormatTooltipText(...), with the exception of the exampe values.
        private string FormatExampleTooltipText(string fmt)
        {
            return Regex.Replace(fmt, "%([nbsdr]|[ct][ir])", new MatchEvaluator(delegate(Match m)
                {
                    string value = String.Empty;
                    char capture = m.Groups[1].Value[0];

                    switch (capture)
                    {
                        case 'n':
                            value = "John Doe";
                            break;
                        case 'b':
                            value = "183,415,254.05";
                            break;
                        case 's':
                            value = "Gunnery";
                            break;
                        case 'd':
                            value = "9/15/2006 6:36 PM";
                            break;
                        case 'r':
                            value = "2h, 53m, 28s";
                            break;
                        default:
                            int level = -1;
                            if (capture == 'c')
                            {
                                level = 3;
                            }
                            else if (capture == 't')
                            {
                                level = 4;
                            }

                            if (m.Groups[1].Value.Length > 1 && level >= 0)
                            {
                                capture = m.Groups[1].Value[1];

                                if (capture == 'i')
                                {
                                    value = level.ToString();
                                }
                                else if (capture == 'r')
                                {
                                    value = Skill.GetRomanForInt(level);
                                }
                            }
                            break;
                    }

                    return value;
                }), RegexOptions.Compiled);
        }

        private void cbTooltipDisplay_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int index = cbTooltipDisplay.SelectedIndex;

            if (index == tooltipCodes.Length)
            {
                tbTooltipString.Text = m_settings.TooltipString;
                DisplayCustomControls(true);
            }
            else
            {
                tbTooltipString.Text = tooltipCodes[index];
                DisplayCustomControls(false);
            }
        }

        /// <summary>
        /// Toggles the visibility of the tooltip example display and code label, as well as the readonly status of the tooltip string itself.
        /// </summary>
        /// <param name="custom">Show tbTooltipTestDisplay?</param>
        private void DisplayCustomControls(bool custom)
        {
            tbTooltipTestDisplay.Visible = custom;
            tbTooltipString.ReadOnly = !custom;
        }

        private void rbSystemTrayOptionsNever_CheckedChanged(object sender, EventArgs e)
        {
            cbCloseToTray.Enabled = !rbSystemTrayOptionsNever.Checked;
            gboxTooltipOptions.Enabled = !rbSystemTrayOptionsNever.Checked;
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
    }
}
