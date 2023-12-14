// ==========================================================================
// 
// File:        LOC.vb
// Location:    Firefly.Texting <Visual Basic .Net>
// Description: 图形文本文件类
// Version:     2009.10.31.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using static System.Math;
using System.Text;
using Firefly.Glyphing;
using Firefly.Imaging;
using Firefly.TextEncoding;

namespace Firefly.Texting
{
    /// <summary>图形文本文件类</summary>
    public class LOC
    {
        public static readonly string IdentifierCompression = "LOCC";
        public static readonly string Identifier = "LOC1";

        protected FontLib FontLibValue;
        protected GlyphMap GlyphMapValue;
        protected Dictionary<CharCode, int> CharGlyphDictValue = new Dictionary<CharCode, int>();
        protected CharCode[][] TextValue;

        /// <summary>字库。不要修改该字库，可以考虑创建LOC的新实例，以使得LOC的内部状态正常。</summary>
        public FontLib FontLib
        {
            get
            {
                return FontLibValue;
            }
        }

        /// <summary>字形图。不要修改该字形图，可以考虑创建LOC的新实例，以使得LOC的内部状态正常。</summary>
        public GlyphMap GlyphMap
        {
            get
            {
                return GlyphMapValue;
            }
        }

        /// <summary>字符码点-字形号映射。不要修改，可以考虑创建LOC的新实例，以使得LOC的内部状态正常。</summary>
        public Dictionary<CharCode, int> CharGlyphDict
        {
            get
            {
                return CharGlyphDictValue;
            }
        }

        /// <summary>文本。不要修改该文本，可以考虑创建LOC的新实例，以使得LOC的内部状态正常。</summary>
        public CharCode[][] Text
        {
            get
            {
                return TextValue;
            }
        }


        /// <summary>已重载。创建新的图形文本类。</summary>
        public LOC(FontLib FontLib, GlyphMap GlyphMap, Dictionary<CharCode, int> CharGlyphDict, CharCode[][] Text)
        {
            FontLibValue = FontLib;
            GlyphMapValue = GlyphMap;
            CharGlyphDictValue = CharGlyphDict;
            TextValue = Text;
        }

        /// <summary>生成图形文本文件。生成32位BMP。若需其他BMP，可仿照此函数生成。所有字库字符大小不得大于最大的字符宽度和高度。</summary>
        public static LOC GenerateLOC(FontLib FontLib, CharCode[][] Text)
        {
            var CharGlyphDict = new Dictionary<CharCode, int>();
            int GlyphWidth = 0;
            int GlyphHeight = 0;
            int GlyphCount = 0;
            foreach (var c in FontLib.CharCodes)
            {
                if (FontLib.get_HasGlyph(c))
                {
                    var Glyph = FontLib[c];
                    if (Glyph.PhysicalWidth > GlyphWidth)
                        GlyphWidth = Glyph.PhysicalWidth;
                    if (Glyph.PhysicalHeight > GlyphHeight)
                        GlyphHeight = Glyph.PhysicalHeight;

                    CharGlyphDict.Add(c, GlyphCount);
                    GlyphCount += 1;
                }
            }

            var g = new GlyphMap(GlyphCount, GlyphWidth, GlyphHeight);
            foreach (var Pair in CharGlyphDict)
            {
                var Glyph = FontLib[Pair.Key];
                int[,] Block = Glyph.Block;
                g.WidthTable[Pair.Value] = (byte)Glyph.VirtualBox.Width;
                var r = g.GetGlyphPhysicalBox(Pair.Value);
                if (Block.GetLength(0) > r.Width)
                    throw new InvalidDataException();
                if (Block.GetLength(1) > r.Height)
                    throw new InvalidDataException();
                g.Bmp.SetRectangle(r.X, r.Y, Block);
            }

            return new LOC(FontLib, g, CharGlyphDict, Text);
        }

        public static object GenerateEmptyLOC(int Count)
        {
            return GenerateLOC(new FontLib(), (from i in Enumerable.Range(0, Count)
                                               select new CharCode[] { }).ToArray());
        }

