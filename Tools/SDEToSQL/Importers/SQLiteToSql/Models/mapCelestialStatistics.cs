namespace EVEMon.SDEToSQL.Importers.SQLiteToSQL.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class mapCelestialStatistics
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long celestialID { get; set; }

        [Column(TypeName = "real")]
        public double? temperature { get; set; }

        [StringLength(10)]
        public string spectralClass { get; set; }

        [Column(TypeName = "real")]
        public double? luminosity { get; set; }

        [Column(TypeName = "real")]
        public double? age { get; set; }

        [Column(TypeName = "real")]
        public double? life { get; set; }

        [Column(TypeName = "real")]
        public double? orbitRadius { get; set; }

        [Column(TypeName = "real")]
        public double? eccentricity { get; set; }

        [Column(TypeName = "real")]
        public double? massDust { get; set; }

        [Column(TypeName = "real")]
        public double? massGas { get; set; }

        public bool? fragmented { get; set; }

        [Column(TypeName = "real")]
        public double? density { get; set; }

        [Column(TypeName = "real")]
        public double? surfaceGravity { get; set; }

        [Column(TypeName = "real")]
        public double? escapeVelocity { get; set; }

        [Column(TypeName = "real")]
        public double? orbitPeriod { get; set; }

        [Column(TypeName = "real")]
        public double? rotationRate { get; set; }

        public bool? locked { get; set; }

        public double? pressure { get; set; }

        public double? radius { get; set; }

        public double? mass { get; set; }
    }
}
