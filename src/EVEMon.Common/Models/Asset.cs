using System;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Service;

namespace EVEMon.Common.Models
{
    public sealed class Asset
    {
        private readonly EveProperty m_volumeProperty = StaticProperties.GetPropertyByID(DBConstants.VolumePropertyID);
        private long m_locationID;
        private string m_flag;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Asset" /> class.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <exception cref="System.ArgumentNullException">src</exception>
        internal Asset(SerializableAssetListItem src)
        {
            src.ThrowIfNull(nameof(src));

            LocationID = src.LocationID;
            Quantity = src.Quantity;
            Item = StaticItems.GetItemByID(src.TypeID);
            FlagID = src.EVEFlag;
            m_flag = EveFlag.GetFlagText(src.EVEFlag);
            TypeOfBlueprint = GetTypeOfBlueprint(src.RawQuantity);
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
                if (m_locationID == value)
                    return;

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
        public Item Item { get; }

        /// <summary>
        /// Gets the container.
        /// </summary>
        public string Container { get; internal set; }

        /// <summary>
        /// Gets the quantity.
        /// </summary>
        public long Quantity { get; }

        /// <summary>
        /// Gets the eve flag identifier.
        /// </summary>
        private short FlagID { get; }

        /// <summary>
        /// Gets the flag.
        /// </summary>
        public string Flag
        {
            get
            {
                if (m_flag.IsEmptyOrUnknown())
                    m_flag = EveFlag.GetFlagText(FlagID);

                return m_flag;
            }
        }

        /// <summary>
        /// Gets the type of the blueprint.
        /// </summary>
        public string TypeOfBlueprint { get; }

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
            => Jumps == -1
                ? String.Empty
                : $"{Jumps} jump{(Jumps != 1 ? "s" : String.Empty)}";

        /// <summary>
        /// Gets the volume.
        /// </summary>
        public double Volume { get; }

        /// <summary>
        /// Gets the total volume.
        /// </summary>
        public double TotalVolume { get; }

        /// <summary>
        /// Gets the price.
        /// </summary>
        public double Price
            => TypeOfBlueprint != BlueprintType.Copy.ToString()
                ? Settings.MarketPricer.Pricer != null
                    ? Settings.MarketPricer.Pricer.GetPriceByTypeID(Item.ID)
                    : 0
                : 0;

        /// <summary>
        /// Gets the cost.
        /// </summary>
        public double Cost => Price * Quantity;

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the type of the blueprint.
        /// </summary>
        /// <param name="rawQuantity">The raw quantity.</param>
        /// <returns></returns>
        private string GetTypeOfBlueprint(int rawQuantity)
            => Item != null && StaticBlueprints.GetBlueprintByID(Item.ID) != null &&
               !Item.MarketGroup.BelongsIn(DBConstants.AncientRelicsMarketGroupID)
                ? rawQuantity == -2 ? BlueprintType.Copy.ToString() : BlueprintType.Original.ToString()
                : String.Empty;

        /// <summary>
        /// Gets the volume.
        /// </summary>
        /// <returns></returns>
        private double GetVolume() => Item != null && m_volumeProperty != null
            ? m_volumeProperty.GetNumericValue(Item)
            : 0d;

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <returns></returns>
        private string GetLocation()
        {
            if (m_locationID == 0)
                return String.Empty;

            string location = m_locationID.ToString(CultureConstants.InvariantCulture);

            if (m_locationID > Int32.MaxValue)
                return location;

            int locationID = Convert.ToInt32(LocationID);
            Station station = EveIDToStation.GetIDToStation(locationID);

            SolarSystem = station == null
                ? StaticGeography.GetSolarSystemByID(locationID)
                : station.SolarSystem;

            FullLocation = station == null
                ? SolarSystem == null
                    ? location
                    : SolarSystem.FullLocation
                : station.FullLocation;

            return station == null
                ? SolarSystem == null
                    ? location
                    : SolarSystem.Name
                : station.Name;
        }

        #endregion
    }
}