        /// <summary>已重载。从流读取图形文本文件。</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "CharInfoDBLength")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "TextInfoDBLength")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "TextLength")]
        public LOC(ZeroPositionStreamPasser sp)
        {
            var s = sp.GetStream();
            bool Close = false;
            try
            {
                string Id = s.ReadSimpleString(4);
                if ((Id ?? "") == (IdentifierCompression ?? ""))
                {
                    var gz = new PartialStreamEx(s, 4L, s.Length - 4L);
                    using (var gzDec = new System.IO.Compression.GZipStream(gz.ToUnsafeStream(), System.IO.Compression.CompressionMode.Decompress, false))
                    {
                        s = new StreamEx();
                        while (true)
                        {
                            int b = gzDec.ReadByte();
                            if (b == -1)
                                break;
                            s.WriteByte((byte)b);
                        }
                    }
                    s.Position = 0L;

                    Close = true;
                    Id = s.ReadSimpleString(4);
                }

                if ((Id ?? "") != (Identifier ?? ""))
                    throw new InvalidDataException();


                int NumSection = s.ReadInt32();

                var FontLibSectionAddress = default(int);
                var FontLibSectionLength = default(int);
                if (NumSection >= 1)
                {
                    FontLibSectionAddress = s.ReadInt32();
                    FontLibSectionLength = s.ReadInt32();
                }
                var TextAddress = default(int);
                int TextLength;
                if (NumSection >= 2)
                {
                    TextAddress = s.ReadInt32();
                    TextLength = s.ReadInt32();
                }


                if (NumSection <= 0)
                    return;
                s.Position = FontLibSectionAddress;
                if (FontLibSectionLength > 0)
                {
                    int CharCount = s.ReadInt32();
                    CharCode[] CharCode = new CharCode[CharCount];
                    int CharInfoDBLength = s.ReadInt32(); // 暂时不用
                    for (int n = 0, loopTo = CharCount - 1; n <= loopTo; n++)
                    {
                        int Index = s.ReadInt32();
                        if (Index != n)
                            throw new InvalidDataException();

                        int GlyphIndex = s.ReadInt32();
                        int Unicode = s.ReadInt32();
                        int Code = s.ReadInt32();

                        CharCode[n] = new CharCode(Unicode, Code);

                        if (GlyphIndex != -1)
                            CharGlyphDictValue.Add(CharCode[n], GlyphIndex);
                    }

                    FontLibValue = new FontLib(CharCode);

                    int GlyphCount = s.ReadInt32();
                    int GlyphWidth = s.ReadInt32();
                    int GlyphHeight = s.ReadInt32();
                    int WidthTableLength = s.ReadInt32();
                    byte[] WidthTable = new byte[GlyphCount];
                    if (WidthTableLength > 0)
                    {
                        for (int n = 0, loopTo1 = GlyphCount - 1; n <= loopTo1; n++)
                            WidthTable[n] = s.ReadByte();
                        s.Position += WidthTableLength - GlyphCount;
                    }
                    s.Position = (s.Position + 15L) / 16L * 16L;
                    int BitmapLength = s.ReadInt32();
                    Bmp Bitmap = null;
                    if (BitmapLength > 0)
                    {
                        var BitmapStream = new PartialStreamEx(s, s.Position, BitmapLength);
                        var ms = new StreamEx();
                        ms.WriteFromStream(BitmapStream, BitmapStream.Length);
                        ms.Position = 0L;
                        Bitmap = Bmp.Open(ms);
                        s.Position += BitmapLength - BitmapStream.Position;
                    }
                    GlyphMapValue = new GlyphMap(GlyphCount, GlyphWidth, GlyphHeight, WidthTable, Bitmap);

                    foreach (var c in CharCode)
                    {
                        if (CharGlyphDictValue.ContainsKey(c))
                        {
                            var r = GlyphMapValue.GetGlyphPhysicalBox(CharGlyphDictValue[c]);
                            FontLibValue[c] = new Glyph() { c = c, Block = GlyphMapValue.Bmp.GetRectangleAsARGB(r.X, r.Y, r.Width, r.Height), VirtualBox = GlyphMapValue.GetGlyphVirtualBox(CharGlyphDictValue[c]) };
                        }
                    }
                }


                if (NumSection <= 1)
                    return;
                s.Position = TextAddress;
                int TextCount = s.ReadInt32();
                int TextInfoDBLength = s.ReadInt32(); // 暂时不用

                int[] TextInfoAddress = new int[TextCount];
                int[] TextInfoLength = new int[TextCount];
                for (int n = 0, loopTo2 = TextCount - 1; n <= loopTo2; n++)
                {
                    TextInfoAddress[n] = s.ReadInt32();
                    TextInfoLength[n] = s.ReadInt32();
                }
                int[][] TextCharIndex = new int[TextCount][];
                for (int n = 0, loopTo3 = TextCount - 1; n <= loopTo3; n++)
                {
                    s.Position = TextAddress + TextInfoAddress[n];
                    byte[] VLEData = s.Read(TextInfoLength[n]);
                    TextCharIndex[n] = VariableLengthDecode(VLEData);
                }
                TextValue = new CharCode[TextCount][];
                for (int n = 0, loopTo4 = TextCount - 1; n <= loopTo4; n++)
                {
                    int[] Original = TextCharIndex[n];
                    CharCode[] SingleText = new CharCode[TextCharIndex[n].Length];
                    for (int k = 0, loopTo5 = TextCharIndex[n].Length - 1; k <= loopTo5; k++)
                        SingleText[k] = FontLibValue.CharCodes[Original[k]];
                    TextValue[n] = SingleText;
                }
            }
            finally
            {
                if (Close)
                    s.Close();
            }
        }

