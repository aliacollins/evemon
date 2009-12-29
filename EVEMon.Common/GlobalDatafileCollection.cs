using System;
using System.Collections.Generic;
using System.Text;
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
        /// Default constructor. This class is only instantiated by EveClient
        /// </summary>
        internal GlobalDatafileCollection()
        {
        }

        /// <summary>
        /// Update the datafiles checksums
        /// </summary>
        public void Refresh()
        {
            try
            {
                m_items.Clear();
                m_items.Add(new Datafile(DatafileConstants.PropertiesDatafile));
                m_items.Add(new Datafile(DatafileConstants.ItemsDatafile));
                m_items.Add(new Datafile(DatafileConstants.SkillsDatafile));
                m_items.Add(new Datafile(DatafileConstants.CertificatesDatafile));
                m_items.Add(new Datafile(DatafileConstants.GeographyDatafile));
                m_items.Add(new Datafile(DatafileConstants.ReprocessingDatafile));
            }
            // Don't worry if we cant create MD5 maybe they have FIPS enforced.
            catch (Exception)
            {
                System.Diagnostics.Trace.Write("Couldn't compute datafiles checksums. FIPS was enforced, the datafiles were missing, or we couldn't copy to %APPDATA%.");
            }
        }
    }
}
