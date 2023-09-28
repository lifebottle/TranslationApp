using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TranslationLib;
using PackingLib;
using System.Reflection;

namespace TranslationApp
{
    public partial class fMain : Form
    {
        private static Config config;
        private GameConfig gameConfig;
        private static TranslationProject Project;
        private static PackingProject PackingAssistant;
        private static List<XMLEntry> CurrentTextList;
        private static List<XMLEntry> CurrentSpeakerList;
        private Dictionary<string, Color> ColorByStatus;
        private string gameName;
        Bitmap fontAtlasImage;

        struct font_glyph
        {
            public byte lskip;
            public byte rskip;

            public font_glyph(byte x, byte y)
            {
                lskip = x;
                rskip = y;
            }
        }

        private readonly font_glyph[] glyphs = new font_glyph[97]
        {
            /*    */ new font_glyph(10, 00),
            /* ０ */ new font_glyph(05, 05),
            /* １ */ new font_glyph(06, 05),
            /* ２ */ new font_glyph(05, 05),
            /* ３ */ new font_glyph(05, 06),
            /* ４ */ new font_glyph(04, 07),
            /* ５ */ new font_glyph(06, 06),
            /* ６ */ new font_glyph(06, 06),
            /* ７ */ new font_glyph(06, 07),
            /* ８ */ new font_glyph(04, 05),
            /* ９ */ new font_glyph(04, 04),
            /* Ａ */ new font_glyph(04, 06),
            /* Ｂ */ new font_glyph(05, 06),
            /* Ｃ */ new font_glyph(05, 06),
            /* Ｄ */ new font_glyph(05, 06),
            /* Ｅ */ new font_glyph(05, 07),
            /* Ｆ */ new font_glyph(05, 08),
            /* Ｇ */ new font_glyph(05, 07),
            /* Ｈ */ new font_glyph(05, 07),
            /* Ｉ */ new font_glyph(08, 09),
            /* Ｊ */ new font_glyph(07, 08),
            /* Ｋ */ new font_glyph(05, 06),
            /* Ｌ */ new font_glyph(05, 08),
            /* Ｍ */ new font_glyph(05, 05),
            /* Ｎ */ new font_glyph(05, 06),
            /* Ｏ */ new font_glyph(05, 05),
            /* Ｐ */ new font_glyph(05, 06),
            /* Ｑ */ new font_glyph(05, 05),
            /* Ｒ */ new font_glyph(05, 07),
            /* Ｓ */ new font_glyph(06, 07),
            /* Ｔ */ new font_glyph(05, 07),
            /* Ｕ */ new font_glyph(05, 06),
            /* Ｖ */ new font_glyph(05, 06),
            /* Ｗ */ new font_glyph(02, 03),
            /* Ｘ */ new font_glyph(05, 07),
            /* Ｙ */ new font_glyph(05, 08),
            /* Ｚ */ new font_glyph(05, 05),
            /* ａ */ new font_glyph(06, 08),
            /* ｂ */ new font_glyph(06, 07),
            /* ｃ */ new font_glyph(07, 08),
            /* ｄ */ new font_glyph(06, 07),
            /* ｅ */ new font_glyph(06, 07),
            /* ｆ */ new font_glyph(07, 09),
            /* ｇ */ new font_glyph(06, 07),
            /* ｈ */ new font_glyph(06, 07),
            /* ｉ */ new font_glyph(08, 09),
            /* ｊ */ new font_glyph(09, 10),
            /* ｋ */ new font_glyph(05, 07),
            /* ｌ */ new font_glyph(09, 09),
            /* ｍ */ new font_glyph(03, 05),
            /* ｎ */ new font_glyph(06, 07),
            /* ｏ */ new font_glyph(06, 07),
            /* ｐ */ new font_glyph(06, 07),
            /* ｑ */ new font_glyph(06, 07),
            /* ｒ */ new font_glyph(07, 09),
            /* ｓ */ new font_glyph(07, 08),
            /* ｔ */ new font_glyph(07, 08),
            /* ｕ */ new font_glyph(06, 07),
            /* ｖ */ new font_glyph(05, 07),
            /* ｗ */ new font_glyph(03, 04),
            /* ｘ */ new font_glyph(06, 08),
            /* ｙ */ new font_glyph(05, 07),
            /* ｚ */ new font_glyph(06, 07),
            /* ， */ new font_glyph(01, 15),
            /* ． */ new font_glyph(01, 15),
            /* ・ */ new font_glyph(06, 08),
            /* ： */ new font_glyph(08, 08),
            /* ； */ new font_glyph(07, 08),
            /* ？ */ new font_glyph(04, 05),
            /* ！ */ new font_glyph(07, 09),
            /* ／ */ new font_glyph(00, 01),
            /* （ */ new font_glyph(12, 01),
            /* ） */ new font_glyph(01, 13),
            /* ［ */ new font_glyph(13, 01),
            /* ］ */ new font_glyph(01, 11),
            /* ｛ */ new font_glyph(14, 01),
            /* ｝ */ new font_glyph(01, 14),
            /* ＋ */ new font_glyph(03, 06),
            /* － */ new font_glyph(06, 07),
            /* ＝ */ new font_glyph(04, 03),
            /* ＜ */ new font_glyph(06, 06),
            /* ＞ */ new font_glyph(06, 06),
            /* ％ */ new font_glyph(02, 09),
            /* ＃ */ new font_glyph(04, 04),
            /* ＆ */ new font_glyph(02, 04),
            /* ＊ */ new font_glyph(04, 04),
            /* ＠ */ new font_glyph(00, 01),
            /* ｜ */ new font_glyph(08, 08),
            /*  ” */ new font_glyph(01, 15),
            /*  ’ */ new font_glyph(01, 18),
            /* ＾ */ new font_glyph(07, 06),
            /* 「 */ new font_glyph(10, 01),
            /* 」 */ new font_glyph(01, 11),
            /* 〜 */ new font_glyph(05, 06),
            /* ＿ */ new font_glyph(00, 00),
            /* 、 */ new font_glyph(00, 13),
            /* 。 */ new font_glyph(01, 12),
        };

