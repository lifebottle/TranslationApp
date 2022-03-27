
namespace TranslationApp
{
    partial class fHexToJapanese
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
            this.tbHextoConvert = new System.Windows.Forms.TextBox();
            this.tbConvertedJapanese = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbGameList = new System.Windows.Forms.ComboBox();
            this.bConvert = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbStartOffset = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.bDumpText = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbFileOrigin = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Hex Values";
            // 
            // tbHextoConvert
            // 
            this.tbHextoConvert.Location = new System.Drawing.Point(35, 89);
            this.tbHextoConvert.Name = "tbHextoConvert";
            this.tbHextoConvert.Size = new System.Drawing.Size(407, 20);
            this.tbHextoConvert.TabIndex = 1;
            this.tbHextoConvert.TextChanged += new System.EventHandler(this.tbHextoConvert_TextChanged);
            // 
            // tbConvertedJapanese
            // 
            this.tbConvertedJapanese.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbConvertedJapanese.Location = new System.Drawing.Point(35, 169);
            this.tbConvertedJapanese.Multiline = true;
            this.tbConvertedJapanese.Name = "tbConvertedJapanese";
            this.tbConvertedJapanese.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbConvertedJapanese.Size = new System.Drawing.Size(407, 269);
            this.tbConvertedJapanese.TabIndex = 3;
            this.tbConvertedJapanese.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Japanese";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Game";
            // 
            // cbGameList
            // 
            this.cbGameList.FormattingEnabled = true;
            this.cbGameList.Items.AddRange(new object[] {
            "TOR",
            "TOPX"});
            this.cbGameList.Location = new System.Drawing.Point(89, 17);
            this.cbGameList.Name = "cbGameList";
            this.cbGameList.Size = new System.Drawing.Size(121, 21);
            this.cbGameList.TabIndex = 5;
            // 
            // bConvert
            // 
            this.bConvert.Location = new System.Drawing.Point(367, 130);
            this.bConvert.Name = "bConvert";
            this.bConvert.Size = new System.Drawing.Size(75, 23);
            this.bConvert.TabIndex = 6;
            this.bConvert.Text = "Convert";
            this.bConvert.UseVisualStyleBackColor = true;
            this.bConvert.Click += new System.EventHandler(this.bConvert_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.tbFileOrigin);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.bDumpText);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tbStartOffset);
            this.groupBox1.Location = new System.Drawing.Point(466, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(310, 107);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Dump text file";
            // 
            // tbStartOffset
            // 
            this.tbStartOffset.Location = new System.Drawing.Point(92, 19);
            this.tbStartOffset.Name = "tbStartOffset";
            this.tbStartOffset.Size = new System.Drawing.Size(82, 20);
            this.tbStartOffset.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Start Offset";
            // 
            // bDumpText
            // 
            this.bDumpText.Location = new System.Drawing.Point(211, 17);
            this.bDumpText.Name = "bDumpText";
            this.bDumpText.Size = new System.Drawing.Size(75, 23);
            this.bDumpText.TabIndex = 8;
            this.bDumpText.Text = "Convert";
            this.bDumpText.UseVisualStyleBackColor = true;
            this.bDumpText.Click += new System.EventHandler(this.bDumpText_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.label5.Location = new System.Drawing.Point(111, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "*Hex values";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 69);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "File Name";
            // 
            // tbFileOrigin
            // 
            this.tbFileOrigin.Location = new System.Drawing.Point(92, 66);
            this.tbFileOrigin.Name = "tbFileOrigin";
            this.tbFileOrigin.Size = new System.Drawing.Size(126, 20);
            this.tbFileOrigin.TabIndex = 10;
            // 
            // fHexToJapanese
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.bConvert);
            this.Controls.Add(this.cbGameList);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbConvertedJapanese);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbHextoConvert);
            this.Controls.Add(this.label1);
            this.Name = "fHexToJapanese";
            this.Text = "HexToJapanese";
            this.Load += new System.EventHandler(this.HexToJapanese_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbHextoConvert;
        private System.Windows.Forms.TextBox tbConvertedJapanese;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbGameList;
        private System.Windows.Forms.Button bConvert;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bDumpText;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbStartOffset;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbFileOrigin;
    }
}