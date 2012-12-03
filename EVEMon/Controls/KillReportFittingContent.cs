using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using System.Drawing.Drawing2D;

namespace EVEMon.Controls
{
    public partial class KillReportFittingContent : UserControl
    {
        #region Fields

        private const TextFormatFlags Format = TextFormatFlags.NoPadding | TextFormatFlags.NoClipping | TextFormatFlags.NoPrefix;

        // KillReportFittingContent drawing - Region & text padding
        private const byte PadLeft = 6;
        private const byte PadRight = 7;

        // KillReportFittingContent drawing - Item
        private const byte FittingDetailHeight = 26;
        private const byte ItemImageSize = 24;

        private readonly Font m_fittingFont;
        private readonly Font m_fittingBoldFont;

        private KillLog m_killLog;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="KillReportFittingContent"/> class.
        /// </summary>
        public KillReportFittingContent()
        {
            InitializeComponent();

            FittingContentListBox.Visible = false;
            SaveFittingButton.Visible = false;
            ColorKeyGroupBox.Visible = false;

            m_fittingFont = FontFactory.GetFont("Tahoma", 8.25F);
            m_fittingBoldFont = FontFactory.GetFont("Tahoma", 8.25F, FontStyle.Bold);
            noItemsLabel.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the kill log.
        /// </summary>
        /// <value>
        /// The kill log.
        /// </value>
        internal KillLog KillLog
        {
            get { return m_killLog; }
            set
            {
                m_killLog = value;
                UpdateContent();
            }
        }

        #endregion


        #region Content Management

        /// <summary>
        /// Updates the content.
        /// </summary>
        private void UpdateContent()
        {
            // Update the standings list
            FittingContentListBox.BeginUpdate();
            try
            {
                IEnumerable<KillLogItem> items = m_killLog.Items;
                IEnumerable<IGrouping<KillLogFittingContentGroup, KillLogItem>> groups = items
                    .GroupBy(item => item.FittingContentGroup).OrderBy(x => x .Key);

                // Scroll through groups
                FittingContentListBox.Items.Clear();
                foreach (IGrouping<KillLogFittingContentGroup, KillLogItem> group in groups)
                {
                    FittingContentListBox.Items.Add(group.Key.GetDescription());

                    foreach (KillLogItem item in group)
                    {
                        bool eventHandlerAdded = false;

                        // Add if the item was destroyed
                        if (item.QtyDestroyed > 0)
                        {
                            FittingContentListBox.Items.Add(item);
                            item.KillLogItemImageUpdated += item_KillLogItemImageUpdated;
                            eventHandlerAdded = true;
                        }

                        // Re-add if the item was also dropped
                        if (item.QtyDropped <= 0)
                            continue;

                        FittingContentListBox.Items.Add(item);
                        if (!eventHandlerAdded)
                            item.KillLogItemImageUpdated += item_KillLogItemImageUpdated;
                    }
                }

                // Display or hide the "no standings" label.
                noItemsLabel.Visible = !items.Any();
                FittingContentListBox.Visible = items.Any();


                // Invalidate display
                FittingContentListBox.Invalidate();
            }
            finally
            {
                FittingContentListBox.EndUpdate();
            }

        }

        #endregion


        #region Drawing

        /// <summary>
        /// Handles the MeasureItem event of the FittingContentListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MeasureItemEventArgs"/> instance containing the event data.</param>
        private void FittingContentListBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index >= 0)
                e.ItemHeight = FittingDetailHeight;
        }

