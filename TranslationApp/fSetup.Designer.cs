
namespace TranslationApp
{
    partial class fSetup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fSetup));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbRepos = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lbPythonInstallations = new System.Windows.Forms.ListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lbPythonLib = new System.Windows.Forms.ListBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tabGame = new System.Windows.Forms.TabControl();
            this.tabTOR = new System.Windows.Forms.TabPage();
            this.tabNDX = new System.Windows.Forms.TabPage();
            this.lbNDXIso = new System.Windows.Forms.ListBox();
            this.lbTORIso = new System.Windows.Forms.ListBox();
            this.bSaveConfiguration = new System.Windows.Forms.Button();
            this.bShowConfig = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tabGame.SuspendLayout();
            this.tabTOR.SuspendLayout();
            this.tabNDX.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(711, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(329, 577);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(27, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(499, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "1) Create a folder that contains PythonLib repo and the Games\'s repo";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(27, 520);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Repos:";
            // 
            // lbRepos
            // 
            this.lbRepos.FormattingEnabled = true;
            this.lbRepos.Items.AddRange(new object[] {
            "https://github.com/lifebottle/PythonLib.git",
            "https://github.com/lifebottle/Narikiri-Dungeon-X.git",
            "https://github.com/SymphoniaLauren/Tales-of-Rebirth.git"});
            this.lbRepos.Location = new System.Drawing.Point(31, 544);
            this.lbRepos.Name = "lbRepos";
            this.lbRepos.Size = new System.Drawing.Size(312, 56);
            this.lbRepos.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(27, 310);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "5)";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(47, 303);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(171, 37);
            this.button1.TabIndex = 6;
            this.button1.Text = "Install Python packages";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.bInstallPackages);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(27, 343);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(455, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "6) Extract the content of the game using the appropriate button";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(27, 499);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(448, 20);
            this.label5.TabIndex = 8;
            this.label5.Text = "7) Validate that everything went fine using the Console window";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(47, 366);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(289, 120);
            this.pictureBox2.TabIndex = 9;
            this.pictureBox2.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(27, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(236, 20);
            this.label6.TabIndex = 10;
            this.label6.Text = "2) Specify the location of Python";
            // 
            // lbPythonInstallations
            // 
            this.lbPythonInstallations.FormattingEnabled = true;
            this.lbPythonInstallations.Location = new System.Drawing.Point(47, 73);
            this.lbPythonInstallations.Name = "lbPythonInstallations";
            this.lbPythonInstallations.Size = new System.Drawing.Size(358, 69);
            this.lbPythonInstallations.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(30, 145);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(257, 20);
            this.label7.TabIndex = 13;
            this.label7.Text = "3) Specify the location of PythonLib";
            // 
            // lbPythonLib
            // 
            this.lbPythonLib.FormattingEnabled = true;
            this.lbPythonLib.Location = new System.Drawing.Point(47, 168);
            this.lbPythonLib.Name = "lbPythonLib";
            this.lbPythonLib.Size = new System.Drawing.Size(358, 17);
            this.lbPythonLib.TabIndex = 14;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(30, 197);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(204, 20);
            this.label8.TabIndex = 16;
            this.label8.Text = "4) Specify your isos location";
            // 
            // tabGame
            // 
            this.tabGame.Controls.Add(this.tabTOR);
            this.tabGame.Controls.Add(this.tabNDX);
            this.tabGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabGame.Location = new System.Drawing.Point(47, 220);
            this.tabGame.Name = "tabGame";
            this.tabGame.SelectedIndex = 0;
            this.tabGame.Size = new System.Drawing.Size(358, 77);
            this.tabGame.TabIndex = 17;
            // 
            // tabTOR
            // 
            this.tabTOR.Controls.Add(this.lbTORIso);
            this.tabTOR.Location = new System.Drawing.Point(4, 22);
            this.tabTOR.Name = "tabTOR";
            this.tabTOR.Padding = new System.Windows.Forms.Padding(3);
            this.tabTOR.Size = new System.Drawing.Size(350, 51);
            this.tabTOR.TabIndex = 0;
            this.tabTOR.Text = "TOR";
            this.tabTOR.UseVisualStyleBackColor = true;
            // 
            // tabNDX
            // 
            this.tabNDX.Controls.Add(this.lbNDXIso);
            this.tabNDX.Location = new System.Drawing.Point(4, 22);
            this.tabNDX.Name = "tabNDX";
            this.tabNDX.Padding = new System.Windows.Forms.Padding(3);
            this.tabNDX.Size = new System.Drawing.Size(350, 51);
            this.tabNDX.TabIndex = 1;
            this.tabNDX.Text = "NDX";
            this.tabNDX.UseVisualStyleBackColor = true;
            // 
            // lbNDXIso
            // 
            this.lbNDXIso.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbNDXIso.FormattingEnabled = true;
            this.lbNDXIso.Location = new System.Drawing.Point(4, 4);
            this.lbNDXIso.Name = "lbNDXIso";
            this.lbNDXIso.Size = new System.Drawing.Size(340, 43);
            this.lbNDXIso.TabIndex = 0;
            // 
            // lbTORIso
            // 
            this.lbTORIso.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTORIso.FormattingEnabled = true;
            this.lbTORIso.Location = new System.Drawing.Point(4, 4);
            this.lbTORIso.Name = "lbTORIso";
            this.lbTORIso.Size = new System.Drawing.Size(340, 43);
            this.lbTORIso.TabIndex = 1;
            // 
            // bSaveConfiguration
            // 
            this.bSaveConfiguration.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bSaveConfiguration.Location = new System.Drawing.Point(411, 263);
            this.bSaveConfiguration.Name = "bSaveConfiguration";
            this.bSaveConfiguration.Size = new System.Drawing.Size(141, 30);
            this.bSaveConfiguration.TabIndex = 18;
            this.bSaveConfiguration.Text = "Save Configuration";
            this.bSaveConfiguration.UseVisualStyleBackColor = true;
            this.bSaveConfiguration.Click += new System.EventHandler(this.bSaveConfiguration_Click);
            // 
            // bShowConfig
            // 
            this.bShowConfig.Location = new System.Drawing.Point(558, 577);
            this.bShowConfig.Name = "bShowConfig";
            this.bShowConfig.Size = new System.Drawing.Size(147, 23);
            this.bShowConfig.TabIndex = 19;
            this.bShowConfig.Text = "Show Config";
            this.bShowConfig.UseVisualStyleBackColor = true;
            this.bShowConfig.Click += new System.EventHandler(this.bShowConfig_Click);
            // 
            // fSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1052, 610);
            this.Controls.Add(this.bShowConfig);
            this.Controls.Add(this.bSaveConfiguration);
            this.Controls.Add(this.tabGame);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lbPythonLib);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lbPythonInstallations);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbRepos);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.KeyPreview = true;
            this.Name = "fSetup";
            this.Text = "Setup for Packing";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fSetup_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tabGame.ResumeLayout(false);
            this.tabTOR.ResumeLayout(false);
            this.tabNDX.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lbRepos;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox lbPythonInstallations;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ListBox lbPythonLib;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TabControl tabGame;
        private System.Windows.Forms.TabPage tabTOR;
        private System.Windows.Forms.TabPage tabNDX;
        private System.Windows.Forms.ListBox lbTORIso;
        private System.Windows.Forms.ListBox lbNDXIso;
        private System.Windows.Forms.Button bSaveConfiguration;
        private System.Windows.Forms.Button bShowConfig;
    }
}