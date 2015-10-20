using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    public sealed class StaticMasteryCertificates : ReadonlyCollection<StaticMasteryCertificate>
    {
        internal StaticMasteryCertificates(StaticMasterieShip staticMasterieShip, SerializableMastery masteryCertificate)
        {
            MasteriShip = staticMasterieShip;

            Grade = masteryCertificate.Grade;
            foreach (var sourceCertificate in masteryCertificate.Certificates)
            {
                Items.Add(new StaticMasteryCertificate(this, sourceCertificate));
            }            
        }

        public int Grade { get; private set; }
        public StaticMasterieShip MasteriShip { get; private set; }
    }
}