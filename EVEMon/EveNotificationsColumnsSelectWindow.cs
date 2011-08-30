using System.Collections.Generic;
using System.Linq;

using EVEMon.Common;
using EVEMon.Common.SettingsObjects;
using EVEMon.Controls;

namespace EVEMon
{
    public sealed class EveNotificationsColumnsSelectWindow : ColumnSelectWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EveNotificationsColumnsSelectWindow"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public EveNotificationsColumnsSelectWindow(IEnumerable<EveNotificationsColumnSettings> settings)
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
            return ((EveNotificationsColumn)key).GetDescription();
        }

        /// <summary>
        /// Gets all keys.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<int> GetAllKeys()
        {
            return EnumExtensions.GetValues<EveNotificationsColumn>().Where(x => x != EveNotificationsColumn.None).Select(x => (int)x);
        }

        /// <summary>
        /// Gets the default columns.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<IColumnSettings> GetDefaultColumns()
        {
            EveNotificationsSettings settings = new EveNotificationsSettings();
            return settings.Columns;
        }
    }
}
