namespace EVEMon.XmlGenerator.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ramInstallationTypeContents
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int installationTypeID { get; set; }

        [Key]
        [Column(Order = 1)]
        public byte assemblyLineTypeID { get; set; }

        public byte? quantity { get; set; }
    }
}