        public fMain()
        {
            InitializeComponent();
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            cbLanguage.Text = "English (if available)";
            CreateColorByStatusDictionnary();
            InitialiseStatusText();
            ChangeEnabledProp(false);
            
            config = new Config();
            config.Load();
            PackingAssistant = new PackingProject();
            InitializeFontAtlas();
        }

        private Bitmap LoadEmbeddedImage(string resourceName)
        {
            try
            {
                using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    if (manifestResourceStream != null)
                    {
                        return new Bitmap(manifestResourceStream);
                    }
                    MessageBox.Show("Failed to load embedded image.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading embedded image: " + ex.Message);
            }
            return null;
        }

        private void InitializeFontAtlas()
        {
            fontAtlasImage = LoadEmbeddedImage("TranslationApp.res.font_atlas.png");
        }

        private void CreateColorByStatusDictionnary()
        {
            ColorByStatus = new Dictionary<string, Color>
            {
                { "To Do", Color.White },
                { "Editing", Color.FromArgb(162, 255, 255) }, // Light Cyan
                { "Proofreading", Color.FromArgb(255, 102, 255) }, // Magenta
                { "Problematic", Color.FromArgb(255, 255, 162) }, // Light Yellow
                { "Done", Color.FromArgb(162, 255, 162) }, // Light Green
            };
        }

        private void InitialiseStatusText()
        {
       
            lNbToDo.Text = "";
            lNbEditing.Text = "";
            lNbProb.Text = "";
            lNbProof.Text = "";
            lNbDone.Text = "";

            lNbToDoSect.Text = "";
            lNbProbSect.Text = "";
            lNbEditingSect.Text = "";
            lNbProofSect.Text = "";
            lNbDoneSect.Text = "";
        }

        private void ChangeEnabledProp(bool status)
        {
            cbFileType.Enabled = status;
            cbFileList.Enabled = status;
            cbLanguage.Enabled = status;
            cbStatus.Enabled = status;
            tbEnglishText.Enabled = status;
            tbJapaneseText.Enabled = status;
            tbNoteText.Enabled = status;

            lbEntries.Enabled = status;

            //Checked List
            cbToDo.Enabled = status;
            cbEditing.Enabled = status;
            cbProof.Enabled = status;
            cbDone.Enabled = status;
            cbProblematic.Enabled = status;
            cbDone.Enabled = status;

            //Button
            bSave.Enabled = status;
            btnRefresh.Enabled = status;

            //Panel
            panelNb1.Enabled = status;
            panelNb2.Enabled = status;
        }

        private void DrawEntries(DrawItemEventArgs e, List<XMLEntry> EntryList)
        {
            bool isSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);

