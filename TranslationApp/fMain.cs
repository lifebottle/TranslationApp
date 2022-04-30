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

namespace TranslationApp
{
    public partial class fMain : Form
    {
        public List<Entry> entryElements = new List<Entry>();
        public Entry entryElement = new Entry();
        public Dictionary<string, Type> dictFileType = new Dictionary<string, Type>();
        public Entry currentEntry;
        public int nbTags = 0;
        private string gameName;
        public string basePath;

        public List<TalesFile> listFileXML = new List<TalesFile>();

        public fMain()
        {
            InitializeComponent();

            
        }

        //Draw entries with multiline and font color changed
        private void lbEntries_DrawItem(object sender, DrawItemEventArgs e)
        {
            bool isSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);


            //Draw only if elements are present in the listbox
            if (e.Index > -1)
            {

                //Create a color that alternate
                Color color = isSelected ? SystemColors.Highlight :
                    e.Index % 2 == 0 ? Color.WhiteSmoke : Color.White;

                // Background item brush
                SolidBrush backgroundBrush = new SolidBrush(color);

                // Text color brush
                SolidBrush textBrush = new SolidBrush(e.ForeColor);
                SolidBrush blueBrush = new SolidBrush(Color.Blue);

                // Draw the background
                e.Graphics.FillRectangle(backgroundBrush, e.Bounds);

                Font normalFont = new Font("Arial", 8, FontStyle.Regular);
                Font boldFont = new Font("Arial", 8, FontStyle.Bold);
                Entry entry = ((Entry)lbEntries.Items[e.Index]);
                string japText = entry.JapaneseText;

                //1. Split based on the line breaks
                // We will need to increase the baseY after each line
                int baseX = 0;
                int baseY = e.Bounds.Y;

                if (japText != "")
                {
                    string[] lines = Regex.Split(japText, "\\n", RegexOptions.IgnoreCase);

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
                                mySize = TextRenderer.MeasureText(element+' ', boldFont);
                                e.Graphics.DrawString(element, boldFont, blueBrush, new Rectangle(new Point(baseX, baseY), mySize), StringFormat.GenericDefault);
                            }
                            else
                            {
                                mySize = TextRenderer.MeasureText(element+' ', normalFont);
                                e.Graphics.DrawString(element, normalFont, textBrush, new Rectangle(new Point(baseX, baseY), mySize), StringFormat.GenericDefault);

                            }
                            baseX = baseX + mySize.Width;
                        }

                        baseY = baseY + mySize.Height - 1;
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
            if (entryElements.Count > 0)
            {
                entryElement = (Entry)lbEntries.Items[lbEntries.SelectedIndex];

                switch (tcType.SelectedTab.Text)
                {
                    case "Struct":
                        loadStructData();
                        break;

                    case "Strings":
                        loadStringsData();
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

        private void loadStringsData()
        {
            currentEntry = entryElements.Where(x => x.PointerOffset == entryElement.PointerOffset).FirstOrDefault();

            string pattern = @"(<\w+:?\w+>)";
            string[] result = Regex.Split(currentEntry.JapaneseText, pattern,
                                    RegexOptions.IgnoreCase);

            tbJapaneseText.Text = currentEntry.JapaneseText.Replace("\n", Environment.NewLine);
            tbEnglishText.Text = currentEntry.EnglishText.Replace("\n", Environment.NewLine);
            tbNoteText.Text = currentEntry.Notes;
            cbStatus.Text = currentEntry.Status;
        }

        /*
            Save the new changes made to the selected XML file
        */
        private void bSave_Click(object sender, EventArgs e)
        {
         
            XmlSerializer serializer = new XmlSerializer(dictFileType[gameName+cbFileType.Text]);

            TextWriter writer = new StreamWriter( $@"{basePath}\{cbFileType.Text}\XML\{cbFileList.Text}");

            //fileStruct.Strings
            serializer.Serialize(writer, listFileXML[ cbFileList.SelectedIndex]);
            writer.Close();

            MessageBox.Show("Text has been written to the XML files");
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

        private void fMain_Load(object sender, EventArgs e)
        {
            //Basepath
            List<string> directories = Directory.GetDirectories(@"..").Where(x => x.Contains("Debug")).ToList();
            if (directories.Count > 0)
                basePath = "../../../../Data/";
            else
                basePath = "../Data/";




            bSave.Enabled = false;
            for (int i = 0; i < cbListStatus.Items.Count; i++)
            {
                if (cbListStatus.Items[i].ToString() != "Done")
                    cbListStatus.SetItemChecked(i, true);
            }

            dictFileType.Add("TORStory", typeof(TORStory));
            dictFileType.Add("TORMenu", typeof(Menu));
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
        private void TOPXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gameName = "TOPX";
            basePath = basePath+ gameName;
            string[] directory = Directory.GetDirectories(basePath).Select(x => Path.GetFileName(x)).ToArray();
            cbFileType.DataSource = directory;
            
            loadFileList();
        }

        private void TORToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gameName = "TOR";
            lbEntries.BorderStyle = BorderStyle.FixedSingle;
            basePath = basePath + gameName;
            string[] directory = Directory.GetDirectories(basePath).Select(x => Path.GetFileName(x)).ToArray();
            cbFileType.DataSource = directory;
            
            loadFileList();
        }

        private void TODDCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gameName = "TODDC";
            lbEntries.BorderStyle = BorderStyle.FixedSingle;
            basePath = basePath + gameName;
            string[] directory = Directory.GetDirectories(basePath).Select(x => Path.GetFileName(x)).ToArray();
            cbFileType.DataSource = directory;

            loadFileList();
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
                    
                    
                    lbEntries.DisplayMember = "DisplayText";
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
            string[] sections = listFileXML[cbFileList.SelectedIndex].Strings.Select(x => x.Section).ToArray();
            cbSections.DataSource = sections;


        }

        private void cbFileType_TextChanged(object sender, EventArgs e)
        {
            loadFileList();

            if (cbFileList.Items.Count > 0)
            {
                listFileXML.Clear();
                //Load all the XML files in memory
                foreach (string fileName in cbFileList.Items)
                {

                    loadFile($@"{basePath}\{cbFileType.Text}\XML\{fileName}");
                }
                loadSections();
            }


        }

        private void cbFileList_TextChanged(object sender, EventArgs e)
        {
            if (cbFileList.SelectedIndex != -1)
            {
                string fileType = cbFileType.Text;
                string fileName = cbFileList.Text;
                string fullName = $@"{basePath}\{fileType}\XML\{fileName}";
                entryElements = Tools.getEntries(listFileXML[cbFileList.SelectedIndex]);
                lbEntries.DataSource = entryElements;
                bSave.Enabled = true;

                loadSections();
                filterStatus();
            }
        }


        private void packIsoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void tOPXToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
      
        }

  

