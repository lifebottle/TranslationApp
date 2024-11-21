using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TranslationLib;

namespace TranslationApp
{
    public class EntryListBox : ListBox
    {
        public bool displayJapanese { get; set; }
        public bool displayIndices { get; set; }
        public CheckState tagMode { get; set; }
        private static readonly Dictionary<string, Color> ColorByStatus = new Dictionary<string, Color> {
            { "To Do", Color.White},
            { "Editing", Color.FromArgb(162, 255, 255) }, // Light Cyan
            { "Proofreading", Color.FromArgb(255, 102, 255) }, // Magenta
            { "Problematic", Color.FromArgb(255, 255, 162) }, // Light Yellow
            { "Done", Color.FromArgb(162, 255, 162) }, // Light Green
        };
        private static readonly Dictionary<string, string> availableTags = new Dictionary<string, string>
        {
            { "nl",          "1"},
            { "cr",          "2"},
            { "var",         "4"},
            { "color",       "5"},
            { "Blue",        "5.1"},
            { "Red",         "5.2"},
            { "Purple",      "5.3"},
            { "Green",       "5.4"},
            { "Cyan",        "5.5"},
            { "Yellow",      "5.6"},
            { "White",       "5.7"},
            { "Grey",        "5.8"},
            { "Black",       "5.9"},
            { "scale",       "6"},
            { "speed",       "7"},
            { "italic",      "8"},
            { "Italic",      "8.1"},
            { "/Italic",     "8.2"},
            { "nmb",         "9"},
            { "ptr",         "10"},
            { "name",        "11"},
            {" Veigue",      "11.1"},
            {" Mao",         "11.2"},
            {" Eugene",      "11.3"},
            {" Annie",       "11.4"},
            {" Tytree",      "11.5"},
            {" Hilda",       "11.6"},
            {" Claire",      "11.7"},
            {" Agarte",      "11.8"},
            {" Annie (NPC)", "11.9"},
            {" Leader",      "11.10"},
            { "item",        "12"},
            { "icon",        "13"},
            { "font",        "14"},
            { "voice",       "15"},
            { "unk13",       "19"},
            { "unk14",       "20"},
            { "unk15",       "21"},
            { "unk16",       "22"},
            { "unk17",       "23"},
            { "unk18",       "24"},
            { "unk19",       "25"},
            { "unk1A",       "26"},
        };
        private static readonly string[] names = {
            "<Veigue>",
            "<Mao>",
            "<Eugene>",
            "<Annie>",
            "<Tytree>",
            "<Hilda>",
            "<Claire>",
            "<Agarte>",
            "<Annie (NPC)>",
            "<Leader>"
        };
        private static readonly string[] allowTags = {
            "<nl>",
            "<cr>",
            "<Blue>",
            "<Red>",
            "<Purple>",
            "<Green>",
            "<Cyan>",
            "<Yellow>",
            "<White>",
            "<Grey>",
            "<Black>",
            "<Italic>",
            "</Italic>"
        };
        private static readonly Lazy<Regex> tagPattern = new Lazy<Regex>(() => new Regex(@"(<[\w/]+:?\w+>)", RegexOptions.Compiled));
        private static readonly Lazy<Regex> nlPattern = new Lazy<Regex>(() => new Regex(@"\r*\n", RegexOptions.Compiled));

        public EntryListBox()
        {
            DrawMode = DrawMode.OwnerDrawVariable;
            HorizontalScrollbar = true;
            FormattingEnabled = true;
            SelectionMode = SelectionMode.MultiExtended;
        }

        public void SetTagMode(CheckState s)
        {
            tagMode = s;
            Invalidate();
        }

        public void SetDisplayIndices(bool value)
        {
            displayIndices = value;
            Invalidate();
        }

        // TODO: Un-rebirth this
        private string GetTagString(string tag)
        {
            switch (tagMode)
            {
                case CheckState.Unchecked:
                    break;
                case CheckState.Checked:
                    if (allowTags.Contains(tag))
                    {
                        return tag;
                    }
                    else if (tag.Contains(":"))
                    {
                        return tag.Substring(0, tag.IndexOf(":")) + ">";
                    }
                    else
                    {
                        return "<>";
                    }
                case CheckState.Indeterminate:
                    var sub = tag.Substring(1, tag.Length - 2);
                    if (sub.Contains(":"))
                    {
                        sub = sub.Substring(0, sub.IndexOf(":"));
                    }

                    string v;
                    if (availableTags.TryGetValue(sub, out v))
                    {
                        return "<" + v + ">";
                    } 
                    else
                    {
                        return "<>";
                    }
            }
            return tag;
        }

