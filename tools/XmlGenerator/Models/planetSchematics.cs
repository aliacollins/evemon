namespace EVEMon.XmlGenerator.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class planetSchematics
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short schematicID { get; set; }

        [StringLength(255)]
        public string schematicName { get; set; }

        public int? cycleTime { get; set; }
    }
}
