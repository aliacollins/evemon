using System.Collections.Generic;
using System.Linq;

using EVEMon.Common;
using EVEMon.Common.SettingsObjects;
using EVEMon.Controls;

namespace EVEMon
{
    public sealed class ResearchColumnsSelectWindow : ColumnSelectWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResearchColumnsSelectWindow"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public ResearchColumnsSelectWindow(IEnumerable<ResearchColumnSettings> settings)
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
            return ((ResearchColumn)key).GetDescription();
        }

        /// <summary>
        /// Gets all keys.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<int> GetAllKeys()
        {
            return EnumExtensions.GetValues<ResearchColumn>().Where(x => x != ResearchColumn.None).Select(x => (int)x);
        }

        /// <summary>
        /// Gets the default columns.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<IColumnSettings> GetDefaultColumns()
        {
            ResearchSettings settings = new ResearchSettings();
            return settings.Columns;
        }
    }
}
