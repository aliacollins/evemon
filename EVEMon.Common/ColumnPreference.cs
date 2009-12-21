using System;
using System.Reflection;
using System.Xml.Serialization;

namespace EVEMon.Common
{
    [XmlRoot("ColumnPreference")]
    public class ColumnPreference : ICloneable
    {
        public class ColumnDisplayAttribute : Attribute
        {
            private string m_text;
            private string m_header;
            private int m_width = -2;

            public string Text
            {
                get { return m_text; }
                set { m_text = value; }
            }

            public string Header
            {
                get { return m_header; }
                set { m_header = value; }
            }

            public int Width
            {
                get { return m_width; }
                set { m_width = value; }
            }

            public ColumnDisplayAttribute(string text)
            {
                m_text = text;
                m_header = text;
            }

            public ColumnDisplayAttribute(string text, string header)
            {
                m_text = text;
                m_header = header;
            }
        }

        public enum ColumnType
        {
            [ColumnDisplay("Skill Name")] SkillName,
            [ColumnDisplay("Plan Group")] PlanGroup,
            [ColumnDisplay("Training Time")] TrainingTime,
            [ColumnDisplay("Training Time (no implants)","Train Time (no implants)")]TrainingTimeNatural,
            [ColumnDisplay("Earliest Start")] EarliestStart,
            [ColumnDisplay("Earliest End")] EarliestEnd,
            [ColumnDisplay("Conflicts")] Conflicts,
            [ColumnDisplay("Percent Complete", "%")] PercentComplete,
            [ColumnDisplay("Skill Rank", "Rank")] SkillRank,
            [ColumnDisplay("Primary Attribute", "Primary")] PrimaryAttribute,
            [ColumnDisplay("Secondary Attribute", "Secondary")] SecondaryAttribute,
            [ColumnDisplay("Skill Group", "Group")] SkillGroup,
            [ColumnDisplay("Notes", "Notes")] Notes,
            [ColumnDisplay("Plan Type (Planned/Prerequisite)", "Type")] PlanType,
            [ColumnDisplay("Estimated Skill Point Total", "Est. SP Total")] SPTotal,
            [ColumnDisplay("SP/Hour", "SP/Hour")] SPPerHour,
            [ColumnDisplay("Priority", "Priority")] Priority,
            [ColumnDisplay("Cost", "Cost")] Cost
        }

        private bool[] m_prefs;

        public ColumnPreference()
        {
            m_prefs = new bool[ColumnCount];
            for (int i = 0; i < m_prefs.Length; i++)
            {
                m_prefs[i] = false;
            }

            this.SkillName = true;
            this.PlanGroup = true;
            this.TrainingTime = true;
            this.EarliestStart = true;
            this.EarliestEnd = true;
            this.Conflicts = true;
            this.Priority = true;
        }

        private string m_order = String.Empty;

        [XmlAttribute("column_order")]
        public string Order
        {
            get { return m_order; }
            set { m_order = value; }
        }

        public static string GetDescription(int index)
        {
            return GetDescription((ColumnType) index);
        }

        public static string GetDescription(ColumnType ct)
        {
            ColumnDisplayAttribute cda = GetAttribute(ct);
            if (cda != null)
            {
                return cda.Text;
            }
            else
            {
                return ct.ToString();
            }
        }

        public static string GetHeader(int index)
        {
            return GetHeader((ColumnType) index);
        }

        public static string GetHeader(ColumnType ct)
        {
            ColumnDisplayAttribute cda = GetAttribute(ct);
            if (cda != null)
            {
                return cda.Header;
            }
            else
            {
                return ct.ToString();
            }
        }

        public static ColumnDisplayAttribute GetAttribute(ColumnType ct)
        {
            MemberInfo[] memInfo = typeof (ColumnType).GetMember(ct.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof (ColumnDisplayAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return (ColumnDisplayAttribute) attrs[0];
                }
            }
            return null;
        }

        [XmlIgnore]
        public static int ColumnCount
        {
            get { return Enum.GetValues(typeof (ColumnType)).Length; }
        }

        [XmlIgnore]
        public bool this[int index]
        {
            get { return m_prefs[index]; }
            set { m_prefs[index] = value; }
        }

