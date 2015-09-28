using System;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.BattleClinic
{
    public sealed class SerializableFilesListItem
    {
        [XmlAttribute("id")]
        public int FileID { get; set; }

        [XmlAttribute("name")]
        public string FileName { get; set; }

        [XmlAttribute("updated")]
        public string FileUpdatedXml
        {
            get { return FileUpdated.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    FileUpdated = value.TimeStringToDateTime();
            }
        }

        [XmlAttribute("revision")]
        public int FileRevision { get; set; }

        [XmlText]
        public string FileContent { get; set; }

        [XmlIgnore]
        public DateTime FileUpdated { get; set; }
    }
}