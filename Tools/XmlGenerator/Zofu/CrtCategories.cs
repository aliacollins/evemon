using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.XmlImporter.Zofu
{
    public sealed class CrtCategories : IHasID
    {
        [XmlElement("categoryID")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement("description")]
        public string Description;

        [XmlElement("categoryName")]
        public string CategoryName;

    }
}
