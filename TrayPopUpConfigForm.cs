using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using System.Globalization;

namespace EVEMon
{
    /// <summary>
    /// Configuration editor for the tray icon popup window
    /// </summary>
    public partial class TrayPopupConfigForm : EVEMonForm
    {
        private TrayPopupConfig m_config = null;
        private string[] m_characterGrouping = { "None", "Training / Not Training", "Not Training / Training", "Account" };
        private string[] m_sortOrder = { "Training completion, earliest at bottom",
                                         "Training completion, earliest at top",
                                         "Alphabetical, first at top",
                                         "Alphabetical, first at bottom"};
        private string[] m_portraitSize = { "16 by 16", "24 by 24", "32 by 32", "40 by 40", "48 by 48", "56 by 56", "64 by 64" };

        public TrayPopupConfig Config
        {
            get { return m_config; }
            set { m_config = value; }
        }

        public TrayPopupConfigForm() 
        {
            InitializeComponent();
            cbGroupBy.Items.AddRange(m_characterGrouping);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (m_config != null)
            {
                DisplayConfig(m_config);
            }
        }

        private void DisplayConfig(TrayPopupConfig config)
        {
            cbHideNotTraining.Checked = config.HideCharNotTraining;
            cbGroupBy.SelectedIndex = (int)config.GroupBy;
            UpdateDisplayOrders();
            if (cbHideNotTraining.Checked)
            {
                cbDisplayOrder1.SelectedItem = m_sortOrder[(int)config.SortOrder1];
            }
            else
            {
                if (config.GroupBy == TrayPopupConfig.CharacterGroupings.None)
                {
                    cbDisplayOrder1.SelectedItem = m_sortOrder[(int)config.SortOrder1];
                }
                else if (config.GroupBy == TrayPopupConfig.CharacterGroupings.Account)
                {
                    cbDisplayOrder1.SelectedItem = m_sortOrder[(int)config.SortOrder1];
                }
                else
                {
                    cbDisplayOrder1.SelectedItem = m_sortOrder[(int)config.SortOrder1];
                    cbDisplayOrder2.SelectedItem = m_sortOrder[(int)config.SortOrder2];
                }
            }
            cbShowSkill.Checked = config.ShowSkill;
            cbShowTimeToCompletion.Checked = config.ShowTimeToCompletion;
            cbShowCompletionTime.Checked = config.ShowCompletionTime;
            cbHighLightConflicts.Checked = config.HighlightConflicts;
            cbShowWallet.Checked = config.ShowWallet;
            cbShowPortrait.Checked = config.ShowPortrait;
            cbPortraitSize.Items.AddRange(m_portraitSize);
            cbPortraitSize.SelectedIndex = (int)config.PortraitSize;
            cbShowWarning.Checked = config.ShowWarning;
            cbShowTQStatus.Checked = config.ShowTQStatus;
            cbShowEveTime.Checked = config.ShowEveTime;
            UpdateEnables();
        }

        private void ApplyToConfig()
        {
            m_config.HideCharNotTraining = cbHideNotTraining.Checked;
            m_config.GroupBy = (TrayPopupConfig.CharacterGroupings)cbGroupBy.SelectedIndex;
            m_config.SortOrder1 = GetSortOrder(cbDisplayOrder1.SelectedItem as string);
            m_config.SortOrder2 = GetSortOrder(cbDisplayOrder2.SelectedItem as string);
            m_config.ShowSkill = cbShowSkill.Checked;
            m_config.ShowTimeToCompletion = cbShowTimeToCompletion.Checked;
            m_config.ShowCompletionTime = cbShowCompletionTime.Checked;
            m_config.HighlightConflicts = cbHighLightConflicts.Checked;
            m_config.ShowWallet = cbShowWallet.Checked;
            m_config.ShowPortrait = cbShowPortrait.Checked;
            m_config.PortraitSize = (TrayPopupConfig.PortraitSizes)cbPortraitSize.SelectedIndex;
            m_config.ShowWarning = cbShowWarning.Checked;
            m_config.ShowTQStatus = cbShowTQStatus.Checked;
            m_config.ShowEveTime = cbShowEveTime.Checked;
        }

        private TrayPopupConfig.SortOrders GetSortOrder(string selectedSortOrder)
        {
            TrayPopupConfig.SortOrders sortOrder = TrayPopupConfig.SortOrders.EarliestAtBottom;
            for (int i = 0; i < m_sortOrder.Length; i++)
            {
                if (selectedSortOrder == m_sortOrder[i])
                    sortOrder = (TrayPopupConfig.SortOrders)i;
            }
            return sortOrder;
        }

