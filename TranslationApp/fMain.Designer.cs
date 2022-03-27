
namespace TranslationApp
{
    partial class fMain
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
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.translationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.storyAndSkitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eventsAndNPCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.packToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tOPXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tORToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.storyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hexToJapaneseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbJapaneseText = new System.Windows.Forms.TextBox();
            this.tbEnglishText = new System.Windows.Forms.TextBox();
            this.tbNoteText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tcType = new System.Windows.Forms.TabControl();
            this.tabType1 = new System.Windows.Forms.TabPage();
            this.lbEntries = new System.Windows.Forms.ListBox();
            this.lFile = new System.Windows.Forms.Label();
            this.bSave = new System.Windows.Forms.Button();
            this.trackBarAlign = new System.Windows.Forms.TrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.verticalLine = new System.Windows.Forms.Panel();
            this.cbFileList = new System.Windows.Forms.ComboBox();
            this.cbFileType = new System.Windows.Forms.ComboBox();
            this.bBrowse = new System.Windows.Forms.Button();
            this.bMassReplace = new System.Windows.Forms.Button();
            this.cbStatus = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lErrors = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cbListStatus = new System.Windows.Forms.CheckedListBox();
            this.cbSections = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.searchJapaneseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStripMain.SuspendLayout();
            this.tcType.SuspendLayout();
            this.tabType1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAlign)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.translationToolStripMenuItem,
            this.packToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(751, 24);
            this.menuStripMain.TabIndex = 0;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // translationToolStripMenuItem
            // 
            this.translationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.storyAndSkitsToolStripMenuItem,
            this.eventsAndNPCToolStripMenuItem});
            this.translationToolStripMenuItem.Name = "translationToolStripMenuItem";
            this.translationToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            this.translationToolStripMenuItem.Text = "Translation";
            // 
            // storyAndSkitsToolStripMenuItem
            // 
            this.storyAndSkitsToolStripMenuItem.Name = "storyAndSkitsToolStripMenuItem";
            this.storyAndSkitsToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.storyAndSkitsToolStripMenuItem.Text = "TOPX";
            this.storyAndSkitsToolStripMenuItem.Click += new System.EventHandler(this.TOPXToolStripMenuItem_Click);
            // 
            // eventsAndNPCToolStripMenuItem
            // 
            this.eventsAndNPCToolStripMenuItem.Name = "eventsAndNPCToolStripMenuItem";
            this.eventsAndNPCToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.eventsAndNPCToolStripMenuItem.Text = "TOR";
            this.eventsAndNPCToolStripMenuItem.Click += new System.EventHandler(this.TORToolStripMenuItem_Click);
            // 
            // packToolStripMenuItem
            // 
            this.packToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tOPXToolStripMenuItem,
            this.tORToolStripMenuItem});
            this.packToolStripMenuItem.Name = "packToolStripMenuItem";
            this.packToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.packToolStripMenuItem.Text = "Packing";
            // 
            // tOPXToolStripMenuItem
            // 
            this.tOPXToolStripMenuItem.Name = "tOPXToolStripMenuItem";
            this.tOPXToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.tOPXToolStripMenuItem.Text = "TOPX";
            this.tOPXToolStripMenuItem.Click += new System.EventHandler(this.tOPXToolStripMenuItem_Click_1);
            // 
            // tORToolStripMenuItem
            // 
            this.tORToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allToolStripMenuItem,
            this.menuToolStripMenuItem,
            this.storyToolStripMenuItem,
            this.skitsToolStripMenuItem,
            this.eventsToolStripMenuItem});
            this.tORToolStripMenuItem.Name = "tORToolStripMenuItem";
            this.tORToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.tORToolStripMenuItem.Text = "TOR";
            // 
            // allToolStripMenuItem
            // 
            this.allToolStripMenuItem.Name = "allToolStripMenuItem";
            this.allToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.allToolStripMenuItem.Text = "All";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.menuToolStripMenuItem.Text = "Menu";
            this.menuToolStripMenuItem.Click += new System.EventHandler(this.menuToolStripMenuItem_Click_1);
            // 
            // storyToolStripMenuItem
            // 
            this.storyToolStripMenuItem.Name = "storyToolStripMenuItem";
            this.storyToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.storyToolStripMenuItem.Text = "Story";
            // 
            // skitsToolStripMenuItem
            // 
            this.skitsToolStripMenuItem.Name = "skitsToolStripMenuItem";
            this.skitsToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.skitsToolStripMenuItem.Text = "Skits";
            // 
            // eventsToolStripMenuItem
            // 
            this.eventsToolStripMenuItem.Name = "eventsToolStripMenuItem";
            this.eventsToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.eventsToolStripMenuItem.Text = "Events";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hexToJapaneseToolStripMenuItem,
            this.searchJapaneseToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // hexToJapaneseToolStripMenuItem
            // 
            this.hexToJapaneseToolStripMenuItem.Name = "hexToJapaneseToolStripMenuItem";
            this.hexToJapaneseToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.hexToJapaneseToolStripMenuItem.Text = "Hex to Japanese";
            this.hexToJapaneseToolStripMenuItem.Click += new System.EventHandler(this.hexToJapaneseToolStripMenuItem_Click);
            // 
            // tbJapaneseText
            // 
            this.tbJapaneseText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbJapaneseText.HideSelection = false;
            this.tbJapaneseText.Location = new System.Drawing.Point(337, 187);
            this.tbJapaneseText.Multiline = true;
            this.tbJapaneseText.Name = "tbJapaneseText";
            this.tbJapaneseText.ReadOnly = true;
            this.tbJapaneseText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbJapaneseText.Size = new System.Drawing.Size(350, 111);
            this.tbJapaneseText.TabIndex = 5;
            // 
            // tbEnglishText
            // 
            this.tbEnglishText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbEnglishText.Location = new System.Drawing.Point(337, 326);
            this.tbEnglishText.Multiline = true;
            this.tbEnglishText.Name = "tbEnglishText";
            this.tbEnglishText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbEnglishText.Size = new System.Drawing.Size(350, 111);
            this.tbEnglishText.TabIndex = 6;
            this.tbEnglishText.TextChanged += new System.EventHandler(this.tbEnglishText_TextChanged);
            // 
            // tbNoteText
            // 
            this.tbNoteText.Location = new System.Drawing.Point(337, 461);
            this.tbNoteText.Multiline = true;
            this.tbNoteText.Name = "tbNoteText";
            this.tbNoteText.Size = new System.Drawing.Size(259, 39);
            this.tbNoteText.TabIndex = 7;
            this.tbNoteText.TextChanged += new System.EventHandler(this.tbNoteText_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(334, 171);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Japanese";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(334, 310);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "English";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(334, 445);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Notes";
            // 
            // tcType
            // 
            this.tcType.Controls.Add(this.tabType1);
            this.tcType.Location = new System.Drawing.Point(7, 188);
            this.tcType.Name = "tcType";
            this.tcType.SelectedIndex = 0;
            this.tcType.Size = new System.Drawing.Size(277, 404);
            this.tcType.TabIndex = 13;
            // 
            // tabType1
            // 
            this.tabType1.Controls.Add(this.lbEntries);
            this.tabType1.Location = new System.Drawing.Point(4, 22);
            this.tabType1.Name = "tabType1";
            this.tabType1.Padding = new System.Windows.Forms.Padding(3);
            this.tabType1.Size = new System.Drawing.Size(269, 378);
            this.tabType1.TabIndex = 0;
            this.tabType1.UseVisualStyleBackColor = true;
            // 
            // lbEntries
            // 
            this.lbEntries.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lbEntries.FormattingEnabled = true;
            this.lbEntries.Location = new System.Drawing.Point(5, 4);
            this.lbEntries.Name = "lbEntries";
            this.lbEntries.Size = new System.Drawing.Size(262, 368);
            this.lbEntries.TabIndex = 0;
            this.lbEntries.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbEntries_DrawItem);
            this.lbEntries.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.lbEntries_MeasureItem);
            this.lbEntries.SelectedIndexChanged += new System.EventHandler(this.lbEntries_SelectedIndexChanged);
            // 
            // lFile
            // 
            this.lFile.AutoSize = true;
            this.lFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFile.Location = new System.Drawing.Point(13, 52);
            this.lFile.Name = "lFile";
            this.lFile.Size = new System.Drawing.Size(0, 16);
            this.lFile.TabIndex = 15;
            // 
            // bSave
            // 
            this.bSave.Location = new System.Drawing.Point(612, 477);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 23);
            this.bSave.TabIndex = 16;
            this.bSave.Text = "Save";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // trackBarAlign
            // 
            this.trackBarAlign.Location = new System.Drawing.Point(481, 139);
            this.trackBarAlign.Name = "trackBarAlign";
            this.trackBarAlign.Size = new System.Drawing.Size(115, 45);
            this.trackBarAlign.TabIndex = 18;
            this.trackBarAlign.ValueChanged += new System.EventHandler(this.trackBarAlign_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(478, 126);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Text Align";
            // 
            // verticalLine
            // 
            this.verticalLine.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.verticalLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.verticalLine.Location = new System.Drawing.Point(685, 188);
            this.verticalLine.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
            this.verticalLine.Name = "verticalLine";
            this.verticalLine.Size = new System.Drawing.Size(2, 250);
            this.verticalLine.TabIndex = 20;
            // 
            // cbFileList
            // 
            this.cbFileList.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbFileList.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbFileList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbFileList.FormattingEnabled = true;
            this.cbFileList.Location = new System.Drawing.Point(136, 49);
            this.cbFileList.Name = "cbFileList";
            this.cbFileList.Size = new System.Drawing.Size(148, 21);
            this.cbFileList.TabIndex = 21;
            this.cbFileList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbFileList_DrawItem);
            this.cbFileList.TextChanged += new System.EventHandler(this.cbFileList_TextChanged);
            // 
            // cbFileType
            // 
            this.cbFileType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbFileType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbFileType.FormattingEnabled = true;
            this.cbFileType.Location = new System.Drawing.Point(12, 49);
            this.cbFileType.Name = "cbFileType";
            this.cbFileType.Size = new System.Drawing.Size(104, 21);
            this.cbFileType.TabIndex = 22;
            this.cbFileType.TextChanged += new System.EventHandler(this.cbFileType_TextChanged);
            // 
            // bBrowse
            // 
            this.bBrowse.Location = new System.Drawing.Point(337, 47);
            this.bBrowse.Name = "bBrowse";
            this.bBrowse.Size = new System.Drawing.Size(75, 23);
            this.bBrowse.TabIndex = 23;
            this.bBrowse.Text = "Browse File";
            this.bBrowse.UseVisualStyleBackColor = true;
            // 
            // bMassReplace
            // 
            this.bMassReplace.Location = new System.Drawing.Point(418, 47);
            this.bMassReplace.Name = "bMassReplace";
            this.bMassReplace.Size = new System.Drawing.Size(144, 23);
            this.bMassReplace.TabIndex = 24;
            this.bMassReplace.Text = "Mass Replace";
            this.bMassReplace.UseVisualStyleBackColor = true;
            // 
            // cbStatus
            // 
            this.cbStatus.FormattingEnabled = true;
            this.cbStatus.Items.AddRange(new object[] {
            "To Do",
            "Problematics",
            "For Review",
            "Done"});
            this.cbStatus.Location = new System.Drawing.Point(337, 126);
            this.cbStatus.Name = "cbStatus";
            this.cbStatus.Size = new System.Drawing.Size(121, 21);
            this.cbStatus.TabIndex = 25;
            this.cbStatus.TextChanged += new System.EventHandler(this.cbStatus_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(334, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Status";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(9, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(127, 15);
            this.label5.TabIndex = 27;
            this.label5.Text = "Filter Entries by Status";
            // 
            // lErrors
            // 
            this.lErrors.AutoSize = true;
            this.lErrors.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lErrors.ForeColor = System.Drawing.Color.Red;
            this.lErrors.Location = new System.Drawing.Point(381, 507);
            this.lErrors.Name = "lErrors";
            this.lErrors.Size = new System.Drawing.Size(14, 15);
            this.lErrors.TabIndex = 1;
            this.lErrors.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(335, 507);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 15);
            this.label6.TabIndex = 0;
            this.label6.Text = "Errors";
            // 
            // cbListStatus
            // 
            this.cbListStatus.FormattingEnabled = true;
            this.cbListStatus.Items.AddRange(new object[] {
            "To Do",
            "In Review",
            "Problematic",
            "Done"});
            this.cbListStatus.Location = new System.Drawing.Point(136, 83);
            this.cbListStatus.MultiColumn = true;
            this.cbListStatus.Name = "cbListStatus";
            this.cbListStatus.Size = new System.Drawing.Size(148, 64);
            this.cbListStatus.TabIndex = 29;
            this.cbListStatus.SelectedIndexChanged += new System.EventHandler(this.cbListStatus_SelectedIndexChanged_1);
            // 
            // cbSections
            // 
            this.cbSections.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbSections.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbSections.FormattingEnabled = true;
            this.cbSections.Location = new System.Drawing.Point(136, 163);
            this.cbSections.Name = "cbSections";
            this.cbSections.Size = new System.Drawing.Size(148, 21);
            this.cbSections.TabIndex = 30;
            this.cbSections.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            this.cbSections.TextChanged += new System.EventHandler(this.cbSections_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(13, 164);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(93, 15);
            this.label8.TabIndex = 31;
            this.label8.Text = "Filter by Section";
            // 
            // searchJapaneseToolStripMenuItem
            // 
            this.searchJapaneseToolStripMenuItem.Name = "searchJapaneseToolStripMenuItem";
            this.searchJapaneseToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.searchJapaneseToolStripMenuItem.Text = "Search files for Japanese";
            // 
            // fMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 591);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cbSections);
            this.Controls.Add(this.cbListStatus);
            this.Controls.Add(this.lErrors);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbStatus);
            this.Controls.Add(this.bMassReplace);
            this.Controls.Add(this.bBrowse);
            this.Controls.Add(this.cbFileType);
            this.Controls.Add(this.cbFileList);
            this.Controls.Add(this.verticalLine);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.trackBarAlign);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.lFile);
            this.Controls.Add(this.tcType);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbNoteText);
            this.Controls.Add(this.tbEnglishText);
            this.Controls.Add(this.menuStripMain);
            this.Controls.Add(this.tbJapaneseText);
            this.MainMenuStrip = this.menuStripMain;
            this.Name = "fMain";
            this.Text = "Translation App";
            this.Load += new System.EventHandler(this.fMain_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.fMain_Paint);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.tcType.ResumeLayout(false);
            this.tabType1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAlign)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem translationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem storyAndSkitsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eventsAndNPCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem packToolStripMenuItem;
        private System.Windows.Forms.TextBox tbJapaneseText;
        private System.Windows.Forms.TextBox tbEnglishText;
        private System.Windows.Forms.TextBox tbNoteText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabControl tcType;
        private System.Windows.Forms.TabPage tabType1;
        private System.Windows.Forms.Label lFile;
        private System.Windows.Forms.ListBox lbEntries;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.TrackBar trackBarAlign;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel verticalLine;
        private System.Windows.Forms.ComboBox cbFileList;
        private System.Windows.Forms.ComboBox cbFileType;
        private System.Windows.Forms.Button bBrowse;
        private System.Windows.Forms.Button bMassReplace;
        private System.Windows.Forms.ToolStripMenuItem tOPXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tORToolStripMenuItem;
        private System.Windows.Forms.ComboBox cbStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lErrors;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckedListBox cbListStatus;
        private System.Windows.Forms.ComboBox cbSections;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hexToJapaneseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem storyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skitsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eventsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchJapaneseToolStripMenuItem;
    }
}

