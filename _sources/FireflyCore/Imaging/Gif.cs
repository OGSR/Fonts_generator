// ==========================================================================
// 
// File:        Gif.vb
// Location:    Firefly.Imaging <Visual Basic .Net>
// Description: 基本Gif文件类
// Version:     2009.03.29.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System.Collections.Generic;
using System.IO;
using static System.Math;
using Firefly.TextEncoding;
using Microsoft.VisualBasic.CompilerServices;

namespace Firefly.Imaging
{
    /// <summary>基本Gif文件类</summary>
    /// <remarks>用于GIF89a，但忽略无用功能</remarks>
    public class Gif
    {
        public const string Identifier = "GIF89a";
        protected short PicWidth;
        public short Width
        {
            get
            {
                return PicWidth;
            }
        }
        protected short PicHeight;
        public short Height
        {
            get
            {
                return PicHeight;
            }
        }
        public bool GlobalColorTableFlag
        {
            get
            {
                return PicPalette is not null;
            }
        }
        protected int PicBitsPerPixel;
        public int BitsPerPixel
        {
            get
            {
                return PicBitsPerPixel;
            }
        }
        public int GlobalColorTableSize
        {
            get
            {
                if (PicPalette is null)
                    return 0;
                return PicPalette.GetLength(0);
            }
        }
        public byte GlobalBackgroundColor;
        protected const byte PixelAspectRadio = 0; // 不用
        protected int[] PicPalette;
        public int[] Palette
        {
            get
            {
                return PicPalette;
            }
        }
        public GifImageBlock[] Flame;
        protected const byte Trailer = 0x3B;

        private Gif()
        {
        }

        public Gif(GifImageBlock SingleFlame, int[] Palette = null)
        {
            if (SingleFlame is null)
                throw new InvalidDataException();
            PicWidth = SingleFlame.Width;
            PicHeight = SingleFlame.Height;
            PicBitsPerPixel = (int)Round(Ceiling(Log(SingleFlame.Palette.GetLength(0)) / Log(2d))); // 色深
            if (Palette is not null)
                PicPalette = (int[])Palette.Clone();
            Flame = new GifImageBlock[] { SingleFlame };
        }
        public Gif(short Width, short Height, byte BitsPerPixel, GifImageBlock[] Flames, int[] Palette = null)
        {
            if (Width < 0 || PicHeight < 0 || BitsPerPixel <= 0)
                throw new InvalidDataException();
            PicWidth = Width;
            PicHeight = Height;
            PicBitsPerPixel = BitsPerPixel; // 色深

            if (Palette is not null)
                PicPalette = (int[])Palette.Clone();
            if (Flames is not null)
                Flame = (GifImageBlock[])Flames.Clone();
        }

