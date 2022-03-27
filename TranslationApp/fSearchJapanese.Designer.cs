
namespace TranslationApp
{
    partial class fSearchJapanese
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
            this.label2 = new System.Windows.Forms.Label();
            this.tbJapaneseSearch = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbEntries = new System.Windows.Forms.ListBox();
            this.cbWholeWordSearch = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Japanese Text";
            // 
            // tbJapaneseSearch
            // 
            this.tbJapaneseSearch.Location = new System.Drawing.Point(15, 88);
            this.tbJapaneseSearch.Name = "tbJapaneseSearch";
            this.tbJapaneseSearch.Size = new System.Drawing.Size(342, 20);
            this.tbJapaneseSearch.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbEntries);
            this.groupBox2.Location = new System.Drawing.Point(15, 126);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(441, 293);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Archives and files";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(322, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "- Find locations of specific japanese text that needs to be extracted";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(174, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "- Validate if XML file is already there";
            // 
            // lbEntries
            // 
            this.lbEntries.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lbEntries.FormattingEnabled = true;
            this.lbEntries.Location = new System.Drawing.Point(6, 29);
            this.lbEntries.Name = "lbEntries";
            this.lbEntries.Size = new System.Drawing.Size(336, 264);
            this.lbEntries.TabIndex = 1;
            // 
            // cbWholeWordSearch
            // 
            this.cbWholeWordSearch.FormattingEnabled = true;
            this.cbWholeWordSearch.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.cbWholeWordSearch.Location = new System.Drawing.Point(363, 87);
            this.cbWholeWordSearch.Name = "cbWholeWordSearch";
            this.cbWholeWordSearch.Size = new System.Drawing.Size(93, 21);
            this.cbWholeWordSearch.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(363, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 16);
            this.label4.TabIndex = 9;
            this.label4.Text = "Match whole word";
            // 
            // fSearchJapanese
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbWholeWordSearch);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.tbJapaneseSearch);
            this.Controls.Add(this.label2);
            this.Name = "fSearchJapanese";
            this.Text = "Search files for Japanese";
            this.Load += new System.EventHandler(this.fSearchJapanese_Load);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbJapaneseSearch;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lbEntries;
        private System.Windows.Forms.ComboBox cbWholeWordSearch;
        private System.Windows.Forms.Label label4;
    }
}