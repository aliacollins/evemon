using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;


namespace EVEMon.SkillPlanner
{
    public partial class CrossPlanSelect : Form
    {
        #region Constructors
        public CrossPlanSelect(String toCharacter)
        {
            InitializeComponent();
            m_ToCharacterName = toCharacter;
        }

        #endregion

        #region Properties
        
        public string SelectedPlan
        {
            get { return m_selectedPlan; }
        }
        
        public string SelectedCharKey
        {
            get { return m_planKeys[m_selectedChar]; }
        }

        public string SelectedCharName
        {
            get { return m_selectedChar; }
        }

        #endregion

        private void PopulatePlans(String characterName)
        {
            btnLoad.Enabled = false;
            lbPlan.Items.Clear();
            foreach (string planName in m_settings.GetPlansForCharacter(m_planKeys[characterName]))
            {
                lbPlan.Items.Add(planName);
            }
        }
        #region event handlers
        /// <summary>
        /// Populate the character list with all characters except the target
        /// </summary>
        private void CrossPlanSelect_Load(object sender, EventArgs e)
        {
            m_settings = Settings.GetInstance();
            cbCharacter.Items.Clear();
            m_planKeys = new Dictionary<String, String>();
            foreach (CharLoginInfo cli in m_settings.CharacterList)
            {
                // character must not be same as the one we're loading a plan for
                // and it must have at least one plan
                if (cli.CharacterName != m_ToCharacterName)
                {
                    m_planKeys.Add(cli.CharacterName, cli.CharacterName);
                    cbCharacter.Items.Add(cli.CharacterName);
                }
            }
            foreach (CharFileInfo cfi in m_settings.CharFileList)
            {
                if (cfi != null && cfi.CharacterName != m_ToCharacterName)
                {
                    m_planKeys.Add(cfi.CharacterName, cfi.Filename);
                    cbCharacter.Items.Add(cfi.CharacterName);
                }
            }
            cbCharacter.Text = cbCharacter.Items[0].ToString();
            PopulatePlans(cbCharacter.Text);
        }

        private void cbCharacter_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulatePlans(cbCharacter.Items[cbCharacter.SelectedIndex].ToString());
        }

        private void lbPlan_SelectedIndexChanged(object sender, EventArgs e)
        {

            btnLoad.Enabled = lbPlan.SelectedItems.Count == 1;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            m_selectedPlan = lbPlan.SelectedItem.ToString();
            m_selectedChar = cbCharacter.Text;
            DialogResult = DialogResult.OK;
            this.Close();
        }

#endregion

        #region private members
        private string m_ToCharacterName;
        private Settings m_settings;
        private Dictionary<String, String> m_planKeys; 
        private String m_selectedPlan;
        private String m_selectedChar;
        #endregion

    }
}