using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationApp
{

    public partial class TextPreview : PictureBox
    {
        public Bitmap fontAtlasImage { get; set; }
        private Bitmap currentImage;
        public string text { get; set; }

        private font_glyph[] glyphs;

        private static readonly Dictionary<string, Color> colors = new Dictionary<string, Color> {
                    { "<Blue>", Color.FromArgb(0x50,0x50,0x80) },
                    { "<Red>", Color.FromArgb(0x80,0x48,0x40) },
                    { "<Purple>", Color.FromArgb(0x80,0x58,0x80) },
                    { "<Green>", Color.FromArgb(0x60,0x80,0x50) },
                    { "<Cyan>", Color.FromArgb(0x60,0x80,0x80) },
                    { "<Yellow>", Color.FromArgb(0x80,0x80,0x50) },
                    { "<White>", Color.FromArgb(0x80,0x80,0x80) },
                    { "<Grey>", Color.FromArgb(0x48,0x48,0x48) },
                    { "<Black>", Color.FromArgb(0x00,0x00,0x00)  },
                };

        private static readonly string[] names = { "<Veigue>", "<Mao>", "<Eugene>", "<Annie>", "<Tytree>", "<Hilda>", "<Claire>", "<Agarte>", "<Annie (NPC)>", "<Leader>" };

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

        public TextPreview()
        {
            InitializeComponent();
            SizeMode = PictureBoxSizeMode.AutoSize;
            Dock = DockStyle.None;
        }

        public void ChangeImage(string id)
        {
            string res;
            switch (id)
            {
                case "TOR":
                    res = "TranslationApp.res.tor_font_atlas.png";
                    BackColor = Color.FromArgb(0);
                    glyphs = tor_glyphs;
                    break;
                case "NDX":
                    res = "TranslationApp.res.ndx_font_atlas.png";
                    BackColor = Color.FromArgb(0x14, 0x3F, 0x60);
                    glyphs = ndx_glyphs;
                    break;
                case "RM2":
                    res = "TranslationApp.res.rm2_font_atlas.png";
                    BackColor = Color.FromArgb(0xA0, 0x0, 0x0, 0x0);
                    glyphs = rm2_glyphs;
                    break;

                case "TOH":
                    res = "TranslationApp.res.toh_font_atlas.png";
                    BackColor = Color.FromArgb(0xA0, 0x0, 0x0, 0x0);
                    glyphs = toh_glyphs;
                    break;
                default:
                    fontAtlasImage = null;
                    glyphs = null;
                    return;
            }

            if (fontAtlasImage != null)
            {
                fontAtlasImage.Dispose();
            }

            fontAtlasImage = LoadEmbeddedImage(res);
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

        public void ReDraw(string text)
        {
            this.text = text;
            Raster();
            Invalidate();
        }


        private void Raster()
        {
            // No image, no fun
            if (fontAtlasImage == null)
            {
                return;
            }

            if (currentImage != null)
            {
                currentImage.Dispose();
            }

            using (Bitmap canvas = new Bitmap(1000, 1000))
            {
                using (Graphics canvas_g = Graphics.FromImage(canvas))
                {
                    canvas_g.CompositingMode = CompositingMode.SourceOver;
                    canvas_g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    canvas_g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    canvas_g.SmoothingMode = SmoothingMode.HighQuality;

                    // Initial state
                    PointF currentPosition = new PointF(10, 10);  // Starting position
                    float finalWidth = 0;
                    string textToRender = text ?? ""; // Avoid null text
                    Color tintColor = colors["<White>"]; // Start as white text
                    Color lastColor = tintColor;

                    ColorMatrix colorMatrix = new ColorMatrix();
                    ImageAttributes imageAttributes = new ImageAttributes();
                    string[] result = Regex.Split(textToRender.Replace("\r", ""), @"(<[\w/]+:?\w+>)", RegexOptions.IgnoreCase).Where(x => x != "").ToArray();

                    bool shear = false;

                    foreach (string element in result)
                    {
                        // Only parse actual potential tags
                        if (element[0] == '<' && element[element.Length - 1] == '>')
                        {
                            if (colors.ContainsKey(element))
                            {
                                lastColor = tintColor;
                                tintColor = colors[element];
                                continue;
                            }
                            else if (names.Contains(element))
                            {
                                textToRender = element.Substring(1, element.Length - 2);
                            }
                            else if (element.StartsWith("<unk") || element.StartsWith("<var") || element.StartsWith("<icon"))
                            {
                                textToRender = "***";
                            }
                            else if (element == "<Italic>")
                            {
                                shear = true;
                                continue;
                            }
                            else if (element == "</Italic>")
                            {
                                shear = false;
                                continue;
                            }
                            else if (element.StartsWith("<nmb:"))
                            {
                                string el = element.Substring(5, element.Length - 6);
                                textToRender = Convert.ToInt32(el, 16).ToString();
                            }
                            else if (element.StartsWith("<color:20"))
                            {
                                int r, g, b;
                                if (element.Length - 10 >= 6)
                                {
                                    try
                                    {
                                        r = Convert.ToInt32(element.Substring(13, 2), 16);
                                        g = Convert.ToInt32(element.Substring(11, 2), 16);
                                        b = Convert.ToInt32(element.Substring(9, 2), 16);
                                        lastColor = tintColor;
                                        tintColor = Color.FromArgb(r, g, b);
                                    }
                                    catch (FormatException)
                                    {
                                        // ignore
                                    }

                                }
                                continue;
                            }
                            else if (element.StartsWith("<color:80"))
                            {
                                tintColor = lastColor;
                                continue;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            textToRender = element;
                        }

                        // Apply colors
                        colorMatrix.Matrix00 = tintColor.R / 128f;
                        colorMatrix.Matrix11 = tintColor.G / 128f;
                        colorMatrix.Matrix22 = tintColor.B / 128f;
                        colorMatrix.Matrix33 = 1.8f;
                        imageAttributes.SetColorMatrix(colorMatrix);

                        foreach (char c in textToRender)
                        {
                            // Get the rectangle for the current character in the atlas
                            Rectangle charRect = GetCharacterRectangleFromAtlas(c, out int shift, out bool line);

                            // Create a destination rectangle using the currentPosition
                            RectangleF destinationRect = new RectangleF(currentPosition, new SizeF(charRect.Width, charRect.Height));

                            if (shear)
                            {
                                canvas_g.TranslateTransform(currentPosition.X, currentPosition.Y);
                                Matrix shearMatrix = new Matrix();
                                shearMatrix.Shear(-0.2f, 0.0f);
                                canvas_g.MultiplyTransform(shearMatrix);
                                canvas_g.TranslateTransform(-currentPosition.X, -currentPosition.Y);
                            }
                            else
                            {
                                canvas_g.ResetTransform();
                            }

                            // Draw the character onto the surface
                            canvas_g.DrawImage(
                                fontAtlasImage,
                                Rectangle.Round(destinationRect),
                                charRect.X,
                                charRect.Y,
                                charRect.Width,
                                charRect.Height,
                                GraphicsUnit.Pixel,
                                imageAttributes
                                );

                            canvas_g.ResetTransform();

                            // Update the current position for the next character
                            if (line)
                            {
                                currentPosition.X = 10;
                                currentPosition.Y += 24;
                            }
                            else
                            {
                                currentPosition.X += charRect.Width - shift;
                                // Update max texture size
                                if (finalWidth < currentPosition.X)
                                {
                                    finalWidth = currentPosition.X;
                                }
                            }
                        }
                    }

                    currentImage = new Bitmap((int)((finalWidth + 10) * 0.75f), (int)((currentPosition.Y + 34) * 0.75f));

                    // Draw the final image scaled down
                    using (Graphics canvas_f = Graphics.FromImage(currentImage))
                    {
                        canvas_f.ScaleTransform(0.75f, 0.75f);
                        canvas_f.DrawImage(canvas, 0, 0);
                    }
                    //Graphics g = e.Graphics;
                    //g.ScaleTransform(0.75f, 0.75f);
                    //g.DrawImage(currentImage, 0, 0);
                    Image = currentImage;
                }
            }
        }

        public string DoLineBreak(string text, int maxWidth)
        {
            List<(string Word, int Size)> words = GetListWords(text);
            int sum = 0;      
            int i = 0;
            string final = "";
            int oCount = words.Count;

            if (maxWidth < words.Select(x => x.Size).Max())
            {
                maxWidth = words.Select(x => x.Size).Max();
                
            }
            while (i < oCount)
            {
                sum = 0;
                List<int> output = words.Select(x => sum += x.Size).ToList();
                List<int> maxCount = Enumerable.Range(0, output.Count).Where(ind => output[ind] <= maxWidth).ToList();

                string line = "";
                foreach (int c in maxCount)
                    line += words[c].Word;

                words.RemoveRange(0, maxCount.Count);
                i += maxCount.Count;
                final += (line + Environment.NewLine);


            }

            return final; 
        }

        public string WordWrap(string text, int maxWidth)
        {
            Rectangle space = GetCharacterRectangleFromAtlas(' ', out int s, out bool add);
            List<(string,int)> words = GetListWords(text);

            int curLineLength = 0;
            StringBuilder strBuilder = new StringBuilder();
            foreach ((string word, int width) in words)
            {
                string finalWord = word;
                // If adding the new word to the current line would be too long,
                // then put it on a new line (and split it up if it's too long).
                if (curLineLength + width > maxWidth)
                {
                    // Only move down to a new line if we have text on the current line.
                    // Avoids situation where
                    // wrapped whitespace causes emptylines in text.
                    if (curLineLength > 0)
                    {
                        strBuilder.Append(Environment.NewLine);
                        curLineLength = 0;
                    }

                    // Remove leading whitespace from the word,
                    // so the new line starts flush to the left.
                    finalWord = word.TrimStart();
                }
                strBuilder.Append(word + " ");
                curLineLength += (width + space.Width);
            }

            return strBuilder.ToString();
        }

        public List<(string, int)> GetListWords(string text)
        {
            //Split tags and text
            string[] result = Regex.Split(text.Replace("\r", ""), @"(<[\w/]+:?\w+>[,|.|\[||\]]*)", RegexOptions.IgnoreCase).Where(x => x != "").ToArray();
            Rectangle space = GetCharacterRectangleFromAtlas(' ', out int s, out bool add);

            string textToRender = "";
            bool shear = false;
            List<(string Word, int Size)> wordsSize = new List<(string, int)>();

            foreach (string element in result)
            {
                string tag = "";

                int pos = element.IndexOf(">");
                if (pos > -1)
                {
                    tag = element.Substring(0, pos+1);

                    if (names.Contains(tag))
                    {
                        textToRender = tag.Substring(1, pos - 1);
                    }
                    else if (element.StartsWith("<unk") || element.StartsWith("<var") || element.StartsWith("<icon"))
                    {
                        textToRender = "***";
                    }
                    else if (element == "<Italic>")
                    {
                        shear = true;
                        continue;
                    }
                    else if (element == "</Italic>")
                    {
                        shear = false;
                        continue;
                    }
                    else if (element.StartsWith("<nmb:"))
                    {
                        string el = element.Substring(5, element.Length - 6);
                        textToRender = Convert.ToInt32(el, 16).ToString();
                    }

                    if (pos < element.Length)
                        textToRender += element.Substring(pos+1, element.Length - pos - 1);
                    
                }
                else
                {
                    textToRender = element;
                }


                //Remove all linebreak
                textToRender = textToRender.Replace("\n", " ");
                
                foreach (string word in textToRender.Split(' ').Where(x => x != ""))
                {

                    wordsSize.Add((word, GetWordWidth(word)));
                    wordsSize.Add((" ", space.Width));

                }
            }

            wordsSize.RemoveAt(wordsSize.Count - 1);
            return wordsSize;
        }

        private int GetWordWidth(string word)
        {
            int width = 0;
            foreach (char c in word) {
                Rectangle rect = GetCharacterRectangleFromAtlas(c, out int x, out bool b);
                width += rect.Width;
            }

            return width;
        }
        private void TextPreview_Paint(object sender, PaintEventArgs e)
        {
            

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
                    case '&':
                        index = 84;
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
                    case '%':
                        index = 82;
                        break;
                    default:
                        index = 0;
                        break;
                }

            // Calculate the position of the character in the atlas based on its index
            int y = index * charHeight;
            int x = glyphs[index].lskip;

            charWidth -= (glyphs[index].lskip);
            s = glyphs[index].rskip;
            return new Rectangle(x, y, charWidth, charHeight);
        }
    }
}
