// ==========================================================================
// 
// File:        Char32.vb
// Location:    Firefly.TextEncoding <Visual Basic .Net>
// Description: UTF-32 字符
// Version:     2009.03.29.
// Copyright(C) F.R.C.
// 
// ==========================================================================


namespace Firefly.TextEncoding
{
    public static class ControlChars
    {
        /// <summary>回车符。</summary>
        public readonly static Char32 Cr = 0xD;
        /// <summary>换行符。</summary>
        public readonly static Char32 Lf = 0xA;
        /// <summary>回车换行符。</summary>
        public readonly static string CrLf = (new Char32[] { 0xD, 0xA }).ToUTF16B();
        /// <summary>空字符。</summary>
        public readonly static Char32 Nul = 0x0;
        /// <summary>双引号。</summary>
        public readonly static Char32 Quote = 0x22;
    }
}