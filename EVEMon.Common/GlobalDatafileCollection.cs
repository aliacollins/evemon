using System;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common
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
        }

        /// <summary>
        /// Update the datafiles list.
        /// </summary>
        public void Refresh()
        {
            try
            {
                Items.Clear();
                Items.Add(new Datafile(DatafileConstants.PropertiesDatafile));
                Items.Add(new Datafile(DatafileConstants.ItemsDatafile));
                Items.Add(new Datafile(DatafileConstants.SkillsDatafile));
                Items.Add(new Datafile(DatafileConstants.CertificatesDatafile));
                Items.Add(new Datafile(DatafileConstants.GeographyDatafile));
                Items.Add(new Datafile(DatafileConstants.ReprocessingDatafile));
                Items.Add(new Datafile(DatafileConstants.BlueprintsDatafile));
            }
            // Don't worry if we can't create MD5 maybe they have FIPS enforced
            catch (Exception)
            {
                EveMonClient.Trace("Couldn't compute datafiles checksums. FIPS was enforced, the datafiles were missing, or we couldn't copy to %APPDATA%.");
            }
        }
    }
}