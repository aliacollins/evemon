namespace EVEMon.XmlGenerator.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class invUniqueNames
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int itemID { get; set; }

        [Required]
        [StringLength(200)]
        public string itemName { get; set; }

        public int? groupID { get; set; }
    }
}
