namespace EVEMon.SDEExternalsToSql.SQLiteToSql.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class mapRegions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long regionID { get; set; }

        [StringLength(100)]
        public string regionName { get; set; }

        [Column(TypeName = "real")]
        public double? x { get; set; }

        [Column(TypeName = "real")]
        public double? y { get; set; }

        [Column(TypeName = "real")]
        public double? z { get; set; }

        [Column(TypeName = "real")]
        public double? xMin { get; set; }

        [Column(TypeName = "real")]
        public double? xMax { get; set; }

        [Column(TypeName = "real")]
        public double? yMin { get; set; }

        [Column(TypeName = "real")]
        public double? yMax { get; set; }

        [Column(TypeName = "real")]
        public double? zMin { get; set; }

        [Column(TypeName = "real")]
        public double? zMax { get; set; }

        public long? factionID { get; set; }

        [Column(TypeName = "real")]
        public double? radius { get; set; }
    }
}
