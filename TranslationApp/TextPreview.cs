﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationApp
{
    public partial class TextPreview : PictureBox
    {
        public Bitmap fontAtlasImage { get; set; }
        public string text { get; set; }

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
        #endregion

        public TextPreview()
        {
            InitializeComponent();
        }

        public void ReDraw(string text)
        {
            this.text = text;
            Invalidate();
        }

        private void TextPreview_Paint(object sender, PaintEventArgs e)
        {
            // No image, no fun
            if (fontAtlasImage == null)
            {
                return;
            }

            Graphics g = e.Graphics;
            //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;

            // Initial state
            PointF currentPosition = new PointF(0, 0);  // Starting position
            g.ScaleTransform(0.75f, 0.75f); // Scale down the size
            string textToRender = text == null ? "" : text; // Avoid null text
            Color tintColor = colors["<White>"]; // Start as white text

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
                    else if (element.Contains("nmb"))
                    {
                        string el = element.Substring(5, element.Length - 6);
                        textToRender = Convert.ToInt32(el, 16).ToString();
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
                        g.ScaleTransform(0.75f, 0.75f);
                        g.TranslateTransform(currentPosition.X, currentPosition.Y);
                        Matrix shearMatrix = new Matrix();
                        shearMatrix.Shear(-0.2f, 0.0f);
                        g.MultiplyTransform(shearMatrix);
                        g.TranslateTransform(-currentPosition.X, -currentPosition.Y);
                    }
                    else
                    {
                        g.ResetTransform();
                        g.ScaleTransform(0.75f, 0.75f);
                    }

                    // Draw the character onto the surface
                    g.DrawImage(
                        fontAtlasImage,
                        Rectangle.Round(destinationRect),
                        charRect.X,
                        charRect.Y,
                        charRect.Width,
                        charRect.Height,
                        GraphicsUnit.Pixel,
                        imageAttributes
                        );

                    g.ResetTransform();

                    // Update the current position for the next character
                    if (line)
                    {
                        currentPosition.X = 0;
                        currentPosition.Y += 24;
                    }
                    else
                    {
                        currentPosition.X += charRect.Width - shift;
                    }
                }
            }
            g.ResetTransform();
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
