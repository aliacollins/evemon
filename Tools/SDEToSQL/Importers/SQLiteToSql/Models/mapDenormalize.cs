namespace EVEMon.SDEToSQL.Importers.SQLiteToSQL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mapDenormalize")]
    public partial class mapDenormalize
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long itemID { get; set; }

        public long? typeID { get; set; }

        public long? groupID { get; set; }

        public long? solarSystemID { get; set; }

        public long? constellationID { get; set; }

        public long? regionID { get; set; }

        public long? orbitID { get; set; }

        [Column(TypeName = "real")]
        public double? x { get; set; }

        [Column(TypeName = "real")]
        public double? y { get; set; }

        [Column(TypeName = "real")]
        public double? z { get; set; }

        [Column(TypeName = "real")]
        public double? radius { get; set; }

        [StringLength(100)]
        public string itemName { get; set; }

        [Column(TypeName = "real")]
        public double? security { get; set; }

        public long? celestialIndex { get; set; }

        public long? orbitIndex { get; set; }
    }
}
