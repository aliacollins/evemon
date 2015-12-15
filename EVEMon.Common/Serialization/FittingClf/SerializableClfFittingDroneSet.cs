using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.FittingClf
{
    [DataContract]
    public sealed class SerializableClfFittingDroneSet
    {
        private Collection<SerializableClfFittingDroneType> m_inBayDrones;
        private Collection<SerializableClfFittingDroneType> m_inSpaceDrones;

        [DataMember(Name = "presetname")]
        public string Name { get; set; }

        [DataMember(Name = "inbay")]
        public Collection<SerializableClfFittingDroneType> InBay
        {
            get { return m_inBayDrones ?? (m_inBayDrones = new Collection<SerializableClfFittingDroneType>()); }
        }

        [DataMember(Name = "inspace")]
        public Collection<SerializableClfFittingDroneType> InSpace
        {
            get { return m_inSpaceDrones ?? (m_inSpaceDrones = new Collection<SerializableClfFittingDroneType>()); }
        }
    }
}