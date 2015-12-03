using System;
using System.Linq;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.BattleClinic.Loadout;

namespace EVEMon.Common.Helpers
{
    public sealed class Loadout : BaseLoadout
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="Loadout"/> class from being created.
        /// </summary>
        private Loadout()
        {
            ID = 0;
            Name = String.Empty;
            Description = String.Empty;
            Author = String.Empty;
            Rating = 0;
            SubmissionDate = DateTimeOffset.MinValue;
            Items = Enumerable.Empty<Item>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loadout"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        internal Loadout(string name, string description)
            : this()
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loadout"/> class.
        /// </summary>
        /// <param name="loadout">The loadout.</param>
        public Loadout(SerializableLoadout loadout)
        {
            ID = loadout.ID;
            Name = loadout.Name;
            Description = String.Empty;
            Author = loadout.Author;
            Rating = loadout.Rating;
            TopicID = loadout.TopicID;
            SubmissionDate = loadout.SubmissionDate;
            Items = Enumerable.Empty<Item>();
        }
    }
}