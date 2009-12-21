using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using System.Drawing;
using System.Globalization;

namespace EVEMon
{
    /// <summary>
    /// A wrapper around <see cref="ToolTip"/> for usage in skill queue control.
    /// </summary>
    internal class SkillQueueToolTip
    {
        ToolTip m_toolTip;
        Control m_owner;
        bool m_canceled = false;
        Size m_size = new Size(0, 0);
        string m_text = "";

        /// <summary>
        /// Initializes <see cref="SkillQueueToolTip"/> instance.
        /// </summary>
        /// <param name="owner">Owner of this tooltip</param>
        public SkillQueueToolTip(Control owner)
        {
            m_owner = owner;
            m_toolTip = new ToolTip();
            m_toolTip.UseFading = false;
            m_toolTip.Popup += new PopupEventHandler(m_toolTip_Popup);
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
                m_text = "";
                return;
            }

            m_canceled = false;
        }

        /// <summary>
        /// Popup a tool tip for the provided skill, above the given point.
        /// </summary>
        /// <param name="skill">Skill to display</param>
        public void Display(QueuedSkill skill, Point pt)
        {
            string format = "{0} {1}\n  Start{2}\t{3}\n  Ends\t{4}";
            string skillName = skill.Skill.Name;
            string skillLevel = Skill.GetRomanForInt(skill.Level);
            string startText = (skill.StartTime < DateTime.UtcNow ? "ed" : "s");
            string skillStart = TimeExtensions.ToAbsoluteDateTimeDescription(skill.StartTime.ToLocalTime());
            string skillEnd = TimeExtensions.ToAbsoluteDateTimeDescription(skill.EndTime.ToLocalTime());
            string text = String.Format(CultureInfo.CurrentCulture, format, skillName, skillLevel, startText, skillStart, skillEnd);
            Display(text, pt);
        }

        /// <summary>
        /// Popups a tool tip with provided text, above the given point.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rect"></param>
        public void Display(string text, Point pt)
        {
            if (text == m_text)
                return;
            m_text = text;
            m_toolTip.Hide(m_owner);
            m_toolTip.Show(text, m_owner, pt.X - m_size.Width / 2, -m_size.Height);

            // Cancel means new height and new position
            if (m_canceled)
            {
                m_toolTip.Hide(m_owner);
                m_toolTip.Show(text, m_owner, pt.X - m_size.Width / 2, -m_size.Height);
            }
        }

        /// <summary>
        /// Hides a tooltip.
        /// </summary>
        public void Hide()
        {
            m_toolTip.Hide(m_owner);
            m_text = "";
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
