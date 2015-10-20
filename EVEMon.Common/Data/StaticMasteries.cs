using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// The static list of masteries
    /// </summary>
    public class StaticMasteries
    {
        #region Initialization

        internal static void Load()
        {
            if (!File.Exists(Datafile.GetFullPath(DatafileConstants.MasteriesDatafile)))
                return;

            MasteriesDatafile datafile = Util.DeserializeDatafile<MasteriesDatafile>(DatafileConstants.MasteriesDatafile);

            foreach (var mastery in datafile.MasteryShips)
            {
                Masteries.Add(new StaticMasterieShip(mastery));
            }

            // Set the certificate class for every 
            foreach (var masteryCertificate in Masteries.SelectMany(m => m.SelectMany(s => s.Select(c => c))))
            {
                masteryCertificate.CompleteInitialization();
            }
        }

        #endregion

        #region Properties
        public static Collection<StaticMasterieShip> Masteries { get; private set; }
        #endregion

    }
}
