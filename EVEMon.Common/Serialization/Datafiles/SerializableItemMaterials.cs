using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    public sealed class SerializableItemMaterials
    {
        private Collection<SerializableMaterialQuantity> m_materials;

        public SerializableItemMaterials()
        {
            m_materials = new Collection<SerializableMaterialQuantity>();
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [XmlAttribute("id")]
        public int ID { get; set; }

        /// <summary>
        /// Gets the materials.
        /// </summary>
        /// <value>The materials.</value>
        [XmlElement("material")]
        public Collection<SerializableMaterialQuantity> Materials
        {
            get { return m_materials; }
        }

        /// <summary>
        /// Adds the specified materials.
        /// </summary>
        /// <param name="materials">The materials.</param>
        public void Add(List<SerializableMaterialQuantity> materials)
        {
            m_materials = new Collection<SerializableMaterialQuantity>(materials);
        }
    }
}