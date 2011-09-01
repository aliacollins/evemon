using System.Collections.Generic;
using System.Linq;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.SettingsObjects;

namespace EVEMon
{
    public sealed class EveMailMessagesColumnsSelectWindow : ColumnSelectWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EveMailMessagesColumnsSelectWindow"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public EveMailMessagesColumnsSelectWindow(IEnumerable<EveMailMessagesColumnSettings> settings)
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
            return ((EveMailMessagesColumn)key).GetDescription();
        }

        /// <summary>
        /// Gets all keys.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<int> GetAllKeys()
        {
            return
                EnumExtensions.GetValues<EveMailMessagesColumn>().Where(x => x != EveMailMessagesColumn.None).Select(x => (int)x);
        }

        /// <summary>
        /// Gets the default columns.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<IColumnSettings> GetDefaultColumns()
        {
            EveMailMessagesSettings settings = new EveMailMessagesSettings();
            return settings.Columns;
        }
    }
}