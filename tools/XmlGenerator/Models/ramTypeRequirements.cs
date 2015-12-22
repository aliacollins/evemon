namespace EVEMon.XmlGenerator.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ramTypeRequirements
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int typeID { get; set; }

        [Key]
        [Column(Order = 1)]
        public byte activityID { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int requiredTypeID { get; set; }

        public int? quantity { get; set; }

        public int? level { get; set; }

        public double? damagePerJob { get; set; }

        public bool? recycle { get; set; }

        public int? raceID { get; set; }

        public double? probability { get; set; }

        public bool? consume { get; set; }
    }
}
