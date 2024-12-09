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

        #region Glyph width data
        private readonly font_glyph[] tor_glyphs = new font_glyph[97]
        {
            /*    */ new font_glyph(18, 00),
			/* ０ */ new font_glyph(05, 06),
			/* １ */ new font_glyph(06, 08),
			/* ２ */ new font_glyph(06, 07),
			/* ３ */ new font_glyph(06, 07),
			/* ４ */ new font_glyph(05, 06),
			/* ５ */ new font_glyph(06, 06),
			/* ６ */ new font_glyph(06, 06),
			/* ７ */ new font_glyph(06, 07),
			/* ８ */ new font_glyph(06, 06),
			/* ９ */ new font_glyph(06, 06),
			/* Ａ */ new font_glyph(04, 06),
			/* Ｂ */ new font_glyph(06, 06),
			/* Ｃ */ new font_glyph(06, 06),
			/* Ｄ */ new font_glyph(05, 06),
			/* Ｅ */ new font_glyph(06, 07),
			/* Ｆ */ new font_glyph(06, 08),
			/* Ｇ */ new font_glyph(05, 07),
			/* Ｈ */ new font_glyph(05, 07),
			/* Ｉ */ new font_glyph(08, 09),
			/* Ｊ */ new font_glyph(07, 08),
			/* Ｋ */ new font_glyph(06, 06),
			/* Ｌ */ new font_glyph(07, 08),
			/* Ｍ */ new font_glyph(05, 05),
			/* Ｎ */ new font_glyph(05, 06),
			/* Ｏ */ new font_glyph(05, 05),
			/* Ｐ */ new font_glyph(06, 06),
			/* Ｑ */ new font_glyph(05, 05),
			/* Ｒ */ new font_glyph(05, 07),
			/* Ｓ */ new font_glyph(06, 07),
			/* Ｔ */ new font_glyph(06, 07),
			/* Ｕ */ new font_glyph(05, 06),
			/* Ｖ */ new font_glyph(05, 06),
			/* Ｗ */ new font_glyph(03, 03),
			/* Ｘ */ new font_glyph(05, 07),
			/* Ｙ */ new font_glyph(05, 08),
			/* Ｚ */ new font_glyph(06, 07),
			/* ａ */ new font_glyph(06, 07),
			/* ｂ */ new font_glyph(06, 07),
			/* ｃ */ new font_glyph(07, 08),
			/* ｄ */ new font_glyph(06, 07),
			/* ｅ */ new font_glyph(06, 07),
			/* ｆ */ new font_glyph(07, 09),
			/* ｇ */ new font_glyph(06, 07),
			/* ｈ */ new font_glyph(06, 07),
			/* ｉ */ new font_glyph(09, 09),
			/* ｊ */ new font_glyph(09, 10),
			/* ｋ */ new font_glyph(06, 07),
			/* ｌ */ new font_glyph(09, 09),
			/* ｍ */ new font_glyph(03, 05),
			/* ｎ */ new font_glyph(06, 07),
			/* ｏ */ new font_glyph(06, 07),
			/* ｐ */ new font_glyph(06, 07),
			/* ｑ */ new font_glyph(06, 07),
			/* ｒ */ new font_glyph(07, 08),
			/* ｓ */ new font_glyph(07, 08),
			/* ｔ */ new font_glyph(08, 08),
			/* ｕ */ new font_glyph(06, 07),
			/* ｖ */ new font_glyph(05, 07),
			/* ｗ */ new font_glyph(04, 04),
			/* ｘ */ new font_glyph(07, 07),
			/* ｙ */ new font_glyph(06, 07),
			/* ｚ */ new font_glyph(06, 08),
			/* ， */ new font_glyph(01, 15),
			/* ． */ new font_glyph(01, 15),
			/* ・ */ new font_glyph(08, 08),
			/* ： */ new font_glyph(09, 09),
			/* ； */ new font_glyph(08, 09),
			/* ？ */ new font_glyph(07, 07),
			/* ！ */ new font_glyph(09, 09),
			/* ／ */ new font_glyph(06, 07),
			/* （ */ new font_glyph(12, 01),
			/* ） */ new font_glyph(01, 13),
			/* ［ */ new font_glyph(13, 01),
			/* ］ */ new font_glyph(01, 11),
			/* ｛ */ new font_glyph(14, 01),
			/* ｝ */ new font_glyph(01, 14),
			/* ＋ */ new font_glyph(05, 06),
			/* － */ new font_glyph(08, 07),
			/* ＝ */ new font_glyph(04, 03),
			/* ＜ */ new font_glyph(03, 03),
			/* ＞ */ new font_glyph(03, 03),
			/* ％ */ new font_glyph(03, 04),
			/* ＃ */ new font_glyph(04, 04),
			/* ＆ */ new font_glyph(05, 04),
			/* ＊ */ new font_glyph(06, 08),
			/* ＠ */ new font_glyph(00, 01),
			/* ｜ */ new font_glyph(08, 08),
			/*  ” */ new font_glyph(01, 15),
			/*  ’ */ new font_glyph(01, 18),
			/* ＾ */ new font_glyph(07, 06),
			/* 「 */ new font_glyph(10, 01),
			/* 」 */ new font_glyph(01, 11),
			/* 〜 */ new font_glyph(05, 06),
			/* ＿ */ new font_glyph(05, 06),
			/* 、 */ new font_glyph(00, 13),
			/* 。 */ new font_glyph(01, 12),
        };

        private readonly font_glyph[] ndx_glyphs = new font_glyph[97]
        {
            /*    */ new font_glyph(10, 00),
            /* ０ */ new font_glyph(06, 06),
            /* １ */ new font_glyph(08, 10),
            /* ２ */ new font_glyph(06, 06),
            /* ３ */ new font_glyph(06, 06),
            /* ４ */ new font_glyph(06, 06),
            /* ５ */ new font_glyph(06, 06),
            /* ６ */ new font_glyph(06, 06),
            /* ７ */ new font_glyph(06, 06),
            /* ８ */ new font_glyph(06, 06),
            /* ９ */ new font_glyph(06, 06),
            /* Ａ */ new font_glyph(03, 03),
            /* Ｂ */ new font_glyph(06, 05),
            /* Ｃ */ new font_glyph(05, 04),
            /* Ｄ */ new font_glyph(05, 04),
            /* Ｅ */ new font_glyph(06, 05),
            /* Ｆ */ new font_glyph(06, 05),
            /* Ｇ */ new font_glyph(05, 04),
            /* Ｈ */ new font_glyph(05, 05),
            /* Ｉ */ new font_glyph(10, 10),
            /* Ｊ */ new font_glyph(07, 06),
            /* Ｋ */ new font_glyph(05, 04),
            /* Ｌ */ new font_glyph(06, 06),
            /* Ｍ */ new font_glyph(04, 04),
            /* Ｎ */ new font_glyph(05, 04),
            /* Ｏ */ new font_glyph(05, 04),
            /* Ｐ */ new font_glyph(06, 05),
            /* Ｑ */ new font_glyph(04, 03),
            /* Ｒ */ new font_glyph(06, 05),
            /* Ｓ */ new font_glyph(06, 05),
            /* Ｔ */ new font_glyph(04, 04),
            /* Ｕ */ new font_glyph(05, 05),
            /* Ｖ */ new font_glyph(04, 04),
            /* Ｗ */ new font_glyph(02, 02),
            /* Ｘ */ new font_glyph(04, 04),
            /* Ｙ */ new font_glyph(05, 05),
            /* Ｚ */ new font_glyph(05, 05),
            /* ａ */ new font_glyph(06, 06),
            /* ｂ */ new font_glyph(06, 05),
            /* ｃ */ new font_glyph(07, 06),
            /* ｄ */ new font_glyph(07, 05),
            /* ｅ */ new font_glyph(07, 07),
            /* ｆ */ new font_glyph(08, 07),
            /* ｇ */ new font_glyph(06, 05),
            /* ｈ */ new font_glyph(06, 06),
            /* ｉ */ new font_glyph(10, 10),
            /* ｊ */ new font_glyph(08, 09),
            /* ｋ */ new font_glyph(06, 05),
            /* ｌ */ new font_glyph(10, 09),
            /* ｍ */ new font_glyph(04, 02),
            /* ｎ */ new font_glyph(06, 06),
            /* ｏ */ new font_glyph(06, 06),
            /* ｐ */ new font_glyph(06, 05),
            /* ｑ */ new font_glyph(06, 05),
            /* ｒ */ new font_glyph(08, 07),
            /* ｓ */ new font_glyph(08, 07),
            /* ｔ */ new font_glyph(08, 07),
            /* ｕ */ new font_glyph(06, 07),
            /* ｖ */ new font_glyph(06, 06),
            /* ｗ */ new font_glyph(03, 03),
            /* ｘ */ new font_glyph(06, 08),
            /* ｙ */ new font_glyph(05, 07),
            /* ｚ */ new font_glyph(06, 07),
            /* ， */ new font_glyph(09, 09),
            /* ． */ new font_glyph(09, 09),
            /* ・ */ new font_glyph(09, 09),
            /* ： */ new font_glyph(09, 09),
            /* ； */ new font_glyph(09, 09),
            /* ？ */ new font_glyph(06, 05),
            /* ！ */ new font_glyph(09, 09),
            /* ／ */ new font_glyph(06, 06),
            /* （ */ new font_glyph(09, 08),
            /* ） */ new font_glyph(09, 08),
            /* ［ */ new font_glyph(09, 08),
            /* ］ */ new font_glyph(09, 08),
            /* ｛ */ new font_glyph(09, 08),
            /* ｝ */ new font_glyph(09, 08),
            /* ＋ */ new font_glyph(04, 03),
            /* － */ new font_glyph(08, 08),
            /* ＝ */ new font_glyph(04, 03),
            /* ＜ */ new font_glyph(06, 06),
            /* ＞ */ new font_glyph(06, 06),
            /* ％ */ new font_glyph(04, 03),
            /* ＃ */ new font_glyph(04, 04),
            /* ＆ */ new font_glyph(04, 03),
            /* ＊ */ new font_glyph(04, 04),
            /* ＠ */ new font_glyph(00, 01),
            /* ｜ */ new font_glyph(08, 08),
            /*  ” */ new font_glyph(01, 15),
            /*  ’ */ new font_glyph(01, 18),
            /* ＾ */ new font_glyph(07, 06),
            /* 「 */ new font_glyph(10, 01),
            /* 」 */ new font_glyph(01, 11),
            /* 〜 */ new font_glyph(05, 06),
            /* ＿ */ new font_glyph(06, 05),
            /* 、 */ new font_glyph(00, 13),
            /* 。 */ new font_glyph(01, 12),
        };

        private readonly font_glyph[] rm2_glyphs = new font_glyph[97]
        {
            /*    */ new font_glyph(16, 00),
            /* ０ */ new font_glyph(03, 03),
            /* １ */ new font_glyph(06, 09),
            /* ２ */ new font_glyph(03, 03),
            /* ３ */ new font_glyph(03, 03),
            /* ４ */ new font_glyph(03, 03),
            /* ５ */ new font_glyph(03, 03),
            /* ６ */ new font_glyph(03, 03),
            /* ７ */ new font_glyph(03, 03),
            /* ８ */ new font_glyph(03, 03),
            /* ９ */ new font_glyph(03, 03),
            /* Ａ */ new font_glyph(03, 04),
            /* Ｂ */ new font_glyph(04, 04),
            /* Ｃ */ new font_glyph(03, 02),
            /* Ｄ */ new font_glyph(04, 04),
            /* Ｅ */ new font_glyph(04, 04),
            /* Ｆ */ new font_glyph(04, 03),
            /* Ｇ */ new font_glyph(04, 03),
            /* Ｈ */ new font_glyph(03, 03),
            /* Ｉ */ new font_glyph(08, 08),
            /* Ｊ */ new font_glyph(04, 05),
            /* Ｋ */ new font_glyph(04, 03),
            /* Ｌ */ new font_glyph(06, 03),
            /* Ｍ */ new font_glyph(02, 03),
            /* Ｎ */ new font_glyph(04, 03),
            /* Ｏ */ new font_glyph(03, 03),
            /* Ｐ */ new font_glyph(04, 04),
            /* Ｑ */ new font_glyph(03, 03),
            /* Ｒ */ new font_glyph(04, 04),
            /* Ｓ */ new font_glyph(04, 03),
            /* Ｔ */ new font_glyph(04, 04),
            /* Ｕ */ new font_glyph(03, 03),
            /* Ｖ */ new font_glyph(03, 04),
            /* Ｗ */ new font_glyph(01, 00),
            /* Ｘ */ new font_glyph(03, 02),
            /* Ｙ */ new font_glyph(04, 03),
            /* Ｚ */ new font_glyph(03, 03),
            /* ａ */ new font_glyph(05, 04),
            /* ｂ */ new font_glyph(05, 04),
            /* ｃ */ new font_glyph(05, 04),
            /* ｄ */ new font_glyph(05, 05),
            /* ｅ */ new font_glyph(05, 05),
            /* ｆ */ new font_glyph(07, 07),
            /* ｇ */ new font_glyph(05, 04),
            /* ｈ */ new font_glyph(06, 04),
            /* ｉ */ new font_glyph(08, 08),
            /* ｊ */ new font_glyph(08, 09),
            /* ｋ */ new font_glyph(06, 04),
            /* ｌ */ new font_glyph(08, 08),
            /* ｍ */ new font_glyph(02, 02),
            /* ｎ */ new font_glyph(06, 04),
            /* ｏ */ new font_glyph(05, 04),
            /* ｐ */ new font_glyph(06, 05),
            /* ｑ */ new font_glyph(05, 05),
            /* ｒ */ new font_glyph(08, 06),
            /* ｓ */ new font_glyph(05, 05),
            /* ｔ */ new font_glyph(08, 07),
            /* ｕ */ new font_glyph(05, 05),
            /* ｖ */ new font_glyph(04, 04),
            /* ｗ */ new font_glyph(03, 02),
            /* ｘ */ new font_glyph(05, 05),
            /* ｙ */ new font_glyph(05, 05),
            /* ｚ */ new font_glyph(05, 05),
            /* ， */ new font_glyph(09, 09),
            /* ． */ new font_glyph(06, 08),
            /* ・ */ new font_glyph(09, 09),
            /* ： */ new font_glyph(09, 09),
            /* ； */ new font_glyph(09, 09),
            /* ？ */ new font_glyph(04, 03),
            /* ！ */ new font_glyph(09, 09),
            /* ／ */ new font_glyph(06, 06),
            /* （ */ new font_glyph(08, 07),
            /* ） */ new font_glyph(08, 08),
            /* ［ */ new font_glyph(08, 07),
            /* ］ */ new font_glyph(08, 08),
            /* ｛ */ new font_glyph(09, 08),
            /* ｝ */ new font_glyph(09, 08),
            /* ＋ */ new font_glyph(04, 03),
            /* － */ new font_glyph(08, 08),
            /* ＝ */ new font_glyph(04, 03),
            /* ＜ */ new font_glyph(06, 06),
            /* ＞ */ new font_glyph(06, 06),
            /* ％ */ new font_glyph(02, 01),
            /* ＃ */ new font_glyph(04, 04),
            /* ＆ */ new font_glyph(04, 03),
            /* ＊ */ new font_glyph(04, 04),
            /* ＠ */ new font_glyph(00, 01),
            /* ｜ */ new font_glyph(08, 08),
            /*  ” */ new font_glyph(09, 09),
            /*  ’ */ new font_glyph(10, 10),
            /* ＾ */ new font_glyph(07, 06),
            /* 「 */ new font_glyph(10, 01),
            /* 」 */ new font_glyph(01, 11),
            /* 〜 */ new font_glyph(05, 06),
            /* ＿ */ new font_glyph(06, 05),
            /* 、 */ new font_glyph(00, 13),
            /* 。 */ new font_glyph(01, 12),
        };
        #endregion

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

            charWidth -= glyphs[index].lskip;
            s = glyphs[index].rskip;
            return new Rectangle(x, y, charWidth, charHeight);
        }
    }
}
