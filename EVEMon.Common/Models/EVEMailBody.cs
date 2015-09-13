using System;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common.Models
{
    public sealed class EveMailBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EveMailBody"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        internal EveMailBody(SerializableMailBodiesListItem src)
        {
            MessageID = src.MessageID;
            BodyText = src.MessageText ?? String.Empty;
        }

        /// <summary>
        /// Gets or sets the message ID.
        /// </summary>
        /// <value>The message ID.</value>
        public long MessageID { get; private set; }

        /// <summary>
        /// Gets or sets the body text.
        /// </summary>
        /// <value>The body text.</value>
        public string BodyText { get; private set; }
    }
}