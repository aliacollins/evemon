using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Factories;
using EVEMon.Common.Helpers;
using EVEMon.Common.Models;
using System.Collections.Generic;
using EVEMon.Common.Extensions;

namespace EVEMon.SkillPlanner
{
    internal partial class ShipBrowserControl : EveObjectBrowserControl
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ShipBrowserControl()
        {
            InitializeComponent();
            scObjectBrowser.RememberDistanceKey = "ShipsBrowser_Left";
            SelectControl = shipSelectControl;
            PropertiesList = lvShipProperties;
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Opens the BattleClinic Loadout window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblBattleclinic_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShipLoadoutSelectWindow window = WindowsFactory.ShowByTag<ShipLoadoutSelectWindow, Plan>(Plan);
            window.Ship = SelectedObject;
        }

        /// <summary>
        /// Exports item info to CSV format.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewExporter.CreateCSV(PropertiesList, true);
        }

        private void tsPlanToLevelOne_Click(object sender, EventArgs e)
        {
            
        }

        private void tsPlanToLevelTwo_Click(object sender, EventArgs e)
        {

        }

        private void tsPlanToLevelThree_Click(object sender, EventArgs e)
        {

        }

        private void tsPlanToLevelFour_Click(object sender, EventArgs e)
        {

        }

        private void tsPlanToLevelFive_Click(object sender, EventArgs e)
        {
            
        }
        #endregion


        #region Inherited Events

        /// <summary>
        /// Occurs when the settings changed.
        /// </summary>
        protected override void OnSettingsChanged()
        {
            base.OnSettingsChanged();
            UpdateControlVisibility();
        }

        /// <summary>
        /// Occurs when the conrol loads.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            // Return on design mode
            if (DesignMode || this.IsDesignModeHosted())
                return;

            base.OnLoad(e);

            UpdateControlVisibility();
        }

        /// <summary>
        /// Updates the controls when the selection is changed.
        /// </summary>
        protected override void OnSelectionChanged()
        {
            base.OnSelectionChanged();
            if (SelectedObject == null)
                return;

            // Description
            tbDescription.Text = SelectedObject.Description;

            // Required Skills
            requiredSkillsControl.Object = SelectedObject;

            ShipLoadoutSelectWindow loadoutSelect = WindowsFactory.GetByTag<ShipLoadoutSelectWindow, Plan>(Plan);
            if (loadoutSelect != null && !loadoutSelect.IsDisposed)
                loadoutSelect.Ship = shipSelectControl.SelectedObject;

            // Update the Masterytab      
            UpdateMasteryInformation();
        }

        /// <summary>
        /// Updates the plan when the selection is changed.
        /// </summary>
        protected override void OnPlanChanged()
        {
            base.OnPlanChanged();
            requiredSkillsControl.Plan = Plan;

            // We recalculate the right panels minimum size
            int reqSkillControlMinWidth = requiredSkillsControl.MinimumSize.Width;
            int reqSkillPanelMinWidth = scDetails.Panel2MinSize;
            scDetails.Panel2MinSize = (reqSkillPanelMinWidth > reqSkillControlMinWidth
                                           ? reqSkillPanelMinWidth
                                           : reqSkillControlMinWidth);
        }

        #endregion


        #region Helper Methods

        private void UpdateMasteryInformation()
        {
            var masteryship = StaticMasteries.GetMasteryShipByID(shipSelectControl.SelectedObject.ID);

            if (masteryship == null)
            {
                return;
            }

            trVwshipMasteries.BeginUpdate();
            try
            {
                // Clear the old items
                trVwshipMasteries.Nodes.Clear();

                // Create the nodes when not done, yet
                if (trVwshipMasteries.Nodes.Count == 0)
                {
                    foreach (Mastery mastery in masteryship)
                    {
                        TreeNode node = CreateNode(mastery);
                        trVwshipMasteries.Nodes.Add(node);
                    }
                }

                // Update the nodes
                foreach (TreeNode node in trVwshipMasteries.Nodes)
                {
                    UpdateNode(node);
                }
            }
            finally
            {
                trVwshipMasteries.EndUpdate();
            }            
        }

        private TreeNode CreateNode(Mastery mastery)
        {
            var node = new TreeNode()
            {
                Text = string.Format("Level {0}", mastery.Level),
                Tag = mastery
            };

            var certLevel = (CertificateGrade)mastery.Level;

            var highestSkills = new Dictionary<int, SkillLevel>();

            //foreach (var skillLevel in mastery.SelectMany(masteryCert => masteryCert.Certificate.Grades.Select(pair => pair.Value.ToCharacter(Plan.))))
            //{                
            //    if(!highestSkills.ContainsKey(skillLevel.Skill.ID))
            //    {
            //        highestSkills.Add(skillLevel.Skill.ID, skillLevel);
            //    }

            //    if(highestSkills[skillLevel.Skill.ID].Level < skillLevel.Level)
            //    {
            //        highestSkills[skillLevel.Skill.ID] = skillLevel;
            //    }
            //}

            //foreach (var staticSkillLevel in highestSkills.Values)
            //{
            //    node.Nodes.Add(CreateNode(staticSkillLevel));
            //}

            return node;
        }

        /// <summary>
        /// Create a node from a skill.
        /// </summary>
        /// <param name="skilllevel"></param>
        /// <returns></returns>
        private static TreeNode CreateNode(SkillLevel skilllevel)
        {
            TreeNode node = new TreeNode
            {
                Text = skilllevel.ToString(),
                Tag = skilllevel
            };

            // Add this skill's prerequisites
            foreach (var prereqSkill in skilllevel.Skill.Prerequisites.Where(prereqSkill => prereqSkill.Skill != skilllevel.Skill))
            {
                node.Nodes.Add(CreateNode(prereqSkill));
            }

            return node;
        }

        /// <summary>
        /// Updates the specified node and its children.
        /// </summary>
        /// <param name="node"></param>
        private void UpdateNode(TreeNode node)
        {
            var certLevel = node.Tag as CertificateLevel;

            //// The node represents a certificate
            //if (certLevel != null)
            //{
            //    switch (certLevel.Status)
            //    {
            //        case CertificateStatus.Trained:
            //            node.ImageIndex = imageList.Images.IndexOfKey(TrainedIcon);
            //            break;
            //        case CertificateStatus.PartiallyTrained:
            //            node.ImageIndex = imageList.Images.IndexOfKey(TrainableIcon);
            //            break;
            //        case CertificateStatus.Untrained:
            //            node.ImageIndex = imageList.Images.IndexOfKey(UntrainableIcon);
            //            break;
            //        default:
            //            throw new NotImplementedException();
            //    }
            //}
            //// The node represents a skill prerequisite
            //else
            //{
            //    SkillLevel skillPrereq = (SkillLevel)node.Tag;
            //    Skill skill = m_character.Skills[skillPrereq.Skill.ID];

            //    if (skillPrereq.IsTrained)
            //        node.ImageIndex = imageList.Images.IndexOfKey(TrainedIcon);
            //    else if (m_plan.IsPlanned(skill, skillPrereq.Level))
            //        node.ImageIndex = imageList.Images.IndexOfKey(PlannedIcon);
            //    else if (skill.IsKnown)
            //        node.ImageIndex = imageList.Images.IndexOfKey(TrainableIcon);
            //    else
            //        node.ImageIndex = imageList.Images.IndexOfKey(UntrainableIcon);
            //}

            // When selected, the image remains the same
            node.SelectedImageIndex = node.ImageIndex;

            // Update the children
            foreach (TreeNode child in node.Nodes)
            {
                UpdateNode(child);
            }
        }


        /// <summary>
        /// Updates the contol visibility.
        /// </summary>
        private void UpdateControlVisibility()
        {
            lblBattleclinic.Location = Settings.UI.SafeForWork
                                           ? new Point(Pad, lblBattleclinic.Location.Y)
                                           : new Point(eoImage.Width + Pad * 2, lblBattleclinic.Location.Y);
        }

        #endregion       
    }
}