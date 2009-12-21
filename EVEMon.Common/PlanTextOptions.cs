using System;
using System.Xml.Serialization;

namespace EVEMon.Common
{
    [XmlRoot]
    public class PlanTextOptions : ICloneable
    {
        private bool m_includeHeader = true;

        [XmlAttribute]
        public bool IncludeHeader
        {
            get { return m_includeHeader; }
            set { m_includeHeader = value; }
        }

        private bool m_entryNumber = true;

        [XmlAttribute]
        public bool EntryNumber
        {
            get { return m_entryNumber; }
            set { m_entryNumber = value; }
        }

        private bool m_entryTrainingTimes = true;

        [XmlAttribute]
        public bool EntryTrainingTimes
        {
            get { return m_entryTrainingTimes; }
            set { m_entryTrainingTimes = value; }
        }

        private bool m_entryStartDate;

        [XmlAttribute]
        public bool EntryStartDate
        {
            get { return m_entryStartDate; }
            set { m_entryStartDate = value; }
        }

        private bool m_entryFinishDate;

        [XmlAttribute]
        public bool EntryFinishDate
        {
            get { return m_entryFinishDate; }
            set { m_entryFinishDate = value; }
        }

        [XmlAttribute]
        private bool m_entryCost;

        public bool EntryCost
        {
            get { return m_entryCost; }
            set { m_entryCost = value; }
        }
	

        private bool m_footerCount;

        [XmlAttribute]
        public bool FooterCount
        {
            get { return m_footerCount; }
            set { m_footerCount = value; }
        }

        private bool m_footerTotalTime;

        [XmlAttribute]
        public bool FooterTotalTime
        {
            get { return m_footerTotalTime; }
            set { m_footerTotalTime = value; }
        }

        private bool m_footerDate;

        [XmlAttribute]
        public bool FooterDate
        {
            get { return m_footerDate; }
            set { m_footerDate = value; }
        }

        private bool m_footerCost;
        [XmlAttribute]
        public bool FooterCost
        {
            get { return m_footerCost; }
            set { m_footerCost = value; }
        }

        private MarkupType m_markupType;

        /// <summary>
        /// Output markup type.
        /// </summary>
        public MarkupType Markup
        {
            get { return m_markupType; }
            set { m_markupType = value; }
        }

        private bool m_shoppingList;

        /// <summary>
        /// If <code>true</code>, known skills are filtered out.
        /// </summary>
        public bool ShoppingList
        {
            get { return m_shoppingList; }
            set { m_shoppingList = value; }
        }

        #region ICloneable Members
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
