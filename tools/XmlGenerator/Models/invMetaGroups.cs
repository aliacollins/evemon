namespace EVEMon.XmlGenerator.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class invMetaGroups
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short metaGroupID { get; set; }

        [StringLength(100)]
        public string metaGroupName { get; set; }

        [StringLength(1000)]
        public string description { get; set; }

        public int? iconID { get; set; }
    }
}
