using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class PlanetaryPin
    {
        internal PlanetaryPin(SerializablePlanetaryPin src)
        {
            TypeName = src.TypeName;
            SchematicID = src.SchematicID;
            CycleTime = src.CycleTime;
            QuantityPerCycle = src.QuantityPerCycle;
            ContentTypeName = src.ContentTypeName;
            ContentQuantity = src.ContentQuantity;
            LastLaunchTime = src.LastLaunchTime;
            InstallTime = src.InstallTime;
            ExpiryTime = src.ExpiryTime;
        }        
        
        public string TypeName { get; set; }

        public long SchematicID { get; set; }

        public short CycleTime { get; set; }

        public int QuantityPerCycle { get; set; }

        public string ContentTypeName { get; set; }

        public int ContentQuantity { get; set; }

        public DateTime LastLaunchTime { get; set; }

        public DateTime InstallTime { get; set; }

        public DateTime ExpiryTime { get; set; }

    }
}
