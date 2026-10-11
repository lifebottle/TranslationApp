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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static TranslationApp.TextPreview;

namespace TranslationApp
{

    public partial class TextPreview : PictureBox
    {
        public Bitmap fontAtlasImage { get; set; }
        private Bitmap currentImage;
        public string text { get; set; }

        private static readonly Dictionary<string, Color> tor_colors = new Dictionary<string, Color> {
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

        private static readonly Dictionary<string, Color> ndx_colors = new Dictionary<string, Color> {
            { "<Black>", Color.FromArgb(0x00, 0x00, 0x00) },
            { "<Blue>", Color.FromArgb(0x80, 0x80, 0xFF) },
            { "<Red>", Color.FromArgb(0xFF, 0x80, 0x80) },
            { "<Purple>", Color.FromArgb(0xC1, 0x82, 0xCA) },
            { "<Green>", Color.FromArgb(0x80, 0xFF, 0x80) },
            { "<Cyan>", Color.FromArgb(0x80, 0xFF, 0xFF) },
            { "<Yellow>", Color.FromArgb(0xFB, 0xEC, 0x8F) },
            { "<White>", Color.FromArgb(0xFF, 0xFF, 0xFF) },
            { "<Grey>", Color.FromArgb(0xA0, 0xA0, 0xA0) },
            { "<DarkBlue>", Color.FromArgb(0x00, 0x49, 0x76) },
            { "<DarkLightBlue>", Color.FromArgb(0x4D, 0xA4, 0xC1) },
            { "<DarkWhite>", Color.FromArgb(0xB7, 0xCF, 0xD2) },
            { "<DarkLime>", Color.FromArgb(0x12, 0xB1, 0x4E) },
            { "<DarkGreen>", Color.FromArgb(0x37, 0x98, 0x37) },
            { "<DarkCyan>", Color.FromArgb(0x1A, 0xBD, 0xCA) },
            { "<DarkYellow>", Color.FromArgb(0xEE, 0xC6, 0x0E) },
            { "<DarkGrey>", Color.FromArgb(0x7E, 0x7D, 0x79) },
            { "<AsOutline>", Color.FromArgb(0x00, 0x00, 0x00) },
            { "<BlackOutline>", Color.FromArgb(0x00, 0x00, 0x00) },
            { "<BlueOutline>", Color.FromArgb(0x80, 0x80, 0xFF) },
            { "<RedOutline>", Color.FromArgb(0xFF, 0x80, 0x80) },
            { "<PurpleOutline>", Color.FromArgb(0xC1, 0x82, 0xCA) },
            { "<GreenOutline>", Color.FromArgb(0x80, 0xFF, 0x80) },
            { "<CyanOutline>", Color.FromArgb(0x80, 0xFF, 0xFF) },
            { "<YellowOutline>", Color.FromArgb(0xFB, 0xEC, 0x8F) },
            { "<WhiteOutline>", Color.FromArgb(0xFF, 0xFF, 0xFF) },
            { "<GreyOutline>", Color.FromArgb(0xA0, 0xA0, 0xA0) },
            { "<DarkBlueOutline>", Color.FromArgb(0x00, 0x49, 0x76) },
            { "<DarkLightBlueOutline>", Color.FromArgb(0x4D, 0xA4, 0xC1) },
            { "<DarkWhiteOutline>", Color.FromArgb(0xB7, 0xCF, 0xD2) },
            { "<DarkLimeOutline>", Color.FromArgb(0x12, 0xB1, 0x4E) },
            { "<DarkGreenOutline>", Color.FromArgb(0x37, 0x98, 0x37) },
            { "<DarkCyanOutline>", Color.FromArgb(0x1A, 0xBD, 0xCA) },
            { "<DarkYellowOutline>", Color.FromArgb(0xEE, 0xC6, 0x0E) },
            { "<DarkGreyOutline>", Color.FromArgb(0x7E, 0x7D, 0x79) },
            { "<NoOutline>", Color.FromArgb(0x00, 0x00, 0x00) },
        };

        private static readonly string[] names = { 
            // Rebirth
            "<Veigue>", "<Mao>", "<Eugene>", "<Annie>", "<Tytree>", "<Hilda>", "<Claire>", "<Agarte>", "<Annie (NPC)>", "<Leader>",
            // NDX
            "<Arche>", "<Beryl>", "<Cless>", "<Chester>", "<Dio>", "<Hisui>", "<Ines>", "<Klarth>", "<Kohaku>", "<Couleur>", "<Kunzite>", "<Mel>", "<Mint>", "<Rondoline>", "<Rody>", "<Shing>", "<Suzu>"
        };

        public class Style
        {
            public font_glyph[] Glyphs;
            public string Resource;
            public Color BackColor;
            public float scale_factor;
            public Dictionary<string, Color> Colors;
            public Style(font_glyph[] glyphs, Dictionary<string, Color> colors, string resource, Color back_color, bool full_range)
            {
                Glyphs = glyphs;
                Resource = resource;
                BackColor = back_color;
                scale_factor = full_range ? 256.0f : 128.0f;
                Colors = colors;
            }
        };

        public static Dictionary<string, Style> textbox_styles;

        private Style current_style;

        public struct font_glyph
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
            textbox_styles = new Dictionary<string, Style> {
                {
                    "TOR", new Style(tor_glyphs, tor_colors,
                    "TranslationApp.res.tor_font_atlas.png",
                    Color.FromArgb(0), false)
                },
                {
                    "NDX", new Style(ndx_glyphs, ndx_colors,
                    "TranslationApp.res.ndx_font_atlas.png",
                    Color.FromArgb(0x14, 0x3F, 0x60), true)
                },
                {
                    "RM2", new Style(rm2_glyphs, tor_colors,
                    "TranslationApp.res.rm2_font_atlas.png",
                    Color.FromArgb(0xA0, 0x00, 0x00, 0x00), false)
                },
                {
                    "RM3", new Style(rm2_glyphs, tor_colors,
                    "TranslationApp.res.rm2_font_atlas.png",
                    Color.FromArgb(0xA0, 0x00, 0x00, 0x00), false)
                },
                {
                    "TB", new Style(rm2_glyphs, tor_colors,
                    "TranslationApp.res.rm2_font_atlas.png",
                    Color.FromArgb(0x92,0x7B,0x59), false)
                },
                {
                    "TOH", new Style(toh_glyphs, tor_colors,
                    "TranslationApp.res.toh_font_atlas.png",
                    Color.FromArgb(0xA0, 0x00, 0x00, 0x00), false)
                },
            };
            InitializeComponent();
            SizeMode = PictureBoxSizeMode.AutoSize;
            Dock = DockStyle.None;
            ChangeImage("TOR");
        }