            //Draw only if elements are present in the listbox
            if (e.Index > -1)
            {
                //Regardless of text, draw elements close together
                //and use the intmax size as per the docs
                TextFormatFlags flags = TextFormatFlags.NoPadding;
                Size proposedSize = new Size(int.MaxValue, int.MaxValue);

                //Grab the current entry to draw
                XMLEntry entry = EntryList[e.Index];

                // Background item brush
                SolidBrush backgroundBrush = new SolidBrush(isSelected ? SystemColors.Highlight : ColorByStatus[entry.Status]);

                // Text colors
                Color regularColor = e.ForeColor;
                Color tagColor = isSelected ? Color.Orange : Color.Blue;

                // Draw the background
                e.Graphics.FillRectangle(backgroundBrush, e.Bounds);

                // Add separators for each entry
                e.Graphics.DrawLine(new Pen(Color.DimGray, 1.5f), new Point(0, e.Bounds.Bottom - 1), new Point(e.Bounds.Width, e.Bounds.Bottom - 1));
                e.Graphics.DrawLine(new Pen(Color.DimGray, 1.5f), new Point(0, e.Bounds.Top - 1), new Point(e.Bounds.Width, e.Bounds.Top - 1));

                Font normalFont = new Font("Arial", 8, FontStyle.Regular);
                Font boldFont = new Font("Arial", 8, FontStyle.Bold);

                string text = GetTextBasedLanguage(e.Index, EntryList);

                //0. Add Speaker name
                Point startPoint = new Point(3, e.Bounds.Y + 3);
                if (EntryList[e.Index].SpeakerId != null)
                {
                    TextRenderer.DrawText(e.Graphics, EntryList[e.Index].SpeakerName, boldFont, startPoint, tagColor, flags);
                    startPoint.Y += 13;
                }

                //1. Split based on the line breaks
                if (!string.IsNullOrEmpty(text))
                {
                    string[] lines = Regex.Split(text, "\\r*\\n", RegexOptions.IgnoreCase);

                    //Starting point for drawing, a little offsetted
                    //in order to not touch the borders
                    //Point startPoint = new Point(3, e.Bounds.Y + 3);
                    Size mySize;

                    foreach (string line in lines)
                    {
                        //2. Split based on the different tags
                        //Split the text based on the Tags < xxx >
                        string pattern = @"(<[\w/]+:?\w+>)";
                        string[] result = Regex.Split(line, pattern, RegexOptions.IgnoreCase).Where(x => x != "").ToArray();

                        //We need to loop over each element to adjust the color
                        foreach (string element in result)
                        {
                            if (element[0] == '<')
                            {
                                mySize = TextRenderer.MeasureText(e.Graphics, element, boldFont, proposedSize, flags);

                                TextRenderer.DrawText(e.Graphics, element, boldFont, startPoint, tagColor, flags);
                                startPoint.X += mySize.Width;
                            }
                            else
                            {
                                mySize = TextRenderer.MeasureText(e.Graphics, element, normalFont, proposedSize, flags);

                                TextRenderer.DrawText(e.Graphics, element, normalFont, startPoint, regularColor, flags);
                                startPoint.X += mySize.Width;
                            }
                        }

                        startPoint.Y += 13;
                        startPoint.X = 3;
                    }
                }

                // Clean up
                backgroundBrush.Dispose();
            }

            e.DrawFocusRectangle();
        }

