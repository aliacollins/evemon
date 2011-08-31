using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace EVEMon.Common.Controls
{
    public delegate void DropDownMouseMoveHandler(Object sender, object item, Point point);

    /// <summary>
    /// A combobox which notified subscribers about which items are under the mouse and when this changes.
    /// </summary>
    /// <remarks>
    /// Pay attention to call this type explicitly and not use it as a cast <see cref="ComboBox"/>. 
    /// Indeed, the <see cref="Items"/>, <see cref="SelectedItem"/> and <see cref="SelectedIndex"/> properties are not overriden, 
    /// they're declared with <c>new</c>.
    /// </remarks>
    public sealed class DropDownMouseMoveComboBox : CustomComboBox
    {
        #region Private Class 'CustomListBox'

        /// <summary>
        /// A custom <see cref="ListBox"/> being shown within the dropdown form representing the dropdown list.
        /// </summary>
        private sealed class CustomListBox : ListBox
        {
            public event DropDownMouseMoveHandler DropDownMouseMove;

            /// <summary>
            /// Constructor.
            /// </summary>
            public CustomListBox()
            {
                SelectionMode = SelectionMode.One;
                HorizontalScrollbar = true;
                IntegralHeight = false;
            }

            /// <summary>
            /// Intercepts the keyboard input, [Enter] and [Esc] confirms a selection.
            /// </summary>
            /// <param name="e">The Key event arguments</param>
            protected override void OnKeyDown(KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
                {
                    // Enact selection.
                    ((Dropdown)Parent).ForceDeactivate(new CustomComboBoxEventArgs(true));
                    e.Handled = true;

                }
                // If no Enter or Esc keys presses, let the base class handle it.
                base.OnKeyDown(e);
            }

            /// <summary>
            /// On mouse move, we notify the subscribers of <see cref="DropDownMouseMove"/>.
            /// </summary>
            /// <param name="e"></param>
            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                int index = IndexFromPoint(e.Location);
                if (index < 0)
                    return;

                if (DropDownMouseMove != null)
                    DropDownMouseMove(this, Items[index], e.Location);
            }

            /// <summary>
            /// On click, closes the dropdown.
            /// </summary>
            /// <param name="e"></param>
            protected override void OnClick(EventArgs e)
            {
                // Enact selection.
                ((Dropdown)Parent).ForceDeactivate(new CustomComboBoxEventArgs(true));
                base.OnClick(e);
            }
        }

        #endregion


        public event DropDownMouseMoveHandler DropDownMouseMove;
        private CustomListBox m_listBox;
        private string m_displayText;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DropDownMouseMoveComboBox()
        {
            // Default value separator.
            Cursor = Cursors.Default;
            DropDownStyle = ComboBoxStyle.DropDownList;
            DrawMode = DrawMode.OwnerDrawFixed;
            DrawItem += ToolTipComboBox_DrawItem;

            // CheckOnClick style for the dropdown (NOTE: must be set after dropdown is created).
            m_listBox.DropDownMouseMove += listBox_DropDownMouseMove;
            m_listBox.SelectedIndexChanged += listBox_SelectedIndexChanged;
        }

        /// <summary>
        /// Create the popup's content
        /// </summary>
        /// <returns>The control to add to the popup</returns>
        protected override Control CreateContent()
        {
            m_listBox = new CustomListBox
                            {
                                BorderStyle = BorderStyle.None,
                                Dock = DockStyle.Fill,
                                FormattingEnabled = true,
                                Location = new Point(0, 0),
                                Name = "m_listBox",
                                Size = new Size(47, 15),
                                TabIndex = 0
                            };
            return m_listBox;
        }

        /// <summary>
        /// Gets the list of items.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(
            "System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
            , typeof(UITypeEditor))]
        public new ListBox.ObjectCollection Items
        {
            get { return m_listBox.Items; }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        [Browsable(false)]
        public new Object SelectedItem
        {
            get { return m_listBox.SelectedItem; }
            set { m_listBox.SelectedItem = value; }
        }

        /// <summary>
        /// Gets or sets the selected index.
        /// </summary>
        [Browsable(false)]
        public new int SelectedIndex
        {
            get { return m_listBox.SelectedIndex; }
            set { m_listBox.SelectedIndex = value; }
        }

        /// <summary>
        /// On selection, updates the list box selection.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectionChangeCommitted(EventArgs e)
        {
            m_listBox.SelectedItem = SelectedItem;
            base.OnSelectionChangeCommitted(e);
        }

        /// <summary>
        /// Gets the text to display.
        /// </summary>
        /// <returns></returns>
        public override string GetTextValue()
        {
            return m_listBox.SelectedItem == null ? String.Empty : m_listBox.SelectedItem.ToString();
        }

        /// <summary>
        /// When the drop down is closed, we hide the tooltip.
        /// </summary>
        /// <param name="validate"></param>
        protected override void OnDropDownDeactivated(bool validate)
        {
            // Set the text portion equal to the string comprising all checked items (if any, otherwise empty!).
            m_displayText = GetTextValue();
            Invalidate();
        }

        /// <summary>
        /// On drop down activation. Nothing to do but the base method was abstract.
        /// </summary>
        protected override void OnDropDownActivated()
        {
        }

        /// <summary>
        /// When the mouse moves over a drop down item, we fire the event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="item"></param>
        /// <param name="point"></param>
        private void listBox_DropDownMouseMove(object sender, object item, Point point)
        {
            if (DropDownMouseMove != null)
                DropDownMouseMove(sender, item, point);
        }

        /// <summary>
        /// When the list box selection changes, we update the combo text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_displayText = GetTextValue();
            Invalidate();
        }

        /// <summary>
        /// Draws the item that appears on the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolTipComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Background
            using (Brush backBrush = new SolidBrush(BackColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            // Display text
            if (!String.IsNullOrEmpty(m_displayText))
            {
                using (Brush foreBrush = new SolidBrush(ForeColor))
                {
                    const float Offset = 3.0f;
                    SizeF size = e.Graphics.MeasureString(m_displayText, Font);
                    RectangleF rect = new RectangleF(Offset, (Bounds.Height - size.Height) * 0.5f, e.Bounds.Width - Offset, size.Height);
                    e.Graphics.DrawString(m_displayText, Font, foreBrush, rect, StringFormat.GenericTypographic);
                }
            }

            // Focus rect
            e.DrawFocusRectangle();
        }
    }
}
