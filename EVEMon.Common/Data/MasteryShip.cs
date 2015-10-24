using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a mastery ship.
    /// </summary>
    public sealed class MasteryShip : ReadonlyCollection<Mastery>
    {
        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="src">The source.</param>
        internal MasteryShip(SerializableMasteryShip src)
            : base(src == null ? 0 : src.Masteries.Count)
        {
            if (src == null)
                return;

            Ship = StaticItems.GetItemByID(src.ID) as Ship;

            foreach (SerializableMastery mastery in src.Masteries)
            {
                Items.Add(new Mastery(this, mastery));
            }
        }

        public void TryUpdateCertificateStatus(Character character)
        {
            bool trained = true;
            int highestLevel = 0;

            // Scan prerequisite skills
            foreach (var mastery in Items)
            {
                trained = true;
                foreach (var skillLevel in mastery.SelectMany(cert => cert.Certificate.PrerequisiteSkills.Where(level => level.Key == (CertificateGrade)mastery.Level).SelectMany(level => level.Value)))
                {
                    var charSkill = character.Skills.FirstOrDefault(s => s.ID == skillLevel.Skill.ID);

                    if(charSkill == null)
                    {                        
                        break;
                    }

                    // Trained only if the skill's level is greater or equal than the minimum level
                    trained &= (charSkill.Level >= skillLevel.Level);
                }

                if(trained)
                {
                    highestLevel = mastery.Level;
                }
            }

            HighestTrainedLevel = Items.FirstOrDefault(m => m.Level == highestLevel);            
        }

        public Mastery HighestTrainedLevel { get; private set; }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the ship.
        /// </summary>
        public Ship Ship { get; private set; }

        #endregion

    }
}