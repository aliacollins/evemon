using System;
using System.Collections.Generic;
using System.Text;

namespace EVEMon.SkillPlanner
{

    /// <summary>
    /// Class that holds a collection of AttributeDisplayData structures for a ship or item browser.
    /// Theattributes should be added to the collection in the sequence that you want them to be displayed.
    /// 
    /// The structure is keyed on the xml property name and the class also maintains the display sequence using an indexer
    /// e.g  if your class is called x, then AttributeDisplayData y = x[3] will get the structure for the 4th item displayed.  
    /// and AttributeDisplayData y = x.GetAttributesForProperty["hp"] will get the structure for the hitpoint property.
    /// 
    /// Any attributes for  a specific item that are in the XML file but not in the collection should still be displayed in the browser.
    /// </summary>
    

    public class AttributeDisplayControl
    {
        private Dictionary<String, AttributeDisplayData> m_data;
        private List<String> m_displaySequence = new List<String>();

        public AttributeDisplayControl()
        {
            m_data = new Dictionary<string, AttributeDisplayData>();
            
        }

        public void add(AttributeDisplayData ad)
        {
            m_data.Add(ad.xmlName, ad);
            m_displaySequence.Insert(m_displaySequence.Count,ad.xmlName);
        }

        public AttributeDisplayData GetAttributesForProperty(String key)
        {
            return m_data[key];
        }

        public int Count
        {
            get
            {
                return m_displaySequence.Count;
            }
        }

        public AttributeDisplayData this [int ind]
        {
            get
            {
                try
                {
                    return m_data[m_displaySequence[ind]];
                }
                catch (KeyNotFoundException)
                {
                    return m_data["hp"];
                }
            }
        }
        
        public Boolean Contains(String key)
        {
            return m_data.ContainsKey(key);
        }
    }

    /// <summary>
    /// Controls how ship and item attributes are displayed in the browser.
    /// isHeader indicates if this is a subsection heading,  if so, displayName will be shown as a heading
    /// xmlName is the name of the property in the XML file
    /// displayName is the property name displayed on screen
    /// hideIfZero - if true and the property value is 0 (i.e. string starts with 0) then do not show this property (e.g. we ony need to see the sensor strength for the racial sensor type and not the other 3)
    /// alwaysShow -  if true and the property is absent from the XML, then display a value of 0 (e.g. so we always show a lot configuration to indicate for example, that freighters have no slots)
    /// </summary>
    /// 
    public struct AttributeDisplayData
    {
        public Boolean isHeader;
        public String xmlName;
        public String displayName;
        public Boolean hideIfZero;  // e.g. so we don't display sensor strength for all sensor types
        public Boolean alwaysShow;  // e.g. always show slot configuration

        public AttributeDisplayData(Boolean isHeader, String xmlName, String displayName, Boolean hideIfZero, Boolean alwaysShow)
        {
            this.isHeader = isHeader;
            this.xmlName = xmlName;
            this.displayName = displayName;
            this.hideIfZero = hideIfZero;
            this.alwaysShow = alwaysShow;
        }
    }


}
