// ==========================================================================
// 
// File:        StringDescape.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 字符串转义语法糖
// Version:     2009.10.20.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Firefly.TextEncoding;
using Microsoft.VisualBasic.CompilerServices;

namespace Firefly
{

    /// <summary>
/// 字符串转义
/// 用于使用 "转义字符串".Descape 和 "格式".Formats(...) 语法
/// </summary>
    public static class StringDescape
    {

        /// <summary>字符串反转义函数</summary>
    /// <remarks>
    /// \0 与null \u0000 匹配
    /// \a 与响铃（警报）\u0007 匹配 
    /// \b 与退格符 \u0008 匹配
    /// \f 与换页符 \u000C 匹配
    /// \n 与换行符 \u000A 匹配
    /// \r 与回车符 \u000D 匹配
    /// \t 与 Tab 符 \u0009 匹配 
    /// \v 与垂直 Tab 符 \u000B 匹配
    /// \x?? 与 \u00?? 匹配
    /// \u???? 与对应的Unicode字符对应
    /// </remarks>
        public static string Descape(this string This)
        {
            var m = r.Match(This);
            if (!m.Success)
                throw new InvalidCastException();

            var ss = new SortedList<int, string>();
            foreach (Capture c in m.Groups["SingleEscape"].Captures)
                ss.Add(c.Index, SingleEscapeDict[c.Value]);
            foreach (Capture c in m.Groups["UnicodeEscape"].Captures)
                ss.Add(c.Index, String32.ChrQ(int.Parse(c.Value, System.Globalization.NumberStyles.HexNumber)));
            foreach (Capture c in m.Groups["ErrorEscape"].Captures)
                throw new ArgumentException("ErrorEscape: Ch " + (c.Index + 1) + " " + c.Value);
            foreach (Capture c in m.Groups["Normal"].Captures)
                ss.Add(c.Index, c.Value);

            var sb = new StringBuilder();
            foreach (var s in ss.Values)
                sb.Append(s);

            return sb.ToString();
        }

        /// <summary>字符串转义函数</summary>
    /// <remarks>
    /// \0 与null \u0000 匹配
    /// \a 与响铃（警报）\u0007 匹配 
    /// \b 与退格符 \u0008 匹配
    /// \f 与换页符 \u000C 匹配
    /// \n 与换行符 \u000A 匹配
    /// \r 与回车符 \u000D 匹配
    /// \t 与 Tab 符 \u0009 匹配 
    /// \v 与垂直 Tab 符 \u000B 匹配
    /// \u???? 与对应的Unicode字符对应(只转义\u0000-\u001F和\u007F中除上述字符的字符)
    /// </remarks>
        public static string Escape(this string This)
        {
            var l = new List<Char32>();
            foreach (var c in This.ToUTF32())
            {
                switch (c.Value)
                {
                    case 0x0:
                        {
                            l.AddRange(@"\0".ToUTF32());
                            break;
                        }
                    case 0x7:
                        {
                            l.AddRange(@"\a".ToUTF32());
                            break;
                        }
                    case 0x8:
                        {
                            l.AddRange(@"\b".ToUTF32());
                            break;
                        }
                    case 0xC:
                        {
                            l.AddRange(@"\f".ToUTF32());
                            break;
                        }
                    case 0xA:
                        {
                            l.AddRange(@"\n".ToUTF32());
                            break;
                        }
                    case 0xD:
                        {
                            l.AddRange(@"\r".ToUTF32());
                            break;
                        }
                    case 0x9:
                        {
                            l.AddRange(@"\t".ToUTF32());
                            break;
                        }
                    case 0xB:
                        {
                            l.AddRange(@"\v".ToUTF32());
                            break;
                        }
                    case var @case when 0x0 <= @case && @case <= 0x1F:
                    case 0x7F:
                        {
                            l.AddRange(@"\u{0:X4}".Formats(c.Value).ToUTF32());
                            break;
                        }

                    default:
                        {
                            l.Add(c);
                            break;
                        }
                }
            }
            return l.ToArray().ToUTF16B();
        }

        /// <summary>将指定的 String 中的格式项替换为指定的 Object 实例的值的文本等效项。</summary>
        public static string Formats(this string This, object arg0)
        {
            return string.Format(This, arg0);
        }
        /// <summary>将指定的 String 中的格式项替换为两个指定的 Object 实例的值的文本等效项。</summary>
        public static string Formats(this string This, object arg0, object arg1)
        {
            return string.Format(This, arg0, arg1);
        }
        /// <summary>将指定的 String 中的格式项替换为三个指定的 Object 实例的值的文本等效项。</summary>
        public static string Formats(this string This, object arg0, object arg1, object arg2)
        {
            return string.Format(This, arg0, arg1, arg2);
        }
        /// <summary>将指定 String 中的格式项替换为指定数组中相应 Object 实例的值的文本等效项。</summary>
        public static string Formats(this string This, params object[] args)
        {
            return string.Format(This, args);
        }
        /// <summary>将指定 String 中的格式项替换为指定数组中相应 Object 实例的值的文本等效项。指定的参数提供区域性特定的格式设置信息。</summary>
        public static string Formats(this string This, IFormatProvider provider, params object[] args)
        {
            return string.Format(provider, This, args);
        }

        private static Dictionary<string, string> _SingleEscapeDict_d = default;


        private static Dictionary<string, string> SingleEscapeDict
        {
            get
            {
                if (_SingleEscapeDict_d is not null)
                    return _SingleEscapeDict_d;
                _SingleEscapeDict_d = new Dictionary<string, string>();
                _SingleEscapeDict_d.Add(@"\", @"\"); // backslash
                _SingleEscapeDict_d.Add("0", String32.ChrQ(0)); // null
                _SingleEscapeDict_d.Add("a", String32.ChrQ(7)); // alert (beep)
                _SingleEscapeDict_d.Add("b", String32.ChrQ(8)); // backspace
                _SingleEscapeDict_d.Add("f", String32.ChrQ(0xC)); // form feed
                _SingleEscapeDict_d.Add("n", String32.ChrQ(0xA)); // newline (lf)
                _SingleEscapeDict_d.Add("r", String32.ChrQ(0xD)); // carriage return (cr) 
                _SingleEscapeDict_d.Add("t", String32.ChrQ(9)); // horizontal tab 
                _SingleEscapeDict_d.Add("v", String32.ChrQ(0xB)); // vertical tab
                return _SingleEscapeDict_d;
            }
        }

        private static string _SingleEscapes_s = default;
        private static string SingleEscapes
        {
            get
            {
                if (_SingleEscapes_s is not null)
                    return _SingleEscapes_s;
                var Chars = new List<string>();
                foreach (var c in @"\0abfnrtv")
                    Chars.Add(Regex.Escape(Conversions.ToString(c)));
                _SingleEscapes_s = @"\\(?<SingleEscape>" + string.Join("|", Chars.ToArray()) + ")";
                return _SingleEscapes_s;
            }
        }
        private static string UnicodeEscapes = @"\\[uU](?<UnicodeEscape>[0-9A-Fa-f]{4})|\\x(?<UnicodeEscape>[0-9A-Fa-f]{2})";
        private static string ErrorEscapes = @"(?<ErrorEscape>\\)";
        private static string Normal = "(?<Normal>.)";

        private static Regex r = new Regex("^" + "(" + SingleEscapes + "|" + UnicodeEscapes + "|" + ErrorEscapes + "|" + Normal + ")*" + "$", RegexOptions.ExplicitCapture);
    }
}