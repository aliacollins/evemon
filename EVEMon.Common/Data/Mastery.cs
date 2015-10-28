using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a mastery level.
    /// </summary>
    public sealed class Mastery : ReadonlyCollection<MasteryCertificate>
    {
        private bool m_updated;

        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="masteryShip">The mastery ship.</param>
        /// <param name="src">The source.</param>
        internal Mastery(MasteryShip masteryShip, SerializableMastery src)
            : base(src == null ? 0 : src.Certificates.Count)
        {
            if (src == null)
                return;

            MasteryShip = masteryShip;
            Level = src.Grade;
            Status = MasteryStatus.Untrained;

            foreach (SerializableMasteryCertificate certificate in src.Certificates)
            {
                Items.Add(new MasteryCertificate(this, certificate));
            }
        }

        #endregion
        

        #region Public Properties

        /// <summary>
        /// Gets the mastery ship.
        /// </summary>
        public MasteryShip MasteryShip { get; private set; }

        /// <summary>
        /// Gets the level.
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public MasteryStatus Status { get; private set; }

        /// <summary>
        /// Gets true whether the mastery is trained.
        /// </summary>
        /// <value>
        /// <c>true</c> if this mastery is trained; otherwise, <c>false</c>.
        /// </value>
        public bool IsTrained
        {
            get { return Status == MasteryStatus.Trained; }
        }

        /// <summary>
        /// Gets true whether the mastery is partially trained.
        /// </summary>
        /// <value>
        /// <c>true</c> if this mastery is partilly trained; otherwise, <c>false</c>.
        /// </value>
        public bool IsPartiallyTrained
        {
            get { return Status == MasteryStatus.PartiallyTrained; }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the prerequisite skills.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns></returns>
        public IEnumerable<SkillLevel> GetPrerequisiteSkills(Character character)
        {
            return Items.SelectMany(cert => cert.Certificate.PrerequisiteSkills
                .Where(level => (int)level.Key == Level)
                .SelectMany(level => level.Value.ToCharacter(character))).Distinct();
        }

        /// <summary>
        /// Gets the training time.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns></returns>
        public TimeSpan GetTrainingTime(Character character)
        {
            return character.GetTrainingTimeToMultipleSkills(GetPrerequisiteSkills(character));
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            Status = MasteryStatus.Untrained;
            m_updated = false;
        }

        /// <summary>
        /// Tries the update mastery level status.
        /// </summary>
        /// <param name="character">The character.</param>
        public bool TryUpdateMasteryStatus(Character character)
        {
            if (m_updated)
                return false;

            bool noPrereq = true;
            bool trained = true;

            // Scan prerequisite skills
            foreach (SkillLevel prereqSkill in GetPrerequisiteSkills(character))
            {
                // Trained only if the skill's level is greater or equal than the minimum level
                trained &= (prereqSkill.Skill.Level >= prereqSkill.Level);

                noPrereq &= prereqSkill.AllDependencies.All(x => !x.IsTrained);
            }

            // Updates status
            if (trained)
                Status = MasteryStatus.Trained;
            else if (noPrereq)
                Status = MasteryStatus.Untrained;
            else
                Status = MasteryStatus.PartiallyTrained;

            m_updated = true;
            return true;
        }


        #endregion
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format(CultureConstants.DefaultCulture, "Level {0}", Skill.GetRomanFromInt((Level)));
        }
    }
}