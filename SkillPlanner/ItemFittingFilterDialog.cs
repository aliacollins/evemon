using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace EVEMon.SkillPlanner
{
    public partial class ItemFittingFilterDialog : Form
    {
        public ItemFittingFilterDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Return the values entered into the dialog.
        /// </summary>
        /// <returns>The values entered into the dialog.</returns>
        public ItemFittingFilterData getItemFittingFilterData()
        {
            decimal? cpu = null;
            decimal? grid = null;

            decimal cpuValue = udCpuAvailable.Value;
            decimal gridValue = udPowergridAvailable.Value;

            if (cpuValue > 0.0m)
                cpu = cpuValue;

            if (gridValue > 0.0m)
                grid = gridValue;

            ItemFittingFilterData args = new ItemFittingFilterData();
            args.cpuAvailable = cpu;
            args.gridAvailable = grid;
            args.highSlotSelected = cbHighSlot.CheckState == CheckState.Checked;
            args.medSlotSelected = cbMedSlot.CheckState == CheckState.Checked;
            args.lowSlotSelected = cbLowSlot.CheckState == CheckState.Checked;

            return args;
        }

        /// <summary>
        /// Init the dialog box from the given filtering data.
        /// </summary>
        /// <param name="data">The data to initialise on</param>
        public void initFrom(ItemFittingFilterData data)
        {
            udCpuAvailable.Value = data.cpuAvailable == null ? 0m : (decimal)data.cpuAvailable;
            udPowergridAvailable.Value = data.gridAvailable == null ? 0m : (decimal)data.gridAvailable;
            cbHighSlot.CheckState = data.highSlotSelected ? CheckState.Checked : CheckState.Unchecked;
            cbMedSlot.CheckState = data.medSlotSelected ? CheckState.Checked : CheckState.Unchecked;
            cbLowSlot.CheckState = data.lowSlotSelected ? CheckState.Checked : CheckState.Unchecked;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            cbHighSlot.CheckState = CheckState.Checked;
            cbMedSlot.CheckState = CheckState.Checked;
            cbLowSlot.CheckState = CheckState.Checked;
            udCpuAvailable.Value = 0;
            udPowergridAvailable.Value = 0;
            this.AcceptButton.PerformClick();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }

    /// <summary>
    /// Encapsulates the various fitting aspects that we can filter on.
    /// </summary>
    public class ItemFittingFilterData
    {
        public ItemFittingFilterData()
        {
            //apply defaults: all slots visible, no cpu/grid restrictions
            highSlotSelected = true;
            medSlotSelected = true;
            lowSlotSelected = true;
            cpuAvailable = null;
            gridAvailable = null;
        }
        public bool allSlotsSelected()
        {
            return highSlotSelected && medSlotSelected && lowSlotSelected;
        }
        public bool allFilteringDisabled()
        {
            return allSlotsSelected() && cpuAvailable == null && gridAvailable == null;
        }
        public bool highSlotSelected;
        public bool medSlotSelected;
        public bool lowSlotSelected;
        public decimal? cpuAvailable;
        public decimal? gridAvailable;
    }
}
