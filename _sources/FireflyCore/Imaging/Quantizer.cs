// ==========================================================================
// 
// File:        Quantizer.vb
// Location:    Firefly.Imaging <Visual Basic .Net>
// Description: 量化器
// Version:     2009.01.21.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;

namespace Firefly.Imaging
{

    /// <summary>量化</summary>
    public static class Quantizer
    {

        /// <summary>按调色板量化，使用自定义颜色距离函数。</summary>
        public static byte QuantizeOnPalette(int Color, int[] Palette, ColorSpace.ColorDistance ColorDistance)
        {
            if (Palette.Length > 256)
                throw new NotSupportedException();
            var Index = default(byte);
            int d = 0x7FFFFFFF;
            for (int n = 0, loopTo = Palette.Length - 1; n <= loopTo; n++)
            {
                int cd = ColorDistance(Color, Palette[n]);
                if (cd < d)
                {
                    d = cd;
                    Index = (byte)n;
                }
            }
            return Index;
        }

        /// <summary>按调色板量化ARGB颜色，使用内置颜色距离函数。</summary>
        public static byte QuantizeOnPalette(int ARGB, int[] Palette)
        {
            return QuantizeOnPalette(ARGB, Palette, ColorSpace.ColourDistanceARGB);
        }

    }

    public class QuantizerCache
    {
        private Func<int, byte> q;

        public QuantizerCache(Func<int, byte> Quantizer)
        {
            q = Quantizer;
        }

        private Dictionary<int, byte> h = new Dictionary<int, byte>();
        public byte Quantize(int Color)
        {
            if (h.ContainsKey(Color))
                return h[Color];
            byte qc = q(Color);
            h.Add(Color, qc);
            return qc;
        }
    }
}