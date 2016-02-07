using System;
using EVEMon.Common.Data;
using EVEMon.Common.Helpers;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Collections.Global
{
    /// <summary>
    /// Implements a collection of datafiles
    /// </summary>
    public sealed class GlobalDatafileCollection : ReadonlyCollection<Datafile>
    {
        /// <summary>
        /// Default constructor. This class is only instantiated by EveMonClient.
        /// </summary>
        internal GlobalDatafileCollection()
        {
            try
            {
                Items.Clear();
                Items.Add(new Datafile(DatafileConstants.PropertiesDatafile));
                Items.Add(new Datafile(DatafileConstants.ItemsDatafile));
                Items.Add(new Datafile(DatafileConstants.SkillsDatafile));
                Items.Add(new Datafile(DatafileConstants.CertificatesDatafile));
                Items.Add(new Datafile(DatafileConstants.MasteriesDatafile));
                Items.Add(new Datafile(DatafileConstants.BlueprintsDatafile));
                Items.Add(new Datafile(DatafileConstants.GeographyDatafile));
                Items.Add(new Datafile(DatafileConstants.ReprocessingDatafile));
            }
            catch (Exception ex)
            {
                // Don't worry if we can't create MD5 maybe they have FIPS enforced
                EveMonClient.Trace(
                    "Couldn't compute datafiles checksums. FIPS was enforced, the datafiles were missing, or we couldn't copy to %APPDATA%.");
                ExceptionHandler.LogRethrowException(ex);
                throw;
            }
        }

        /// <summary>
        /// Loads the static data.
        /// </summary>
        internal static void Load()
        {
            // This is the time optimal loading order
            // (min order to follow : 
            // skills before anything else,
            // items before blueprints, reprocessing and certificates,
            // certs before masteries)

            EveMonClient.Trace("Datafiles.Load - begin", printMethod: false);

            TaskHelper.RunIOBoundTaskAsync(() => StaticSkills.Load());
            TaskHelper.RunIOBoundTaskAsync(() => StaticProperties.Load());
            TaskHelper.RunIOBoundTaskAsync(() => StaticGeography.Load());
            // Must always run synchronously as blueprints, reprocessing and certificates depend on it
            StaticItems.Load();
            TaskHelper.RunIOBoundTaskAsync(() => StaticReprocessing.Load());
            TaskHelper.RunIOBoundTaskAsync(() => StaticBlueprints.Load());
            // Must always run synchronously as masteries depend on it
            StaticCertificates.Load();
            TaskHelper.RunIOBoundTaskAsync(() => StaticMasteries.Load());

            EveMonClient.Trace("Datafiles.Load - done", printMethod: false);
        }
    }
}