using System;
using System.Globalization;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    public sealed class EveProperty
    {
        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="serial"></param>
        internal EveProperty(EvePropertyCategory category, SerializableProperty serial)
        {
            ID = serial.ID;
            Name = serial.Name;
            Description = serial.Description;
            DefaultValue = serial.DefaultValue;
            Icon = serial.Icon;
            Unit = serial.Unit;
            UnitID = serial.UnitID;
            HigherIsBetter = serial.HigherIsBetter;
            Category = category;

            switch (ID)
            {
                case DBConstants.CPUNeedPropertyID:
                    CPU = this;
                    break;
                case DBConstants.PGNeedPropertyID:
                    Powergrid = this;
                    break;
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the property's ID.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Gets the property name (the displayName in the CCP tables).
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the property's description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the string representation of the default value.
        /// </summary>
        public string DefaultValue { get; private set; }

        /// <summary>
        /// Gets the icon for this property.
        /// </summary>
        public string Icon { get; private set; }

        /// <summary>
        /// Gets the unit for this property.
        /// </summary>
        public string Unit { get; private set; }

        /// <summary>
        /// Gets the unitID for this property.
        /// </summary>
        public long UnitID { get; private set; }

        /// <summary>
        /// When true, the higher the value, the better it is.
        /// </summary>
        public bool HigherIsBetter { get; private set; }

        /// <summary>
        /// Gets the property's category.
        /// </summary>
        public EvePropertyCategory Category { get; private set; }

        /// <summary>
        /// When true, the property is always visible in the ships browsers, even when an object has no value for this property.
        /// </summary>
        public bool AlwaysVisibleForShips { get; internal set; }

        /// <summary>
        /// When true, the property is hidden in the ships/items browsers if the value is the same then the default value.
        /// </summary>
        public bool HideIfDefault { get; internal set; }

        /// <summary>
        /// Gets the CPU Output property.
        /// </summary>
        public static EveProperty CPU { get; private set; }

        /// <summary>
        /// Gets the Powergrid Output property.
        /// </summary>
        public static EveProperty Powergrid { get; private set; }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the label of this property's value for the given item or, when the property is absent, the default value.
        /// The returned string will include the unit.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public String GetLabelOrDefault(Item obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            EvePropertyValue? value = obj.Properties[ID];
            return Format(value == null ? DefaultValue : value.Value.Value);
        }

        /// <summary>
        /// Gets the numeric interpretation of the object's property value, or NaN if the property value is not a numeric. 
        /// When the given object has no value for this property, we use the default one.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Double GetNumericValue(Item obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            // Retrieve the string for the number
            Nullable<EvePropertyValue> value = obj.Properties[ID];
            String number = (value == null ? DefaultValue : value.Value.Value);

            // Try to parse it as a real
            Single result;
            return Single.TryParse(number, NumberStyles.Number, CultureConstants.InvariantCulture, out result)
                       ? result
                       : default(Single);
        }

        /// <summary>
        /// Format a property value as shown in EVE.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private String Format(String value)
        {
            Single numericValue;
            if (Single.TryParse(value, NumberStyles.Number, CultureConstants.InvariantCulture, out numericValue))
            {
                try
                {
                    // Format a value of Packaged Volume
                    if (Name == DBConstants.PackagedVolumePropertyName)
                        return String.Format(CultureConstants.DefaultCulture, "{0:N1} {1}", numericValue, Unit);

                    // Format a value of Structure Volume
                    if (ID == DBConstants.VolumePropertyID)
                    {
                        return String.Format(CultureConstants.DefaultCulture, numericValue <= 1000
                                                                                  ? "{0:N2} {1}"
                                                                                  : "{0:#,##0.0##} {1}", numericValue, Unit);
                    }

                    // Format a value of Capacitor Capacity
                    if (ID == DBConstants.CapacitorCapacityPropertyID)
                        return String.Format(CultureConstants.DefaultCulture, "{0:N0} {1}", Math.Floor(numericValue), Unit);

                    // Format a value of Ships Warp Speed
                    if (ID == DBConstants.ShipWarpSpeedPropertyID)
                        return String.Format(CultureConstants.DefaultCulture, "{0:N2} {1}", numericValue, Unit);

                    switch (UnitID)
                    {
                            // Format a value of Mass
                        case DBConstants.MassUnitID:
                            return String.Format(CultureConstants.DefaultCulture, numericValue <= 1000
                                                                                      ? "{0:#,##0.0#} {1}"
                                                                                      : "{0:N0} {1}", numericValue, Unit);

                            // Format a value of Millseconds
                        case DBConstants.MillsecondsUnitID:
                            return String.Format(CultureConstants.DefaultCulture, "{0:N2} {1}", numericValue / 1000, Unit);

                            // Format a value of Absolute Percentage
                        case DBConstants.AbsolutePercentUnitID:
                            return String.Format(CultureConstants.DefaultCulture, "{0} {1}", (numericValue) * 100, Unit);

                            // Format a value of Inverse Absolute Percentage
                        case DBConstants.InverseAbsolutePercentUnitID:
                            return String.Format(CultureConstants.DefaultCulture, "{0} {1}", (1 - numericValue) * 100, Unit);

                            // Format a value of Modifier Percentage
                        case DBConstants.ModifierPercentUnitID:
                            return String.Format(CultureConstants.DefaultCulture, "{0:0.###} {1}", (numericValue - 1) * 100, Unit);

                            // Format a value of Inverse Modifier Percentage
                        case DBConstants.InversedModifierPercentUnitID:
                            return String.Format(CultureConstants.DefaultCulture, "{0:0.###} {1}", (1 - numericValue) * 100, Unit);

                            // A reference to a group (groupID), it has been pre-transformed on XmlGenerator
                        case DBConstants.GroupIDUnitID:
                            return value;

                            // A reference to an item or a skill (typeID)
                        case DBConstants.TypeUnitID:
                            int id = Int32.Parse(value, CultureConstants.InvariantCulture);
                            Item item = StaticItems.GetItemByID(id);
                            return id == 0
                                       ? String.Empty
                                       : item != null
                                             ? item.Name
                                             : "Unknown";

                            // Format a Sizeclass ("1=small 2=medium 3=l")
                        case DBConstants.SizeclassUnitID:
                            int size = Int32.Parse(value, CultureConstants.InvariantCulture);
                            switch (size)
                            {
                                case 1:
                                    return "Small";
                                case 2:
                                    return "Medium";
                                case 3:
                                    return "Large";
                                case 4:
                                    return "Extra Large";
                                default:
                                    return "Unknown";
                            }

                            // Format all other values (use of thousand and decimal separator)
                        default:
                            return String.Format(CultureConstants.DefaultCulture, "{0:#,##0.###} {1}", numericValue, Unit);
                    }
                }
                catch (FormatException)
                {
                    return "N/A";
                }
            }
            return value;
        }

        #endregion
    }
}