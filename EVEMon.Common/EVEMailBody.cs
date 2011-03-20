using System;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class EVEMailBody
    {
        internal EVEMailBody(SerializableMailBodiesListItem src)
        {
            MessageID = src.MessageID;
            BodyText = src.MessageText.Normalize();
        }

        public long MessageID { get; set; }

        public string BodyText { get; set; }
    }
}
