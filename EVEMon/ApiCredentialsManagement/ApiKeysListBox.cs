using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using EVEMon.Common;
using EVEMon.Common.Controls;
using CommonProperties = EVEMon.Common.Properties;

namespace EVEMon.ApiCredentialsManagement
{
    /// <summary>
    /// Displays a list of API keys.
    /// </summary>
    public sealed class ApiKeysListBox : NoFlickerListBox
    {
        private readonly List<APIKey> m_apiKeys = new List<APIKey>();
        private bool m_pendingUpdate;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ApiKeysListBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            DrawItem += OnDrawItem;
        }

        /// <summary>
        /// Gets or sets the enumeration of displayed API keys.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IEnumerable<APIKey> APIKeys
        {
            get { return m_apiKeys; }
            set
            {
                m_apiKeys.Clear();
                if (value != null)
                    m_apiKeys.AddRange(value);

                UpdateContent();
            }
        }

        /// <summary>
        /// Gets the size of the check box.
        /// </summary>
        /// <value>The size of the check box.</value>
        internal static Size CheckBoxSize
        {
            get { return new Size(12, 12); }
        }

        /// <summary>
        /// Updates the content.
        /// </summary>
        private void UpdateContent()
        {
            if (!Visible)
            {
                m_pendingUpdate = true;
                return;
            }
            m_pendingUpdate = false;

            APIKey oldSelection = SelectedItem as APIKey;
            BeginUpdate();
            try
            {
                Items.Clear();
                foreach (APIKey apiKey in APIKeys)
                {
                    Items.Add(apiKey);
                    if (apiKey == oldSelection)
                        SelectedIndex = Items.Count - 1;
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <summary>
        /// When the visibility changes, we check whether there is an update pending.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Visible && m_pendingUpdate)
                UpdateContent();

            base.OnVisibleChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDrawItem(object sender, DrawItemEventArgs e)
        {
            // Background
            Graphics g = e.Graphics;
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Brush fontBrush = (isSelected ? SystemBrushes.HighlightText : SystemBrushes.ControlText);
            e.DrawBackground();

            if (e.Index < 0 || e.Index >= Items.Count)
                return;

            APIKey apiKey = (APIKey)Items[e.Index];

            Image icon;
            switch (apiKey.Type)
            {
                default:
                    icon = CommonProperties.Resources.KeyWrong32;
                    break;
                case APIKeyType.Character:
                    icon = CommonProperties.Resources.DefaultCharacterImage32;
                    break;
                case APIKeyType.Corporation:
                    icon = CommonProperties.Resources.DefaultCorporationImage32;
                    break;
                case APIKeyType.Account:
                    icon = CommonProperties.Resources.AccountWide32;
                    break;
            }

            // Associate account info for corporation type API key
            // if related info exist in another API key of the binded character
            foreach (APIKey apiKeyWithAccountStatusInfo in EveMonClient.CharacterIdentities.Where(
                id => id.APIKeys.Contains(apiKey)).Select(id => id.APIKeys.FirstOrDefault(
                    apikey => apikey.AccountCreated != DateTime.MinValue)).Where(
                    apiKeyWithAccountStatusInfo => apiKeyWithAccountStatusInfo != null))
            {
                apiKey.AccountCreated = apiKeyWithAccountStatusInfo.AccountCreated;
                apiKey.AccountExpires = apiKeyWithAccountStatusInfo.AccountExpires;
            }

            Margin = new Padding((ItemHeight - icon.Height) / 2);
            int left = e.Bounds.Left + Margin.Left;
            int top = e.Bounds.Top;

            // Draws the checbox
            CheckBoxRenderer.DrawCheckBox(g, new Point(left, (ItemHeight - CheckBoxSize.Height) / 2),
                                          apiKey.Monitored ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal);
            left += CheckBoxSize.Width + Margin.Left * 2;

            // Draws the picture of the API key type
            g.DrawImage(icon, new Rectangle(left, top + Margin.Top, icon.Width, icon.Height));

            // Texts drawing
            using (Font boldFont = FontFactory.GetFont(Font, FontStyle.Bold))
            {
                // Draws the texts on the upper third
                left += icon.Width + Margin.Left;
                string apiKeyId = apiKey.ID.ToString(CultureConstants.DefaultCulture);
                g.DrawString(apiKeyId, boldFont, fontBrush, new PointF(left, top));
                int indentedLeft = left + (int)g.MeasureString(apiKeyId, boldFont).Width + Margin.Left;

                g.DrawString(apiKey.VerificationCode, Font, fontBrush, new PointF(indentedLeft, top));
                indentedLeft += (int)g.MeasureString(apiKey.VerificationCode, Font).Width + Margin.Left * 4;

                string apiKeyExpiration = String.Format(CultureConstants.DefaultCulture, "Expires: {0}",
                                                        (apiKey.Expiration != DateTime.MinValue
                                                             ? apiKey.Expiration.ToLocalTime().ToString()
                                                             : "Never"));
                g.DrawString(apiKeyExpiration, Font, fontBrush, new PointF(indentedLeft, top));

                using (Font middleFont = FontFactory.GetFont(Font.FontFamily, 8.0f))
                {
                    // Draw the texts on the middle third
                    top = ItemHeight / 3;
                    string accountCreated = String.Format(CultureConstants.DefaultCulture, "Account Created: {0}",
                                                          (apiKey.AccountCreated != DateTime.MinValue
                                                               ? apiKey.AccountCreated.ToLocalTime().ToString()
                                                               : "-"));
                    g.DrawString(accountCreated, middleFont, fontBrush, new PointF(left, top));
                    indentedLeft = left + (int)g.MeasureString(accountCreated, middleFont).Width + Margin.Left * 4;

                    string accountExpires = String.Format(CultureConstants.DefaultCulture, "Account Paid Until: {0}",
                                                          (apiKey.AccountExpires != DateTime.MinValue
                                                               ? apiKey.AccountExpires.ToLocalTime().ToString()
                                                               : "-"));
                    g.DrawString(accountExpires, middleFont, fontBrush, new PointF(indentedLeft, top));

                    using (Font smallFont = FontFactory.GetFont(Font.FontFamily, 6.5f))
                    {
                        using (Font strikeoutFont = FontFactory.GetFont(smallFont, FontStyle.Strikeout))
                        {
                            using (Font smallBoldFont = FontFactory.GetFont(smallFont, FontStyle.Bold))
                            {
                                // Draws the texts on the lower third
                                top *= 2;
                                bool isFirst = true;

                                foreach (CharacterIdentity identity in apiKey.CharacterIdentities)
                                {
                                    // Draws "; " between ids
                                    if (!isFirst)
                                    {
                                        g.DrawString("; ", smallFont, fontBrush, new PointF(left, top));
                                        left += (int)g.MeasureString("; ", Font).Width;
                                    }
                                    isFirst = false;

                                    // Selects font
                                    Font font = smallFont;
                                    CCPCharacter ccpCharacter = identity.CCPCharacter;
                                    if (apiKey.IdentityIgnoreList.Contains(identity))
                                        font = strikeoutFont;
                                    else if (ccpCharacter != null && ccpCharacter.Monitored)
                                        font = smallBoldFont;

                                    // Draws character's name
                                    g.DrawString(identity.CharacterName, font, fontBrush, new PointF(left, top));
                                    left += (int)g.MeasureString(identity.CharacterName, font).Width;
                                }
                            }
                        }
                    }
                }
            }

            e.DrawFocusRectangle();
        }
    }
}