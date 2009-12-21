namespace EVEMon.ImpGroups
{
    partial class ImpGroups
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
            this.components = new System.ComponentModel.Container();
            this.JumpCloneTxt = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lbJumpClone = new System.Windows.Forms.ComboBox();
            this.lblJumpClone = new System.Windows.Forms.Label();
            this.pnlImplants = new System.Windows.Forms.GroupBox();
            this.btnSlot10 = new System.Windows.Forms.Button();
            this.btnSlot9 = new System.Windows.Forms.Button();
            this.btnSlot8 = new System.Windows.Forms.Button();
            this.btnSlot7 = new System.Windows.Forms.Button();
            this.btnSlot6 = new System.Windows.Forms.Button();
            this.btnSlot5 = new System.Windows.Forms.Button();
            this.btnSlot4 = new System.Windows.Forms.Button();
            this.btnSlot3 = new System.Windows.Forms.Button();
            this.btnSlot2 = new System.Windows.Forms.Button();
            this.btnSlot1 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblImplant2 = new System.Windows.Forms.Label();
            this.lblImplant1 = new System.Windows.Forms.Label();
            this.txtImplant10 = new System.Windows.Forms.TextBox();
            this.txtImplant9 = new System.Windows.Forms.TextBox();
            this.txtImplant8 = new System.Windows.Forms.TextBox();
            this.txtImplant7 = new System.Windows.Forms.TextBox();
            this.txtImplant6 = new System.Windows.Forms.TextBox();
            this.txtImplant5 = new System.Windows.Forms.TextBox();
            this.txtImplant4 = new System.Windows.Forms.TextBox();
            this.txtImplant3 = new System.Windows.Forms.TextBox();
            this.txtImplant2 = new System.Windows.Forms.TextBox();
            this.txtImplant1 = new System.Windows.Forms.TextBox();
            this.btnSwapWithCurrent = new System.Windows.Forms.Button();
            this.btnDeleteCurrent = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.flowLayoutPanel1.SuspendLayout();
            this.pnlImplants.SuspendLayout();
            this.SuspendLayout();
            // 
            // JumpCloneTxt
            // 
            this.JumpCloneTxt.AutoSize = true;
            this.JumpCloneTxt.Location = new System.Drawing.Point(12, 9);
            this.JumpCloneTxt.Name = "JumpCloneTxt";
            this.JumpCloneTxt.Size = new System.Drawing.Size(219, 26);
            this.JumpCloneTxt.TabIndex = 1;
            this.JumpCloneTxt.Text = "{0} has the skill for {1} Jump Clones\r\n (plus 1 for the implants in your active b" +
                "ody)";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnOK);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(198, 373);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0, 7, 0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(162, 29);
            this.flowLayoutPanel1.TabIndex = 4;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(84, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(3, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lbJumpClone
            // 
            this.lbJumpClone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lbJumpClone.FormattingEnabled = true;
            this.lbJumpClone.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbJumpClone.Location = new System.Drawing.Point(85, 44);
            this.lbJumpClone.Name = "lbJumpClone";
            this.lbJumpClone.Size = new System.Drawing.Size(146, 21);
            this.lbJumpClone.TabIndex = 5;
            this.lbJumpClone.Tag = "Jump Clone";
            this.lbJumpClone.SelectedIndexChanged += new System.EventHandler(this.lbJumpClone_SelectedIndexChanged);
            // 
            // lblJumpClone
            // 
            this.lblJumpClone.AutoSize = true;
            this.lblJumpClone.Location = new System.Drawing.Point(12, 47);
            this.lblJumpClone.Name = "lblJumpClone";
            this.lblJumpClone.Size = new System.Drawing.Size(67, 13);
            this.lblJumpClone.TabIndex = 6;
            this.lblJumpClone.Text = "Jump Clones";
            // 
            // pnlImplants
            // 
            this.pnlImplants.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlImplants.BackColor = System.Drawing.SystemColors.Control;
            this.pnlImplants.Controls.Add(this.btnSlot10);
            this.pnlImplants.Controls.Add(this.btnSlot9);
            this.pnlImplants.Controls.Add(this.btnSlot8);
            this.pnlImplants.Controls.Add(this.btnSlot7);
            this.pnlImplants.Controls.Add(this.btnSlot6);
            this.pnlImplants.Controls.Add(this.btnSlot5);
            this.pnlImplants.Controls.Add(this.btnSlot4);
            this.pnlImplants.Controls.Add(this.btnSlot3);
            this.pnlImplants.Controls.Add(this.btnSlot2);
            this.pnlImplants.Controls.Add(this.btnSlot1);
            this.pnlImplants.Controls.Add(this.label8);
            this.pnlImplants.Controls.Add(this.label7);
            this.pnlImplants.Controls.Add(this.label6);
            this.pnlImplants.Controls.Add(this.label5);
            this.pnlImplants.Controls.Add(this.label4);
            this.pnlImplants.Controls.Add(this.label3);
            this.pnlImplants.Controls.Add(this.label2);
            this.pnlImplants.Controls.Add(this.label1);
            this.pnlImplants.Controls.Add(this.lblImplant2);
            this.pnlImplants.Controls.Add(this.lblImplant1);
            this.pnlImplants.Controls.Add(this.txtImplant10);
            this.pnlImplants.Controls.Add(this.txtImplant9);
            this.pnlImplants.Controls.Add(this.txtImplant8);
            this.pnlImplants.Controls.Add(this.txtImplant7);
            this.pnlImplants.Controls.Add(this.txtImplant6);
            this.pnlImplants.Controls.Add(this.txtImplant5);
            this.pnlImplants.Controls.Add(this.txtImplant4);
            this.pnlImplants.Controls.Add(this.txtImplant3);
            this.pnlImplants.Controls.Add(this.txtImplant2);
            this.pnlImplants.Controls.Add(this.txtImplant1);
            this.pnlImplants.Location = new System.Drawing.Point(15, 71);
            this.pnlImplants.Name = "pnlImplants";
            this.pnlImplants.Padding = new System.Windows.Forms.Padding(4);
            this.pnlImplants.Size = new System.Drawing.Size(342, 292);
            this.pnlImplants.TabIndex = 7;
            this.pnlImplants.TabStop = false;
            this.pnlImplants.Text = "Implants";
            // 
            // btnSlot10
            // 
            this.btnSlot10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSlot10.Location = new System.Drawing.Point(301, 264);
            this.btnSlot10.Name = "btnSlot10";
            this.btnSlot10.Size = new System.Drawing.Size(34, 21);
            this.btnSlot10.TabIndex = 29;
            this.btnSlot10.Text = "...";
            this.btnSlot10.UseVisualStyleBackColor = true;
            this.btnSlot10.Click += new System.EventHandler(this.Get_Implant);
            // 
            // btnSlot9
            // 
            this.btnSlot9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSlot9.Location = new System.Drawing.Point(301, 237);
            this.btnSlot9.Name = "btnSlot9";
            this.btnSlot9.Size = new System.Drawing.Size(34, 21);
            this.btnSlot9.TabIndex = 28;
            this.btnSlot9.Text = "...";
            this.btnSlot9.UseVisualStyleBackColor = true;
            this.btnSlot9.Click += new System.EventHandler(this.Get_Implant);
            // 
            // btnSlot8
            // 
            this.btnSlot8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSlot8.Location = new System.Drawing.Point(301, 210);
            this.btnSlot8.Name = "btnSlot8";
            this.btnSlot8.Size = new System.Drawing.Size(34, 21);
            this.btnSlot8.TabIndex = 27;
            this.btnSlot8.Text = "...";
            this.btnSlot8.UseVisualStyleBackColor = true;
            this.btnSlot8.Click += new System.EventHandler(this.Get_Implant);
            // 
            // btnSlot7
            // 
            this.btnSlot7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSlot7.Location = new System.Drawing.Point(301, 183);
            this.btnSlot7.Name = "btnSlot7";
            this.btnSlot7.Size = new System.Drawing.Size(34, 21);
            this.btnSlot7.TabIndex = 26;
            this.btnSlot7.Text = "...";
            this.btnSlot7.UseVisualStyleBackColor = true;
            this.btnSlot7.Click += new System.EventHandler(this.Get_Implant);
            // 
            // btnSlot6
            // 
            this.btnSlot6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSlot6.Location = new System.Drawing.Point(301, 156);
            this.btnSlot6.Name = "btnSlot6";
            this.btnSlot6.Size = new System.Drawing.Size(34, 21);
            this.btnSlot6.TabIndex = 25;
            this.btnSlot6.Text = "...";
            this.btnSlot6.UseVisualStyleBackColor = true;
            this.btnSlot6.Click += new System.EventHandler(this.Get_Implant);
            // 
            // btnSlot5
            // 
            this.btnSlot5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSlot5.Location = new System.Drawing.Point(301, 129);
            this.btnSlot5.Name = "btnSlot5";
            this.btnSlot5.Size = new System.Drawing.Size(34, 21);
            this.btnSlot5.TabIndex = 24;
            this.btnSlot5.Text = "...";
            this.btnSlot5.UseVisualStyleBackColor = true;
            this.btnSlot5.Click += new System.EventHandler(this.Get_Implant);
            // 
            // btnSlot4
            // 
            this.btnSlot4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSlot4.Location = new System.Drawing.Point(301, 102);
            this.btnSlot4.Name = "btnSlot4";
            this.btnSlot4.Size = new System.Drawing.Size(34, 21);
            this.btnSlot4.TabIndex = 23;
            this.btnSlot4.Text = "...";
            this.btnSlot4.UseVisualStyleBackColor = true;
            this.btnSlot4.Click += new System.EventHandler(this.Get_Implant);
            // 
            // btnSlot3
            // 
            this.btnSlot3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSlot3.Location = new System.Drawing.Point(301, 75);
            this.btnSlot3.Name = "btnSlot3";
            this.btnSlot3.Size = new System.Drawing.Size(34, 21);
            this.btnSlot3.TabIndex = 22;
            this.btnSlot3.Text = "...";
            this.btnSlot3.UseVisualStyleBackColor = true;
            this.btnSlot3.Click += new System.EventHandler(this.Get_Implant);
            // 
            // btnSlot2
            // 
            this.btnSlot2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSlot2.Location = new System.Drawing.Point(301, 48);
            this.btnSlot2.Name = "btnSlot2";
            this.btnSlot2.Size = new System.Drawing.Size(34, 21);
            this.btnSlot2.TabIndex = 21;
            this.btnSlot2.Text = "...";
            this.btnSlot2.UseVisualStyleBackColor = true;
            this.btnSlot2.Click += new System.EventHandler(this.Get_Implant);
            // 
            // btnSlot1
            // 
            this.btnSlot1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSlot1.Location = new System.Drawing.Point(301, 21);
            this.btnSlot1.Name = "btnSlot1";
            this.btnSlot1.Size = new System.Drawing.Size(34, 21);
            this.btnSlot1.TabIndex = 20;
            this.btnSlot1.Text = "...";
            this.btnSlot1.UseVisualStyleBackColor = true;
            this.btnSlot1.Click += new System.EventHandler(this.Get_Implant);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1, 267);
            this.label8.Name = "label8";
            this.label8.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label8.Size = new System.Drawing.Size(43, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Slot 10";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 240);
            this.label7.Name = "label7";
            this.label7.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label7.Size = new System.Drawing.Size(37, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Slot 9";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 213);
            this.label6.Name = "label6";
            this.label6.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Slot 8";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 186);
            this.label5.Name = "label5";
            this.label5.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Slot 7";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 159);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Slot 6";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 132);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Slot 5";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 105);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Slot 4";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 78);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Slot 3";
            // 
            // lblImplant2
            // 
            this.lblImplant2.AutoSize = true;
            this.lblImplant2.Location = new System.Drawing.Point(7, 51);
            this.lblImplant2.Name = "lblImplant2";
            this.lblImplant2.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.lblImplant2.Size = new System.Drawing.Size(37, 13);
            this.lblImplant2.TabIndex = 11;
            this.lblImplant2.Text = "Slot 2";
            // 
            // lblImplant1
            // 
            this.lblImplant1.AutoSize = true;
            this.lblImplant1.Location = new System.Drawing.Point(7, 25);
            this.lblImplant1.Name = "lblImplant1";
            this.lblImplant1.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.lblImplant1.Size = new System.Drawing.Size(37, 13);
            this.lblImplant1.TabIndex = 10;
            this.lblImplant1.Text = "Slot 1";
            // 
            // txtImplant10
            // 
            this.txtImplant10.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtImplant10.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtImplant10.Location = new System.Drawing.Point(50, 264);
            this.txtImplant10.Name = "txtImplant10";
            this.txtImplant10.ReadOnly = true;
            this.txtImplant10.Size = new System.Drawing.Size(245, 21);
            this.txtImplant10.TabIndex = 9;
            // 
            // txtImplant9
            // 
            this.txtImplant9.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtImplant9.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtImplant9.Location = new System.Drawing.Point(50, 237);
            this.txtImplant9.Name = "txtImplant9";
            this.txtImplant9.ReadOnly = true;
            this.txtImplant9.Size = new System.Drawing.Size(245, 21);
            this.txtImplant9.TabIndex = 8;
            // 
            // txtImplant8
            // 
            this.txtImplant8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtImplant8.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtImplant8.Location = new System.Drawing.Point(50, 210);
            this.txtImplant8.Name = "txtImplant8";
            this.txtImplant8.ReadOnly = true;
            this.txtImplant8.Size = new System.Drawing.Size(245, 21);
            this.txtImplant8.TabIndex = 7;
            // 
            // txtImplant7
            // 
            this.txtImplant7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtImplant7.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtImplant7.Location = new System.Drawing.Point(50, 183);
            this.txtImplant7.Name = "txtImplant7";
            this.txtImplant7.ReadOnly = true;
            this.txtImplant7.Size = new System.Drawing.Size(245, 21);
            this.txtImplant7.TabIndex = 6;
            // 
            // txtImplant6
            // 
            this.txtImplant6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtImplant6.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtImplant6.Location = new System.Drawing.Point(50, 156);
            this.txtImplant6.Name = "txtImplant6";
            this.txtImplant6.ReadOnly = true;
            this.txtImplant6.Size = new System.Drawing.Size(245, 21);
            this.txtImplant6.TabIndex = 5;
            // 
            // txtImplant5
            // 
            this.txtImplant5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtImplant5.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtImplant5.Location = new System.Drawing.Point(50, 129);
            this.txtImplant5.Name = "txtImplant5";
            this.txtImplant5.ReadOnly = true;
            this.txtImplant5.Size = new System.Drawing.Size(245, 21);
            this.txtImplant5.TabIndex = 4;
            // 
            // txtImplant4
            // 
            this.txtImplant4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtImplant4.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtImplant4.Location = new System.Drawing.Point(50, 102);
            this.txtImplant4.Name = "txtImplant4";
            this.txtImplant4.ReadOnly = true;
            this.txtImplant4.Size = new System.Drawing.Size(245, 21);
            this.txtImplant4.TabIndex = 3;
            // 
            // txtImplant3
            // 
            this.txtImplant3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtImplant3.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtImplant3.Location = new System.Drawing.Point(50, 75);
            this.txtImplant3.Name = "txtImplant3";
            this.txtImplant3.ReadOnly = true;
            this.txtImplant3.Size = new System.Drawing.Size(245, 21);
            this.txtImplant3.TabIndex = 2;
            // 
            // txtImplant2
            // 
            this.txtImplant2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtImplant2.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtImplant2.Location = new System.Drawing.Point(50, 48);
            this.txtImplant2.Name = "txtImplant2";
            this.txtImplant2.ReadOnly = true;
            this.txtImplant2.Size = new System.Drawing.Size(245, 21);
            this.txtImplant2.TabIndex = 1;
            // 
            // txtImplant1
            // 
            this.txtImplant1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtImplant1.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtImplant1.Location = new System.Drawing.Point(50, 21);
            this.txtImplant1.Name = "txtImplant1";
            this.txtImplant1.ReadOnly = true;
            this.txtImplant1.Size = new System.Drawing.Size(245, 21);
            this.txtImplant1.TabIndex = 0;
            // 
            // btnSwapWithCurrent
            // 
            this.btnSwapWithCurrent.Enabled = false;
            this.btnSwapWithCurrent.Location = new System.Drawing.Point(237, 44);
            this.btnSwapWithCurrent.Name = "btnSwapWithCurrent";
            this.btnSwapWithCurrent.Size = new System.Drawing.Size(120, 21);
            this.btnSwapWithCurrent.TabIndex = 8;
            this.btnSwapWithCurrent.Tag = "Weeee, jumpy jumpy";
            this.btnSwapWithCurrent.Text = "Swap with \"Current\"";
            this.btnSwapWithCurrent.UseVisualStyleBackColor = true;
            this.btnSwapWithCurrent.Click += new System.EventHandler(this.btnSwapWithCurrent_Click);
            this.btnSwapWithCurrent.MouseHover += new System.EventHandler(this.btnSwapWithCurrent_MouseHover);
            // 
            // btnDeleteCurrent
            // 
            this.btnDeleteCurrent.Location = new System.Drawing.Point(237, 17);
            this.btnDeleteCurrent.Name = "btnDeleteCurrent";
            this.btnDeleteCurrent.Size = new System.Drawing.Size(120, 21);
            this.btnDeleteCurrent.TabIndex = 9;
            this.btnDeleteCurrent.Tag = "Arrggg, podded again!";
            this.btnDeleteCurrent.Text = "Delete \"Current\"";
            this.btnDeleteCurrent.UseVisualStyleBackColor = true;
            this.btnDeleteCurrent.Click += new System.EventHandler(this.btnDeleteCurrent_Click);
            this.btnDeleteCurrent.MouseHover += new System.EventHandler(this.btnDeleteCurrent_MouseHover);
            // 
            // toolTip1
            // 
            this.toolTip1.Tag = "Arrrggg, podded again";
            // 
            // ImpGroups
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 411);
            this.Controls.Add(this.btnDeleteCurrent);
            this.Controls.Add(this.btnSwapWithCurrent);
            this.Controls.Add(this.pnlImplants);
            this.Controls.Add(this.lblJumpClone);
            this.Controls.Add(this.lbJumpClone);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.JumpCloneTxt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ImpGroups";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "Jump Clone";
            this.Text = "Jump Clones";
            this.Load += new System.EventHandler(this.ImpGroups_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.pnlImplants.ResumeLayout(false);
            this.pnlImplants.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label JumpCloneTxt;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox lbJumpClone;
        private System.Windows.Forms.Label lblJumpClone;
        private System.Windows.Forms.GroupBox pnlImplants;
        private System.Windows.Forms.Label lblImplant1;
        private System.Windows.Forms.Label lblImplant2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSlot10;
        private System.Windows.Forms.Button btnSlot9;
        private System.Windows.Forms.Button btnSlot8;
        private System.Windows.Forms.Button btnSlot7;
        private System.Windows.Forms.Button btnSlot6;
        private System.Windows.Forms.Button btnSlot5;
        private System.Windows.Forms.Button btnSlot4;
        private System.Windows.Forms.Button btnSlot3;
        private System.Windows.Forms.Button btnSlot2;
        private System.Windows.Forms.Button btnSlot1;
        private System.Windows.Forms.TextBox txtImplant10;
        private System.Windows.Forms.TextBox txtImplant9;
        private System.Windows.Forms.TextBox txtImplant8;
        private System.Windows.Forms.TextBox txtImplant7;
        private System.Windows.Forms.TextBox txtImplant6;
        private System.Windows.Forms.TextBox txtImplant5;
        private System.Windows.Forms.TextBox txtImplant4;
        private System.Windows.Forms.TextBox txtImplant3;
        private System.Windows.Forms.TextBox txtImplant2;
        private System.Windows.Forms.TextBox txtImplant1;
        private System.Windows.Forms.Button btnSwapWithCurrent;
        private System.Windows.Forms.Button btnDeleteCurrent;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
