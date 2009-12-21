using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.Zofu
{
    public sealed class DgmTypeEffect
        : IRelation
    {
        [XmlElement("typeID")]
        public int ItemID;

        [XmlElement("effectID")]
        public int EffectID;

        #region IRelation Members
        int IRelation.Left
        {
            get { return ItemID; }
        }

        int IRelation.Right
        {
            get { return EffectID; }
        }
        #endregion
    }
}
