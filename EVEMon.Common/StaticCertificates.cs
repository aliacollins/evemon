using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace EVEMon.Common
{

    /// <summary>
    /// The static list of the certificates
    /// </summary>
    public static class StaticCertificates
    {
        private static readonly Dictionary<string, CertificateClass> m_classesByName = new Dictionary<string, CertificateClass>();
        private static readonly Dictionary<int, Certificate> m_certificatesByID = new Dictionary<int, Certificate>();
        private static readonly List<CertificateCategory> m_categories = new List<CertificateCategory>();
        private static bool m_initalized;

        /// <summary>
        /// Gets the categories, sorted by name
        /// </summary>
        public static IEnumerable<CertificateCategory> Categories
        {
            get 
            {
                EnsureInitialized();
                return m_categories; 
            }
        }

        /// <summary>
        /// Gets the certificate classes, hierarchically sorted (category's name, class's name)
        /// </summary>
        public static IEnumerable<CertificateClass> Classes
        {
            get
            {
                EnsureInitialized();
                foreach (var category in m_categories)
                {
                    foreach (var certClass in category.Classes)
                    {
                        yield return certClass;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the certificates, hierarchically sorted (category's name, class's name, grade)
        /// </summary>
        public static IEnumerable<Certificate> Certificates
        {
            get 
            {
                EnsureInitialized();
                foreach (var category in m_categories)
                {
                    foreach (var certClass in category.Classes)
                    {
                        foreach (var cert in certClass.Certificates)
                        {
                            yield return cert;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the certificate with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Certificate GetCertificate(int id)
        {
            EnsureInitialized();
            return m_certificatesByID[id];
        }

        /// <summary>
        /// Gets the certificates class with the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CertificateClass GetCertificateClass(string name)
        {
            EnsureInitialized();
            return m_classesByName[name];
        }

        /// <summary>
        /// Ensure the static datas are initialized
        /// </summary>
        private static void EnsureInitialized()
        {
            if (m_initalized) return;
            lock (m_certificatesByID)
            {
                if (m_initalized) return;
                StaticSkill.LoadStaticSkills();

                // We're ready. First, let's check the file exists
                string file = Settings.FindDatafile("eve-certificates.xml.gz");

                if (!File.Exists(file))
                {
                    throw new ApplicationException(file + " not found!");
                }

                // Open the stream and deserialize the content
                using (FileStream s = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    using (GZipStream zs = new GZipStream(s, CompressionMode.Decompress))
                    {
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.Load(zs);
                        var root = xdoc["certificates"];

                        foreach (XmlElement elem in root.ChildNodes)
                        {
                            m_categories.Add(new CertificateCategory(elem));
                        }
                    }
                }

                // Sort categories by name
                m_categories.Sort((c1, c2) => String.CompareOrdinal(c1.Name, c2.Name));

                // Build inner collections
                foreach(var certCategory in m_categories)
                {
                    foreach(var certClass in certCategory.Classes)
                    {
                        m_classesByName[certClass.Name] = certClass;
                        foreach(var cert in certClass.Certificates)
                        {
                            m_certificatesByID[cert.ID] = cert;
                        }
                    }
                }

                // Resolve prereq's names
                foreach(var certificate in m_certificatesByID.Values)
                {
                    certificate.Complete(m_classesByName);
                }

                // Mark as initialized
                m_initalized = true;
            }
        }
    }
}
