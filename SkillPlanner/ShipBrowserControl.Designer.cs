namespace EVEMon.SkillPlanner
{
    partial class ShipBrowserControl
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
            this.scShipSelect = new EVEMon.SkillPlanner.PersistentSplitContainer();
            this.shipSelectControl = new EVEMon.SkillPlanner.ShipSelectControl();
            this.lvShipProperties = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.pnlShipDescription = new System.Windows.Forms.Panel();
            this.lblShipDescription = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblShipSkillC = new System.Windows.Forms.LinkLabel();
            this.lblShipSkillB = new System.Windows.Forms.LinkLabel();
            this.lblShipSkillA = new System.Windows.Forms.LinkLabel();
            this.btnShipSkillsAdd = new System.Windows.Forms.Button();
            this.lblShipTimeRequired = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pbShipImage = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblShipName = new System.Windows.Forms.Label();
            this.lblShipClass = new System.Windows.Forms.Label();
            this.ttShip = new System.Windows.Forms.ToolTip(this.components);
            this.scShipSelect.Panel1.SuspendLayout();
            this.scShipSelect.Panel2.SuspendLayout();
            this.scShipSelect.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.pnlShipDescription.SuspendLayout();
            this.panel4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbShipImage)).BeginInit();
            this.SuspendLayout();
            // 
            // scShipSelect
            // 
            this.scShipSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scShipSelect.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scShipSelect.Location = new System.Drawing.Point(0, 0);
            this.scShipSelect.Margin = new System.Windows.Forms.Padding(2);
            this.scShipSelect.Name = "scShipSelect";
            // 
            // scShipSelect.Panel1
            // 
            this.scShipSelect.Panel1.Controls.Add(this.shipSelectControl);
            // 
            // scShipSelect.Panel2
            // 
            this.scShipSelect.Panel2.Controls.Add(this.lvShipProperties);
            this.scShipSelect.Panel2.Controls.Add(this.panel2);
            this.scShipSelect.Panel2.Controls.Add(this.lblShipName);
            this.scShipSelect.Panel2.Controls.Add(this.lblShipClass);
            this.scShipSelect.RememberDistanceKey = null;
            this.scShipSelect.Size = new System.Drawing.Size(650, 413);
            this.scShipSelect.SplitterDistance = 163;
            this.scShipSelect.SplitterWidth = 5;
            this.scShipSelect.TabIndex = 1;
            // 
            // shipSelectControl
            // 
            this.shipSelectControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.shipSelectControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shipSelectControl.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.shipSelectControl.Location = new System.Drawing.Point(0, 0);
            this.shipSelectControl.Margin = new System.Windows.Forms.Padding(2);
            this.shipSelectControl.Name = "shipSelectControl";
            this.shipSelectControl.Plan = null;
            this.shipSelectControl.SelectedObject = null;
            this.shipSelectControl.SelectedObjects = null;
            this.shipSelectControl.Size = new System.Drawing.Size(163, 413);
            this.shipSelectControl.TabIndex = 0;
            this.shipSelectControl.SelectedObjectChanged += new System.EventHandler<System.EventArgs>(this.shipSelectControl_SelectedShipChanged);
            // 
            // lvShipProperties
            // 
            this.lvShipProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvShipProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvShipProperties.FullRowSelect = true;
            this.lvShipProperties.Location = new System.Drawing.Point(3, 37);
            this.lvShipProperties.Name = "lvShipProperties";
            this.lvShipProperties.Size = new System.Drawing.Size(214, 373);
            this.lvShipProperties.TabIndex = 3;
            this.lvShipProperties.UseCompatibleStateImageBehavior = false;
            this.lvShipProperties.View = System.Windows.Forms.View.Details;
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
            // panel2
            // 
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(223, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3);
            this.panel2.Size = new System.Drawing.Size(259, 413);
            this.panel2.TabIndex = 10;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.pnlShipDescription);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(3, 278);
            this.panel5.Margin = new System.Windows.Forms.Padding(2);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(253, 18);
            this.panel5.TabIndex = 13;
            // 
            // pnlShipDescription
            // 
            this.pnlShipDescription.AutoScroll = true;
            this.pnlShipDescription.Controls.Add(this.lblShipDescription);
            this.pnlShipDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlShipDescription.Location = new System.Drawing.Point(0, 0);
            this.pnlShipDescription.Name = "pnlShipDescription";
            this.pnlShipDescription.Padding = new System.Windows.Forms.Padding(2);
            this.pnlShipDescription.Size = new System.Drawing.Size(253, 18);
            this.pnlShipDescription.TabIndex = 8;
            this.pnlShipDescription.ClientSizeChanged += new System.EventHandler(this.pnlShipDescription_Changed);
            // 
            // lblShipDescription
            // 
            this.lblShipDescription.AutoSize = true;
            this.lblShipDescription.Location = new System.Drawing.Point(-2, 2);
            this.lblShipDescription.Name = "lblShipDescription";
            this.lblShipDescription.Size = new System.Drawing.Size(91, 13);
            this.lblShipDescription.TabIndex = 4;
            this.lblShipDescription.Text = "lblShipDescription";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.groupBox1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(3, 296);
            this.panel4.Margin = new System.Windows.Forms.Padding(2);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(253, 114);
            this.panel4.TabIndex = 12;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblShipSkillC);
            this.groupBox1.Controls.Add(this.lblShipSkillB);
            this.groupBox1.Controls.Add(this.lblShipSkillA);
            this.groupBox1.Controls.Add(this.btnShipSkillsAdd);
            this.groupBox1.Controls.Add(this.lblShipTimeRequired);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(253, 114);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Required Skills";
            // 
            // lblShipSkillC
            // 
            this.lblShipSkillC.AutoSize = true;
            this.lblShipSkillC.Location = new System.Drawing.Point(6, 43);
            this.lblShipSkillC.Name = "lblShipSkillC";
            this.lblShipSkillC.Size = new System.Drawing.Size(55, 13);
            this.lblShipSkillC.TabIndex = 7;
            this.lblShipSkillC.TabStop = true;
            this.lblShipSkillC.Text = "linkLabel2";
            this.lblShipSkillC.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblShipSkill_LinkClicked);
            this.lblShipSkillC.MouseHover += new System.EventHandler(this.lblShipSkill_MouseHover);
            // 
            // lblShipSkillB
            // 
            this.lblShipSkillB.AutoSize = true;
            this.lblShipSkillB.Location = new System.Drawing.Point(6, 30);
            this.lblShipSkillB.Name = "lblShipSkillB";
            this.lblShipSkillB.Size = new System.Drawing.Size(55, 13);
            this.lblShipSkillB.TabIndex = 6;
            this.lblShipSkillB.TabStop = true;
            this.lblShipSkillB.Text = "linkLabel1";
            this.lblShipSkillB.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblShipSkill_LinkClicked);
            this.lblShipSkillB.MouseHover += new System.EventHandler(this.lblShipSkill_MouseHover);
            // 
            // lblShipSkillA
            // 
            this.lblShipSkillA.AutoSize = true;
            this.lblShipSkillA.Location = new System.Drawing.Point(6, 16);
            this.lblShipSkillA.Name = "lblShipSkillA";
            this.lblShipSkillA.Size = new System.Drawing.Size(55, 13);
            this.lblShipSkillA.TabIndex = 5;
            this.lblShipSkillA.TabStop = true;
            this.lblShipSkillA.Text = "linkLabel1";
            this.lblShipSkillA.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblShipSkill_LinkClicked);
            this.lblShipSkillA.MouseHover += new System.EventHandler(this.lblShipSkill_MouseHover);
            // 
            // btnShipSkillsAdd
            // 
            this.btnShipSkillsAdd.Location = new System.Drawing.Point(124, 84);
            this.btnShipSkillsAdd.Name = "btnShipSkillsAdd";
            this.btnShipSkillsAdd.Size = new System.Drawing.Size(129, 23);
            this.btnShipSkillsAdd.TabIndex = 4;
            this.btnShipSkillsAdd.Text = "Add Skills to Plan";
            this.btnShipSkillsAdd.UseVisualStyleBackColor = true;
            this.btnShipSkillsAdd.Click += new System.EventHandler(this.btnShipSkillsAdd_Click);
            // 
            // lblShipTimeRequired
            // 
            this.lblShipTimeRequired.AutoSize = true;
            this.lblShipTimeRequired.Location = new System.Drawing.Point(6, 65);
            this.lblShipTimeRequired.Name = "lblShipTimeRequired";
            this.lblShipTimeRequired.Size = new System.Drawing.Size(79, 13);
            this.lblShipTimeRequired.TabIndex = 3;
            this.lblShipTimeRequired.Text = "Time Required:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.pbShipImage);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Margin = new System.Windows.Forms.Padding(2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(253, 275);
            this.panel3.TabIndex = 11;
            // 
            // pbShipImage
            // 
            this.pbShipImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbShipImage.Location = new System.Drawing.Point(0, 0);
            this.pbShipImage.Name = "pbShipImage";
            this.pbShipImage.Size = new System.Drawing.Size(253, 275);
            this.pbShipImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbShipImage.TabIndex = 0;
            this.pbShipImage.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 410);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(253, 0);
            this.panel1.TabIndex = 9;
            // 
            // lblShipName
            // 
            this.lblShipName.AutoSize = true;
            this.lblShipName.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblShipName.Location = new System.Drawing.Point(3, 16);
            this.lblShipName.Name = "lblShipName";
            this.lblShipName.Size = new System.Drawing.Size(88, 18);
            this.lblShipName.TabIndex = 2;
            this.lblShipName.Text = "Ship Name";
            // 
            // lblShipClass
            // 
            this.lblShipClass.AutoSize = true;
            this.lblShipClass.Location = new System.Drawing.Point(3, 3);
            this.lblShipClass.Name = "lblShipClass";
            this.lblShipClass.Size = new System.Drawing.Size(65, 13);
            this.lblShipClass.TabIndex = 1;
            this.lblShipClass.Text = "Ship > Class";
            // 
            // ShipBrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.scShipSelect);
            this.Name = "ShipBrowserControl";
            this.Size = new System.Drawing.Size(650, 413);
            this.scShipSelect.Panel1.ResumeLayout(false);
            this.scShipSelect.Panel2.ResumeLayout(false);
            this.scShipSelect.Panel2.PerformLayout();
            this.scShipSelect.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.pnlShipDescription.ResumeLayout(false);
            this.pnlShipDescription.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbShipImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private EVEMon.SkillPlanner.PersistentSplitContainer scShipSelect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnShipSkillsAdd;
        private System.Windows.Forms.Label lblShipTimeRequired;
        private System.Windows.Forms.Label lblShipName;
        private System.Windows.Forms.Label lblShipClass;
        private System.Windows.Forms.PictureBox pbShipImage;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel pnlShipDescription;
        private System.Windows.Forms.Label lblShipDescription;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.ListView lvShipProperties;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private ShipSelectControl shipSelectControl;
        private System.Windows.Forms.LinkLabel lblShipSkillA;
        private System.Windows.Forms.LinkLabel lblShipSkillC;
        private System.Windows.Forms.LinkLabel lblShipSkillB;
        private System.Windows.Forms.ToolTip ttShip;


    }
}
