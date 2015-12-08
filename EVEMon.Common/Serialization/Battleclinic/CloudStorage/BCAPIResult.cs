using System;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;
using EVEMon.Common.Interfaces;

namespace EVEMon.Common.Serialization.BattleClinic.CloudStorage
{
    [XmlRoot("BattleClinicApi")]
    public sealed class BCAPIResult<T>
    {
        [XmlElement("cacheExpires")]
        public string CacheExpiresXml
        {
            get { return CacheExpires.DateTimeToTimeString(); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                CacheExpires = value.TimeStringToDateTime();
            }
        }

        [XmlElement("result")]
        public T Result { get; set; }

        [XmlElement("error")]
        public BCAPIError Error { get; set; }

        [XmlIgnore]
        public DateTime CacheExpires { get; set; }

        [XmlIgnore]
        public bool HasError
        {
            get { return Error != null; }
        }
    }
}