// ==========================================================================
// 
// File:        Bmp.vb
// Location:    Firefly.Imaging <Visual Basic .Net>
// Description: 基本Bmp文件流类
// Version:     2010.02.15.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Drawing;
using System.IO;
using Firefly.TextEncoding;
using Microsoft.VisualBasic.CompilerServices;

namespace Firefly.Imaging
{

    /// <summary>基本Bmp文件流类</summary>
    /// <remarks>不能使用压缩等无用功能</remarks>
    public class Bmp : IDisposable
    {

        /// <summary>标志符。</summary>
        public const string Identifier = "BM";
        /// <summary>文件大小。</summary>
        public int FileSize
        {
            get
            {
                return BitmapDataOffset + BitmapDataSize;
            }
        }
        protected const int Reserved = 0;
        /// <summary>位图数据偏移量。</summary>
        public int BitmapDataOffset
        {
            get
            {
                if (PicBitsPerPixel == 1 || PicBitsPerPixel == 4 || PicBitsPerPixel == 8)
                {
                    return 54 + (1 << PicBitsPerPixel) * 4;
                }
                else if (PicBitsPerPixel == 16)
                {
                    return 70;
                }
                else
                {
                    return 54;
                }
            }
        }
        protected const int BitmapHeaderSize = 0x28;
        protected int PicWidth;
        /// <summary>宽度。</summary>
        public int Width
        {
            get
            {
                return PicWidth;
            }
            set
            {
                if (value >= 0)
                    PicWidth = value;
                else
                    return;
                CalcLineBitLength();
                BaseStream.SetLength(FileSize);
                BaseStream.Position = 18L;
                BaseStream.WriteInt32(PicWidth);
            }
        }
        protected int LineBitLength;
        protected void CalcLineBitLength()
        {
            if (PicWidth * PicBitsPerPixel % 32 != 0)
            {
                LineBitLength = (PicWidth * PicBitsPerPixel >> 5) + 1 << 5;
            }
            else
            {
                LineBitLength = PicWidth * PicBitsPerPixel;
            }
        }
        protected int PicHeight;
        /// <summary>高度。</summary>
        public int Height
        {
            get
            {
                return PicHeight;
            }
            set
            {
                if (value >= 0)
                    PicHeight = value;
                else
                    return;
                BaseStream.SetLength(FileSize);
                BaseStream.Position = 22L;
                BaseStream.WriteInt32(PicHeight);
            }
        }
        protected const short Planes = 1;
        protected short PicBitsPerPixel;
        /// <summary>位深度。</summary>
        public short BitsPerPixel
        {
            get
            {
                if (PicBitsPerPixel == 16 && !r5g6b5)
                    return 15;
                return PicBitsPerPixel;
            }
        }
        protected int PicCompression;
        /// <summary>压缩方式。</summary>
        public int Compression
        {
            get
            {
                return PicCompression;
            }
        }
        /// <summary>位图数据大小。</summary>
        public int BitmapDataSize
        {
            get
            {
                return LineBitLength * PicHeight >> 3;
            }
        }
        protected const int HResolution = 0; // 不用
        protected const int VResolution = 0; // 不用
        protected const int Colors = 0;
        protected const int ImportantColors = 0;
        protected int[] PicPalette;
        /// <summary>调色板。</summary>
        public int[] Palette
        {
            get
            {
                int[] Value;
                if (PicBitsPerPixel == 1 || PicBitsPerPixel == 4 || PicBitsPerPixel == 8)
                {
                    Value = new int[(1 << PicBitsPerPixel)];
                    BaseStream.Position = 0x36L;
                    for (int n = 0, loopTo = (1 << PicBitsPerPixel) - 1; n <= loopTo; n++)
                        Value[n] = BaseStream.ReadInt32();
                }
                else
                {
                    throw new InvalidDataException();
                }
                PicPalette = Value;
                return Value;
            }
            set
            {
                if (PicBitsPerPixel == 1 || PicBitsPerPixel == 4 || PicBitsPerPixel == 8)
                {
                    if (value.Length != 1 << PicBitsPerPixel)
                        throw new InvalidDataException();
                    BaseStream.Position = 0x36L;
                    for (int n = 0, loopTo = (1 << PicBitsPerPixel) - 1; n <= loopTo; n++)
                        BaseStream.WriteInt32(value[n]);
                }
                else
                {
                    throw new InvalidDataException();
                }
                PicPalette = (int[])value.Clone();
            }
        }

