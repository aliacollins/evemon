namespace EVEMon.SDEToSQL.SQLiteToSQL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class mapSolarSystemJumps
    {
        public long? fromRegionID { get; set; }

        public long? fromConstellationID { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long fromSolarSystemID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long toSolarSystemID { get; set; }

        public long? toConstellationID { get; set; }

        public long? toRegionID { get; set; }
    }
}
