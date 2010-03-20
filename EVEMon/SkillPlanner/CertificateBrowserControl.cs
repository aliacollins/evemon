using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;

namespace EVEMon.SkillPlanner
{
    public partial class CertificateBrowserControl : UserControl
    {
        private Plan m_plan = null;
        private const int HPad = 40;

        /// <summary>
        /// Constructor
        /// </summary>
        public CertificateBrowserControl()
        {
            InitializeComponent();
            this.rightSplitContainer.RememberDistanceKey = "CertificateBrowser_Right";
            this.leftSplitContainer.RememberDistanceKey = "CertificateBrowser_Left";

            this.lblName.Font = FontFactory.GetFont("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            this.certSelectCtl.SelectionChanged += new EventHandler<EventArgs>(certSelectCtl_SelectionChanged);
            this.certDisplayCtl.SelectionChanged += new EventHandler(certDisplayCtl_SelectionChanged);

            EveClient.PlanChanged += new EventHandler<PlanChangedEventArgs>(EveClient_PlanChanged);
            this.Disposed += new EventHandler(OnDisposed);

            // Read the SafeForWork settings
            if (Settings.UI.SafeForWork)
            {
                pictureBox1.Visible = false;
                tsPlanToMenu.DisplayStyle = ToolStripItemDisplayStyle.Text;
            }
            else 
            {
                pictureBox1.Visible = true;
                tsPlanToMenu.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            }
        }

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDisposed(object sender, EventArgs e)
        {
            EveClient.PlanChanged -= new EventHandler<PlanChangedEventArgs>(EveClient_PlanChanged);
            this.Disposed -= new EventHandler(OnDisposed);
        }

        /// <summary>
        /// Gets or sets the character this control is bound to
        /// </summary>
        public Character Character
        {
            get { return m_plan.Character as Character; }
        }

        /// <summary>
        /// Gets or sets the current plan
        /// </summary>
        public Plan Plan
        {
            get { return m_plan; }
            set
            {
                m_plan = value;

                this.certSelectCtl.Plan = m_plan;
                this.certDisplayCtl.Plan = m_plan;
                UpdateEligibility();
            }
        }

        /// <summary>
        /// This is the way to get and set the selected certificate class
        /// </summary>
        public CertificateClass SelectedCertificateClass
        {
            get
            {
                return this.certSelectCtl.SelectedCertificateClass;
            }
            set
            {
                this.certSelectCtl.SelectedCertificateClass = value;
            }
        }

        /// <summary>
        /// This is the way to set the selected certificate
        /// </summary>
        public Certificate SelectedCertificate
        {
            set
            {
                if(this.SelectedCertificateClass != value.Class)
                    this.SelectedCertificateClass = value.Class;
                this.certDisplayCtl.ExpandCert(value);
            }
        }

        /// <summary>
        /// When the user select a new certificate class, we update everything
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void certSelectCtl_SelectionChanged(object sender, EventArgs e)
        {
            var certClass = this.certSelectCtl.SelectedCertificateClass;
            this.certDisplayCtl.CertificateClass = certClass;

            // When no certificate class is selected, we just hide the right panel.
            if (certClass == null)
            {
                panelRight.Visible = false;
                return;
            }

            panelRight.Visible = true;
            var firstCert = certClass.LowestGradeCertificate;
            lblName.Text = certClass.Name + (firstCert == null ? String.Empty : " " + firstCert.Grade.ToString());
            lblCategory.Text = certClass.Category.Name;

            // Initialize the labels' text for every existing grade
            List<Control> newItems = new List<Control>();
            Label[] labels = new Label[] { lblLevel1Time, lblLevel2Time, lblLevel3Time, lblLevel4Time };
            int lbIndex = 0;
            var rSplCont = this.rightSplitContainer;
            foreach (var cert in certClass)
            {
                var label = labels[lbIndex];
                var time = cert.GetTrainingTime();
                label.Text = cert.Grade + " : " + Skill.TimeSpanToDescriptiveText(time, DescriptiveTextOptions.IncludeCommas, false);
                label.Visible = true;
                lbIndex++;

                SortedList<string, Item> ships = new SortedList<string, Item>();
                foreach (Item s in cert.Recommendations)
                {
                    ships.Add(s.Name, s);
                }
                if (ships.Count != 0)
                {
                    Label tsl = new Label();
                    tsl.AutoSize = true;
                    tsl.Dock = DockStyle.Top;
                    tsl.Text = "Recommends " + cert.Grade.ToString() + ":";
                    tsl.Font = new Font(tsl.Font, FontStyle.Bold);
                    tsl.Padding = new Padding(5);
                    newItems.Add(tsl);

                    Size tslTextSize = TextRenderer.MeasureText(tsl.Text, Font);
                    int panelMinSize = rSplCont.Panel2MinSize;
                    rSplCont.Panel2MinSize = (panelMinSize > tslTextSize.Width + HPad ?
                                                                panelMinSize : tslTextSize.Width + HPad);
                    rSplCont.SplitterDistance = rSplCont.Width - rSplCont.Panel2MinSize;

                    foreach (Item s in ships.Values)
                    {
                        LinkLabel ll = new LinkLabel();
                        ll.MouseClick += new MouseEventHandler(recommendations_MenuItem);
                        ll.LinkBehavior = LinkBehavior.HoverUnderline;
                        ll.Padding = new Padding(16, 0, 0, 0);
                        ll.Dock = DockStyle.Top;
                        ll.Text = s.Name;
                        ll.Tag = s;
                        newItems.Add(ll);
                    }
                }
            }
            this.rightSplitContainer.Panel2.Controls.Clear();
            if (newItems.Count != 0)
            {
                newItems.Reverse();
                rSplCont.Panel2.Controls.AddRange(newItems.ToArray());
            }
            else
            {
                Label tsl = new Label();
                tsl.Dock = DockStyle.Fill;
                tsl.Text = "No Recommendations";
                tsl.Enabled = false;
                tsl.TextAlign = ContentAlignment.MiddleCenter;
                rSplCont.Panel2.Controls.Add(tsl);

                Size tslTextSize = TextRenderer.MeasureText(tsl.Text, Font);
                rSplCont.Panel2MinSize = tslTextSize.Width + HPad;
                rSplCont.SplitterDistance = rSplCont.Width - rSplCont.Panel2MinSize;
            }

            // Hides the other labels
            while (lbIndex < labels.Length)
            {
                labels[lbIndex].Visible = false;
                lbIndex++;
            }

            // Update the menus and such
            UpdateEligibility();
        }

        /// <summary>
        /// Handler for the ship-links generated for the recommendations
        /// </summary>
        void recommendations_MenuItem(object sender, EventArgs e)
        {
            Control tsi = sender as Control;
            if (tsi == null)
                return;
            Item ship = tsi.Tag as Item;
            PlanWindow window = WindowsFactory<PlanWindow>.GetByTag(m_plan);
            if (ship != null && window != null && !window.IsDisposed)
            {
                window.ShowShipInBrowser(ship);
            }
        }

        /// <summary>
        /// When the display tree's selection changes, we may update the description
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void certDisplayCtl_SelectionChanged(object sender, EventArgs e)
        {
            var cert = certDisplayCtl.SelectedCertificateLevel;
            var certClass = this.certSelectCtl.SelectedCertificateClass;

            // No certificate or not one of the roots ? Then, we display the description for the lowest grade cert
            if (cert == null || cert.Class != certClass)
            {
                var firstCert = certClass.LowestGradeCertificate;
                textboxDescription.Text = (firstCert == null ? String.Empty : firstCert.Description);
                lblName.Text = certClass.Name + (firstCert == null ? String.Empty : " " + firstCert.Grade.ToString());
            }
            // So, one of our cert class's grades has been selected, we use its description
            else
            {
                textboxDescription.Text = cert.Description;
                lblName.Text = certClass.Name + " " + cert.Grade.ToString();
            }
        }

        /// <summary>
        /// When the current plan changes (new skills, etc), we need to update some informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveClient_PlanChanged(object sender, PlanChangedEventArgs e)
        {
            if(e.Plan == m_plan) UpdateEligibility();
        }

        /// <summary>
        /// Updates eligibility label and planning menus
        /// </summary>
        private void UpdateEligibility()
        {
            Certificate lastEligibleCert = null;
            var certClass = this.certSelectCtl.SelectedCertificateClass;

            if (certClass != null)
            {
                // First we search the highest eligible certificate after this plan
                foreach (var cert in certClass)
                {
                    if (!m_plan.WillGrantEligibilityFor(cert)) break;
                    lastEligibleCert = cert;
                }

                if (lastEligibleCert == null)
                {
                    tslbEligible.Text = "(none)";
                }
                else
                {
                    tslbEligible.Text = lastEligibleCert.Grade.ToString();

                    var highestClaimedCertificate = certClass.HighestClaimedGrade;
                    if (highestClaimedCertificate == null)
                    {
                        tslbEligible.Text += " (improved from \"none\")";
                    }
                    else if ((int)lastEligibleCert.Grade > (int)highestClaimedCertificate.Grade)
                    {
                        tslbEligible.Text += " (improved from \"" + highestClaimedCertificate.Grade.ToString().ToLower() + "\")";
                    }
                    else
                    {
                        tslbEligible.Text += " (no change)";
                    }
                }

                UpdatePlanningMenuStatus(tsPlanToBasic, certClass, CertificateGrade.Basic, lastEligibleCert);
                UpdatePlanningMenuStatus(tsPlanToStandard, certClass, CertificateGrade.Standard, lastEligibleCert);
                UpdatePlanningMenuStatus(tsPlanToImproved, certClass, CertificateGrade.Improved, lastEligibleCert);
                UpdatePlanningMenuStatus(tsPlanToElite, certClass, CertificateGrade.Elite, lastEligibleCert);
            }
        }

        /// <summary>
        /// Updates a "plan to" menu
        /// </summary>
        /// <param name="menu">The menu to update</param>
        /// <param name="certClass">The selected certificate class</param>
        /// <param name="grade">The grade represent by this menu</param>
        /// <param name="lastEligibleCert">The highest eligible certificate after this plan</param>
        private void UpdatePlanningMenuStatus(ToolStripMenuItem menu, CertificateClass certClass, CertificateGrade grade, Certificate lastEligibleCert)
        {
            var cert = certClass[grade];
            if (cert == null)
            {
                menu.Visible = false;
            }
            else
            {
                menu.Visible = true;
                if (lastEligibleCert == null) menu.Enabled = true;
                else menu.Enabled = ((int)cert.Grade > (int)lastEligibleCert.Grade);
            }

        }

        private void tsPlanToBasic_Click(object sender, EventArgs e)
        {
            var operation = m_plan.TryPlanTo(this.certSelectCtl.SelectedCertificateClass[CertificateGrade.Basic]);
            PlanHelper.SelectPerform(operation);
        }

        private void tsPlanToStandard_Click(object sender, EventArgs e)
        {
            var operation = m_plan.TryPlanTo(this.certSelectCtl.SelectedCertificateClass[CertificateGrade.Standard]);
            PlanHelper.SelectPerform(operation);
        }

        private void tsPlanToImproved_Click(object sender, EventArgs e)
        {
            var operation = m_plan.TryPlanTo(this.certSelectCtl.SelectedCertificateClass[CertificateGrade.Improved]);
            PlanHelper.SelectPerform(operation);
        }

        private void tsPlanToElite_Click(object sender, EventArgs e)
        {
            var operation = m_plan.TryPlanTo(this.certSelectCtl.SelectedCertificateClass[CertificateGrade.Elite]);
            PlanHelper.SelectPerform(operation);
        }
    }
}