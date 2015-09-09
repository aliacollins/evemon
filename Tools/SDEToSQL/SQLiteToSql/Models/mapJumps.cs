namespace EVEMon.SDEToSQL.SQLiteToSQL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class mapJumps
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long stargateID { get; set; }

        public long? destinationID { get; set; }
    }
}
