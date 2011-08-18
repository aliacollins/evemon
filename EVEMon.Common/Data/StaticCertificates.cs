using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{

    /// <summary>
    /// The static list of the certificates.
    /// </summary>
    public static class StaticCertificates
    {
        private static readonly Dictionary<string, StaticCertificateClass> s_classesByName = new Dictionary<string, StaticCertificateClass>();
        private static readonly Dictionary<long, StaticCertificate> s_certificatesByID = new Dictionary<long, StaticCertificate>();


        #region Initialization

        /// <summary>
        /// Initialize static certificates.
        /// </summary>
        internal static void Load()
        {
            CertificatesDatafile datafile = Util.DeserializeDatafile<CertificatesDatafile>(DatafileConstants.CertificatesDatafile);
            Categories = new List<StaticCertificateCategory>();

            foreach (SerializableCertificateCategory srcCat in datafile.Categories)
            {
                Categories.Add(new StaticCertificateCategory(srcCat));
            }

            // Sort categories by name
            Categories.Sort((c1, c2) => String.CompareOrdinal(c1.Name, c2.Name));

            // Build inner collections
            foreach (StaticCertificateClass certClass in Categories.SelectMany(certCategory => certCategory))
            {
                s_classesByName[certClass.Name] = certClass;
                foreach (StaticCertificate cert in certClass)
                {
                    s_certificatesByID[cert.ID] = cert;
                }
            }

            // Completes intialization
            foreach (SerializableCertificateCategory srcCat in datafile.Categories)
            {
                foreach (SerializableCertificateClass srcClass in srcCat.Classes)
                {
                    StaticCertificateClass certClass = s_classesByName[srcClass.Name];
                    foreach (SerializableCertificate srcCert in srcClass.Certificates)
                    {
                        certClass[srcCert.Grade].CompleteInitialization(srcCert.Prerequisites);
                    }
                }
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the categories, sorted by name.
        /// </summary>
        public static List<StaticCertificateCategory> Categories { get; private set; }

        /// <summary>
        /// Gets the certificate classes, hierarchically sorted (category's name, class's name).
        /// </summary>
        public static IEnumerable<StaticCertificateClass> AllClasses
        {
            get { return Categories.SelectMany(category => category); }
        }

        /// <summary>
        /// Gets the certificates, hierarchically sorted (category's name, class's name, grade).
        /// </summary>
        public static IEnumerable<StaticCertificate> AllCertificates
        {
            get
            {
                return from category in Categories from certClass in category from cert in certClass select cert;
            }
        }

        #endregion


        #region Public Finders

        /// <summary>
        /// Gets the certificate with the specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static StaticCertificate GetCertificate(int id)
        {
            return s_certificatesByID[id];
        }

        /// <summary>
        /// Gets the certificates class with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static StaticCertificateClass GetCertificateClass(string name)
        {
            return s_classesByName[name];
        }

        #endregion
    }
}