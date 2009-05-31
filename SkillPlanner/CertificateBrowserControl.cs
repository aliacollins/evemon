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
    public partial class CertificateBrowserControl : UserControl
    {
        public CertificateBrowserControl()
        {
            InitializeComponent();
            this.lblName.Font = FontHelper.GetFont("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            this.certSelectCtl.SelectionChanged += new EventHandler<EventArgs>(certSelectCtl_SelectionChanged);
            this.certDisplayCtl.SelectionChanged += new EventHandler(certDisplayCtl_SelectionChanged);
        }

        private CharacterInfo m_character;
        private Plan m_plan = null;

        /// <summary>
        /// Gets or sets the character this control is bound to
        /// </summary>
        public CharacterInfo Character
        {
            get { return m_character; }
            set
            {
                m_character = value;
                this.certSelectCtl.Character = value;
                this.certDisplayCtl.Character = value;
            }
        }

        /// <summary>
        /// Gets or sets the current plan
        /// </summary>
        public Plan Plan
        {
            get { return m_plan; }
            set
            {
                if (m_plan != null) m_plan.Changed -= new EventHandler<EventArgs>(PlanChanged);
                m_plan = value;
                if (m_plan != null) m_plan.Changed += new EventHandler<EventArgs>(PlanChanged);

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

            if (certClass == null)
            {
                panelRight.Visible = false;
            }
            else
            {
                panelRight.Visible = true;
                lblName.Text = certClass.Name;
                lblCategory.Text = certClass.Category.Name;

                // Initialize the labels' text for every existing grade
                Label[] labels = new Label[] { lblLevel1Time, lblLevel2Time, lblLevel3Time, lblLevel4Time };
                int lbIndex = 0;
                foreach (var cert in certClass.Certificates)
                {
                    var label = labels[lbIndex];
                    var time = cert.ComputeTrainingTime(m_character);
                    label.Text = cert.Grade + " : " + Skill.TimeSpanToDescriptiveText(time, DescriptiveTextOptions.IncludeCommas);
                    label.Visible = true;
                    lbIndex++;
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

        }

        /// <summary>
        /// When the display tree's selection changes, we may update the description
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void certDisplayCtl_SelectionChanged(object sender, EventArgs e)
        {
            var cert = certDisplayCtl.SelectedCertificate;
            var certClass = this.certSelectCtl.SelectedCertificateClass;

            // No certificate or not one of the roots ? Then, we display the description for the lowest grade cert
            if (cert == null || cert.Class != certClass)
            {
                var firstCert = certClass.LowestGradeCertificate;
                textboxDescription.Text = (firstCert == null ? "" : firstCert.Description);
            }
            // So, one of our cert class's grades has been selected, we use its description
            else
            {
                textboxDescription.Text = cert.Description;
            }
        }

        /// <summary>
        /// When the current plan changes (new skills, etc), we need to update some informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PlanChanged(object sender, EventArgs e)
        {
            UpdateEligibility();
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
                foreach (var cert in certClass.Certificates)
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

                    var highestClaimedCertificate = certClass.GetHighestGradeClaimedCertificate(m_character);
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
            m_plan.PlanTo(this.certSelectCtl.SelectedCertificateClass[CertificateGrade.Basic], false);
        }

        private void tsPlanToStandard_Click(object sender, EventArgs e)
        {
            m_plan.PlanTo(this.certSelectCtl.SelectedCertificateClass[CertificateGrade.Standard], false);
        }

        private void tsPlanToImproved_Click(object sender, EventArgs e)
        {
            m_plan.PlanTo(this.certSelectCtl.SelectedCertificateClass[CertificateGrade.Improved], false);
        }

        private void tsPlanToElite_Click(object sender, EventArgs e)
        {
            m_plan.PlanTo(this.certSelectCtl.SelectedCertificateClass[CertificateGrade.Elite], false);
        }
    }
}
