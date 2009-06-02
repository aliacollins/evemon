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
        private static Dictionary<string, Ship> sm_shipDict = null;

        public static Ship[] GetShips()
        {
            if (sm_ships == null)
            {
                string shipfile = Settings.FindDatafile("eve-ships2.xml.gz");
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

        public static Ship GetShip(string name)
        {
            if (sm_shipDict == null)
            {
                if(sm_ships == null)
                    GetShips();
                sm_shipDict = new Dictionary<string, Ship>(sm_ships.Length);
                foreach (Ship s in sm_ships)
                {
                    sm_shipDict[s.Name] = s;
                }
            }
            return sm_shipDict[name];
        }

        public override string GetCategoryPath()
        {
            return this.Type + " > " + this.Race;
        }

        /// <summary>
        /// Gives you all recommended certificates for this ship
        /// </summary>
        /// <returns>Possibly empty List, but not null</returns>
        public List<Certificate> GetRecommendedCertificates()
        {
            return Certificate.GetCertsRecommendedForShip(this._name);
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
