using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml.Serialization;

namespace EVEMon.Common
{
    #region Character implants
    public class ImplantSet
    {
        // This stores the implant details.
        private UserImplant[] m_values;

        public ImplantSet(UserImplant[] a)
            : this()
        {
            a.CopyTo(m_values,0);
        }

        public ImplantSet()
        {
            m_values = new UserImplant[10];
        }

        public UserImplant this[int implant]
        {
            get { return m_values[implant]; }
            set { m_values[implant] = value; }
        }

        public UserImplant[] Array
        {
            get { return m_values; }
            set { m_values = value; }
        }
    }

    // This class is used in the program to store enough info about the users implants to make look up easier
    // Handy as it's fully serializeable in it's native form so can be used to save the implant info in the settings.xml file too.
    [XmlRoot("UserImplant")]
    public class UserImplant
    {
        private int m_slot;

        [XmlAttribute]
        public int Slot
        {
            get { return m_slot; }
            set { m_slot = value; }
        }

        private bool m_tech2;

        [XmlAttribute]
        public bool Tech2
        {
            get { return m_tech2; }
            set { m_tech2 = value; }
        }

        private int m_bonus;

        [XmlAttribute]
        public int Bonus
        {
            get { return m_bonus; }
            set { m_bonus = value; }
        }

        private int m_id;

        [XmlAttribute]
        public int ID
        {
            get { return m_id; }
            set { m_id = value; }
        }

        private string m_name;

        [XmlAttribute]
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private bool m_manual = false;

        [XmlAttribute]
        public bool Manual
        {
            get { return m_manual; }
            set { m_manual = value; }
        }

        public override string ToString()
        {
            return m_name;
        }

        public UserImplant()
        {
            m_slot = -1;
            m_tech2 = false;
            m_bonus = 0;
            m_id = -1;
            m_name = "<None>";
            m_manual = false;
        }

        public UserImplant(UserImplant UI)
        {
            m_slot = UI.Slot;
            m_name = UI.Name;
            m_bonus = UI.Bonus;
            m_manual = UI.Manual;
            m_tech2 = UI.Tech2;
            m_id = UI.ID;
        }

        public UserImplant(int Slot, Implant a)
            : this(Slot, a, false)
        {
        }

        public UserImplant(int Slot, Implant a, bool manual)
        {
            m_slot = Slot;
            m_manual = manual;
            if (a != null)
            {
                m_tech2 = a.Tech2;
                m_bonus = a.Bonus;
                m_id = a.ID;
                m_name = a.Name;
            }
            else
            {
                m_tech2 = false;
                m_bonus = 0;
                m_id = -1;
                m_name = String.Empty;
            }
        }

        public static EveAttribute SlotToAttrib(int slot)
        {
            switch (slot)
            {
                case 1:
                    return EveAttribute.Perception;
                case 2:
                    return EveAttribute.Memory;
                case 3:
                    return EveAttribute.Willpower;
                case 4:
                    return EveAttribute.Intelligence;
                case 5:
                    return EveAttribute.Charisma;
                default:
                    return EveAttribute.None;
            }
        }

        public static int AttribToSlot(EveAttribute attr)
        {
            switch (attr)
            {
                case EveAttribute.Perception:
                    return 1;
                case EveAttribute.Memory:
                    return 2;
                case EveAttribute.Willpower:
                    return 3;
                case EveAttribute.Intelligence:
                    return 4;
                case EveAttribute.Charisma:
                    return 5;
                default:
                    return -1;
            }
        }

        public UserImplant(GrandEveAttributeBonus a, bool T2, int ID)
        {
            m_slot = AttribToSlot(a.EveAttribute);
            m_name = a.Name;
            m_bonus = a.Amount;
            m_manual = a.Manual;
            m_tech2 = T2;
            m_id = ID;
        }
    }
    #endregion

    #region Implants from Implants.xml.gz
    public class Slot
    {
        private int m_slot = -1;

        [XmlAttribute]
        public int Number
        {
            get { return m_slot; }
            set { m_slot = value; }
        }

        private string m_attribute = String.Empty;

        [XmlAttribute("a")]
        public string Attribute
        {
            get { return m_attribute; }
            set { m_attribute = StringTable.GetSharedString(value); }
        }

        private List<Implant> m_Implants = new List<Implant>();
        private Dictionary<int, Implant> m_dictImplants = new Dictionary<int, Implant>();

