namespace EVEMon.XmlGenerator.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class invGroups
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int groupID { get; set; }

        public int? categoryID { get; set; }

        [StringLength(100)]
        public string groupName { get; set; }

        [StringLength(3000)]
        public string description { get; set; }

        public int? iconID { get; set; }

        public bool? useBasePrice { get; set; }

        public bool? allowManufacture { get; set; }

        public bool? allowRecycler { get; set; }

        public bool? anchored { get; set; }

        public bool? anchorable { get; set; }

        public bool? fittableNonSingleton { get; set; }

        public bool? published { get; set; }
    }
}
