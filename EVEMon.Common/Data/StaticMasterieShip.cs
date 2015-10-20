using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class StaticMasterieShip : ReadonlyCollection<StaticMasteryCertificates>
    {
        internal StaticMasterieShip(SerializableMasteryShip src) : base(src.Masteries.Count)
        {
            ID = src.ID;
            Name = src.Name;
            foreach (var masteryCertificate in src.Masteries)
            {
                Items.Add(new StaticMasteryCertificates(this, masteryCertificate));
            }
        }

        public int ID { get; private set; }
        public string Name { get; private set; }

        #region Overridden Methods
        public override string ToString()
        {
            return Name;
        }
        #endregion
    }
}