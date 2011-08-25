using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.BattleClinic
{
    [XmlRoot("error", Namespace = "http://api.battleclinic.com")]
    public sealed class BCAPIError
    {
        [XmlAttribute("code")]
        public string ErrorCode { get; set; }

        [XmlText]
        public string ErrorMessage { get; set; }
    }
}
