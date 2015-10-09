namespace EVEMon.SDEToSQL.Importers.SQLiteToSQL.Models
{
    using System;
    using System.Data.Common;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class UniverseData : DbContext
    {
        public UniverseData()
            : base("name=UniverseData")
        {
        }

        public UniverseData(DbConnection connection)
            : base(connection, true)
        {
        }

        public virtual DbSet<mapCelestialStatistics> mapCelestialStatistics { get; set; }
        public virtual DbSet<mapConstellationJumps> mapConstellationJumps { get; set; }
        public virtual DbSet<mapConstellations> mapConstellations { get; set; }
        public virtual DbSet<mapDenormalize> mapDenormalize { get; set; }
        public virtual DbSet<mapJumps> mapJumps { get; set; }
        public virtual DbSet<mapLandmarks> mapLandmarks { get; set; }
        public virtual DbSet<mapLocationScenes> mapLocationScenes { get; set; }
        public virtual DbSet<mapLocationWormholeClasses> mapLocationWormholeClasses { get; set; }
        public virtual DbSet<mapRegionJumps> mapRegionJumps { get; set; }
        public virtual DbSet<mapRegions> mapRegions { get; set; }
        public virtual DbSet<mapSolarSystemJumps> mapSolarSystemJumps { get; set; }
        public virtual DbSet<mapSolarSystems> mapSolarSystems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<mapCelestialStatistics>()
                .Property(e => e.spectralClass)
                .IsUnicode(false);

            modelBuilder.Entity<mapConstellations>()
                .Property(e => e.constellationName)
                .IsUnicode(false);

            modelBuilder.Entity<mapDenormalize>()
                .Property(e => e.itemName)
                .IsUnicode(false);

            modelBuilder.Entity<mapLandmarks>()
                .Property(e => e.landmarkName)
                .IsUnicode(false);

            modelBuilder.Entity<mapLandmarks>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<mapRegions>()
                .Property(e => e.regionName)
                .IsUnicode(false);

            modelBuilder.Entity<mapSolarSystems>()
                .Property(e => e.solarSystemName)
                .IsUnicode(false);

            modelBuilder.Entity<mapSolarSystems>()
                .Property(e => e.securityClass)
                .IsUnicode(false);
        }
    }
}