        public Gif(string Path)
        {
            using (var gf = new StreamEx(Path, FileMode.Open))
            {
                {
                    var withBlock = this;
                    for (int n = 0; n <= 5; n++)
                    {
                        if (gf.ReadByte() != String16.AscW(Identifier[n]))
                        {
                            gf.Close();
                            throw new InvalidDataException();
                        }
                    }
                    withBlock.PicWidth = gf.ReadInt16();
                    withBlock.PicHeight = gf.ReadInt16();
                    byte b = gf.ReadByte();
                    withBlock.PicBitsPerPixel = ((b & 0x70) >> 4) + 1;
                    int PicGlobalColorTableSize = (int)Round(Pow(2d, (b & 7) + 1));
                    withBlock.GlobalBackgroundColor = gf.ReadByte();
                    gf.ReadByte();
                    if (Conversions.ToBoolean(b & 128))
                    {
                        int c;
                        withBlock.PicPalette = new int[PicGlobalColorTableSize];
                        for (int n = 0, loopTo = PicGlobalColorTableSize - 1; n <= loopTo; n++)
                        {
                            c = gf.ReadByte();
                            c = c << 8;
                            c = c | gf.ReadByte();
                            c = c << 8;
                            c = c | gf.ReadByte();
                            withBlock.PicPalette[n] = c;
                        }
                    }
                    var Flame = new List<GifImageBlock>();
                    var cur = GetNextImageBlock(gf);
                    while (cur is not null)
                    {
                        Flame.Add(cur);
                        cur = GetNextImageBlock(gf);
                    }
                    withBlock.Flame = Flame.ToArray();
                }
            }
        }
        protected static GifImageBlock GetNextImageBlock(PositionedStreamPasser sp)
        {
            var s = sp.GetStream();
            GifImageBlock ret = null;
            switch (s.ReadByte())
            {
                case GifImageBlock.ExtensionIntroducer:
                    {
                        if (s.ReadByte() != GifImageBlock.ExtGraphicControlLabel)
                        {
                            int Len = s.ReadByte();
                            while (Len != 0)
                            {
                                s.Position += Len;
                                Len = s.ReadByte();
                            }
                            return GetNextImageBlock(s);
                        }
                        ret = new GifImageBlock();
                        ReadExtendedImageBlock(s, ref ret);
                        break;
                    }
                case GifImageBlock.ImageDescriptorIntroducer:
                    {
                        ret = new GifImageBlock();
                        ReadNotExtendedImageBlock(s, ref ret);
                        break;
                    }
                case Trailer:
                    {
                        return null;
                    }

                default:
                    {
                        throw new InvalidDataException();
                    }
            }
            return ret;
        }
        protected static void ReadNotExtendedImageBlock(PositionedStreamPasser sp, ref GifImageBlock i)
        {
            var s = sp.GetStream();
            s.Position += 4L;
            i.Width = s.ReadInt16();
            i.Height = s.ReadInt16();
            byte b = s.ReadByte();
            i.InterlaceFlag = Conversions.ToBoolean(b & 64);
            int LocalColorTableSize = 1 << (b & 7) + 1;
            if (Conversions.ToBoolean(b & 128))
            {
                int c;
                i.Palette = new int[LocalColorTableSize];
                for (int n = 0, loopTo = LocalColorTableSize - 1; n <= loopTo; n++)
                {
                    c = s.ReadByte();
                    c = c << 8;
                    c = c | s.ReadByte();
                    c = c << 8;
                    c = c | s.ReadByte();
                    i.Palette[n] = c;
                }
            }

            int CodeSize = s.ReadByte();
            var TarBytes = new Queue<byte>();

            int Len = s.ReadByte();
            while (Len != 0)
            {
                for (int n = 0, loopTo1 = Len - 1; n <= loopTo1; n++)
                    TarBytes.Enqueue(s.ReadByte());
                Len = s.ReadByte();
            }

            var LZW = new LZWCodec(CodeSize);
            byte[] SrcBytes = LZW.UnLZW(TarBytes.ToArray());

            i.Rectangle = new byte[i.Width, i.Height];
            if (i.InterlaceFlag)
            {
                var YInBytes = default(int);
                for (int y = 0, loopTo2 = i.Height - 1; y <= loopTo2; y += 8)
                {
                    for (int x = 0, loopTo3 = i.Width - 1; x <= loopTo3; x++)
                        i.Rectangle[x, y] = SrcBytes[x + YInBytes * i.Width];
                    YInBytes += 1;
                }
                for (int y = 4, loopTo4 = i.Height - 1; y <= loopTo4; y += 8)
                {
                    for (int x = 0, loopTo5 = i.Width - 1; x <= loopTo5; x++)
                        i.Rectangle[x, y] = SrcBytes[x + YInBytes * i.Width];
                    YInBytes += 1;
                }
                for (int y = 2, loopTo6 = i.Height - 1; y <= loopTo6; y += 4)
                {
                    for (int x = 0, loopTo7 = i.Width - 1; x <= loopTo7; x++)
                        i.Rectangle[x, y] = SrcBytes[x + YInBytes * i.Width];
                    YInBytes += 1;
                }
                for (int y = 1, loopTo8 = i.Height - 1; y <= loopTo8; y += 2)
                {
                    for (int x = 0, loopTo9 = i.Width - 1; x <= loopTo9; x++)
                        i.Rectangle[x, y] = SrcBytes[x + YInBytes * i.Width];
                    YInBytes += 1;
                }
            }
            else
            {
                for (int y = 0, loopTo10 = i.Height - 1; y <= loopTo10; y++)
                {
                    for (int x = 0, loopTo11 = i.Width - 1; x <= loopTo11; x++)
                        i.Rectangle[x, y] = SrcBytes[x + y * i.Width];
                }
            }
        }
        protected static void ReadExtendedImageBlock(PositionedStreamPasser sp, ref GifImageBlock i)
        {
            var s = sp.GetStream();
            {
                ref var withBlock = ref i;
                withBlock.EnableControlExtension = true;
                s.ReadByte();
                withBlock.TransparentColorFlag = Conversions.ToBoolean(s.ReadByte() & 1);
                withBlock.DelayTime = s.ReadInt16();
                withBlock.TransparentColorIndex = s.ReadByte();
                s.ReadByte();
                if (s.ReadByte() != GifImageBlock.ImageDescriptorIntroducer)
                    throw new InvalidDataException();
                ReadNotExtendedImageBlock(s, ref i);
            }
        }