        private void DrawLines(DrawItemEventArgs e, string text, ref Point startPoint, Font tagFont, Color tagColor, Font regularFont, Color regularColor, Size proposedSize, TextFormatFlags flags)
        {
            Size mySize;
            var initialX = startPoint.X;
            string[] lines = nlPattern.Value.Split(text);

            //Starting point for drawing, a little offsetted
            //in order to not touch the borders
            //Point startPoint = new Point(3, e.Bounds.Y + 3);

            for (int i = 0; i < lines.Length; i++)
            {

                //3. Split based on the different tags
                //Split the text based on the Tags < xxx >
                string line = lines[i];
                string[] result = tagPattern.Value.Split(line).Where(x => x != "").ToArray();
                //We need to loop over each element to adjust the color
                foreach (string element in result)
                {
                    if (element[0] == '<')
                    {
                        string tag = GetTagString(element);
                        mySize = TextRenderer.MeasureText(e.Graphics, tag, tagFont, proposedSize, flags);

                        TextRenderer.DrawText(e.Graphics, tag, tagFont, startPoint, tagColor, flags);
                        startPoint.X += mySize.Width;
                    }
                    else
                    {
                        mySize = TextRenderer.MeasureText(e.Graphics, element, regularFont, proposedSize, flags);

                        TextRenderer.DrawText(e.Graphics, element, regularFont, startPoint, regularColor, flags);
                        startPoint.X += mySize.Width;
                    }
                }

                // Update HorizonalExtent so we can have horizontal scrolling
                if (HorizontalExtent < startPoint.X)
                {
                    HorizontalExtent = startPoint.X + 20;
                }

                if (i < lines.Length - 1)
                {
                    startPoint.Y += 13;
                    startPoint.X = initialX;
                }
            }
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            if (DesignMode)
            {
                return;
            }
            List<XMLEntry> entries = DataSource as List<XMLEntry>;

            if (e.Index >= entries.Count)
                return;

            XMLEntry entry = entries[e.Index];
            string text = GetTextBasedLanguage(entry);

            text = text == null ? "" : text;

            int nb = 0;
            if (entry.SpeakerId != null)
            {
                nb += 1;
            }

            nb += nlPattern.Value.Matches(text).Count;

            var size = ((nb + 1) * 14) + 6;

            e.ItemHeight = size;
        }

        // TODO: Maybe move this? So it's not Rebirth specific
        private string stripTags(string input)
        {
            string output = "";
            string[] result = tagPattern.Value.Split(input.Replace("\r", "").Replace("\n", "")).Where(x => x != "").ToArray();

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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                List<string> st = new List<string>();

                foreach (XMLEntry et in SelectedItems)
                {
                    st.Add(stripTags(et.JapaneseText));
                }
                Clipboard.SetText(string.Join("\n", st));
            }
        }

        private string GetTextBasedLanguage(XMLEntry entry)
        {
            if (displayJapanese)
                return entry.JapaneseText;
            else
                return entry.EnglishText == null ? entry.JapaneseText : entry.EnglishText;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (DesignMode)
            {
                return;
            }
            List<XMLEntry> entries = DataSource as List<XMLEntry>;
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            //Draw only if elements are present in the listbox
            if (e.Index > -1)
            {
                //Regardless of text, draw elements close together
                //and use the intmax size as per the docs
                TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;
                Size proposedSize = new Size(int.MaxValue, int.MaxValue);

                //Grab the current entry to draw
                XMLEntry entry = entries[e.Index];

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

                string text = GetTextBasedLanguage(entry);
                Point startPoint = new Point(3, e.Bounds.Y + 3);

                if (displayIndices)
                {
                    e.Graphics.DrawLine(new Pen(Color.DimGray, 1.5f), new Point(25, e.Bounds.Top - 1), new Point(25, e.Bounds.Bottom - 1));
                    TextRenderer.DrawText(e.Graphics, "" + (entry.Id ?? 0), boldFont, startPoint, tagColor, flags);
                    startPoint.X += 28;
                }


                //1. Add Speaker name
                if (entry.SpeakerId != null)
                {
                    TextRenderer.DrawText(e.Graphics, entry.SpeakerName, boldFont, startPoint, tagColor, flags);
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
    }
}
