using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Controls;
using EVEMon.Common.Extensions;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.CharacterMonitoring
{
    public sealed class ContractsColumnsSelectWindow : ColumnSelectWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContractsColumnsSelectWindow"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public ContractsColumnsSelectWindow(IEnumerable<ContractColumnSettings> settings)
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
            return ((ContractColumn)key).GetDescription();
        }

        /// <summary>
        /// Gets all keys.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<int> AllKeys
        {
            get
            {
                return EnumExtensions.GetValues<ContractColumn>().Where(
                    x => x != ContractColumn.None).Select(x => (int)x);
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
                ContractSettings settings = new ContractSettings();
                return settings.DefaultColumns;
            }
        }
    }
}
