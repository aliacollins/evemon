using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.PieChart;
using System.Collections;

namespace EVEMon
{
    public partial class SkillsPieChart : EVEMonForm
    {
        private Settings m_settings;
        public string active_character;
        public string plan_key;
        private CharacterInfo character_info = null;

        public SkillsPieChart(CharacterInfo character_info)
        {
            InitializeComponent();
            this.character_info = character_info;

            m_settings = Settings.GetInstance();
            skillPieChartControl.LeftMargin = 20F;
            skillPieChartControl.TopMargin = 15F;
            skillPieChartControl.RightMargin = 20F;
            skillPieChartControl.BottomMargin = 15F;
            skillPieChartControl.FitChart = true;
            skillPieChartControl.SliceRelativeHeight = m_settings.SkillPieChartSliceRelativeHeight;
            skillPieChartControl.InitialAngle = m_settings.SkillPieChartInitialAngle;
            sortBySizeCheck.Checked = m_settings.SkillPieChartSortBySize;
            mergeMinorCheck.Checked = m_settings.SkillPieChartMergeMinorGroups;
            pieHeight.Value = (decimal)m_settings.SkillPieChartSliceRelativeHeight;
            pieAngle.Value = (decimal)m_settings.SkillPieChartInitialAngle;
            skillPieChartControl.ShadowStyle = ShadowStyle.GradualShadow;
            skillPieChartControl.EdgeColorType = EVEMon.PieChart.EdgeColorType.DarkerThanSurface;
            skillPieChartControl.EdgeLineWidth = 1F;
            if (m_settings.SkillPieChartColors.Count < character_info.SkillGroups.Count)
            {
                int alpha = 125;
                List<Color> newColors = new List<Color>();
                while (newColors.Count < character_info.SkillGroups.Count)
                {
                    newColors.Add(Color.FromArgb(alpha, Color.Red));
                    newColors.Add(Color.FromArgb(alpha, Color.Green));
                    newColors.Add(Color.FromArgb(alpha, Color.Blue));
                }
                skillPieChartControl.Colors = newColors.ToArray();
            }
            else
            {
                List<Color> loadedColors = new List<Color>();
                foreach (SerializableColor c in m_settings.SkillPieChartColors)
                {
                    loadedColors.Add(c.ToColor());
                }
                skillPieChartControl.Colors = loadedColors.ToArray();
            }
        }

