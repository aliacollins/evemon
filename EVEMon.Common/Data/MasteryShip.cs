using System;
using System.Linq;
using EVEMon.Common.Collections;
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

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the ship.
        /// </summary>
        public Ship Ship { get; private set; }

        /// <summary>
        /// Gets the highest trained mastery level.
        /// May be null if no level has been trained.
        /// </summary>
        public Mastery HighestTrainedLevel
        {
            get { return Items.LastOrDefault(level => level.IsTrained); }
        }
        
        #endregion

        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        public Mastery GetLevel(int level)
        {
            return Items.FirstOrDefault(mastery => mastery.Level == level);
        }

        /// <summary>
        /// Gets this mastery's ship representation for the provided character.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <exception cref="System.ArgumentNullException">character</exception>
        public void ToCharacter(Character character)
        {
            if (character == null)
                throw new ArgumentNullException("character");

            foreach (Mastery mastery in Items)
            {
                mastery.Reset();
            }

            while (true)
            {
                bool updatedAnything = Items
                    .Aggregate(false, (current, mastery) => current | mastery.TryUpdateMasteryStatus(character));

                if (!updatedAnything)
                    break;
            }
        }
    }
}