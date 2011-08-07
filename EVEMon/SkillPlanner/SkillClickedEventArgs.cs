using System;
using System.Drawing;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    #region SkillClickedHandler
    /// <summary>
    /// Handler for the <see cref="SkillTreeDisplayControl.SkillClicked"/> event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void SkillClickedHandler(object sender, SkillClickedEventArgs e);

    /// <summary>
    /// Arguments for the skill clicked event args
    /// </summary>
    public class SkillClickedEventArgs : EventArgs
    {
        private Skill m_skill;
        private MouseButtons m_button;
        private Point m_location;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillClickedEventArgs"/> class.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <param name="button">The button.</param>
        /// <param name="location">The location.</param>
        internal SkillClickedEventArgs(Skill skill, MouseButtons button, Point location)
        {
            m_skill = skill;
            m_button = button;
            m_location = location;
        }

        /// <summary>
        /// Gets the skill.
        /// </summary>
        /// <value>The skill.</value>
        public Skill Skill
        {
            get { return m_skill; }
        }

        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <value>The button.</value>
        public MouseButtons Button
        {
            get { return m_button; }
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        public Point Location
        {
            get { return m_location; }
        }
    }
    #endregion
}
