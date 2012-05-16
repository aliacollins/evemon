using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.Properties;

namespace EVEMon.ApiCredentialsManagement
{
    /// <summary>
    /// Displays a list of API keys.
    /// </summary>
    public sealed class ApiKeysListBox : NoFlickerListBox
    {
        private readonly Font m_smallFont;
        private readonly Font m_smallBoldFont;
        private readonly Font m_strikeoutFont;
        private readonly Font m_middleFont;
        private readonly Font m_boldFont;

        private readonly List<APIKey> m_apiKeys = new List<APIKey>();
        private bool m_pendingUpdate;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ApiKeysListBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            DrawItem += OnDrawItem;

            m_smallFont = FontFactory.GetFont(Font.FontFamily, 6.5f);
            m_smallBoldFont = FontFactory.GetFont(m_smallFont, FontStyle.Bold);
            m_strikeoutFont = FontFactory.GetFont(m_smallFont, FontStyle.Strikeout);
            m_middleFont = FontFactory.GetFont(Font.FontFamily, 8.0f);
            m_boldFont = FontFactory.GetFont(Font, FontStyle.Bold);

            ItemHeight = m_boldFont.Height + m_middleFont.Height + m_smallBoldFont.Height * 2;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"/> 
        /// and its child controls and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            DrawItem -= OnDrawItem;
            base.Dispose(disposing);
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

            int scrollBarPosition = TopIndex;
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
                TopIndex = scrollBarPosition;
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
        /// Draws the API key info.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DrawItemEventArgs"/> instance containing the event data.</param>
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
            Image icon = GetIcon(apiKey);

            // Associate account info for corporation type API key
            // if related info exist in another API key of the binded character
            AssociateAccountInfo(apiKey);

            Margin = new Padding((ItemHeight - icon.Height) / 2);
            int left = e.Bounds.Left + Margin.Left;
            int top = e.Bounds.Top + Margin.Top / 4;

            // Draws the checbox
            CheckBoxRenderer.DrawCheckBox(g, new Point(left, (ItemHeight - CheckBoxSize.Height) / 2),
                                          apiKey.Monitored ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal);
            left += CheckBoxSize.Width + Margin.Left * 2;

            // Draws the picture of the API key type
            g.DrawImage(icon, new Rectangle(left, top + Margin.Top, icon.Width, icon.Height));

            // Texts drawing
            DrawTexts(g, apiKey, left, top, fontBrush, icon);

            e.DrawFocusRectangle();
        }

        /// <summary>
        /// Draws the texts.
        /// </summary>
        /// <param name="top">The top.</param>
        /// <param name="g">The g.</param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="left">The left.</param>
        /// <param name="fontBrush">The font brush.</param>
        /// <param name="icon">The icon.</param>
        private void DrawTexts(Graphics g, APIKey apiKey, int left, int top, Brush fontBrush, Image icon)
        {
            // Draws the texts on the upper third
            left += icon.Width + Margin.Left;
            string apiKeyId = apiKey.ID.ToString(CultureConstants.DefaultCulture);
            g.DrawString(apiKeyId, m_boldFont, fontBrush, new Point(left, top + 2));
            int indentedLeft = left + g.MeasureString(apiKeyId, m_boldFont).ToSize().Width + Margin.Left;

            g.DrawString(apiKey.VerificationCode, Font, fontBrush, new Point(indentedLeft, top));
            indentedLeft += g.MeasureString(apiKey.VerificationCode, Font).ToSize().Width + Margin.Left * 4;

            string apiKeyExpiration = String.Format(CultureConstants.DefaultCulture, "Expires: {0}",
                                                    (apiKey.Expiration != DateTime.MinValue
                                                         ? apiKey.Expiration.ToLocalTime().ToString()
                                                         : "Never"));
            g.DrawString(apiKeyExpiration, Font, fontBrush, new Point(indentedLeft, top));

            // Draw the texts on the middle third
            top += g.MeasureString(apiKey.VerificationCode, Font).ToSize().Height;
            string accountCreated = String.Format(CultureConstants.DefaultCulture, "Account Created: {0}",
                                                  (apiKey.AccountCreated != DateTime.MinValue
                                                       ? apiKey.AccountCreated.ToLocalTime().ToString()
                                                       : "-"));
            g.DrawString(accountCreated, m_middleFont, fontBrush, new Point(left, top));
            indentedLeft = left + g.MeasureString(accountCreated, m_middleFont).ToSize().Width + Margin.Left * 4;

            string accountExpires = String.Format(CultureConstants.DefaultCulture, "Account Paid Until: {0}",
                                                  (apiKey.AccountExpires != DateTime.MinValue
                                                       ? apiKey.AccountExpires.ToLocalTime().ToString()
                                                       : "-"));
            g.DrawString(accountExpires, m_middleFont, fontBrush, new Point(indentedLeft, top));

            // Draws the texts on the lower third
            top += g.MeasureString(accountCreated, m_middleFont).ToSize().Height;
            bool isFirst = true;

            foreach (CharacterIdentity identity in apiKey.CharacterIdentities)
            {
                // Draws "; " between ids
                if (!isFirst)
                {
                    g.DrawString("; ", m_smallFont, fontBrush, new Point(left, top));
                    left += g.MeasureString("; ", Font).ToSize().Width;
                }
                isFirst = false;

                // Selects font
                Font font = m_smallFont;
                CCPCharacter ccpCharacter = identity.CCPCharacter;
                if (apiKey.IdentityIgnoreList.Contains(identity))
                    font = m_strikeoutFont;
                else if (ccpCharacter != null && ccpCharacter.Monitored)
                    font = m_smallBoldFont;

                // Draws character's name
                g.DrawString(identity.CharacterName, font, fontBrush, new Point(left, top));
                left += g.MeasureString(identity.CharacterName, font).ToSize().Width;
            }
        }

        /// <summary>
        /// Associates the account info.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        private static void AssociateAccountInfo(APIKey apiKey)
        {
            foreach (APIKey apiKeyWithAccountStatusInfo in EveMonClient.CharacterIdentities.Where(
                id => id.APIKeys.Contains(apiKey)).Select(id => id.APIKeys.FirstOrDefault(
                    apikey => apikey.AccountCreated != DateTime.MinValue)).Where(
                        apiKeyWithAccountStatusInfo => apiKeyWithAccountStatusInfo != null))
            {
                apiKey.AccountCreated = apiKeyWithAccountStatusInfo.AccountCreated;
                apiKey.AccountExpires = apiKeyWithAccountStatusInfo.AccountExpires;
            }
        }

        /// <summary>
        /// Gets the icon.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <returns></returns>
        private static Image GetIcon(APIKey apiKey)
        {
            Image icon;
            switch (apiKey.Type)
            {
                default:
                    icon = Resources.KeyWrong32;
                    break;
                case APIKeyType.Character:
                    icon = Resources.DefaultCharacterImage32;
                    break;
                case APIKeyType.Corporation:
                    icon = Resources.DefaultCorporationImage32;
                    break;
                case APIKeyType.Account:
                    icon = Resources.AccountWide32;
                    break;
            }
            return icon;
        }
    }
}