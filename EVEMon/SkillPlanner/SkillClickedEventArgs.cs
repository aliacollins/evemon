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
        /// <summary>
        /// Initializes a new instance of the <see cref="SkillClickedEventArgs"/> class.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <param name="button">The button.</param>
        /// <param name="location">The location.</param>
        internal SkillClickedEventArgs(Skill skill, MouseButtons button, Point location)
        {
            Skill = skill;
            Button = button;
            Location = location;
        }

        /// <summary>
        /// Gets the skill.
        /// </summary>
        /// <value>The skill.</value>
        public Skill Skill { get; private set; }

        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <value>The button.</value>
        public MouseButtons Button { get; private set; }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        public Point Location { get; private set; }
    }
    #endregion
}
