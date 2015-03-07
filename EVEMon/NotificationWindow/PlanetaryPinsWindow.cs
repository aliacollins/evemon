using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.NotificationWindow
{
    public partial class PlanetaryPinsWindow : EVEMonForm
    {
        private readonly bool m_init;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanetaryPinsWindow"/> class.
        /// </summary>
        public PlanetaryPinsWindow()
        {
            InitializeComponent();
            RememberPositionKey = "PlanetaryPinsWindow";
            m_init = true;
        }

        /// <summary>
        /// Gets or sets the grouping mode.
        /// </summary>
        internal Enum Grouping
        {
            get { return planetaryList.Grouping; }
            set
            {
                planetaryList.Grouping = value;

                if (!m_init)
                    return;

                planetaryList.UpdateColumns();
                planetaryList.Visibility = planetaryList.PlanetaryPins.Any();
            }
        }

        /// <summary>
        /// Gets or sets the enumeration of jobs to display.
        /// </summary>
        internal IEnumerable<PlanetaryPin> PlanetaryPins
        {
            get { return planetaryList.PlanetaryPins; }
            set { planetaryList.PlanetaryPins = value; }
        }

        /// <summary>
        /// Gets or sets the enumeration of displayed columns.
        /// </summary>
        internal IEnumerable<IColumnSettings> Columns
        {
            get { return planetaryList.Columns; }
            set
            {
                planetaryList.Columns = value;

                if (!m_init)
                    return;

                planetaryList.UpdateColumns();
                planetaryList.Visibility = planetaryList.PlanetaryPins.Any();
            }
        }
    }
}
