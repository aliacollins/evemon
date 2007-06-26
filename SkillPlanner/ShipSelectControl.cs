using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    public partial class ShipSelectControl : EveObjectSelectControl
    {
        public ShipSelectControl()
        {
            InitializeComponent();
        }

        private CheckBox cbFaction;
        private CheckBox cbAmarr;
        private CheckBox cbORE;
        private CheckBox cbGallente;
        private CheckBox cbMinmatar;
        private CheckBox cbCaldari;

        private Ship[] m_ships;

        protected override void EveObjectSelectControl_Load(object sender, EventArgs e)
        {
            base.EveObjectSelectControl_Load(sender, e);
            try
            {
                cbSkillFilter.SelectedIndex = m_settings.ShipBrowserFilter;
                m_ships = Ship.GetShips();
                if (m_settings.StoreBrowserFilters)
                {
                    tbSearchText.Text = m_settings.ShipBrowserSearch;
                }
                if (m_ships != null)
                {
                    BuildTreeView();
                }
            }
            catch (Exception err)
            {
                // This occurs when we're in the designer. DesignMode doesn't get set
                // when the control is a subcontrol of a user control, so we should handle
                // this here :(
                ExceptionHandler.LogException(err, true);
                return;
            }
            this.cbSkillFilter.Items[0] = "All Ships";
            this.cbSkillFilter.Items[1] = "Ships I can fly";
            this.cbSkillFilter.Items[2] = "Ships I cannot fly";

            this.cbAmarr.Checked = m_settings.ShowAmarrShips;
            this.cbCaldari.Checked = m_settings.ShowCaldariShips;
            this.cbGallente.Checked = m_settings.ShowGallenteShips;
            this.cbMinmatar.Checked = m_settings.ShowMinmatarShips;
            this.cbFaction.Checked = m_settings.ShowFactionShips;
            this.cbORE.Checked = m_settings.ShowOreShips;
        }

        #region Filters

        protected void cbSkillFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_settings.ShipBrowserFilter = cbSkillFilter.SelectedIndex;
            UpdateDisplay();
        }

        private void cbRace_SelectedChanged(object sender, EventArgs e)
        {
            switch (sender)
            {
                case cbAmarr:
                    m_settings.ShowAmarrShips = cbAmarr.Checked;
                    break;
                case cbCaldari:
                    m_settings.ShowCaldariShips = cbCaldari.Checked;
                    break;
                case cbGallente:
                    m_settings.ShowGallenteShips = cbGallente.Checked;
                    break;
                case cbMinmater:
                    m_settings.ShowMinmatarShips = cbMinmatar.Checked;
                    break;
                case cbFaction:
                    m_settings.ShowFactionShips = cbFaction.Checked;
                    break;
                case cbORE:
                    m_settings.ShowOreShips = cbORE.Checked;
                    break;
            }
            UpdateDisplay();
        }


        protected void UpdateSkillFilter()
        {
            switch (cbSkillFilter.SelectedIndex)
            {
                default:
                case 0: m_filterDelegate = SelectAll;
                    break;
                case 1: // Ships I Can fly
                    m_filterDelegate = CanUse;
                    break;
                case 2: // Ships I Can NOT fly
                    m_filterDelegate = CannotUse;
                    break;
            }
        }

        private bool ShipRaceFilter(Ship s)
        {
            switch (s.Race)
            {
                case "Amarr":
                    return cbAmarr.Checked;
                case "Gallente":
                    return cbGallente.Checked;
                case "Caldari":
                    return cbCaldari.Checked;
                case "Minmatar":
                    return cbMinmatar.Checked;
                case "Faction":
                    return cbFaction.Checked;
                case "ORE":
                    return cbORE.Checked;
                default:
                    return false;
            }
        }

        #endregion

        #region Display
        protected void UpdateDisplay()
        {
            UpdateSkillFilter();
            if (m_ships != null)
            {
                BuildTreeView();
            }
            SearchTextChanged();
        }

        protected void BuildTreeView()
        {
            tvItems.Nodes.Clear();
            tvItems.BeginUpdate();
            try
            {
                SortedList<string, List<Ship>> types = new SortedList<string, List<Ship>>();
                foreach (Ship s in m_ships)
                {
                    if (m_filterDelegate(s) && ShipRaceFilter(s))
                    {
                        // check with filter if this ship is to be ad
                        if (!types.ContainsKey(s.Type))
                        {
                            List<Ship> nl = new List<Ship>();
                            nl.Add(s);
                            types.Add(s.Type, nl);
                        }
                        else
                        {
                            types[s.Type].Add(s);
                        }
                    }
                }

                foreach (KeyValuePair<string, List<Ship>> kvp in types)
                {
                    TreeNode tvn = new TreeNode();
                    tvn.Text = kvp.Key;

                    SortedList<string, List<Ship>> races = new SortedList<string, List<Ship>>();
                    foreach (Ship s in kvp.Value)
                    {
                        if (!races.ContainsKey(s.Race))
                        {
                            List<Ship> nl = new List<Ship>();
                            nl.Add(s);
                            races.Add(s.Race, nl);
                        }
                        else
                        {
                            races[s.Race].Add(s);
                        }
                    }
                    foreach (KeyValuePair<string, List<Ship>> ssvp in races)
                    {
                        TreeNode racenode = new TreeNode();
                        racenode.Text = ssvp.Key;

                        SortedList<string, Ship> ships = new SortedList<string, Ship>();
                        foreach (Ship s in ssvp.Value)
                        {
                            ships[s.Name] = s;
                        }
                        foreach (Ship s in ships.Values)
                        {
                            TreeNode stn = new TreeNode();
                            stn.Text = s.Name;
                            stn.Tag = s;
                            racenode.Nodes.Add(stn);
                        }

                        tvn.Nodes.Add(racenode);
                    }

                    tvItems.Nodes.Add(tvn);
                }
            }
            finally
            {
                tvItems.EndUpdate();
            }
        }
        #endregion

        #region Search
        protected override void tbSearchText_TextChanged(object sender, EventArgs e)
        {
            if (m_settings.StoreBrowserFilters)
            {
                m_settings.ShipBrowserSearch = tbSearchText.Text;
            }
            base.tbSearchText_TextChanged(sender, e);
        }

        #endregion

        protected void InitializeComponent()
        {
            this.cbCaldari = new System.Windows.Forms.CheckBox();
            this.cbFaction = new System.Windows.Forms.CheckBox();
            this.cbGallente = new System.Windows.Forms.CheckBox();
            this.cbMinmatar = new System.Windows.Forms.CheckBox();
            this.cbORE = new System.Windows.Forms.CheckBox();
            this.cbAmarr = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSearchImage)).BeginInit();
            this.SuspendLayout();
            //
            // cbSkillFilter
            //
            this.cbSkillFilter.SelectedIndexChanged += new System.EventHandler(this.cbSkillFilter_SelectedIndexChanged);
            //
            // tbSearchText
            //
            this.tbSearchText.Location = new System.Drawing.Point(33, 99);
            this.tbSearchText.Size = new System.Drawing.Size(152, 21);
            //
            // tvItems
            //
            this.tvItems.LineColor = System.Drawing.Color.Black;
            this.tvItems.Size = new System.Drawing.Size(185, 285);
            //
            // lbNoMatches
            //
            this.lbNoMatches.Location = new System.Drawing.Point(2, 2);
            this.lbNoMatches.Size = new System.Drawing.Size(168, 22);
            //
            // lbSearchList
            //
            this.lbSearchList.Size = new System.Drawing.Size(185, 285);
            //
            // panel1
            //
            this.panel1.Controls.Add(this.cbFaction);
            this.panel1.Controls.Add(this.cbORE);
            this.panel1.Controls.Add(this.cbAmarr);
            this.panel1.Controls.Add(this.cbMinmatar);
            this.panel1.Controls.Add(this.cbGallente);
            this.panel1.Controls.Add(this.cbCaldari);
            this.panel1.Size = new System.Drawing.Size(185, 125);
            this.panel1.Controls.SetChildIndex(this.tbSearchText, 0);
            this.panel1.Controls.SetChildIndex(this.pbSearchImage, 0);
            this.panel1.Controls.SetChildIndex(this.lbSearchTextHint, 0);
            this.panel1.Controls.SetChildIndex(this.cbCaldari, 0);
            this.panel1.Controls.SetChildIndex(this.cbGallente, 0);
            this.panel1.Controls.SetChildIndex(this.cbMinmatar, 0);
            this.panel1.Controls.SetChildIndex(this.cbAmarr, 0);
            this.panel1.Controls.SetChildIndex(this.cbORE, 0);
            this.panel1.Controls.SetChildIndex(this.cbFaction, 0);
            this.panel1.Controls.SetChildIndex(this.label1, 0);
            this.panel1.Controls.SetChildIndex(this.cbSkillFilter, 0);
            //
            // panel2
            //
            this.panel2.Location = new System.Drawing.Point(0, 125);
            this.panel2.Size = new System.Drawing.Size(185, 285);
            //
            // lbSearchTextHint
            //
            this.lbSearchTextHint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.lbSearchTextHint.Location = new System.Drawing.Point(34, 100);
            this.lbSearchTextHint.Size = new System.Drawing.Size(68, 18);
            //
            // pbSearchImage
            //
            this.pbSearchImage.Location = new System.Drawing.Point(9, 99);
            //
            // cbCaldari
            //
            this.cbCaldari.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.cbCaldari.AutoSize = true;
            this.cbCaldari.Cursor = System.Windows.Forms.Cursors.Default;
            this.cbCaldari.Location = new System.Drawing.Point(9, 53);
            this.cbCaldari.Name = "cbCaldari";
            this.cbCaldari.Size = new System.Drawing.Size(59, 17);
            this.cbCaldari.TabIndex = 27;
            this.cbCaldari.Text = "Caldari";
            this.cbCaldari.UseVisualStyleBackColor = true;
            this.cbCaldari.CheckedChanged += new System.EventHandler(this.cbRace_SelectedChanged);
            //
            // cbFaction
            //
            this.cbFaction.AutoSize = true;
            this.cbFaction.Location = new System.Drawing.Point(90, 53);
            this.cbFaction.Name = "cbFaction";
            this.cbFaction.Size = new System.Drawing.Size(61, 17);
            this.cbFaction.TabIndex = 30;
            this.cbFaction.Text = "Faction";
            this.cbFaction.UseVisualStyleBackColor = true;
            this.cbFaction.CheckedChanged += new System.EventHandler(this.cbRace_SelectedChanged);
            //
            // cbGallente
            //
            this.cbGallente.AutoSize = true;
            this.cbGallente.Location = new System.Drawing.Point(9, 76);
            this.cbGallente.Name = "cbGallente";
            this.cbGallente.Size = new System.Drawing.Size(65, 17);
            this.cbGallente.TabIndex = 28;
            this.cbGallente.Text = "Gallente";
            this.cbGallente.UseVisualStyleBackColor = true;
            this.cbGallente.CheckedChanged += new System.EventHandler(this.cbRace_SelectedChanged);
            //
            // cbMinmatar
            //
            this.cbMinmatar.AutoSize = true;
            this.cbMinmatar.Location = new System.Drawing.Point(90, 30);
            this.cbMinmatar.Name = "cbMinmatar";
            this.cbMinmatar.Size = new System.Drawing.Size(70, 17);
            this.cbMinmatar.TabIndex = 29;
            this.cbMinmatar.Text = "Minmatar";
            this.cbMinmatar.UseVisualStyleBackColor = true;
            this.cbMinmatar.CheckedChanged += new System.EventHandler(this.cbRace_SelectedChanged);
            //
            // cbORE
            //
            this.cbORE.AutoSize = true;
            this.cbORE.Location = new System.Drawing.Point(90, 76);
            this.cbORE.Name = "cbORE";
            this.cbORE.Size = new System.Drawing.Size(47, 17);
            this.cbORE.TabIndex = 31;
            this.cbORE.Text = "ORE";
            this.cbORE.UseVisualStyleBackColor = true;
            this.cbORE.CheckedChanged += new System.EventHandler(this.cbRace_SelectedChanged);
            //
            // cbAmarr
            //
            this.cbAmarr.AutoSize = true;
            this.cbAmarr.Location = new System.Drawing.Point(9, 30);
            this.cbAmarr.Name = "cbAmarr";
            this.cbAmarr.Size = new System.Drawing.Size(55, 17);
            this.cbAmarr.TabIndex = 26;
            this.cbAmarr.Text = "Amarr";
            this.cbAmarr.UseVisualStyleBackColor = true;
            this.cbAmarr.CheckedChanged += new System.EventHandler(this.cbRace_SelectedChanged);
            //
            // ShipSelectControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.Name = "ShipSelectControl";
            this.Size = new System.Drawing.Size(185, 410);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbSearchImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
