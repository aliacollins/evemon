using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    #region SerializableAPIOrderList
    public sealed class SerializableAPIOrderList
    {
        [XmlArray("orders")]
        [XmlArrayItem("order")]
        public List<SerializableAPIOrder> Orders
        {
            get;
            set;
        }
    }
    #endregion
}
