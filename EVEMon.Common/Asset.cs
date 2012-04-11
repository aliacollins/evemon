using System;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class Asset
    {
        private readonly short m_rawQuantity;
        private string m_location;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Asset"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public Asset(SerializableAssetListItem src)
        {
            ID = src.ItemID;
            LocationID = src.LocationID;
            Item = StaticItems.GetItemByID(src.TypeID);
            Quantity = src.Quantity;
            Flag = EveFlags.GetFlagText(src.Flag);
            m_rawQuantity = src.RawQuantity;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the ID.
        /// </summary>
        private long ID { get; set; }

        /// <summary>
        /// Gets the location ID.
        /// </summary>
        internal long LocationID { get; set; }

        /// <summary>
        /// Gets the location.
        /// </summary>
        public string Location
        {
            get
            {
                return String.IsNullOrEmpty(m_location)
                           ? m_location = GetLocation()
                           : m_location;
            }
        }

        /// <summary>
        /// Gets the full celestrial path of the item's location.
        /// </summary>
        public string FullLocation { get; private set; }

        /// <summary>
        /// Gets the solar system.
        /// </summary>
        public SolarSystem SolarSystem { get; private set; }

        /// <summary>
        /// Gets the item.
        /// </summary>
        public Item Item { get; private set; }

        /// <summary>
        /// Gets the type of the blueprint.
        /// </summary>
        public string BlueprintType
        {
            get
            {
                return Item.CategoryName == ItemFamily.Blueprint.ToString()
                           ? m_rawQuantity == -2 ? "Copy" : "Original"
                           : String.Empty;
            }
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        public string Container { get; internal set; }

        /// <summary>
        /// Gets the quantity.
        /// </summary>
        public long Quantity { get; private set; }

        /// <summary>
        /// Gets the volume.
        /// </summary>
        public double Volume
        {
            get
            {
                if (Item != null)
                {
                    EveProperty prop = StaticProperties.GetPropertyByID(DBConstants.VolumePropertyID);
                    if (prop != null)
                        return prop.GetNumericValue(Item);
                }

                return 0d;
            }
        }

        /// <summary>
        /// Gets the flag.
        /// </summary>
        public string Flag { get; private set; }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <returns></returns>
        private string GetLocation()
        {
            if (LocationID == 0)
                return String.Empty;

            if (LocationID <= Int32.MaxValue)
            {
                int locationID = Convert.ToInt32(LocationID);
                Station station = ConquerableStation.GetStationByID(locationID) ?? StaticGeography.GetStationByID(locationID);

                if (station == null)
                {
                    SolarSystem = StaticGeography.GetSolarSystemByID(locationID);
                    FullLocation = SolarSystem == null ? LocationID.ToString() : SolarSystem.FullLocation;
                    return SolarSystem == null ? LocationID.ToString() : SolarSystem.Name;
                }

                SolarSystem = station.SolarSystem;
                FullLocation = station.FullLocation;
                return station.Name;
            }

            return LocationID.ToString();
        }

        #endregion

    }
}