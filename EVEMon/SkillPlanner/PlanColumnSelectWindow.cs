using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Controls;
using EVEMon.Common.Extensions;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.SkillPlanner
{
    public sealed class PlanColumnSelectWindow : ColumnSelectWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlanColumnSelectWindow"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public PlanColumnSelectWindow(IEnumerable<PlanColumnSettings> settings)
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
            return ((PlanColumn)key).GetDescription();
        }

        /// <summary>
        /// Gets all keys.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<int> AllKeys
        {
            get
            {
                return EnumExtensions.GetValues<PlanColumn>().Where(
                    x => x != PlanColumn.None).Select(x => (int)x);
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
                PlanWindowSettings settings = new PlanWindowSettings();
                return settings.DefaultColumns;
            }
        }
    }
}