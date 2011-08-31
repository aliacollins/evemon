using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using EVEMon.Common;
using EVEMon.Controls;

using CommonProperties = EVEMon.Common.Properties;

namespace EVEMon.Accounting
{
    /// <summary>
    /// Displays a list of accounts.
    /// </summary>
    public sealed class AccountsListBox : NoFlickerListBox
    {
        private readonly List<Account> m_accounts = new List<Account>();
        private bool m_pendingUpdate;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AccountsListBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            DrawItem += OnDrawItem;
        }

        /// <summary>
        /// Gets or sets the enumeration of displayed accounts.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Account> Accounts
        {
            get { return m_accounts; }
            set
            {
                m_accounts.Clear();
                if (value != null)
                    m_accounts.AddRange(value);

                UpdateContent();
            }
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

            Account oldSelection = SelectedItem as Account;
            BeginUpdate();
            try
            {
                Items.Clear();
                foreach (Account account in EveMonClient.Accounts)
                {
                    Items.Add(account);
                    if (account == oldSelection)
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
            bool isSelected = (e.State & DrawItemState.Selected) != DrawItemState.None;
            Brush fontBrush = (isSelected ? SystemBrushes.HighlightText : SystemBrushes.ControlText);
            e.DrawBackground();

            if (e.Index < 0 || e.Index >= Items.Count)
                return;

            Account account = (Account)Items[e.Index];

            int height = ItemHeight;
            int margin = (height - 32) / 2;

            // Updates the picture and label for key level
            Image icon;
            switch (account.KeyLevel)
            {
                default:
                    icon = CommonProperties.Resources.APIKeyWrong;
                    break;
                case CredentialsLevel.Limited:
                    icon = CommonProperties.Resources.APIKeyLimited;
                    break;
                case CredentialsLevel.Full:
                    icon = CommonProperties.Resources.APIKeyFull;
                    break;
            }

            g.DrawImageUnscaled(icon, new Point(e.Bounds.Left + margin, e.Bounds.Top + margin * 2));

            // Texts drawing
            using (Font boldFont = FontFactory.GetFont(Font, FontStyle.Bold))
            {
                // Draws the texts on the upper third
                int left = e.Bounds.Left + 32 + 2 * margin;
                int top = e.Bounds.Top + margin;
                string userID = account.UserID.ToString();
                Size userIDTextSize = TextRenderer.MeasureText(g, userID, boldFont);
                g.DrawString(userID, boldFont, fontBrush, new PointF(left, top));
                g.DrawString(account.APIKey.ToLower(CultureConstants.DefaultCulture),
                             Font, fontBrush, new PointF(left + userIDTextSize.Width, top));

                using (Font middleFont = FontFactory.GetFont(Font.FontFamily, 8.0f))
                {
                    // Draw the texts on the middle third
                    top = e.Bounds.Top + height / 3 + 4;
                    string accountCreated = String.Format(CultureConstants.DefaultCulture, "Created: {0}",
                                                          (account.KeyLevel == CredentialsLevel.Full
                                                               ? account.AccountCreated.ToLocalTime().ToString()
                                                               : "Full API Key Required"));
                    g.DrawString(accountCreated, middleFont, fontBrush, new PointF(left, top));
                    left += (int)g.MeasureString(accountCreated, middleFont).Width + margin * 4;

                    string accountExpires = String.Format(CultureConstants.DefaultCulture, "Paid Until: {0}",
                                                          (account.KeyLevel == CredentialsLevel.Full
                                                               ? account.AccountExpiration.ToLocalTime().ToString()
                                                               : "Full API Key Required"));
                    g.DrawString(accountExpires, middleFont, fontBrush, new PointF(left, top));

                    using (Font smallFont = FontFactory.GetFont(Font.FontFamily, 6.5f))
                    {
                        using (Font strikeoutFont = FontFactory.GetFont(smallFont, FontStyle.Strikeout))
                        {
                            using (Font smallBoldFont = FontFactory.GetFont(smallFont, FontStyle.Bold))
                            {
                                // Draws the texts on the lower third
                                top = e.Bounds.Top + (height / 3) * 2 + 4;
                                left = e.Bounds.Left + 32 + 2 * margin;
                                bool isFirst = true;
                                List<CharacterIdentity> identities = new List<CharacterIdentity>();
                                identities.AddRange(account.CharacterIdentities
                                                        .Where(x => !account.IgnoreList.Contains(x)).ToArray().OrderBy(x => x.Name));
                                identities.AddRange(account.IgnoreList.OrderBy(x => x.Name));

                                foreach (CharacterIdentity id in identities)
                                {
                                    // Skip if no CCP character
                                    CCPCharacter ccpCharacter = id.CCPCharacter;
                                    if (ccpCharacter == null)
                                        continue;

                                    // Draws "; " between ids
                                    if (!isFirst)
                                    {
                                        g.DrawString("; ", smallFont, fontBrush, new PointF(left, top));
                                        left += (int)g.MeasureString("; ", Font).Width;
                                    }
                                    isFirst = false;

                                    // Selects font
                                    Font font = smallFont;
                                    if (account.IgnoreList.Contains(id))
                                    {
                                        font = strikeoutFont;
                                    }
                                    else if (ccpCharacter.Monitored)
                                    {
                                        font = smallBoldFont;
                                    }

                                    // Draws character's name
                                    g.DrawString(id.Name, font, fontBrush, new PointF(left, top));
                                    left += (int)g.MeasureString(id.Name, font).Width;
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
