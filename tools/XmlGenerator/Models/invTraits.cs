namespace EVEMon.XmlGenerator.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class invTraits
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int traitID { get; set; }

        [Required]
        [StringLength(500)]
        public string BonusText { get; set; }

        public byte? unitID { get; set; }
    }
}
