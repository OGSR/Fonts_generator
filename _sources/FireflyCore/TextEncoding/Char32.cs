// ==========================================================================
// 
// File:        Char32.vb
// Location:    Firefly.TextEncoding <Visual Basic .Net>
// Description: UTF-32 字符
// Version:     2009.11.07.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace Firefly.TextEncoding
{

    /// <summary>UTF-32字符。</summary>
    [DebuggerDisplay("{ToDisplayString()}")]
    public struct Char32 : IEquatable<Char32>, IComparable<Char32>
    {

        private int Unicode;
        public Char32(int Unicode)
        {
            this.Unicode = Unicode;
        }

        /// <summary>UTF-32值。</summary>
        public int Value
        {
            get
            {
                return Unicode;
            }
        }

        /// <summary>生成显示用字符串。</summary>
        public string ToDisplayString()
        {
            var List = new List<string>();
            List.Add(string.Format("U+{0:X4}", Unicode));
            if (!IsControlChar)
                List.Add(string.Format("\"{0}\"", ToString()));

            return "Char32{" + string.Join(", ", List.ToArray()) + "}";
        }

        /// <summary>已重载。将UTF-16 Big-Endian转换成Unicode(UTF-32)。</summary>
        public override string ToString()
        {
            return ToString(this);
        }


        /// <summary>指示是否是控制符。</summary>
        private bool IsControlChar
        {
            get
            {
                return Unicode >= 0 && Unicode <= 0x1F;
            }
        }

        /// <summary>已重载。将Unicode(UTF-32)转换成UTF-16 Big-Endian。</summary>
        public static string ToString(Char32 c)
        {
            if (c.Unicode >= 0 && c.Unicode < 0x10000)
            {
                return Conversions.ToString(String16.ChrW(c.Unicode));
            }
            else if (c.Unicode >= 0x10000 && c.Unicode < 0x10FFFF)
            {
                var S0 = default(int);
                var S1 = default(int);
                BitOperations.SplitBits(ref S1, 10, ref S0, 10, c.Unicode - 0x10000);
                int L = BitOperations.ConcatBits(0x37, 6, S0, 10); // 110111
                int H = BitOperations.ConcatBits(0x36, 6, S1, 10); // 110110
                return Conversions.ToString(String16.ChrW(H)) + String16.ChrW(L);
            }
            else
            {
                throw new InvalidDataException();
            }
        }

        /// <summary>将UTF-16 Big-Endian转换成Unicode(UTF-32)。</summary>
        public static Char32 FromString(string UTF16B)
        {
            if (string.IsNullOrEmpty(UTF16B))
                return new Char32(0);
            int H = String16.AscW(UTF16B[0]);
            if (H >= 0xD800 && H <= 0xDBFF)
            {
                if (UTF16B.Length != 2)
                    throw new InvalidDataException();
                int L = String16.AscW(UTF16B[1]);
                if (L < 0xDC00 || L > 0xDFFF)
                    throw new InvalidDataException();
                return new Char32(BitOperations.ConcatBits(H.Bits(9, 0), 10, L.Bits(9, 0), 10) + 0x10000);
            }
            else
            {
                if (UTF16B.Length != 1)
                    throw new InvalidDataException();
                return new Char32(H);
            }
        }

        /// <summary>转换UTF-32字符到32位整数。</summary>
        public static implicit operator int(Char32 c)
        {
            return c.Unicode;
        }

        /// <summary>转换32位整数到UTF-32字符。</summary>
        public static implicit operator Char32(int c)
        {
            return new Char32(c);
        }

        /// <summary>转换UTF-16 Big-Endian字符到UTF-32字符。</summary>
        public static implicit operator Char32(char c)
        {
            return FromString(Conversions.ToString(c));
        }

        /// <summary>转换Uncode(UTF-32)字符到转换UTF-16 Big-Endian字符。</summary>
        public static explicit operator char(Char32 c)
        {
            string l = c.ToString();
            if (l.Length > 1)
                throw new ArgumentOutOfRangeException();
            return l[0];
        }

        /// <summary>转换UTF-16 Big-Endian字符串到UTF-32字符。</summary>
        public static explicit operator Char32(string c)
        {
            return FromString(c);
        }

        /// <summary>转换UTF-32字符到转换UTF-16 Big-Endian字符串。</summary>
        public static implicit operator string(Char32 c)
        {
            return c.ToString();
        }

        /// <summary>比较两个字符是否相等。</summary>
        public bool Equals(Char32 other)
        {
            return Unicode == other.Unicode;
        }

        /// <summary>比较两个字符是否相等。</summary>
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is Char32))
                return false;
            return Equals((Char32)obj);
        }

        /// <summary>获取字符的HashCode。</summary>
        public override int GetHashCode()
        {
            return Unicode;
        }

        /// <summary>比较两个字符的大小。</summary>
        public int CompareTo(Char32 other)
        {
            return Unicode.CompareTo(other.Unicode);
        }

        public static bool operator ==(Char32 l, Char32 r)
        {
            return l.Equals(r);
        }

        public static bool operator !=(Char32 l, Char32 r)
        {
            return !l.Equals(r);
        }

        public static bool operator <(Char32 l, Char32 r)
        {
            return l.CompareTo(r) < 0;
        }

        public static bool operator <=(Char32 l, Char32 r)
        {
            return l.CompareTo(r) <= 0;
        }

        public static bool operator >(Char32 l, Char32 r)
        {
            return l.CompareTo(r) > 0;
        }

        public static bool operator >=(Char32 l, Char32 r)
        {
            return l.CompareTo(r) >= 0;
        }

        public static string operator +(Char32 l, Char32 r)
        {
            return l + r;
        }

        public static string operator +(Char32 l, string r)
        {
            return l + r;
        }

        public static string operator +(string l, Char32 r)
        {
            return l + r;
        }

        public static string operator +(Char32 l, Char32[] r)
        {
            return l + r.ToUTF16B();
        }

        public static string operator +(Char32[] l, Char32 r)
        {
            return l.ToUTF16B() + r;
        }
    }

    /// <summary>UTF-32字符串，即Char32()。</summary>
    public static class String32
    {
        /// <summary>UTF-32数值转到UTF-32字符。</summary>
        public static Char32 ChrQ(int u)
        {
            return (Char32)u;
        }

        /// <summary>UTF-32字符转到UTF-32数值。</summary>
        public static int AscQ(Char32 c)
        {
            return (int)c;
        }

        /// <summary>转换UTF-16 Big-Endian字符串到Uncode(UTF-32)字符串。</summary>
        public static Char32[] FromUTF16B(string s)
        {
            var cl = new List<Char32>();

            for (int n = 0, loopTo = s.Length - 1; n <= loopTo; n++)
            {
                char c = s[n];
                int H = String16.AscW(c);
                if (H >= 0xD800 && H <= 0xDBFF)
                {
                    cl.Add((Char32)(Conversions.ToString(c) + s[n + 1]));
                    n += 1;
                }
                else
                {
                    cl.Add((Char32)c);
                }
            }

            return cl.ToArray();
        }

        /// <summary>转换Uncode(UTF-32)字符串到UTF-16 Big-Endian字符串。</summary>
        public static string ToUTF16B(this Char32[] s)
        {
            var sb = new StringBuilder();

            foreach (var c in s)
                sb.Append(c.ToString());

            return sb.ToString();
        }
    }

    /// <summary>UTF-16字符串，即String。</summary>
    public static class String16
    {
        /// <summary>UTF-16数值转到UTF-16字符。</summary>
        public static char ChrW(short u)
        {
            return Convert.ToChar(DirectIntConvert.CSU(u));
        }

        /// <summary>UTF-16数值转到UTF-16字符。</summary>
        public static char ChrW(ushort u)
        {
            return Convert.ToChar(u);
        }

        /// <summary>UTF-32数值转到UTF-16字符。</summary>
        public static char ChrW(int u)
        {
            return Convert.ToChar(u);
        }

        /// <summary>UTF-16字符转到UTF-16数值。</summary>
        public static ushort AscW(char c)
        {
            return Convert.ToUInt16(c);
        }

        /// <summary>转换UTF-16 Big-Endian字符串到Uncode(UTF-32)字符串。</summary>
        public static Char32[] ToUTF32(this string s)
        {
            return String32.FromUTF16B(s);
        }

        /// <summary>转换Uncode(UTF-32)字符串到UTF-16 Big-Endian字符串。</summary>
        public static string FromUTF32(Char32[] s)
        {
            return s.ToUTF16B();
        }

        /// <summary>统一换行符为回车换行。</summary>
        public static string UnifyNewLineToCrLf(this string s)
        {
            return s.Replace(ControlChars.CrLf, ControlChars.Lf).Replace(ControlChars.Cr, ControlChars.Lf).Replace(ControlChars.Lf, ControlChars.CrLf);
        }

        /// <summary>统一换行符为换行。</summary>
        public static string UnifyNewLineToLf(this string s)
        {
            return s.Replace(ControlChars.CrLf, ControlChars.Lf).Replace(ControlChars.Cr, ControlChars.Lf);
        }

        /// <summary>从当前 String 对象移除数组中指定的一个字符的所有前导匹配项。</summary>
        public static string TrimStart(this string s, Char32 c)
        {
            Char32[] s32 = s.ToUTF32();
            for (int n = s32.Length - 1; n >= 0; n -= 1)
            {
                if (s32[n] != c)
                {
                    return s32.SubArray(0, n + 1).ToUTF16B();
                }
            }
            return "";
        }

        /// <summary>从当前 String 对象移除数组中指定的一个字符的所有尾部匹配项。</summary>
        public static string TrimEnd(this string s, Char32 c)
        {
            Char32[] s32 = s.ToUTF32();
            for (int n = 0, loopTo = s32.Length - 1; n <= loopTo; n++)
            {
                if (s32[n] != c)
                {
                    return s32.SubArray(n).ToUTF16B();
                }
            }
            return "";
        }

        /// <summary>从当前 String 对象移除一个指定字符的所有前导匹配项和尾部匹配项。</summary>
        public static string Trim(this string s, Char32 c)
        {
            return s.TrimStart(c).TrimEnd(c);
        }
    }
}