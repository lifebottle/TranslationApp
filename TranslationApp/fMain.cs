using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;

namespace TranslationApp
{
    public partial class fMain : Form
    {

        public Dictionary<string, Type> dictFileType = new Dictionary<string, Type>();
        public Entry currentEntry;
        public Struct currentStruct;
        public int nbTags = 0;
        private string gameName;
        public string basePath;

        private Dictionary<int, int> dictItemSize = new Dictionary<int, int>();
        private Dictionary<string, bool> dictStatus = new Dictionary<string, bool>();
        private Dictionary<string, Color> dictColor = new Dictionary<string, Color>();
        private List<Entry> listEntries = new List<Entry>();
        public List<TalesFile> listFileXML = new List<TalesFile>();

        public fMain()
        {
            InitializeComponent();

            
        }

        private void fMain_Load(object sender, EventArgs e)
        {



            dictStatus["To Do"] = true;

            

            cbLanguage.Text = "English (if available)";
            dictFileType.Add("TORStory", typeof(TORStory));
            dictFileType.Add("TORMenu", typeof(Menu));

            dictColor["To Do"]          = Color.White;
            dictColor["Proofreading"]   = Color.FromArgb(255, 152, 228);
            dictColor["In Review"]      = Color.Purple;
            dictColor["Problematic"]    = Color.Orange;
            dictColor["Done"]           = Color.Green;

            lNbToDo.Text = "";
            lNbReview.Text = "";
            lNbProb.Text = "";
            lNbProof.Text = "";
            lNbDone.Text = "";

            lNbToDoSect.Text = "";
            lNbProbSect.Text = "";
            lNbReviewSect.Text = "";
            lNbProofSect.Text = "";
            lNbDoneSect.Text = "";
            changeEnabledProp(false);
        }

        private void changeEnabledProp(bool status)
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
            cbInReview.Enabled = status;
            cbProof.Enabled = status;
            cbDone.Enabled = status;
            cbProblematic.Enabled = status;
            cbDone.Enabled = status;

            //Button
            bSave.Enabled = status;
            bRefresh.Enabled = status;

