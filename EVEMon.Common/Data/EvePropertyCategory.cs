using System;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    public sealed class EvePropertyCategory : ReadonlyCollection<EveProperty>
    {
        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="serial"></param>
        public EvePropertyCategory(SerializablePropertyCategory serial)
            : base(serial != null ? serial.Properties.Count : 0)
        {
            if (serial == null)
                throw new ArgumentNullException("serial");

            ID = serial.ID;
            Name = serial.Name;
            Description = serial.Description;

            foreach (SerializableProperty serialProp in serial.Properties)
            {
                Items.Add(new EveProperty(this, serialProp));
            }

            // Sets the display name
            switch (Name)
            {
                default:
                    DisplayName = Name;
                    break;
                case "NULL":
                    DisplayName = "System";
                    break;
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets this category's id.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Gets this category's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets this category's description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the category's display name.
        /// </summary>
        public string DisplayName { get; private set; }

        #endregion
    }
}