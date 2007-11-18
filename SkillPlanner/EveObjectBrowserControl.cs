using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// Base class for EveObject browsers.
    /// Provides basic split container layout and item header including icon, name and category, 
    /// along with event handling for item selection and worksafeMode changes. Derived classes
    /// should override DisplayItemDetails() as required
    /// </summary>
    /// <remarks>
    /// Should be an abstract class but Visual Studio Designer throws a wobbler when you
    /// try to design a class that inherits from an abstract class.</remarks>
    public partial class EveObjectBrowserControl : UserControl
    {

        #region Fields
        private EveObjectSelectControl  _objectSelectControl;
        private Plan                    _plan;
        private EveObject               _selectedObject;

        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public EveObjectBrowserControl()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties
        /// <summary>
        /// An EveObjectSelectControl that supplies item selection to the browser
        /// </summary>
        protected EveObjectSelectControl ObjectSelectControl
        {
            get { return _objectSelectControl; }
            set { _objectSelectControl = value; }
        }
        /// <summary>
        /// The current Plan for this planner window.
        /// </summary>
        public Plan Plan
        {
            get { return _plan; }
            set
            {
                _plan = value;
                if (this._objectSelectControl != null) { this._objectSelectControl.Plan = value; }
                PlanChanged(this, new EventArgs());
            }
        }
        /// <summary>
        /// The EveObject selected by ObjectSelectControl
        /// </summary>
        public EveObject SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                _selectedObject = value;
                this._objectSelectControl.SelectedObject = value;
                objectSelectControl_SelectedObjectChanged(this, null);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event for change of plan to notify derived classes.
        /// </summary>
        protected event EventHandler PlanChanged;
        #endregion

        #region Event handlers
        /// <summary>
        /// Adds required event handlers and repositions help text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveObjectBrowserControl_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                // Get settings instance & watch for worksafeMode changes
                Settings.GetInstance().WorksafeChanged += new EventHandler<EventArgs>(Settings_WorksafeChanged);
                if (this._objectSelectControl != null)
                {
                    // Watch for selection changes
                    this._objectSelectControl.SelectedObjectChanged += new EventHandler<EventArgs>(objectSelectControl_SelectedObjectChanged);
                    // Reposition the help text along side the treeview
                    Control[] result = this._objectSelectControl.Controls.Find("panel2", true);
                    if (result.Length > 0)
                    {
                        lblHelp.Location = new Point(lblHelp.Location.X, result[0].Location.Y);
                    }
                }
                // Watch for the form being closed so we can clean up after ourselves
                this.ParentForm.FormClosing += new FormClosingEventHandler(ParentForm_FormClosing);
                // Force a refresh
                objectSelectControl_SelectedObjectChanged(null, null);
            }
        }
        /// <summary>
        /// Updates header and detail pane visibility and contents.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void objectSelectControl_SelectedObjectChanged(object sender, EventArgs e)
        {
            this._selectedObject = this._objectSelectControl.SelectedObject;
            if (this._selectedObject != null)
            {
                // Hide help message
                lblHelp.Visible = false;
                // View details and header
                pnlDetails.Visible = true;
                pnlBrowserHeader.Visible = true;
                // Display details
                DisplayItemDetails(this._selectedObject);
            }
            else
            {
                // Hide details and header
                pnlBrowserHeader.Visible = false;
                pnlDetails.Visible = false;
                // View help message
                lblHelp.Visible = true;
            }
        }
        /// <summary>
        /// Event cleanup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Remove event handlers we don't need
            Settings.GetInstance().WorksafeChanged -= new EventHandler<EventArgs>(Settings_WorksafeChanged);
        }
        /// <summary>
        /// Capture changes to worksafeMode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_WorksafeChanged(object sender, EventArgs e)
        {
            SetWorksafe(Settings.GetInstance().WorksafeMode);
        }
        
        #endregion

        #region Methods
        /// <summary>
        /// Displays item header content. Derived classes show override to display additional content.
        /// </summary>
        /// <param name="item">EveObject whose details to display</param>
        protected virtual void DisplayItemDetails(EveObject item)
        {
            eoImage.EveItem = item;
            lblEveObjName.Text = item.Name;
            lblEveObjCategory.Text = item.GetCategoryPath();
        }
        /// <summary>
        /// Updates header pane content to reflect worksafeMode. Derived classes should override to refelct worksafeMode.
        /// </summary>
        /// <param name="worksafeMode"></param>
        public virtual void SetWorksafe(bool worksafeMode)
        {
            if (worksafeMode == true)
            {
                eoImage.ImageSize = EveImage.EveImageSize._0_0;
                lblEveObjCategory.Location = new Point(3, lblEveObjCategory.Location.Y);
                lblEveObjName.Location = new Point(3, lblEveObjName.Location.Y);
            }
            else
            {
                eoImage.ImageSize = EveImage.EveImageSize._64_64;
                lblEveObjCategory.Location = new Point(70, lblEveObjCategory.Location.Y);
                lblEveObjName.Location = new Point(70, lblEveObjName.Location.Y);
            }
        }
        #endregion
    }
}
