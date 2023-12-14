// ==========================================================================
// 
// File:        GlyphArranger.vb
// Location:    Firefly.Glyphing <Visual Basic .Net>
// Description: 字形集合
// Version:     2010.04.11.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static System.Math;

namespace Firefly.Glyphing
{
    public delegate void SetRectangleFromARGB(int x, int y, int[,] a);

    public interface IGlyphArranger
    {
        int GetLeastGlyphCount(int PicWidth, int PicHeight);
        Size GetPreferredSize(IEnumerable<IGlyph> Glyphs);
        int GetPreferredHeight(IEnumerable<IGlyph> Glyphs, int PicWidth);
        IEnumerable<GlyphDescriptor> GetGlyphArrangement(IEnumerable<IGlyph> Glyphs, int PicWidth, int PicHeight);
    }

    public class GlyphArranger : IGlyphArranger
    {

        private int PhysicalWidth;
        private int PhysicalHeight;

        public GlyphArranger(int PhysicalWidth, int PhysicalHeight)
        {
            if (PhysicalWidth <= 0)
                throw new ArgumentOutOfRangeException();
            if (PhysicalHeight <= 0)
                throw new ArgumentOutOfRangeException();
            this.PhysicalWidth = PhysicalWidth;
            this.PhysicalHeight = PhysicalHeight;
        }

        public int GetLeastGlyphCount(int PicWidth, int PicHeight)
        {
            int NumGlyphInLine = PicWidth / PhysicalWidth;
            int NumGlyphOfPart = NumGlyphInLine * (PicHeight / PhysicalHeight);
            return NumGlyphOfPart;
        }

        public Size GetPreferredSize(IEnumerable<IGlyph> Glyphs)
        {
            int Count = Glyphs.Count();
            int k = (int)Round(Ceiling(Log(Sqrt(Count * PhysicalWidth * PhysicalHeight), 2d)));
            while (true)
            {
                double PicSize = Pow(2d, k);

                long NumGlyphInLine = (long)Round(PicSize) / PhysicalWidth;
                long NumGlyphOfPart = NumGlyphInLine * ((long)Round(PicSize) / PhysicalHeight);

                if (Count <= NumGlyphOfPart)
                    return new Size((int)Round(PicSize), (int)Round(PicSize));
                k += 1;
            }
        }

        public int GetPreferredHeight(IEnumerable<IGlyph> Glyphs, int PicWidth)
        {
            int Count = Glyphs.Count();
            int k = (int)Round(Ceiling(Log(Count * PhysicalWidth * PhysicalHeight / (double)PicWidth)));
            int NumGlyphInLine = PicWidth / PhysicalWidth;

            while (true)
            {
                double PicHeight = Pow(2d, k);

                long NumGlyphOfPart = NumGlyphInLine * ((long)Round(PicHeight) / PhysicalHeight);

                if (Count <= NumGlyphOfPart)
                    return (int)Round(PicHeight);
                k += 1;
            }
        }

        public IEnumerable<GlyphDescriptor> GetGlyphArrangement(IEnumerable<IGlyph> Glyphs, int PicWidth, int PicHeight)
        {
            int NumGlyphInLine = PicWidth / PhysicalWidth;
            int NumGlyphOfPart = NumGlyphInLine * (PicHeight / PhysicalHeight);

            var l = new List<GlyphDescriptor>();
            int Count = NumericOperations.Min(NumGlyphOfPart, Glyphs.Count());

            for (int GlyphIndex = 0, loopTo = Count - 1; GlyphIndex <= loopTo; GlyphIndex++)
            {
                var g = Glyphs.ElementAtOrDefault(GlyphIndex);
                if (g.PhysicalWidth > PhysicalWidth)
                    throw new InvalidOperationException("PhysicalWidthOverflow:{0}".Formats(g.c.ToString()));
                if (g.PhysicalHeight > PhysicalHeight)
                    throw new InvalidOperationException("PhysicalHeightOverflow:{0}".Formats(g.c.ToString()));
                int x = GlyphIndex % NumGlyphInLine * PhysicalWidth;
                int y = GlyphIndex / NumGlyphInLine * PhysicalHeight;
                l.Add(new GlyphDescriptor() { c = g.c, PhysicalBox = new Rectangle(x, y, g.PhysicalWidth, g.PhysicalHeight), VirtualBox = g.VirtualBox });
            }

            return l;
        }
    }

