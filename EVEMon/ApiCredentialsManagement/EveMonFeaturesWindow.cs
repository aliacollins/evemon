using System;
using EVEMon.Common;
using EVEMon.Common.Controls;

namespace EVEMon.ApiCredentialsManagement
{
    public partial class EVEMonFeaturesWindow : EVEMonForm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EVEMonFeaturesWindow"/> class.
        /// </summary>
        public EVEMonFeaturesWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the LinkClicked event of the CreateBasicAPIKeyLinkLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void CreateBasicAPIKeyLinkLabel_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            Util.OpenURL(String.Format(NetworkConstants.APICredentialsPredefined, (int)APIMethods.BasicFeatures));
        }

        /// <summary>
        /// Handles the LinkClicked event of the CreateAdvancedAPIKeyLinkLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void CreateAllFeaturesAPIKeyLinkLabel_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            Util.OpenURL(String.Format(NetworkConstants.APICredentialsPredefined, (int)APIMethods.AllFeatures));
        }
    }
}