using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Attributes;
using EVEMon.Common.SettingsObjects;
using CommonProperties = EVEMon.Common.Properties;

namespace EVEMon.SettingsUI
{
    public partial class UpdateSettingsControl : UserControl
    {
        // Would have love to use tableLayoutPanel, unfortunately, they are just a piece of trash.
        private const int RowHeight = 28;

        private readonly List<ComboBox> m_combos = new List<ComboBox>();

        private UpdateSettings m_settings;

        public UpdateSettingsControl()
        {
            InitializeComponent();

            // Add the controls for every member of the enumeration
            int height = RowHeight;
            IEnumerable<APIMethods> methods = Enum.GetValues(typeof(APIMethods)).Cast<APIMethods>();
            foreach (APIMethods method in methods)
            {
                // Skip if there is no header
                if (!method.HasHeader())
                    continue;

                // Add the icon
                Bitmap icon = CommonProperties.Resources.APIKeyLimited16;
                string iconToolTip = "This query requires a limited API key.";
                if (method.HasAttribute<FullKeyAttribute>())
                {
                    icon = CommonProperties.Resources.APIKeyFull16;
                    iconToolTip = "This query requires a full API key.";
                }

                PictureBox picture = new PictureBox
                                         {
                                             Image = icon,
                                             Size = icon.Size,
                                             Location = new Point(0, height + (RowHeight - icon.Size.Height) / 2)
                                         };
                toolTip.SetToolTip(picture, iconToolTip);
                Controls.Add(picture);

                // Add the label
                Label label = new Label
                                  {
                                      AutoSize = false,
                                      Text = method.GetHeader(),
                                      TextAlign = ContentAlignment.MiddleLeft,
                                      Location = new Point(labelMethod.Location.X, height),
                                      Width = labelMethod.Width,
                                      Height = RowHeight
                                  };
                toolTip.SetToolTip(label, method.GetDescription());
                Controls.Add(label);

                // Add the "system tray tooltip" combo box
                ComboBox combo = new ComboBox { Tag = method };

                foreach (UpdatePeriod period in GetUpdatePeriods(method))
                {
                    string header = period.GetHeader();
                    if (period == UpdatePeriod.Never && method.HasAttribute<ForcedOnStartupAttribute>())
                        header = "On Startup";

                    combo.Items.Add(header);
                }

                combo.SelectedIndex = 0;
                combo.Margin = new Padding(3);
                combo.Height = RowHeight - 4;
                combo.Width = labelPeriod.Width;
                combo.DropDownStyle = ComboBoxStyle.DropDownList;
                combo.Location = new Point(labelPeriod.Location.X, height + 2);
                combo.SelectedIndexChanged += combo_SelectedIndexChanged;
                Controls.Add(combo);
                m_combos.Add(combo);

                // Updates the row ordinate
                height += RowHeight;
            }

            Height = height;
        }

        /// <summary>
        /// Gets or sets the settings to edit (should be a copy of the actual settings).
        /// </summary>
        [Browsable(false)]
        public UpdateSettings Settings
        {
            get { return m_settings; }
            set
            {
                m_settings = value;
                if (value == null)
                    return;

                foreach (ComboBox combo in m_combos)
                {
                    APIMethods method = (APIMethods)combo.Tag;
                    List<UpdatePeriod> periods = GetUpdatePeriods(method);
                    combo.SelectedIndex = Math.Max(0, periods.IndexOf(m_settings.Periods[method]));
                }
            }
        }

        /// <summary>
        /// When the selected indew changes, we update the settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            APIMethods method = (APIMethods)combo.Tag;
            List<UpdatePeriod> periods = GetUpdatePeriods(method);

            if (combo.SelectedIndex < 0 || combo.SelectedIndex >= periods.Count)
                return;

            if (method == APIMethods.MarketOrders)
                m_settings.Periods[APIMethods.CorporationMarketOrders] = periods[combo.SelectedIndex];

            if (method == APIMethods.IndustryJobs)
                m_settings.Periods[APIMethods.CorporationIndustryJobs] = periods[combo.SelectedIndex];

            m_settings.Periods[method] = periods[combo.SelectedIndex];
        }

        /// <summary>
        /// Gets the available update periods for the given method.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private List<UpdatePeriod> GetUpdatePeriods(APIMethods method)
        {
            List<UpdatePeriod> periods = new List<UpdatePeriod> { UpdatePeriod.Never };

            UpdateAttribute updateAttribute = method.GetAttribute<UpdateAttribute>();
            int min = (int)updateAttribute.Minimum;
            int max = (int)updateAttribute.Maximum;

            periods.AddRange(Enum.GetValues(typeof(UpdatePeriod)).Cast<UpdatePeriod>().Where(
                period => period != UpdatePeriod.Never).Select(period => new { period, index = (int)period }).Where(
                    period => period.index >= min && period.index <= max).Select(period => period.period));

            return periods;
        }
    }
}