        private void UpdateEnables()
        {
            cbGroupBy.Enabled = !cbHideNotTraining.Checked;
            cbPortraitSize.Enabled = cbShowPortrait.Checked;
        }

        private void UpdateDisplayOrders()
        {
            if (cbHideNotTraining.Checked)
            {
                lblDisplayOrder1.Text = "Characters in training:";
                cbDisplayOrder1.Items.Clear();
                cbDisplayOrder1.Items.Add(m_sortOrder[(int)TrayPopupConfig.SortOrders.EarliestAtBottom]);
                cbDisplayOrder1.Items.Add(m_sortOrder[(int)TrayPopupConfig.SortOrders.EarliestAtTop]);
                cbDisplayOrder1.Items.Add(m_sortOrder[(int)TrayPopupConfig.SortOrders.AlphaFirstAtTop]);
                cbDisplayOrder1.Items.Add(m_sortOrder[(int)TrayPopupConfig.SortOrders.AlphaFirstAtBottom]);
                cbDisplayOrder1.SelectedItem = m_sortOrder[(int)TrayPopupConfig.SortOrders.EarliestAtBottom];
                lblDisplayOrder2.Visible = false;
                cbDisplayOrder2.Visible = false;
            }
            else
            {
                TrayPopupConfig.CharacterGroupings groupBy = (TrayPopupConfig.CharacterGroupings)cbGroupBy.SelectedIndex;
                if (groupBy == TrayPopupConfig.CharacterGroupings.None)
                {
                    lblDisplayOrder1.Text = "All characters:";
                    cbDisplayOrder1.Items.Clear();
                    cbDisplayOrder1.Items.Add(m_sortOrder[(int)TrayPopupConfig.SortOrders.AlphaFirstAtTop]);
                    cbDisplayOrder1.Items.Add(m_sortOrder[(int)TrayPopupConfig.SortOrders.AlphaFirstAtBottom]);
                    cbDisplayOrder1.SelectedItem = m_sortOrder[(int)TrayPopupConfig.SortOrders.AlphaFirstAtTop];
                    lblDisplayOrder2.Visible = false;
                    cbDisplayOrder2.Visible = false;
                }
                else if (groupBy == TrayPopupConfig.CharacterGroupings.Account)
                {
                    lblDisplayOrder1.Text = "Accounts:";
                    cbDisplayOrder1.Items.Clear();
                    cbDisplayOrder1.Items.Add(m_sortOrder[(int)TrayPopupConfig.SortOrders.EarliestAtBottom]);
                    cbDisplayOrder1.Items.Add(m_sortOrder[(int)TrayPopupConfig.SortOrders.EarliestAtTop]);
                    cbDisplayOrder1.SelectedItem = m_sortOrder[(int)TrayPopupConfig.SortOrders.EarliestAtBottom];
                    lblDisplayOrder2.Visible = false;
                    cbDisplayOrder2.Visible = false;
                }
                else
                {
                    lblDisplayOrder1.Text = "Characters in training:";
                    cbDisplayOrder1.Items.Clear();
                    cbDisplayOrder1.Items.Add(m_sortOrder[(int)TrayPopupConfig.SortOrders.EarliestAtBottom]);
                    cbDisplayOrder1.Items.Add(m_sortOrder[(int)TrayPopupConfig.SortOrders.EarliestAtTop]);
                    cbDisplayOrder1.Items.Add(m_sortOrder[(int)TrayPopupConfig.SortOrders.AlphaFirstAtTop]);
                    cbDisplayOrder1.Items.Add(m_sortOrder[(int)TrayPopupConfig.SortOrders.AlphaFirstAtBottom]);
                    cbDisplayOrder1.SelectedItem = m_sortOrder[(int)TrayPopupConfig.SortOrders.EarliestAtBottom];
                    lblDisplayOrder2.Text = "Characters not training:";
                    cbDisplayOrder2.Items.Clear();
                    cbDisplayOrder2.Items.Add(m_sortOrder[(int)TrayPopupConfig.SortOrders.AlphaFirstAtTop]);
                    cbDisplayOrder2.Items.Add(m_sortOrder[(int)TrayPopupConfig.SortOrders.AlphaFirstAtBottom]);
                    cbDisplayOrder2.SelectedItem = m_sortOrder[(int)TrayPopupConfig.SortOrders.AlphaFirstAtTop];
                    lblDisplayOrder2.Visible = true;
                    cbDisplayOrder2.Visible = true;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ApplyToConfig();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cbHideNotTraining_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnables();
            UpdateDisplayOrders();
        }

        private void cbGroupBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDisplayOrders();
        }

        private void cbShowPortrait_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnables();
        }

        private void btnUseDefaults_Click(object sender, EventArgs e)
        {
            DisplayConfig(new TrayPopupConfig());
        }

    }
}

