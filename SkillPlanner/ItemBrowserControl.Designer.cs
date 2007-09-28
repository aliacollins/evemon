namespace EVEMon.SkillPlanner
{
    partial class ItemBrowserControl
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
            this.gbAttributes = new System.Windows.Forms.GroupBox();
            this.lvItemProperties = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.itemSelectControl = new EVEMon.SkillPlanner.ItemSelectControl();
            this.ttItem = new System.Windows.Forms.ToolTip(this.components);
            this.scDetails.Panel1.SuspendLayout();
            this.scDetails.SuspendLayout();
            this.scObjectBrowser.Panel1.SuspendLayout();
            this.scObjectBrowser.Panel2.SuspendLayout();
            this.scObjectBrowser.SuspendLayout();
            this.pnlDetails.SuspendLayout();
            this.gbAttributes.SuspendLayout();
            this.SuspendLayout();
            // 
            // scDetails
            // 
            // 
            // scDetails.Panel1
            // 
            this.scDetails.Panel1.Controls.Add(this.gbAttributes);
            // 
            // scObjectBrowser
            // 
            // 
            // scObjectBrowser.Panel1
            // 
            this.scObjectBrowser.Panel1.Controls.Add(this.itemSelectControl);
            // 
            // gbAttributes
            // 
            this.gbAttributes.Controls.Add(this.lvItemProperties);
            this.gbAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbAttributes.Location = new System.Drawing.Point(0, 0);
            this.gbAttributes.Name = "gbAttributes";
            this.gbAttributes.Size = new System.Drawing.Size(238, 343);
            this.gbAttributes.TabIndex = 15;
            this.gbAttributes.TabStop = false;
            this.gbAttributes.Text = "Attributes";
            // 
            // lvItemProperties
            // 
            this.lvItemProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvItemProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvItemProperties.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lvItemProperties.FullRowSelect = true;
            this.lvItemProperties.Location = new System.Drawing.Point(3, 16);
            this.lvItemProperties.Name = "lvItemProperties";
            this.lvItemProperties.Size = new System.Drawing.Size(232, 324);
            this.lvItemProperties.TabIndex = 8;
            this.lvItemProperties.UseCompatibleStateImageBehavior = false;
            this.lvItemProperties.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Attribute";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 140;
            // 
            // itemSelectControl
            // 
            this.itemSelectControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.itemSelectControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemSelectControl.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.itemSelectControl.Location = new System.Drawing.Point(0, 0);
            this.itemSelectControl.Margin = new System.Windows.Forms.Padding(2);
            this.itemSelectControl.Name = "itemSelectControl";
            this.itemSelectControl.Plan = null;
            this.itemSelectControl.SelectedObject = null;
            this.itemSelectControl.SelectedObjects = null;
            this.itemSelectControl.Size = new System.Drawing.Size(163, 413);
            this.itemSelectControl.TabIndex = 0;
            // 
            // ItemBrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ItemBrowserControl";
            this.scDetails.Panel1.ResumeLayout(false);
            this.scDetails.ResumeLayout(false);
            this.scObjectBrowser.Panel1.ResumeLayout(false);
            this.scObjectBrowser.Panel2.ResumeLayout(false);
            this.scObjectBrowser.Panel2.PerformLayout();
            this.scObjectBrowser.ResumeLayout(false);
            this.pnlDetails.ResumeLayout(false);
            this.gbAttributes.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvItemProperties;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private ItemSelectControl itemSelectControl;
        private System.Windows.Forms.ToolTip ttItem;
        private System.Windows.Forms.GroupBox gbAttributes;
    }
}
