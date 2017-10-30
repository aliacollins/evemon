namespace EVEMon.XmlGenerator.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class invMarketGroups
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long marketGroupID { get; set; }

        public long? parentGroupID { get; set; }

        [StringLength(100)]
        public string marketGroupName { get; set; }

        [StringLength(3000)]
        public string description { get; set; }

        public int? iconID { get; set; }

        public bool? hasTypes { get; set; }
    }
}
