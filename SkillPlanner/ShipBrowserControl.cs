using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    public partial class ShipBrowserControl : EveObjectBrowserControl
    {
        private Plan m_plan;
        public Plan Plan
        {
            get { return m_plan; }
            set { 
                m_plan = value;
                shipSelectControl.Plan = value;
            }
        }

        private bool m_allSkillsKnown;
        private bool m_skillsUnplanned;

        private bool m_showImages;

        public ShipBrowserControl()
        {
            InitializeComponent();
            this.scShipSelect.RememberDistanceKey = "ShipBrowser";
            shipSelectControl_SelectedShipChanged(null, null);
            m_showImages = !Settings.GetInstance().WorksafeMode;
            InitializeDisplayControl();

        }

        private  static AttributeDisplayControl m_DisplayAttributes = null;
        
        private void InitializeDisplayControl()
        {
            if (m_DisplayAttributes == null)
            {
                m_DisplayAttributes = new AttributeDisplayControl();

               // Add the attributes to the class in the order that they should be displayed
                
                // Price
                m_DisplayAttributes.add(new AttributeDisplayData(false,"Base price","Base price",false,true));
                m_DisplayAttributes.add(new AttributeDisplayData(false, "Tech Level", "Tech Level", false, false));
                // Fitting
        	    m_DisplayAttributes.add(new AttributeDisplayData(true,"=Fitting","Fitting",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"CPU Output","CPU",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"powergrid Output","Powergrid",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Calibration","Calibration",false,true));
                m_DisplayAttributes.add(new AttributeDisplayData(false,"High Slots", "High Slots", false, true));
                m_DisplayAttributes.add(new AttributeDisplayData(false,"Med Slots","Med Slots",false,true));
                m_DisplayAttributes.add(new AttributeDisplayData(false,"Low Slots", "Low Slots", false, true));
                m_DisplayAttributes.add(new AttributeDisplayData(false,"Launcher hardpoints","Launcher Hardpoints",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Turret hardpoints","Turret Hardpoints",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Rig Slots","Rig Slots",false,true));
        	    // Attributes - structure
        	    m_DisplayAttributes.add(new AttributeDisplayData(true,"=Structure","Structure",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"hp","Structure Hitpoints",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Capacity","Capacity",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Drone Capacity","Drone Capacity",false,false));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Mass","Mass",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Volume","Volume",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"EM dmg resistance","EM Dmg Resistance",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Explosive dmg resistance","Explosive Dmg Resistance",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Kinetic dmg resistance","Kinetic Dmg Resistance",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Thermal dmg resistance","Thermal Dmg Resistance",false,true));
        	    // Attributes - Armor"Attributes - Armor
        	    m_DisplayAttributes.add(new AttributeDisplayData(true,"=Armor","Armor",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Armor Hitpoints","Armor Hitpoints",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Armor Em Damage Resistance","Armor Em Damage Resistance",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Armor Explosive Damage Resistance","Armor Explosive Damage Resistance",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Armor Kinetic Damage Resistance","Armor Kinetic Damage Resistance",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Armor Thermal Damage Resistance","Armor Thermal Damage Resistance",false,true));
        	    // Attributes - Shield"Attributes - Shield
        	    m_DisplayAttributes.add(new AttributeDisplayData(true,"=Shield","Shield",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Shield Capacity","Shield Capacity",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Shield recharge time","Shield Recharge Time",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Shield Em Damage Resistance","Shield Em Damage Resistance",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Shield Explosive Damage Resistance","Shield Explosive Damage Resistance",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Shield Kinetic Damage Resistance","Shield Kinetic Damage Resistance",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Shield Thermal Damage Resistance","Shield Thermal Damage Resistance",false,true));
        	    // Attributes - cap"Attributes - cap
        	    m_DisplayAttributes.add(new AttributeDisplayData(true,"=Cap","Capacitor",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Capacitor Capacity","Capacitor Capacity",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Recharge time","Recharge Time",false,true));
        	    // Attributes - Targeting
        	    m_DisplayAttributes.add(new AttributeDisplayData(true,"=Targeting","Targeting",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Maximum Targeting Range","Maximum Targeting Range",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Max  Locked Targets","Max Locked Targets",false,false));
                m_DisplayAttributes.add(new AttributeDisplayData(false,"Targeting Speed", "Targeting Speed", false, false));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Scan Resolution","Scan Resolution",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Gravimetric Sensor Strength","Gravimetric Sensor Strength",true,false));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"LADAR Sensor Strength","LADAR Sensor Strength",true,false));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Magnetometric Sensor Strength","Magnetometric Sensor Strength",true,false));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"RADAR Sensor Strength","RADAR Sensor Strength",true,false));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Signature Radius","Signature Radius",false,true));
        	    // Attributes - Propulsion
        	    m_DisplayAttributes.add(new AttributeDisplayData(true,"=Propulsion","Propulsion",false,true));
        	    m_DisplayAttributes.add(new AttributeDisplayData(false,"Max Velocity","Max Velocity",false,true));
                m_DisplayAttributes.add(new AttributeDisplayData(false, "agility", "Agility", false, false));
        	    m_DisplayAttributes.add(new AttributeDisplayData(true,"=Other","Other",false,true));
            }
            
            if (this.DesignMode)
            {
                return;
            }
 
            if (!m_showImages)
            {
                panel3.Size = new Size(0, this.Size.Height - lvShipProperties.Size.Height);
            }
        }

        private static string m_propName;
        private static bool findShipProperty(EntityProperty p)
        {
            return p.Name.Equals(ShipBrowserControl.m_propName);
        }

        public Ship SelectedShip
        {
            set
            {
                shipSelectControl.SelectedObject = value;
                shipSelectControl_SelectedShipChanged(this, null);
            }
        }

        private void shipSelectControl_SelectedShipChanged(object sender, EventArgs e)
        {
            Settings settings = Settings.GetInstance();
            if (m_showImages)
            {
                Bitmap b = new Bitmap(256, 256);
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.FillRectangle(Brushes.Black, new Rectangle(0, 0, 256, 256));
                }
                pbShipImage.Image = b;
            }
            if (shipSelectControl.SelectedObject != null)
            {
                Ship s = shipSelectControl.SelectedObject as Ship;
                int shipId = s.Id;

                if (m_showImages)
                {
                    if (System.IO.File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\Optional\\Ships256_256.resources"))
                    {
                        System.Resources.IResourceReader basic;
                        basic = new System.Resources.ResourceReader(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\Optional\\Ships256_256.resources");
                        System.Collections.IDictionaryEnumerator basicx = basic.GetEnumerator();
                        while (basicx.MoveNext())
                        {
                            if (basicx.Key.ToString() == "_" + s.Id)
                            {
                                pbShipImage.Image = (System.Drawing.Image)basicx.Value;
                            }
                        }
                    }
                    else
                    {
                        EveSession.GetImageAsync(
                            "http://www.eve-online.com/bitmaps/icons/itemdb/shiptypes/256_256/" +
                            shipId.ToString() + ".png", true, delegate(EveSession ss, Image i)
                                                                  {
                                                                      GotShipImage(shipId, i);
                                                                  });
                    }
                }

                lblShipClass.Text = s.Type + " > " + s.Race;
                lblShipName.Text = s.Name;
                lblShipDescription.Text = Regex.Replace(s.Description, "<.+?>", String.Empty, RegexOptions.Singleline);
                // force the label to fit the panel
                pnlShipDescription_Changed(null, null);

                m_allSkillsKnown = true;
                m_skillsUnplanned = false;

                SetShipSkillLabel(0, lblShipSkillA, s.RequiredSkills);
                SetShipSkillLabel(1, lblShipSkillB, s.RequiredSkills);
                SetShipSkillLabel(2, lblShipSkillC, s.RequiredSkills);

                if (! m_allSkillsKnown)
                {
                    List<Pair<Skill, int>> reqSkills = new List<Pair<Skill, int>>();
                    foreach (EntityRequiredSkill srs in s.RequiredSkills)
                    {
                        Pair<Skill, int> p = new Pair<Skill, int>();
                        p.A = m_plan.GrandCharacterInfo.GetSkill(srs.Name);
                        p.B = srs.Level;
                        reqSkills.Add(p);
                    }
                    TimeSpan trainTime = m_plan.GrandCharacterInfo.GetTrainingTimeToMultipleSkills(reqSkills);
                    lblShipTimeRequired.Text = "Training Time: " +
                                               Skill.TimeSpanToDescriptiveText(trainTime,
                                                                                    DescriptiveTextOptions.IncludeCommas |
                                                                                    DescriptiveTextOptions.SpaceText);
                    
                }
                else
                {
                    lblShipTimeRequired.Text = String.Empty;
                }
                if (m_skillsUnplanned)
                {
                    btnShipSkillsAdd.Enabled = true;
                }
                else
                {
                    btnShipSkillsAdd.Enabled = false;
                }

                lvShipProperties.BeginUpdate();
                try
                {
                    // remove excess columns that might have been added by 'compare with' earlier
                    while (lvShipProperties.Columns.Count > 2)
                        lvShipProperties.Columns.RemoveAt(2);

                    // (re)construct ship properties list
                    lvShipProperties.Items.Clear();

                    // display the properties in a logical sequence

                    ListViewItem listItem = null;
                    for (int i = 0; i < m_DisplayAttributes.Count; i++)
                    {
                        AttributeDisplayData att = m_DisplayAttributes[i];
                        if (att.isHeader)
                        {
                            listItem = new ListViewItem(att.displayName);
                            listItem.BackColor = Color.LightGray;
                            lvShipProperties.Items.Add(listItem);
                        }
                        else
                        {
                            m_propName = att.xmlName;
                            EntityProperty sp = s.Properties.Find(findShipProperty);
                            if (sp != null)
                            {
                                if (att.hideIfZero && sp.Value.StartsWith("0"))
                                {
                                    continue;
                                }
                                listItem = new ListViewItem(new string[] { att.displayName, removeNegative(sp.Value) });
                                listItem.Name = sp.Name;
                                lvShipProperties.Items.Add(listItem);
                            }
                            else if (att.alwaysShow)
                            {
                                listItem = new ListViewItem(new string[] { att.displayName, "0" });
                                listItem.Name = att.xmlName;
                                lvShipProperties.Items.Add(listItem);
                            }
                        }
                    }

                    // Display any properties not shown in the sorted list
                    foreach (EntityProperty prop in s.Properties)
                    {
                        // make sure we haven't already displayed this property
                        if (!m_DisplayAttributes.Contains(prop.Name))
                        {
                            listItem = new ListViewItem(new string[] { prop.Name, removeNegative(prop.Value) });
                            listItem.Name = prop.Name;
                            lvShipProperties.Items.Add(listItem);
                        }
                    }

                    // Add compare with columns (if additional selections)
                    for (int i_ship = 0; i_ship < shipSelectControl.SelectedObjects.Count; i_ship++)
                    {
                        Ship selectedShip = shipSelectControl.SelectedObjects[i_ship] as Ship;
                        // Skip if it's the mothership or not a ship
                        if (selectedShip == shipSelectControl.SelectedObject || selectedShip == null)
                            continue;

                        // add new column header and values
                        lvShipProperties.Columns.Add(selectedShip.Name);

                        // use the  display logic to get any new attributes in the right order

                        int lastpos = 0;
                        for (int i = 0; i < m_DisplayAttributes.Count; i++)
                        {
                            AttributeDisplayData att = m_DisplayAttributes[i];

                            if (att.isHeader) continue;

                            m_propName = att.xmlName;
                            EntityProperty sp = selectedShip.Properties.Find(findShipProperty);
                            if (sp != null)
                            {
                                // found a property to display. Does it exist already?
                                int pos = AddAnotherValue(sp.Name, sp.Value);
                                if (pos >= 0)
                                {
                                    lastpos = pos;
                                }
                                else
                                {
                                    // attribute wasn't in the list - add it if needed
                                    if (att.hideIfZero && sp.Value.StartsWith("0"))
                                    {
                                        // nope. 
                                        continue;
                                    }

                                    // adding a previously missing property
                                    int skipColumns = lvShipProperties.Columns.Count - 2;
                                    ListViewItem newItem = lvShipProperties.Items.Insert(lastpos + 1, sp.Name);
                                    newItem.Name = sp.Name;
                                    while (skipColumns-- > 0)
                                        newItem.SubItems.Add("");
                                    newItem.SubItems.Add(removeNegative(sp.Value));

                                }
                            }
                            else if (att.alwaysShow)
                            {
                                // Ship is missing a displayed attribute - fake one!
                                lastpos = AddAnotherValue(att.xmlName, "0");
                            }
                        }

                        // we've dealt with the formatted, ordered list, now add the rest

                        // Display any properties not shown in the sorted list
                        foreach (EntityProperty prop in selectedShip.Properties)
                        {
                            // make sure we haven't already displayed this property
                            if (!m_DisplayAttributes.Contains(prop.Name))
                            {
                                ListViewItem[] items = lvShipProperties.Items.Find(prop.Name, false);
                                if (items.Length != 0)
                                {
                                    // existing property
                                    ListViewItem oldItem = items[0];
                                    oldItem.SubItems.Add(removeNegative(prop.Value));
                                }
                                else
                                {
                                    int skipColumns = lvShipProperties.Columns.Count - 2;
                                    ListViewItem newItem = lvShipProperties.Items.Add(prop.Name);
                                    newItem.Name = prop.Name;
                                    while (skipColumns-- > 0)
                                        newItem.SubItems.Add("");
                                    newItem.SubItems.Add(removeNegative(prop.Value));
                                }

                            }
                        }


                        // mark properties with changed value in blue
                        foreach (ListViewItem li in lvShipProperties.Items)
                        {
                            for (int i = 2; i < li.SubItems.Count; i++)
                            {
                                if (li.SubItems[i - 1].Text.CompareTo(li.SubItems[i].Text) != 0)
                                {
                                    li.BackColor = Color.LightBlue;
                                    break;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    lvShipProperties.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent); 
                    lvShipProperties.EndUpdate();
                }

                foreach (Control c in scShipSelect.Panel2.Controls)
                {
                    c.Visible = true;
                }
            }
            else
            {
                foreach (Control c in scShipSelect.Panel2.Controls)
                {
                    c.Visible = false;
                }
            }
        }

        // helper function remove any -signs from ship attributes
        private String removeNegative(String s)
        {
            if (s.StartsWith("-")) return s.Substring(1);
            else return s;
        }

        private void SetShipSkillLabel(int rnum, LinkLabel skillLabel, List<EntityRequiredSkill> list)
        {
            if (list.Count > rnum)
            {
                Skill gs = m_plan.GrandCharacterInfo.GetSkill(list[rnum].Name);
                skillLabel.Tag = gs;

                string addText = String.Empty;
                if (gs.Level >= list[rnum].Level)
                {
                    addText = " (Known)";
                }
                else if (Plan.IsPlanned(gs, list[rnum].Level))
                {
                    addText = " (Planned)";
                    m_allSkillsKnown = false;
                }
                else
                {
                    m_allSkillsKnown = false;
                    m_skillsUnplanned = true;
                }
                skillLabel.Text = list[rnum].Name + " " +
                                  Skill.GetRomanForInt(list[rnum].Level) + addText;
            }
            else
            {
                skillLabel.Text = String.Empty;
                skillLabel.Tag = null;
            }
        }

        private void GotShipImage(int shipId, Image i)
        {
            if (i == null)
            {
                return;
            }
            if (shipSelectControl.SelectedObject == null)
            {
                return;
            }
            if (shipId != shipSelectControl.SelectedObject.Id)
            {
                return;
            }
            pbShipImage.Image = i;
        }

        
        private void btnShipSkillsAdd_Click(object sender, EventArgs e)
        {
            Ship s = shipSelectControl.SelectedObject as Ship;
            if (s == null)
            {
                return;
            }

            string m_note = s.Name;
            List<Pair<string, int>> skillsToAdd = new List<Pair<string, int>>();
            foreach (EntityRequiredSkill srs in s.RequiredSkills)
            {
                skillsToAdd.Add(new Pair<string, int>(srs.Name, srs.Level));
            }
            m_plan.PlanSetTo(skillsToAdd, m_note, true);
            shipSelectControl_SelectedShipChanged(new Object(), new EventArgs());
        }

        private void pnlShipDescription_Changed(object sender, EventArgs e)
        {
            int w = pnlShipDescription.ClientSize.Width;
            lblShipDescription.MaximumSize = new Size(w, Int32.MaxValue);
            if (lblShipDescription.PreferredHeight > pnlShipDescription.ClientSize.Height)
            {
                pnlShipDescription.Visible = false;
                pnlShipDescription.PerformLayout();
                int xw = pnlShipDescription.ClientSize.Width;
                lblShipDescription.MaximumSize = new Size(xw, Int32.MaxValue);
                pnlShipDescription.Visible = true;
            }
        }

        private int AddAnotherValue(string name, string value)
        {
            ListViewItem[] items = lvShipProperties.Items.Find(name, false);
            if (items.Length != 0)
            {
                // existing property - add our value to it.
                ListViewItem oldItem = items[0];
                oldItem.SubItems.Add(removeNegative(value));
                return lvShipProperties.Items.IndexOf(items[0]);
            }
            return -1;
        }

        private void lblShipSkill_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel lbl = sender as LinkLabel;
            if (lbl.Tag == null) return;
            NewPlannerWindow pw = m_plan.PlannerWindow.Target as NewPlannerWindow;
            pw.ShowSkillInTree(lbl.Tag as Skill);

        }

        private void lblShipSkill_MouseHover(object sender, EventArgs e)
        {
            LinkLabel lbl = sender as LinkLabel;
            if (lbl.Tag == null) return;
            Skill s = lbl.Tag as Skill;
            StringBuilder prereqs = new StringBuilder(lbl.Text + "\n");
            prereqs.Append(buildPrereqs(s));
            ttShip.SetToolTip(lbl, prereqs.ToString());
            ttShip.IsBalloon = false;
            ttShip.Active = true;
        }

        private StringBuilder buildPrereqs(Skill s)
        {
            StringBuilder msg = new StringBuilder();
            foreach (Skill.Prereq p in s.Prereqs)
            {
                msg.Append(p.Name + " " + Skill.GetRomanForInt(p.Level));
                Skill gs = m_plan.GrandCharacterInfo.GetSkill(p.Name);
                if (gs.Level >= p.Level)
                {
                    msg.Append(" (Known)");
                }
                else if (Plan.IsPlanned(gs, p.Level))
                {
                    msg.Append(" (Planned)");
                }
                msg.Append("\n");
                msg.Append(buildPrereqs(gs));
            }
            return msg;
        }
    }
}









