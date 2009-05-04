namespace EVEMon.SkillPlanner
{
    partial class AttributesOptimizationSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttributesOptimizationSettingsForm));
            this.buttonWholePlan = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonRemappingPoints = new System.Windows.Forms.Button();
            this.buttonCharacter = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonWholePlan
            // 
            this.buttonWholePlan.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonWholePlan.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonWholePlan.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonWholePlan.Image = global::EVEMon.Properties.Resources.Blue_Glass_Arrow;
            this.buttonWholePlan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonWholePlan.Location = new System.Drawing.Point(12, 67);
            this.buttonWholePlan.Name = "buttonWholePlan";
            this.buttonWholePlan.Size = new System.Drawing.Size(472, 48);
            this.buttonWholePlan.TabIndex = 1;
            this.buttonWholePlan.Text = "Attributes that would be best for the first year of this plan";
            this.buttonWholePlan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonWholePlan.UseVisualStyleBackColor = true;
            this.buttonWholePlan.Click += new System.EventHandler(this.buttonWholePlan_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 172);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(281, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "* Assuming the learning skills would have been trained first";
            // 
            // buttonRemappingPoints
            // 
            this.buttonRemappingPoints.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemappingPoints.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonRemappingPoints.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRemappingPoints.Image = global::EVEMon.Properties.Resources.Blue_Glass_Arrow;
            this.buttonRemappingPoints.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonRemappingPoints.Location = new System.Drawing.Point(12, 13);
            this.buttonRemappingPoints.Name = "buttonRemappingPoints";
            this.buttonRemappingPoints.Size = new System.Drawing.Size(472, 48);
            this.buttonRemappingPoints.TabIndex = 0;
            this.buttonRemappingPoints.Text = "Use the remapping points I set up";
            this.buttonRemappingPoints.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonRemappingPoints.UseVisualStyleBackColor = true;
            this.buttonRemappingPoints.Click += new System.EventHandler(this.buttonRemappingPoints_Click);
            // 
            // buttonCharacter
            // 
            this.buttonCharacter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCharacter.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonCharacter.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCharacter.Image = global::EVEMon.Properties.Resources.Blue_Glass_Arrow;
            this.buttonCharacter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonCharacter.Location = new System.Drawing.Point(12, 121);
            this.buttonCharacter.Name = "buttonCharacter";
            this.buttonCharacter.Size = new System.Drawing.Size(472, 48);
            this.buttonCharacter.TabIndex = 2;
            this.buttonCharacter.Text = "Attributes that would have been best for what I have trained so far*";
            this.buttonCharacter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonCharacter.UseVisualStyleBackColor = true;
            this.buttonCharacter.Click += new System.EventHandler(this.buttonCharacter_Click);
            // 
            // AttributesOptimizationSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 200);
            this.Controls.Add(this.buttonRemappingPoints);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonWholePlan);
            this.Controls.Add(this.buttonCharacter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AttributesOptimizationSettingsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Attributes optimization settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCharacter;
        private System.Windows.Forms.Button buttonWholePlan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonRemappingPoints;

    }
}