        /// <summary>写入图形文本文件到流。</summary>
        public void WriteToFile(ZeroPositionStreamPasser sp, bool Compress = false)
        {
            var f = sp.GetStream();
            using (var s = new StreamEx())
            {
                s.WriteSimpleString(Identifier, 4);
                int NumSection = 2;
                s.WriteInt32(NumSection);


                int FontLibSectionAddress;
                int FontLibSectionLength;
                int TextAddress;
                int TextLength;
                s.Position += 8 * NumSection;
                s.Position = (s.Position + 15L) / 16L * 16L;


                FontLibSectionAddress = (int)s.Position;
                var CodeIndexDict = new Dictionary<CharCode, int>();
                int CharCount = FontLibValue.CharCount;
                var CharCodes = FontLibValue.CharCodes;
                for (int n = 0, loopTo = CharCount - 1; n <= loopTo; n++)
                    CodeIndexDict.Add(CharCodes[n], n);
                if (FontLibValue is not null)
                {
                    s.WriteInt32(CharCount);
                    s.WriteInt32(16);
                    for (int n = 0, loopTo1 = CharCount - 1; n <= loopTo1; n++)
                    {
                        var c = CharCodes[n];
                        s.WriteInt32(n);
                        if (FontLibValue.get_HasGlyph(c))
                        {
                            s.WriteInt32(CharGlyphDictValue[c]);
                        }
                        else
                        {
                            s.WriteInt32(-1);
                        }
                        s.WriteInt32(c.Unicode);
                        s.WriteInt32(c.Code);
                    }

                    {
                        ref var withBlock = ref GlyphMapValue;
                        s.WriteInt32(withBlock.GlyphCount);
                        s.WriteInt32(withBlock.GlyphWidth);
                        s.WriteInt32(withBlock.GlyphHeight);
                        if (withBlock.WidthTable is null)
                        {
                            s.WriteInt32(0);
                        }
                        else
                        {
                            s.WriteInt32(withBlock.WidthTable.Length);
                            s.Write(withBlock.WidthTable);
                        }
                        s.Position = (s.Position + 15L) / 16L * 16L;
                        if (withBlock.GlyphCount == 0)
                        {
                            s.WriteInt32(0);
                        }
                        else
                        {
                            using (var ms = new StreamEx())
                            {
                                withBlock.Bmp.SaveTo(ms);
                                ms.Position = 0L;
                                s.WriteInt32((int)ms.Length);
                                s.WriteFromStream(ms, ms.Length);
                            }
                        }
                    }
                }
                FontLibSectionLength = (int)s.Position - FontLibSectionAddress;
                s.Position = (s.Position + 15L) / 16L * 16L;


                TextAddress = (int)s.Position;
                int TextCount = 0;
                if (TextValue is not null)
                    TextCount = TextValue.Length;
                s.WriteInt32(TextCount);
                s.WriteInt32(8);

                int[] TextInfoAddress = new int[TextCount];
                int[] TextInfoLength = new int[TextCount];
                int TextIndexPosition = (int)s.Position;
                s.Position += 8 * TextCount;
                for (int n = 0, loopTo2 = TextCount - 1; n <= loopTo2; n++)
                {
                    CharCode[] Original = TextValue[n];
                    int[] TextCharIndex = new int[Original.Length];
                    for (int k = 0, loopTo3 = Original.Length - 1; k <= loopTo3; k++)
                        TextCharIndex[k] = CodeIndexDict[Original[k]];
                    byte[] VLECode = VariableLengthEncode(TextCharIndex);
                    TextInfoAddress[n] = (int)s.Position - TextAddress;
                    TextInfoLength[n] = VLECode.Length;
                    s.Write(VLECode);
                }
                long TextEndPosition = s.Position;
                s.Position = TextIndexPosition;
                for (int n = 0, loopTo4 = TextCount - 1; n <= loopTo4; n++)
                {
                    s.WriteInt32(TextInfoAddress[n]);
                    s.WriteInt32(TextInfoLength[n]);
                }
                s.Position = TextEndPosition;
                TextLength = (int)s.Position - TextAddress;
                s.Position = (s.Position + 15L) / 16L * 16L;


                s.SetLength(s.Position);


                long Position = s.Position;
                s.Position = 8L;
                s.WriteInt32(FontLibSectionAddress);
                s.WriteInt32(FontLibSectionLength);
                s.WriteInt32(TextAddress);
                s.WriteInt32(TextLength);
                s.Position = Position;

                s.Position = 0L;
                if (!Compress)
                {
                    f.WriteFromStream(s, s.Length);
                }
                else
                {
                    f.WriteSimpleString(IdentifierCompression, 4);
                    using (var gz = new System.IO.Compression.GZipStream(new PartialStreamEx(f, 4L, long.MaxValue), System.IO.Compression.CompressionMode.Compress, true))
                    {
                        gz.Write(s.Read((int)s.Length), 0, (int)s.Length);
                    }
                }
            }
        }

        protected static byte[] VariableLengthEncode(int[] Original)
        {
            var l = new List<byte>();
            foreach (var i in Original)
            {
                int r = i;
                while ((r & ~0x7F) != 0)
                {
                    l.Add((byte)(r & 0x7F | 0x80));
                    r >>= 7;
                }
                l.Add((byte)r);
            }
            return l.ToArray();
        }
        protected static int[] VariableLengthDecode(byte[] Encoded)
        {
            var l = new List<int>();
            var i = default(int);
            var p = default(int);
            foreach (var b in Encoded)
            {
                i = i | (b & 0x7F) << p;
                if ((b & 0x80) != 0)
                {
                    p += 7;
                }
                else
                {
                    p = 0;
                    l.Add(i);
                    i = 0;
                }
            }
            return l.ToArray();
        }

