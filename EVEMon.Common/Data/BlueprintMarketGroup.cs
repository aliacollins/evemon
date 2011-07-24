using System;
using System.Collections.Generic;
using System.Text;

using EVEMon.Common.Serialization.Datafiles;
using EVEMon.Common.Collections;

namespace EVEMon.Common.Data
{
    public sealed class BlueprintMarketGroup : MarketGroup
    {
        #region Constructors

        /// <summary>
        /// Deserialization constructor for root category only.
        /// </summary>
        /// <param name="src"></param>
        public BlueprintMarketGroup(SerializableBlueprintGroup src)
            :base(src)
        {
            SubGroups = new BlueprintMarketGroupCollection(this, src.SubGroups);
            Blueprints = new BlueprintCollection(this, src.Blueprints);
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="src"></param>
        public BlueprintMarketGroup(BlueprintMarketGroup parent, SerializableBlueprintGroup src)
            : base(parent, src)
        {
            SubGroups = new BlueprintMarketGroupCollection(this, src.SubGroups);
            Blueprints = new BlueprintCollection(this, src.Blueprints);
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the sub categories.
        /// </summary>
        public new BlueprintMarketGroupCollection SubGroups { get; private set; }

        /// <summary>
        /// Gets the blueprints in this category.
        /// </summary>
        public BlueprintCollection Blueprints { get; private set; }

        /// <summary>
        /// Gets the collection of all the blueprints in this category and its descendants.
        /// </summary>
        public IEnumerable<Blueprint> AllBlueprints
        {
            get
            {
                foreach (Blueprint blueprint in Blueprints)
                {
                    yield return blueprint;
                }

                foreach (BlueprintMarketGroup cat in SubGroups)
                {
                    foreach (Blueprint subBlueprint in cat.AllBlueprints)
                    {
                        yield return subBlueprint;
                    }
                }
            }
        }

        #endregion

    }
}
