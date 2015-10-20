using EVEMon.Common.Serialization.Datafiles;
using System.Linq;

namespace EVEMon.Common.Data
{
    public class StaticMasteryCertificate
    {
        private SerializableMasteryCertificate sourceCertificate;
        private StaticMasteryCertificates staticMasteryCertificates;

        public StaticMasteryCertificate(StaticMasteryCertificates staticMasteryCertificates, SerializableMasteryCertificate sourceCertificate)
        {
            MasteryCertificates = staticMasteryCertificates;
            ID = sourceCertificate.ID;
            ClassName = sourceCertificate.ClassName;            
        }

        public string ClassName { get; private set; }
        public int ID { get; private set; }
        public StaticMasteryCertificates MasteryCertificates { get; private set; }

        public StaticCertificateClass CertificateClass { get; private set; }

        public void CompleteInitialization()
        {
            CertificateClass = StaticCertificates.AllClasses.FirstOrDefault(certClass => certClass.ID == ID);            
        }
    }
}