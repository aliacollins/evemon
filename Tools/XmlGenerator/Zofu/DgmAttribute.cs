using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.XmlImporter.Zofu
{
    public sealed class DgmAttribute : IHasID
    {
        [XmlElement("attributeID")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement("description")]
        public string Description;

        [XmlElement("attributeName")]
        public string Name;

        [XmlElement("displayName")]
        public string DisplayName;

        [XmlElement("defaultValue")]
        public string DefaultValue;

        [XmlElement("graphicID")]
        public Nullable<int> GraphicID;

        [XmlElement("unitID")]
        public Nullable<int> UnitID;

        [XmlElement("categoryID")]
        public Nullable<int> CategoryID;

        [XmlElement("highIsGood")]
        public bool HigherIsBetter;

    }
}
