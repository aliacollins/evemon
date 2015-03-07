using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// The static list of the certificates.
    /// </summary>
    public static class StaticCertificates
    {
        private static readonly Dictionary<string, StaticCertificateClass> s_classesByName =
            new Dictionary<string, StaticCertificateClass>();

        private static readonly Dictionary<int, StaticCertificate> s_certificatesByID = new Dictionary<int, StaticCertificate>();


        #region Initialization

        /// <summary>
        /// Initialize static certificates.
        /// </summary>
        internal static void Load()
        {
            if (!File.Exists(Datafile.GetFullPath(DatafileConstants.CertificatesDatafile)))
                return;

            CertificatesDatafile datafile = Util.DeserializeDatafile<CertificatesDatafile>(DatafileConstants.CertificatesDatafile);
            Groups = new Collection<StaticCertificateGroup>();

            foreach (SerializableCertificateGroup srcGroup in datafile.Groups)
            {
                Groups.Add(new StaticCertificateGroup(srcGroup));
            }

            // Build inner collections
            foreach (StaticCertificateClass certClass in Groups.SelectMany(certClass => certClass))
            {
                s_classesByName[certClass.Name] = certClass;
                s_certificatesByID[certClass.Certificate.ID] = certClass.Certificate;
            }

            // Completes intialization
            foreach (SerializableCertificateClass srcClass in datafile.Groups.SelectMany(srcGroup => srcGroup.Classes))
            {
                s_classesByName[srcClass.Name].Certificate.CompleteInitialization(srcClass.Certificate.Prerequisites);
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the categories, sorted by name.
        /// </summary>
        public static Collection<StaticCertificateGroup> Groups { get; private set; }

        /// <summary>
        /// Gets the certificate classes, hierarchically sorted (category's name, class's name).
        /// </summary>
        public static IEnumerable<StaticCertificateClass> AllClasses
        {
            get { return Groups.SelectMany(certClass => certClass); }
        }

        /// <summary>
        /// Gets the certificates, hierarchically sorted (category's name, class's name, grade).
        /// </summary>
        public static IEnumerable<StaticCertificate> AllCertificates
        {
            get
            {
                return Groups.SelectMany(group => group, (group, certClass) => new { group, certClass }).
                    Select(x => x.certClass.Certificate);
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