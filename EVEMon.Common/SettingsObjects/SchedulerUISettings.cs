using System.Drawing;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class SchedulerUISettings
    {
        public SchedulerUISettings()
        {
            TextColor = (SerializableColor)Color.White;
            BlockingColor = (SerializableColor)Color.Red;
            SimpleEventGradientStart = (SerializableColor)Color.Blue;
            SimpleEventGradientEnd = (SerializableColor)Color.LightBlue;
            RecurringEventGradientStart = (SerializableColor)Color.Green;
            RecurringEventGradientEnd = (SerializableColor)Color.LightGreen;
        }

        [XmlElement("textColor")]
        public SerializableColor TextColor { get; set; }

        [XmlElement("blockColor")]
        public SerializableColor BlockingColor { get; set; }

        [XmlElement("recurringEventGradientStart")]
        public SerializableColor RecurringEventGradientStart { get; set; }

        [XmlElement("recurringEventGradientEnd")]
        public SerializableColor RecurringEventGradientEnd { get; set; }

        [XmlElement("simpleEventGradientStart")]
        public SerializableColor SimpleEventGradientStart { get; set; }

        [XmlElement("simpleEventGradientEnd")]
        public SerializableColor SimpleEventGradientEnd { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal SchedulerUISettings Clone()
        {
            return new SchedulerUISettings
                       {
                           TextColor = TextColor.Clone(),
                           BlockingColor = BlockingColor.Clone(),
                           SimpleEventGradientStart = SimpleEventGradientStart.Clone(),
                           SimpleEventGradientEnd = SimpleEventGradientEnd.Clone(),
                           RecurringEventGradientStart = RecurringEventGradientStart.Clone(),
                           RecurringEventGradientEnd = RecurringEventGradientEnd.Clone()
                       };
        }
    }
}