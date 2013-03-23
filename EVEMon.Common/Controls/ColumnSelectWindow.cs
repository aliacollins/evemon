using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common.Collections;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common.Controls
{
    /// <summary>
    /// This form allow the user to selected and order the columns he wants to use for plans.
    /// </summary>
    public partial class ColumnSelectWindow : EVEMonForm
    {
        private readonly List<IColumnSettings> m_columns = new List<IColumnSettings>();

        /// <summary>
        /// Prevents a default instance of the <see cref="ColumnSelectWindow"/> class from being created.
        /// </summary>
        private ColumnSelectWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnSelectWindow"/> class.
        /// </summary>
        /// <param name="columns">The columns.</param>
        protected ColumnSelectWindow(IEnumerable<IColumnSettings> columns)
            :this()
        {
            // Fill the columns list
            m_columns.AddRange(columns);
        }

        /// <summary>
        /// Gets the columns settings.
        /// </summary>
        public IEnumerable<IColumnSettings> Columns
        {
            get { return m_columns; }
        }

        /// <summary>
        /// On load, rebuild the window.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateContent();
        }

        /// <summary>
        /// Updates the content.
        /// </summary>
        private void UpdateContent()
        {
            clbColumns.Items.Clear();
            foreach (int key in AllKeys)
            {
                IColumnSettings column = m_columns.First(x => x.Key == key);
                clbColumns.Items.Add(GetHeader(key), column.Visible);
            }
        }

        /// <summary>
        /// When a checkbox state changed, we update the controls list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clbColumns_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            bool isChecked = (e.NewValue == CheckState.Checked);

            // Gets the key of the modified column
            string header = (string)clbColumns.Items[e.Index];
            int key = AllKeys.First(x => GetHeader(x) == header);

            // Gets the column for this key
            IColumnSettings column = m_columns.First(x => x.Key == key);

            // Add or remove from the list
            if (column.Visible == isChecked)
                return;

            column.Visible = isChecked;
            if (!isChecked)
                return;

            m_columns.Remove(column);
            m_columns.Add(column);
        }

        /// <summary>
        /// On cancel, nothing special.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// On OK, we update the settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// On reset button... we reset (how surprising).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            m_columns.Clear();
            m_columns.AddRange(DefaultColumns);
            UpdateContent();
        }

        protected virtual string GetHeader(int key)
        {
            return String.Empty;
        }

        protected virtual IEnumerable<int> AllKeys
        {
            get { return new EmptyEnumerable<int>(); }
        }

        protected virtual IEnumerable<IColumnSettings> DefaultColumns
        {
            get { return new EmptyEnumerable<IColumnSettings>(); }
        }
    }
}