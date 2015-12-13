using System;
using System.Linq;
using EVEMon.Common.Data;

namespace EVEMon.Common.Helpers
{
    public sealed class Loadout : BaseLoadout
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="Loadout"/> class from being created.
        /// </summary>
        public Loadout()
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
    }
}