        /// <summary>
        /// Handles the DrawItem event of the FittingContentListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DrawItemEventArgs"/> instance containing the event data.</param>
        private void FittingContentListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= FittingContentListBox.Items.Count)
                return;

            object listItem = FittingContentListBox.Items[e.Index];
            KillLogItem item = listItem as KillLogItem;
            if (item != null)
            {
                // If item is the same with previous item then we previously have drawn a destroyed item
                // and now we need to draw a dropped one
                if (FittingContentListBox.Items[e.Index - 1] == item)
                    DrawItem(item, e, true);
                else
                {
                    // Draw a destroyed item or dropped one
                    if (item.QtyDestroyed > 0)
                        DrawItem(item, e);
                    else
                        DrawItem(item, e, true);
                }
            }
            else
                DrawItem((string)listItem, e);
        }

        /// <summary>
        /// Draws the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DrawItemEventArgs"/> instance containing the event data.</param>
        /// <param name="itemIsDropped">if set to <c>true</c> item is dropped.</param>
        private void DrawItem(KillLogItem item, DrawItemEventArgs e, bool itemIsDropped = false)
        {
            Graphics g = e.Graphics;

            // Draw background
            g.FillRectangle(itemIsDropped ? Brushes.DarkGreen : Brushes.LightGray, e.Bounds);

            int itemQty = itemIsDropped ? item.QtyDropped : item.QtyDestroyed;

            Size itemTextSize = TextRenderer.MeasureText(g, item.Name, m_fittingFont, Size.Empty, Format);
            Size itemQtyTextSize = TextRenderer.MeasureText(g, itemQty.ToNumericString(0), m_fittingFont);

            Rectangle itemTextRect = new Rectangle(e.Bounds.Left + PadLeft * 2 + ItemImageSize,
                                                   e.Bounds.Top + ((e.Bounds.Height - itemTextSize.Height) / 2),
                                                   itemTextSize.Width + PadRight, itemTextSize.Height);
            Rectangle itemQtyTextRect = new Rectangle(e.Bounds.Right - itemQtyTextSize.Width - PadRight,
                                                      e.Bounds.Top + ((e.Bounds.Height - itemTextSize.Height) / 2),
                                                      itemQtyTextSize.Width + PadRight, itemQtyTextSize.Height);

            TextRenderer.DrawText(g, item.Name, m_fittingFont, itemTextRect, Color.Black);
            TextRenderer.DrawText(g, itemQty.ToNumericString(0), m_fittingFont, itemQtyTextRect, Color.Black);

            if (Settings.UI.SafeForWork)
                return;

            g.DrawImage(item.ItemImage, new Rectangle(e.Bounds.Left + PadLeft * 2,
                                                      e.Bounds.Top + ((FittingDetailHeight - ItemImageSize) / 2),
                                                      24, 24));
        }

        /// <summary>
        /// Draws the list item for the given group.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="e"></param>
        private void DrawItem(string group, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;

            using (Brush brush = Settings.UI.SafeForWork
                                     ? new SolidBrush(Color.FromArgb(75, 75, 75))
                                     : (Brush)new LinearGradientBrush(new PointF(0F, 0F), new PointF(0F, FittingDetailHeight),
                                                                      Color.FromArgb(75, 75, 75), Color.FromArgb(25, 25, 25)))
            {
                g.FillRectangle(brush, e.Bounds);
            }

            using (Pen pen = new Pen(Color.FromArgb(100, 100, 100)))
            {
                g.DrawLine(pen, e.Bounds.Left, e.Bounds.Top, e.Bounds.Right + 1, e.Bounds.Top);
            }

            Size fittingGroupTextSize = TextRenderer.MeasureText(g, group, m_fittingBoldFont, Size.Empty, Format);
            Rectangle fittingGroupTextRect = new Rectangle(e.Bounds.Left + PadLeft,
                                                                             e.Bounds.Top +
                                                                             ((e.Bounds.Height - fittingGroupTextSize.Height) / 2),
                                                                             fittingGroupTextSize.Width + PadRight,
                                                                             fittingGroupTextSize.Height);

            TextRenderer.DrawText(g, group, m_fittingBoldFont, fittingGroupTextRect, Color.White);
        }


        #endregion


        #region Local Events

        /// <summary>
        /// Handles the MouseWheel event of the FittingContentListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void FittingContentListBox_MouseWheel(object sender, MouseEventArgs e)
        {
            // Update the drawing based upon the mouse wheel scrolling
            int numberOfItemLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
            int lines = numberOfItemLinesToMove;
            if (lines == 0)
                return;

            // Compute the number of lines to move
            int direction = lines / Math.Abs(lines);
            int[] numberOfPixelsToMove = new int[lines * direction];
            for (int i = 1; i <= Math.Abs(lines); i++)
            {
                object item = null;

                // Going up
                if (direction == Math.Abs(direction))
                {
                    // Retrieve the next top item
                    if (FittingContentListBox.TopIndex - i >= 0)
                        item = FittingContentListBox.Items[FittingContentListBox.TopIndex - i];
                }
                    // Going down
                else
                {
                    // Compute the height of the items from current the topindex (included)
                    int height = 0;
                    for (int j = FittingContentListBox.TopIndex + i - 1; j < FittingContentListBox.Items.Count; j++)
                    {
                        height += FittingDetailHeight;
                    }

                    // Retrieve the next bottom item
                    if (height > FittingContentListBox.ClientSize.Height)
                        item = FittingContentListBox.Items[FittingContentListBox.TopIndex + i - 1];
                }

                // If found a new item as top or bottom
                if (item != null)
                    numberOfPixelsToMove[i - 1] = FittingDetailHeight * direction;
                else
                    lines -= direction;
            }

            // Scroll 
            if (lines != 0)
                FittingContentListBox.Invalidate();
        }

        /// <summary>
        /// Handles the KillLogItemImageUpdated event of the item control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void item_KillLogItemImageUpdated(object sender, EventArgs e)
        {
            // Force to redraw
            FittingContentListBox.Invalidate();
        }

        /// <summary>
        /// Handles the Resize event of the FittingContentListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void FittingContentListBox_Resize(object sender, EventArgs e)
        {
            FittingContentListBox.Invalidate();
        }

        /// <summary>
        /// Handles the Click event of the ToggleColorKeyPictureBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ToggleColorKeyPictureBox_Click(object sender, EventArgs e)
        {
            ColorKeyGroupBox.Visible = !ColorKeyGroupBox.Visible;
        }

        #endregion
    }
}
