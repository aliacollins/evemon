using System.Collections.Generic;
using System.Linq;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.CharacterMonitoring
{
    public sealed class WalletTransactionsColumnsSelectWindow : ColumnSelectWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WalletTransactionsColumnsSelectWindow"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public WalletTransactionsColumnsSelectWindow(IEnumerable<WalletTransactionColumnSettings> settings)
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
            return ((WalletTransactionColumn)key).GetDescription();
        }

        /// <summary>
        /// Gets all keys.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<int> AllKeys
        {
            get
            {
                return EnumExtensions.GetValues<WalletTransactionColumn>().Where(
                    x => x != WalletTransactionColumn.None).Select(x => (int)x);
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
                WalletTransactionSettings settings = new WalletTransactionSettings();
                return settings.DefaultColumns;
            }
        }
    }
}
