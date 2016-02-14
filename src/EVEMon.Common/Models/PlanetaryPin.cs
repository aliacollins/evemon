using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.Models
{
    public sealed class PlanetaryPin
    {
        private readonly EveProperty m_volumeProperty = StaticProperties.GetPropertyByID(DBConstants.VolumePropertyID);
        private readonly char[] m_baseString = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanetaryPin"/> class.
        /// </summary>
        /// <param name="colony">The colony.</param>
        /// <param name="src">The source.</param>
        internal PlanetaryPin(PlanetaryColony colony, SerializablePlanetaryPin src)
        {
            Colony = colony;
            ID = src.PinID;
            TypeID = src.TypeID;
            TypeName = GetPinName(src.TypeName);
            SchematicID = src.SchematicID;
            CycleTime = src.CycleTime;
            QuantityPerCycle = src.QuantityPerCycle;
            ContentQuantity = src.ContentQuantity;
            ContentTypeID = src.ContentTypeID;
            ContentTypeName = src.ContentTypeName;
            LastLaunchTime = src.LastLaunchTime;
            InstallTime = src.InstallTime;
            ExpiryTime = src.ExpiryTime;
            State = GetState();
            ContentVolume = GetVolume();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the colony.
        /// </summary>
        /// <value>
        /// The colony.
        /// </value>
        public PlanetaryColony Colony { get; }

        /// <summary>
        /// Gets the pin identifier.
        /// </summary>
        /// <value>
        /// The pin identifier.
        /// </value>
        public long ID { get; }

        /// <summary>
        /// Gets the type identifier.
        /// </summary>
        /// <value>
        /// The type identifier.
        /// </value>
        public int TypeID { get; }

        /// <summary>
        /// Gets or sets the name of the type.
        /// </summary>
        /// <value>
        /// The name of the type.
        /// </value>
        public string TypeName { get; }

        /// <summary>
        /// Gets or sets the schematic identifier.
        /// </summary>
        /// <value>
        /// The schematic identifier.
        /// </value>
        public long SchematicID { get; }

        /// <summary>
        /// Gets or sets the cycle time.
        /// </summary>
        /// <value>
        /// The cycle time.
        /// </value>
        public short CycleTime { get; }

        /// <summary>
        /// Gets or sets the quantity per cycle.
        /// </summary>
        /// <value>
        /// The quantity per cycle.
        /// </value>
        public int QuantityPerCycle { get; }

        /// <summary>
        /// Gets the content type identifier.
        /// </summary>
        /// <value>
        /// The content type identifier.
        /// </value>
        public int ContentTypeID { get; }

        /// <summary>
        /// Gets or sets the name of the content type.
        /// </summary>
        /// <value>
        /// The name of the content type.
        /// </value>
        public string ContentTypeName { get; }

        /// <summary>
        /// Gets the content volume.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public double ContentVolume { get; }

        /// <summary>
        /// Gets or sets the content quantity.
        /// </summary>
        /// <value>
        /// The content quantity.
        /// </value>
        public int ContentQuantity { get; }

        /// <summary>
        /// Gets or sets the last launch time.
        /// </summary>
        /// <value>
        /// The last launch time.
        /// </value>
        public DateTime LastLaunchTime { get; }

        /// <summary>
        /// Gets or sets the install time.
        /// </summary>
        /// <value>
        /// The install time.
        /// </value>
        public DateTime InstallTime { get; }

        /// <summary>
        /// Gets or sets the expiry time.
        /// </summary>
        /// <value>
        /// The expiry time.
        /// </value>
        public DateTime ExpiryTime { get; }

        /// <summary>
        /// Gets or sets the jobs state.
        /// </summary>
        public PlanetaryPinState State { get; set; }

        /// <summary>
        /// Gets the estimated time to completion.
        /// </summary>
        public string TTC 
            => State == PlanetaryPinState.Extracting
            ? ExpiryTime.ToRemainingTimeDigitalDescription(DateTimeKind.Utc)
            : String.Empty;

        /// <summary>
        /// Gets the linked to.
        /// </summary>
        /// <value>
        /// The linked to.
        /// </value>
        public IEnumerable<PlanetaryPin> LinkedTo
            => Colony.Links
                .Where(link => link.SourcePinID == ID || link.DestinationPinID == ID)
                .SelectMany(link => Colony.Pins
                    .Where(pin => pin.ID != ID && (pin.ID == link.SourcePinID || pin.ID == link.DestinationPinID)));

        /// <summary>
        /// Gets the routed to.
        /// </summary>
        /// <value>
        /// The routed to.
        /// </value>
        public IEnumerable<PlanetaryPin> RoutedTo
            => Colony.Routes
                .Where(route => route.SourcePinID == ID || route.DestinationPinID == ID)
                .SelectMany(route => Colony.Pins
                    .Where(pin => pin.ID != ID && (pin.ID == route.SourcePinID || pin.ID == route.DestinationPinID)));

        /// <summary>
        /// Gets true if we have notified the user.
        /// </summary>
        public bool NotificationSend { get; set; }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the name of the pin.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        private string GetPinName(string typeName)
        {
            int lenght = m_baseString.Length - 1;
            string pinNameID = String.Empty;

            for (int i = 0; i < 5; i++)
            {
                pinNameID += m_baseString[(int)(ID / Math.Pow(lenght, i) % lenght)];
            }

            return String.Format(CultureConstants.InvariantCulture, "{0} {1}-{2}", typeName,
                pinNameID.Substring(0, 2), pinNameID.Substring(2, 3));
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <returns></returns>
        private PlanetaryPinState GetState()
        {
            if (DBConstants.EcuTypeIDs.Contains(TypeID))
            {
                return ExpiryTime > DateTime.UtcNow
                    ? PlanetaryPinState.Extracting
                    : PlanetaryPinState.Idle;
            }

            return PlanetaryPinState.None;
        }

        /// <summary>
        /// Gets the volume.
        /// </summary>
        /// <returns></returns>
        private double GetVolume()
        {
            Item item = StaticItems.GetItemByID(ContentTypeID);
            return item != null && m_volumeProperty != null
                ? m_volumeProperty.GetNumericValue(item) * ContentQuantity
                : 0d;
        }

        #endregion
    }
}