        [XmlElement("Implant")]
        public List<Implant> ImplantList
        {
            get { return m_Implants; }
            set
            {
                m_Implants = value;
                m_dictImplants.Clear();
                foreach (Implant x in m_Implants)
                {
                    m_dictImplants.Add(x.ID, x);
                }
            }
        }

        [XmlIgnore]
        public Dictionary<int, Implant> ImplantDict
        {
            get
            {
                if (m_dictImplants.Count == 0 && m_Implants.Count != 0)
                {
                    foreach (Implant x in m_Implants)
                    {
                        m_dictImplants.Add(x.ID, x);
                    }
                }
                return m_dictImplants; 
            }
        }

        public Implant this[int implant]
        {
            get { return m_dictImplants[implant]; }
            set { m_dictImplants[implant] = value; }
        }

        public Implant this[string implantName]
        {
            get 
            {
                if (m_dictImplants.Count == 0)
                {
                    foreach (Implant x in m_Implants)
                    {
                        m_dictImplants.Add(x.ID, x);
                    }
                }
                foreach (int x in m_dictImplants.Keys)
                {
                    if (m_dictImplants[x].Name.ToUpper() == implantName.ToUpper())
                        return m_dictImplants[x];
                }
                return null;
            }
        }

        private static Slot[] sm_implants = null;

        public static Slot[] GetImplants()
        {
            if (sm_implants == null)
            {
                string implantfile = String.Format(
                    "{1}Resources{0}eve-implants2.xml.gz",
                    Path.DirectorySeparatorChar,
                    System.AppDomain.CurrentDomain.BaseDirectory);
                if (!File.Exists(implantfile))
                {
                    throw new ApplicationException(implantfile + " not found!");
                }
                using (FileStream s = new FileStream(implantfile, FileMode.Open, FileAccess.Read))
                using (GZipStream zs = new GZipStream(s, CompressionMode.Decompress))
                {
                    try
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(Slot[]));
                        sm_implants = (Slot[])xs.Deserialize(zs);
                    }
                    catch (InvalidCastException)
                    {
                        // deserialization failed - probably in design mode
                        return null;
                    }
                }
            }
            return sm_implants;
        }
    }

    // This class is used to load data from the implant.xml.gz file
    public class Implant
    {
        private bool m_tech2;

        [XmlAttribute]
        public bool Tech2
        {
            get { return m_tech2; }
            set { m_tech2 = value; }
        }

        private int m_bonus;

        [XmlAttribute]
        public int Bonus
        {
            get { return m_bonus; }
            set { m_bonus = value; }
        }

        private int m_id;

        [XmlAttribute]
        public int ID
        {
            get { return m_id; }
            set { m_id = value; }
        }

        private string m_icon;

        [XmlAttribute]
        public string Icon
        {
            get { return m_icon; }
            set { m_icon = StringTable.GetSharedString(value); }
        }

        private string m_name;

        [XmlAttribute]
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private string m_description;

        [XmlAttribute]
        public string Description
        {
            get { return m_description; }
            set { m_description = StringTable.GetSharedString(value); }
        }

        private List<ImplantProperty> m_properties;

        [XmlArrayItem("prop")]
        public List<ImplantProperty> Properties
        {
            get { return m_properties; }
        }

        private List<ImplantRequiredSkill> m_requiredSkills;

        [XmlArrayItem("skill")]
        public List<ImplantRequiredSkill> RequiredSkills
        {
            get { return m_requiredSkills; }
        }

        public override string ToString()
        {
            return m_name;
        }

        public Implant()
        {
            m_tech2 = false;
            m_bonus = 0;
            m_id = -1;
            m_icon = String.Empty;
            m_name = "<None>";
            m_description = String.Empty;
            m_properties = new List<ImplantProperty>();
            m_requiredSkills = new List<ImplantRequiredSkill>();
        }
    }

    public class ImplantProperty
    {
        private string m_name;

        [XmlAttribute]
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private string m_value;

        [XmlAttribute]
        public string Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public override string ToString()
        {
            return m_name + ": " + m_value;
        }

        public ImplantProperty()
        {
            m_name = String.Empty;
            m_value = String.Empty;
        }
    }

    public class ImplantRequiredSkill
    {
        private string m_name;

        [XmlAttribute]
        public string Name
        {
            get { return m_name; }
            set { m_name = StringTable.GetSharedString(value); }
        }

        private int m_level;

        [XmlAttribute]
        public int Level
        {
            get { return m_level; }
            set { m_level = value; }
        }

        public ImplantRequiredSkill()
        {
            m_name = String.Empty;
            m_level = -1;
        }
    }
    #endregion
}
