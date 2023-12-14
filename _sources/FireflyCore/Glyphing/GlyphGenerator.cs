// ==========================================================================
// 
// File:        GlyphGenerator.vb
// Location:    Firefly.Glyphing <Visual Basic .Net>
// Description: 字形生成器
// Version:     2010.04.06.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Drawing;
using System.Linq;
using static System.Math;
using Firefly.Imaging;
using Firefly.TextEncoding;
using Microsoft.VisualBasic.CompilerServices;

namespace Firefly.Glyphing
{

    /// <summary>通道类型</summary>
    public enum ChannelPattern
    {
        Zero,
        Draw,
        One
    }

    /// <summary>字形生成器</summary>
    public class GlyphGenerator : IGlyphProvider
    {

        private int PhysicalWidthValue;
        private int PhysicalHeightValue;
        public int PhysicalWidth
        {
            get
            {
                return PhysicalWidthValue;
            }
        }
        public int PhysicalHeight
        {
            get
            {
                return PhysicalHeightValue;
            }
        }
        private int DrawOffsetX;
        private int DrawOffsetY;
        private int VirtualOffsetX;
        private int VirtualOffsetY;
        private int VirtualDeltaWidth;
        private int VirtualDeltaHeight;
        private bool AnchorLeft;
        private ChannelPattern[] ChannelPatterns;

        private Font Font;
        private Bitmap GlyphPiece;
        private Graphics g;

