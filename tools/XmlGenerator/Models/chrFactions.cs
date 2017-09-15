namespace EVEMon.XmlGenerator.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class chrFactions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int factionID { get; set; }

        [StringLength(100)]
        public string factionName { get; set; }

        [StringLength(1000)]
        public string description { get; set; }

        public int? raceIDs { get; set; }

        public int? solarSystemID { get; set; }

        public int? corporationID { get; set; }

        public double? sizeFactor { get; set; }

        public short? stationCount { get; set; }

        public short? stationSystemCount { get; set; }

        public int? militiaCorporationID { get; set; }

        public int? iconID { get; set; }
    }
}