        public GlyphText GetGlyphText()
        {
            return new GlyphText(this);
        }
    }

    /// <summary>字形图片。</summary>
    public class GlyphMap
    {
        protected int GlyphCountValue;

        /// <summary>字形数量。</summary>
        public int GlyphCount
        {
            get
            {
                return GlyphCountValue;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();
                GlyphCountValue = value;
            }
        }

        protected int GlyphWidthValue;
        /// <summary>字形的最大宽度。</summary>
        public int GlyphWidth
        {
            get
            {
                return GlyphWidthValue;
            }
        }

        protected int GlyphHeightValue;
        /// <summary>字形的高度。</summary>
        public int GlyphHeight
        {
            get
            {
                return GlyphHeightValue;
            }
        }

        protected byte[] WidthTableValue;
        /// <summary>字形宽度表。</summary>
        public byte[] WidthTable
        {
            get
            {
                return WidthTableValue;
            }
            set
            {
                if (value is null)
                    throw new ArgumentNullException();
                if (value.Length != GlyphCount)
                    throw new ArgumentException();
                WidthTableValue = value;
            }
        }

        protected Bmp BmpValue;
        /// <summary>图片。</summary>
        public Bmp Bmp
        {
            get
            {
                return BmpValue;
            }
        }

        protected int NumGlyphInLineValue;
        /// <summary>每行的字形数。</summary>
        public int NumGlyphInLine
        {
            get
            {
                return NumGlyphInLineValue;
            }
        }

        /// <summary>图片宽度。</summary>
        public int Width
        {
            get
            {
                return BmpValue.Width;
            }
        }
        /// <summary>图片高度。</summary>
        public int Height
        {
            get
            {
                return BmpValue.Height;
            }
        }

        /// <summary>已重载。构造字形图片。自动创建默认的宽度表，且所有值均初始化为GlyphWidth。如果传入空的Bmp，会自动按GlyphCount创建适当大小的32位Bmp。</summary>
        public GlyphMap(int GlyphCount, int GlyphWidth, int GlyphHeight, Bmp Bmp = null) : this(GlyphCount, GlyphWidth, GlyphHeight, new byte[GlyphCount], Bmp)
        {
            for (int n = 0, loopTo = WidthTable.Length - 1; n <= loopTo; n++)
                WidthTable[n] = (byte)GlyphWidth;
        }

        /// <summary>已重载。构造字形图片。如果传入空的Bmp，会自动按GlyphCount创建适当大小的32位Bmp。</summary>
        public GlyphMap(int GlyphCount, int GlyphWidth, int GlyphHeight, byte[] WidthTable, Bmp Bmp = null)
        {
            if (GlyphCount < 0)
                throw new ArgumentOutOfRangeException();
            if (GlyphWidth <= 0)
                throw new ArgumentOutOfRangeException();
            if (GlyphHeight <= 0)
                throw new ArgumentOutOfRangeException();
            if (WidthTable is null)
                throw new ArgumentNullException();
            if (WidthTable.Length != GlyphCount)
                throw new ArgumentException();
            GlyphCountValue = GlyphCount;
            GlyphWidthValue = GlyphWidth;
            GlyphHeightValue = GlyphHeight;
            WidthTableValue = WidthTable;
            if (Bmp is not null)
            {
                NumGlyphInLineValue = Bmp.Width / GlyphWidth;
                BmpValue = Bmp;
            }
            else
            {
                const int BitmapWidth = 512;
                NumGlyphInLineValue = BitmapWidth / GlyphWidth;
                int NumGlyphLine = (GlyphCount + NumGlyphInLineValue - 1) / NumGlyphInLineValue;
                if (NumGlyphLine <= 0)
                    NumGlyphInLineValue = 1;
                var Bitmap = new Bmp(BitmapWidth, GlyphHeight * NumGlyphLine, 32);
                BmpValue = Bitmap;
            }
        }

        /// <summary>获取字形的正方形位置。</summary>
        public Rectangle GetGlyphVirtualBox(int GlyphIndex)
        {
            if (GlyphIndex < 0 || GlyphIndex > GlyphCount)
                throw new InvalidDataException();
            return new Rectangle(0, 0, WidthTable[GlyphIndex], GlyphHeight);
        }

        /// <summary>获取字形的正方形位置。</summary>
        public Rectangle GetGlyphPhysicalBox(int GlyphIndex)
        {
            if (GlyphIndex < 0 || GlyphIndex > GlyphCount)
                throw new InvalidDataException();
            int x = GlyphIndex % NumGlyphInLine * GlyphWidth;
            int y = GlyphIndex / NumGlyphInLine * GlyphHeight;
            return new Rectangle(x, y, GlyphWidth, GlyphHeight);
        }
    }

    /// <summary>图形文本类</summary>
    public class GlyphText
    {

        protected FontLib FontLibValue;
        /// <summary>字库。</summary>
        public FontLib FontLib
        {
            get
            {
                return FontLibValue;
            }
        }

