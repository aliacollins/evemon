namespace EVEMon.XmlGenerator.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class invFlags
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short flagID { get; set; }

        [StringLength(200)]
        public string flagName { get; set; }

        [StringLength(100)]
        public string flagText { get; set; }

        public int? orderID { get; set; }
    }
}
