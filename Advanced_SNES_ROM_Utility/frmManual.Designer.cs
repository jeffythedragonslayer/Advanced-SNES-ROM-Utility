﻿namespace Advanced_SNES_ROM_Utility
{
    partial class frmManual
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmManual));
            this.labelAboutFunctions = new System.Windows.Forms.Label();
            this.buttonManualClose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelAboutFunctions
            // 
            this.labelAboutFunctions.AutoSize = true;
            this.labelAboutFunctions.Location = new System.Drawing.Point(10, 21);
            this.labelAboutFunctions.Name = "labelAboutFunctions";
            this.labelAboutFunctions.Size = new System.Drawing.Size(389, 533);
            this.labelAboutFunctions.TabIndex = 7;
            this.labelAboutFunctions.Text = resources.GetString("labelAboutFunctions.Text");
            // 
            // buttonManualClose
            // 
            this.buttonManualClose.Location = new System.Drawing.Point(12, 575);
            this.buttonManualClose.Name = "buttonManualClose";
            this.buttonManualClose.Size = new System.Drawing.Size(405, 23);
            this.buttonManualClose.TabIndex = 8;
            this.buttonManualClose.Text = "Close";
            this.buttonManualClose.UseVisualStyleBackColor = true;
            this.buttonManualClose.Click += new System.EventHandler(this.buttonManualClose_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelAboutFunctions);
            this.groupBox1.Location = new System.Drawing.Point(12, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(405, 563);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Manual";
            // 
            // Form5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 605);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonManualClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form5";
            this.Text = "Manual of the Advanced SNES ROM Utility v1.0";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelAboutFunctions;
        private System.Windows.Forms.Button buttonManualClose;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}