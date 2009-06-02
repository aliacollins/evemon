using System;
using System.Collections.Generic;
using System.Xml;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a certificate category. Every category
    /// (i.e. "Business and Industry") contains certificate classes
    /// (i.e. "Production Manager"), which contain certificates
    /// (i.e. "Production Manager Basic").
    /// </summary>
    public sealed class CertificateCategory
    {
        private readonly List<CertificateClass> classes = new List<CertificateClass>();

        public readonly int ID;
        public readonly string Name;
        public readonly string Description;

        /// <summary>
        /// Constructor from XML
        /// </summary>
        /// <param name="element"></param>
        internal CertificateCategory(XmlElement element)
        {
            this.Name = element.GetAttribute("name");
            this.Description = element.GetAttribute("descr");
            this.ID = Int32.Parse(element.GetAttribute("id"));

            if (element.HasChildNodes)
            {
                foreach (var child in element.ChildNodes)
                {
                    var certClass = new CertificateClass(this, (XmlElement)child);
                    this.classes.Add(certClass);
                }

                // Sorty by name
                this.classes.Sort((c1, c2) => String.Compare(c1.Name, c2.Name));
            }
        }

        /// <summary>
        /// Gets the certificate classes, sorted by name
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CertificateClass> Classes
        {
            get { return this.classes; }
        }
    } 
}