        private void tbEnglishText_TextChanged(object sender, EventArgs e)
        {
            ((Entry)lbEntries.Items[lbEntries.SelectedIndex]).EnglishText = tbEnglishText.Text;
            bool error = (tbEnglishText.Text.Count(x => x == '<') == tbEnglishText.Text.Count(x => x == '>'));
            if (!error)
            {
                lErrors.Text = "Invalid number of symbols </>";
                lErrors.ForeColor = Color.Red;
            }
            else
            {
                lErrors.Text = "";

            }
        }

        private void cbStatus_TextChanged(object sender, EventArgs e)
        {
            ((Entry)lbEntries.Items[lbEntries.SelectedIndex]).Status = cbStatus.Text;
        }

        private void tbNoteText_TextChanged(object sender, EventArgs e)
        {
            ((Entry)lbEntries.Items[lbEntries.SelectedIndex]).Notes = tbNoteText.Text;
        }

        private void lbEntries_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            string text = ((Entry)((ListBox)sender).Items[e.Index]).JapaneseText;
            int nb = Regex.Matches(text, "\\n").Count;
            e.ItemHeight = (int)((nb + 1) * ((ListBox)sender).Font.GetHeight() + 2) + 10;
        }


        private void filterStatus()
        {
            List<string> checkedBox = new List<string>();
            for (int i = 0; i < cbListStatus.CheckedItems.Count; i++)
            {
                checkedBox.Add(cbListStatus.CheckedItems[i].ToString());
            }

            lbEntries.DataSource = Tools.getEntries(listFileXML[cbFileList.SelectedIndex]).Where(x => checkedBox.Any(y => x.Status == y)).ToList();
        }
        private void cbListStatus_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            filterStatus();
        }

        private void cbFileList_DrawItem(object sender, DrawItemEventArgs e)
        {

            //Get the file selected
            if (listFileXML.Count > 0)
            {
                List<Entry> currentEntryElements = Tools.getEntries(listFileXML[e.Index]);
                string text = ((ComboBox)sender).Items[e.Index].ToString();

                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    e.Graphics.FillRectangle(new SolidBrush(Color.Black), e.Bounds);
                else
                if (currentEntryElements.Count(x => x.Status == "Done") == currentEntryElements.Count)
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbSections_TextChanged(object sender, EventArgs e)
        {
            string section = cbSections.Text;
            lbEntries.DataSource = Tools.getEntries(listFileXML[cbFileList.SelectedIndex], section);
        }

        private void hexToJapaneseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fHexToJapanese myForm = new fHexToJapanese();
            myForm.Show();
        }


        private void menuToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            gameName = "TOR";
            string res = Tools.callFunction( gameName, "pack", "Menu");
            MessageBox.Show(res);
        }

        
    }
}

    
