using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EVEMon.Common.Notifications
{
    public sealed class SkillCompletionNotificationEventArgs : NotificationEventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="skills"></param>
        public SkillCompletionNotificationEventArgs(Object sender, IEnumerable<QueuedSkill> skills)
            : base(sender, NotificationCategory.SkillCompletion)
        {
            if (skills == null)
                throw new ArgumentNullException("skills");

            Skills = new Collection<QueuedSkill>();
            foreach (QueuedSkill skill in skills)
            {
                Skills.Add(skill);
            }
            UpdateDescription();
        }

        /// <summary>
        /// Gets the associated API result.
        /// </summary>
        public Collection<QueuedSkill> Skills { get; private set; }

        /// <summary>
        /// Gets true if the notification has details.
        /// </summary>
        public override bool HasDetails
        {
            get { return Skills.Count != 1; }
        }

        /// <summary>
        /// Enqueue the skills from the given notification at the end of this notification.
        /// </summary>
        /// <param name="other"></param>
        public override void Append(NotificationEventArgs other)
        {
            List<QueuedSkill> skills = ((SkillCompletionNotificationEventArgs)other).Skills.ToList();
            foreach (QueuedSkill skill in skills.Where(skill => !Skills.Contains(skill)))
            {
                Skills.Add(skill);
            }
            UpdateDescription();
        }

        /// <summary>
        /// Updates the description.
        /// </summary>
        private void UpdateDescription()
        {
            Description = Skills.Count == 1
                              ? String.Format(CultureConstants.DefaultCulture, "{0} {1} completed.", Skills[0].SkillName,
                                              Skill.GetRomanFromInt(Skills[0].Level))
                              : String.Format(CultureConstants.DefaultCulture, "{0} skills completed.", Skills.Count);
        }
    }
}