    public class GlyphArrangerCompact : IGlyphArranger
    {

        private int PhysicalWidth;
        private int PhysicalHeight;

        public GlyphArrangerCompact(int PhysicalWidth, int PhysicalHeight)
        {
            if (PhysicalWidth <= 0)
                throw new ArgumentOutOfRangeException();
            if (PhysicalHeight <= 0)
                throw new ArgumentOutOfRangeException();
            this.PhysicalWidth = PhysicalWidth;
            this.PhysicalHeight = PhysicalHeight;
        }

        public int GetLeastGlyphCount(int PicWidth, int PicHeight)
        {
            int NumGlyphInLine = PicWidth / PhysicalWidth;
            int NumGlyphOfPart = NumGlyphInLine * (PicHeight / PhysicalHeight);
            return NumGlyphOfPart;
        }

        public Size GetPreferredSize(IEnumerable<IGlyph> Glyphs)
        {
            int Count = Glyphs.Count();
            int k = (int)Round(Ceiling(Log(Sqrt(Count * PhysicalWidth * PhysicalHeight), 2d)));
            while (true)
            {
                double PicSize = Pow(2d, k);

                long NumGlyphInLine = (long)Round(PicSize) / PhysicalWidth;
                long NumGlyphOfPart = NumGlyphInLine * ((long)Round(PicSize) / PhysicalHeight);

                if (Count <= NumGlyphOfPart)
                    return new Size((int)Round(PicSize), (int)Round(PicSize));
                k += 1;
            }
        }

        public int GetPreferredHeight(IEnumerable<IGlyph> Glyphs, int PicWidth)
        {
            int Count = Glyphs.Count();
            int k = (int)Round(Ceiling(Log(Count * PhysicalWidth * PhysicalHeight / (double)PicWidth)));
            int NumGlyphInLine = PicWidth / PhysicalWidth;

            while (true)
            {
                double PicHeight = Pow(2d, k);

                long NumGlyphOfPart = NumGlyphInLine * ((long)Round(PicHeight) / PhysicalHeight);

                if (Count <= NumGlyphOfPart)
                    return (int)Round(PicHeight);
                k += 1;
            }
        }

        public IEnumerable<GlyphDescriptor> GetGlyphArrangement(IEnumerable<IGlyph> Glyphs, int PicWidth, int PicHeight)
        {
            var l = new List<GlyphDescriptor>();

            int x = 0;
            int y = 0;
            int h = 0;
            var lLine = new List<GlyphDescriptor>();
            for (int GlyphIndex = 0, loopTo = Glyphs.Count() - 1; GlyphIndex <= loopTo; GlyphIndex++)
            {
                var g = Glyphs.ElementAtOrDefault(GlyphIndex);
                if (g.PhysicalWidth > PhysicalWidth)
                    throw new InvalidOperationException("PhysicalWidthOverflow:{0}".Formats(g.c.ToString()));
                if (g.PhysicalHeight > PhysicalHeight)
                    throw new InvalidOperationException("PhysicalHeightOverflow:{0}".Formats(g.c.ToString()));
                if (x + g.PhysicalWidth > PicWidth)
                {
                    x = 0;
                    if (y + h > PicHeight)
                    {
                        break;
                    }
                    else
                    {
                        y += h;
                        l.AddRange(lLine);
                        lLine.Clear();
                        h = 0;
                    }
                }
                lLine.Add(new GlyphDescriptor() { c = g.c, PhysicalBox = new Rectangle(x, y, g.PhysicalWidth, g.PhysicalHeight), VirtualBox = g.VirtualBox });
                x += g.PhysicalWidth;
                h = NumericOperations.Max(h, g.PhysicalHeight);
            }
            if (lLine.Count > 0)
            {
                l.AddRange(lLine);
                lLine.Clear();
            }

            return l;
        }
    }
}