        private void UpdatePieChart()
        {
            // To avoid trouble during startup
            if (active_character == null)
                return;

            SerializableCharacterSheet c_info = m_settings.GetCharacterSheet(active_character);

            // Retrieve the selected Plan
            Plan plan = null;
            if (planSelector.SelectedIndex != 0)
            {
                plan = m_settings.GetPlanByName(plan_key, character_info, planSelector.Items[planSelector.SelectedIndex].ToString());
            }

            // Init Pie Chart
            decimal[] newValues = new decimal[c_info.SkillGroups.Count];
            string[] newTexts = new string[c_info.SkillGroups.Count];
            string[] newToolTips = new string[c_info.SkillGroups.Count];
            float[] newSliceRelativeDisplacements = new float[c_info.SkillGroups.Count];
            int[] skillGroupIds = new int[c_info.SkillGroups.Count];

            int tinyGroups = 1;
            for (int i = 0; i < c_info.SkillGroups.Count; i++)
            {
                SerializableSkillGroup sg = c_info.SkillGroups[i];
                newValues[i] = 0;
                newValues[i] += sg.GetTotalPoints();
                newTexts[i] = sg.Name;
                skillGroupIds[i] = sg.Id;

                long partialSkillPoints = 0;
                long plannedSkillPoints = 0;

                if (plan != null)
                {
                    // Check all Plan Entries
                    foreach (Plan.Entry entry in plan.Entries)
                    {
                        // Does this plan entry match the current skill group?
                        if (entry.Skill.SkillGroup.ID == sg.Id)
                        {
                            Skill skill = entry.Skill;

                            // Add Partially Trained Points
                            partialSkillPoints += skill.CurrentSkillPoints;

                            // Add Points at Planed Level
                            plannedSkillPoints += skill.GetPointsRequiredForLevel(entry.Level);

                        }
                    }

                    // Check if we have to add the planned skill points to the tooltip
                    if (plannedSkillPoints > 0)
                    {
                        newToolTips[i] = sg.Name + " (" + sg.Skills.Count + " skills, " + String.Format("{0:#,###}", newValues[i]) + " skillpoints / " + String.Format("{0:#,###}", newValues[i] + plannedSkillPoints - partialSkillPoints) + " after plan completion)";
                    }
                    else
                    {
                        newToolTips[i] = sg.Name + " (" + sg.Skills.Count + " skills, " + String.Format("{0:#,###}", newValues[i]) + " skillpoints)";
                    }

                    // Update newValues with the skill point changes
                    newValues[i] -= partialSkillPoints;
                    newValues[i] += plannedSkillPoints;
                }
                else
                {
                    // Normal Tooltip when no Plan is selected
                    newToolTips[i] = sg.Name + " (" + sg.Skills.Count + " skills, " + String.Format("{0:#,###}", newValues[i]) + " skillpoints)";
                }

                newSliceRelativeDisplacements[i] = (newValues[i] < 100000) ? 0.06F + (0.008F * ++tinyGroups) : 0.05F;                
            }

            ArrayList skillGroupsToCheck = new ArrayList();
            if (plan != null)
            {
                // Check all Plan Entries
                foreach (Plan.Entry entry in plan.Entries)
                {
                    bool found = false;
                    foreach (int id in skillGroupIds)
                    {
                        if (entry.Skill.SkillGroup.ID == id)
                        {
                            found = true;
                            break;
                        }
                    }

                    // We don't have this skill in the character sheet yet. Add it!
                    if (!found && !skillGroupsToCheck.Contains(entry.Skill.SkillGroup.ID))
                    {
                        skillGroupsToCheck.Add(entry.Skill.SkillGroup.ID);
                    }
                }

                decimal[] newAddValues = new decimal[skillGroupsToCheck.Count];
                string[] newAddTexts = new string[skillGroupsToCheck.Count];
                string[] newAddToolTips = new string[skillGroupsToCheck.Count];
                float[] newAddSliceRelativeDisplacements = new float[skillGroupsToCheck.Count];

                for (int i = 0; i < skillGroupsToCheck.Count; i++ )
                {
                    newAddValues[i] = 0;

                    // Stores all skill ids so we know the unique number of skills
                    ArrayList skills = new ArrayList();

                    foreach (Plan.Entry entry in plan.Entries)
                    {
                         if (entry.Skill.SkillGroup.ID == (int)skillGroupsToCheck[i])
                         {
                             Skill skill = entry.Skill;

                             newAddValues[i] += skill.GetPointsForLevelOnly(entry.Level, false);

                             if (newAddTexts[i] == null)
                             {
                                 newAddTexts[i] = skill.SkillGroup.Name;
                             }

                             if (!skills.Contains(entry.Skill.Id))
                             {
                                 skills.Add(entry.Skill.Id);
                             }
                         }
                        
                    }

                    newAddToolTips[i] = newAddTexts[i] + " (" + skills.Count + " skills with " + String.Format("{0:#,###}", newAddValues[i]) + " skillpoints after plan completion)";
                    newAddSliceRelativeDisplacements[i] = (newAddValues[i] < 100000) ? 0.06F + (0.008F * ++tinyGroups) : 0.05F;                
                }

                // Merge Both Arrays
                int mergedCount = newValues.Length + newAddValues.Length;
                decimal[] final_newValues = new decimal[mergedCount];
                string[] final_newTexts = new string[mergedCount];
                string[] final_newToolTips = new string[mergedCount];
                float[] final_newSliceRelativeDisplacements = new float[mergedCount];

                for (int i = 0; i < newValues.Length; i++)
                {
                    final_newValues[i] = newValues[i];
                    final_newTexts[i] = newTexts[i];
                    final_newToolTips[i] = newToolTips[i];
                    final_newSliceRelativeDisplacements[i] = newSliceRelativeDisplacements[i];
                }

                for (int i = newValues.Length; i < mergedCount; i++)
                {
                    final_newValues[i] = newAddValues[i - newValues.Length];
                    final_newTexts[i] = newAddTexts[i - newValues.Length];
                    final_newToolTips[i] = newAddToolTips[i - newValues.Length];
                    final_newSliceRelativeDisplacements[i] = newAddSliceRelativeDisplacements[i - newValues.Length];
                }

                newValues = final_newValues;
                newTexts = final_newTexts;
                newToolTips = final_newToolTips;
                newSliceRelativeDisplacements = final_newSliceRelativeDisplacements;
            }

            if (m_settings.SkillPieChartMergeMinorGroups)
            {
                decimal totalValue = 0;
                for (int i = 0; i < newValues.Length; i++)
                {
                    totalValue += newValues[i];
                }
                decimal tresholdValue = 0.01m * totalValue;

                int mergedCount = 0;
                for (int i = 0; i < newValues.Length; i++)
                {
                    if (newValues[i] > tresholdValue)
                        mergedCount++;
                }

                if (mergedCount < newValues.Length)
                {
                    // Add the "Other" slice
                    mergedCount++;

                    decimal[] merged_newValues = new decimal[mergedCount];
                    string[] merged_newTexts = new string[mergedCount];
                    string[] merged_newToolTips = new string[mergedCount];
                    float[] merged_newSliceRelativeDisplacements = new float[mergedCount];

                    decimal otherValue = 0;
                    string otherText = "Other";
                    string otherToolTip = "";
                    float otherSliceRelativeDisplacement = 0;

                    int newindex = 0;
                    bool firstother = true;
                    for (int oldindex = 0; oldindex < newValues.Length; oldindex++)
                    {
                        if (newValues[oldindex] > tresholdValue)
                        {
                            merged_newValues[newindex] = newValues[oldindex];
                            merged_newTexts[newindex] = newTexts[oldindex];
                            merged_newToolTips[newindex] = newToolTips[oldindex];
                            merged_newSliceRelativeDisplacements[newindex] = newSliceRelativeDisplacements[oldindex];
                            newindex++;
                        }
                        else
                        {
                            otherValue += newValues[oldindex];

                            if (firstother)
                                firstother = false;
                            else
                                otherToolTip += '\n';
                            otherToolTip += newToolTips[oldindex];
                        }
                    }
                    otherSliceRelativeDisplacement = (otherValue < 100000) ? 0.06F + (0.008F * ++tinyGroups) : 0.05F;

                    merged_newValues[merged_newValues.Length - 1] = otherValue;
                    merged_newTexts[merged_newValues.Length - 1] = otherText;
                    merged_newToolTips[merged_newValues.Length - 1] = otherToolTip;
                    merged_newSliceRelativeDisplacements[merged_newValues.Length - 1] = otherSliceRelativeDisplacement;

                    newValues = merged_newValues;
                    newTexts = merged_newTexts;
                    newToolTips = merged_newToolTips;
                    newSliceRelativeDisplacements = merged_newSliceRelativeDisplacements;
                }
            }

            skillPieChartControl.Values = newValues;
            skillPieChartControl.Texts = newTexts;
            skillPieChartControl.ToolTips = newToolTips;
            skillPieChartControl.SliceRelativeDisplacements = newSliceRelativeDisplacements;

            //skillPieChartControl.CopyDataToDrawVars();
            skillPieChartControl.OrderSlices(m_settings.SkillPieChartSortBySize);

            skillPieChartControl.AngleChange += new EventHandler(skillPieChartControl_AngleChange);
        }