        public void WriteToFile(string Path)
        {
            var gf = new StreamEx(Path, FileMode.Create);
            for (int n = 0, loopTo = Identifier.Length - 1; n <= loopTo; n++)
                gf.WriteByte((byte)String16.AscW(Identifier[n]));
            gf.WriteInt16(PicWidth);
            gf.WriteInt16(PicHeight);
            var b = default(byte);
            byte cr = (byte)(PicBitsPerPixel - 1);
            if (cr >= 8)
                cr = 7;
            b = (byte)(b | cr << 4);
            if (GlobalColorTableFlag)
            {
                b = (byte)(b | 128);
                byte pixel = (byte)Round(Ceiling(Log(GlobalColorTableSize) / Log(2d)) - 1d);
                if (pixel >= 8)
                    pixel = 7;
                b = (byte)(b | pixel);
            }
            gf.WriteByte(b);
            gf.WriteByte(GlobalBackgroundColor);
            gf.WriteByte(PixelAspectRadio);
            if (GlobalColorTableFlag)
            {
                int c;
                for (int n = 0, loopTo1 = GlobalColorTableSize - 1; n <= loopTo1; n++)
                {
                    c = PicPalette[n];
                    gf.WriteByte((byte)(c >> 16 & 255));
                    gf.WriteByte((byte)(c >> 8 & 255));
                    gf.WriteByte((byte)(c & 255));
                }
            }
            if (Flame is not null)
            {
                foreach (GifImageBlock i in Flame)
                    WriteImageBlock(gf, i);
            }
            gf.WriteByte(Trailer);
            gf.Close();
        }
        protected void WriteImageBlock(PositionedStreamPasser sp, GifImageBlock i)
        {
            var s = sp.GetStream();
            if (i.EnableControlExtension)
            {
                s.WriteByte(GifImageBlock.ExtensionIntroducer);
                s.WriteByte(GifImageBlock.ExtGraphicControlLabel);
                s.WriteByte(4);
                if (i.TransparentColorFlag)
                {
                    s.WriteByte(1 | GifImageBlock.DisposalMethod << 2);
                }
                else
                {
                    s.WriteByte(0 | GifImageBlock.DisposalMethod << 2);
                }
                s.WriteInt16(i.DelayTime);
                s.WriteByte(i.TransparentColorIndex);
                s.WriteByte(GifImageBlock.BlockTerminator);
            }
            s.WriteByte(GifImageBlock.ImageDescriptorIntroducer);
            s.WriteInt16(0);
            s.WriteInt16(0);
            s.WriteInt16(i.Width);
            s.WriteInt16(i.Height);
            var b = default(byte);
            if (i.InterlaceFlag)
                b = (byte)(b | 64);
            if (i.LocalColorTableFlag)
            {
                b = (byte)(b | 128);
                byte pixel = (byte)Round(Ceiling(Log(i.LocalColorTableSize) / Log(2d)) - 1d);
                if (pixel >= 8)
                    pixel = 7;
                b = (byte)(b | pixel);
            }
            s.WriteByte(b);
            if (i.LocalColorTableFlag)
            {
                int c;
                for (int n = 0, loopTo = i.LocalColorTableSize - 1; n <= loopTo; n++)
                {
                    c = i.Palette[n];
                    s.WriteByte((byte)(c >> 16 & 255));
                    s.WriteByte((byte)(c >> 8 & 255));
                    s.WriteByte((byte)(c & 255));
                }
            }

            int CodeSize;
            if (PicBitsPerPixel != 1)
            {
                CodeSize = PicBitsPerPixel;
            }
            else
            {
                CodeSize = 2;
            }
            s.WriteByte((byte)CodeSize);

            byte[] SrcBytes = new byte[(i.Width * i.Height)];

            if (i.InterlaceFlag)
            {
                var YInBytes = default(int);
                for (int y = 0, loopTo1 = i.Height - 1; y <= loopTo1; y += 8)
                {
                    for (int x = 0, loopTo2 = i.Width - 1; x <= loopTo2; x++)
                        SrcBytes[x + YInBytes * i.Width] = i.Rectangle[x, y];
                    YInBytes += 1;
                }
                for (int y = 4, loopTo3 = i.Height - 1; y <= loopTo3; y += 8)
                {
                    for (int x = 0, loopTo4 = i.Width - 1; x <= loopTo4; x++)
                        SrcBytes[x + YInBytes * i.Width] = i.Rectangle[x, y];
                    YInBytes += 1;
                }
                for (int y = 2, loopTo5 = i.Height - 1; y <= loopTo5; y += 4)
                {
                    for (int x = 0, loopTo6 = i.Width - 1; x <= loopTo6; x++)
                        SrcBytes[x + YInBytes * i.Width] = i.Rectangle[x, y];
                    YInBytes += 1;
                }
                for (int y = 1, loopTo7 = i.Height - 1; y <= loopTo7; y += 2)
                {
                    for (int x = 0, loopTo8 = i.Width - 1; x <= loopTo8; x++)
                        SrcBytes[x + YInBytes * i.Width] = i.Rectangle[x, y];
                    YInBytes += 1;
                }
            }
            else
            {
                for (int y = 0, loopTo9 = i.Height - 1; y <= loopTo9; y++)
                {
                    for (int x = 0, loopTo10 = i.Width - 1; x <= loopTo10; x++)
                        SrcBytes[x + y * i.Width] = i.Rectangle[x, y];
                }
            }

            var LZW = new LZWCodec(CodeSize);
            var TarBytes = new Queue<byte>(LZW.LZW(SrcBytes));
            while (TarBytes.Count > 254)
            {
                s.WriteByte(254);
                for (int n = 0; n <= 253; n++)
                    s.WriteByte(TarBytes.Dequeue());
            }
            if (TarBytes.Count > 0)
            {
                s.WriteByte((byte)TarBytes.Count);
                for (int n = 0, loopTo11 = TarBytes.Count - 1; n <= loopTo11; n++)
                    s.WriteByte(TarBytes.Dequeue());
            }
            s.WriteByte(0);
        }

