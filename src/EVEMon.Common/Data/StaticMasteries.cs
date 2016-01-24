using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// The static list of the masteries.
    /// </summary>
    public static class StaticMasteries
    {
        private static readonly Dictionary<int, MasteryShip> s_masteryShipsByID = new Dictionary<int, MasteryShip>();


        #region Initialization

        /// <summary>
        /// Initialize static masteries.
        /// </summary>
        internal static Task LoadAsync()
            => Task.Run(() =>
            {
                if (!File.Exists(Datafile.GetFullPath(DatafileConstants.MasteriesDatafile)))
                    return;

                MasteriesDatafile datafile = Util.DeserializeDatafile<MasteriesDatafile>(DatafileConstants.MasteriesDatafile);

                foreach (SerializableMasteryShip srcShip in datafile.MasteryShips)
                {
                    Ship ship = StaticItems.GetItemByID(srcShip.ID) as Ship;
                    if (ship == null)
                        continue;

                    s_masteryShipsByID[ship.ID] = new MasteryShip(srcShip, ship);
                }
            });

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets an enumeration of all the mastery ships.
        /// </summary>
        public static IEnumerable<MasteryShip> AllMasteryShips
        {
            get { return s_masteryShipsByID.Values; }
        }

        #endregion

    }
}
