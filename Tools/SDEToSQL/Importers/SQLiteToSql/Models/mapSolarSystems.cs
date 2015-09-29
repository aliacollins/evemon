namespace EVEMon.SDEToSQL.Importers.SQLiteToSQL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class mapSolarSystems
    {
        public long? regionID { get; set; }

        public long? constellationID { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long solarSystemID { get; set; }

        [StringLength(100)]
        public string solarSystemName { get; set; }

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

        [Column(TypeName = "real")]
        public double? luminosity { get; set; }

        public bool? border { get; set; }

        public bool? fringe { get; set; }

        public bool? corridor { get; set; }

        public bool? hub { get; set; }

        public bool? international { get; set; }

        public bool? regional { get; set; }

        public bool? constellation { get; set; }

        [Column(TypeName = "real")]
        public double? security { get; set; }

        public long? factionID { get; set; }

        [Column(TypeName = "real")]
        public double? radius { get; set; }

        public long? sunTypeID { get; set; }

        [StringLength(2)]
        public string securityClass { get; set; }
    }
}