        protected CharCode[][] TextValue;
        /// <summary>文本。</summary>
        public CharCode[][] Text
        {
            get
            {
                return TextValue;
            }
            set
            {
                if (value is null)
                    throw new ArgumentNullException();
                TextValue = value;
            }
        }

        protected int GlyphWidthValue;
        /// <summary>字形的默认宽度(最大宽度)。</summary>
        public int GlyphWidth
        {
            get
            {
                return GlyphWidthValue;
            }
        }

        protected int GlyphHeightValue;
        /// <summary>字形的高度。</summary>
        public int GlyphHeight
        {
            get
            {
                return GlyphHeightValue;
            }
        }

        /// <summary>已重载。从字库、默认字形大小和文本码点创建实例。</summary>
        public GlyphText(FontLib FontLib, int GlyphWidth, int GlyphHeight, CharCode[][] Text)
        {
            if (FontLib is null)
                throw new ArgumentNullException();
            if (GlyphWidth <= 0)
                throw new ArgumentOutOfRangeException();
            if (GlyphHeight <= 0)
                throw new ArgumentOutOfRangeException();
            if (Text is null)
                throw new ArgumentNullException();
            FontLibValue = FontLib;
            GlyphWidthValue = GlyphWidth;
            GlyphHeightValue = GlyphHeight;
            TextValue = Text;
        }

        /// <summary>已重载。从默认字形大小和文本创建实例。</summary>
        public GlyphText(int GlyphWidth, int GlyphHeight, string[] Text)
        {
            CharCode[][] CharCodeText = new CharCode[Text.Length][];
            for (int n = 0, loopTo = Text.Length - 1; n <= loopTo; n++)
                CharCodeText[n] = CharCodeString.FromString16(Text[n]);
            FontLibValue = new FontLib(CharCodeString.FromString32(EncodingString.GetEncodingString32FromText(Text)));
            for (int n = 0, loopTo1 = Text.Length - 1; n <= loopTo1; n++)
            {
                CharCode[] t = CharCodeText[n];
                for (int k = 0, loopTo2 = t.Length - 1; k <= loopTo2; k++)
                    t[n] = FontLibValue.LookupWithUnicode(t[n].Unicode);
            }
            GlyphWidthValue = GlyphWidth;
            GlyphHeightValue = GlyphHeight;
            TextValue = CharCodeText;
        }

        /// <summary>已重载。从图形文本文件创建实例。若图形文本文件不包含FontLib或GlyphMap，则会抛出异常。</summary>
        public GlyphText(LOC LOC) : this(LOC.FontLib, LOC.GlyphMap.GlyphWidth, LOC.GlyphMap.GlyphHeight, LOC.Text)
        {
        }

        /// <summary>为字符添加字形。</summary>
        public delegate int[,] DrawGlyph(string c);

        /// <summary>已重载。使用指定字体为所有无字形非控制Unicode字符添加字形。</summary>
        public virtual void DrawGlyphForAllChar(DrawGlyph DrawGlyph)
        {
            foreach (var c in FontLibValue.CharCodes)
            {
                if (c.IsControlChar)
                    continue;
                if (FontLibValue.get_HasGlyph(c))
                    continue;
                if (!c.HasUnicode)
                    continue;
                int[,] Block = DrawGlyph(c.Character);
                FontLibValue[c] = new Glyph() { c = c, Block = Block, VirtualBox = new Rectangle(0, 0, Block.GetLength(0), Block.GetLength(1)) };
            }
        }
        /// <summary>已重载。使用指定字体为所有无字形非控制Unicode字符添加字形。</summary>
        public virtual void DrawGlyphForAllChar(Font Font)
        {
            foreach (var c in FontLibValue.CharCodes)
            {
                if (c.IsControlChar)
                    continue;
                if (FontLibValue.get_HasGlyph(c))
                    continue;
                if (!c.HasUnicode)
                    continue;
                using (var Bitmap = new Bitmap(GlyphWidthValue, GlyphHeightValue, PixelFormat.Format32bppArgb))
                {
                    using (var g = Graphics.FromImage(Bitmap))
                    {
                        int Width = (int)Round(Round((double)g.MeasureStringWidth(c.Character, Font)));
                        Width = NumericOperations.Min(Width, GlyphWidthValue);
                        g.DrawString(c.Character, Font, Brushes.Black, 0f, 0f, StringFormat.GenericTypographic);
                        int[,] Block = Bitmap.GetRectangle(0, 0, Width, GlyphHeight);
                        FontLibValue[c] = new Glyph() { c = c, Block = Block, VirtualBox = new Rectangle(0, 0, Block.GetLength(0), Block.GetLength(1)) };
                    }
                }
            }
        }

        /// <summary>生成图形文本文件。</summary>
        public virtual LOC GenerateLOC()
        {
            return LOC.GenerateLOC(FontLibValue, TextValue);
        }