        // End Of Class
        // Start Of SubClasses

        public class LZWCodec
        {
            private int StartCodeSize;
            private int CodeSize;
            private List<byte> SrcBytes;
            private int SrcPos;
            private bool SrcReadEnd;
            private List<byte> TarBytes;
            private int TarPos;
            private bool TarReadEnd;
            public LZWCodec(int StartCodeSize)
            {
                if (StartCodeSize <= 0)
                    throw new InvalidDataException();
                this.StartCodeSize = StartCodeSize;
            }
            private byte ReadSrc()
            {
                // 读取SrcBytes直到一字节也读不出来
                if (SrcPos <= SrcBytes.Count - 1)
                {
                    byte ret = SrcBytes[SrcPos];
                    SrcPos += 1;
                    return ret;
                }
                else
                {
                    SrcPos += 1;
                    SrcReadEnd = true;
                    return 0;
                }
            }
            private short ReadTar()
            {
                // 读取TarBytes直到一位也读不出来
                int r;
                int d = DivRem(TarPos, 8, out r);
                TarPos += CodeSize + 1;
                if (d > TarBytes.Count - 1)
                {
                    TarReadEnd = true;
                    return 0;
                }
                int ret = TarBytes[d];
                ret = ret >> r;
                if (r + CodeSize + 1 <= 8)
                {
                    return (short)(ret & (long)Round(Pow(2d, CodeSize + 1) - 1d));
                }
                else if (d + 1 > TarBytes.Count - 1)
                {
                    return (short)ret;
                }
                ret = ret | TarBytes[d + 1] << 8 - r;
                if (r + CodeSize + 1 <= 16)
                {
                    return (short)(ret & (long)Round(Pow(2d, CodeSize + 1) - 1d));
                }
                else if (d + 2 > TarBytes.Count - 1)
                {
                    return (short)ret;
                }
                ret = ret | TarBytes[d + 2] << 16 - r;
                return (short)(ret & (long)Round(Pow(2d, CodeSize + 1) - 1d));
            }
            private void WriteSrc(string b)
            {
                for (int n = 0, loopTo = b.Length - 1; n <= loopTo; n++)
                    SrcBytes.Add((byte)String16.AscW(b[n]));
                SrcPos += b.Length;
            }
            private void WriteTar(short i)
            {
                int r;
                int d = DivRem(TarPos, 8, out r);
                TarPos += CodeSize + 1;
                if (d > TarBytes.Count - 1)
                    TarBytes.Add(0);
                TarBytes[d] = (byte)(TarBytes[d] | i << r & 255);
                if (r + CodeSize + 1 <= 8)
                    return;
                if (d + 1 > TarBytes.Count - 1)
                    TarBytes.Add(0);
                i = (short)(i >> 8 - r);
                TarBytes[d + 1] = (byte)(i & 255);
                if (r + CodeSize + 1 <= 16)
                    return;
                if (d + 2 > TarBytes.Count - 1)
                    TarBytes.Add(0);
                i = (short)(i >> 8);
                TarBytes[d + 2] = (byte)(i & 255);
            }
            public byte[] LZW(byte[] SrcBytes)
            {
                this.SrcBytes = new List<byte>(SrcBytes);
                TarBytes = new List<byte>();
                CodeSize = StartCodeSize;
                SrcPos = 0;
                TarPos = 0;
                TarReadEnd = false;
                var Table = new List<string>();
                var RTable = new Dictionary<string, int>();
                for (int n = 0, loopTo = (1 << StartCodeSize) - 1; n <= loopTo; n++)
                {
                    Table.Add(Conversions.ToString(String16.ChrW(n)));
                    RTable.Add(Conversions.ToString(String16.ChrW(n)), n);
                }
                Table.Add("CC");
                Table.Add("EC");
                short ClearCode = (short)(1 << StartCodeSize);
                short OverflowCode = (short)(1 << StartCodeSize + 1);

                string Prefix = "";
                var IndexOfPrefix = default(short);
                char Root;
                WriteTar((short)(1 << StartCodeSize));

                string CurStr;

                while (true)
                {
                    Root = String16.ChrW(ReadSrc());
                    if (SrcReadEnd)
                        break;
                    CurStr = Prefix + Root;

                    if (RTable.ContainsKey(CurStr))
                    {
                        IndexOfPrefix = (short)RTable[CurStr];
                        Prefix = CurStr;
                    }
                    else
                    {
                        WriteTar(IndexOfPrefix);
                        if (Table.Count == OverflowCode)
                        {
                            if (OverflowCode == 4096)
                            {
                                WriteTar((short)String16.AscW(Root));
                                WriteTar((short)(1 << StartCodeSize));
                                Table.RemoveRange((1 << StartCodeSize) + 2, Table.Count - (1 << StartCodeSize) - 2);
                                RTable.Clear();
                                for (int n = 0, loopTo1 = (1 << StartCodeSize) - 1; n <= loopTo1; n++)
                                    RTable.Add(Conversions.ToString(String16.ChrW(n)), n);
                                CodeSize = StartCodeSize;
                                OverflowCode = (short)(1 << StartCodeSize + 1);
                                Prefix = "";
                                continue;
                            }
                            else
                            {
                                CodeSize += 1;
                                OverflowCode = (short)(OverflowCode << 1);
                            }
                        }
                        Table.Add(CurStr);
                        RTable.Add(CurStr, Table.Count - 1);
                        Prefix = Conversions.ToString(Root);
                        IndexOfPrefix = (short)String16.AscW(Root);
                    }
                }
                WriteTar(IndexOfPrefix);
                WriteTar((short)(ClearCode + 1));

                return TarBytes.ToArray();
            }
            public byte[] UnLZW(byte[] TarBytes)
            {
                SrcBytes = new List<byte>();
                this.TarBytes = new List<byte>(TarBytes);
                CodeSize = StartCodeSize;
                SrcPos = 0;
                TarPos = 0;
                TarReadEnd = false;
                var Table = new List<string>();
                for (int n = 0, loopTo = (1 << StartCodeSize) - 1; n <= loopTo; n++)
                    Table.Add(Conversions.ToString(String16.ChrW(n)));
                Table.Add("CC");
                Table.Add("EC");
                short ClearCode = (short)(1 << StartCodeSize);
                short OverflowCode = (short)(1 << StartCodeSize + 1);
                string CurStr;
                short cur = ReadTar();
                cur = ReadTar();
                WriteSrc(Table[cur]);
                short old = cur;
                cur = ReadTar();
                while (true)
                {
                    bool exitWhile = false;
                    switch (cur)
                    {
                        case var @case when @case == ClearCode:
                            {
                                Table.RemoveRange((1 << StartCodeSize) + 2, Table.Count - (1 << StartCodeSize) - 2);
                                CodeSize = StartCodeSize;
                                OverflowCode = (short)(1 << StartCodeSize + 1);
                                cur = ReadTar();
                                WriteSrc(Table[cur]);
                                break;
                            }
                        case var case1 when case1 == ClearCode + 1:
                            {
                                exitWhile = true;
                                break;
                            }

                        default:
                            {
                                if (cur <= Table.Count - 1)
                                {
                                    WriteSrc(Table[cur]);
                                    Table.Add(Table[old] + Table[cur][0]);
                                }
                                else
                                {
                                    CurStr = Table[old] + Table[old][0];
                                    WriteSrc(CurStr);
                                    Table.Add(CurStr);
                                }
                                if (Table.Count == OverflowCode && OverflowCode != 4096)
                                {
                                    CodeSize += 1;
                                    OverflowCode = (short)(OverflowCode << 1);
                                }

                                break;
                            }
                    }

                    if (exitWhile)
                    {
                        break;
                    }
                    if (TarReadEnd)
                        break;
                    old = cur;
                    cur = ReadTar();
                }
                byte[] ret = SrcBytes.ToArray();
                SrcBytes = null;
                this.TarBytes = null;
                return ret;
            }
        }

