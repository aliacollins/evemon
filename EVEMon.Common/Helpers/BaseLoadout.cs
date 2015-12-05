using System;
using System.Collections.Generic;
using EVEMon.Common.Data;

namespace EVEMon.Common.Helpers
{
    public abstract class BaseLoadout
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int ID { get; protected set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name of the loadout.
        /// </value>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The loadout description.
        /// </value>
        public string Description { get; protected set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>
        /// The author.
        /// </value>
        public string Author { get; protected set; }

        /// <summary>
        /// Gets or sets the rating.
        /// </summary>
        /// <value>
        /// The rating.
        /// </value>
        public double Rating { get; protected set; }

        /// <summary>
        /// Gets or sets the topic identifier.
        /// </summary>
        /// <value>
        /// The topic identifier.
        /// </value>
        public int TopicID { get; protected set; }

        /// <summary>
        /// Gets or sets the submission date.
        /// </summary>
        /// <value>
        /// The submission date.
        /// </value>
        public DateTimeOffset SubmissionDate { get; protected set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IEnumerable<Item> Items { get; protected internal set; }
    }
}