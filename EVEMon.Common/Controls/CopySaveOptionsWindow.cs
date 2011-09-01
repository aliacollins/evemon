using System;
using System.Windows.Forms;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common.Controls
{
    public partial class CopySaveOptionsWindow : EVEMonForm
    {
        private readonly PlanExportSettings m_planTextOptions;
        private readonly Plan m_plan;
        private readonly bool m_isForCopy;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopySaveOptionsWindow"/> class.
        /// </summary>
        private CopySaveOptionsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CopySaveOptionsWindow"/> class.
        /// </summary>
        /// <param name="pto">The pto.</param>
        /// <param name="p">The p.</param>
        /// <param name="isForCopy">if set to <c>true</c> [is for copy].</param>
        public CopySaveOptionsWindow(PlanExportSettings pto, Plan p, bool isForCopy)
            : this()
        {
            m_planTextOptions = pto;
            m_plan = p;
            m_isForCopy = isForCopy;
        }

        private void CopySaveOptionsWindow_Load(object sender, EventArgs e)
        {
            Text = m_isForCopy ? "Copy Options" : "Save Options";

            cbIncludeHeader.Checked = m_planTextOptions.IncludeHeader;
            cbEntryNumber.Checked = m_planTextOptions.EntryNumber;
            cbEntryTrainingTimes.Checked = m_planTextOptions.EntryTrainingTimes;
            cbEntryCost.Checked = m_planTextOptions.EntryCost;
            cbEntryStartDate.Checked = m_planTextOptions.EntryStartDate;
            cbEntryFinishDate.Checked = m_planTextOptions.EntryFinishDate;
            cbFooterCount.Checked = m_planTextOptions.FooterCount;
            cbFooterTotalTime.Checked = m_planTextOptions.FooterTotalTime;
            cbFooterDate.Checked = m_planTextOptions.FooterDate;
            cbFooterCost.Checked = m_planTextOptions.FooterCost;
            cbShoppingList.Checked = m_planTextOptions.ShoppingList;
            cmbFormatting.SelectedIndex = m_planTextOptions.Markup == MarkupType.Forum
                                              ? 0
                                              : m_planTextOptions.Markup == MarkupType.Html ? 1 : 2;

            RecurseUnder(this);
            OptionChange();
        }

        /// <summary>
        /// Gets a value indicating whether [set as default].
        /// </summary>
        /// <value><c>true</c> if [set as default]; otherwise, <c>false</c>.</value>
        public bool SetAsDefault { get; private set; }

        /// <summary>
        /// Recurses the under.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void RecurseUnder(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is CheckBox && c != cbRememberOptions)
                {
                    CheckBox cb = (CheckBox)c;
                    cb.CheckedChanged += cb_CheckedChanged;
                }
                RecurseUnder(c);
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the cb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cb_CheckedChanged(object sender, EventArgs e)
        {
            OptionChange();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the comboBox1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            OptionChange();
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Updates the options.
        /// </summary>
        private void UpdateOptions()
        {
            m_planTextOptions.IncludeHeader = cbIncludeHeader.Checked;
            m_planTextOptions.EntryNumber = cbEntryNumber.Checked;
            m_planTextOptions.EntryTrainingTimes = cbEntryTrainingTimes.Checked;
            m_planTextOptions.EntryStartDate = cbEntryStartDate.Checked;
            m_planTextOptions.EntryFinishDate = cbEntryFinishDate.Checked;
            m_planTextOptions.EntryCost = cbEntryCost.Checked;
            m_planTextOptions.FooterCount = cbFooterCount.Checked;
            m_planTextOptions.FooterTotalTime = cbFooterTotalTime.Checked;
            m_planTextOptions.FooterCost = cbFooterCost.Checked;
            m_planTextOptions.FooterDate = cbFooterDate.Checked;
            m_planTextOptions.ShoppingList = cbShoppingList.Checked;
            m_planTextOptions.Markup = cmbFormatting.SelectedIndex == 0
                                           ? MarkupType.Forum
                                           : cmbFormatting.SelectedIndex == 1 ? MarkupType.Html : MarkupType.None;
        }

        /// <summary>
        /// Handles the Click event of the btnOk control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            UpdateOptions();

            SetAsDefault = cbRememberOptions.Checked;
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Options the change.
        /// </summary>
        private void OptionChange()
        {
            UpdateOptions();
            tbPreview.Text = PlanExporter.ExportAsText(m_plan, m_planTextOptions);
        }
    }
}