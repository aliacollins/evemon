using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.PieChart;

namespace EVEMon
{
    public partial class SkillsPieChart : Form
    {
        private Settings m_settings;
        public string active_character;

        public SkillsPieChart()
        {
            InitializeComponent();
            m_settings = Settings.GetInstance();
            skillPieChartControl.LeftMargin = 20F;
            skillPieChartControl.TopMargin = 15F;
            skillPieChartControl.RightMargin = 20F;
            skillPieChartControl.BottomMargin = 15F;
            skillPieChartControl.FitChart = true;
            skillPieChartControl.SliceRelativeHeight = m_settings.SkillPieChartSliceRelativeHeight;
            skillPieChartControl.InitialAngle = m_settings.SkillPieChartInitialAngle;
            sortBySizeCheck.Checked = m_settings.SkillPieChartSortBySize;
            pieHeight.Value = (decimal)m_settings.SkillPieChartSliceRelativeHeight;
            pieAngle.Value = (decimal)m_settings.SkillPieChartInitialAngle;
            skillPieChartControl.ShadowStyle = ShadowStyle.GradualShadow;
            skillPieChartControl.EdgeColorType = EVEMon.PieChart.EdgeColorType.DarkerThanSurface;
            skillPieChartControl.EdgeLineWidth = 1F;
            if (m_settings.SkillPieChartColors.Count < 15)
            {
                int alpha = 125;
                skillPieChartControl.Colors = new Color[] {
                    Color.FromArgb(alpha, Color.Red),
                    Color.FromArgb(alpha, Color.Green),
                    Color.FromArgb(alpha, Color.Blue),
                    Color.FromArgb(alpha, Color.Red),
                    Color.FromArgb(alpha, Color.Green),
                    Color.FromArgb(alpha, Color.Blue),
                    Color.FromArgb(alpha, Color.Red),
                    Color.FromArgb(alpha, Color.Green),
                    Color.FromArgb(alpha, Color.Blue),
                    Color.FromArgb(alpha, Color.Red),
                    Color.FromArgb(alpha, Color.Green),
                    Color.FromArgb(alpha, Color.Blue),
                    Color.FromArgb(alpha, Color.Red),
                    Color.FromArgb(alpha, Color.Green),
                    Color.FromArgb(alpha, Color.Blue)
                };
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

        private void SkillsPieChart_Load(object sender, EventArgs e)
        {
            SerializableCharacterSheet c_info = m_settings.GetCharacterSheet(active_character);
            decimal[] newValues = new decimal[c_info.SkillGroups.Count];
            string[] newTexts = new string[c_info.SkillGroups.Count];
            string[] newToolTips = new string[c_info.SkillGroups.Count];
            float[] newSliceRelativeDisplacements = new float[c_info.SkillGroups.Count];

            decimal[] n_newValues = new decimal[c_info.SkillGroups.Count];
            string[] n_newTexts = new string[c_info.SkillGroups.Count];
            string[] n_newToolTips = new string[c_info.SkillGroups.Count];
            float[] n_newSliceRelativeDisplacements = new float[c_info.SkillGroups.Count];

            int tinyGroups = 1;
            for (int i = 0; i < c_info.SkillGroups.Count; i++)
            {
                SerializableSkillGroup sg = c_info.SkillGroups[i];
                newValues[i] = 0;
                newValues[i] += sg.GetTotalPoints();
                newTexts[i] = sg.Name;
                newSliceRelativeDisplacements[i] = (newValues[i] < 100000) ? 0.06F + (0.008F * ++tinyGroups) : 0.05F;
                newToolTips[i] = sg.Name + " (" + sg.Skills.Count + " skills, " + String.Format("{0:#,###}", newValues[i]) + " skillpoints)";
            }

            skillPieChartControl.Values = newValues;
            skillPieChartControl.Texts = newTexts;
            skillPieChartControl.ToolTips = newToolTips;
            skillPieChartControl.SliceRelativeDisplacements = newSliceRelativeDisplacements;

            //skillPieChartControl.CopyDataToDrawVars();
            skillPieChartControl.OrderSlices(m_settings.SkillPieChartSortBySize);
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
    }
}
