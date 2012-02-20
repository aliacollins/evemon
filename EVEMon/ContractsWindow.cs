using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.SettingsObjects;

namespace EVEMon
{
    public partial class ContractsWindow : EVEMonForm
    {
        private readonly bool m_init;

        public ContractsWindow()
        {
            InitializeComponent();
            RememberPositionKey = "ContractsWindow";
            m_init = true;
        }

        /// <summary>
        /// Gets or sets the grouping mode.
        /// </summary>
        [Browsable(false)]
        public Enum Grouping
        {
            get { return contractsList.Grouping; }
            set
            {
                contractsList.Grouping = value;

                if (!m_init)
                    return;

                contractsList.UpdateColumns();
                contractsList.Visibility = !contractsList.Contracts.IsEmpty();
            }
        }

        /// <summary>
        /// Gets or sets the showIssuedFor mode.
        /// </summary>
        [Browsable(false)]
        public IssuedFor ShowIssuedFor
        {
            get { return contractsList.ShowIssuedFor; }
            set
            {
                contractsList.ShowIssuedFor = value;

                if (!m_init)
                    return;

                contractsList.UpdateColumns();
                contractsList.Visible = !contractsList.Contracts.IsEmpty();
            }
        }

        /// <summary>
        /// Gets or sets the enumeration of contracts to display.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IEnumerable<Contract> Contracts
        {
            get { return contractsList.Contracts; }
            set { contractsList.Contracts = value; }
        }

        /// <summary>
        /// Gets or sets the enumeration of displayed columns.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IEnumerable<IColumnSettings> Columns
        {
            get { return contractsList.Columns; }
            set
            {
                contractsList.Columns = value;

                if (!m_init)
                    return;

                contractsList.UpdateColumns();
                contractsList.Visibility = !contractsList.Contracts.IsEmpty();
            }
        }
    }
}
