using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Root SkillsDatafile Serialization Class
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    [XmlRoot("skills")]
    public sealed class SkillsDatafile
    {
        private Collection<SerializableSkillGroup> m_groups;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillsDatafile"/> class.
        /// </summary>
        public SkillsDatafile()
        {
            m_groups = new Collection<SerializableSkillGroup>();
        }

        /// <summary>
        /// Gets the groups.
        /// </summary>
        /// <value>The groups.</value>
        [XmlElement("group")]
        public Collection<SerializableSkillGroup> Groups
        {
            get { return m_groups; }
        }

        /// <summary>
        /// Adds the specified groups.
        /// </summary>
        /// <param name="groups">The groups.</param>
        public void Add(List<SerializableSkillGroup> groups)
        {
            m_groups = new Collection<SerializableSkillGroup>(groups);
        }
    }
}