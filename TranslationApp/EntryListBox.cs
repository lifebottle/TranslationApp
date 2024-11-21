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
        private static readonly Dictionary<string, Color> ColorByStatus = new Dictionary<string, Color>
            {
                { "To Do", Color.White},
                { "Editing", Color.FromArgb(162, 255, 255) }, // Light Cyan
                { "Proofreading", Color.FromArgb(255, 102, 255) }, // Magenta
                { "Problematic", Color.FromArgb(255, 255, 162) }, // Light Yellow
                { "Done", Color.FromArgb(162, 255, 162) }, // Light Green
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

        private void DrawLines(DrawItemEventArgs e, string text, ref Point startPoint, Font tagFont, Color tagColor, Font regularFont, Color regularColor, Size proposedSize, TextFormatFlags flags)
        {
            Size mySize;

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

                // Update HorizonalExtent so we can have horizontal scrolling
                if (HorizontalExtent < startPoint.X)
                {
                    HorizontalExtent = startPoint.X + 20;
                }

                if (i < lines.Length - 1)
                {
                    startPoint.Y += 13;
                    startPoint.X = 3;
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
