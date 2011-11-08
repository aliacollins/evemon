using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
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

            int height = RowHeight;
            
            // Gets the API methods
            IEnumerable<Enum> apiMethods = APIMethods.Methods.Where(x => x.HasHeader());
            
            // Group the methods by usage
            List<Enum> methods = apiMethods.Where(method => method is APIGenericMethods).ToList();

            methods.AddRange(apiMethods.OfType<APICharacterMethods>().Where(
                method => (int)method == ((int)method & (int)(APIMethodsExtensions.BasicCharacterFeatures))).Cast<Enum>());

            methods.AddRange(apiMethods.OfType<APICharacterMethods>().Where(
                method => (int)method == ((int)method & (int)APIMethodsExtensions.AdvancedCharacterFeatures)).Cast<Enum>().OrderBy(
                    method => method.GetHeader()));

            // Uncomment upon implementing an exclicit corporation monitor feature
            //methods.AddRange(apiMethods.OfType<APICorporationMethods>().Where(
            //    method => (int)method == ((int)method & (int)APIMethodsExtensions.AdvancedCorporationFeatures)).Cast<Enum>().OrderBy(
            //        method => method.GetHeader()));

            // Add the controls for every member of the enumeration
            foreach (Enum method in methods)
            {
                // Add the icon
                Bitmap icon = CommonProperties.Resources.KeyGrey16;
                string iconToolTip = "This is a basic feature query.";
                if (method is APICharacterMethods)
                {
                    APICharacterMethods apiMethod = (APICharacterMethods)method;
                    if ((int)apiMethod == ((int)apiMethod & (int)APIMethodsExtensions.AdvancedCharacterFeatures))
                    {
                        icon = CommonProperties.Resources.KeyGold16;
                        iconToolTip = "This is an advanced feature query.";
                    }
                }

                // Uncomment upon implementing an exclicit corporation monitor feature
                //if (method is APICorporationMethods)
                //{
                //    APICorporationMethods apiMethod = (APICorporationMethods)method;
                //    if ((int)apiMethod == ((int)apiMethod & (int)APIMethodsExtensions.AdvancedCorporationFeatures))
                //    {
                //        icon = CommonProperties.Resources.KeyGold16;
                //        iconToolTip = "This is an advanced feature query.";
                //    }
                //}

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
                    if (period == UpdatePeriod.Never && method.HasForcedOnStartup())
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
                    Enum method = (Enum)combo.Tag;
                    List<UpdatePeriod> periods = GetUpdatePeriods(method);
                    combo.SelectedIndex = Math.Max(0, periods.IndexOf(m_settings.Periods[method.ToString()]));
                }
            }
        }

        /// <summary>
        /// When the selected index changes, we update the settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            Enum method = (Enum)combo.Tag;
            List<UpdatePeriod> periods = GetUpdatePeriods(method);

            if (combo.SelectedIndex < 0 || combo.SelectedIndex >= periods.Count)
                return;

            if (method.Equals(APICharacterMethods.MarketOrders))
                m_settings.Periods[APICorporationMethods.CorporationMarketOrders.ToString()] = periods[combo.SelectedIndex];

            if (method.Equals(APICharacterMethods.IndustryJobs))
                m_settings.Periods[APICorporationMethods.CorporationIndustryJobs.ToString()] = periods[combo.SelectedIndex];

            if (method.Equals(APIGenericMethods.CharacterList))
                m_settings.Periods[APIGenericMethods.APIKeyInfo.ToString()] = periods[combo.SelectedIndex];

            m_settings.Periods[method.ToString()] = periods[combo.SelectedIndex];
        }

        /// <summary>
        /// Gets the available update periods for the given method.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static List<UpdatePeriod> GetUpdatePeriods(Enum method)
        {
            List<UpdatePeriod> periods = new List<UpdatePeriod> { UpdatePeriod.Never };

            int min = (int)method.GetUpdatePeriod().Minimum;
            int max = (int)method.GetUpdatePeriod().Maximum;

            periods.AddRange(Enum.GetValues(typeof(UpdatePeriod)).Cast<UpdatePeriod>().Where(
                period => period != UpdatePeriod.Never).Select(period => new { period, index = (int)period }).Where(
                    period => period.index >= min && period.index <= max).Select(period => period.period));

            return periods;
        }
    }
}