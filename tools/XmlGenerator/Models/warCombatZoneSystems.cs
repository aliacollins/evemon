namespace EVEMon.XmlGenerator.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class warCombatZoneSystems
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int solarSystemID { get; set; }

        public int? combatZoneID { get; set; }
    }
}
