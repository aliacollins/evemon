namespace EVEMon.XmlGenerator.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class invCategories
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int categoryID { get; set; }

        [StringLength(100)]
        public string categoryName { get; set; }

        public int? iconID { get; set; }

        public bool? published { get; set; }
    }
}
