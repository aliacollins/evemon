namespace EVEMon.CharacterMonitoring
{
    partial class CharacterWalletTransactionsList
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharacterWalletTransactionsList));
            this.noWalletTransactionsLabel = new System.Windows.Forms.Label();
            this.lvWalletTransactions = new System.Windows.Forms.ListView();
            this.ilIcons = new System.Windows.Forms.ImageList(this.components);
            this.chDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chItem = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chPrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chQuantity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // noWalletTransactionsLabel
            // 
            this.noWalletTransactionsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.noWalletTransactionsLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.noWalletTransactionsLabel.Location = new System.Drawing.Point(0, 0);
            this.noWalletTransactionsLabel.Name = "noWalletTransactionsLabel";
            this.noWalletTransactionsLabel.Size = new System.Drawing.Size(454, 434);
            this.noWalletTransactionsLabel.TabIndex = 3;
            this.noWalletTransactionsLabel.Text = "No wallet transactions are available.";
            this.noWalletTransactionsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lvWalletTransactions
            // 
            this.lvWalletTransactions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvWalletTransactions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chDate,
            this.chItem,
            this.chPrice,
            this.chQuantity});
            this.lvWalletTransactions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvWalletTransactions.FullRowSelect = true;
            this.lvWalletTransactions.HideSelection = false;
            this.lvWalletTransactions.Location = new System.Drawing.Point(0, 0);
            this.lvWalletTransactions.MultiSelect = false;
            this.lvWalletTransactions.Name = "lvWalletTransactions";
            this.lvWalletTransactions.Size = new System.Drawing.Size(454, 434);
            this.lvWalletTransactions.SmallImageList = this.ilIcons;
            this.lvWalletTransactions.TabIndex = 4;
            this.lvWalletTransactions.UseCompatibleStateImageBehavior = false;
            this.lvWalletTransactions.View = System.Windows.Forms.View.Details;
            // 
            // ilIcons
            // 
            this.ilIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilIcons.ImageStream")));
            this.ilIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.ilIcons.Images.SetKeyName(0, "arrow_up.png");
            this.ilIcons.Images.SetKeyName(1, "arrow_down.png");
            this.ilIcons.Images.SetKeyName(2, "16x16Transparant.png");
            // 
            // chDate
            // 
            this.chDate.Text = "Date";
            this.chDate.Width = 125;
            // 
            // chItem
            // 
            this.chItem.Text = "Item";
            this.chItem.Width = 169;
            // 
            // chPrice
            // 
            this.chPrice.Text = "Price";
            this.chPrice.Width = 77;
            // 
            // chQuantity
            // 
            this.chQuantity.Text = "Quantity";
            this.chQuantity.Width = 81;
            // 
            // CharacterWalletTransactionsList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvWalletTransactions);
            this.Controls.Add(this.noWalletTransactionsLabel);
            this.Name = "CharacterWalletTransactionsList";
            this.Size = new System.Drawing.Size(454, 434);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label noWalletTransactionsLabel;
        private System.Windows.Forms.ListView lvWalletTransactions;
        private System.Windows.Forms.ImageList ilIcons;
        private System.Windows.Forms.ColumnHeader chDate;
        private System.Windows.Forms.ColumnHeader chItem;
        private System.Windows.Forms.ColumnHeader chPrice;
        private System.Windows.Forms.ColumnHeader chQuantity;
    }
}