        public void ChangeImage(string id)
        {
            textbox_styles.TryGetValue(id, out current_style);
            ChangeStyle(current_style);
        }

        public void ChangeStyle(Style style)
        {
            current_style = style;
            BackColor = style.BackColor;

            if (fontAtlasImage != null)
            {
                fontAtlasImage.Dispose();
            }

            fontAtlasImage = LoadEmbeddedImage(current_style.Resource);
            Raster();
            Invalidate();
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
                    var colors = current_style.Colors;
                    var scale_factor = current_style.scale_factor;
                    Color tintColor = colors["<White>"]; // Start as white text
                    Color lastColor = tintColor;
                    Color outlineColor = colors["<White>"];
                    bool outline = false;

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
                                if (element.Contains("NoOutline"))
                                {
                                    outline = false;
                                }
                                else if (element.Contains("Outline"))
                                {
                                    outlineColor = colors[element];
                                    outline = true;
                                }
                                else
                                {
                                    lastColor = tintColor;
                                    tintColor = colors[element];
                                }
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
                            else if (element == "<lt>")
                            {
                                textToRender = "<";
                            }
                            else if (element == "<gt>")
                            {
                                textToRender = ">";
                            }
                            else if (element == "<lb>")
                            {
                                textToRender = "{";
                            }
                            else if (element == "<rb>")
                            {
                                textToRender = "}";
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
                        colorMatrix.Matrix00 = tintColor.R / scale_factor;
                        colorMatrix.Matrix11 = tintColor.G / scale_factor;
                        colorMatrix.Matrix22 = tintColor.B / scale_factor;
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
                            if (outline)
                            {
                                ColorMatrix colorMatrix1 = new ColorMatrix();
                                ImageAttributes imageAttributes1 = new ImageAttributes();
                                colorMatrix1.Matrix00 = outlineColor.R / scale_factor;
                                colorMatrix1.Matrix11 = outlineColor.G / scale_factor;
                                colorMatrix1.Matrix22 = outlineColor.B / scale_factor;
                                colorMatrix1.Matrix33 = 1.8f;
                                imageAttributes1.SetColorMatrix(colorMatrix1);

                                canvas_g.DrawImage(
                                    fontAtlasImage,
                                    Rectangle.Round(destinationRect),
                                    charRect.X - 2,
                                    charRect.Y - 2,
                                    charRect.Width,
                                    charRect.Height,
                                    GraphicsUnit.Pixel,
                                    imageAttributes1
                                );
                                canvas_g.DrawImage(
                                    fontAtlasImage,
                                    Rectangle.Round(destinationRect),
                                    charRect.X + 2,
                                    charRect.Y - 2,
                                    charRect.Width,
                                    charRect.Height,
                                    GraphicsUnit.Pixel,
                                    imageAttributes1
                                );
                                canvas_g.DrawImage(
                                    fontAtlasImage,
                                    Rectangle.Round(destinationRect),
                                    charRect.X - 2,
                                    charRect.Y + 2,
                                    charRect.Width,
                                    charRect.Height,
                                    GraphicsUnit.Pixel,
                                    imageAttributes1
                                );
                                canvas_g.DrawImage(
                                    fontAtlasImage,
                                    Rectangle.Round(destinationRect),
                                    charRect.X + 2,
                                    charRect.Y + 2,
                                    charRect.Width,
                                    charRect.Height,
                                    GraphicsUnit.Pixel,
                                    imageAttributes1
                                );
                            }

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
            List<string> lines = new List<string>();
            int space_width = GetCharacterRectangleFromAtlas(' ', out _, out _).Width;

            int oCount = words.Count;

            // Nothing to do...
            if (words.Count == 0)
                return string.Empty;

            int largestWord = words.Max(x => x.Size);
            maxWidth = Math.Max(maxWidth, largestWord);

            while (words.Count > 0)
            {
                int sum = 0;
                List<int> output = words.Select((x, ind) => sum += x.Size + (ind > 0 ? space_width : 0)).ToList();
                int count = output.TakeWhile(x => x <= maxWidth).Count();

                // Always include at least one word.
                count = Math.Max(1, count);
                lines.Add(string.Join(" ", words.Take(count).Select(x => x.Word)));
                words.RemoveRange(0, count);
            }

            return string.Join(Environment.NewLine, lines);
        }

        public List<(string, int)> GetListWords(string text)
        {
            //Split tags and text
            if (text == null)
            {
                return new List<(string, int)>();
            }
            text = string.Join(" ", text.Replace("\r", "").Split('\n'));
            string[] result = Regex.Split(text, @"(<[\w/]+:?\w+>[,|.|\[||\]]*)", RegexOptions.IgnoreCase).Where(x => x != "").ToArray();
            Rectangle space = GetCharacterRectangleFromAtlas(' ', out _, out _);

            string textToRender = "";
            List<(string Word, int Size)> wordsSize = new List<(string, int)>();

            foreach (string element in result)
            {
                string tag = "";

                int pos = element.IndexOf(">");
                if (pos > -1)
                {
                    tag = element.Substring(0, pos + 1);

                    if (names.Contains(tag))
                    {
                        textToRender = tag.Substring(1, pos - 1);
                    }
                    else if (element.StartsWith("<unk") || element.StartsWith("<var") || element.StartsWith("<icon"))
                    {
                        textToRender = "***";
                    }
                    else if (element.StartsWith("<nmb:"))
                    {
                        string el = element.Substring(5, element.Length - 6);
                        textToRender = Convert.ToInt32(el, 16).ToString();
                    }

                    if (pos < element.Length)
                        textToRender += element.Substring(pos + 1, element.Length - pos - 1);

                }
                else
                {
                    textToRender = element;
                }

                foreach (string word in textToRender.Split(' ').Where(x => x != ""))
                {
                    wordsSize.Add((word, GetWordWidth(word)));
                }
            }

            return wordsSize;
        }

        private int GetWordWidth(string word)
        {
            int width = 0;
            foreach (char c in word)
            {
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
            int x = current_style.Glyphs[index].lskip;

            charWidth -= (current_style.Glyphs[index].lskip);
            s = current_style.Glyphs[index].rskip;
            return new Rectangle(x, y, charWidth, charHeight);
        }
    }
}
