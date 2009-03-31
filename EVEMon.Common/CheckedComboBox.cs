using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using EVEMon.Common;
using System.ComponentModel;
using System.Drawing.Design;

namespace EveMon.Common
{
    /// <summary>
    /// Code based on Stelios Alexandrakis's CheckedComboBox, see :
    /// http://www.codeproject.com/KB/combobox/checkedcombobox.aspx
    /// License : http://www.codeproject.com/info/cpol10.aspx
    /// </summary>
    public class CheckedComboBox : CustomComboBox
    {
        #region CustomCheckedListBox
        /// <summary>
        /// A custom CheckedListBox being shown within the dropdown form representing the dropdown list of the CheckedComboBox.
        /// </summary>
        internal class CustomCheckedListBox : CheckedListBox
        {
            private int curSelIndex = -1;

            public CustomCheckedListBox()
                : base()
            {
                this.SelectionMode = SelectionMode.One;
                this.HorizontalScrollbar = true;
            }

            /// <summary>
            /// Intercepts the keyboard input, [Enter] confirms a selection and [Esc] cancels it.
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
                else if (e.KeyCode == Keys.Delete)
                {
                    // Delete unckecks all, [Shift + Delete] checks all.
                    for (int i = 0; i < Items.Count; i++)
                    {
                        SetItemChecked(i, e.Shift);
                    }
                    e.Handled = true;
                }
                // If no Enter or Esc keys presses, let the base class handle it.
                base.OnKeyDown(e);
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                int index = IndexFromPoint(e.Location);
                //Debug.WriteLine("Mouse over item: " + (index >= 0 ? GetItemText(Items[index]) : "None"));
                if ((index >= 0) && (index != curSelIndex))
                {
                    curSelIndex = index;
                    SetSelected(index, true);
                }
            }

        }
        #endregion



        // The content of the popup
        private CustomCheckedListBox listBox;


        // The valueSeparator character(s) between the ticked elements as they appear in the 
        // text portion of the CheckedComboBox.
        private string valueSeparator;
        public string ValueSeparator
        {
            get { return valueSeparator; }
            set { valueSeparator = value; }
        }

        public bool CheckOnClick
        {
            get { return this.listBox.CheckOnClick; }
            set { this.listBox.CheckOnClick = value; }
        }

        public new string DisplayMember
        {
            get { return this.listBox.DisplayMember; }
            set { this.listBox.DisplayMember = value; }
        }

        public delegate string CheckedComboBoxTextBuilderDelegate(CheckedComboBox box);
        private CheckedComboBoxTextBuilderDelegate customTextBuilder;
        public CheckedComboBoxTextBuilderDelegate CustomTextBuilder
        {
            get { return customTextBuilder; }
            set { customTextBuilder = value; }
        }

        private string textForAll;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TextForAll
        {
            get { return this.textForAll; }
            set { this.textForAll = value; }
        }

        private string textForNone;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TextForNone
        {
            get { return this.textForNone; }
            set { this.textForNone = value; }
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public new CheckedListBox.ObjectCollection Items
        {
            get { return this.listBox.Items; }
        }

        public CheckedListBox.CheckedItemCollection CheckedItems
        {
            get { return this.listBox.CheckedItems; }
        }

        public CheckedListBox.CheckedIndexCollection CheckedIndices
        {
            get { return this.listBox.CheckedIndices; }
        }

        // Array holding the checked states of the items. This will be used to reverse any changes if user cancels selection.
        bool[] oldStates;

        // Event handler for when an item check state changes.
        public event ItemCheckEventHandler ItemCheck;

        /// <summary>
        /// Constructor
        /// </summary>
        public CheckedComboBox()
            : base()
        {
            // Default value separator.
            this.textForAll = "All";
            this.textForNone = "None";
            this.valueSeparator = ", ";

            // CheckOnClick style for the dropdown (NOTE: must be set after dropdown is created).
            this.CheckOnClick = true;
            this.listBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listBox_ItemCheck);
        }


        /// <summary>
        /// Create the popup's content
        /// </summary>
        /// <returns>The control to add to the popup</returns>
        protected override Control CreateContent()
        {
            this.listBox = new CustomCheckedListBox();
            this.listBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(0, 0);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(47, 15);
            this.listBox.TabIndex = 0;
            return this.listBox;
        }

        public override string GetTextValue()
        {
            if (this.listBox.CheckedItems.Count == 0) return this.textForNone;
            if (this.listBox.CheckedItems.Count == this.listBox.Items.Count) return this.textForAll;

            if (this.customTextBuilder != null)
            {
                return this.customTextBuilder(this);
            }
            else
            {
                StringBuilder sb = new StringBuilder("");
                for (int i = 0; i < this.listBox.CheckedItems.Count; i++)
                {
                    if (i != 0) sb.Append(this.valueSeparator);
                    sb.Append(this.listBox.GetItemText(this.listBox.CheckedItems[i]));
                }
                return sb.ToString();
            }
        }

        protected override void OnDropDownDeactivated(bool validate)
        {
            // Perform the actual selection and display of checked items.
            if (!validate)
            {
                // Caller cancelled selection - need to restore the checked items to their original state.
                for (int i = 0; i < this.listBox.Items.Count; i++)
                {
                    this.SetItemChecked(i, oldStates[i]);
                }
            }
            // Set the text portion equal to the string comprising all checked items (if any, otherwise empty!).
            this.Text = GetTextValue();
        }

        protected override void OnDropDownActivated()
        {
            // Make a copy of the checked state of each item, in cace caller cancels selection.
            oldStates = new bool[this.Items.Count];
            for (int i = 0; i < this.Items.Count; i++)
            {
                oldStates[i] = this.GetItemChecked(i);
            }
        }

        public bool GetItemChecked(int index)
        {
            if (index < 0 || index > Items.Count)
            {
                throw new ArgumentOutOfRangeException("index", "value out of range");
            }
            else
            {
                return this.listBox.GetItemChecked(index);
            }
        }

        public void SetItemChecked(int index, bool isChecked)
        {
            if (index < 0 || index > Items.Count)
            {
                throw new ArgumentOutOfRangeException("index", "value out of range");
            }
            else
            {
                this.listBox.SetItemChecked(index, isChecked);
                // Need to update the Text.
                this.Text = this.GetTextValue();
            }
        }

        public CheckState GetItemCheckState(int index)
        {
            if (index < 0 || index > Items.Count)
            {
                throw new ArgumentOutOfRangeException("index", "value out of range");
            }
            else
            {
                return this.listBox.GetItemCheckState(index);
            }
        }

        public void SetItemCheckState(int index, CheckState state)
        {
            if (index < 0 || index > Items.Count)
            {
                throw new ArgumentOutOfRangeException("index", "value out of range");
            }
            else
            {
                this.listBox.SetItemCheckState(index, state);
                // Need to update the Text.
                this.Text = this.GetTextValue();
            }
        }

        private bool manuallyFired;
        private void listBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!manuallyFired)
            {
                if (e.NewValue == CheckState.Checked)
                {
                    Debug.WriteLine("Checked : " + this.listBox.Items[e.Index].ToString());
                }
                else
                {
                    Debug.WriteLine("Unchecked : " + this.listBox.Items[e.Index].ToString());
                }

                // Force the update of the checkeditems
                try
                {
                    manuallyFired = true;
                    this.SetItemCheckState(e.Index, e.NewValue);
                }
                finally
                {
                    manuallyFired = false;
                }

                // Update the combobox's text
                this.Text = GetTextValue();

                if (this.ItemCheck != null)
                {
                    this.ItemCheck(sender, e);
                }
            }
        }
    }

}
