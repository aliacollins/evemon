using System;
using System.Windows.Forms;
using System.Collections.Generic;
using EVEMon.Common;

namespace EVEMon
{
    public partial class ManualImplantDetailWindow : EVEMonForm
    {
        public ManualImplantDetailWindow()
        {
            InitializeComponent();
        }

        public ManualImplantDetailWindow(UserImplant b)
            : this()
        {
            m_inBonus = b;
            m_Implants = Slot.GetImplants()[b.Slot - 1];
        }

        private Slot m_Implants;

        private UserImplant m_inBonus = null;

        private bool found = false;

        private void ManualImplantDetailWindow_Load(object sender, EventArgs e)
        {
            cbImplant.BeginUpdate();
            try
            {
                cbImplant.Items.Clear();
                SortedList<string, Implant> implants = new SortedList<string, Implant>();
                foreach (Implant i in m_Implants.ImplantDict.Values)
                {
                    implants[i.Name] = i;
                }

                cbImplant.Items.Add("<None>");
                foreach (Implant i in implants.Values)
                {
                    if (i.Name == m_inBonus.Name)
                        found = true;
                    cbImplant.Items.Add(i.Name);
                }
                if (!found && m_inBonus.Name != "<None>")
                    cbImplant.Items.Add(m_inBonus.Name);
            }
            finally
            {
                cbImplant.EndUpdate();
            }

            cbAttribute.Text = "<None>";
            nudAmount.Text = "0";
            chkbTech2.Checked = false;
            rtbDescription.Text = "";

            if (m_inBonus != null)
            {
                tbSlot.Text = m_inBonus.Slot.ToString();
                cbImplant.SelectedItem = m_inBonus.Name;
                if (m_inBonus.Slot < 6)
                    cbAttribute.Text = (UserImplant.SlotToAttrib(m_inBonus.Slot)).ToString();
                else
                    cbAttribute.Text = "<None>";
                nudAmount.Text = m_inBonus.Bonus.ToString();

                Implant x = m_Implants[m_inBonus.Name];
                if (x != null)
                {
                    nudAmount.Text = x.Bonus.ToString();
                    chkbTech2.Checked = x.Tech2;
                    rtbDescription.Text = x.Description;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public UserImplant ResultBonus
        {
            get { return m_implant; }
        }

        private UserImplant m_implant;

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (chkbDelete.Checked)
                m_implant = null;
            else if (cbImplant.SelectedItem.ToString() == "<None>") // Handy for nulling out "Auto" implants in "Current"
                m_implant = new UserImplant(m_inBonus.Slot, new Implant(), true);
            else
            {
                bool manual = false;
                if (cbImplant.SelectedItem.ToString() == m_inBonus.Name)
                    manual = m_inBonus.Manual;
                else
                    manual = true;
                m_implant = new UserImplant(m_inBonus.Slot, m_Implants[cbImplant.SelectedItem.ToString()], manual);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cbImplant_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbImplant.SelectedItem.ToString() == "<None>")
            {
                nudAmount.Text = "0";
                chkbTech2.Checked = false;
                rtbDescription.Text = "";
            }
            else if (!found && cbImplant.SelectedItem.ToString() == m_inBonus.Name)
            {
                nudAmount.Text = m_inBonus.Bonus.ToString();
                chkbTech2.Checked = false;
                rtbDescription.Text = "";
            }
            else
            {
                Implant x = m_Implants[cbImplant.SelectedItem.ToString()];
                if (x != null)
                {
                    nudAmount.Text = x.Bonus.ToString();
                    chkbTech2.Checked = x.Tech2;
                    rtbDescription.Text = x.Description;
                }
            }
            tableLayoutPanel1.Refresh();
        }
    }
}