        public GlyphGenerator(string FontName, FontStyle FontStyle, int FontSize, int PhysicalWidth, int PhysicalHeight, int DrawOffsetX, int DrawOffsetY, int VirtualOffsetX, int VirtualOffsetY, int VirtualDeltaWidth, int VirtualDeltaHeight, bool AnchorLeft, ChannelPattern[] ChannelPatterns)
        {
            if (FontSize <= 0)
                throw new ArgumentOutOfRangeException();
            if (PhysicalWidth <= 0)
                throw new ArgumentOutOfRangeException();
            if (PhysicalHeight <= 0)
                throw new ArgumentOutOfRangeException();
            if (ChannelPatterns.Length != 4)
                throw new ArgumentException();

            PhysicalWidthValue = PhysicalWidth;
            PhysicalHeightValue = PhysicalHeight;
            this.DrawOffsetX = DrawOffsetX;
            this.DrawOffsetY = DrawOffsetY;
            this.VirtualOffsetX = VirtualOffsetX;
            this.VirtualOffsetY = VirtualOffsetY;
            this.VirtualDeltaWidth = VirtualDeltaWidth;
            this.VirtualDeltaHeight = VirtualDeltaHeight;
            this.AnchorLeft = AnchorLeft;
            this.ChannelPatterns = ChannelPatterns;

            Font = new Font(FontName, FontSize, FontStyle, GraphicsUnit.Pixel);
            GlyphPiece = new Bitmap(PhysicalWidth, PhysicalHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            g = Graphics.FromImage(GlyphPiece);
        }

        public IGlyph GetGlyph(StringCode c)
        {
            int[,] Block = new int[PhysicalWidthValue, PhysicalHeightValue];
            if (!c.HasUnicode)
            {
                for (int ry = 0, loopTo = PhysicalHeightValue - 1; ry <= loopTo; ry++)
                {
                    for (int rx = 0, loopTo1 = PhysicalWidthValue - 1; rx <= loopTo1; rx++)
                        Block[rx, ry] = 0;
                }
                return new Glyph() { c = c, Block = Block, VirtualBox = new Rectangle(0, 0, PhysicalWidthValue, PhysicalHeightValue) };
            }

            byte GetGray(object ARGB) => Conversions.ToByte(Operators.IntDivideObject(Operators.AddObject(Operators.AddObject(Operators.AddObject(Operators.RightShiftObject(Operators.AndObject(ARGB, 0xFF0000), 16), Operators.RightShiftObject(Operators.AndObject(ARGB, 0xFF00), 8)), Operators.AndObject(ARGB, 0xFF)), 2), 3));

            var DrawedRectangle = g.MeasureStringRectangle(c.Unicode.ToString(), Font);
            int X = (int)Round(Round((double)DrawedRectangle.Left));
            int Y = (int)Round(Round((double)DrawedRectangle.Top));
            int X2 = (int)Round(Round((double)DrawedRectangle.Right));
            int Y2 = (int)Round(Round((double)DrawedRectangle.Bottom));
            int Width = X2 - X;
            int Height = Y2 - Y;
            int ox = (PhysicalWidthValue - Width) / 2;
            int oy = (PhysicalHeightValue - Height) / 2;
            if (AnchorLeft)
                ox = 0;

            g.Clear(Color.Black);
            g.DrawString(c.Unicode.ToString(), Font, Brushes.White, ox - X + DrawOffsetX, oy - Y + DrawOffsetY);
            int[,] r = GlyphPiece.GetRectangle(0, 0, PhysicalWidthValue, PhysicalHeightValue);
            for (int ry = 0, loopTo2 = PhysicalHeightValue - 1; ry <= loopTo2; ry++)
            {
                for (int rx = 0, loopTo3 = PhysicalWidthValue - 1; rx <= loopTo3; rx++)
                {
                    byte L = GetGray(r[rx, ry]);
                    int ARGB = BitOperations.ConcatBits(GetChannel(ChannelPatterns[0], L), 8, GetChannel(ChannelPatterns[1], L), 8, GetChannel(ChannelPatterns[2], L), 8, GetChannel(ChannelPatterns[3], L), 8);
                    Block[rx, ry] = ARGB;
                }
            }

            int VirtualRectangleX = ox - VirtualDeltaWidth / 2 + VirtualOffsetX;
            int VirtualRectangleY = oy - VirtualDeltaHeight / 2 + VirtualOffsetY;
            int VirtualRectangleX2 = VirtualRectangleX + Width + VirtualDeltaWidth;
            int VirtualRectangleY2 = VirtualRectangleY + Height + VirtualDeltaHeight;
            if (VirtualRectangleX < 0)
                VirtualRectangleX = 0;
            if (VirtualRectangleY < 0)
                VirtualRectangleY = 0;
            if (VirtualRectangleX2 > PhysicalWidthValue)
                VirtualRectangleX2 = PhysicalWidthValue;
            if (VirtualRectangleY2 > PhysicalHeightValue)
                VirtualRectangleY2 = PhysicalHeightValue;
            return new Glyph() { c = c, Block = Block, VirtualBox = new Rectangle(VirtualRectangleX, VirtualRectangleY, VirtualRectangleX2 - VirtualRectangleX, VirtualRectangleY2 - VirtualRectangleY) };
        }

        private static byte GetChannel(ChannelPattern Pattern, byte L)
        {
            switch (Pattern)
            {
                case ChannelPattern.Zero:
                    {
                        return 0;
                    }
                case ChannelPattern.Draw:
                    {
                        return L;
                    }
                case ChannelPattern.One:
                    {
                        return 0xFF;
                    }

                default:
                    {
                        throw new InvalidOperationException();
                    }
            }
        }

        #region  IDisposable 支持 
        private bool DisposedValue = false; // 检测冗余的调用
        /// <summary>释放流的资源。</summary>
        /// <remarks>对继承者的说明：不要调用基类的Dispose()，而应调用Dispose(True)，否则会出现无限递归。</remarks>
        private void Dispose(bool Disposing)
        {
            if (DisposedValue)
                return;
            if (Disposing)
            {
                // 释放其他状态(托管对象)。
                if (Font is not null)
                    Font.Dispose();
                if (GlyphPiece is not null)
                    GlyphPiece.Dispose();
                if (g is not null)
                    g.Dispose();
            }

            // 释放您自己的状态(非托管对象)。
            // 将大型字段设置为 null。
            DisposedValue = true;
        }
        /// <summary>释放流的资源。</summary>
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入上面的 Dispose(ByVal disposing As Boolean) 中。
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    /// <summary>两倍超采样字形生成器</summary>
    public class GlyphGeneratorDoubleSample : IGlyphProvider
    {

        private int PhysicalWidthValue;
        private int PhysicalHeightValue;
        public int PhysicalWidth
        {
            get
            {
                return PhysicalWidthValue;
            }
        }
        public int PhysicalHeight
        {
            get
            {
                return PhysicalHeightValue;
            }
        }
        private int DrawOffsetX;
        private int DrawOffsetY;
        private int VirtualOffsetX;
        private int VirtualOffsetY;
        private int VirtualDeltaWidth;
        private int VirtualDeltaHeight;
        private bool AnchorLeft;
        private ChannelPattern[] ChannelPatterns;

        private Font Font;
        private Bitmap GlyphPiece;
        private Graphics g;

        public GlyphGeneratorDoubleSample(string FontName, FontStyle FontStyle, int FontSize, int PhysicalWidth, int PhysicalHeight, int DrawOffsetX, int DrawOffsetY, int VirtualOffsetX, int VirtualOffsetY, int VirtualDeltaWidth, int VirtualDeltaHeight, bool AnchorLeft, ChannelPattern[] ChannelPatterns)
        {
            if (FontSize <= 0)
                throw new ArgumentOutOfRangeException();
            if (PhysicalWidth <= 0)
                throw new ArgumentOutOfRangeException();
            if (PhysicalHeight <= 0)
                throw new ArgumentOutOfRangeException();
            if (ChannelPatterns.Length != 4)
                throw new ArgumentException();

            PhysicalWidthValue = PhysicalWidth;
            PhysicalHeightValue = PhysicalHeight;
            this.DrawOffsetX = DrawOffsetX;
            this.DrawOffsetY = DrawOffsetY;
            this.VirtualOffsetX = VirtualOffsetX;
            this.VirtualOffsetY = VirtualOffsetY;
            this.VirtualDeltaWidth = VirtualDeltaWidth;
            this.VirtualDeltaHeight = VirtualDeltaHeight;
            this.AnchorLeft = AnchorLeft;
            this.ChannelPatterns = ChannelPatterns;

            Font = new Font(FontName, FontSize * 2, FontStyle, GraphicsUnit.Pixel);
            GlyphPiece = new Bitmap(PhysicalWidth * 2, PhysicalHeight * 2, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            g = Graphics.FromImage(GlyphPiece);
        }

        private static int Mix(int a1, int a2, int a3, int a4)
        {
            int[] s = (from a in new int[] { a1, a2, a3, a4 }

                       orderby a
                       select a).ToArray();
            return (s[0] * 1 + s[1] * 3 + s[2] * 5 + s[3] * 7 + 15) / 16;
        }

        public IGlyph GetGlyph(StringCode c)
        {
            int[,] Block = new int[PhysicalWidthValue, PhysicalHeightValue];
            if (!c.HasUnicode)
            {
                for (int ry = 0, loopTo = PhysicalHeightValue - 1; ry <= loopTo; ry++)
                {
                    for (int rx = 0, loopTo1 = PhysicalWidthValue - 1; rx <= loopTo1; rx++)
                        Block[rx, ry] = 0;
                }
                return new Glyph() { c = c, Block = Block, VirtualBox = new Rectangle(0, 0, PhysicalWidthValue, PhysicalHeightValue) };
            }

            byte GetGray(object ARGB) => Conversions.ToByte(Operators.IntDivideObject(Operators.AddObject(Operators.AddObject(Operators.AddObject(Operators.RightShiftObject(Operators.AndObject(ARGB, 0xFF0000), 16), Operators.RightShiftObject(Operators.AndObject(ARGB, 0xFF00), 8)), Operators.AndObject(ARGB, 0xFF)), 2), 3));

            var DrawedRectangle = g.MeasureStringRectangle(c.Unicode.ToString(), Font);
            int X = (int)Round(Round((double)DrawedRectangle.Left));
            int Y = (int)Round(Round((double)DrawedRectangle.Top));
            int X2 = (int)Round(Round((double)DrawedRectangle.Right));
            int Y2 = (int)Round(Round((double)DrawedRectangle.Bottom));
            int Width = X2 - X;
            int Height = Y2 - Y;
            int ox = (PhysicalWidthValue * 2 - Width) / 2;
            int oy = (PhysicalHeightValue * 2 - Height) / 2;
            if (AnchorLeft)
                ox = ox % 2;

            g.Clear(Color.Black);
            g.DrawString(c.Unicode.ToString(), Font, Brushes.White, ox - X + DrawOffsetX * 2, oy - Y + DrawOffsetY * 2);
            int[,] r = GlyphPiece.GetRectangle(0, 0, PhysicalWidthValue * 2, PhysicalHeightValue * 2);
            for (int ry = 0, loopTo2 = PhysicalHeightValue - 1; ry <= loopTo2; ry++)
            {
                for (int rx = 0, loopTo3 = PhysicalWidthValue - 1; rx <= loopTo3; rx++)
                {
                    byte L1 = GetGray(r[rx * 2, ry * 2]);
                    byte L2 = GetGray(r[rx * 2 + 1, ry * 2]);
                    byte L3 = GetGray(r[rx * 2, ry * 2 + 1]);
                    byte L4 = GetGray(r[rx * 2 + 1, ry * 2 + 1]);
                    int L = Mix(L1, L2, L3, L4);
                    if (L < 0)
                        L = 0;
                    if (L > 255)
                        L = 255;
                    int ARGB = BitOperations.ConcatBits(GetChannel(ChannelPatterns[0], (byte)L), 8, GetChannel(ChannelPatterns[1], (byte)L), 8, GetChannel(ChannelPatterns[2], (byte)L), 8, GetChannel(ChannelPatterns[3], (byte)L), 8);
                    Block[rx, ry] = ARGB;
                }
            }

            int VirtualRectangleX = ox / 2 - VirtualDeltaWidth / 2 + VirtualOffsetX;
            int VirtualRectangleY = oy / 2 - VirtualDeltaHeight / 2 + VirtualOffsetY;
            int VirtualRectangleX2 = VirtualRectangleX + (Width + 1) / 2 + VirtualDeltaWidth;
            int VirtualRectangleY2 = VirtualRectangleY + (Height + 1) / 2 + VirtualDeltaHeight;
            if (VirtualRectangleX < 0)
                VirtualRectangleX = 0;
            if (VirtualRectangleY < 0)
                VirtualRectangleY = 0;
            if (VirtualRectangleX2 > PhysicalWidthValue)
                VirtualRectangleX2 = PhysicalWidthValue;
            if (VirtualRectangleY2 > PhysicalHeightValue)
                VirtualRectangleY2 = PhysicalHeightValue;
            return new Glyph() { c = c, Block = Block, VirtualBox = new Rectangle(VirtualRectangleX, VirtualRectangleY, VirtualRectangleX2 - VirtualRectangleX, VirtualRectangleY2 - VirtualRectangleY) };
        }

        private static byte GetChannel(ChannelPattern Pattern, byte L)
        {
            switch (Pattern)
            {
                case ChannelPattern.Zero:
                    {
                        return 0;
                    }
                case ChannelPattern.Draw:
                    {
                        return L;
                    }
                case ChannelPattern.One:
                    {
                        return 0xFF;
                    }

                default:
                    {
                        throw new InvalidOperationException();
                    }
            }
        }

        #region  IDisposable 支持 
        private bool DisposedValue = false; // 检测冗余的调用
        /// <summary>释放流的资源。</summary>
        /// <remarks>对继承者的说明：不要调用基类的Dispose()，而应调用Dispose(True)，否则会出现无限递归。</remarks>
        private void Dispose(bool Disposing)
        {
            if (DisposedValue)
                return;
            if (Disposing)
            {
                // 释放其他状态(托管对象)。
                if (Font is not null)
                    Font.Dispose();
                if (GlyphPiece is not null)
                    GlyphPiece.Dispose();
                if (g is not null)
                    g.Dispose();
            }

            // 释放您自己的状态(非托管对象)。
            // 将大型字段设置为 null。
            DisposedValue = true;
        }
        /// <summary>释放流的资源。</summary>
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入上面的 Dispose(ByVal disposing As Boolean) 中。
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}