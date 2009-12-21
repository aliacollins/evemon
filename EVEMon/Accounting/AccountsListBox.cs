using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using EVEMon.Controls;
using System.Windows.Forms;
using EVEMon.Common;
using System.Drawing;
using System.ComponentModel;

namespace EVEMon.Accounting
{
    /// <summary>
    /// Displays a list of accounts.
    /// </summary>
    public sealed class AccountsListBox : NoFlickerListBox
    {
        private const int UserIDLength = 64;

        private readonly List<Account> m_accounts = new List<Account>();
        private bool m_pendingUpdate;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AccountsListBox()
            : base()
        {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DrawItem += new System.Windows.Forms.DrawItemEventHandler(OnDrawItem);
            this.ItemHeight = 40;
        }

        /// <summary>
        /// Gets or sets the enumeration of displayed accounts.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Account> Accounts
        {
            get
            {
                foreach (var account in m_accounts) yield return account;
            }
            set
            {
                m_accounts.Clear();
                if (value != null) m_accounts.AddRange(value);
                UpdateContent();
            }
        }

        /// <summary>
        /// Updates the content.
        /// </summary>
        private void UpdateContent()
        {
            if (!this.Visible)
            {
                m_pendingUpdate = true;
                return;
            }
            m_pendingUpdate = false;

            Account oldSelection = this.SelectedItem as Account;
            this.BeginUpdate();
            try
            {
                this.Items.Clear();
                foreach (var account in EveClient.Accounts)
                {
                    this.Items.Add(account);
                    if (account == oldSelection) this.SelectedIndex = this.Items.Count - 1;
                }
            }
            finally
            {
                this.EndUpdate();
            }
        }

        /// <summary>
        /// When the visibility changes, we check whether there is an update pending.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (this.Visible && m_pendingUpdate) UpdateContent();
            base.OnVisibleChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDrawItem(object sender, DrawItemEventArgs e)
        {
            // Background
            var g = e.Graphics;
            bool isSelected = (e.State & DrawItemState.Selected) != DrawItemState.None; 
            var fontBrush = (isSelected ? SystemBrushes.HighlightText : SystemBrushes.ControlText);
            e.DrawBackground();

            if (e.Index < 0 || e.Index >= this.Items.Count)return;
            var account = (Account)this.Items[e.Index];

            int height = this.ItemHeight;
            int margin = (height - 32) / 2;

            // Updates the picture and label for key level
            Image icon;
            switch (account.KeyLevel)
            {
                default:
                    icon = Properties.Resources.APIKeyWrong;
                    break;
                case CredentialsLevel.Limited:
                    icon = Properties.Resources.APIKeyLimited;
                    break;
                case CredentialsLevel.Full:
                    icon = Properties.Resources.APIKeyFull;
                    break;
            }

            g.DrawImageUnscaled(icon, new Point(e.Bounds.Left + margin, e.Bounds.Top + margin));

            // Texts drawing
            using (var boldFont = FontFactory.GetFont(this.Font, FontStyle.Bold))
            {
                // Draws the texts on the upper half
                int left = e.Bounds.Left + 32 + 2 * margin;
                int top = e.Bounds.Top + margin;
                g.DrawString(account.UserID.ToString(), boldFont, fontBrush, new PointF(left, top));
                g.DrawString(account.APIKey.ToLower(), this.Font, fontBrush, new PointF(left + UserIDLength, top));

                // Draws the identities bounds to this account
                top = e.Bounds.Top + height / 2 + 4;
                bool isFirst = true;
                var identities = new List<CharacterIdentity>();
                identities.AddRange(account.CharacterIdentities.Where(x => !account.IgnoreList.Contains(x)).ToArray().OrderBy(x => x.Name));
                identities.AddRange(account.IgnoreList.OrderBy(x => x.Name));

                using (var smallFont = FontFactory.GetFont(this.Font.FontFamily, 6.5f, FontStyle.Regular))
                {
                    using (var strikeoutFont = FontFactory.GetFont(smallFont, FontStyle.Strikeout))
                    {
                        using (var smallBoldFont = FontFactory.GetFont(smallFont, FontStyle.Bold))
                        {
                            foreach (var id in identities)
                            {
                                // Skip if no CCP character
                                var ccpCharacter = id.CCPCharacter;
                                if (ccpCharacter == null) continue;

                                // Draws "; " between ids
                                if (!isFirst)
                                {
                                    g.DrawString("; ", smallFont, fontBrush, new PointF(left, top));
                                    left += (int)g.MeasureString("; ", this.Font).Width;
                                }
                                isFirst = false;

                                // Selects font
                                var font = smallFont;
                                if (account.IgnoreList.Contains(id)) font = strikeoutFont;
                                else if (ccpCharacter.Monitored) font = smallBoldFont;

                                // Draws character's name
                                g.DrawString(id.Name, font, fontBrush, new PointF(left, top));
                                left += (int)g.MeasureString(id.Name, font).Width;
                            }
                        }
                    }
                }
            }

            e.DrawFocusRectangle();
        }
    }
}
