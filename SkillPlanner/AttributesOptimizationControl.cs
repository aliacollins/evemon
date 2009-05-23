using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using System.Drawing.Imaging;

namespace EVEMon.SkillPlanner
{
    public partial class AttributesOptimizationControl : UserControl
    {
        private readonly Pen m_borderPen;
        private readonly Pen m_outerBorderPen;
        private readonly Brush m_activeBrush;
        private readonly Brush m_inactiveBrush;
        private readonly Brush m_basePointBrush;
        private readonly Brush m_spentPointBrush;
        private readonly AttributesOptimizer.Remapping m_remapping;


        public AttributesOptimizationControl(CharacterInfo character, AttributesOptimizer.Remapping remapping)
        {
            InitializeComponent();
            this.label9.Font = FontHelper.GetDefaultFont(FontStyle.Bold);
            this.label7.Font = FontHelper.GetDefaultFont(FontStyle.Bold);
            this.label5.Font = FontHelper.GetDefaultFont(FontStyle.Bold);
            this.label3.Font = FontHelper.GetDefaultFont(FontStyle.Bold);
            this.label1.Font = FontHelper.GetDefaultFont(FontStyle.Bold);
            this.lbMEM.Font = FontHelper.GetFont("Tahoma", 8.25F);
            this.lbWIL.Font = FontHelper.GetFont("Tahoma", 8.25F);
            this.lbCHA.Font = FontHelper.GetFont("Tahoma", 8.25F);
            this.lbPER.Font = FontHelper.GetFont("Tahoma", 8.25F);
            this.lbINT.Font = FontHelper.GetFont("Tahoma", 8.25F);
            this.lbOptimizedTimeInfo.Font = FontHelper.GetFont("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            this.lbCurrentTimeInfo.Font = FontHelper.GetFont("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            this.lbWarning.Font = FontHelper.GetFont("Microsoft Sans Serif", 8.25F, FontStyle.Bold);

            m_borderPen = new Pen(Brushes.Black);
            m_outerBorderPen = new Pen(Brushes.Gray);
            m_activeBrush = new SolidBrush(Color.FromArgb(208, 208, 208));
            m_inactiveBrush = new SolidBrush(Color.FromArgb(96, 96, 96));
            m_basePointBrush = new SolidBrush(Color.FromArgb(208, 208, 208));
            m_spentPointBrush = new SolidBrush(Color.FromArgb(54, 202, 54));
            m_remapping = remapping;

            UpdateAttributeControls(character, EveAttribute.Perception, lbPER, pbPERBase, pbPERImplants, pbPERSkills);
            UpdateAttributeControls(character, EveAttribute.Willpower, lbWIL, pbWILBase, pbWILImplants, pbWILSkills);
            UpdateAttributeControls(character, EveAttribute.Memory, lbMEM, pbMEMBase, pbMEMImplants, pbMEMSkills);
            UpdateAttributeControls(character, EveAttribute.Intelligence, lbINT, pbINTBase, pbINTImplants, pbINTSkills);
            UpdateAttributeControls(character, EveAttribute.Charisma, lbCHA, pbCHABase, pbCHAImplants, pbCHASkills);

            // Update the current time control
            this.lbCurrentTime.Text = Skill.TimeSpanToDescriptiveText(m_remapping.BaseDuration, DescriptiveTextOptions.IncludeCommas);

            // Update the optimized time control
            this.lbOptimizedTime.Text = Skill.TimeSpanToDescriptiveText(m_remapping.BestDuration, DescriptiveTextOptions.IncludeCommas);

            // Update the time benefit control
            if (m_remapping.BestDuration < m_remapping.BaseDuration)
            {
                this.lbGain.Text = Skill.TimeSpanToDescriptiveText(m_remapping.BaseDuration - m_remapping.BestDuration, 
                    DescriptiveTextOptions.IncludeCommas) + " better than current";
            }
            else
            {
                this.lbGain.Text = "Your skills are already optimized";
            }

            // A plan may not have a years worth of skills in it,
            // only fair to warn the user
            this.lbWarning.Visible = m_remapping.BestDuration < new TimeSpan(365, 0, 0, 0);
        }

        public void CleanUp()
        {
            // Cleanup the brushes on closed
            m_activeBrush.Dispose();
            m_inactiveBrush.Dispose();
            m_basePointBrush.Dispose();
            m_spentPointBrush.Dispose();
            m_outerBorderPen.Dispose();
            m_borderPen.Dispose();

            // Cleanup the generated bitmaps on closed
            foreach (Control ctl in this.Controls)
            {
                PictureBox box = ctl as PictureBox;
                if (box != null && box.Image != null)
                {
                    Image oldImage = box.Image;
                    box.Image = null;
                    oldImage.Dispose();
                }
            }
        }

        public void Update(CharacterInfo character, EveAttributeScratchpad baseScratchpad, EveAttributeScratchpad bestScratchpad)
        {
        }

        private void UpdateAttributeControls(CharacterInfo character, EveAttribute attrib, Label lb, PictureBox pbBase, PictureBox pbImplants, PictureBox pbSkills)
        {
            // Compute base and effective attributes
            double effectiveAttribute = character.GetEffectiveAttribute(attrib, m_remapping.BestScratchpad);
            int oldBaseAttribute = character.GetBaseAttribute(attrib);
            int baseAttribute = oldBaseAttribute + (m_remapping.BestScratchpad.GetAttributeBonus(attrib) - m_remapping.BaseScratchpad.GetAttributeBonus(attrib));
            int implantsBonus = (int)character.getImplantValue(attrib);
            int skillsBonus = (int)effectiveAttribute - (baseAttribute + implantsBonus);

            // Update the label
            lb.Text = effectiveAttribute.ToString("##.##") + " (new : " + baseAttribute.ToString() + " ; old : " + oldBaseAttribute.ToString() + ")";

            // Update the bars
            UpdateAttributeBar(pbBase, baseAttribute, true);
            UpdateAttributeBar(pbImplants, implantsBonus, false);
            UpdateAttributeBar(pbSkills, skillsBonus, false);
        }

        private void UpdateAttributeBar(PictureBox pb, int value, bool isBase)
        {
            // Dispose the old image
            if (pb.Image != null)
            {
                Image oldImg = pb.Image;
                pb.Image = null;
                oldImg.Dispose();
            }

            // Create the new image
            Bitmap bmp = new Bitmap(pb.Width, pb.Height, PixelFormat.Format32bppArgb);

            // Draw the image
            const int tileWidth = 6;
            int tileHeight = pb.Height - 4;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Draw the borders
                g.DrawRectangle(m_outerBorderPen, 0, 0, bmp.Width - 1, bmp.Height - 1);
                g.DrawRectangle(m_borderPen, 1, 1, bmp.Width - 3, bmp.Height - 3);

                // Draw the tiles
                int i = 0;
                int x = 2;
                while (x < bmp.Width - 2)
                {
                    // Draw the tile
                    Brush brush = m_inactiveBrush;
                    if (i < value)
                    {
                        if (isBase) brush = (i < 4 ? m_basePointBrush : m_spentPointBrush);
                        else brush = m_activeBrush;
                    }

                    g.FillRectangle(brush, x, 2, tileWidth, tileHeight);

                    // Draw the tile's border
                    x += tileWidth;
                    g.DrawLine(m_borderPen, x, 2, x, bmp.Height - 2);

                    // Update for next cycle
                    x++;
                    i++;
                }
            }

            pb.Image = bmp;
        }
    }
}
