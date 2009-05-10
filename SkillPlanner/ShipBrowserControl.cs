using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EVEMon.Common;
using System.IO;

namespace EVEMon.SkillPlanner
{
    public partial class ShipBrowserControl : EveObjectBrowserSimple
    {
        public ShipBrowserControl()
        {
            InitializeComponent();
            this.scObjectBrowser.RememberDistanceKey = "ShipBrowser";
            this.ObjectSelectControl = this.shipSelectControl;
            InitializeDisplayControl();
            PlanChanged += new EventHandler(ShipBrowserControl_PlanChanged);
        }

        protected override void DisplayItemDetails(EveObject item)
        {
            base.DisplayItemDetails(item);

            Ship s = (Ship)item;
            int shipId = s.Id;

            lvShipProperties.BeginUpdate();
            try
            {
                // remove excess columns that might have been added by 'compare with' earlier
                while (lvShipProperties.Columns.Count > 2)
                {
                    lvShipProperties.Columns.RemoveAt(2);
                }

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
                    {
                        continue;
                    }

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
                                {
                                    newItem.SubItems.Add("");
                                }
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
                                {
                                    newItem.SubItems.Add("");
                                }
                                newItem.SubItems.Add(removeNegative(prop.Value));
                            }

                        }
                    }


                    // mark properties with changed value in blue
                    foreach (ListViewItem li in lvShipProperties.Items)
                    {
                        if (li.SubItems.Count > 1 && li.SubItems.Count != lvShipProperties.Columns.Count)
                        {
                            li.BackColor = Color.LightBlue;
                        }
                        else
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
            }
            finally
            {
                lvShipProperties.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                lvShipProperties.EndUpdate();
            }

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
                m_DisplayAttributes.add(new AttributeDisplayData(false, "Meta Level", "Meta Level", false, false));
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
                m_DisplayAttributes.add(new AttributeDisplayData(false, "Sub System Slots", "Sub System Slots", false, false));
                // Attributes - drones
                m_DisplayAttributes.add(new AttributeDisplayData(true, "=Drones", "Drones", false, true));
                m_DisplayAttributes.add(new AttributeDisplayData(false, "Drone Capacity", "Drone Capacity", false, true));
                m_DisplayAttributes.add(new AttributeDisplayData(false, "Drone Bandwidth", "Drone Bandwidth", false, true));
                // Attributes - structure
                m_DisplayAttributes.add(new AttributeDisplayData(true,"=Structure","Structure",false,true));
                m_DisplayAttributes.add(new AttributeDisplayData(false,"hp","Structure Hitpoints",false,true));
                m_DisplayAttributes.add(new AttributeDisplayData(false,"Capacity","Cargo Capacity",false,true));
                m_DisplayAttributes.add(new AttributeDisplayData(false, "Corporate Hangar Capacity", "Corporate Hangar Capacity", false, false));
                m_DisplayAttributes.add(new AttributeDisplayData(false, "Ship Maintenance Bay Capacity", "Ship Maintenance Bay Capacity", false, false));
                m_DisplayAttributes.add(new AttributeDisplayData(false,"Mass","Mass",false,true));
                m_DisplayAttributes.add(new AttributeDisplayData(false,"Volume","Volume",false,true));
                m_DisplayAttributes.add(new AttributeDisplayData(false,"EM dmg resistance","EM Dmg Resistance",true,false));
                m_DisplayAttributes.add(new AttributeDisplayData(false,"Explosive dmg resistance","Explosive Dmg Resistance",true,false));
                m_DisplayAttributes.add(new AttributeDisplayData(false,"Kinetic dmg resistance","Kinetic Dmg Resistance",true,false));
                m_DisplayAttributes.add(new AttributeDisplayData(false,"Thermal dmg resistance","Thermal Dmg Resistance",true,false));
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
                m_DisplayAttributes.add(new AttributeDisplayData(false, "Inertia Modifier", "Inertia Modifier", false, false));
                m_DisplayAttributes.add(new AttributeDisplayData(false, "Warp Speed Multiplier", "Warp Speed Multiplier", false, false));
                m_DisplayAttributes.add(new AttributeDisplayData(false, "Maximum Jump Range", "Maximum Jump Range", false, false));
                m_DisplayAttributes.add(new AttributeDisplayData(false, "Jump Drive Capacitor Need", "Jump Drive Capacitor Need", false, false));
                m_DisplayAttributes.add(new AttributeDisplayData(false, "Jump Drive Fuel Need", "Jump Drive Fuel Need", false, false));
                m_DisplayAttributes.add(new AttributeDisplayData(false, "Jump Drive Consumption Amount", "Jump Drive Consumption Amount", false, false));
                m_DisplayAttributes.add(new AttributeDisplayData(true,"=Other","Other",false,true));
            }
            
        }

        private static string m_propName;
        private static bool findShipProperty(EntityProperty p)
        {
            return p.Name.Equals(ShipBrowserControl.m_propName);
        }

        // helper function remove any -signs from ship attributes
        private String removeNegative(String s)
        {
            if (s.StartsWith("-"))
            {
                return s.Substring(1);
            }
            else
            {
                return s;
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

        private void lblBattleclinic_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            NewPlannerWindow npw = Plan.PlannerWindow.Target as NewPlannerWindow;
            if (npw == null) return;
            if (npw.LoadoutForm == null || npw.LoadoutForm.IsDisposed)
            {
                npw.LoadoutForm = new LoadoutSelect(shipSelectControl.SelectedObject as Ship, this.Plan);
            }
            else
            {
                npw.LoadoutForm.SetShip(shipSelectControl.SelectedObject as Ship);
            }
            npw.LoadoutForm.Show();
        }

        void ShipBrowserControl_PlanChanged(object sender, EventArgs e)
        {
            try
            {
                if (Plan != null)
                {
                    NewPlannerWindow npw = Plan.PlannerWindow.Target as NewPlannerWindow;
                    if (npw == null) return;
                    if (npw.LoadoutForm != null)
                    {
                        npw.LoadoutForm.SetPlan(Plan);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e) {
            ListViewExporter.CreateCSV(lvShipProperties);
        }


    }
}


