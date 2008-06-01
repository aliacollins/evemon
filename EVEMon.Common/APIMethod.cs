using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.Common
{
    public class APIMethod
    {
        private APIMethods _method;
        private string _path;

        public APIMethod()
        {}

        public APIMethod(APIMethods method, string path)
        {
            _method = method;
            _path = path;
        }

        [XmlIgnore]
        public string Name
        {
            get
            {
                return _method.ToString();
            }
        }

        [XmlAttribute]
        public APIMethods Method
        {
            get { return _method; }
            set { _method = value; }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
    }
}
