using System.Collections.Generic;
using System.Linq;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.SettingsObjects;

namespace EVEMon
{
    public sealed class ResearchColumnsSelectWindow : ColumnSelectWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResearchColumnsSelectWindow"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public ResearchColumnsSelectWindow(IEnumerable<ResearchColumnSettings> settings)
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
            return ((ResearchColumn)key).GetDescription();
        }

        /// <summary>
        /// Gets all keys.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<int> AllKeys
        {
            get
            {
                return EnumExtensions.GetValues<ResearchColumn>().Where(
                    x => x != ResearchColumn.None).Select(x => (int)x);
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
                ResearchSettings settings = new ResearchSettings();
                return settings.DefaultColumns;
            }
        }
    }
}