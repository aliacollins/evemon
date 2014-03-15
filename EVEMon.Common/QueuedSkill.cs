using System;
using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a skill training.
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class QueuedSkill
    {
        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="character">The character for this training</param>
        /// <param name="serial">The serialization object for this training</param>
        /// <param name="isPaused">When true, the training is currently paused.</param>
        /// <param name="startTimeWhenPaused">Training starttime when the queue is actually paused.
        /// Indeed, in such case, CCP returns empty start and end time, so we compute a "what if we start now" scenario.</param>
        internal QueuedSkill(Character character, SerializableQueuedSkill serial, bool isPaused, ref DateTime startTimeWhenPaused)
        {
            Owner = character;
            StartSP = serial.StartSP;
            EndSP = serial.EndSP;
            Level = serial.Level;
            Skill = character.Skills[serial.ID];

            if (!isPaused)
            {
                // Not paused, we should trust CCP
                StartTime = serial.StartTime;
                EndTime = serial.EndTime;
            }
            else
            {
                // StartTime and EndTime were empty on the serialization object if the skill was paused
                // So we compute a "what if we start now" scenario
                StartTime = startTimeWhenPaused;
                if (Skill != null)
                    startTimeWhenPaused += Skill.GetLeftTrainingTimeForLevelOnly(Level);
                EndTime = startTimeWhenPaused;
            }
        }

        /// <summary>
        /// Gets the character training this.
        /// </summary>
        public Character Owner { get; private set; }

        /// <summary>
        /// Gets the trained level.
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Gets the trained skill. May be null if the skill is not in our datafiles.
        /// </summary>
        public Skill Skill { get; private set; }

        /// <summary>
        /// Gets the skill name, or "Unknown skill" if the skill was not in our datafiles.
        /// </summary>
        public string SkillName
        {
            get { return (Skill != null ? Skill.Name : "Unknown Skill"); }
        }

        /// <summary>
        /// Gets the training start time (UTC).
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// Gets the time this training will be completed (UTC).
        /// </summary>
        public DateTime EndTime { get; private set; }

        /// <summary>
        /// Gets the number of SP this skill had when the training started.
        /// </summary>
        public int StartSP { get; private set; }

        /// <summary>
        /// Gets the number of SP this skill will have once the training is over.
        /// </summary>
        public int EndSP { get; private set; }

        /// <summary>
        /// Gets the fraction completed, between 0 and 1.
        /// </summary>
        public float FractionCompleted
        {
            get
            {
                return Skill == null
                    ? 0
                    : Skill == Skill.UnknownSkill
                        ? (float)
                            (1 -
                             EndTime.Subtract(DateTime.UtcNow).TotalMilliseconds / EndTime.Subtract(StartTime).TotalMilliseconds)
                        : Skill.FractionCompleted;
            }
        }

        /// <summary>
        /// Gets the percent completed.
        /// </summary>
        /// <returns></returns>
        public double PercentCompleted
        {
            get
            {
                if (Skill == null || Skill == Skill.UnknownSkill)
                    return FractionCompleted * 100;

                return Level == Skill.Level + 1 ? Skill.PercentCompleted : 0;
            }
        }

        /// <summary>
        /// Computes an estimation of the current SP.
        /// </summary>
        public int CurrentSP
        {
            get
            {
                double estimatedSP = EndSP - (EndTime.Subtract(DateTime.UtcNow)).TotalHours * SkillPointsPerHour;
                return (IsTraining ? Math.Max((int)estimatedSP, StartSP) : StartSP);
            }
        }

        /// <summary>
        /// Gets the rank.
        /// </summary>
        /// <value>
        /// The rank.
        /// </value>
        public long Rank
        {
            get
            {
                if (Skill != Skill.UnknownSkill)
                    return Skill.Rank;

                switch (Level)
                {
                    case 0:
                        return 0;
                    case 1:
                        return EndSP / 250;
                    case 2:
                        return EndSP / 1414;
                    case 3:
                        return EndSP / 8000;
                    case 4:
                        return EndSP / Convert.ToInt32(Math.Ceiling(Math.Pow(2, (2.5 * Level) - 2.5) * 250));
                    case 5:
                        return EndSP / 256000;
                }
                return Skill.Rank;
            }
        }

        /// <summary>
        /// Gets the training speed.
        /// </summary>
        /// <returns></returns>
        public double SkillPointsPerHour
        {
            get
            {
                return Skill == Skill.UnknownSkill
                    ? Math.Ceiling((EndSP - StartSP) / EndTime.Subtract(StartTime).TotalHours)
                    : Skill.SkillPointsPerHour;
            }
        }

        /// <summary>
        /// Computes the remaining time.
        /// </summary>
        /// <value>The remaining time.</value>
        /// <returns> Returns <see cref="TimeSpan.Zero"/> if already completed.</returns>
        public TimeSpan RemainingTime
        {
            get
            {
                TimeSpan left = EndTime.Subtract(DateTime.UtcNow);
                return left < TimeSpan.Zero ? TimeSpan.Zero : left;
            }
        }

        /// <summary>
        /// Gets true if the skill is currently in training.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the skill is training; otherwise, <c>false</c>.
        /// </value>
        public bool IsTraining
        {
            get
            {
                var ccpCharacter = Owner as CCPCharacter;
                return Skill.IsTraining ||
                       (ccpCharacter != null && ccpCharacter.SkillQueue.IsTraining && ccpCharacter.SkillQueue.First() == this);
            }
        }

        /// <summary>
        /// Gets true if the training has been completed, false otherwise.
        /// </summary>
        public bool IsCompleted
        {
            get { return EndTime <= DateTime.UtcNow; }
        }

        /// <summary>
        /// Generates a deserialization object.
        /// </summary>
        /// <returns></returns>
        internal SerializableQueuedSkill Export()
        {
            SerializableQueuedSkill skill = new SerializableQueuedSkill
                                                {
                                                    ID = (Skill == null ? 0 : Skill.ID),
                                                    Level = Level,
                                                    StartSP = StartSP,
                                                    EndSP = EndSP,
                                                };

            // CCP's API indicates paused training skill with missing start and end times
            // Mimicing them is ugly but necessary
            if (!Owner.IsTraining)
                return skill;

            skill.StartTime = StartTime;
            skill.EndTime = EndTime;

            return skill;
        }

        /// <summary>
        /// Gets a string representation of this skill.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(CultureConstants.DefaultCulture, "{0} {1}", SkillName, Skill.GetRomanFromInt(Level));
        }
    }
}