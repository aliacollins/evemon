using System;
using System.Drawing;
using System.Windows.Forms;

namespace EVEMon.Common
{
    /// <summary>
    /// A wrapper around <see cref="ToolTip"/> for usage in skill queue control.
    /// </summary>
    public class SkillQueueToolTip
    {
        private readonly ToolTip m_toolTip;
        private readonly Control m_owner;
        private bool m_canceled;
        private Size m_size = new Size(0, 0);
        private string m_text = String.Empty;

        /// <summary>
        /// Initializes <see cref="SkillQueueToolTip"/> instance.
        /// </summary>
        /// <param name="owner">Owner of this tooltip</param>
        public SkillQueueToolTip(Control owner)
        {
            m_owner = owner;
            m_toolTip = new ToolTip { UseFading = false };
            m_toolTip.Popup += m_toolTip_Popup;
        }

        /// <summary>
        /// Handles popup event from ToolTip control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_toolTip_Popup(object sender, PopupEventArgs e)
        {
            // If height of tooltip differs from the previous one, then we 
            // must store it and do not show tooltip at wrong position
            if (!m_size.Equals(e.ToolTipSize))
            {
                m_size = e.ToolTipSize;
                e.Cancel = true;
                m_canceled = true;
                m_text = String.Empty;
                return;
            }

            m_canceled = false;
        }

        /// <summary>
        /// Popup a tool tip for the provided skill, above the given point.
        /// </summary>
        /// <param name="skill">Skill to display</param>
        /// <param name="pt">The pt.</param>
        public void Display(QueuedSkill skill, Point pt)
        {
            const string Format = "{0} {1}\n  Start{2}\t{3}\n  Ends\t{4}";
            string skillName = skill.SkillName;
            string skillLevel = Skill.GetRomanFromInt(skill.Level);
            string skillStart = (skill.Owner.IsTraining
                                     ? skill.StartTime.ToLocalTime().ToAbsoluteDateTimeDescription(DateTimeKind.Local)
                                     : "Paused");
            string skillEnd = (skill.Owner.IsTraining
                                   ? skill.EndTime.ToLocalTime().ToAbsoluteDateTimeDescription(DateTimeKind.Local)
                                   : "Paused");
            string startText = (skill.StartTime < DateTime.UtcNow ? "ed" : "s");
            string text = String.Format(CultureConstants.DefaultCulture, Format, skillName, skillLevel, startText, skillStart,
                                        skillEnd);
            Display(text, pt);
        }

        /// <summary>
        /// Popups a tool tip with provided text, above the given point.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="pt">The pt.</param>
        public void Display(string text, Point pt)
        {
            if (text == m_text)
                return;
            m_text = text;
            m_toolTip.Hide(m_owner);
            m_toolTip.Show(text, m_owner,pt.X - m_size.Width / 2, -m_size.Height);

            // Cancel means new height and new position
            if (!m_canceled)
                return;

            m_toolTip.Hide(m_owner);
            m_toolTip.Show(text, m_owner, pt.X - m_size.Width / 2, -m_size.Height);
        }

        /// <summary>
        /// Hides a tooltip.
        /// </summary>
        public void Hide()
        {
            m_toolTip.Hide(m_owner);
            m_text = String.Empty;
        }

        /// <summary>
        /// Disposes resources.
        /// </summary>
        public void Dispose()
        {
            m_toolTip.Dispose();
        }
    }
}