        public class GifImageBlock
        {
            public bool EnableControlExtension;
            public const byte ExtensionIntroducer = 0x21;
            public const byte ExtGraphicControlLabel = 0xF9;
            public const byte ExtBlockSize = 4;
            public const byte DisposalMethod = 2;
            public bool TransparentColorFlag;
            public short DelayTime; // /0.01s
            public byte TransparentColorIndex;
            public const byte BlockTerminator = 0;

            public const byte ImageDescriptorIntroducer = 0x2C;
            public short Width;
            public short Height;
            public bool LocalColorTableFlag
            {
                get
                {
                    return Palette is not null;
                }
            }
            public bool InterlaceFlag = true;
            public int LocalColorTableSize
            {
                get
                {
                    if (Palette is null)
                        return 0;
                    return Palette.GetLength(0);
                }
            }
            public int[] Palette;
            public byte[,] Rectangle;
            public GifImageBlock()
            {
            }
            public GifImageBlock(byte[,] Rectangle, int[] Palette = null)
            {
                if (Rectangle is null)
                    throw new InvalidDataException();
                this.Rectangle = Rectangle;
                Width = (short)Rectangle.GetLength(0);
                Height = (short)Rectangle.GetLength(1);
                if (Palette is not null)
                    this.Palette = (int[])Palette.Clone();
            }
            /// <param name="DelayTime">单位为0.01s</param>
            public void SetControl(short DelayTime)
            {
                EnableControlExtension = true;
                this.DelayTime = DelayTime;
            }
            /// <param name="DelayTime">单位为0.01s</param>
            public void SetControl(short DelayTime, byte TransparentColorIndex)
            {
                EnableControlExtension = true;
                TransparentColorFlag = true;
                this.DelayTime = DelayTime;
                this.TransparentColorIndex = TransparentColorIndex;
            }
        }
    }
}