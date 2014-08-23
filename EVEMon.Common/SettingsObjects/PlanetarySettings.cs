using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Settings for Planetary Colonies.
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    public sealed class PlanetarySettings
    {
        private readonly Collection<PlanetaryColumnSettings> m_columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanetarySettings"/> class.
        /// </summary>
        public PlanetarySettings()
        {
            m_columns = new Collection<PlanetaryColumnSettings>();
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>The columns.</value>
        [XmlArray("columns")]
        [XmlArrayItem("column")]
        public Collection<PlanetaryColumnSettings> Columns
        {
            get { return m_columns; }
        }

        /// <summary>
        /// Gets the default columns.
        /// </summary>
        /// <value>The default columns.</value>
        public IEnumerable<PlanetaryColumnSettings> DefaultColumns
        {
            get
            {
                PlanetaryColumn[] defaultColumns =
                {
                    PlanetaryColumn.State,
                    PlanetaryColumn.TTC,
                    PlanetaryColumn.TypeName,
                    PlanetaryColumn.ContentTypeName,
                    PlanetaryColumn.Quantity,
                    PlanetaryColumn.QuantityPerCycle,
                };

                return EnumExtensions.GetValues<PlanetaryColumn>().Where(
                    column => column != PlanetaryColumn.None).Where(
                        column => Columns.All(columnSetting => columnSetting.Column != column)).Select(
                            column => new PlanetaryColumnSettings
                            {
                                Column = column,
                                Visible = defaultColumns.Contains(column),
                                Width = -2
                            });
            }
        }
    }
}