        //Draw entries with multiline and font color changed
        private void lbEntries_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawEntries(e, CurrentTextList);
        }

        private void lbSpeaker_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawEntries(e, CurrentSpeakerList);
        }

        private void lbEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEntryData(lbEntries);
        }

        private void LoadEntryData(ListBox lb)
        {
            tbEnglishText.TextChanged -= tbEnglishText_TextChanged;

            XMLEntry currentEntry = (XMLEntry)lb.SelectedItem;

            if (currentEntry == null)
            {
                tbJapaneseText.Enabled = false;
                tbEnglishText.Enabled = false;
                cbStatus.Enabled = false;
                cbEmpty.Enabled = false;
                return;
            }
            else
            {
                tbJapaneseText.Enabled = true;
                tbEnglishText.Enabled = true;
                cbStatus.Enabled = true;
                cbEmpty.Enabled = true;
            }

            tbJapaneseText.Text = string.Empty;
            tbEnglishText.Text = string.Empty;

            TranslationEntry TranslationEntry;
            if (currentEntry.JapaneseText == null)
                TranslationEntry = new TranslationEntry { EnglishTranslation = "" };
            else
                Project.CurrentFolder.Translations.TryGetValue(currentEntry.JapaneseText, out TranslationEntry);

            if (TranslationEntry != null && TranslationEntry.Count > 1)
                lblJapanese.Text = $@"Japanese ({TranslationEntry.Count - 1} duplicate(s) found)";
            else
                lblJapanese.Text = $@"Japanese";

            if (currentEntry.JapaneseText != null)
                tbJapaneseText.Text = currentEntry.JapaneseText.Replace("\r", "").Replace("\n", Environment.NewLine);
            if (currentEntry.EnglishText != null)
                tbEnglishText.Text = currentEntry.EnglishText.Replace("\r", "").Replace("\n", Environment.NewLine);
            if (tbNoteText != null)
                tbNoteText.Text = currentEntry.Notes;

            cbEmpty.Checked = currentEntry.EnglishText?.Equals("") ?? false;

            cbStatus.Text = currentEntry.Status;
            pictureBox1.Invalidate();
            tbEnglishText.TextChanged += tbEnglishText_TextChanged;
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            Project.XmlFolders.ForEach(f => f.XMLFiles.ForEach(x => x.SaveToDisk()));
            MessageBox.Show("Text has been written to the XML files");

            UpdateDisplayedEntries();
            UpdateStatusData();
        }

        private void fMain_Paint(object sender, PaintEventArgs e)
        {
            Point p = new Point();
            p.X = tbJapaneseText.Location.X + tbJapaneseText.Size.Width - trackBarAlign.Value * 10;
            p.Y = tbJapaneseText.Location.Y;
            verticalLine.Location = p;
        }

        private void trackBarAlign_ValueChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private string GetTextBasedLanguage(int entryIndex, List<XMLEntry> EntryList)
        {
            var myEntry = EntryList[entryIndex];
            if (cbLanguage.Text == "Japanese")
                return myEntry.JapaneseText;
            else
                return string.IsNullOrEmpty(myEntry.EnglishText) ? myEntry.JapaneseText : myEntry.EnglishText;
        }

        public string GetFolderPath()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    return fbd.SelectedPath;

                return "";
            }
        }

        private void LoadNewFolder(string gameName, string path)
        {
            lbEntries.BorderStyle = BorderStyle.FixedSingle;
            var loadedFolder = TryLoadFolder(GetFolderPath() + path);
            gameConfig = config.GamesConfigList.Where(x => x.Game == gameName).FirstOrDefault();

            if (gameConfig == null)
            {
                GameConfig newConfig = new GameConfig();
                newConfig.FolderPath = loadedFolder;
                newConfig.LastFolderPath = loadedFolder;
                newConfig.Game = gameName;
                config.GamesConfigList.Add(newConfig);
                gameConfig = config.GetGameConfig(gameName);
            }
            else
            {
                gameConfig.FolderPath = loadedFolder;
                gameConfig.LastFolderPath = loadedFolder;
                gameConfig.Game = gameName;
            }

            config.Save();
            UpdateOptionsVisibility();
        }
        private void LoadLastFolder(string gameName)
        {
            var myConfig = config.GetGameConfig(gameName);
            if (myConfig != null)
            {
                TryLoadFolder(config.GetGameConfig(gameName).FolderPath);
                gameConfig = myConfig;
                UpdateOptionsVisibility();
            }
            else
                MessageBox.Show("The game you are trying to load is not inside the configuration file,\nplease load a new folder.");
        }
        private void tsTORLoadNew_Click(object sender, EventArgs e)
        {
            LoadNewFolder("TOR", "/2_translated");
        }
        private void tsNDXLoadNew_Click(object sender, EventArgs e)
        {
            LoadNewFolder("NDX", "/Data/NDX");
        }
        private void tsTORLoadLast_Click(object sender, EventArgs e)
        {
            LoadLastFolder("TOR");
        }
        private void tsNDXLoadLast_Click(object sender, EventArgs e)
        {
            LoadLastFolder("NDX");
        }
        public string TryLoadFolder(string path)
        {
            if (Directory.Exists(path))
            {
                LoadFolder(path);
                return path;
            }

            MessageBox.Show("Are you sure you selected the right folder?\n" +
                            "The folder you choose doesn't represent a valid Tales of Repo.\n" +
                            $"It should have the Data folder in it.\nPath {path} not valid.");
            return null;
        }

        private void LoadFolder(string path)
        {
            DisableEventHandlers();

            var folderIncluded = new List<string> { "Story", "Menu", "Skits" };
            Project = new TranslationProject(path, folderIncluded);

            CurrentTextList = Project.CurrentFolder.CurrentFile.CurrentSection.Entries;
            CurrentSpeakerList = Project.CurrentFolder.CurrentFile.Speakers;
            cbFileType.DataSource = Project.GetFolderNames().OrderByDescending(x=>x).ToList();
            cbFileList.DataSource = Project.CurrentFolder.FileList();
            cbSections.DataSource = Project.CurrentFolder.CurrentFile.GetSectionNames();
            UpdateDisplayedEntries();
            UpdateStatusData();

            ChangeEnabledProp(true);
            EnableEventHandlers();
        }

        private void DisableEventHandlers()
        {
            cbFileType.TextChanged -= cbFileType_TextChanged;
            cbFileList.DrawItem -= cbFileList_DrawItem;
            cbFileList.TextChanged -= cbFileList_TextChanged;
            cbSections.TextChanged -= cbSections_TextChanged;
            cbSections.SelectedIndexChanged -= cbSections_SelectedIndexChanged;
        }

        private void EnableEventHandlers()
        {
            cbFileType.TextChanged += cbFileType_TextChanged;
            cbFileList.DrawItem += cbFileList_DrawItem;
            cbFileList.TextChanged += cbFileList_TextChanged;
            cbSections.TextChanged += cbSections_TextChanged;
            cbSections.SelectedIndexChanged += cbSections_SelectedIndexChanged;
        }

        private void cbFileType_TextChanged(object sender, EventArgs e)
        {
            if (cbFileType.SelectedItem.ToString() != string.Empty)
            {
                Project.SetCurrentFolder(cbFileType.SelectedItem.ToString());
                cbFileList.DataSource = Project.CurrentFolder.FileList();

                cbSections.DataSource = Project.CurrentFolder.CurrentFile.GetSectionNames();
                UpdateStatusData();
            }
        }

        private void UpdateDisplayedEntries()
        {
            var checkedFilters = new List<string>
            {
                cbToDo.Checked ? "To Do" : string.Empty,
                cbProof.Checked ? "Proofreading" : string.Empty,
                cbEditing.Checked ? "Editing" : string.Empty,
                cbProblematic.Checked ? "Problematic" : string.Empty,
                cbDone.Checked ? "Done" : string.Empty
            };

            if (tcType.Controls[tcType.SelectedIndex].Text == "Text")
            {
                CurrentTextList = Project.CurrentFolder.CurrentFile.CurrentSection.Entries.Where(e => checkedFilters.Contains(e.Status)).ToList();
                var old_index = lbEntries.SelectedIndex;
                lbEntries.DataSource = CurrentTextList;
                if (lbEntries.Items.Count > old_index)
                {
                    lbEntries.SelectedIndex = old_index;
                }
            }
            else
            {
                CurrentSpeakerList = Project.CurrentFolder.CurrentFile.Speakers.Where(e => checkedFilters.Contains(e.Status)).ToList();
                var old_index = lbSpeaker.SelectedIndex;
                lbSpeaker.DataSource = CurrentSpeakerList;
                if (lbSpeaker.Items.Count > old_index)
                {
                    lbSpeaker.SelectedIndex = old_index;
                }
            }
        }

        public void UpdateOptionsVisibility()
        {
            bool TORValid = config.IsPackingVisibility("TOR");
            tsTORPacking.Enabled = tsTORMakeIso.Enabled = tsTORExtract.Enabled = TORValid;






        }

        private void UpdateStatusData()
        {
            var speakerStatusStats = Project.CurrentFolder.CurrentFile.SpeakersGetStatusData();
            var statusStats = Project.CurrentFolder.CurrentFile.GetStatusData();
            //File Count of status
            lNbToDo.Text = (statusStats["To Do"]).ToString();
            lNbProof.Text = (statusStats["Proofreading"]).ToString();
            lNbProb.Text = (statusStats["Problematic"]).ToString();
            lNbEditing.Text = (statusStats["Editing"]).ToString();
            lNbDone.Text = (statusStats["Done"]).ToString();

            Dictionary<string, int> sectionStatusStats= new Dictionary<string, int>();
            if (tcType.SelectedTab.Text == "Speaker")
                sectionStatusStats = speakerStatusStats;
            else
                sectionStatusStats = Project.CurrentFolder.CurrentFile.CurrentSection.GetStatusData();
            //Section Count of status
            lNbToDoSect.Text = sectionStatusStats["To Do"].ToString();
            lNbProofSect.Text = sectionStatusStats["Proofreading"].ToString();
            lNbProbSect.Text = sectionStatusStats["Problematic"].ToString();
            lNbEditingSect.Text = sectionStatusStats["Editing"].ToString();
            lNbDoneSect.Text = sectionStatusStats["Done"].ToString();
        }

        private void cbFileList_TextChanged(object sender, EventArgs e)
        {
            if (cbFileList.SelectedIndex != -1)
            {
                Project.CurrentFolder.SetCurrentFile(cbFileList.SelectedItem.ToString());

                var old_section = cbSections.SelectedItem.ToString();

                List<string> sections = Project.CurrentFolder.CurrentFile.GetSectionNames();
                if (cbFileType.Text != "Menu")
                    cbSections.DataSource = sections.OrderBy(x=>x).ToList();
                else
                    cbSections.DataSource = sections.OrderBy(x => x).ToList();

                if (cbSections.Items.Contains(old_section))
                    cbSections.SelectedItem = old_section;

                CurrentTextList = Project.CurrentFolder.CurrentFile.CurrentSection.Entries;
                CurrentSpeakerList = Project.CurrentFolder.CurrentFile.Speakers;
                FilterEntryList();

                bSave.Enabled = true;
            }
        }

        private void NDXToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
        }

        private void tbEnglishText_TextChanged(object sender, EventArgs e)
        {

            bool error = (tbEnglishText.Text.Count(x => x == '<') == tbEnglishText.Text.Count(x => x == '>'));

            lErrors.Text = "";
            if (!error)
            {
                lErrors.Text = "Warning: You might be missing a </> for your tags.\nIf you are using </> as a symbol, you can continue.";
                lErrors.ForeColor = Color.Red;
            }

            string status = cbStatus.Text;
            if (tbEnglishText.Text == tbJapaneseText.Text)
                status = "Editing";
            else if (tbEnglishText.Text != "")
                status = "Editing";

            if (tcType.Controls[tcType.SelectedIndex].Text == "Speaker")
            {
                CurrentSpeakerList[lbSpeaker.SelectedIndex].EnglishText = status == "To Do" ? null: tbEnglishText.Text;
                CurrentSpeakerList[lbSpeaker.SelectedIndex].Status = status;
                int? speakerId = CurrentSpeakerList[lbSpeaker.SelectedIndex].Id;
                Project.CurrentFolder.CurrentFile.CurrentSection.Entries.ForEach(x => x.SpeakerName = x.Id == speakerId ? x.SpeakerName = tbEnglishText.Text : x.SpeakerName);
            }
            else
            {
                if (tbEnglishText.Text.Length == 0)
                {
                    status = "To Do";
                    CurrentTextList[lbEntries.SelectedIndex].EnglishText = null;
                } else
                {
                    CurrentTextList[lbEntries.SelectedIndex].EnglishText = tbEnglishText.Text;
                }
                CurrentTextList[lbEntries.SelectedIndex].Status = status;
            }

            cbStatus.Text = status;
        }

        private void cbStatus_TextChanged(object sender, EventArgs e)
        {
            if ((cbStatus.Text != string.Empty) && (tcType.Controls[tcType.SelectedIndex].Text == "Speaker"))
                CurrentSpeakerList[lbSpeaker.SelectedIndex].Status = cbStatus.Text;
            else
                CurrentTextList[lbEntries.SelectedIndex].Status = cbStatus.Text;
            UpdateStatusData();

        }

        private void tbNoteText_TextChanged(object sender, EventArgs e)
        {
            if (lbEntries.SelectedIndex > -1 && lbEntries.SelectedIndex < CurrentTextList.Count)
            {
                CurrentTextList[lbEntries.SelectedIndex].Notes = tbNoteText.Text;
            }
        }

        private void lbEntries_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index >= CurrentTextList.Count)
                return;
            
            string text = GetTextBasedLanguage(e.Index, CurrentTextList);

            text = text == null ? "" : text;

            int nb = 0;
            if (CurrentTextList[e.Index].SpeakerId != null)
            {
                nb += 1;
            }
            
            nb += Regex.Matches(text, "\\r*\\n").Count;

            var size = (int)((nb + 1) * 14) + 6;

            e.ItemHeight = size;
        }

        private void cbFileList_DrawItem(object sender, DrawItemEventArgs e)
        {
            //Get the file selected
            if (Project.CurrentFolder.FileList().Count > 0)
            {
                string text = ((ComboBox)sender).Items[e.Index].ToString();
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Black), e.Bounds);
                }
                else
                {
                    var count = CurrentTextList.Count;
                    if (CurrentTextList.Count(x => x.Status == "Done") == count && count > 0)
                    {
                        SolidBrush backgroundBrush = new SolidBrush(Color.LightGreen);
                        e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
                    }
                    else
                    {
                        e.Graphics.FillRectangle(new SolidBrush(((Control)sender).BackColor), e.Bounds);
                    }
                }

                SolidBrush textBrush = new SolidBrush(e.ForeColor);
                e.Graphics.DrawString(text, ((Control)sender).Font, textBrush, e.Bounds, StringFormat.GenericDefault);

                textBrush.Dispose();
            }
        }

        private void cbSections_TextChanged(object sender, EventArgs e)
        {
        }

        private void hexToJapaneseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fHexToJapanese myForm = new fHexToJapanese();
            myForm.Show();
        }

        private void menuToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Extraction of Rebirth's files is in progress.\n You can still continue other work in the meantime");
            string successMessage = "Extraction of the files";
            PackingAssistant.CallPython(config.PythonLocation, Path.Combine(config.GetGameConfig("TOR").LastFolderPath, @"..\..\..\PythonLib"), "TOR", "unpack", $"Init --iso \"{config.GetGameConfig("TOR").IsoPath}\"", successMessage);
        }

        private void cbToDo_CheckedChanged(object sender, EventArgs e)
        {
            FilterEntryList();
        }

        private void cbProof_CheckedChanged(object sender, EventArgs e)
        {
            FilterEntryList();
        }

        private void cbDone_CheckedChanged(object sender, EventArgs e)
        {
            FilterEntryList();
        }

        private void cbProblematic_CheckedChanged(object sender, EventArgs e)
        {
            FilterEntryList();
        }

        private void cbInReview_CheckedChanged(object sender, EventArgs e)
        {
            FilterEntryList();
        }

        private void FilterEntryList()
        {
            UpdateDisplayedEntries();
            UpdateStatusData();
        }

        private void cbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbEntries.Invalidate();
        }

        private void cbSections_SelectedIndexChanged(object sender, EventArgs e)
        {
            Project.CurrentFolder.CurrentFile.SetSection(cbSections.SelectedItem.ToString());
            UpdateDisplayedEntries();
            UpdateStatusData();
        }

        private void fMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.Down:
                        if (e.Alt)
                        {
                            if (cbFileList.Items.Count - 1 != cbFileList.SelectedIndex)
                                cbFileList.SelectedIndex += 1;
                            break;
                        }

                        if (tcType.SelectedIndex == 0)
                        {
                            if (lbEntries.Items.Count - 1 != lbEntries.SelectedIndex)
                                lbEntries.SelectedIndex += 1;
                        }
                        else
                        {
                            if (lbSpeaker.Items.Count - 1 != lbSpeaker.SelectedIndex)
                                lbSpeaker.SelectedIndex += 1;
                        }
                        break;
                    case Keys.Up:
                        if (e.Alt)
                        {
                            if (cbFileList.SelectedIndex > 0)
                                cbFileList.SelectedIndex -= 1;
                            break;
                        }

                        if (tcType.SelectedIndex == 0)
                        {
                            if (lbEntries.SelectedIndex > 0)
                                lbEntries.SelectedIndex -= 1;
                        }
                        else
                        {
                            if (lbSpeaker.SelectedIndex > 0)
                                lbSpeaker.SelectedIndex -= 1;
                        }

                        break;
                    case Keys.L:
                        if (string.IsNullOrWhiteSpace(tbEnglishText.Text))
                            tbEnglishText.Text = tbJapaneseText.Text;
                        break;
                    case Keys.S:
                        bSave.PerformClick();
                        break;
                    case Keys.E:
                        if (cbEmpty.Enabled)
                            cbEmpty.Checked = !cbEmpty.Checked;
                        break;
                    default:
                        e.Handled = false;
                        return;
                }

                e.Handled = true;
            }
        }

        private string stripTags(string input)
        {
            string output = "";
            string pattern = @"(<[\w/]+:?\w+>)";
            string[] result = Regex.Split(input.Replace("\r", "").Replace("\n", ""), pattern, RegexOptions.IgnoreCase).Where(x => x != "").ToArray();

            string[] names = { "<Veigue>", "<Mao>", "<Eugene>", "<Annie>", "<Tytree>", "<Hilda>", "<Claire>", "<Agarte>", "<Annie (NPC)>", "<Leader>" };

            foreach (string element in result)
            {
                if (element[0] == '<')
                {
                    if (names.Contains(element))
                    {
                        output += element.Substring(1, element.Length - 2);
                    }

                    if (element.Contains("Unk") || element.Contains("04"))
                    {
                        output += "〇〇〇";
                    }

                    if (element.Contains("nmb"))
                    {
                        string el = element.Substring(5, element.Length - 7);
                        output += Convert.ToInt32(el, 16);
                    }
                }
                else
                {
                    output += element;
                }
            }

            return output;
        }

        private void lbEntries_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.SetText(stripTags(tbJapaneseText.Text));
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            var selectedFileType = cbFileType.Text;
            var selectedFile = cbFileList.Text;
            var selectedSection = cbSections.Text;
            var selectedLanguage = cbLanguage.Text;
            LoadFolder(Project.ProjectPath);

            DisableEventHandlers();
            Project.SetCurrentFolder(selectedFileType);
            cbFileType.Text = selectedFileType;
            Project.CurrentFolder.SetCurrentFile(selectedFile);
            cbFileList.DataSource = Project.CurrentFolder.FileList();
            cbFileList.Text = selectedFile;
            Project.CurrentFolder.CurrentFile.SetSection(selectedSection);
            cbSections.DataSource = Project.CurrentFolder.CurrentFile.GetSectionNames();
            cbSections.Text = selectedSection;
            cbLanguage.Text = selectedLanguage;
            lbEntries.DataSource = Project.CurrentFolder.CurrentFile.CurrentSection.Entries;
            lbSpeaker.DataSource = Project.CurrentFolder.CurrentFile.Speakers;
            EnableEventHandlers();
            
            UpdateDisplayedEntries();
            UpdateStatusData();
        }

        private void tsSetup_Click(object sender, EventArgs e)
        {
            fSetup setupForm = new fSetup( this, config, PackingAssistant);
            setupForm.Show();
        }

        private void lbSpeaker_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEntryData(lbSpeaker);
        }

        private void lbSpeaker_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index >= CurrentSpeakerList.Count)
                return;

            string text = GetTextBasedLanguage(e.Index, CurrentSpeakerList);

            int nb;
            if (string.IsNullOrEmpty(text))
                nb = 0;
            else
                nb = Regex.Matches(text, "\\r*\\n").Count;

            var size = (int)((nb + 1) * 14) + 6;

            e.ItemHeight = size;
        }

        private void bBrowse_Click(object sender, EventArgs e)
        {

        }

        private void tcType_Selected(object sender, TabControlEventArgs e)
        {
            if (Project == null)
                return;
            UpdateDisplayedEntries();
            UpdateStatusData();
        }

        private void extractIsoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Extraction of NDX's files is in progress.\n You can still continue other work in the meantime");
            string successMessage = "Extraction of the files";
            PackingAssistant.CallPython(config.PythonLocation, Path.Combine(config.GetGameConfig("NDX").LastFolderPath, @"..\..\..\PythonLib"), "NDX", "unpack", $"Init --iso \"{config.GetGameConfig("NDX").IsoPath}\"", successMessage);
        }

        private void cbEmpty_CheckedChanged(object sender, EventArgs e)
        {
            if (tcType.Controls[tcType.SelectedIndex].Text == "Text")
            {
                setEmpty(lbEntries);
            }
            else
            {
                setEmpty(lbSpeaker);
            }

        }

        private void setEmpty(ListBox lb)
        {
            XMLEntry e = (XMLEntry)lb.SelectedItem;
            if (cbEmpty.Checked)
            {
                e.EnglishText = "";
                e.Status = "Done";
                cbStatus.Text = "Done";
            }
            else
            {
                if (e.EnglishText != null && e.EnglishText.Length == 0)
                {
                    e.EnglishText = null;
                    e.Status = "To Do";
                    cbStatus.Text = "To Do";
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            pictureBox1.BackColor = Color.Transparent;
            if (fontAtlasImage != null)
            {
                Graphics graphics = e.Graphics;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                PointF location = new PointF(0.0f, 0.0f);

                // handle null text
                string text = (tbEnglishText.Text == null) ? "" : tbEnglishText.Text;

                foreach (char c in text)
                {
                    Rectangle charRect = GetCharacterRectangleFromAtlas(c, out int shift, out bool line);
                    RectangleF destRect = new RectangleF(location, new SizeF(charRect.Width, charRect.Height));
                    graphics.DrawImage(fontAtlasImage, destRect, charRect, GraphicsUnit.Pixel);
                    
                    if (line)
                    {
                        location.X = 0f;
                        location.Y += 24f;
                    }
                    else
                    {
                        location.X += charRect.Width - shift;
                    }
                }
                graphics.ResetTransform();
            }
        }

        private Rectangle GetCharacterRectangleFromAtlas(int character, out int s, out bool addline)
        {
            int charWidth = 24;
            int charHeight = 24;
            addline = false;

            // Calculate the index of the character in the font atlas
            int index;
            if (character >= 0x30 && character <= 0x39)
            {
                index = character - 0x2F;
            }
            else if (character >= 0x41 && character <= 0x5A)
            {
                index = character - 0x36;
            }
            else if (character >= 0x61 && character <= 0x7A)
            {
                index = character - 0x3C;
            }
            else switch (character)
            {
                case '\n':
                    index = 0;
                    addline = true;
                    break;
                case '!':
                    index = 69;
                    break;
                case ',':
                    index = 63;
                    break;
                case '/':
                    index = 70;
                    break;
                case '~':
                    index = 93;
                    break;
                case '_':
                    index = 94;
                    break;
                case '+':
                    index = 77;
                    break;
                case '*':
                    index = 85;
                    break;
                case '=':
                    index = 79;
                    break;
                case '(':
                    index = 71;
                    break;
                case ')':
                    index = 72;
                    break;
                case '[':
                    index = 73;
                    break;
                case ']':
                    index = 74;
                    break;
                case '{':
                    index = 75;
                    break;
                case '}':
                    index = 76;
                    break;
                case '-':
                    index = 78;
                    break;
                case '\'':
                    index = 89;
                    break;
                case '"':
                    index = 88;
                    break;
                case '.':
                    index = 64;
                    break;
                case ':':
                    index = 66;
                    break;
                case ';':
                    index = 67;
                    break;
                case '?':
                    index = 68;
                    break;
                case '<':
                    index = 80;
                    break;
                case '>':
                    index = 81;
                    break;
                default:
                    index = 0;
                    break;
            }

            // Calculate the position of the character in the atlas based on its index
            int y = index * charHeight;
            int x = glyphs[index].lskip;

            charWidth -= glyphs[index].lskip;
            s = glyphs[index].rskip;
            return new Rectangle(x, y, charWidth, charHeight);

        }
    }
}