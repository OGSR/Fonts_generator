// ==========================================================================
// 
// File:        Glyph.vb
// Location:    Firefly.Glyphing <Visual Basic .Net>
// Description: 字形信息
// Version:     2009.11.21.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Drawing;
using Firefly.Imaging;
using Firefly.TextEncoding;

namespace Firefly.Glyphing
{
    /// <summary>字形描述信息</summary>
    public sealed class GlyphDescriptor
    {
        /// <summary>字符信息</summary>
        public StringCode c;

        /// <summary>物理包围盒，字符的图片信息在此包围盒内。</summary>
        public Rectangle PhysicalBox;

        /// <summary>虚拟包围盒，字符的显示相对位置为此包围盒。</summary>
        public Rectangle VirtualBox;
    }

    /// <summary>字形信息接口</summary>
    public interface IGlyph
    {
        /// <summary>字符信息</summary>
        StringCode c { get; }

        /// <summary>字符的32位颜色数据。</summary>
        int[,] Block { get; }

        /// <summary>字符的宽度。</summary>
        int PhysicalWidth { get; }

        /// <summary>字符的高度。</summary>
        int PhysicalHeight { get; }

        /// <summary>虚拟包围盒，字符的显示相对位置为此包围盒。</summary>
        Rectangle VirtualBox { get; }

        bool IsValid { get; }
    }

    /// <summary>字形提供器接口</summary>
    public interface IGlyphProvider : IDisposable
    {

        /// <summary>物理宽度</summary>
        int PhysicalWidth { get; }

        /// <summary>物理高度</summary>
        int PhysicalHeight { get; }

        /// <summary>获取字形</summary>
        IGlyph GetGlyph(StringCode c);
    }

    /// <summary>字形信息</summary>
    public sealed class Glyph : IGlyph
    {

        /// <summary>字符信息</summary>
        public StringCode c;

        /// <summary>字符的32位颜色数据。</summary>
        public int[,] Block;

        /// <summary>字符的宽度。</summary>
        public int PhysicalWidth
        {
            get
            {
                return Block.GetLength(0);
            }
        }

        /// <summary>字符的高度。</summary>
        public int PhysicalHeight
        {
            get
            {
                return Block.GetLength(1);
            }
        }

        /// <summary>虚拟包围盒，字符的显示相对位置为此包围盒。</summary>
        public Rectangle VirtualBox;

        private StringCode cI
        {
            get
            {
                return c;
            }
        }

        StringCode IGlyph.c { get => cI; }

        private int[,] BlockI
        {
            get
            {
                return Block;
            }
        }

        int[,] IGlyph.Block { get => BlockI; }

        private Rectangle VirtualBoxI
        {
            get
            {
                return VirtualBox;
            }
        }

        Rectangle IGlyph.VirtualBox { get => VirtualBoxI; }

        public bool IsValid { get; set; }
    }
}