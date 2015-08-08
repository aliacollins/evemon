namespace EVEMon.XmlGenerator.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class crtRecommendations
    {
        [Key]
        public int recommendationID { get; set; }

        public int? shipTypeID { get; set; }

        public int? certificateID { get; set; }

        public byte recommendationLevel { get; set; }
    }
}
