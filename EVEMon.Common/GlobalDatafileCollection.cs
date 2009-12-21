using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.Common.Collections;

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
                m_items.Add(new Datafile("eve-properties.xml.gz"));
                m_items.Add(new Datafile("eve-items.xml.gz"));
                m_items.Add(new Datafile("eve-skills.xml.gz"));
                m_items.Add(new Datafile("eve-certificates.xml.gz"));
                m_items.Add(new Datafile("eve-geography.xml.gz"));
                m_items.Add(new Datafile("eve-reprocessing.xml.gz"));
            }
            // Don't worry if we cant create MD5  maybe they have FIPS enforced.
            catch (Exception)
            {
                System.Diagnostics.Trace.Write("Couldn't compute datafiles checksums. FIPS was enforced, the datafiles were missing, or we couldn't copy to %APPDATA%.");
            }
        }
    }
}
