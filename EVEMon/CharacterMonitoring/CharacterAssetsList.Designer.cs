namespace EVEMon.CharacterMonitoring
{
    partial class CharacterAssetsList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharacterAssetsList));
            this.noAssetsLabel = new System.Windows.Forms.Label();
            this.lvAssets = new System.Windows.Forms.ListView();
            this.chItem = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chQuantity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chVolume = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chGroup = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chCategory = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ilIcons = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // noAssetsLabel
            // 
            this.noAssetsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.noAssetsLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.noAssetsLabel.Location = new System.Drawing.Point(0, 0);
            this.noAssetsLabel.Name = "noAssetsLabel";
            this.noAssetsLabel.Size = new System.Drawing.Size(454, 434);
            this.noAssetsLabel.TabIndex = 3;
            this.noAssetsLabel.Text = "No assets are available.";
            this.noAssetsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lvAssets
            // 
            this.lvAssets.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvAssets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chItem,
            this.chQuantity,
            this.chVolume,
            this.chGroup,
            this.chCategory});
            this.lvAssets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvAssets.FullRowSelect = true;
            this.lvAssets.HideSelection = false;
            this.lvAssets.Location = new System.Drawing.Point(0, 0);
            this.lvAssets.Name = "lvAssets";
            this.lvAssets.Size = new System.Drawing.Size(454, 434);
            this.lvAssets.SmallImageList = this.ilIcons;
            this.lvAssets.TabIndex = 3;
            this.lvAssets.UseCompatibleStateImageBehavior = false;
            this.lvAssets.View = System.Windows.Forms.View.Details;
            // 
            // chItem
            // 
            this.chItem.Text = "Item";
            this.chItem.Width = 166;
            // 
            // chQuantity
            // 
            this.chQuantity.Text = "Quantity";
            this.chQuantity.Width = 72;
            // 
            // chVolume
            // 
            this.chVolume.Text = "Volume";
            this.chVolume.Width = 74;
            // 
            // chGroup
            // 
            this.chGroup.Text = "Group";
            this.chGroup.Width = 80;
            // 
            // chCategory
            // 
            this.chCategory.Text = "Category";
            // 
            // ilIcons
            // 
            this.ilIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilIcons.ImageStream")));
            this.ilIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.ilIcons.Images.SetKeyName(0, "arrow_up.png");
            this.ilIcons.Images.SetKeyName(1, "arrow_down.png");
            this.ilIcons.Images.SetKeyName(2, "16x16Transparant.png");
            // 
            // CharacterAssetsList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvAssets);
            this.Controls.Add(this.noAssetsLabel);
            this.Name = "CharacterAssetsList";
            this.Size = new System.Drawing.Size(454, 434);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label noAssetsLabel;
        private System.Windows.Forms.ListView lvAssets;
        private System.Windows.Forms.ColumnHeader chItem;
        private System.Windows.Forms.ColumnHeader chQuantity;
        private System.Windows.Forms.ColumnHeader chVolume;
        private System.Windows.Forms.ColumnHeader chGroup;
        private System.Windows.Forms.ColumnHeader chCategory;
        private System.Windows.Forms.ImageList ilIcons;
    }
}
