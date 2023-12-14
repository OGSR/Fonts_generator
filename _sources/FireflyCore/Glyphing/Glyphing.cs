// ==========================================================================
// 
// File:        Glyphing.vb
// Location:    Firefly.Glyphing <Visual Basic .Net>
// Description: 图形绘制相关函数
// Version:     2009.10.31.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System.Drawing;

namespace Firefly.Glyphing
{
    public static class Glyphing
    {
        /// <summary>测定字符串显示的宽度。</summary>
        public static float MeasureStringWidth(this Graphics g, string Text, Font f)
        {
            if (Text.Length == 0)
                return 0f;
            var format = new StringFormat(StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox);
            var rect = new RectangleF(0f, 0f, 0f, 0f);
            CharacterRange[] ranges = new[] { new CharacterRange(0, Text.Length) };
            format.SetMeasurableCharacterRanges(ranges);
            Region[] regions = g.MeasureCharacterRanges(Text, f, rect, format);
            rect = regions[0].GetBounds(g);
            return rect.Width;
        }
        /// <summary>测定字符串显示的矩形。</summary>
        public static RectangleF MeasureStringRectangle(this Graphics g, string Text, Font f)
        {
            if (Text.Length == 0)
                return new RectangleF(0f, 0f, 0f, 0f);
            var format = new StringFormat(StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox);
            var rect = new RectangleF(0f, 0f, 0f, 0f);
            CharacterRange[] ranges = new[] { new CharacterRange(0, Text.Length) };
            format.SetMeasurableCharacterRanges(ranges);
            Region[] regions = g.MeasureCharacterRanges(Text, f, rect, format);
            rect = regions[0].GetBounds(g);
            return rect;
        }
    }
}