        /// <summary>绘制CharInfo表示的文本。</summary>
        public virtual Bitmap GetBitmap(int TextIndex, int Space = 0, Dictionary<string, string> PhoneticDictionary = null)
        {
            bool EnablePhonetic = PhoneticDictionary is not null;

            CharCode[] GlyphText = TextValue[TextIndex];
            if (GlyphText is null || GlyphText.Length == 0)
                return null;

            int Size = GlyphHeight;
            Func<int, int> GetWidth = (Width) => Width + Space;
            if (PhoneticDictionary is not null)
            {
                GetWidth = new Func<int, int>((Width) => (int)Round(Round((Width + Space) * 1.4d)));
            }

            var Lines = new List<CharCode[]>();
            var Line = new List<CharCode>();
            var MaxLineWidth = default(int);
            var LineWidth = default(int);
            foreach (var c in GlyphText)
            {
                if (c.Unicode == 10)
                {
                    Lines.Add(Line.ToArray());
                    Line.Clear();
                    if (MaxLineWidth < LineWidth)
                        MaxLineWidth = LineWidth;
                    LineWidth = 0;
                }
                else if (FontLib.get_HasGlyph(c))
                {
                    Line.Add(c);
                    LineWidth += GetWidth(FontLib[c].VirtualBox.Width);
                }
                else
                {
                    Line.Add(c);
                    LineWidth += GetWidth(GlyphWidth);
                }
            }
            Lines.Add(Line.ToArray());
            Line.Clear();
            if (MaxLineWidth < LineWidth)
                MaxLineWidth = LineWidth;
            LineWidth = 0;


            var ZHFont = new Font("宋体", Size, FontStyle.Regular, GraphicsUnit.Pixel);
            var PFont = new Font("宋体", Size * 2 / 3, FontStyle.Regular, GraphicsUnit.Pixel);
            var JPFont = new Font("MingLiU", Size, FontStyle.Regular, GraphicsUnit.Pixel);

            int PadX = 5;
            int PadY = 5;
            Bitmap Bitmap;
            if (EnablePhonetic)
            {
                Bitmap = new Bitmap(MaxLineWidth + PadX * 2, GetWidth(Size) * 2 * Lines.Count + PadY * 2);
            }
            else
            {
                Bitmap = new Bitmap(MaxLineWidth + PadX * 2, GetWidth(Size) * Lines.Count + PadY * 2);
            }
            using (var g = Graphics.FromImage(Bitmap))
            {
                g.Clear(Color.White);

                int x = PadX;
                int y = PadY;

                foreach (var GlyphLine in Lines)
                {
                    if (EnablePhonetic)
                        y += GetWidth(Size);

                    var ControlCode = new List<Char32>();
                    bool ControlCodeMode = false;

                    foreach (var c in GlyphLine)
                    {
                        if (c.HasUnicode)
                        {
                            var ch = c.Unicode;
                            switch ((string)ch)
                            {
                                case "<":
                                case "{":
                                    {
                                        if (!ControlCodeMode)
                                        {
                                            ControlCodeMode = true;
                                            ControlCode.Add(ch);
                                        }
                                        continue;
                                    }
                                case ">":
                                case "}":
                                    {
                                        if (ControlCodeMode)
                                        {
                                            ControlCodeMode = false;
                                            ControlCode.Add(ch);

                                            string s = ControlCode.ToArray().ToUTF16B();
                                            g.DrawString(s, ZHFont, Brushes.Black, x, y, StringFormat.GenericTypographic);
                                            x = (int)Round(x + (g.MeasureStringWidth(s, ZHFont) + 1f));

                                            ControlCode.Clear();
                                        }
                                        continue;
                                    }

                                default:
                                    {
                                        if (ControlCodeMode)
                                        {
                                            ControlCode.Add(ch);
                                            continue;
                                        }
                                        else
                                        {
                                        }

                                        break;
                                    }
                            }
                        }

                        if (c.IsControlChar || !(FontLibValue.get_HasGlyph(c) || c.HasUnicode || c.HasCode))
                        {
                            g.FillRectangle(Brushes.Gray, new Rectangle(x, y, GlyphWidth, GlyphHeight));
                            x += GetWidth(GlyphWidth);
                        }
                        else if (FontLib.get_HasGlyph(c))
                        {
                            int Width = FontLib[c].VirtualBox.Width;
                            var Glyph = FontLibValue[c];
                            if (Glyph.VirtualBox.Width <= 0 || Glyph.VirtualBox.Height <= 0)
                            {

                            }
                            var SrcImage = new Bitmap(Glyph.VirtualBox.Width, Glyph.VirtualBox.Height);
                            SrcImage.SetRectangle(0, 0, Glyph.Block);
                            var SrcRect = new Rectangle(0, 0, Glyph.VirtualBox.Width, Glyph.VirtualBox.Height);
                            var DestRect = new Rectangle(x, y, Glyph.VirtualBox.Width, Glyph.VirtualBox.Height);
                            g.DrawImage(SrcImage, DestRect, SrcRect, GraphicsUnit.Pixel);
                            // 下面这句因为.Net Framework 2.0内部错误源矩形会向右偏移1像素
                            // g.DrawImage(SrcImage, x, y, SrcRect, GraphicsUnit.Pixel)

                            if (c.HasUnicode)
                            {
                                string ch = c.Character;
                                if (EnablePhonetic && PhoneticDictionary.ContainsKey(ch))
                                {
                                    string s = PhoneticDictionary[ch];
                                    int OffsetX = (int)((long)Round(Width - g.MeasureStringWidth(s, PFont)) / 2L);
                                    if (g is not null)
                                        g.DrawString(s, PFont, Brushes.DimGray, x + OffsetX, y - Size, StringFormat.GenericTypographic);
                                }
                            }
                            x += GetWidth(Width);
                        }
                        else
                        {
                            string ch = c.Character;
                            if (c.Unicode >= 0x3040 && c.Unicode < 0x3100)
                            {
                                float Width = g.MeasureStringWidth(ch, JPFont);
                                g.DrawString(ch, JPFont, Brushes.Black, x, y, StringFormat.GenericTypographic);
                                if (EnablePhonetic && PhoneticDictionary.ContainsKey(ch))
                                {
                                    string s = PhoneticDictionary[ch];
                                    int OffsetX = (int)((long)Round(Width - g.MeasureStringWidth(s, PFont)) / 2L);
                                    if (g is not null)
                                        g.DrawString(s, PFont, Brushes.DimGray, x + OffsetX, y - Size, StringFormat.GenericTypographic);
                                }
                                x += GetWidth((int)Round(Width));
                            }
                            else
                            {
                                float Width = g.MeasureStringWidth(ch, ZHFont);
                                g.DrawString(ch, ZHFont, Brushes.Black, x, y, StringFormat.GenericTypographic);
                                if (EnablePhonetic && PhoneticDictionary.ContainsKey(ch))
                                {
                                    string s = PhoneticDictionary[ch];
                                    int OffsetX = (int)((long)Round(Width - g.MeasureStringWidth(s, PFont)) / 2L);
                                    if (g is not null)
                                        g.DrawString(s, PFont, Brushes.DimGray, x + OffsetX, y - Size, StringFormat.GenericTypographic);
                                }
                                x += GetWidth((int)Round(Width));
                            }
                        }
                    }

                    y += GetWidth(Size);
                    x = PadX;
                }
            }

            ZHFont.Dispose();
            JPFont.Dispose();
            return Bitmap;
        }
        /// <summary>得到字符文字的普通字符串，只有已映射有Unicode的字符才会转换。</summary>
        public virtual string Parse(int TextIndex)
        {
            CharCode[] GlyphText = TextValue[TextIndex];
            var sb = new StringBuilder();
            foreach (var ci in GlyphText)
            {
                if (ci.HasUnicode)
                {
                    sb.Append(ci.Character);
                }
            }
            return sb.ToString().UnifyNewLineToCrLf();
        }
    }

