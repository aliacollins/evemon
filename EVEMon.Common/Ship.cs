using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml.Serialization;

namespace EVEMon.Common
{
    public class Ship : EveObject
    {
        private static Ship[] sm_ships = null;

        public static Ship[] GetShips()
        {
            if (sm_ships == null)
            {
                string shipfile = String.Format(
                    "{1}Resources{0}eve-ships2.xml.gz",
                    Path.DirectorySeparatorChar,
                    System.AppDomain.CurrentDomain.BaseDirectory);
                if (!File.Exists(shipfile))
                {
                    throw new ApplicationException(shipfile + " not found!");
                }
                using (FileStream s = new FileStream(shipfile, FileMode.Open, FileAccess.Read))
                using (GZipStream zs = new GZipStream(s, CompressionMode.Decompress))
                {
                    try
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(Ship[]));
                        sm_ships = (Ship[])xs.Deserialize(zs);
                    }
                    catch(InvalidCastException)
                    {
                        // deserialization failed - probably in design mode
                        return null;
                    }
                }
            }
            return sm_ships;
        }


        private string m_race = String.Empty;

        [XmlAttribute]
        public string Race
        {
            get { return m_race; }
            set { m_race = value; }
        }

        private string m_type = String.Empty;

        [XmlAttribute]
        public string Type
        {
            get { return m_type; }
            set { m_type = value; }
        }
    }
 }