        void skillPieChartControl_AngleChange(object sender, EventArgs e)
        {
            pieAngle.Value = (decimal) (e as AngleChangeEventArgs).NewAngle;
        }

        private void SkillsPieChart_Load(object sender, EventArgs e)
        {
            RememberPositionKey = "SkillsPieChart";

            // Init Plans Combox Box                        
            foreach (string plan in m_settings.GetPlansForCharacter(plan_key))
            {
                planSelector.Items.Add(plan);
            }
            planSelector.SelectedIndex = 0;

            UpdatePieChart();
        }
        private void SkillsPieChart_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_settings.SkillPieChartColors.Clear();
            foreach (Color c in skillPieChartControl.Colors)
            {
                m_settings.SkillPieChartColors.Add(new SerializableColor(c));
            }
            m_settings.SkillPieChartSliceRelativeHeight = (float)pieHeight.Value;
            m_settings.SkillPieChartInitialAngle = (float)pieAngle.Value;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pieHeight_ValueChanged(object sender, EventArgs e)
        {
            skillPieChartControl.SliceRelativeHeight = (float)pieHeight.Value;
        }

        private void pieAngle_ValueChanged(object sender, EventArgs e)
        {
            skillPieChartControl.InitialAngle = (float)pieAngle.Value;
        }

        private void skillPieChartControl_DoubleClick(object sender, EventArgs e)
        {
            MouseEventArgs ev = (MouseEventArgs)e;
            PieChart3D m_pieChart = skillPieChartControl.PieChart;
            int index = m_pieChart.FindPieSliceUnderPoint(new PointF(ev.X, ev.Y));
            if (index != -1 && m_colorDialog.ShowDialog() == DialogResult.OK)
            {
                if (sortBySizeCheck.Checked)
                {
                    int realIndex = skillPieChartControl.GetIndex(index);
                    skillPieChartControl.Colors[realIndex] = Color.FromArgb(125, m_colorDialog.Color);
                }
                else
                {
                    skillPieChartControl.Colors[index] = Color.FromArgb(125, m_colorDialog.Color);
                }
                skillPieChartControl.OrderSlices(sortBySizeCheck.Checked);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Bitmap pie = new Bitmap(skillPieChartControl.Width, skillPieChartControl.Height);
            Rectangle bounds = new Rectangle(0, 0, skillPieChartControl.Width, skillPieChartControl.Height);
            skillPieChartControl.DrawToBitmap(pie, bounds);
            DialogResult savePieResult = savePieDialog.ShowDialog();
            if (savePieResult == DialogResult.OK)
            {
                pie.Save(savePieDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void sortBySizeCheck_CheckedChanged(object sender, EventArgs e)
        {
            m_settings.SkillPieChartSortBySize = sortBySizeCheck.Checked;
            // skillPieChartControl.CopyDataToDrawVars();
            skillPieChartControl.OrderSlices(sortBySizeCheck.Checked);
        }

        private void planSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePieChart();
        }

        private void mergeMinorCheck_CheckedChanged(object sender, EventArgs e)
        {
            m_settings.SkillPieChartMergeMinorGroups = mergeMinorCheck.Checked;
            UpdatePieChart();
        }
    }
}