        protected StreamEx BaseStream;
        protected bool r5g6b5;
        private Bmp()
        {
        }

        /// <summary>新建内存流Bmp</summary>
        /// <param name="BitsPerPixel">Bmp位数：可以取1、4、8、15、16、24、32</param>
        public Bmp(int Width, int Height, short BitsPerPixel = 24)
        {
            BaseStream = new MemoryStream();
            if (Width < 0 || Height < 0)
            {
                BaseStream.Close();
                throw new InvalidDataException();
            }
            PicWidth = Width;
            PicHeight = Height;
            if (BitsPerPixel == 1 || BitsPerPixel == 4 || BitsPerPixel == 8)
            {
                PicBitsPerPixel = BitsPerPixel;
                PicPalette = new int[((int)Math.Round(Math.Pow(2d, PicBitsPerPixel)))];
            }
            else if (BitsPerPixel == 15 || BitsPerPixel == 16)
            {
                r5g6b5 = BitsPerPixel == 16;
                PicBitsPerPixel = 16;
                PicCompression = 3;
            }
            else if (BitsPerPixel == 24 || BitsPerPixel == 32)
            {
                PicBitsPerPixel = BitsPerPixel;
            }
            else
            {
                throw new NotSupportedException("PicBitsPerPixelNotSupported");
            }
            CalcLineBitLength();
            BaseStream.SetLength(FileSize);

            BaseStream.Position = 0L;
            for (int n = 0, loopTo = Identifier.Length - 1; n <= loopTo; n++)
                BaseStream.WriteByte((byte)String32.AscQ(Identifier[n]));
            BaseStream.WriteInt32(FileSize);
            BaseStream.WriteInt32(Reserved);
            BaseStream.WriteInt32(BitmapDataOffset);
            BaseStream.WriteInt32(BitmapHeaderSize);
            BaseStream.WriteInt32(PicWidth);
            BaseStream.WriteInt32(PicHeight);
            BaseStream.WriteInt16(Planes);
            BaseStream.WriteInt16(PicBitsPerPixel);
            BaseStream.WriteInt32(PicCompression);
            BaseStream.WriteInt32(BitmapDataSize);
            BaseStream.WriteInt32(HResolution);
            BaseStream.WriteInt32(VResolution);
            BaseStream.WriteInt32(Colors);
            BaseStream.WriteInt32(ImportantColors);

            if (PicCompression == 3 && PicBitsPerPixel == 16)
            {
                if (r5g6b5)
                {
                    BaseStream.WriteInt32(0xF800);
                    BaseStream.WriteInt32(0x7E0);
                    BaseStream.WriteInt32(0x1F);
                    BaseStream.WriteInt32(0x0);
                }
                else
                {
                    BaseStream.WriteInt32(0x7C00);
                    BaseStream.WriteInt32(0x3E0);
                    BaseStream.WriteInt32(0x1F);
                    BaseStream.WriteInt32(0x0);
                }
            }
        }
        /// <summary>新建文件流Bmp</summary>
        /// <param name="BitsPerPixel">Bmp位数：可以取1、4、8、15、16、24、32</param>
        public Bmp(string Path, int Width, int Height, short BitsPerPixel = 24)
        {
            BaseStream = new FileStream(Path, FileMode.Create);
            if (Width < 0 || Height < 0)
            {
                BaseStream.Close();
                throw new InvalidDataException();
            }
            PicWidth = Width;
            PicHeight = Height;
            if (BitsPerPixel == 1 || BitsPerPixel == 4 || BitsPerPixel == 8)
            {
                PicBitsPerPixel = BitsPerPixel;
                PicPalette = new int[((int)Math.Round(Math.Pow(2d, PicBitsPerPixel)))];
            }
            else if (BitsPerPixel == 15 || BitsPerPixel == 16)
            {
                r5g6b5 = BitsPerPixel == 16;
                PicBitsPerPixel = 16;
                PicCompression = 3;
            }
            else if (BitsPerPixel == 24 || BitsPerPixel == 32)
            {
                PicBitsPerPixel = BitsPerPixel;
            }
            else
            {
                throw new NotSupportedException("PicBitsPerPixelNotSupported");
            }
            CalcLineBitLength();
            BaseStream.SetLength(FileSize);

            BaseStream.Position = 0L;
            for (int n = 0, loopTo = Identifier.Length - 1; n <= loopTo; n++)
                BaseStream.WriteByte((byte)String32.AscQ(Identifier[n]));
            BaseStream.WriteInt32(FileSize);
            BaseStream.WriteInt32(Reserved);
            BaseStream.WriteInt32(BitmapDataOffset);
            BaseStream.WriteInt32(BitmapHeaderSize);
            BaseStream.WriteInt32(PicWidth);
            BaseStream.WriteInt32(PicHeight);
            BaseStream.WriteInt16(Planes);
            BaseStream.WriteInt16(PicBitsPerPixel);
            BaseStream.WriteInt32(PicCompression);
            BaseStream.WriteInt32(BitmapDataSize);
            BaseStream.WriteInt32(HResolution);
            BaseStream.WriteInt32(VResolution);
            BaseStream.WriteInt32(Colors);
            BaseStream.WriteInt32(ImportantColors);

            if (PicCompression == 3 && PicBitsPerPixel == 16)
            {
                if (r5g6b5)
                {
                    BaseStream.WriteInt32(0xF800);
                    BaseStream.WriteInt32(0x7E0);
                    BaseStream.WriteInt32(0x1F);
                    BaseStream.WriteInt32(0x0);
                }
                else
                {
                    BaseStream.WriteInt32(0x7C00);
                    BaseStream.WriteInt32(0x3E0);
                    BaseStream.WriteInt32(0x1F);
                    BaseStream.WriteInt32(0x0);
                }
            }
        }