        [XmlIgnore]
        public bool this[ColumnType index]
        {
            get { return this[(int) index]; }
            set { this[(int) index] = value; }
        }

        [XmlAttribute]
        public bool SkillName
        {
            get { return this[ColumnType.SkillName]; }
            set { this[ColumnType.SkillName] = value; }
        }

        [XmlAttribute]
        public bool PlanGroup
        {
            get { return this[ColumnType.PlanGroup]; }
            set { this[ColumnType.PlanGroup] = value; }
        }

        [XmlAttribute]
        public bool TrainingTime
        {
            get { return this[ColumnType.TrainingTime]; }
            set { this[ColumnType.TrainingTime] = value; }
        }

        [XmlAttribute]
        public bool TrainingTimeNatural
        {
            get { return this[ColumnType.TrainingTimeNatural]; }
            set { this[ColumnType.TrainingTimeNatural] = value; }
        }

        [XmlAttribute]
        public bool EarliestStart
        {
            get { return this[ColumnType.EarliestStart]; }
            set { this[ColumnType.EarliestStart] = value; }
        }

        [XmlAttribute]
        public bool EarliestEnd
        {
            get { return this[ColumnType.EarliestEnd]; }
            set { this[ColumnType.EarliestEnd] = value; }
        }

        [XmlAttribute]
        public bool Conflicts
        {
            get { return this[ColumnType.Conflicts]; }
            set { this[ColumnType.Conflicts] = value; }
        }


        [XmlAttribute]
        public bool PercentComplete
        {
            get { return this[ColumnType.PercentComplete]; }
            set { this[ColumnType.PercentComplete] = value; }
        }

        [XmlAttribute]
        public bool SkillRank
        {
            get { return this[ColumnType.SkillRank]; }
            set { this[ColumnType.SkillRank] = value; }
        }

        [XmlAttribute]
        public bool PrimaryAttribute
        {
            get { return this[ColumnType.PrimaryAttribute]; }
            set { this[ColumnType.PrimaryAttribute] = value; }
        }

        [XmlAttribute]
        public bool SecondaryAttribute
        {
            get { return this[ColumnType.SecondaryAttribute]; }
            set { this[ColumnType.SecondaryAttribute] = value; }
        }

        [XmlAttribute]
        public bool SkillGroup
        {
            get { return this[ColumnType.SkillGroup]; }
            set { this[ColumnType.SkillGroup] = value; }
        }

        [XmlAttribute]
        public bool Notes
        {
            get { return this[ColumnType.Notes]; }
            set { this[ColumnType.Notes] = value; }
        }

        [XmlAttribute]
        public bool PlanType
        {
            get { return this[ColumnType.PlanType]; }
            set { this[ColumnType.PlanType] = value; }
        }

        [XmlAttribute]
        public bool SPTotal
        {
            get { return this[ColumnType.SPTotal]; }
            set { this[ColumnType.SPTotal] = value; }
        }

        [XmlAttribute]
        public bool SPPerHour
        {
            get { return this[ColumnType.SPPerHour]; }
            set { this[ColumnType.SPPerHour] = value; }
        }

        [XmlAttribute]
        public bool Priority
        {
            get { return this[ColumnType.Priority]; }
            set { this[ColumnType.Priority] = value; }
        }

        [XmlAttribute]
        public bool Owned
        {
            get { return this[ColumnType.Cost]; }
            set { this[ColumnType.Cost] = value; }
        }
	

        #region ICloneable Members
       
        public object Clone()
        {
            ColumnPreference cp = new ColumnPreference();
            for (int i = 0; i < this.m_prefs.Length; i++)
            {
                cp.m_prefs[i] = this.m_prefs[i];
            }
            cp.m_order = (string)this.m_order.Clone();
            return cp;
        }
        
        public void CopyFrom(ColumnPreference cp)
        {
            for (int i = 0; i < this.m_prefs.Length; i++)
            {
                this.m_prefs[i] = cp.m_prefs[i];
            }
            this.m_order = (string)cp.m_order.Clone();
        }
        #endregion
 
    }
}
