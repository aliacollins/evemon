using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.XmlImporter.Zofu
{
    public sealed class DgmAttributeCategory : IHasID
    {
        [XmlElement("categoryID")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement("categoryName")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement("categoryDescription")]
        public string Description
        {
            get;
            set;
        }

    }
}
