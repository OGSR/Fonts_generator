// ==========================================================================
// 
// File:        ByteTextSearch.vb
// Location:    Firefly.Texting <Visual Basic .Net>
// Description: 基于字节的正则表达式搜索
// Version:     2009.10.31.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Firefly.TextEncoding;
using Microsoft.VisualBasic.CompilerServices;

namespace Firefly.Texting
{
    public sealed class ByteTextSearch
    {
        private ByteTextSearch()
        {
        }

        public static string EncodeAsByteString(byte[] Input)
        {
            Char32[] sb = new Char32[Input.Length];
            for (int n = 0, loopTo = Input.Length - 1; n <= loopTo; n++)
                sb[n] = String32.ChrQ(Input[n]);
            return sb.ToUTF16B();
        }

        public static byte[] DecodeFromByteString(string Input)
        {
            byte[] bb = new byte[Input.Length];
            for (int n = 0, loopTo = Input.Length - 1; n <= loopTo; n++)
                bb[n] = (byte)String32.AscQ(Input[n]);
            return bb;
        }

        public static string EncodingRangeToRegexPattern(Range[] ByteRanges)
        {
            var l = new List<string>();
            foreach (var r in ByteRanges)
                l.Add(string.Format(@"\x{0:X2}-\x{1:X2}", r.Lower, r.Upper));
            return "[" + string.Join("", l.ToArray()) + "]";
        }
        public static string EncodingRangeToRegexPattern(Range[] FirstByteRanges, Range[] SecondByteRanges)
        {
            return EncodingRangeToRegexPattern(FirstByteRanges) + EncodingRangeToRegexPattern(SecondByteRanges);
        }
        public static string EncodingRangeToRegexPattern(Range[] FirstByteRanges, Range[] SecondByteRanges, Range[] ThirdByteRanges)
        {
            return EncodingRangeToRegexPattern(FirstByteRanges) + EncodingRangeToRegexPattern(SecondByteRanges) + EncodingRangeToRegexPattern(ThirdByteRanges);
        }
        public static string EncodingRangeToRegexPattern(Range[] FirstByteRanges, Range[] SecondByteRanges, Range[] ThirdByteRanges, Range[] ForthByteRanges)
        {
            return EncodingRangeToRegexPattern(FirstByteRanges) + EncodingRangeToRegexPattern(SecondByteRanges) + EncodingRangeToRegexPattern(ThirdByteRanges) + EncodingRangeToRegexPattern(ForthByteRanges);
        }

        public static WQSG.Triple[] MatchAll(Encoding Encoding, byte[] Input, string Pattern, RegexOptions Options = RegexOptions.ExplicitCapture)
        {
            var t = new List<WQSG.Triple>();
            string InputStr = EncodeAsByteString(Input);
            var Matches = Regex.Matches(InputStr, Pattern, Options);
            foreach (Match m in Matches)
            {
                char[] Text = Encoding.GetChars(DecodeFromByteString(m.Value));
                t.Add(new WQSG.Triple() { Offset = m.Index, Length = m.Length, Text = Conversions.ToString(Text) });
            }
            return t.ToArray();
        }
    }
}