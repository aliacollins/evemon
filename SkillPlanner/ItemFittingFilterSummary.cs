using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// The <code>ItemFittingFilterSummary</code> control displays the item
    /// fitting filtering options currently selected, and gives the user access
    /// to a dialog box to set different ones.
    /// </summary>
    public partial class ItemFittingFilterSummary : UserControl
    {
        private ItemFittingFilterData currentFilterData;

        public ItemFittingFilterSummary()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Clears all filter data and update the displayed values to match.
        /// </summary>
        public void reset()
        {
            this.currentFilterData = new ItemFittingFilterData();
            updateShownValues(this.currentFilterData);
            fireItemFilterDataChanged(this.currentFilterData);
        }

        private void btnConfigure_Click(object sender, EventArgs e)
        {
            ItemFittingFilterDialog dialog = new ItemFittingFilterDialog();
            if (currentFilterData != null)
                dialog.initFrom(currentFilterData);

            DialogResult result = dialog.ShowDialog(this.ParentForm);
            if (result == DialogResult.OK)
            {
                ItemFittingFilterData data = dialog.getItemFittingFilterData();
                currentFilterData = data;
                updateShownValues(currentFilterData);
                fireItemFilterDataChanged(currentFilterData);
            }
        }

        private static String format(decimal? formatme, String defaultValue)
        {
            if (!formatme.HasValue) return defaultValue;
            return formatme.Value.ToString("N0");
        }

        private void updateShownValues(ItemFittingFilterData data)
        {
            if (data == null)
            {
                //rely on defaults
                data = new ItemFittingFilterData();
            }
            lblCpuValue.Text = format(data.cpuAvailable, "no limit");
            lblGridValue.Text = format(data.gridAvailable, "no limit");
            if (data.allSlotsSelected())
            {
                lblSlotValue.Text = "any";
            }
            else
            {
                String value = "";
                if (data.highSlotSelected)
                {
                    value += "High";
                    if (data.medSlotSelected || data.lowSlotSelected)
                        value += ", ";
                }
                if (data.medSlotSelected)
                {
                    value += "Med";
                    if (data.lowSlotSelected)
                        value += ", ";
                }
                if (data.lowSlotSelected)
                {
                    value += "Low";
                }
                lblSlotValue.Text = value;
            }
        }

        private void fireItemFilterDataChanged(ItemFittingFilterData data)
        {
            if (ItemFilterDataChanged != null)
                ItemFilterDataChanged(this, new ItemFilteringChangedEvent(data));
        }

        /// <summary>
        /// This event is fired when the user sets item restrictions. Its EventArgs object
        /// is of type ItemRestrictionsChangedEvent.
        /// </summary>
        public event System.EventHandler<ItemFilteringChangedEvent> ItemFilterDataChanged;
    }

    public class ItemFilteringChangedEvent : EventArgs
    {
        public ItemFilteringChangedEvent(ItemFittingFilterData initWith)
        {
            this.filterData = initWith;
        }
        public ItemFittingFilterData filterData;
    }
}