            //Panel
            panelNb1.Enabled = status;
            panelNb2.Enabled = status;
        }
        //Draw entries with multiline and font color changed
        private void lbEntries_DrawItem(object sender, DrawItemEventArgs e)
        {
            bool isSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);

  
            //Draw only if elements are present in the listbox
            if (e.Index > -1)
            {

                //Grab the current entry to draw
                Entry entry = listEntries[e.Index];

                //Create a color for the background and text
                Color backgroundColor;
                Color textColor = Color.White;
                if (isSelected)
                {
                    backgroundColor = SystemColors.Highlight;
                }
                else
                {
                    backgroundColor = dictColor[entry.Status];
                    //textColor       = 
                }


                // Background item brush
                Color color = isSelected ? SystemColors.Highlight :
                    e.Index % 2 == 0 ? Color.WhiteSmoke : Color.White;
                SolidBrush backgroundBrush = new SolidBrush(color);

                // Text color brush
                SolidBrush textBrush = new SolidBrush(e.ForeColor);
                SolidBrush blueBrush = new SolidBrush(Color.Blue);

                // Draw the background
                e.Graphics.FillRectangle(backgroundBrush, e.Bounds);

                Font normalFont = new Font("Arial", 8, FontStyle.Regular);
                Font boldFont = new Font("Arial", 8, FontStyle.Bold);
                
                string text = getTextBasedLanguage(e.Index);
              

                //1. Split based on the line breaks
                // We will need to increase the baseY after each line
                int baseX = 0;
                int baseY = e.Bounds.Y;
                if (text != "")
                {
                    string[] lines = Regex.Split(text, "\\r*\\n", RegexOptions.IgnoreCase);

                    foreach (string line in lines)
                    {
                        //2. Split based on the different tags
                        //Split the text based on the Tags < xxx >
                        string pattern = @"(<\w+:?\w+>)";
                        string[] result = Regex.Split(line, pattern, RegexOptions.IgnoreCase).Where(x => x != "").ToArray();

                        //We need to loop over each element to adjust the color
                        Size mySize = TextRenderer.MeasureText(line, normalFont);
                     
                        foreach (string element in result)
                        {

                            if (element[0] == '<')
                            {
                                mySize = TextRenderer.MeasureText(element, boldFont);
                                e.Graphics.DrawString(element, boldFont, blueBrush, new Rectangle(new Point(baseX, baseY), mySize), StringFormat.GenericTypographic);
                                baseX = baseX + mySize.Width;
                                //baseX = baseX + mySize.Width - 5;

                            }
                            else
                            {
                                mySize = TextRenderer.MeasureText(element, normalFont);

                                //if (!element.Any( x => x >= 0x2016 && x<= 0xFF5D) && element.ToUpper() == element) 
                                //    mySize.Width = mySize.Width + 5;

                                e.Graphics.DrawString(element, normalFont, textBrush, new Rectangle(new Point(baseX, baseY), mySize), StringFormat.GenericTypographic);
                                baseX = baseX + mySize.Width;
                            }
                            
                        }

                        //baseY = baseY + mySize.Height - 1;
                        baseY = baseY + 12;
                        baseX = 0;

                    }
                }





                // Clean up
                backgroundBrush.Dispose();
                textBrush.Dispose();
            }
            e.DrawFocusRectangle();

        }

        //Event when an entry is selected in the listbox
        private void lbEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listEntries.Count > 0)
            {
              
                switch (tcType.SelectedTab.Text)
                {
                    case "Struct":
                        loadStructData();
                        break;

                    case "Strings":
                        loadStringsData(listEntries[lbEntries.SelectedIndex]);
                        break;
                }



            }

        }

        private void loadStructData()
        {
            /*
            currentStruct = fileStruct.Struct.Where(x => x.PointerOffset == entryElement.PointerOffset).FirstOrDefault();
            currentEntry = currentStruct.Entries.Where(x => x.Id == entryElement.Id).FirstOrDefault();

            tbJapaneseText.Text = currentEntry.JapaneseText.Replace("\n", Environment.NewLine);
            tbEnglishText.Text = currentEntry.EnglishText.Replace("\n", Environment.NewLine);
            tbNoteText.Text = currentEntry.Notes;
            */


        }

        private void loadStringsData(Entry currentEntry)
        {


            tbJapaneseText.Text = currentEntry.JapaneseText.Replace("\r","").Replace("\n", Environment.NewLine);
            tbEnglishText.Text = currentEntry.EnglishText.Replace("\r", "").Replace("\n", Environment.NewLine);
            tbNoteText.Text = currentEntry.Notes;
            cbStatus.Text = currentEntry.Status;
        }

        /*
            Save the new changes made to the selected XML file
        */
        private void bSave_Click(object sender, EventArgs e)
        {

            //Remove declaration
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };


            //Remove Namespace
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var xml_string = "";
            var result = "";

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                var serializer = new XmlSerializer(dictFileType[gameName + cbFileType.Text]);
                serializer.Serialize(writer, listFileXML[cbFileList.SelectedIndex], ns);
                xml_string = stream.ToString();
                string pattern = @" \/>";
                result = Regex.Replace(xml_string, pattern, @"/>") +"\r\n";
            }

            File.WriteAllText($@"{basePath}\{cbFileType.Text}\XML\{cbFileList.Text}", result);
            //TextWriter writer = new StreamWriter( );



            MessageBox.Show("Text has been written to the XML files");
            updateListEntries();
        }

        private void fMain_Paint(object sender, PaintEventArgs e)
        {
            Pen blackPen = new Pen(Color.Black, 4);
            Point p = new Point();
            p.X = tbJapaneseText.Location.X + tbJapaneseText.Size.Width - trackBarAlign.Value * 10;
            p.Y = tbJapaneseText.Location.Y;
            verticalLine.Location = p;
        }

        private void trackBarAlign_ValueChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private string getTextBasedLanguage(int entryIndex)
        {
            string res = "";
            Entry myEntry = listEntries[entryIndex];

            if (cbLanguage.Text == "Japanese")
                res = myEntry.JapaneseText;
            else
                res = string.IsNullOrEmpty(myEntry.EnglishText) ? myEntry.JapaneseText : myEntry.EnglishText;

                return res;
        }
        

        private void loadFileList()
        {
            string fileType = cbFileType.Text;
            string[] fileList;
            string path = $@"{basePath}\{fileType}\XML";

            if (Directory.Exists(path))
            {
                fileList = Directory.GetFiles($@"{basePath}\{fileType}\XML").Select(x => Path.GetFileName(x)).ToArray();
                cbFileList.DataSource = fileList;

               
            }
   
        }

        private string get_Folder_Path()
        {
            int size = -1;
            string folderPath = "";
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);
                    folderPath = fbd.SelectedPath;
                }
            }
            return folderPath;
        }

        private void TOPXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gameName = "TOPX";
            basePath = get_Folder_Path();
            string[] directory = Directory.GetDirectories(basePath).Select(x => Path.GetFileName(x)).ToArray();
            cbFileType.DataSource = directory;
            
            loadFileList();
        }

        private void TORToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gameName = "TOR";
            lbEntries.BorderStyle = BorderStyle.FixedSingle;
            if (basePath == null)
            {
                basePath = get_Folder_Path() + "/Data/TOR";

                List<string> folderIncluded = new List<string> { "Story", "Menu" };
                if (Directory.Exists(basePath))
                {
                    string[] directory = Directory.GetDirectories(basePath).Select(x => Path.GetFileName(x)).Where(x => folderIncluded.Contains(x)).ToArray();
                    cbFileType.DataSource = directory;

                    loadFileList();
                    changeEnabledProp(true);
                }
                else
                {
                    MessageBox.Show($"Are you sure you selected the right folder?\nThe folder you choose doesn't represent a valid Tales of Repo.\nIt should have the Data folder in it.\nPath {basePath} not valid.");
                    basePath = null;
                }
            }
        }

        private void loadFile(string fileName)
        {
            try
            {
  
             
                
                using (FileStream stream = File.OpenRead(fileName))
                {
                    XmlSerializer serializer;
                    
                    switch (cbFileType.Text)
                    {
                        case "Story": 
                            serializer = new XmlSerializer(typeof(TORStory));
                            listFileXML.Add((TORStory)serializer.Deserialize(stream));
                            break;

                        case "Menu":
                            serializer = new XmlSerializer(typeof(Menu));
                            listFileXML.Add((Menu)serializer.Deserialize(stream));
                            break;

                    }
                    
                    
                    
                    //lbEntries.DataSource = entryElements;

                    tabType1.Text = "Strings";
                    //sr.Close();
                }
            }
            catch (SecurityException ex)
            {
                MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                $"Details:\n\n{ex.StackTrace}");
            }

            //Filter to remove the entries with status = Done
            //filterStatus();
        }

        private void loadSections()
        {
            string[] sections = listFileXML[cbFileList.SelectedIndex].Strings.OrderBy(x => x.Section).Select(x => x.Section).ToArray();
            cbSections.DataSource = sections;


        }

        private void loadAllFiles()
        {
            if (cbFileList.Items.Count > 0)
            {
                listFileXML.Clear();
                //Load all the XML files in memory
                foreach (string fileName in cbFileList.Items)
                {

                    loadFile($@"{basePath}\{cbFileType.Text}\XML\{fileName}");
                }
                
            }
        }

        private void cbFileType_TextChanged(object sender, EventArgs e)
        {
            loadFileList();

            loadAllFiles();

            loadSections();



        }
        private string countEntries(string status, List<Strings> listEntries, bool withSection = false)
        {
            int nbEntries = 0;
            if (withSection)
            {

                //Count nb entries only inside that section
                nbEntries = listEntries
                .Where(x => x.Section == cbSections.Text)
                .SelectMany(x => x.Entries)
                .Where(y => y.Status == status)
                .Count();
            }
            else
            {

                //Count nb entries for the whole file
                nbEntries = listEntries.SelectMany(x => x.Entries).Where(x => x.Status == status).Count();
            }

            return nbEntries.ToString();
        }
        //Create a list of entries and filter the Section and Status then update the lbentries datasource
        private void updateListEntries()
        {
            List<string> listStatus = dictStatus.Where(x => x.Value == true).Select(x => x.Key).ToList();
            List<Strings> listBasic = listFileXML[cbFileList.SelectedIndex].Strings;
            listEntries = listBasic
                .Where(x => x.Section == cbSections.Text).FirstOrDefault().Entries
                .Where(y => listStatus.Contains(y.Status)).ToList();

            lbEntries.DataSource = listEntries;


            //File Count of status
            lNbToDo.Text = countEntries("To Do", listBasic);
            lNbProof.Text = countEntries("Proofreading", listBasic);
            lNbProb.Text = countEntries("Problematic", listBasic);
            lNbReview.Text = countEntries("In Review", listBasic);
            lNbDone.Text = countEntries("Done", listBasic);

            //Section Count of status
            lNbToDoSect.Text = countEntries("To Do", listBasic,true);
            lNbProofSect.Text = countEntries("Proofreading", listBasic, true);
            lNbProbSect.Text = countEntries("Problematic", listBasic, true); 
            lNbReviewSect.Text = countEntries("In Review", listBasic, true);
            lNbDoneSect.Text = countEntries("Done", listBasic, true);
          
        }
        private void cbFileList_TextChanged(object sender, EventArgs e)
        {
            if (cbFileList.SelectedIndex != -1)
            {
                string fileType = cbFileType.Text;
                string fileName = cbFileList.Text;
                string fullName = $@"{basePath}\{fileType}\XML\{fileName}";

                loadSections();

                updateListEntries();
                
                bSave.Enabled = true;

                

            }
        }

        private void tOPXToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
      
        }

  

        private void tbEnglishText_TextChanged(object sender, EventArgs e)
        {
            listEntries[lbEntries.SelectedIndex].EnglishText = tbEnglishText.Text;


            bool error = (tbEnglishText.Text.Count(x => x == '<') == tbEnglishText.Text.Count(x => x == '>'));
            if (!error)
            {
                lErrors.Text = "Warning: You might be missing a </> for your tags.\nIf you are using </> as a symbol, you can continue.";
                lErrors.ForeColor = Color.Red;
            }
            else
            {
                lErrors.Text = "";

            }

            if (tbEnglishText.Text == tbJapaneseText.Text)
            {
                cbStatus.Text = "Done";
            }else if (tbEnglishText.Text == "")
            {
                cbStatus.Text = "To Do";
            }
            else
            {
                cbStatus.Text = "Proofreading";
            }
        }

        private void cbStatus_TextChanged(object sender, EventArgs e)
        {
            listEntries[lbEntries.SelectedIndex].Status = cbStatus.Text;
            
        }

        private void tbNoteText_TextChanged(object sender, EventArgs e)
        {
            listEntries[lbEntries.SelectedIndex].Notes = tbNoteText.Text;
            
        }

        private void lbEntries_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            var myEntry = listEntries[e.Index];
            string text = getTextBasedLanguage(e.Index);
            int nb = Regex.Matches(text, "\\r*\\n").Count;
            int size = (int)((nb + 1) * 14) + 6;
            dictItemSize[e.Index] = size;
            e.ItemHeight = size;
            
        }

        private void cbFileList_DrawItem(object sender, DrawItemEventArgs e)
        {

            //Get the file selected
            if (listFileXML.Count > 0)
            {
                
                string text = ((ComboBox)sender).Items[e.Index].ToString();
                var count = listEntries.Count;
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    e.Graphics.FillRectangle(new SolidBrush(Color.Black), e.Bounds);
                else
                if (listEntries.Count(x => x.Status == "Done") == count && count > 0)
                {
                    // Background item brush
                    SolidBrush backgroundBrush = new SolidBrush(Color.LightGreen);

                    e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
                }
                else
                    e.Graphics.FillRectangle(new SolidBrush(((Control)sender).BackColor),
                                             e.Bounds);
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
            gameName = "TOR";
            //string res = Tools.callFunction( gameName, "pack", "Menu");
            //MessageBox.Show(res);
        }

        private void cbToDo_CheckedChanged(object sender, EventArgs e)
        {
            dictStatus["To Do"] = cbToDo.Checked;
            updateListEntries();
              
        }

        private void cbProof_CheckedChanged(object sender, EventArgs e)
        {
            dictStatus["Proofreading"] = cbProof.Checked;
            updateListEntries();
        }

        private void cbInReview_CheckedChanged(object sender, EventArgs e)
        {
            dictStatus["In Review"] = cbInReview.Checked;
            updateListEntries();
        }

        private void cbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbEntries.Invalidate();
        }

        private void cbSections_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateListEntries();
        }

        private void bRefresh_Click(object sender, EventArgs e)
        {
            loadAllFiles();

            loadSections();
        }

        private void fMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.Down)
                    if (lbEntries.Items.Count - 1 != lbEntries.SelectedIndex)
                        lbEntries.SelectedIndex += 1;

                if (e.KeyCode == Keys.Up)
                    if (lbEntries.SelectedIndex > 0)
                        lbEntries.SelectedIndex -= 1;

                if (e.KeyCode == Keys.W && String.IsNullOrWhiteSpace(tbEnglishText.Text))
                    tbEnglishText.Text = tbJapaneseText.Text;

                //Swallow event 
                e.Handled = true;
            }
        }
    }
}

    
