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
        public EveMailMessagesColumnsSelectWindow(IEnumerable<EveMailMessageColumnSettings> settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected override string GetHeader(int key)
        {
            return ((EveMailMessageColumn)key).GetDescription();
        }

        /// <summary>
        /// Gets all keys.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<int> AllKeys
        {
            get
            {
                return EnumExtensions.GetValues<EveMailMessageColumn>().Where(
                    x => x != EveMailMessageColumn.None).Select(x => (int)x);
            }
        }

        /// <summary>
        /// Gets the default columns.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<IColumnSettings> DefaultColumns
        {
            get
            {
                EveMailMessageSettings settings = new EveMailMessageSettings();
                return settings.DefaultColumns;
            }
        }
    }
}