using System;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.BattleClinic.MarketPrices;

namespace EVEMon.Common
{
    public sealed class Asset
    {
        private long m_locationID;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Asset"/> class.
        /// </summary>
        /// <param name="src">The source.</param>
        internal Asset(SerializableAssetListItem src)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            LocationID = src.LocationID;
            Quantity = src.Quantity;
            Item = StaticItems.GetItemByID(src.TypeID);
            Flag = EveFlag.GetFlagText(src.EVEFlag);
            BlueprintType = GetBlueprintType(src.RawQuantity);
            Container = String.Empty;
            Volume = GetVolume();
            TotalVolume = Quantity * Volume;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the location ID.
        /// </summary>
        internal long LocationID
        {
            get { return m_locationID; }
            set
            {
                m_locationID = value;
                Location = GetLocation();
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
        /// Gets the container.
        /// </summary>
        public string Container { get; internal set; }

        /// <summary>
        /// Gets the quantity.
        /// </summary>
        public long Quantity { get; private set; }

        /// <summary>
        /// Gets the flag.
        /// </summary>
        public string Flag { get; private set; }

        /// <summary>
        /// Gets the type of the blueprint.
        /// </summary>
        public string BlueprintType { get; private set; }

        /// <summary>
        /// Gets the location.
        /// </summary>
        public string Location { get; private set; }

        /// <summary>
        /// Gets the jumps count.
        /// </summary>
        public int Jumps { get; internal set; }

        /// <summary>
        /// Gets the jumps text.
        /// </summary>
        public string JumpsText
        {
            get
            {
                return Jumps == -1
                           ? String.Empty
                           : String.Format(CultureConstants.DefaultCulture,
                                           "{0} jump{1}", Jumps, Jumps != 1 ? "s" : String.Empty);
            }
        }

        /// <summary>
        /// Gets the volume.
        /// </summary>
        public decimal Volume { get; private set; }

        /// <summary>
        /// Gets the total volume.
        /// </summary>
        public decimal TotalVolume { get; private set; }

        /// <summary>
        /// Gets the price.
        /// </summary>
        public double Price
        {
            get { return BCItemPrices.GetPriceByTypeID(Item.ID); }
        }

        /// <summary>
        /// Gets the cost.
        /// </summary>
        public double Cost
        {
            get { return Price * Quantity; }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the type of the blueprint.
        /// </summary>
        /// <param name="rawQuantity">The raw quantity.</param>
        /// <returns></returns>
        private string GetBlueprintType(int rawQuantity)
        {
            return Item != null && Item.Family == ItemFamily.Blueprint
                       ? rawQuantity == -2 ? Common.BlueprintType.Copy.ToString() : Common.BlueprintType.Original.ToString()
                       : String.Empty;
        }

        /// <summary>
        /// Gets the volume.
        /// </summary>
        /// <returns></returns>
        private decimal GetVolume()
        {
            if (Item != null)
            {
                EveProperty prop = StaticProperties.GetPropertyByID(DBConstants.VolumePropertyID);
                if (prop != null)
                    return Convert.ToDecimal(prop.GetNumericValue(Item));
            }

            return 0M;
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <returns></returns>
        private string GetLocation()
        {
            if (m_locationID == 0)
                return String.Empty;

            string location = m_locationID.ToString(CultureConstants.InvariantCulture);

            if (m_locationID <= Int32.MaxValue)
            {
                int locationID = Convert.ToInt32(LocationID);
                Station station = Station.GetByID(locationID);
                ConquerableStation outpost = station as ConquerableStation;

                SolarSystem = station == null
                                  ? StaticGeography.GetSolarSystemByID(locationID)
                                  : station.SolarSystem;

                FullLocation = station == null
                                   ? SolarSystem == null
                                         ? location
                                         : SolarSystem.FullLocation
                                   : outpost != null
                                         ? outpost.FullLocation
                                         : station.FullLocation;

                return station == null
                           ? SolarSystem == null
                                 ? location
                                 : SolarSystem.Name
                           : station.Name;
            }

            return location;
        }

        #endregion
    }
}