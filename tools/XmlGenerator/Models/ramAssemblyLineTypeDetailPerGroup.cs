namespace EVEMon.XmlGenerator.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ramAssemblyLineTypeDetailPerGroup")]
    public partial class ramAssemblyLineTypeDetailPerGroup
    {
        [Key]
        [Column(Order = 0)]
        public byte assemblyLineTypeID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int groupID { get; set; }

        public double? timeMultiplier { get; set; }

        public double? materialMultiplier { get; set; }

        public double? costMultiplier { get; set; }
    }
}
