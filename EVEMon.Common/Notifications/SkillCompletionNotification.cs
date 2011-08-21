using System;
using System.Collections.Generic;
using System.Linq;

namespace EVEMon.Common.Notifications
{
    public sealed class SkillCompletionNotification : Notification
    {
        private readonly List<QueuedSkill> m_skills;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="skills"></param>
        public SkillCompletionNotification(Object sender, IEnumerable<QueuedSkill> skills)
            : base(NotificationCategory.SkillCompletion, sender)
        {
            m_skills = new List<QueuedSkill>();
            foreach (QueuedSkill skill in skills)
            {
                m_skills.Add(skill);
            }
            m_skills.Reverse();
            UpdateDescription();
        }

        /// <summary>
        /// Gets the associated API result.
        /// </summary>
        public IEnumerable<QueuedSkill> Skills
        {
            get { return m_skills; }
        }

        /// <summary>
        /// Gets true if the notification has details.
        /// </summary>
        public override bool HasDetails
        {
            get { return m_skills.Count != 1; }
        }

        /// <summary>
        /// Enqueue the skills from the given notification at the end of this notification.
        /// </summary>
        /// <param name="other"></param>
        public override void Append(Notification other)
        {
            List<QueuedSkill> skills = ((SkillCompletionNotification) other).m_skills;
            foreach (QueuedSkill skill in skills.Where(skill => !m_skills.Contains(skill)))
            {
                m_skills.Add(skill);
            }
            UpdateDescription();
        }


        /// <summary>
        /// Updates the description.
        /// </summary>
        private void UpdateDescription()
        {
            m_description = m_skills.Count == 1
                                ? String.Format(CultureConstants.DefaultCulture, "{0} {1} completed.", m_skills[0].SkillName,
                                                Skill.GetRomanFromInt(m_skills[0].Level))
                                : String.Format(CultureConstants.DefaultCulture, "{0} skills completed.", m_skills.Count);
        }
    }
}
