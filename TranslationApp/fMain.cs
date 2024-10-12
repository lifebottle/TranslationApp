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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

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
        private static List<EntryFound> ListSearch;
        private static List<EntryFound> OtherTranslations;
        private static List<XMLEntry> ContextTranslations;
        private Dictionary<string, Color> ColorByStatus;
        private string gameName;
        private int nbJapaneseDuplicate;
        private static string windowName;
        FormWindowState LastWindowState = FormWindowState.Minimized;

        private readonly string MULTIPLE_STATUS = "<Multiple Status>";
        private readonly string MULTIPLE_SELECT = "<Multiple Entries Selected>";

        struct ProjectEntry
        {
            public string shortName, fullName, folder;
            public ProjectEntry(string sname, string fname, string dir)
            {
                shortName = sname;
                fullName = fname;
                folder = dir;
            }
        }

        private static readonly ProjectEntry[] Projects = new ProjectEntry[]
        {
            new ProjectEntry("NDX", "Narikiri Dungeon X", "2_translated"),
            new ProjectEntry("TOR", "Tales of Rebirth", "2_translated"),
            new ProjectEntry("TOH", "Tales of Hearts (DS)", "2_translated"),
            new ProjectEntry("RM2", "Tales of the World: Radiant Mythology 2", "2_translated"),
        };

        public fMain()
        {
            InitializeComponent();
            // Use reflection to allow spliters to ignore the Form size
            typeof(Splitter).GetField("minExtra", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(splitter1, -10000);
            typeof(Splitter).GetField("minExtra", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(splitter2, -10000);
            var gitInfo = Assembly.GetExecutingAssembly().GetType("GitVersionInformation");
            var ver = gitInfo.GetField("FullSemVer").GetValue(null);
            var sha = gitInfo.GetField("ShortSha").GetValue(null);
#if DEBUG
            Text = "Translation App v" + ver + " (commit: " + sha + ")";
#else
            Text = "Translation App v" + ver;
#endif
            windowName = Text;
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            cbLanguage.Text = "English (if available)";
            CreateColorByStatusDictionnary();
            PopulateProjectTypes();
            InitialiseStatusText();
            ChangeEnabledProp(false);

            config = new Config();
            config.Load();
            PackingAssistant = new PackingProject();
        }

        private void PopulateProjectTypes()
        {
            List<ToolStripMenuItem> items = new List<ToolStripMenuItem>();
            foreach (ProjectEntry pe in Projects)
            {
                var item = new ToolStripMenuItem();
                item.Name = "tsi" + pe.shortName;
                item.Text = pe.shortName;
                item.DropDownItems.Add(new ToolStripMenuItem("Open Last Folder") { Tag = pe });
                item.DropDownItems[0].Click += new EventHandler(LoadLastFolder_Click);
                item.DropDownItems.Add(new ToolStripMenuItem("Open New Folder") { Tag = pe });
                item.DropDownItems[1].Click += new EventHandler(LoadNewFolder_Click);
                items.Add(item);
            }
            translationToolStripMenuItem.DropDownItems.AddRange(items.ToArray());
        }

        private void LoadNewFolder_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            ProjectEntry pe = (ProjectEntry)clickedItem.Tag;
            LoadProjectFolder(pe.shortName, pe.folder);
            textPreview1.ChangeImage(pe.shortName);
            UpdateTitle(pe.fullName);
        }

        private void LoadLastFolder_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            ProjectEntry pe = (ProjectEntry)clickedItem.Tag;
            LoadLastFolder(pe.shortName);
            textPreview1.ChangeImage(pe.shortName);
            UpdateTitle(pe.fullName);
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string p = TryLoadFolder(GetFolderPath(), false);
            if (p != null)
            {
                UpdateTitle($"Single folder {Path.GetDirectoryName(p)}");
            }
        }

        private void UpdateTitle(string name)
        {
            if (Project.CurrentFolder == null || string.IsNullOrWhiteSpace(name))
            {
                Text = windowName;
            }
            else
            {
                Text = windowName + " | " + name;
            }
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
            tbFriendlyName.Enabled = status;
            tbSectionName.Enabled = status;
            tbNoteText.Enabled = status;
            tabSearchMass.Enabled = status;

            lbEntries.Enabled = status;

            //Checked List
            cbToDo.Enabled = status;
            cbEditing.Enabled = status;
            cbProof.Enabled = status;
            cbDone.Enabled = status;
            cbProblematic.Enabled = status;
            cbDone.Enabled = status;

            //Button
            bSaveAll.Enabled = status;
            btnRefresh.Enabled = status;
            btnSaveFile.Enabled = status;

            //Panel
            panelNb1.Enabled = status;
            panelNb2.Enabled = status;
        }

        private void DrawEntries(DrawItemEventArgs e, List<XMLEntry> EntryList, bool displaySection)
        {
            bool isSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);

            //Draw only if elements are present in the listbox
            if (e.Index > -1)
            {
                //Regardless of text, draw elements close together
                //and use the intmax size as per the docs
                TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;
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
                Point startPoint = new Point(3, e.Bounds.Y + 3);

                //0. Add Section if needed
                if (displaySection)
                {

                    EntryFound entryFound = ListSearch[e.Index];
                    string sectionDetail = $"{entryFound.Folder} - " +
                $"{Project.GetFolderByName(entryFound.Folder).XMLFiles[entryFound.FileId].Name} - " +
                $"{entryFound.Section} - {entry.Id}";

                    SolidBrush backgroundBrushSection = new SolidBrush(Color.LightGray);
                    Size mySize = TextRenderer.MeasureText(e.Graphics, sectionDetail, normalFont, proposedSize, flags);
                    e.Graphics.FillRectangle(backgroundBrushSection, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, 19);
                    TextRenderer.DrawText(e.Graphics, sectionDetail, boldFont, startPoint, Color.Black, flags);
                    startPoint.Y += 16;


                    e.Graphics.DrawLine(new Pen(Color.LightGray, 1.5f), new Point(0, startPoint.Y), new Point(e.Bounds.Width, startPoint.Y));
                    startPoint.Y += 3;
                }


                //1. Add Speaker name
                if (EntryList[e.Index].SpeakerId != null)
                {
                    TextRenderer.DrawText(e.Graphics, EntryList[e.Index].SpeakerName, boldFont, startPoint, tagColor, flags);
                    startPoint.Y += 13;
                }

                //2. Split based on the line breaks
                if (!string.IsNullOrEmpty(text))
                    DrawLines(e, text, ref startPoint, boldFont, tagColor, normalFont, regularColor, proposedSize, flags);


                // Clean up
                backgroundBrush.Dispose();
            }

            e.DrawFocusRectangle();
        }

        private void DrawSearchEntries(DrawItemEventArgs e, List<EntryFound> EntryList, bool highlightSearch)
        {
            bool isSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);

            //Draw only if elements are present in the listbox
            if (e.Index > -1)
            {
                //Regardless of text, draw elements close together
                //and use the intmax size as per the docs
                TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;
                Size proposedSize = new Size(int.MaxValue, int.MaxValue);

                //Grab the current entry to draw
                EntryFound entryFound = EntryList[e.Index];

                // Background item brush
                SolidBrush backgroundBrush = new SolidBrush(isSelected ? SystemColors.Highlight : ColorByStatus["To Do"]);

                // Text colors
                Color regularColor = e.ForeColor;
                Color hightlightSearch = Color.Orange;
                Color tagColor = isSelected ? Color.Orange : Color.Blue;

                // Draw the background
                e.Graphics.FillRectangle(backgroundBrush, e.Bounds);

                // Add separators for each entry
                e.Graphics.DrawLine(new Pen(Color.DimGray, 1.5f), new Point(0, e.Bounds.Bottom - 1), new Point(e.Bounds.Width, e.Bounds.Bottom - 1));
                e.Graphics.DrawLine(new Pen(Color.DimGray, 1.5f), new Point(0, e.Bounds.Top - 1), new Point(e.Bounds.Width, e.Bounds.Top - 1));

                Font normalFont = new Font("Arial", 8, FontStyle.Regular);
                Font boldFont = new Font("Arial", 8, FontStyle.Bold);

                string text = GetTextBasedLanguage(e.Index, EntryList.Select(x => x.Entry).ToList());
                Point startPoint = new Point(3, e.Bounds.Y + 3);

                //0. Add Section if needed
                string sectionDetail = $"{entryFound.Folder} - " +
            $"{Project.GetFolderByName(entryFound.Folder).XMLFiles[entryFound.FileId].Name} - " +
            $"{entryFound.Section} - {entryFound.Id}";

                SolidBrush backgroundBrushSection = new SolidBrush(Color.LightGray);
                Size mySize = TextRenderer.MeasureText(e.Graphics, sectionDetail, normalFont, proposedSize, flags);
                e.Graphics.FillRectangle(backgroundBrushSection, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, 19);
                TextRenderer.DrawText(e.Graphics, sectionDetail, boldFont, startPoint, Color.Black, flags);
                startPoint.Y += 16;


                e.Graphics.DrawLine(new Pen(Color.LightGray, 1.5f), new Point(0, startPoint.Y), new Point(e.Bounds.Width, startPoint.Y));
                startPoint.Y += 3;



                //1. Add Speaker name
                if (entryFound.Entry.SpeakerId != null)
                {
                    TextRenderer.DrawText(e.Graphics, entryFound.Entry.SpeakerName, boldFont, startPoint, tagColor, flags);
                    startPoint.Y += 13;
                }

                //2. Split based on the line breaks
                if (!string.IsNullOrEmpty(text))
                {

                    //Split based on searched item
                    string pattern = $@"({tbSearch.Text})";
                    List<string> result = Regex.Split(text, pattern, RegexOptions.IgnoreCase).Where(x => x != "").ToList();
                    List<Color> textColors = result.Select(x => x.Equals(tbSearch.Text, StringComparison.OrdinalIgnoreCase) ? Color.OrangeRed : e.ForeColor).ToList();
                    List<Font> textFont = result.Select(x => x.Equals(tbSearch.Text, StringComparison.OrdinalIgnoreCase) ? boldFont : normalFont).ToList();

                    for (int i = 0; i < result.Count; i++)
                    {
                        DrawLines(e, result[i], ref startPoint, normalFont, tagColor, textFont[i], textColors[i], proposedSize, flags);
                    }

                }

                // Clean up
                backgroundBrush.Dispose();
            }

            e.DrawFocusRectangle();

        }

        private void DrawLines(DrawItemEventArgs e, string text, ref Point startPoint, Font tagFont, Color tagColor, Font regularFont, Color regularColor, Size proposedSize, TextFormatFlags flags)
        {
            Size mySize;

            string[] lines = Regex.Split(text, "\\r*\\n", RegexOptions.IgnoreCase);

            //Starting point for drawing, a little offsetted
            //in order to not touch the borders
            //Point startPoint = new Point(3, e.Bounds.Y + 3);

            for (int i = 0; i < lines.Length; i++)
            {

                //3. Split based on the different tags
                //Split the text based on the Tags < xxx >
                string line = lines[i];
                string pattern = @"(<[\w/]+:?\w+>)";
                string[] result = Regex.Split(line, pattern, RegexOptions.IgnoreCase).Where(x => x != "").ToArray();
                //We need to loop over each element to adjust the color
                foreach (string element in result)
                {
                    if (element[0] == '<')
                    {
                        mySize = TextRenderer.MeasureText(e.Graphics, element, tagFont, proposedSize, flags);

                        TextRenderer.DrawText(e.Graphics, element, tagFont, startPoint, tagColor, flags);
                        startPoint.X += mySize.Width;
                    }
                    else
                    {
                        mySize = TextRenderer.MeasureText(e.Graphics, element, regularFont, proposedSize, flags);

                        TextRenderer.DrawText(e.Graphics, element, regularFont, startPoint, regularColor, flags);
                        startPoint.X += mySize.Width;
                    }
                }

                if (i < lines.Length - 1)
                {
                    startPoint.Y += 13;
                    startPoint.X = 3;
                }
            }
        }


        //Draw entries with multiline and font color changed
        private void lbEntries_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawEntries(e, CurrentTextList, false);
        }

        private void lbSpeaker_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawEntries(e, CurrentSpeakerList, false);
        }

        private void ShowOtherTranslations()
        {
            if (tbJapaneseText.Text != "")
            {
                string translation = string.IsNullOrEmpty(tbEnglishText.Text) ? tbJapaneseText.Text : tbEnglishText.Text;
                translation = translation.Replace("\r\n", "\n");
                List<EntryFound> Entryfound = FindOtherTranslations("All", tbJapaneseText.Text.Replace("\r\n", "\n"), "Japanese", true, false, false);
                OtherTranslations = Entryfound.Where(x => x.Entry.JapaneseText == tbJapaneseText.Text && x.Entry.EnglishText != translation).ToList();

                string cleanedString = tbEnglishText.Text.Replace("\r\n", "").Replace(" ", "");
                List<EntryFound> DifferentLineBreak = Entryfound.Where(x => x.Entry.EnglishText != null).
                    Where(x => x.Entry.EnglishText.Replace("\n", "").Replace(" ", "") == cleanedString && x.Entry.EnglishText != translation).ToList();
                DifferentLineBreak.ForEach(x => x.Category = "Linebreak");

                OtherTranslations.AddRange(DifferentLineBreak);
                lNbOtherTranslations.ForeColor = OtherTranslations.Count > 0 ? Color.Red : Color.Green;
                lLineBreak.ForeColor = DifferentLineBreak.Count > 0 ? Color.Red : Color.Green;
                int distinctCount = OtherTranslations.Select(x => x.Entry.EnglishText).Distinct().Count();

                if (nbJapaneseDuplicate > 0)
                {
                    lNbOtherTranslations.Text = $"({distinctCount} other/missing translation(s) found)";
                    lLineBreak.Text = $"({DifferentLineBreak.Count} linebreak(s) different found)";
                }
                else
                {
                    lNbOtherTranslations.Text = "";
                    lLineBreak.Text = "";
                }

                lbDistinctTranslations.DataSource = OtherTranslations.Select(x => $"{x.Folder} - " +
                $"{Project.GetFolderByName(x.Folder).XMLFiles[Convert.ToInt32(x.FileId)].Name} - " +
                $"{x.Section} - {x.Entry.EnglishText}").ToList();
            }
        }
        private void lbEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEntryData(lbEntries);
            ShowOtherTranslations();
        }

        private List<EntryFound> FindOtherTranslations(string folderSearch, string textToFind, string language, bool matchWholeEntry, bool matchCase, bool matchWholeWord)
        {
            List<EntryFound> res = new List<EntryFound>();
            if (folderSearch != "All")
            {
                XMLFolder folder = Project.XmlFolders.Where(x => String.Equals(x.Name, folderSearch, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (folder != null)
                {
                    res = folder.SearchJapanese(textToFind, matchWholeEntry, matchCase, matchWholeWord, language);
                }
            }
            else
            {
                foreach (XMLFolder folder in Project.XmlFolders)
                {
                    res.AddRange(folder.SearchJapanese(textToFind, matchWholeEntry, matchCase, matchWholeWord, language));
                }
            }
            return res;
        }

        private void LoadEntryData(ListBox lb)
        {
            tbEnglishText.TextChanged -= tbEnglishText_TextChanged;

            if (lb.SelectedIndices.Count > 1)
            {
                tbJapaneseText.Text = MULTIPLE_SELECT;
                tbJapaneseText.Enabled = false;

                tbEnglishText.Text = MULTIPLE_SELECT;
                tbEnglishText.Enabled = false;
                string st = ((XMLEntry)lb.SelectedItems[0]).Status;
                foreach (XMLEntry e in lb.SelectedItems)
                {
                    if (st != e.Status)
                    {
                        if (!cbStatus.Items.Contains(MULTIPLE_STATUS))
                        {
                            cbStatus.Items.Add(MULTIPLE_STATUS);
                        }
                        cbStatus.Text = MULTIPLE_STATUS;
                        return;
                    }
                }
                cbStatus.Text = st;
            }
            else
            {
                if (cbStatus.Items.Contains(MULTIPLE_STATUS))
                {
                    cbStatus.Items.Remove(MULTIPLE_STATUS);
                }

                tbJapaneseText.Text = string.Empty;
                tbEnglishText.Text = string.Empty;

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

                TranslationEntry TranslationEntry;
                nbJapaneseDuplicate = 0;
                if (currentEntry.JapaneseText == null)

                    TranslationEntry = new TranslationEntry { EnglishTranslation = "" };
                else
                {
                    foreach (XMLFolder folder in Project.XmlFolders)
                    {
                        folder.Translations.TryGetValue(currentEntry.JapaneseText, out TranslationEntry);

                        if (TranslationEntry != null)
                            nbJapaneseDuplicate += TranslationEntry.Count;
                    }
                    nbJapaneseDuplicate -= 1;
                }


                if (nbJapaneseDuplicate > 0)
                    lblJapanese.Text = $@"Japanese ({nbJapaneseDuplicate} duplicate(s) found)";
                else
                    lblJapanese.Text = $@"Japanese";

                if (currentEntry.JapaneseText != null)
                    tbJapaneseText.Text = currentEntry.JapaneseText.Replace("\r", "").Replace("\n", Environment.NewLine);
                if (currentEntry.EnglishText != null)
                    tbEnglishText.Text = currentEntry.EnglishText.Replace("\r", "").Replace("\n", Environment.NewLine);
                if (tbNoteText != null)
                    tbNoteText.Text = currentEntry.Notes;

                cbEmpty.Checked = currentEntry.EnglishText?.Equals("") ?? false;

                cbStatus.Text = currentEntry._Status; // Need the modified name (bandaid)
            }
            textPreview1.ReDraw(tbEnglishText.Text);
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
            p.X = tbJapaneseText.Location.X + ((tbJapaneseText.Size.Width / trackBarAlign.Maximum) * trackBarAlign.Value);
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
                return myEntry.EnglishText == null ? myEntry.JapaneseText : myEntry.EnglishText;
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

        private void LoadProjectFolder(string gameName, string path)
        {
            lbEntries.BorderStyle = BorderStyle.FixedSingle;
            var loadedFolder = TryLoadFolder(Path.Combine(GetFolderPath(), path), gameName.Equals("NDX"));
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
                TryLoadFolder(config.GetGameConfig(gameName).FolderPath, gameName.Equals("NDX"));
                gameConfig = myConfig;
                UpdateOptionsVisibility();
            }
            else
                MessageBox.Show("The game you are trying to load is not inside the configuration file,\nplease load a new folder.");
        }

        public string TryLoadFolder(string path, bool legacy)
        {
            if (Directory.Exists(path))
            {
                LoadFolder(path, legacy);
                return path;
            }

            MessageBox.Show("Are you sure you selected the right folder?\n" +
                            "The folder you choose doesn't represent a valid Tales of Repo.\n" +
                            $"It should have the Data folder in it.\nPath {path} not valid.");
            return null;
        }

        private void LoadFolder(string path, bool legacy)
        {
            DisableEventHandlers();

            var folderIncluded = new List<string>();
            foreach (string p in Directory.GetDirectories(path))
            {
                folderIncluded.Add(new DirectoryInfo(p).Name);
            }

            Project = new TranslationProject(path, folderIncluded, legacy);

            if (Project.CurrentFolder == null)
            {
                MessageBox.Show("Are you sure you selected the right folder?\n" +
                            "The folder you have chosen doesn't contain any subfolders\n" +
                            "or they are empty, please try again.");
                return;
            }

            CurrentTextList = Project.CurrentFolder.CurrentFile.CurrentSection.Entries;
            CurrentSpeakerList = Project.CurrentFolder.CurrentFile.Speakers;
            cbFileType.DataSource = Project.GetFolderNames().OrderByDescending(x => x).ToList();
            cbFileList.DataSource = Project.CurrentFolder.FileList();
            cbSections.DataSource = Project.CurrentFolder.CurrentFile.GetSectionNames();
            cbFileList.SelectedIndex = 0;

            UpdateDisplayedEntries();
            UpdateStatusData();

            ChangeEnabledProp(true);
            EnableEventHandlers();
            cbFileType.Text = "___";
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

        private List<XMLEntry> getSkitNameList()
        {
            XMLFile slps = Project.XmlFolders[1].XMLFiles.FirstOrDefault(x => x.Name == "SLPS");
            XMLSection section = slps?.Sections.FirstOrDefault(x => x.Name.StartsWith("Skit"));
            List<XMLEntry> r = section?.Entries;
            return r ?? new List<XMLEntry>();
        }

        private void cbFileType_TextChanged(object sender, EventArgs e)
        {
            if (cbFileType.SelectedItem.ToString() != string.Empty)
            {
                Project.SetCurrentFolder(cbFileType.SelectedItem.ToString());
                List<string> filelist = Project.CurrentFolder.FileList();

                if (cbFileType.SelectedItem.ToString().Equals("Menu", StringComparison.InvariantCultureIgnoreCase))
                {
                    cbFileList.DataSource = filelist;
                }
                else if (cbFileType.SelectedItem.ToString() == "Skits")
                {
                    List<XMLEntry> names = getSkitNameList();
                    if (names.Count != 1157)
                    {
                        cbFileList.DataSource = filelist.Select(x => x + ".xml").ToList();
                    }
                    else
                    {
                        for (int i = 0, j = 0; i < filelist.Count; i++)
                        {
                            if (((i > 1072) && (i < 1082)) || ((i > 1092) && (i < 1099)))
                            {
                                j++;
                                filelist[i] += " | NO NAME";
                                continue;
                            }
                            filelist[i] = filelist[i] + " | " + (names[i - j].EnglishText ?? names[i - j].JapaneseText);
                        }
                        cbFileList.DataSource = filelist;
                    }
                }
                else
                {
                    for (int i = 0; i < filelist.Count; i++)
                    {
                        string fname = Project.CurrentFolder.XMLFiles[i].FriendlyName ?? "NO NAME";
                        filelist[i] = filelist[i] + " | " + fname;
                    }
                    cbFileList.DataSource = filelist;
                }


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
                if (lbEntries.SelectedIndices.Count == 1)
                {
                    if (lbEntries.Items.Count > old_index)
                    {
                        lbEntries.SelectedIndices.Clear();
                        lbEntries.SelectedIndices.Add(old_index);
                    }
                }
                LoadEntryData(lbEntries);
            }
            else
            {
                var speakers = Project.CurrentFolder.CurrentFile.Speakers;
                if (speakers != null)
                {
                    CurrentSpeakerList = speakers.Where(e => checkedFilters.Contains(e.Status)).ToList();
                }
                else
                {
                    CurrentSpeakerList = new List<XMLEntry>();
                }
                var old_index = lbSpeaker.SelectedIndex;
                lbSpeaker.DataSource = CurrentSpeakerList;
                if (lbSpeaker.SelectedIndices.Count == 1)
                {
                    if (lbSpeaker.Items.Count > old_index)
                    {
                        lbSpeaker.SelectedIndices.Clear();
                        lbSpeaker.SelectedIndices.Add(old_index);
                    }
                }
                LoadEntryData(lbSpeaker);
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
            lNbProof.Text = (statusStats["Proofread"]).ToString();
            lNbProb.Text = (statusStats["Problematic"]).ToString();
            lNbEditing.Text = (statusStats["Edited"]).ToString();
            lNbDone.Text = (statusStats["Done"]).ToString();

            Dictionary<string, int> sectionStatusStats = new Dictionary<string, int>();
            if (tcType.SelectedTab.Text == "Speaker")
                sectionStatusStats = speakerStatusStats;
            else
                sectionStatusStats = Project.CurrentFolder.CurrentFile.CurrentSection.GetStatusData();
            //Section Count of status
            lNbToDoSect.Text = sectionStatusStats["To Do"].ToString();
            lNbProofSect.Text = sectionStatusStats["Proofread"].ToString();
            lNbProbSect.Text = sectionStatusStats["Problematic"].ToString();
            lNbEditingSect.Text = sectionStatusStats["Edited"].ToString();
            lNbDoneSect.Text = sectionStatusStats["Done"].ToString();
        }

        private void cbFileList_TextChanged(object sender, EventArgs e)
        {
            if (cbFileList.SelectedIndex != -1)
            {
                Project.CurrentFolder.SetCurrentFile(cbFileList.SelectedIndex);

                string filetype = cbFileType.SelectedItem.ToString();
                if (filetype.Equals("Menu", StringComparison.InvariantCultureIgnoreCase))
                {
                    tbFriendlyName.Enabled = false;
                    tbFriendlyName.Text = cbFileList.Text;
                }
                else
                {
                    tbFriendlyName.Enabled = true;
                    tbFriendlyName.Text = Project.CurrentFolder.CurrentFile.FriendlyName ?? "";
                }

                var old_section = cbSections.SelectedItem.ToString();

                cbSections.DataSource = Project.CurrentFolder.CurrentFile.GetSectionNames();

                if (cbSections.Items.Contains(old_section))
                    cbSections.SelectedItem = old_section;
                CurrentTextList = Project.CurrentFolder.CurrentFile.CurrentSection.Entries;
                CurrentSpeakerList = Project.CurrentFolder.CurrentFile.Speakers;
                FilterEntryList();

                bSaveAll.Enabled = true;
            }
        }

        private void NDXToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
        }

        private void tbEnglishText_TextPasted(object sender, ClipboardEventArgs e)
        {
            tbEnglishText.Paste(e.ClipboardText.Replace("\r", "").Replace("\n", Environment.NewLine));
        }

        private void tbEnglishText_TextChanged(object sender, EventArgs e)
        {

            bool error = (tbEnglishText.Text.Count(x => x == '<') == tbEnglishText.Text.Count(x => x == '>'));

            lErrors.Text = "";
            if (!error)
            {
                lErrors.Text = "Warning: Missing '<' or '>' in tag.";
                lErrors.ForeColor = Color.Red;
            }

            string status = cbStatus.Text;
            if (tbEnglishText.Text == tbJapaneseText.Text)
                status = "Edited";
            else if (tbEnglishText.Text != "")
                status = "Edited";

            if (tcType.Controls[tcType.SelectedIndex].Text == "Speaker")
            {
                CurrentSpeakerList[lbSpeaker.SelectedIndex].EnglishText = status == "To Do" ? null : tbEnglishText.Text;
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
                }
                else
                {
                    CurrentTextList[lbEntries.SelectedIndex].EnglishText = tbEnglishText.Text;
                }
                CurrentTextList[lbEntries.SelectedIndex].Status = status;
            }

            cbStatus.Text = status;
            textPreview1.ReDraw(tbEnglishText.Text);
        }

        private void cbStatus_TextChanged(object sender, EventArgs e)
        {
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
            if (Project?.CurrentFolder.FileList().Count > 0)
            {
                string text = ((ComboBox)sender).Items[e.Index].ToString();
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Black), e.Bounds);
                }
                else
                {
                    var count = CurrentTextList.Count;
                    var sdata = Project.CurrentFolder.XMLFiles[e.Index].GetStatusData();
                    if (sdata["Problematic"] != 0)
                    {
                        SolidBrush backgroundBrush = new SolidBrush(ColorByStatus["Problematic"]);
                        e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
                    }
                    else if (sdata["To Do"] > 0)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(((Control)sender).BackColor), e.Bounds);
                    }
                    else if (sdata["Edited"] > 0)
                    {
                        SolidBrush backgroundBrush = new SolidBrush(ColorByStatus["Editing"]);
                        e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
                    }
                    else if (sdata["Proofread"] > 0)
                    {
                        SolidBrush backgroundBrush = new SolidBrush(ColorByStatus["Proofreading"]);
                        e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
                    }
                    else
                    {
                        SolidBrush backgroundBrush = new SolidBrush(ColorByStatus["Done"]);
                        e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
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
            string item = cbSections.SelectedItem.ToString();
            Project.CurrentFolder.CurrentFile.SetSection(item);

            if (cbSections.SelectedIndex < 1)
            {
                tbSectionName.Enabled = false;
            }
            else
            {
                tbSectionName.Enabled = true;
            }
            tbSectionName.Text = cbSections.Text;
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
                            {
                                int idx = lbEntries.SelectedIndex;
                                lbEntries.ClearSelected();
                                lbEntries.SelectedIndex = idx + 1;
                            }
                        }
                        else
                        {
                            if (lbSpeaker.Items.Count - 1 != lbSpeaker.SelectedIndex)
                            {
                                int idx = lbSpeaker.SelectedIndex;
                                lbSpeaker.ClearSelected();
                                lbSpeaker.SelectedIndex = idx + 1;

                            }
                        }
                        tbEnglishText.Select();
                        tbEnglishText.SelectionStart = tbEnglishText.Text.Length;
                        tbEnglishText.SelectionLength = 0;
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
                            {
                                int idx = lbEntries.SelectedIndex;
                                lbEntries.ClearSelected();
                                lbEntries.SelectedIndex = idx - 1;
                            }
                        }
                        else
                        {
                            if (lbSpeaker.SelectedIndex > 0)
                            {
                                int idx = lbSpeaker.SelectedIndex;
                                lbSpeaker.ClearSelected();
                                lbSpeaker.SelectedIndex = idx - 1;
                            }
                        }
                        tbEnglishText.Select();
                        tbEnglishText.SelectionStart = tbEnglishText.Text.Length;
                        tbEnglishText.SelectionLength = 0;
                        break;
                    case Keys.L:
                        if (string.IsNullOrWhiteSpace(tbEnglishText.Text))
                            tbEnglishText.Text = tbJapaneseText.Text;
                        break;
                    case Keys.S:
                        bSaveAll.PerformClick();
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

                    if (element.Contains("unk") || element.Contains("var"))
                    {
                        output += "***";
                    }

                    if (element.Contains("nmb"))
                    {
                        string el = element.Substring(5, element.Length - 6);
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
                List<string> st = new List<string>();
                ListBox curr_lb = (tcType.SelectedIndex == 0) ? lbEntries : lbSpeaker;

                if (curr_lb.SelectedIndices.Count > 1)
                {
                    foreach (XMLEntry et in curr_lb.SelectedItems)
                    {
                        st.Add(stripTags(et.JapaneseText));
                    }
                    Clipboard.SetText(string.Join("\n", st));
                }
                else
                {
                    Clipboard.SetText(stripTags(tbJapaneseText.Text));
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            DisableEventHandlers();
            Project.CurrentFolder.XMLFiles[cbFileList.SelectedIndex] = Project.CurrentFolder.LoadXML(Project.CurrentFolder.CurrentFile.FilePath);
            Project.CurrentFolder.InvalidateTranslations();
            EnableEventHandlers();

            UpdateDisplayedEntries();
            UpdateStatusData();
        }

        private void tsSetup_Click(object sender, EventArgs e)
        {
            fSetup setupForm = new fSetup(this, config, PackingAssistant);
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
            if (cbEmpty.Checked)
            {
                foreach (XMLEntry e in lb.SelectedItems)
                {
                    e.EnglishText = "";
                    e.Status = "Done";
                    cbStatus.Text = "Done";
                }
            }
            else
            {
                foreach (XMLEntry e in lb.SelectedItems)
                {
                    if (e.EnglishText != null && e.EnglishText.Length == 0)
                    {
                        e.EnglishText = null;
                        e.Status = "To Do";
                        cbStatus.Text = "To Do";
                    }
                }
            }
        }


        private void cbStatus_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if ((cbStatus.Text == string.Empty) || (cbStatus.Text == MULTIPLE_STATUS))
                return;
            if (cbStatus.Items.Contains(MULTIPLE_STATUS))
            {
                cbStatus.Items.Remove(MULTIPLE_STATUS);
            }
            ListBox lb;
            if (tcType.Controls[tcType.SelectedIndex].Text == "Text")
            {
                lb = lbEntries;
            }
            else
            {
                lb = lbSpeaker;
            }
            foreach (XMLEntry entry in lb.SelectedItems)
            {
                entry.Status = cbStatus.Text;
            }
            UpdateStatusData();
        }


        private void btnRename_Click(object sender, EventArgs e)
        {
            // Project.CurrentFolder.CurrentFile.Sections[]
        }
        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            Project.CurrentFolder.CurrentFile.SaveToDisk();
            UpdateDisplayedEntries();
            UpdateStatusData();
        }
        private void saveCurrentFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        private void reloadCurrentFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.CurrentFolder.CurrentFile.SaveToDisk();
            UpdateDisplayedEntries();
            UpdateStatusData();
        }
        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.XmlFolders.ForEach(f => f.XMLFiles.ForEach(x => x.SaveToDisk()));
            MessageBox.Show("Text has been written to the XML files");
            UpdateDisplayedEntries();
            UpdateStatusData();
        }
        private void reloadAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedFileType = cbFileType.Text;
            var selectedFile = cbFileList.Text;
            var selectedSection = cbSections.Text;
            var selectedLanguage = cbLanguage.Text;
            LoadFolder(Project.ProjectPath, Project.isLegacy);
            DisableEventHandlers();
            Project.SetCurrentFolder(selectedFileType);
            cbFileType.Text = selectedFileType;
            if (cbFileList.SelectedIndex > -1)
            {
                Project.CurrentFolder.SetCurrentFile(cbFileList.SelectedIndex);
            }
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

        private void tbFriendlyName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int sindex = cbSections.SelectedIndex;
                int tindex = lbEntries.SelectedIndex;
                int findex = cbFileList.SelectedIndex;
                Project.CurrentFolder.CurrentFile.FriendlyName = tbFriendlyName.Text;
                cbFileType.Text = "___";
                cbFileList.SelectedIndex = findex;
                cbSections.SelectedIndex = sindex;
                lbEntries.SelectedIndices.Clear();
                lbEntries.SelectedIndex = tindex;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
        private void tbSectionName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int sindex = cbSections.SelectedIndex;
                int tindex = lbEntries.SelectedIndex;
                Project.CurrentFolder.CurrentFile.Sections[cbSections.SelectedIndex].Name = tbSectionName.Text;
                cbFileList.Text = "___";
                cbSections.SelectedIndex = sindex;
                lbEntries.SelectedIndices.Clear();
                lbEntries.SelectedIndex = tindex;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void exportFileToCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                string fname = saveDialog.FileName;
                Project.CurrentFolder.CurrentFile.SaveAsCsv(fname);
                MessageBox.Show("File exported");
            }
        }

        private void importFromCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not implemented yet");
        }

        private void setFileAsDoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (XMLSection s in Project.CurrentFolder.CurrentFile.Sections.Where(s => s.Name != "All strings"))
            {
                foreach (XMLEntry entry in s.Entries)
                {
                    entry.Status = "Done";
                }
            }
            cbFileList.Text = "___";
        }

        private void setSectionAsDoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cbSections.SelectedIndex == 0)
            {
                return;
            }

            foreach (XMLEntry entry in Project.CurrentFolder.CurrentFile.Sections[cbSections.SelectedIndex].Entries)
            {
                entry.Status = "Done";
            }
            cbFileList.Text = "___";
        }

        private void bSearch_Click(object sender, EventArgs e)
        {
            string textToFind = tbSearch.Text.Replace("\r\n", "\n");
            ListSearch = FindOtherTranslations(cbFileKindSearch.Text, textToFind, cbLangSearch.Text, cbExact.Checked, cbCase.Checked, cbMatchWhole.Checked);

            lEntriesFound.Text = $"Entries Found ({ListSearch.Count} entries)";
            lbSearch.DataSource = ListSearch.Select(x => $"{x.Folder} - " +
            $"{Project.GetFolderByName(x.Folder).XMLFiles[Convert.ToInt32(x.FileId)].Name} - " +
            $"{x.Section} - {x.Id}").ToList();
        }

        private void lNbOtherTranslations_Click(object sender, EventArgs e)
        {
            tabSearchMass.SelectedIndex = 1;
        }


        private void lbMassReplace_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawEntries(e, OtherTranslations.Select(x => x.Entry).ToList(), false);
        }

        private void lbSearch_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index >= ListSearch.Count)
                return;

            string text = GetTextBasedLanguage(e.Index, ListSearch.Select(x => x.Entry).ToList());

            text = text == null ? "" : text;

            int nb = 2;
            if (ListSearch[e.Index].Entry.SpeakerId != null)
            {
                nb += 1;
            }

            nb += Regex.Matches(text, "\\r*\\n").Count;

            var size = (int)((nb + 1) * 14) + 6;

            e.ItemHeight = size;
        }

        private void lbSearch_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawSearchEntries(e, ListSearch, true);
        }

        private void lbDistinctTranslations_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawSearchEntries(e, OtherTranslations, false);
        }

        private void lbDistinctTranslations_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index >= OtherTranslations.Count)
                return;

            string text = GetTextBasedLanguage(e.Index, OtherTranslations.Select(x => x.Entry).ToList());

            text = text == null ? "" : text;

            int nb = 2;
            if (OtherTranslations[e.Index].Entry.SpeakerId != null)
            {
                nb += 1;
            }

            nb += Regex.Matches(text, "\\r*\\n").Count;

            var size = (int)((nb + 1) * 14) + 6;

            e.ItemHeight = size;
        }

        private void lbDistinctTranslations_SelectedIndexChanged(object sender, EventArgs e)
        {
            //OtherTranslations[0].
            EntryFound entry = OtherTranslations[lbDistinctTranslations.SelectedIndex];
            int folderId = Project.GetFolderId(entry.Folder);
            List<XMLEntry> entries = Project.XmlFolders[folderId].XMLFiles[entry.FileId].Sections.Where(x => x.Name == entry.Section).First().Entries;

            List<int?> idList = new List<int?>();
            if (entry.Id > 0)
                idList.Add(entry.Id - 1);

            idList.Add(entry.Id);

            if (entry.Id < entries.Count - 1)
                idList.Add(entry.Id + 1);

            entries = entries.Where(x => idList.Contains(x.Id)).ToList();

            ContextTranslations = entries;
            lbContext.DataSource = ContextTranslations;
        }

        private void lbContext_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawEntries(e, ContextTranslations, false);
        }

        private void lbContext_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index >= ContextTranslations.Count)
                return;

            string text = GetTextBasedLanguage(e.Index, ContextTranslations);

            text = text == null ? "" : text;

            int nb = 2;
            if (ContextTranslations[e.Index].SpeakerId != null)
            {
                nb += 1;
            }

            nb += Regex.Matches(text, "\\r*\\n").Count;

            var size = (int)((nb + 1) * 14) + 6;

            e.ItemHeight = size;
        }

        private void lbSearch_Click(object sender, EventArgs e)
        {
            if (!(cbDone.Checked && cbDone.Checked && cbProblematic.Checked && cbEditing.Checked && cbToDo.Checked && cbProof.Checked))
            {
                cbToDo.Checked = true;
                cbProof.Checked = true;
                cbEditing.Checked = true;
                cbProblematic.Checked = true;
                cbDone.Checked = true;
            }

            if (ListSearch != null)
            {
                if (cbDone.Checked && cbDone.Checked && cbProblematic.Checked && cbEditing.Checked && cbToDo.Checked && cbProof.Checked)
                {


                    EntryFound eleSelected = ListSearch[lbSearch.SelectedIndex];
                    cbFileType.Text = eleSelected.Folder;
                    cbFileList.SelectedIndex = eleSelected.FileId;


                    if (eleSelected.Section == "Speaker")
                    {
                        lbSpeaker.ClearSelected();
                        tcType.SelectedIndex = 1;
                        lbSpeaker.SelectedIndex = eleSelected.Id;
                    }
                    else
                    {


                        lbEntries.ClearSelected();
                        cbSections.Text = "All strings";
                        tcType.SelectedIndex = 0;
                        lbEntries.SelectedIndex = CurrentTextList.FindIndex(x => x.Id == eleSelected.Id);

                    }


                }
            }
        }

        private void splitter2_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void fMain_Resize(object sender, EventArgs e)
        {
            if (WindowState != LastWindowState)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    leftColumn.Size = new Size((int)(ClientSize.Width * 0.3f), leftColumn.Height);
                    middleColumn.Size = new Size((int)(ClientSize.Width * 0.4f), middleColumn.Height);
                    //rightColumn.Size = new Size((int)(ClientSize.Width * 0.3f), rightColumn.Height);
                }
                else if(WindowState == FormWindowState.Normal)
                {
                    leftColumn.Size = new Size((int)(ClientSize.Width * 0.3f), leftColumn.Height);
                    middleColumn.Size = new Size((int)(ClientSize.Width * 0.35f), middleColumn.Height);
                    //rightColumn.Size = new Size((int)(ClientSize.Width * 0.3f), rightColumn.Height);
                }
                LastWindowState = WindowState;
            }
        }
    }

}