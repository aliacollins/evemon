using System;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class PlanetaryPin
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanetaryPin"/> class.
        /// </summary>
        /// <param name="colony">The colony.</param>
        /// <param name="src">The source.</param>
        internal PlanetaryPin(PlanetaryColony colony, SerializablePlanetaryPin src)
        {
            Colony = colony;
            TypeName = src.TypeName;
            SchematicID = src.SchematicID;
            CycleTime = src.CycleTime;
            QuantityPerCycle = src.QuantityPerCycle;
            ContentTypeName = src.ContentTypeName;
            ContentQuantity = src.ContentQuantity;
            LastLaunchTime = src.LastLaunchTime;
            InstallTime = src.InstallTime;
            ExpiryTime = src.ExpiryTime;
            State = GetState();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the colony.
        /// </summary>
        /// <value>
        /// The colony.
        /// </value>
        public PlanetaryColony Colony { get; private set; }

        /// <summary>
        /// Gets or sets the name of the type.
        /// </summary>
        /// <value>
        /// The name of the type.
        /// </value>
        public string TypeName { get; private set; }

        /// <summary>
        /// Gets or sets the schematic identifier.
        /// </summary>
        /// <value>
        /// The schematic identifier.
        /// </value>
        public long SchematicID { get; private set; }

        /// <summary>
        /// Gets or sets the cycle time.
        /// </summary>
        /// <value>
        /// The cycle time.
        /// </value>
        public short CycleTime { get; private set; }

        /// <summary>
        /// Gets or sets the quantity per cycle.
        /// </summary>
        /// <value>
        /// The quantity per cycle.
        /// </value>
        public int QuantityPerCycle { get; private set; }

        /// <summary>
        /// Gets or sets the name of the content type.
        /// </summary>
        /// <value>
        /// The name of the content type.
        /// </value>
        public string ContentTypeName { get; private set; }

        /// <summary>
        /// Gets or sets the content quantity.
        /// </summary>
        /// <value>
        /// The content quantity.
        /// </value>
        public int ContentQuantity { get; private set; }

        /// <summary>
        /// Gets or sets the last launch time.
        /// </summary>
        /// <value>
        /// The last launch time.
        /// </value>
        public DateTime LastLaunchTime { get; private set; }

        /// <summary>
        /// Gets or sets the install time.
        /// </summary>
        /// <value>
        /// The install time.
        /// </value>
        public DateTime InstallTime { get; private set; }

        /// <summary>
        /// Gets or sets the expiry time.
        /// </summary>
        /// <value>
        /// The expiry time.
        /// </value>
        public DateTime ExpiryTime { get; private set; }

        /// <summary>
        /// Gets or sets the jobs state.
        /// </summary>
        public PlanetaryPinState State { get; private set; }

        /// <summary>
        /// Gets the estimated time to completion.
        /// </summary>
        public string TTC
        {
            get
            {
                return State == PlanetaryPinState.Extracting
                    ? ExpiryTime.ToRemainingTimeDigitalDescription(DateTimeKind.Utc)
                    : String.Empty;
            }
        }

        #endregion

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <returns></returns>
        private PlanetaryPinState GetState()
        {
            if (ExpiryTime == DateTime.MinValue)
                return PlanetaryPinState.None;

            if (TypeName.Contains("Extractor Control Unit"))
                return ExpiryTime > DateTime.UtcNow ? PlanetaryPinState.Extracting : PlanetaryPinState.Idle;

            return PlanetaryPinState.None;
        }
    }
}