    /// <summary>字库，字符码点到字形的映射。</summary>
    public class FontLib
    {
        protected List<CharCode> Code = new List<CharCode>();
        protected Dictionary<CharCode, IGlyph> Dict = new Dictionary<CharCode, IGlyph>();
        protected Dictionary<int, CharCode> UnicodeDict = new Dictionary<int, CharCode>();
        protected Dictionary<int, CharCode> CodeDict = new Dictionary<int, CharCode>();

        /// <summary>已重载。构造空字库。</summary>
        public FontLib()
        {
        }

        /// <summary>已重载。从字符码点构造字库。后面的字符若与前面重复，不会覆盖。</summary>
        public FontLib(ICollection<CharCode> CharCodes)
        {
            if (CharCodes is null)
                throw new ArgumentNullException();

            foreach (var c in CharCodes)
            {
                if (Dict.ContainsKey(c))
                    continue;
                Code.Add(c);

                if (c.HasUnicode && !UnicodeDict.ContainsKey(c.Unicode))
                    UnicodeDict.Add(c.Unicode, c);
                if (c.HasCode && !CodeDict.ContainsKey(c.Code))
                    CodeDict.Add(c.Code, c);
            }
        }

        /// <summary>已重载。从字符码点和字形构造字库。后面的字符若与前面重复，不会覆盖。</summary>
        /// <remarks>字符码点和字形的数量要一致。</remarks>
        public FontLib(IList<CharCode> CharCodes, IList<IGlyph> CharGlyph)
        {
            if (CharCodes is null)
                throw new ArgumentNullException();
            if (CharGlyph is null)
                throw new ArgumentNullException();
            if (CharCodes.Count != CharGlyph.Count)
                throw new ArgumentException();

            for (int n = 0, loopTo = CharCodes.Count - 1; n <= loopTo; n++)
            {
                var c = CharCodes[n];
                var g = CharGlyph[n];
                if (Dict.ContainsKey(c))
                    continue;
                Code.Add(c);
                Dict.Add(c, g);

                if (c.HasUnicode && !UnicodeDict.ContainsKey(c.Unicode))
                    UnicodeDict.Add(c.Unicode, c);
                if (c.HasCode && !CodeDict.ContainsKey(c.Code))
                    CodeDict.Add(c.Code, c);
            }
        }

        /// <summary>字符数量。</summary>
        public int CharCount
        {
            get
            {
                if (Code is null)
                    return 0;
                return Code.Count;
            }
        }

        /// <summary>字符码点。</summary>
        public List<CharCode> CharCodes
        {
            get
            {
                return new List<CharCode>(Code);
            }
        }

