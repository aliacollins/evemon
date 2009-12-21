using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.SkillPlanner;
using System.Text.RegularExpressions;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// Superclass for simple one page item browsers.
    /// Extends EveObjectBrowserControl with the addition of Description and Required Skills panes
    /// </summary>
    public partial class EveObjectBrowserSimple : EveObjectBrowserControl
    {
        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public EveObjectBrowserSimple()
        {
            InitializeComponent();
            this.PlanChanged += new EventHandler(EveObjectBrowserSimple_PlanChanged);
        }

        #endregion

        #region Event Handlers
        /// <summary>
        /// Updates Required Skills control when selected plan is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveObjectBrowserSimple_PlanChanged(object sender, EventArgs e)
        {
            requiredSkillsControl.Plan = this.Plan;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Displays item description and updates Required Skills control
        /// </summary>
        /// <param name="item"></param>
        protected override void DisplayItemDetails(EveObject item)
        {
            base.DisplayItemDetails(item);
            // Description
            tbDescription.Text = Regex.Replace(item.Description, "<.+?>", String.Empty, RegexOptions.Singleline); ;
            // Required Skills
            requiredSkillsControl.EveItem = item;
        }
        #endregion
    }
}

