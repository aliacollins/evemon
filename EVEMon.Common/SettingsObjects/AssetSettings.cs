using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Settings for Assets.
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    public class AssetSettings
    {
        private readonly Collection<AssetColumnSettings> m_columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetSettings"/> class.
        /// </summary>
        public AssetSettings()
        {
            m_columns = new Collection<AssetColumnSettings>();
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>The columns.</value>
        [XmlArray("columns")]
        [XmlArrayItem("column")]
        public Collection<AssetColumnSettings> Columns
        {
            get { return m_columns; }
        }

        /// <summary>
        /// Gets the default columns.
        /// </summary>
        /// <value>The default columns.</value>
        public IEnumerable<AssetColumnSettings> DefaultColumns
        {
            get
            {
                AssetColumn[] defaultColumns = new[]
                                                   {
                                                       AssetColumn.ItemName,
                                                       AssetColumn.Quantity,
                                                       AssetColumn.Group,
                                                       AssetColumn.Category,
                                                       AssetColumn.Container,
                                                       AssetColumn.Flag,
                                                       AssetColumn.Location
                                                   };

                return EnumExtensions.GetValues<AssetColumn>().Where(
                    column => column != AssetColumn.None).Where(
                        column => Columns.All(columnSetting => columnSetting.Column != column)).Select(
                            column => new AssetColumnSettings
                                          {
                                              Column = column,
                                              Visible = defaultColumns.Contains(column),
                                              Width = -2
                                          });
            }
        }
    }
}