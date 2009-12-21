using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Controls;
using EVEMon.Common.SettingsObjects;
using EVEMon.Common;

namespace EVEMon
{
    public sealed class MarketOrdersColumnsSelectWindow : ColumnSelectWindow
    {
        public MarketOrdersColumnsSelectWindow(IEnumerable<MarketOrderColumnSettings> settings)
            : base(settings.Select(x => x.Clone()).Cast<IColumnSettings>())
        {
        }

        protected override string GetHeader(int key)
        {
            return ((MarketOrderColumn)key).GetDescription();
        }

        protected override IEnumerable<int> GetAllKeys()
        {
            return EnumExtensions.GetValues<MarketOrderColumn>().Where(x => x != MarketOrderColumn.None).Select(x => (int)x);
        }

        protected override IEnumerable<IColumnSettings> GetDefaultColumns()
        {
            var settings = new MarketOrderSettings();
            return settings.Columns.Cast<IColumnSettings>();
        }
    }

}
