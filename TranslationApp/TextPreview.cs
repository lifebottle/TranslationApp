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
            public byte width;

            public font_glyph(byte x, byte y)
            {
                lskip = x;
                width = y;
            }
        }

        private float scale = 1.0f;
        private float finalRenderScale = 1.0f;

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

        private readonly font_glyph[] toh_glyphs = new font_glyph[97]
        {
            /*    */ new font_glyph(00, 18),
            /* ０ */ new font_glyph(00, 13),
            /* １ */ new font_glyph(00, 19),
            /* ２ */ new font_glyph(00, 13),
            /* ３ */ new font_glyph(00, 13),
            /* ４ */ new font_glyph(00, 13),
            /* ５ */ new font_glyph(00, 13),
            /* ６ */ new font_glyph(00, 13),
            /* ７ */ new font_glyph(00, 13),
            /* ８ */ new font_glyph(00, 13),
            /* ９ */ new font_glyph(00, 13),
            /* Ａ */ new font_glyph(00, 13),
            /* Ｂ */ new font_glyph(00, 13),
            /* Ｃ */ new font_glyph(00, 13),
            /* Ｄ */ new font_glyph(00, 13),
            /* Ｅ */ new font_glyph(00, 13),
            /* Ｆ */ new font_glyph(00, 13),
            /* Ｇ */ new font_glyph(00, 13),
            /* Ｈ */ new font_glyph(00, 13),
            /* Ｉ */ new font_glyph(00, 13),
            /* Ｊ */ new font_glyph(00, 21),
            /* Ｋ */ new font_glyph(00, 13),
            /* Ｌ */ new font_glyph(00, 13),
            /* Ｍ */ new font_glyph(00, 13),
            /* Ｎ */ new font_glyph(00, 13),
            /* Ｏ */ new font_glyph(00, 13),
            /* Ｐ */ new font_glyph(00, 13),
            /* Ｑ */ new font_glyph(00, 13),
            /* Ｒ */ new font_glyph(00, 13),
            /* Ｓ */ new font_glyph(00, 13),
            /* Ｔ */ new font_glyph(00, 13),
            /* Ｕ */ new font_glyph(00, 13),
            /* Ｖ */ new font_glyph(00, 13),
            /* Ｗ */ new font_glyph(00, 13),
            /* Ｘ */ new font_glyph(00, 13),
            /* Ｙ */ new font_glyph(00, 13),
            /* Ｚ */ new font_glyph(00, 13),
            /* ａ */ new font_glyph(00, 13),
            /* ｂ */ new font_glyph(00, 13),
            /* ｃ */ new font_glyph(00, 13),
            /* ｄ */ new font_glyph(00, 13),
            /* ｅ */ new font_glyph(00, 13),
            /* ｆ */ new font_glyph(00, 13),
            /* ｇ */ new font_glyph(00, 13),
            /* ｈ */ new font_glyph(00, 13),
            /* ｉ */ new font_glyph(00, 21),
            /* ｊ */ new font_glyph(00, 15),
            /* ｋ */ new font_glyph(00, 15),
            /* ｌ */ new font_glyph(00, 19),
            /* ｍ */ new font_glyph(00, 13),
            /* ｎ */ new font_glyph(00, 13),
            /* ｏ */ new font_glyph(00, 13),
            /* ｐ */ new font_glyph(00, 13),
            /* ｑ */ new font_glyph(00, 13),
            /* ｒ */ new font_glyph(00, 13),
            /* ｓ */ new font_glyph(00, 14),
            /* ｔ */ new font_glyph(00, 14),
            /* ｕ */ new font_glyph(00, 15),
            /* ｖ */ new font_glyph(00, 13),
            /* ｗ */ new font_glyph(00, 13),
            /* ｘ */ new font_glyph(00, 13),
            /* ｙ */ new font_glyph(00, 13),
            /* ｚ */ new font_glyph(00, 13),
            /* ， */ new font_glyph(00, 20),
            /* ． */ new font_glyph(00, 20),
            /* ・ */ new font_glyph(00, 20),
            /* ： */ new font_glyph(00, 20),
            /* ； */ new font_glyph(00, 20),
            /* ？ */ new font_glyph(00, 14),
            /* ！ */ new font_glyph(00, 22),
            /* ／ */ new font_glyph(00, 14),
            /* （ */ new font_glyph(00, 18),
            /* ） */ new font_glyph(00, 18),
            /* ［ */ new font_glyph(00, 18),
            /* ］ */ new font_glyph(00, 18),
            /* ｛ */ new font_glyph(00, 16),
            /* ｝ */ new font_glyph(00, 16),
            /* ＋ */ new font_glyph(00, 14),
            /* － */ new font_glyph(00, 20),
            /* ＝ */ new font_glyph(00, 14),
            /* ＜ */ new font_glyph(00, 18),
            /* ＞ */ new font_glyph(00, 18),
            /* ％ */ new font_glyph(00, 14),
            /* ＃ */ new font_glyph(00, 14),
            /* ＆ */ new font_glyph(00, 14),
            /* ＊ */ new font_glyph(00, 14),
            /* ＠ */ new font_glyph(00, 14),
            /* ｜ */ new font_glyph(00, 22),
            /*  ” */ new font_glyph(00, 18),
            /*  ’ */ new font_glyph(00, 22),
            /* ＾ */ new font_glyph(00, 16),
            /* 「 */ new font_glyph(00, 18),
            /* 」 */ new font_glyph(00, 18),
            /* 〜 */ new font_glyph(00, 14),
            /* ＿ */ new font_glyph(00, 14),
            /* 、 */ new font_glyph(00, 20),
            /* 。 */ new font_glyph(00, 16),
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

        private readonly font_glyph[] srwz_glyphs = new font_glyph[]
        {
            /*   */ new font_glyph(0,6),
            /* ! */ new font_glyph(0,4),
            /* " */ new font_glyph(0,4),
            /* # */ new font_glyph(0,13),
            /* $ */ new font_glyph(0,13),
            /* % */ new font_glyph(0,13),
            /* & */ new font_glyph(0,13),
            /* ' */ new font_glyph(0,3),
            /* ( */ new font_glyph(0,6),
            /* ) */ new font_glyph(0,6),
            /* * */ new font_glyph(0,13),
            /* + */ new font_glyph(0,13),
            /* , */ new font_glyph(0,3),
            /* - */ new font_glyph(0,13),
            /* . */ new font_glyph(0,3),
            /* / */ new font_glyph(0,13),
            /* 0 */ new font_glyph(0,12),
            /* 1 */ new font_glyph(0,11),
            /* 2 */ new font_glyph(0,11),
            /* 3 */ new font_glyph(0,11),
            /* 4 */ new font_glyph(0,11),
            /* 5 */ new font_glyph(0,11),
            /* 6 */ new font_glyph(0,11),
            /* 7 */ new font_glyph(0,11),
            /* 8 */ new font_glyph(0,11),
            /* 9 */ new font_glyph(0,11),
            /* : */ new font_glyph(0,13),
            /* ; */ new font_glyph(0,13),
            /* < */ new font_glyph(0,13),
            /* = */ new font_glyph(0,13),
            /* > */ new font_glyph(0,13),
            /* ? */ new font_glyph(0,13),
            /* @ */ new font_glyph(0,13),
            /* A */ new font_glyph(0,12),
            /* B */ new font_glyph(0,12),
            /* C */ new font_glyph(0,12),
            /* D */ new font_glyph(0,11),
            /* E */ new font_glyph(0,11),
            /* F */ new font_glyph(0,9),
            /* G */ new font_glyph(0,13),
            /* H */ new font_glyph(0,12),
            /* I */ new font_glyph(0,3),
            /* J */ new font_glyph(0,9),
            /* K */ new font_glyph(0,12),
            /* L */ new font_glyph(0,11),
            /* M */ new font_glyph(0,14),
            /* N */ new font_glyph(0,13),
            /* O */ new font_glyph(0,13),
            /* P */ new font_glyph(0,12),
            /* Q */ new font_glyph(0,14),
            /* R */ new font_glyph(0,12),
            /* S */ new font_glyph(0,11),
            /* T */ new font_glyph(0,13),
            /* U */ new font_glyph(0,12),
            /* V */ new font_glyph(0,12),
            /* W */ new font_glyph(0,15),
            /* X */ new font_glyph(0,12),
            /* Y */ new font_glyph(0,11),
            /* Z */ new font_glyph(0,10),
            /* [ */ new font_glyph(0,7),
            /* \ */ new font_glyph(0,7),
            /* ] */ new font_glyph(0,7),
            /*   */ new font_glyph(0,7),
            /* ^ */ new font_glyph(0,13),
            /* _ */ new font_glyph(0,13),
            /* ` */ new font_glyph(0,13),
            /* a */ new font_glyph(0,8),
            /* b */ new font_glyph(0,9),
            /* c */ new font_glyph(0,9),
            /* d */ new font_glyph(0,10),
            /* e */ new font_glyph(0,8),
            /* f */ new font_glyph(0,8),
            /* g */ new font_glyph(0,9),
            /* h */ new font_glyph(0,9),
            /* i */ new font_glyph(0,3),
            /* j */ new font_glyph(0,7),
            /* k */ new font_glyph(0,9),
            /* l */ new font_glyph(0,3),
            /* m */ new font_glyph(0,14),
            /* n */ new font_glyph(0,9),
            /* o */ new font_glyph(0,9),
            /* p */ new font_glyph(0,9),
            /* q */ new font_glyph(0,10),
            /* r */ new font_glyph(0,6),
            /* s */ new font_glyph(0,8),
            /* t */ new font_glyph(0,7),
            /* u */ new font_glyph(0,9),
            /* v */ new font_glyph(0,9),
            /* w */ new font_glyph(0,10),
            /* x */ new font_glyph(0,8),
            /* y */ new font_glyph(0,8),
            /* z */ new font_glyph(0,10),
            /* { */ new font_glyph(0,10),
            /* | */ new font_glyph(0,10),
            /* } */ new font_glyph(0,10),
            /* ~ */ new font_glyph(0,10),
            /* ≥ */ new font_glyph(0,10)

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
            scale = 1.0f;
            finalRenderScale = 1.0f;

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

                case "SRWZ":
                    res = "TranslationApp.res.srwz_font_atlas.png";
                    BackColor = Color.FromArgb(0xA0, 0x0, 0x0, 0x0);
                    glyphs = srwz_glyphs;
                    scale = 0.8f;
                    finalRenderScale = 0.8f;
                    break;

                default:
                    fontAtlasImage = null;
                    glyphs = srwz_glyphs;
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

        public int GetLayoutWidthFromRenderedWidth(int renderedWidth)
        {
            float safeScale = Math.Max(0.01f, finalRenderScale);
            return Math.Max(1, (int)Math.Ceiling(renderedWidth / safeScale));
        }

        public int GetRenderedContentWidthForPreview(int previewWidth)
        {
            float safeScale = Math.Max(0.01f, finalRenderScale);

            // Raster() starts text 10 layout pixels from the left and adds
            // another 10 layout pixels on the right of the rendered image.
            int renderedHorizontalPadding = (int)Math.Ceiling(20 * safeScale);
            return Math.Max(1, previewWidth - renderedHorizontalPadding);
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
                            Rectangle charRect = GetCharacterRectangleFromAtlas(c, out int glyphWidth, out bool line);

                            // Create a destination rectangle using the currentPosition
                            RectangleF destinationRect = new RectangleF(
                                currentPosition,
                                new SizeF(charRect.Width * scale, charRect.Height * scale));

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
                                currentPosition.Y += 24 * scale;
                            }
                            else
                            {
                                // glyphWidth is already stored at the intended font scale.
                                currentPosition.X += (glyphWidth + 1);
                                finalWidth = Math.Max(finalWidth, currentPosition.X);
                            }
                        }
                    }

                    int outputWidth = Math.Max(1, (int)Math.Ceiling(finalWidth + 10));
                    int outputHeight = Math.Max(
                        1,
                        (int)Math.Ceiling(currentPosition.Y + (24 * scale) + 10));

                    int finalOutputWidth = Math.Max(
                        1,
                        (int)Math.Ceiling(outputWidth * finalRenderScale));
                    int finalOutputHeight = Math.Max(
                        1,
                        (int)Math.Ceiling(outputHeight * finalRenderScale));

                    currentImage = new Bitmap(
                        finalOutputWidth,
                        finalOutputHeight,
                        PixelFormat.Format32bppArgb);

                    // Scale the completed rendering without changing glyph layout metrics.
                    using (Graphics canvas_f = Graphics.FromImage(currentImage))
                    {
                        canvas_f.Clear(Color.Transparent);
                        canvas_f.CompositingMode = CompositingMode.SourceCopy;
                        canvas_f.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        canvas_f.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        canvas_f.DrawImage(
                            canvas,
                            new Rectangle(0, 0, finalOutputWidth, finalOutputHeight),
                            new Rectangle(0, 0, outputWidth, outputHeight),
                            GraphicsUnit.Pixel);
                    }
                    Image = currentImage;
                }
            }
        }

        


        public string DoLineBreak(string text, int maxWidth)
        {
            if (string.IsNullOrWhiteSpace(text) || glyphs == null)
            {
                return text ?? string.Empty;
            }

            List<(string Text, int Width)> words = GetLineBreakWords(text);
            if (words.Count == 0)
            {
                return string.Empty;
            }

            maxWidth = Math.Max(1, maxWidth);
            int spaceWidth = GetRenderedTextWidth(" ");
            int currentLineWidth = 0;
            bool hasContentOnLine = false;
            StringBuilder output = new StringBuilder();

            foreach ((string wordText, int wordWidth) in words)
            {
                int separatorWidth = hasContentOnLine ? spaceWidth : 0;

                // Keep words intact. A word wider than maxWidth is placed alone.
                if (hasContentOnLine &&
                    currentLineWidth + separatorWidth + wordWidth > maxWidth)
                {
                    output.Append(Environment.NewLine);
                    currentLineWidth = 0;
                    hasContentOnLine = false;
                    separatorWidth = 0;
                }

                if (hasContentOnLine)
                {
                    output.Append(' ');
                    currentLineWidth += separatorWidth;
                }

                output.Append(wordText);
                currentLineWidth += wordWidth;
                hasContentOnLine = true;
            }

            return output.ToString();
        }

        private List<(string Text, int Width)> GetLineBreakWords(string source)
        {
            List<(string Text, int Width)> words = new List<(string, int)>();
            StringBuilder currentWord = new StringBuilder();
            int currentWidth = 0;

            // Tags remain in the returned text but only their visible replacement
            // contributes to the measured width. Whitespace is normalized to one
            // separator so existing line breaks can be reflowed.
            MatchCollection tokens = Regex.Matches(
                source,
                @"<[^>]*>|\s+|[^<\s]+|<",
                RegexOptions.IgnoreCase);

            foreach (Match token in tokens)
            {
                string value = token.Value;
                if (Regex.IsMatch(value, @"^\s+$"))
                {
                    if (currentWord.Length > 0)
                    {
                        words.Add((currentWord.ToString(), currentWidth));
                        currentWord.Clear();
                        currentWidth = 0;
                    }
                    continue;
                }

                currentWord.Append(value);
                if (value.StartsWith("<") && value.EndsWith(">"))
                {
                    currentWidth += GetTagRenderedWidth(value);
                }
                else
                {
                    currentWidth += GetRenderedTextWidth(value);
                }
            }

            if (currentWord.Length > 0)
            {
                words.Add((currentWord.ToString(), currentWidth));
            }

            return words;
        }

        private int GetTagRenderedWidth(string tag)
        {
            if (colors.ContainsKey(tag) ||
                tag == "<Italic>" ||
                tag == "</Italic>" ||
                tag.StartsWith("<color:"))
            {
                return 0;
            }

            string renderedText;
            if (names.Contains(tag))
            {
                renderedText = tag.Substring(1, tag.Length - 2);
            }
            else if (tag.StartsWith("<unk") ||
                     tag.StartsWith("<var") ||
                     tag.StartsWith("<icon"))
            {
                renderedText = "***";
            }
            else if (tag.StartsWith("<nmb:") && tag.EndsWith(">"))
            {
                string hexadecimalValue = tag.Substring(5, tag.Length - 6);
                if (!int.TryParse(
                    hexadecimalValue,
                    System.Globalization.NumberStyles.HexNumber,
                    null,
                    out int number))
                {
                    return 0;
                }
                renderedText = number.ToString();
            }
            else
            {
                // Unknown control tags are ignored by Raster().
                return 0;
            }

            return GetRenderedTextWidth(renderedText);
        }

        private int GetRenderedTextWidth(string renderedText)
        {
            int width = 0;
            foreach (char character in renderedText)
            {
                GetCharacterRectangleFromAtlas(character, out int glyphWidth, out bool addLine);
                if (!addLine)
                {
                    // This must match the character advance used by Raster().
                    width += glyphWidth + 1;
                }
            }
            return width;
        }

        public string WordWrap(string text, int maxWidth)
        {
            GetCharacterRectangleFromAtlas(' ', out int spaceWidth, out bool add);
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
                curLineLength += width + spaceWidth;
            }

            return strBuilder.ToString();
        }

        public List<(string, int)> GetListWords(string text)
        {
            //Split tags and text
            string[] result = Regex.Split(text.Replace("\r", ""), @"(<[\w/]+:?\w+>[,|.|\[||\]]*)", RegexOptions.IgnoreCase).Where(x => x != "").ToArray();
            GetCharacterRectangleFromAtlas(' ', out int spaceWidth, out bool add);

            string textToRender = "";
            bool shear = false;
            List<(string Word, int Size)> wordsSize = new List<(string, int)>();

            foreach (string element in result)
            {
                
                int d = 0;
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
                    wordsSize.Add((" ", spaceWidth));

                }
            }

            wordsSize.RemoveAt(wordsSize.Count - 1);
            return wordsSize;
        }

        private int GetWordWidth(string word)
        {
            int width = 0;
            foreach (char c in word) {
                GetCharacterRectangleFromAtlas(c, out int glyphWidth, out bool b);
                width += glyphWidth;
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

            int index;
            if (character == '\n')
            {
                index = 0;
                addline = true;
            }
            else
            {
                // The SRWZ atlas is ordered consecutively from ASCII 0x20.
                index = character - 0x20;

                if (character >= 0x61)
                    index += 1;
            }

            // Unsupported characters use the first (space) glyph.
            if (index < 0 || index >= glyphs.Length)
            {
                index = 0;
            }

            // Source coordinates always remain in the native 24x24 atlas.
            int y = index * charHeight;
            int x = glyphs[index].lskip;

            charWidth -= glyphs[index].lskip;
            s = glyphs[index].width;
            return new Rectangle(x, y, charWidth, charHeight);
        }
    }
}
