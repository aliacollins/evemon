using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.CharacterMonitoring
{
    public partial class CharacterPlanetaryColoniesList : UserControl, IListView
    {
        #region Constructor

        public CharacterPlanetaryColoniesList()
        {
            InitializeComponent();

            lvPlanetaryColonies.Visible = false;
            lvPlanetaryColonies.AllowColumnReorder = true;
            lvPlanetaryColonies.Columns.Clear();

            noPlanetaryColoniesLabel.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);

            ListViewHelper.EnableDoubleBuffer(lvPlanetaryColonies);

            //lvPlanetaryColonies.ColumnClick += lvPlanetaryColonies_ColumnClick;
            //lvPlanetaryColonies.ColumnWidthChanged += lvPlanetaryColonies_ColumnWidthChanged;
            //lvPlanetaryColonies.ColumnReordered += lvPlanetaryColonies_ColumnReordered;
        }

        #endregion


        public string TextFilter { get; set; }
        public Enum Grouping { get; set; }
        public IEnumerable<IColumnSettings> Columns { get; set; }
        public CCPCharacter Character { get; set; }

        public void AutoResizeColumns()
        {
            throw new NotImplementedException();
        }
    }
}
