namespace EVEMon.SDEToSQL.Importers.SQLiteToSQL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class mapLocationScenes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long locationID { get; set; }

        public long? graphicID { get; set; }
    }
}
