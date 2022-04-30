
namespace TranslationApp
{
    partial class fMassReplace
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.datagridMassReplace = new System.Windows.Forms.DataGridView();
            this.bSearchMass = new System.Windows.Forms.Button();
            this.cbExactMatch = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.datagridMassReplace)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Japanese to search";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(15, 46);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(250, 53);
            this.textBox1.TabIndex = 1;
            // 
            // datagridMassReplace
            // 
            this.datagridMassReplace.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagridMassReplace.Location = new System.Drawing.Point(15, 117);
            this.datagridMassReplace.Name = "datagridMassReplace";
            this.datagridMassReplace.Size = new System.Drawing.Size(705, 150);
            this.datagridMassReplace.TabIndex = 2;
            // 
            // bSearchMass
            // 
            this.bSearchMass.Location = new System.Drawing.Point(271, 76);
            this.bSearchMass.Name = "bSearchMass";
            this.bSearchMass.Size = new System.Drawing.Size(75, 23);
            this.bSearchMass.TabIndex = 3;
            this.bSearchMass.Text = "Search";
            this.bSearchMass.UseVisualStyleBackColor = true;
            this.bSearchMass.Click += new System.EventHandler(this.bSearchMass_Click);
            // 
            // cbExactMatch
            // 
            this.cbExactMatch.FormattingEnabled = true;
            this.cbExactMatch.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.cbExactMatch.Location = new System.Drawing.Point(283, 46);
            this.cbExactMatch.Name = "cbExactMatch";
            this.cbExactMatch.Size = new System.Drawing.Size(63, 21);
            this.cbExactMatch.TabIndex = 4;
            // 
            // fMassReplace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cbExactMatch);
            this.Controls.Add(this.bSearchMass);
            this.Controls.Add(this.datagridMassReplace);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Name = "fMassReplace";
            this.Text = "Module for Mass replace";
            this.Load += new System.EventHandler(this.fMassReplace_Load);
            ((System.ComponentModel.ISupportInitialize)(this.datagridMassReplace)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridView datagridMassReplace;
        private System.Windows.Forms.Button bSearchMass;
        private System.Windows.Forms.ComboBox cbExactMatch;
    }
}