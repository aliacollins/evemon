namespace EVEMon.XmlGenerator.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class mapLocationWormholeClasses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int locationID { get; set; }

        public byte? wormholeClassID { get; set; }
    }
}
