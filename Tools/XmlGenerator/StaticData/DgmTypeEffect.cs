using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class DgmTypeEffect : IRelation
    {
        [XmlElement("typeID")]
        public long ItemID { get; set; }

        [XmlElement("effectID")]
        public long EffectID { get; set; }


        #region IRelation Members

        /// <summary>
        /// Gets the left column value.
        /// </summary>
        /// <value>The left.</value>
        long IRelation.Left
        {
            get { return ItemID; }
        }

        /// <summary>
        /// Gets the right column value.
        /// </summary>
        /// <value>The right.</value>
        long IRelation.Right
        {
            get { return EffectID; }
        }

        #endregion
    }
}