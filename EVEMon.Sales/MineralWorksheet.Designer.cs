namespace EVEMon.Sales
{
    partial class MineralWorksheet
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MineralWorksheet));
            this.MineralWorksheetToolStrip = new System.Windows.Forms.ToolStrip();
            this.btnLockPrices = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsddFetch = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnReset = new System.Windows.Forms.ToolStripButton();
            this.copyTotalDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.copyFormattedTotalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyUnformattedTotalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MineralWorksheetStatusStrip = new System.Windows.Forms.StatusStrip();
            this.TotalValueToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslTotal = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslCourtesy = new System.Windows.Forms.ToolStripStatusLabel();
            this.mineralTile2 = new EVEMon.Sales.MineralTile();
            this.mineralTile3 = new EVEMon.Sales.MineralTile();
            this.mt_mexallon = new EVEMon.Sales.MineralTile();
            this.mt_pyerite = new EVEMon.Sales.MineralTile();
            this.mt_megacyte = new EVEMon.Sales.MineralTile();
            this.mt_isogen = new EVEMon.Sales.MineralTile();
            this.mt_zydrine = new EVEMon.Sales.MineralTile();
            this.mt_morphite = new EVEMon.Sales.MineralTile();
            this.mt_nocxium = new EVEMon.Sales.MineralTile();
            this.mt_tritanium = new EVEMon.Sales.MineralTile();
            this.MineralTileTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.mineralTile1 = new EVEMon.Sales.MineralTile();
            this.MineralWorksheetToolStrip.SuspendLayout();
            this.MineralWorksheetStatusStrip.SuspendLayout();
            this.MineralTileTableLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // MineralWorksheetToolStrip
            // 
            this.MineralWorksheetToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLockPrices,
            this.toolStripSeparator1,
            this.tsddFetch,
            this.btnReset,
            this.copyTotalDropDownButton});
            this.MineralWorksheetToolStrip.Location = new System.Drawing.Point(0, 0);
            this.MineralWorksheetToolStrip.Name = "MineralWorksheetToolStrip";
            this.MineralWorksheetToolStrip.Size = new System.Drawing.Size(654, 25);
            this.MineralWorksheetToolStrip.TabIndex = 0;
            this.MineralWorksheetToolStrip.Text = "toolStrip1";
            // 
            // btnLockPrices
            // 
            this.btnLockPrices.Image = ((System.Drawing.Image)(resources.GetObject("btnLockPrices.Image")));
            this.btnLockPrices.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLockPrices.Name = "btnLockPrices";
            this.btnLockPrices.Size = new System.Drawing.Size(86, 22);
            this.btnLockPrices.Text = "Lock Prices";
            this.btnLockPrices.Click += new System.EventHandler(this.btnLockPrices_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsddFetch
            // 
            this.tsddFetch.Image = ((System.Drawing.Image)(resources.GetObject("tsddFetch.Image")));
            this.tsddFetch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddFetch.Name = "tsddFetch";
            this.tsddFetch.Size = new System.Drawing.Size(137, 22);
            this.tsddFetch.Text = "Fetch Online Prices";
            // 
            // btnReset
            // 
            this.btnReset.Image = ((System.Drawing.Image)(resources.GetObject("btnReset.Image")));
            this.btnReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(112, 22);
            this.btnReset.Text = "Reset Quantities";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // copyTotalDropDownButton
            // 
            this.copyTotalDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyFormattedTotalToolStripMenuItem,
            this.copyUnformattedTotalToolStripMenuItem});
            this.copyTotalDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("copyTotalDropDownButton.Image")));
            this.copyTotalDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyTotalDropDownButton.Name = "copyTotalDropDownButton";
            this.copyTotalDropDownButton.Size = new System.Drawing.Size(94, 22);
            this.copyTotalDropDownButton.Text = "Copy Total";
            this.copyTotalDropDownButton.DropDownOpening += new System.EventHandler(this.copyTotalDropDownButton_DropDownOpening);
            // 
            // copyFormattedTotalToolStripMenuItem
            // 
            this.copyFormattedTotalToolStripMenuItem.Name = "copyFormattedTotalToolStripMenuItem";
            this.copyFormattedTotalToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.copyFormattedTotalToolStripMenuItem.Text = "Formatted";
            this.copyFormattedTotalToolStripMenuItem.Click += new System.EventHandler(this.copyFormattedTotalToolStripMenuItem_Click);
            // 
            // copyUnformattedTotalToolStripMenuItem
            // 
            this.copyUnformattedTotalToolStripMenuItem.Name = "copyUnformattedTotalToolStripMenuItem";
            this.copyUnformattedTotalToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.copyUnformattedTotalToolStripMenuItem.Text = "Unformatted";
            this.copyUnformattedTotalToolStripMenuItem.Click += new System.EventHandler(this.copyUnformattedTotalToolStripMenuItem_Click);
            // 
            // MineralWorksheetStatusStrip
            // 
            this.MineralWorksheetStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TotalValueToolStripStatusLabel,
            this.tslTotal,
            this.tslCourtesy});
            this.MineralWorksheetStatusStrip.Location = new System.Drawing.Point(0, 510);
            this.MineralWorksheetStatusStrip.Name = "MineralWorksheetStatusStrip";
            this.MineralWorksheetStatusStrip.Size = new System.Drawing.Size(654, 22);
            this.MineralWorksheetStatusStrip.TabIndex = 10;
            this.MineralWorksheetStatusStrip.Text = "statusStrip1";
            // 
            // TotalValueToolStripStatusLabel
            // 
            this.TotalValueToolStripStatusLabel.Name = "TotalValueToolStripStatusLabel";
            this.TotalValueToolStripStatusLabel.Size = new System.Drawing.Size(69, 17);
            this.TotalValueToolStripStatusLabel.Text = "Total Value:";
            // 
            // tslTotal
            // 
            this.tslTotal.Image = ((System.Drawing.Image)(resources.GetObject("tslTotal.Image")));
            this.tslTotal.Margin = new System.Windows.Forms.Padding(5, 3, 0, 2);
            this.tslTotal.Name = "tslTotal";
            this.tslTotal.Size = new System.Drawing.Size(63, 17);
            this.tslTotal.Text = "0.00 ISK";
            // 
            // tslCourtesy
            // 
            this.tslCourtesy.Image = ((System.Drawing.Image)(resources.GetObject("tslCourtesy.Image")));
            this.tslCourtesy.IsLink = true;
            this.tslCourtesy.Margin = new System.Windows.Forms.Padding(15, 3, 0, 2);
            this.tslCourtesy.Name = "tslCourtesy";
            this.tslCourtesy.Size = new System.Drawing.Size(179, 17);
            this.tslCourtesy.Text = "Mineral Prices Courtesy of ---";
            this.tslCourtesy.Visible = false;
            this.tslCourtesy.Click += new System.EventHandler(this.tslCourtesy_Click);
            // 
            // mineralTile2
            // 
            this.mineralTile2.AutoSize = true;
            this.mineralTile2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mineralTile2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(44)))));
            this.mineralTile2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mineralTile2.Location = new System.Drawing.Point(13, 120);
            this.mineralTile2.Margin = new System.Windows.Forms.Padding(4);
            this.mineralTile2.MineralName = "";
            this.mineralTile2.Name = "mineralTile2";
            this.mineralTile2.PriceLocked = false;
            this.mineralTile2.PricePerUnit = new decimal(new int[] {
            0,
            0,
            0,
            131072});
            this.mineralTile2.Quantity = 0;
            this.mineralTile2.Size = new System.Drawing.Size(264, 83);
            this.mineralTile2.TabIndex = 1;
            // 
            // mineralTile3
            // 
            this.mineralTile3.AutoSize = true;
            this.mineralTile3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mineralTile3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(44)))));
            this.mineralTile3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mineralTile3.Location = new System.Drawing.Point(13, 211);
            this.mineralTile3.Margin = new System.Windows.Forms.Padding(4);
            this.mineralTile3.MineralName = "";
            this.mineralTile3.Name = "mineralTile3";
            this.mineralTile3.PriceLocked = false;
            this.mineralTile3.PricePerUnit = new decimal(new int[] {
            0,
            0,
            0,
            131072});
            this.mineralTile3.Quantity = 0;
            this.mineralTile3.Size = new System.Drawing.Size(264, 83);
            this.mineralTile3.TabIndex = 2;
            // 
            // mt_mexallon
            // 
            this.mt_mexallon.AutoSize = true;
            this.mt_mexallon.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mt_mexallon.Location = new System.Drawing.Point(4, 238);
            this.mt_mexallon.Margin = new System.Windows.Forms.Padding(4);
            this.mt_mexallon.MineralName = "Mexallon";
            this.mt_mexallon.Name = "mt_mexallon";
            this.mt_mexallon.PriceLocked = false;
            this.mt_mexallon.PricePerUnit = new decimal(new int[] {
            0,
            0,
            0,
            131072});
            this.mt_mexallon.Quantity = 0;
            this.mt_mexallon.Size = new System.Drawing.Size(261, 109);
            this.mt_mexallon.TabIndex = 3;
            this.mt_mexallon.MineralPriceChanged += new System.EventHandler<System.EventArgs>(this.mt_MineralPriceChanged);
            // 
            // mt_pyerite
            // 
            this.mt_pyerite.AutoSize = true;
            this.mt_pyerite.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mt_pyerite.Location = new System.Drawing.Point(4, 121);
            this.mt_pyerite.Margin = new System.Windows.Forms.Padding(4);
            this.mt_pyerite.MineralName = "Pyerite";
            this.mt_pyerite.Name = "mt_pyerite";
            this.mt_pyerite.PriceLocked = false;
            this.mt_pyerite.PricePerUnit = new decimal(new int[] {
            0,
            0,
            0,
            131072});
            this.mt_pyerite.Quantity = 0;
            this.mt_pyerite.Size = new System.Drawing.Size(261, 109);
            this.mt_pyerite.TabIndex = 2;
            this.mt_pyerite.MineralPriceChanged += new System.EventHandler<System.EventArgs>(this.mt_MineralPriceChanged);
            // 
            // mt_megacyte
            // 
            this.mt_megacyte.AutoSize = true;
            this.mt_megacyte.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mt_megacyte.Location = new System.Drawing.Point(273, 238);
            this.mt_megacyte.Margin = new System.Windows.Forms.Padding(4);
            this.mt_megacyte.MineralName = "Megacyte";
            this.mt_megacyte.Name = "mt_megacyte";
            this.mt_megacyte.PriceLocked = false;
            this.mt_megacyte.PricePerUnit = new decimal(new int[] {
            0,
            0,
            0,
            131072});
            this.mt_megacyte.Quantity = 0;
            this.mt_megacyte.Size = new System.Drawing.Size(261, 109);
            this.mt_megacyte.TabIndex = 7;
            this.mt_megacyte.MineralPriceChanged += new System.EventHandler<System.EventArgs>(this.mt_MineralPriceChanged);
            // 
            // mt_isogen
            // 
            this.mt_isogen.AutoSize = true;
            this.mt_isogen.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mt_isogen.Location = new System.Drawing.Point(4, 355);
            this.mt_isogen.Margin = new System.Windows.Forms.Padding(4);
            this.mt_isogen.MineralName = "Isogen";
            this.mt_isogen.Name = "mt_isogen";
            this.mt_isogen.PriceLocked = false;
            this.mt_isogen.PricePerUnit = new decimal(new int[] {
            0,
            0,
            0,
            131072});
            this.mt_isogen.Quantity = 0;
            this.mt_isogen.Size = new System.Drawing.Size(261, 109);
            this.mt_isogen.TabIndex = 4;
            this.mt_isogen.MineralPriceChanged += new System.EventHandler<System.EventArgs>(this.mt_MineralPriceChanged);
            // 
            // mt_zydrine
            // 
            this.mt_zydrine.AutoSize = true;
            this.mt_zydrine.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mt_zydrine.Location = new System.Drawing.Point(273, 121);
            this.mt_zydrine.Margin = new System.Windows.Forms.Padding(4);
            this.mt_zydrine.MineralName = "Zydrine";
            this.mt_zydrine.Name = "mt_zydrine";
            this.mt_zydrine.PriceLocked = false;
            this.mt_zydrine.PricePerUnit = new decimal(new int[] {
            0,
            0,
            0,
            131072});
            this.mt_zydrine.Quantity = 0;
            this.mt_zydrine.Size = new System.Drawing.Size(261, 109);
            this.mt_zydrine.TabIndex = 6;
            this.mt_zydrine.MineralPriceChanged += new System.EventHandler<System.EventArgs>(this.mt_MineralPriceChanged);
            // 
            // mt_morphite
            // 
            this.mt_morphite.AutoSize = true;
            this.mt_morphite.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mt_morphite.Location = new System.Drawing.Point(273, 355);
            this.mt_morphite.Margin = new System.Windows.Forms.Padding(4);
            this.mt_morphite.MineralName = "Morphite";
            this.mt_morphite.Name = "mt_morphite";
            this.mt_morphite.PriceLocked = false;
            this.mt_morphite.PricePerUnit = new decimal(new int[] {
            0,
            0,
            0,
            131072});
            this.mt_morphite.Quantity = 0;
            this.mt_morphite.Size = new System.Drawing.Size(261, 109);
            this.mt_morphite.TabIndex = 8;
            this.mt_morphite.MineralPriceChanged += new System.EventHandler<System.EventArgs>(this.mt_MineralPriceChanged);
            // 
            // mt_nocxium
            // 
            this.mt_nocxium.AutoSize = true;
            this.mt_nocxium.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mt_nocxium.Location = new System.Drawing.Point(273, 4);
            this.mt_nocxium.Margin = new System.Windows.Forms.Padding(4);
            this.mt_nocxium.MineralName = "Nocxium";
            this.mt_nocxium.Name = "mt_nocxium";
            this.mt_nocxium.PriceLocked = false;
            this.mt_nocxium.PricePerUnit = new decimal(new int[] {
            0,
            0,
            0,
            131072});
            this.mt_nocxium.Quantity = 0;
            this.mt_nocxium.Size = new System.Drawing.Size(261, 109);
            this.mt_nocxium.TabIndex = 5;
            this.mt_nocxium.MineralPriceChanged += new System.EventHandler<System.EventArgs>(this.mt_MineralPriceChanged);
            // 
            // mt_tritanium
            // 
            this.mt_tritanium.AutoSize = true;
            this.mt_tritanium.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mt_tritanium.Location = new System.Drawing.Point(4, 4);
            this.mt_tritanium.Margin = new System.Windows.Forms.Padding(4);
            this.mt_tritanium.MineralName = "Tritanium";
            this.mt_tritanium.Name = "mt_tritanium";
            this.mt_tritanium.PriceLocked = false;
            this.mt_tritanium.PricePerUnit = new decimal(new int[] {
            0,
            0,
            0,
            131072});
            this.mt_tritanium.Quantity = 0;
            this.mt_tritanium.Size = new System.Drawing.Size(261, 109);
            this.mt_tritanium.TabIndex = 1;
            this.mt_tritanium.MineralPriceChanged += new System.EventHandler<System.EventArgs>(this.mt_MineralPriceChanged);
            // 
            // MineralTileTableLayout
            // 
            this.MineralTileTableLayout.AutoSize = true;
            this.MineralTileTableLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MineralTileTableLayout.ColumnCount = 2;
            this.MineralTileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MineralTileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MineralTileTableLayout.Controls.Add(this.mt_tritanium, 0, 0);
            this.MineralTileTableLayout.Controls.Add(this.mt_nocxium, 1, 0);
            this.MineralTileTableLayout.Controls.Add(this.mt_morphite, 1, 3);
            this.MineralTileTableLayout.Controls.Add(this.mt_zydrine, 1, 1);
            this.MineralTileTableLayout.Controls.Add(this.mt_isogen, 0, 3);
            this.MineralTileTableLayout.Controls.Add(this.mt_megacyte, 1, 2);
            this.MineralTileTableLayout.Controls.Add(this.mt_pyerite, 0, 1);
            this.MineralTileTableLayout.Controls.Add(this.mt_mexallon, 0, 2);
            this.MineralTileTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MineralTileTableLayout.Location = new System.Drawing.Point(0, 25);
            this.MineralTileTableLayout.Name = "MineralTileTableLayout";
            this.MineralTileTableLayout.RowCount = 4;
            this.MineralTileTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MineralTileTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MineralTileTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MineralTileTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MineralTileTableLayout.Size = new System.Drawing.Size(654, 485);
            this.MineralTileTableLayout.TabIndex = 9;
            // 
            // mineralTile1
            // 
            this.mineralTile1.AutoSize = true;
            this.mineralTile1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mineralTile1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(44)))));
            this.mineralTile1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mineralTile1.Location = new System.Drawing.Point(13, 29);
            this.mineralTile1.Margin = new System.Windows.Forms.Padding(4);
            this.mineralTile1.MineralName = "";
            this.mineralTile1.Name = "mineralTile1";
            this.mineralTile1.PriceLocked = false;
            this.mineralTile1.PricePerUnit = new decimal(new int[] {
            0,
            0,
            0,
            131072});
            this.mineralTile1.Quantity = 0;
            this.mineralTile1.Size = new System.Drawing.Size(264, 83);
            this.mineralTile1.TabIndex = 0;
            // 
            // MineralWorksheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(654, 532);
            this.Controls.Add(this.MineralTileTableLayout);
            this.Controls.Add(this.MineralWorksheetStatusStrip);
            this.Controls.Add(this.MineralWorksheetToolStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MineralWorksheet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mineral Worksheet";
            this.Load += new System.EventHandler(this.MineralWorksheet_Load);
            this.MineralWorksheetToolStrip.ResumeLayout(false);
            this.MineralWorksheetToolStrip.PerformLayout();
            this.MineralWorksheetStatusStrip.ResumeLayout(false);
            this.MineralWorksheetStatusStrip.PerformLayout();
            this.MineralTileTableLayout.ResumeLayout(false);
            this.MineralTileTableLayout.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip MineralWorksheetToolStrip;
        private MineralTile mineralTile2;
        private MineralTile mineralTile3;
        private System.Windows.Forms.ToolStripButton btnLockPrices;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton tsddFetch;
        private System.Windows.Forms.StatusStrip MineralWorksheetStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel TotalValueToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel tslTotal;
        private System.Windows.Forms.ToolStripStatusLabel tslCourtesy;
        private System.Windows.Forms.ToolStripButton btnReset;
        private System.Windows.Forms.ToolStripDropDownButton copyTotalDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem copyFormattedTotalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyUnformattedTotalToolStripMenuItem;
        private MineralTile mt_mexallon;
        private MineralTile mt_pyerite;
        private MineralTile mt_megacyte;
        private MineralTile mt_isogen;
        private MineralTile mt_zydrine;
        private MineralTile mt_morphite;
        private MineralTile mt_nocxium;
        private MineralTile mt_tritanium;
        private System.Windows.Forms.TableLayoutPanel MineralTileTableLayout;
        private MineralTile mineralTile1;

    }
}




