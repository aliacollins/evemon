using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a certificate category from our datafiles
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    public sealed class SerializableCertificateCategory
    {
        private readonly Collection<SerializableCertificateClass> m_classes;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableCertificateCategory"/> class.
        /// </summary>
        public SerializableCertificateCategory()
        {
            m_classes = new Collection<SerializableCertificateClass>();
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [XmlAttribute("id")]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlAttribute("descr")]
        public string Description { get; set; }

        /// <summary>
        /// Gets the classes.
        /// </summary>
        /// <value>The classes.</value>
        [XmlElement("certificateClass")]
        public Collection<SerializableCertificateClass> Classes
        {
            get { return m_classes; }
        }

        /// <summary>
        /// Adds the specified classes.
        /// </summary>
        /// <param name="certificateClasses">The certificate classes.</param>
        public void AddRange(IEnumerable<SerializableCertificateClass> certificateClasses)
        {
            m_classes.Clear();
            certificateClasses.ToList().ForEach(certificateClass => m_classes.Add(certificateClass));
        }
    }
}