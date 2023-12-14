// ==========================================================================
// 
// File:        StringCode.vb
// Location:    Firefly.TextEncoding <Visual Basic .Net>
// Description: 字符串码点信息
// Version:     2009.11.26.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace Firefly.TextEncoding
{

    /// <summary>字符串码点值对，可用于码点转换。</summary>
    [DebuggerDisplay("{ToString()}")]
    public class StringCode : IEquatable<StringCode>
    {

        /// <summary>Unicode字符串。</summary>
        public string Unicode = "";
        /// <summary>码点形式的自定义码点。</summary>
        public int Code = -1;
        /// <summary>自定义码点的字节长度。</summary>
        public int CodeLength = -1;

        /// <summary>已重载。创建字符码点值对的实例。</summary>
        public StringCode()
        {
            Unicode = "";
            Code = -1;
            CodeLength = 0;
        }
        /// <summary>已重载。创建字符码点值对的实例。</summary>
        /// <param name="UniStr">Unicode字符串，空字符串表示不存在。</param>
        /// <param name="Code">自定义码点，-1表示不存在。</param>
        /// <param name="CodeLength">自定义码点的字节长度，只能为-1、0、1、2、3、4。其中-1表示不明确，0表示码点不存在。</param>
        /// <remarks></remarks>
        public StringCode(string UniStr, int Code, int CodeLength = -1)
        {
            Unicode = UniStr;
            this.Code = Code;
            if (CodeLength < -1 || CodeLength > 4)
                throw new ArgumentException();
            this.CodeLength = CodeLength;
        }

        /// <summary>已重载。创建字符码点值对的实例。</summary>
        /// <param name="UniChar">Unicode字符，-1表示不存在。</param>
        /// <param name="Code">自定义码点，-1表示不存在。</param>
        /// <param name="CodeLength">自定义码点的字节长度，只能为-1、0、1、2、3、4。其中-1表示不明确，0表示码点不存在。</param>
        /// <remarks></remarks>
        public StringCode(Char32 UniChar, int Code, int CodeLength = -1)
        {
            if (UniChar == -1)
            {
                Unicode = "";
            }
            else
            {
                Unicode = UniChar;
            }
            this.Code = Code;
            if (CodeLength < -1 || CodeLength > 4)
                throw new ArgumentException();
            this.CodeLength = CodeLength;
        }

        /// <summary>创建字符码点值对的实例。</summary>
        public static StringCode FromNothing()
        {
            return new StringCode("", -1, 0);
        }

        /// <summary>创建字符码点值对的实例。</summary>
        /// <param name="UniStr">Unicode字符串。</param>
        public static StringCode FromUniStr(string UniStr)
        {
            return new StringCode(UniStr, -1, 0);
        }

        /// <summary>创建字符码点值对的实例。</summary>
        /// <param name="UniChar">Unicode字符。</param>
        public static StringCode FromUniChar(Char32 UniChar)
        {
            return new StringCode(UniChar, -1, 0);
        }

        /// <summary>创建字符码点值对的实例。</summary>
        /// <param name="Unicode">Unicode码。</param>
        public static StringCode FromUnicode(int Unicode)
        {
            return new StringCode(new Char32(Unicode), -1, 0);
        }

        /// <summary>创建字符码点值对的实例。</summary>
        /// <param name="Code">自定义码点，-1表示不存在。</param>
        /// <param name="CodeLength">自定义码点的字节长度，只能为-1、0、1、2、3、4。其中-1表示不明确，0表示码点不存在。</param>
        public static StringCode FromCode(int Code, int CodeLength = -1)
        {
            return new StringCode("", Code, CodeLength);
        }

        /// <summary>创建字符码点值对的实例。</summary>
        /// <param name="CodeString">自定义码点的字符串形式，""表示不存在。</param>
        public static StringCode FromCodeString(string CodeString)
        {
            if (string.IsNullOrEmpty(CodeString))
            {
                return new StringCode("", -1, 0);
            }
            else
            {
                return new StringCode("", int.Parse(CodeString, System.Globalization.NumberStyles.HexNumber), (CodeString.Length + 1) / 2);
            }
        }

        /// <summary>创建字符码点值对的实例。</summary>
        /// <param name="CharCode">CharCode字符串。</param>
        public static StringCode FromCharCode(CharCode CharCode)
        {
            return new StringCode(CharCode.Unicode, CharCode.Code, CharCode.CodeLength);
        }

        /// <summary>字符。</summary>
        public string Character
        {
            get
            {
                if (HasUnicode)
                    return Unicode;
                throw new InvalidOperationException();
            }
            set
            {
                Unicode = value;
            }
        }

        /// <summary>指示是否是控制符。</summary>
        public virtual bool IsControlChar
        {
            get
            {
                if (HasUnicode)
                {
                    Char32[] Str32 = Unicode.ToUTF32();
                    if (Str32.Length == 1)
                    {
                        return Str32[0] >= 0 && Str32[0] <= 0x1F;
                    }
                }
                return false;
            }
        }

        /// <summary>指示是否是换行符。</summary>
        public virtual bool IsNewLine
        {
            get
            {
                return Unicode == ControlChars.Lf;
            }
        }

        /// <summary>指示是否已建立映射。</summary>
        public bool IsCodeMappable
        {
            get
            {
                return HasUnicode && HasCode;
            }
        }

        /// <summary>指示Unicode是否存在。</summary>
        public bool HasUnicode
        {
            get
            {
                return !string.IsNullOrEmpty(Unicode);
            }
        }

        /// <summary>指示自定义码点是否存在。</summary>
        public bool HasCode
        {
            get
            {
                return CodeLength != 0;
            }
        }

        /// <summary>生成显示用字符串。</summary>
        public override string ToString()
        {
            var List = new List<string>();
            if (HasUnicode)
            {
                List.Add(string.Join(" ", (from c in Unicode.ToUTF32()
                                           select string.Format("U+{0:X4}", c.Value)).ToArray()));
                if (!IsControlChar)
                    List.Add(string.Format("\"{0}\"", Unicode));
            }
            if (HasCode)
                List.Add(string.Format("Code = {0}", CodeString));

            return "StringCode{" + string.Join(", ", List.ToArray()) + "}";
        }

        /// <summary>自定义码点的字符串形式。</summary>
        public string CodeString
        {
            get
            {
                switch (CodeLength)
                {
                    case var @case when @case > 0:
                        {
                            return Code.ToString("X" + CodeLength * 2);
                        }
                    case 0:
                        {
                            return "";
                        }

                    default:
                        {
                            switch (Code)
                            {
                                case var case1 when 0 <= case1 && case1 <= 0xFF:
                                    {
                                        return Code.ToString("X2");
                                    }
                                case var case2 when 0x100 <= case2 && case2 <= 0xFFFF:
                                    {
                                        return Code.ToString("X4");
                                    }
                                case var case3 when 0x10000 <= case3 && case3 <= 0xFFFFFF:
                                    {
                                        return Code.ToString("X6");
                                    }

                                default:
                                    {
                                        return Code.ToString("X8");
                                    }
                            }

                            break;
                        }
                }
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Code = -1;
                    CodeLength = 0;
                }
                else
                {
                    Code = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
                    CodeLength = (value.Length + 1) / 2;
                }
            }
        }

        /// <summary>比较两个字符码点是否相等。</summary>
        public bool Equals(StringCode other)
        {
            return (Unicode ?? "") == (other.Unicode ?? "") && Code == other.Code;
        }

        /// <summary>比较两个字符码点是否相等。</summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj is null)
                return false;
            StringCode c = obj as StringCode;
            if (c is null)
                return false;
            return Equals(c);
        }

        /// <summary>获取字符码点的HashCode。</summary>
        public override int GetHashCode()
        {
            return Unicode.GetHashCode() ^ (Code << 16 | Code >> 16 & 0xFFFF);
        }

        /// <summary>转换CharCode到StringCode。</summary>
        public static implicit operator StringCode(CharCode c)
        {
            return new StringCode(c.Unicode, c.Code, c.CodeLength);
        }
    }

    /// <summary>字符码点值对字符串。</summary>
    public static class StringCodeString
    {
        /// <summary>转换CharCode()到StringCode()。</summary>
        public static StringCode[] FromCharCodeString(CharCode[] s)
        {
            StringCode[] StringCodes = new StringCode[s.Length];
            for (int n = 0, loopTo = s.Length - 1; n <= loopTo; n++)
                StringCodes[n] = StringCode.FromCharCode(s[n]);
            return StringCodes;
        }

        /// <summary>转换UTF-32字符串到StringCode()。</summary>
        public static StringCode[] FromString32(Char32[] s)
        {
            StringCode[] StringCodes = new StringCode[s.Length];
            for (int n = 0, loopTo = s.Length - 1; n <= loopTo; n++)
                StringCodes[n] = StringCode.FromUniChar(s[n]);
            return StringCodes;
        }

        /// <summary>转换UTF-16 Big-Endian字符串到UTF-32字符串。</summary>
        public static StringCode[] FromString16(string s)
        {
            var cl = new List<StringCode>();

            for (int n = 0, loopTo = s.Length - 1; n <= loopTo; n++)
            {
                char c = s[n];
                int H = String32.AscQ(c);
                if (H >= 0xD800 && H <= 0xDBFF)
                {
                    cl.Add(StringCode.FromUniChar(Char32.FromString(Conversions.ToString(c) + s[n + 1])));
                    n += 1;
                }
                else
                {
                    cl.Add(StringCode.FromUniChar(c));
                }
            }

            return cl.ToArray();
        }

        /// <summary>转换UTF-32字符串到UTF-16 Big-Endian字符串。</summary>
        public static string ToString16(this StringCode[] s)
        {
            var sb = new StringBuilder();

            foreach (var c in s)
            {
                if (!c.HasUnicode)
                    throw new InvalidDataException();
                sb.Append(c.Unicode.ToString());
            }

            return sb.ToString();
        }
    }
}