using System.Collections.Generic;
using System.Linq;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.SettingsObjects;

namespace EVEMon
{
    public sealed class MarketOrdersColumnsSelectWindow : ColumnSelectWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketOrdersColumnsSelectWindow"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public MarketOrdersColumnsSelectWindow(IEnumerable<MarketOrderColumnSettings> settings)
            : base(settings.Select(x => x.Clone()))
        {
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected override string GetHeader(int key)
        {
            return ((MarketOrderColumn)key).GetDescription();
        }

        /// <summary>
        /// Gets all keys.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<int> GetAllKeys()
        {
            return EnumExtensions.GetValues<MarketOrderColumn>().Where(x => x != MarketOrderColumn.None).Select(x => (int)x);
        }

        /// <summary>
        /// Gets the default columns.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<IColumnSettings> GetDefaultColumns()
        {
            MarketOrderSettings settings = new MarketOrderSettings();
            return settings.Columns;
        }
    }
}