using System.Windows.Forms;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class CustomMsgBoxSettings
    {
        public CustomMsgBoxSettings()
        {
            ShowDialogBox = true;
        }

        [XmlAttribute("showDialogBox")]
        public bool ShowDialogBox { get; set; }

        [XmlAttribute("dialogResult")]
        public DialogResult DialogResult { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal CustomMsgBoxSettings Clone()
        {
            return (CustomMsgBoxSettings)MemberwiseClone();
        }
    }
}