        /// <summary>已重载。从流打开一个位图。</summary>
        public static Bmp Open(ZeroPositionStreamPasser sp)
        {
            var s = sp.GetStream();
            var bf = new Bmp();
            bf.BaseStream = s;
            bf.BaseStream.Position = 0L;
            for (int n = 0, loopTo = Identifier.Length - 1; n <= loopTo; n++)
            {
                if (bf.BaseStream.ReadByte() != String32.AscQ(Identifier[n]))
                {
                    throw new InvalidDataException();
                }
            }
            bf.BaseStream.ReadInt32(); // 跳过File Size
            bf.BaseStream.ReadInt32(); // 跳过Reserved
            bf.BaseStream.ReadInt32(); // 跳过Bitmap Data Offset
            bf.BaseStream.ReadInt32(); // 跳过Bitmap Header Size
            bf.PicWidth = bf.BaseStream.ReadInt32();
            bf.PicHeight = bf.BaseStream.ReadInt32();
            if (bf.PicWidth < 0 || bf.PicHeight < 0)
                throw new InvalidDataException();
            bf.BaseStream.ReadInt16(); // 跳过Planes
            bf.PicBitsPerPixel = bf.BaseStream.ReadInt16();
            bf.PicCompression = bf.BaseStream.ReadInt32();
            bf.BaseStream.ReadInt32(); // 跳过Bitmap Data Size
            bf.BaseStream.ReadInt32(); // 跳过HResolution
            bf.BaseStream.ReadInt32(); // 跳过VResolution
            bf.BaseStream.ReadInt32(); // 跳过Colors
            bf.BaseStream.ReadInt32(); // 跳过Important Colors

            if (bf.PicCompression != 0)
            {
                if (bf.PicCompression == 3 && bf.PicBitsPerPixel == 16)
                {
                    bf.r5g6b5 = Conversions.ToBoolean(bf.BaseStream.ReadInt32() & 0x8000); // 检验红色掩码是否从最高位开始
                    bf.BaseStream.ReadInt32(); // 跳过绿色掩码
                    bf.BaseStream.ReadInt32(); // 跳过蓝色掩码
                    bf.BaseStream.ReadInt32();
                }
                else
                {
                    throw new InvalidDataException();
                }
            }

            if (bf.PicBitsPerPixel == 1 || bf.PicBitsPerPixel == 4 || bf.PicBitsPerPixel == 8)
            {
                bf.PicPalette = new int[(1 << bf.PicBitsPerPixel)];
                for (int n = 0, loopTo1 = (1 << bf.PicBitsPerPixel) - 1; n <= loopTo1; n++)
                    bf.PicPalette[n] = bf.BaseStream.ReadInt32();
            }
            else if (bf.PicBitsPerPixel == 16 || bf.PicBitsPerPixel == 24 || bf.PicBitsPerPixel == 32)
            {
            }
            else
            {
                throw new NotSupportedException("PicBitsPerPixelNotSupported");
            }

            bf.CalcLineBitLength();
            return bf;
        }
        /// <summary>已重载。从文件打开一个位图。</summary>
        public static Bmp Open(string Path)
        {
            var s = new StreamEx(Path, FileMode.Open);
            try
            {
                return Open(s);
            }
            catch
            {
                s.Close();
                throw;
            }
        }
        /// <summary>关闭。</summary>
        public void Close()
        {
            BaseStream.Close();
        }
        /// <summary>转换为System.Drawing.Bitmap。</summary>
        public Bitmap ToBitmap()
        {
            BaseStream.Position = 0L;
            BaseStream.Flush();
            return new Bitmap(BaseStream);
        }
        /// <summary>保存到流。</summary>
        public void SaveTo(ZeroPositionStreamPasser sp)
        {
            var s = sp.GetStream();
            BaseStream.Position = 0L;
            s.WriteFromStream(BaseStream, BaseStream.Length);
        }

