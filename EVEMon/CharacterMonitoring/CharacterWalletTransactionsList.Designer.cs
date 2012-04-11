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
            this.noWalletTransactionsLabel = new System.Windows.Forms.Label();
            this.lvWalletTransactions = new System.Windows.Forms.ListView();
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
            this.lvWalletTransactions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvWalletTransactions.FullRowSelect = true;
            this.lvWalletTransactions.HideSelection = false;
            this.lvWalletTransactions.Location = new System.Drawing.Point(0, 0);
            this.lvWalletTransactions.Name = "lvWalletTransactions";
            this.lvWalletTransactions.Size = new System.Drawing.Size(454, 434);
            this.lvWalletTransactions.TabIndex = 4;
            this.lvWalletTransactions.UseCompatibleStateImageBehavior = false;
            this.lvWalletTransactions.View = System.Windows.Forms.View.Details;
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
    }
}
