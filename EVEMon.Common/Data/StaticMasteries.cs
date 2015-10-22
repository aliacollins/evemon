using System.Collections.Generic;
using System.IO;
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
        internal static void Load()
        {
            if (!File.Exists(Datafile.GetFullPath(DatafileConstants.MasteriesDatafile)))
                return;

            MasteriesDatafile datafile = Util.DeserializeDatafile<MasteriesDatafile>(DatafileConstants.MasteriesDatafile);

            foreach (SerializableMasteryShip ship in datafile.MasteryShips)
            {
                MasteryShip masteryShip = new MasteryShip(ship);
                s_masteryShipsByID[ship.ID] = masteryShip;
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets an enumeration of all the mastery ships.
        /// </summary>
        public static IEnumerable<MasteryShip> AllMasteryShips
        {
            get { return s_masteryShipsByID.Values; }
        }

        /// <summary>
        /// Gets the mastery ship with the provided ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static MasteryShip GetMasteryShipByID(int id)
        {
            return s_masteryShipsByID[id];
        }

        #endregion

    }
}