        protected int get_Pos(int x, int y)
        {
            return LineBitLength * (Height - 1 - y) + x * PicBitsPerPixel >> 3;
        }
        /// <summary>获得像素点。</summary>
        public int GetPixel(int x, int y)
        {
            if (x < 0 || x > PicWidth - 1 || y < 0 || y > PicHeight - 1)
                return 0;
            BaseStream.Position = BitmapDataOffset + get_Pos(x, y);
            switch (PicBitsPerPixel)
            {
                case 1:
                    {
                        return BaseStream.ReadByte() >> 7 - x % 8 & 1;
                    }
                case 4:
                    {
                        return BaseStream.ReadByte() >> 4 * (1 - x % 2) & 15;
                    }
                case 8:
                    {
                        return BaseStream.ReadByte();
                    }
                case 16:
                    {
                        return BaseStream.ReadInt16();
                    }
                case 24:
                    {
                        return BaseStream.ReadInt32() & 0xFFFFFF;
                    }
                case 32:
                    {
                        return BaseStream.ReadInt32();
                    }

                default:
                    {
                        throw new InvalidOperationException();
                    }
            }
        }
        /// <summary>设置像素点。</summary>
        public void SetPixel(int x, int y, int c)
        {
            if (x < 0 || x > PicWidth - 1 || y < 0 || y > PicHeight - 1)
                return;
            BaseStream.Position = BitmapDataOffset + get_Pos(x, y);
            switch (PicBitsPerPixel)
            {
                case 1:
                    {
                        byte k = BaseStream.ReadByte();
                        k = (byte)(k & ~(byte)(1 << 7 - x % 8) | (byte)((Conversions.ToByte(c != 0) & 1) << 7 - x % 8));
                        BaseStream.Position -= 1L;
                        BaseStream.WriteByte(k);
                        break;
                    }
                case 4:
                    {
                        byte k = BaseStream.ReadByte();
                        k = (byte)(k & ~(byte)(15 << 4 * (1 - x % 2)) | (byte)((c & 15) << 4 * (1 - x % 2)));
                        BaseStream.Position -= 1L;
                        BaseStream.WriteByte(k);
                        break;
                    }
                case 8:
                    {
                        BaseStream.WriteByte((byte)(c & 0xFF));
                        break;
                    }
                case 16:
                    {
                        BaseStream.WriteInt16(DirectIntConvert.CID(c & 0xFFFF));
                        break;
                    }
                case 24:
                    {
                        BaseStream.WriteInt16(DirectIntConvert.CID(c & 0xFFFF));
                        BaseStream.WriteByte((byte)(c >> 16 & 0xFF));
                        break;
                    }
                case 32:
                    {
                        BaseStream.WriteInt32(c);
                        break;
                    }
            }
        }
        /// <summary>获取矩形。</summary>
        public int[,] GetRectangle(int x, int y, int w, int h)
        {
            if (w < 0 || h < 0)
                return null;
            int[,] a = new int[w, h];
            int ox, oy;
            if (y < 0)
            {
                h = h + y;
                oy = 0;
            }
            else
            {
                oy = y;
            }
            if (oy + h > PicHeight)
            {
                h = PicHeight - oy;
            }
            if (x < 0)
            {
                ox = 0;
            }
            else
            {
                ox = x;
            }
            if (ox + w > PicWidth)
            {
                w = PicWidth - ox;
            }
            int xl = ox - x;
            int xu;
            if (x >= 0)
            {
                xu = w + ox - x - 1;
            }
            else
            {
                xu = w - 1;
            }

            var t = default(byte);
            var t1 = default(byte);
            for (int m = oy + h - y - 1, loopTo = oy - y; m >= loopTo; m -= 1)
            {
                BaseStream.Position = BitmapDataOffset + get_Pos(ox, oy + m);
                switch (PicBitsPerPixel)
                {
                    case 1:
                        {
                            if ((ox & 7) != 0)
                            {
                                t1 = BaseStream.ReadByte();
                                t1 = (byte)(t1 << (ox & 7));
                            }
                            for (int n = xl, loopTo1 = xu; n <= loopTo1; n++)
                            {
                                if ((n & 7) == 0)
                                {
                                    t1 = BaseStream.ReadByte();
                                }
                                a[n, m] = (t1 & 128) >> 7;
                                t1 = (byte)(t1 << 1);
                            }

                            break;
                        }
                    case 4:
                        {
                            if ((ox & 1) == 1)
                            {
                                t1 = BaseStream.ReadByte();
                            }
                            for (int n = xl, loopTo2 = xu; n <= loopTo2; n++)
                            {
                                if ((n & 1) == 0)
                                {
                                    t1 = BaseStream.ReadByte();
                                    a[n, m] = t1 >> 4;
                                }
                                else
                                {
                                    a[n, m] = t1 & 15;
                                }
                            }

                            break;
                        }
                    case 8:
                        {
                            for (int n = xl, loopTo3 = xu; n <= loopTo3; n++)
                                a[n, m] = BaseStream.ReadByte();
                            break;
                        }
                    case 16:
                        {
                            for (int n = xl, loopTo4 = xu; n <= loopTo4; n++)
                                a[n, m] = DirectIntConvert.EID(BaseStream.ReadInt16());
                            break;
                        }
                    case 24:
                        {
                            for (int n = xl, loopTo5 = xu; n <= loopTo5; n++)
                            {
                                a[n, m] = DirectIntConvert.EID(BaseStream.ReadInt16());
                                a[n, m] = a[n, m] | BaseStream.ReadByte() << 16;
                                a[n, m] = int.MinValue + 0x7F000000 | a[n, m];
                            }

                            break;
                        }
                    case 32:
                        {
                            for (int n = xl, loopTo6 = xu; n <= loopTo6; n++)
                                a[n, m] = BaseStream.ReadInt32();
                            break;
                        }
                }
            }
            return a;
        }
        /// <summary>获取矩形。表示为字节。仅供8位及以下图片使用。</summary>
        public byte[,] GetRectangleBytes(int x, int y, int w, int h)
        {
            if (w < 0 || h < 0)
                return null;
            byte[,] a = new byte[w, h];
            int ox, oy;
            if (y < 0)
            {
                h = h + y;
                oy = 0;
            }
            else
            {
                oy = y;
            }
            if (oy + h > PicHeight)
            {
                h = PicHeight - oy;
            }
            if (x < 0)
            {
                ox = 0;
            }
            else
            {
                ox = x;
            }
            if (ox + w > PicWidth)
            {
                w = PicWidth - ox;
            }
            int xl = ox - x;
            int xu;
            if (x >= 0)
            {
                xu = w + ox - x - 1;
            }
            else
            {
                xu = w - 1;
            }

            var t = default(byte);
            var t1 = default(byte);
            for (int m = oy + h - y - 1, loopTo = oy - y; m >= loopTo; m -= 1)
            {
                BaseStream.Position = BitmapDataOffset + get_Pos(ox, oy + m);
                switch (PicBitsPerPixel)
                {
                    case 1:
                        {
                            if ((ox & 7) != 0)
                            {
                                t1 = BaseStream.ReadByte();
                                t1 = (byte)(t1 << (ox & 7));
                            }
                            for (int n = xl, loopTo1 = xu; n <= loopTo1; n++)
                            {
                                if ((n & 7) == 0)
                                {
                                    t1 = BaseStream.ReadByte();
                                }
                                a[n, m] = (byte)((t1 & 128) >> 7);
                                t1 = (byte)(t1 << 1);
                            }

                            break;
                        }
                    case 4:
                        {
                            if ((ox & 1) == 1)
                            {
                                t1 = BaseStream.ReadByte();
                            }
                            for (int n = xl, loopTo2 = xu; n <= loopTo2; n++)
                            {
                                if ((n & 1) == 0)
                                {
                                    t1 = BaseStream.ReadByte();
                                    a[n, m] = (byte)(t1 >> 4);
                                }
                                else
                                {
                                    a[n, m] = (byte)(t1 & 15);
                                }
                            }

                            break;
                        }
                    case 8:
                        {
                            for (int n = xl, loopTo3 = xu; n <= loopTo3; n++)
                                a[n, m] = BaseStream.ReadByte();
                            break;
                        }
                    case 16:
                    case 24:
                    case 32:
                        {
                            throw new InvalidOperationException();
                        }
                }
            }
            return a;
        }
        /// <summary>已重载。设置矩形。</summary>
        public void SetRectangle(int x, int y, int[,] a)
        {
            if (a is null)
                return;
            int w = a.GetLength(0);
            int h = a.GetLength(1);
            int ox, oy;
            if (y < 0)
            {
                h = h + y;
                oy = 0;
            }
            else
            {
                oy = y;
            }
            if (oy + h > PicHeight)
            {
                h = PicHeight - oy;
            }
            if (x < 0)
            {
                ox = 0;
            }
            else
            {
                ox = x;
            }
            if (ox + w > PicWidth)
            {
                w = PicWidth - ox;
            }
            int xl = ox - x;
            int xu;
            if (x >= 0)
            {
                xu = w + ox - x - 1;
            }
            else
            {
                xu = w - 1;
            }

            var t = default(byte);
            var t1 = default(byte);
            for (int m = oy + h - y - 1, loopTo = oy - y; m >= loopTo; m -= 1)
            {
                BaseStream.Position = BitmapDataOffset + get_Pos(ox, oy + m);
                switch (PicBitsPerPixel)
                {
                    case 1:
                        {
                            if ((ox & 7) != 0)
                            {
                                t1 = (byte)(BaseStream.ReadByte() >> (8 - ox & 7));
                                BaseStream.Position -= 1L;
                            }
                            int n;
                            var loopTo1 = xu;
                            for (n = xl; n <= loopTo1; n++)
                            {
                                t1 = (byte)(t1 << 1);
                                if (a[n, m] != 0)
                                    t1 = (byte)(t1 | 1);
                                if ((n & 7) == 7)
                                {
                                    BaseStream.WriteByte(t1);
                                }
                            }
                            if ((n & 7) != 0)
                            {
                                t1 = (byte)(t1 << (7 - n & 7));
                                BaseStream.WriteByte(t1);
                            }

                            break;
                        }
                    case 4:
                        {
                            if ((ox & 1) != 0)
                            {
                                t1 = (byte)(BaseStream.ReadByte() >> 4 * (2 - ox & 1));
                                BaseStream.Position -= 1L;
                            }
                            int n;
                            var loopTo2 = xu;
                            for (n = xl; n <= loopTo2; n++)
                            {
                                t1 = (byte)(t1 << 4);
                                t1 = (byte)(t1 | (byte)(a[n, m] & 15));
                                if ((n & 1) == 1)
                                {
                                    BaseStream.WriteByte(t1);
                                }
                            }
                            if ((n & 1) != 0)
                            {
                                t1 = (byte)(t1 << 4);
                                BaseStream.WriteByte(t1);
                            }

                            break;
                        }
                    case 8:
                        {
                            for (int n = xl, loopTo3 = xu; n <= loopTo3; n++)
                                BaseStream.WriteByte((byte)(a[n, m] & 0xFF));
                            break;
                        }
                    case 16:
                        {
                            for (int n = xl, loopTo4 = xu; n <= loopTo4; n++)
                                BaseStream.WriteInt16(DirectIntConvert.CID(a[n, m] & 0xFFFF));
                            break;
                        }
                    case 24:
                        {
                            for (int n = xl, loopTo5 = xu; n <= loopTo5; n++)
                            {
                                BaseStream.WriteInt16(DirectIntConvert.CID(a[n, m] & 0xFFFF));
                                BaseStream.WriteByte((byte)(a[n, m] >> 16 & 0xFF));
                            }

                            break;
                        }
                    case 32:
                        {
                            for (int n = xl, loopTo6 = xu; n <= loopTo6; n++)
                                BaseStream.WriteInt32(a[n, m]);
                            break;
                        }
                }
            }
        }
        /// <summary>已重载。设置矩形。</summary>
        public void SetRectangle(int x, int y, byte[,] a)
        {
            if (a is null)
                return;
            int w = a.GetLength(0);
            int h = a.GetLength(1);
            int ox, oy;
            if (y < 0)
            {
                h = h + y;
                oy = 0;
            }
            else
            {
                oy = y;
            }
            if (oy + h > PicHeight)
            {
                h = PicHeight - oy;
            }
            if (x < 0)
            {
                ox = 0;
            }
            else
            {
                ox = x;
            }
            if (ox + w > PicWidth)
            {
                w = PicWidth - ox;
            }
            int xl = ox - x;
            int xu;
            if (x >= 0)
            {
                xu = w + ox - x - 1;
            }
            else
            {
                xu = w - 1;
            }

            var t = default(byte);
            var t1 = default(byte);
            for (int m = oy + h - y - 1, loopTo = oy - y; m >= loopTo; m -= 1)
            {
                BaseStream.Position = BitmapDataOffset + get_Pos(ox, oy + m);
                switch (PicBitsPerPixel)
                {
                    case 1:
                        {
                            if ((ox & 7) != 0)
                            {
                                t1 = (byte)(BaseStream.ReadByte() >> (8 - ox & 7));
                                BaseStream.Position -= 1L;
                            }
                            int n;
                            var loopTo1 = xu;
                            for (n = xl; n <= loopTo1; n++)
                            {
                                t1 = (byte)(t1 << 1);
                                if (a[n, m] != 0)
                                    t1 = (byte)(t1 | 1);
                                if ((n & 7) == 7)
                                {
                                    BaseStream.WriteByte(t1);
                                }
                            }
                            if ((n & 7) != 0)
                            {
                                t1 = (byte)(t1 << (7 - n & 7));
                                BaseStream.WriteByte(t1);
                            }

                            break;
                        }
                    case 4:
                        {
                            if ((ox & 1) != 0)
                            {
                                t1 = (byte)(BaseStream.ReadByte() >> 4 * (2 - ox & 1));
                                BaseStream.Position -= 1L;
                            }
                            int n;
                            var loopTo2 = xu;
                            for (n = xl; n <= loopTo2; n++)
                            {
                                t1 = (byte)(t1 << 4);
                                t1 = (byte)(t1 | (byte)(a[n, m] & 15));
                                if ((n & 1) == 1)
                                {
                                    BaseStream.WriteByte(t1);
                                }
                            }
                            if ((n & 1) != 0)
                            {
                                t1 = (byte)(t1 << 4);
                                BaseStream.WriteByte(t1);
                            }

                            break;
                        }
                    case 8:
                        {
                            for (int n = xl, loopTo3 = xu; n <= loopTo3; n++)
                                BaseStream.WriteByte((byte)(a[n, m] & 0xFF));
                            break;
                        }
                    case 16:
                    case 24:
                    case 32:
                        {
                            throw new InvalidOperationException();
                        }
                }
            }
        }
        /// <summary>获取矩形为ARGB整数。对非24、32位位图会进行转换。</summary>
        public int[,] GetRectangleAsARGB(int x, int y, int w, int h)
        {
            int[,] a = GetRectangle(x, y, w, h);
            switch (PicBitsPerPixel)
            {
                case 1:
                case 4:
                case 8:
                    {
                        for (int py = 0, loopTo = h - 1; py <= loopTo; py++)
                        {
                            for (int px = 0, loopTo1 = w - 1; px <= loopTo1; px++)
                                a[px, py] = PicPalette[a[px, py]];
                        }

                        break;
                    }
                case 16:
                    {
                        if (r5g6b5)
                        {
                            for (int py = 0, loopTo2 = h - 1; py <= loopTo2; py++)
                            {
                                for (int px = 0, loopTo3 = w - 1; px <= loopTo3; px++)
                                    a[px, py] = ColorSpace.RGB16To32(DirectIntConvert.CID(a[px, py]));
                            }
                        }
                        else
                        {
                            for (int py = 0, loopTo4 = h - 1; py <= loopTo4; py++)
                            {
                                for (int px = 0, loopTo5 = w - 1; px <= loopTo5; px++)
                                    a[px, py] = ColorSpace.RGB15To32(DirectIntConvert.CID(a[px, py]));
                            }
                        }

                        break;
                    }
                case 24:
                case 32:
                    {
                        break;
                    }
            }
            return a;
        }
        /// <summary>从ARGB整数设置矩形。对非24、32位位图会进行转换。使用自定义的量化器。</summary>
        public void SetRectangleFromARGB(int x, int y, int[,] a, Func<int, byte> Quantize)
        {
            if (a is null)
                return;
            int w = a.GetLength(0);
            int h = a.GetLength(1);
            int ox, oy;
            if (y < 0)
            {
                h = h + y;
                oy = 0;
            }
            else
            {
                oy = y;
            }
            if (oy + h > PicHeight)
            {
                h = PicHeight - oy;
            }
            if (x < 0)
            {
                ox = 0;
            }
            else
            {
                ox = x;
            }
            if (ox + w > PicWidth)
            {
                w = PicWidth - ox;
            }
            int xl = ox - x;
            int xu;
            if (x >= 0)
            {
                xu = w + ox - x - 1;
            }
            else
            {
                xu = w - 1;
            }

            var t = default(byte);
            var t1 = default(byte);
            for (int m = oy + h - y - 1, loopTo = oy - y; m >= loopTo; m -= 1)
            {
                BaseStream.Position = BitmapDataOffset + get_Pos(ox, oy + m);
                switch (PicBitsPerPixel)
                {
                    case 1:
                        {
                            if ((ox & 7) != 0)
                            {
                                t1 = (byte)(BaseStream.ReadByte() >> (8 - ox & 7));
                                BaseStream.Position -= 1L;
                            }
                            int n;
                            var loopTo1 = xu;
                            for (n = xl; n <= loopTo1; n++)
                            {
                                t1 = (byte)(t1 << 1);
                                if (Quantize(a[n, m]) != 0)
                                    t1 = (byte)(t1 | 1);
                                if ((n & 7) == 7)
                                {
                                    BaseStream.WriteByte(t1);
                                }
                            }
                            if ((n & 7) != 0)
                            {
                                t1 = (byte)(t1 << (7 - n & 7));
                                BaseStream.WriteByte(t1);
                            }

                            break;
                        }
                    case 4:
                        {
                            if ((ox & 1) != 0)
                            {
                                t1 = (byte)(BaseStream.ReadByte() >> 4 * (2 - ox & 1));
                                BaseStream.Position -= 1L;
                            }
                            int n;
                            var loopTo2 = xu;
                            for (n = xl; n <= loopTo2; n++)
                            {
                                t1 = (byte)(t1 << 4);
                                t1 = (byte)(t1 | (byte)(Quantize(a[n, m]) & 15));
                                if ((n & 1) == 1)
                                {
                                    BaseStream.WriteByte(t1);
                                }
                            }
                            if ((n & 1) != 0)
                            {
                                t1 = (byte)(t1 << 4);
                                BaseStream.WriteByte(t1);
                            }

                            break;
                        }
                    case 8:
                        {
                            for (int n = xl, loopTo3 = xu; n <= loopTo3; n++)
                                BaseStream.WriteByte((byte)(Quantize(a[n, m]) & 0xFF));
                            break;
                        }
                    case 16:
                        {
                            if (r5g6b5)
                            {
                                for (int n = xl, loopTo4 = xu; n <= loopTo4; n++)
                                    BaseStream.WriteInt16(DirectIntConvert.CID(ColorSpace.RGB32To16(a[n, m]) & 0xFFFF));
                            }
                            else
                            {
                                for (int n = xl, loopTo5 = xu; n <= loopTo5; n++)
                                    BaseStream.WriteInt16(DirectIntConvert.CID(ColorSpace.RGB32To15(a[n, m]) & 0xFFFF));
                            }

                            break;
                        }
                    case 24:
                        {
                            for (int n = xl, loopTo6 = xu; n <= loopTo6; n++)
                            {
                                BaseStream.WriteInt16(DirectIntConvert.CID(a[n, m] & 0xFFFF));
                                BaseStream.WriteByte((byte)(a[n, m] >> 16 & 0xFF));
                            }

                            break;
                        }
                    case 32:
                        {
                            for (int n = xl, loopTo7 = xu; n <= loopTo7; n++)
                                BaseStream.WriteInt32(a[n, m]);
                            break;
                        }
                }
            }
        }
        /// <summary>从ARGB整数设置矩形。对非24、32位位图会进行转换。</summary>
        public void SetRectangleFromARGB(int x, int y, int[,] a)
        {
            var qc = new QuantizerCache(c => Quantizer.QuantizeOnPalette(c, PicPalette));
            SetRectangleFromARGB(x, y, a, qc.Quantize);
        }

        #region  IDisposable 支持 
        private bool DisposedValue = false; // 检测冗余的调用
        /// <summary>释放资源。</summary>
        /// <remarks>对继承者的说明：不要调用基类的Dispose()，而应调用Dispose(True)，否则会出现无限递归。</remarks>
        protected virtual void Dispose(bool Disposing)
        {
            if (DisposedValue)
                return;
            if (Disposing)
            {
                // 释放其他状态(托管对象)。
            }

            // 释放您自己的状态(非托管对象)。
            // 将大型字段设置为 null。
            BaseStream.Dispose();
            DisposedValue = true;
        }
        /// <summary>释放资源。</summary>
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入上面的 Dispose(ByVal disposing As Boolean) 中。
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }

}