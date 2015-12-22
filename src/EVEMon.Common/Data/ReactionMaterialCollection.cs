using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    public sealed class ReactionMaterialCollection : ReadonlyCollection<SerializableReactionInfo>
    {       
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactionMaterialCollection"/> class.
        /// </summary>
        /// <param name="reactionInfo">The reactionInfo.</param>
        internal ReactionMaterialCollection(IEnumerable<SerializableReactionInfo> reactionInfo)
            : base(reactionInfo == null ? 0 : reactionInfo.Count())
        {
            if (reactionInfo == null)
                return;

            foreach (SerializableReactionInfo reaction in reactionInfo)
            {
                Items.Add(reaction);
            }
        }
    }
}