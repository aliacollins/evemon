using EVEMon.Common;
using System;

namespace EVEMon
{
    /// <summary>
    /// Notification dialog to inform user of a possible clock error
    /// </summary>
    public partial class TimeCheckNotification : EVEMonForm
    {
        private readonly Settings _settings;
        private readonly DateTime _serverTime;
        private readonly DateTime _localTime;

        public TimeCheckNotification()
        {
            InitializeComponent();
            this.uxTitleLabel.Font = FontHelper.GetFont("Tahoma", 12F);
        }

        public TimeCheckNotification(DateTime serverTime, DateTime localTime)
            : this ()
        {
            _settings = Settings.GetInstance();
            _serverTime = serverTime;
            _localTime = localTime;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            uxTimeZoneField.Text = TimeZone.CurrentTimeZone.StandardName;
            uxExpectedTimeField.Text = _serverTime.ToString();
            uxActualTimeField.Text = _localTime.ToString();
            uxCheckTimeOnStartUpCheckBox.Checked = _settings.CheckTimeOnStartup;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _settings.CheckTimeOnStartup = uxCheckTimeOnStartUpCheckBox.Checked;
            base.OnClosing(e);
        }

        private void uxCloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