        /// <summary>字符码点-字形映射。</summary>
        public Dictionary<CharCode, IGlyph> GetDict()
        {
            return new Dictionary<CharCode, IGlyph>(Dict);
        }

        /// <summary>获得Unicode-字符码点映射。</summary>
        public Dictionary<int, CharCode> GetUnicodeDict()
        {
            return new Dictionary<int, CharCode>(UnicodeDict);
        }

        /// <summary>获得自定义码点-字符码点映射。</summary>
        public Dictionary<int, CharCode> GetCodeDict()
        {
            return new Dictionary<int, CharCode>(CodeDict);
        }

        /// <summary>从Unicode查找字符码点。</summary>
        public CharCode LookupWithUnicode(int Unicode)
        {
            if (Unicode == -1)
                return null;
            if (UnicodeDict.ContainsKey(Unicode))
                return UnicodeDict[Unicode];
            return null;
        }

        /// <summary>从自定义码点查找字符码点。</summary>
        public CharCode LookupWithCode(int Code)
        {
            if (Code == -1)
                return null;
            if (CodeDict.ContainsKey(Code))
                return CodeDict[Code];
            return null;
        }

        /// <summary>从字符(UTF-16B)查找字符码点。</summary>
        public CharCode LookupWithChar(string c)
        {
            return LookupWithUnicode(Char32.FromString(c));
        }

        /// <summary>添加字符，仅码点。后面的字符若与前面重复，不会覆盖。</summary>
        public void Add(CharCode CharCode)
        {
            if (Dict.ContainsKey(CharCode))
                throw new InvalidOperationException();

            Code.Add(CharCode);
            Dict.Add(CharCode, null);

            if (CharCode.HasUnicode && !UnicodeDict.ContainsKey(CharCode.Unicode))
                UnicodeDict.Add(CharCode.Unicode, CharCode);
            if (CharCode.HasCode && !CodeDict.ContainsKey(CharCode.Code))
                CodeDict.Add(CharCode.Code, CharCode);
        }

        /// <summary>添加字符，从码点和字形。后面的字符若与前面重复，不会覆盖。</summary>
        public void Add(CharCode CharCode, IGlyph CharGlyph)
        {
            if (Dict.ContainsKey(CharCode))
                throw new InvalidOperationException();

            Code.Add(CharCode);
            Dict.Add(CharCode, CharGlyph);

            if (CharCode.HasUnicode && !UnicodeDict.ContainsKey(CharCode.Unicode))
                UnicodeDict.Add(CharCode.Unicode, CharCode);
            if (CharCode.HasCode && !CodeDict.ContainsKey(CharCode.Code))
                CodeDict.Add(CharCode.Code, CharCode);
        }

        /// <summary>移除字符。</summary>
        public void Remove(CharCode CharCode)
        {
            Code.Remove(CharCode);
            if (Dict.ContainsKey(CharCode))
                Dict.Remove(CharCode);

            if (CharCode.HasUnicode && UnicodeDict.ContainsKey(CharCode.Unicode) && ReferenceEquals(UnicodeDict[CharCode.Unicode], CharCode))
                UnicodeDict.Remove(CharCode.Unicode);
            if (CharCode.HasCode && CodeDict.ContainsKey(CharCode.Code) && ReferenceEquals(CodeDict[CharCode.Code], CharCode))
                CodeDict.Remove(CharCode.Code);
        }

        /// <summary>移除字符。</summary>
        public void RemoveMany(IEnumerable<CharCode> CharCodes)
        {
            var NewCode = new List<CharCode>();
            var CharCodeDict = new HashSet<CharCode>(CharCodes);
            foreach (var c in Code)
            {
                if (CharCodeDict.Contains(c))
                    continue;
                NewCode.Add(c);
            }
            Code = NewCode;
            foreach (var CharCode in CharCodes)
            {
                if (Dict.ContainsKey(CharCode))
                    Dict.Remove(CharCode);

                if (CharCode.HasUnicode && UnicodeDict.ContainsKey(CharCode.Unicode) && ReferenceEquals(UnicodeDict[CharCode.Unicode], CharCode))
                    UnicodeDict.Remove(CharCode.Unicode);
                if (CharCode.HasCode && CodeDict.ContainsKey(CharCode.Code) && ReferenceEquals(CodeDict[CharCode.Code], CharCode))
                    CodeDict.Remove(CharCode.Code);
            }
        }

        /// <summary>获取或设置指定字形。</summary>
        public IGlyph this[CharCode CharCode]
        {
            get
            {
                return Dict[CharCode];
            }
            set
            {
                Dict[CharCode] = value;
            }
        }

        /// <summary>指示指定字符码点是否存在字形。</summary>
        public bool get_HasGlyph(CharCode CharCode)
        {
            return Dict.ContainsKey(CharCode) && Dict[CharCode] is not null;
        }

        /// <summary>获取字库中的所有Unicode字符。</summary>
        public string GetEncodingString()
        {
            var sb = new StringBuilder();
            foreach (var c in CharCodes)
            {
                if (c.HasUnicode)
                    sb.Append(Char32.ToString(c.Unicode));
            }
            return sb.ToString();
        }
    }
}