// ==========================================================================
// 
// File:        Encoding.vb
// Location:    Firefly.TextEncoding <Visual Basic .Net>
// Description: 编码
// Version:     2009.12.22.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace Firefly.TextEncoding
{
    public class EncodingNoPreambleWrapper : Encoding
    {

        private Encoding BaseEncoding;
        public EncodingNoPreambleWrapper(Encoding BaseEncoding)
        {
            this.BaseEncoding = BaseEncoding;
        }

        public override Decoder GetDecoder()
        {
            return BaseEncoding.GetDecoder();
        }
        public override Encoder GetEncoder()
        {
            return BaseEncoding.GetEncoder();
        }
        public override byte[] GetPreamble()
        {
            return new byte[] { };
        }

        public override int GetByteCount(char[] chars, int index, int count)
        {
            return BaseEncoding.GetByteCount(chars, index, count);
        }
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            return BaseEncoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
        }
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return BaseEncoding.GetCharCount(bytes, index, count);
        }
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            return BaseEncoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        }
        public override int GetMaxByteCount(int charCount)
        {
            return BaseEncoding.GetMaxByteCount(charCount);
        }
        public override int GetMaxCharCount(int byteCount)
        {
            return BaseEncoding.GetMaxCharCount(byteCount);
        }

        public override object Clone()
        {
            return base.Clone();
        }
        public override bool Equals(object value)
        {
            EncodingNoPreambleWrapper e = value as EncodingNoPreambleWrapper;
            if (e is not null)
                return BaseEncoding.Equals(e.BaseEncoding);
            return BaseEncoding.Equals(value);
        }
        public override int GetHashCode()
        {
            return BaseEncoding.GetHashCode();
        }
        public override string ToString()
        {
            return BaseEncoding.ToString();
        }

        public override string BodyName
        {
            get
            {
                return BaseEncoding.BodyName;
            }
        }
        public override int CodePage
        {
            get
            {
                return BaseEncoding.CodePage;
            }
        }
        public override string EncodingName
        {
            get
            {
                return BaseEncoding.EncodingName;
            }
        }
        public override int GetByteCount(char[] chars)
        {
            return BaseEncoding.GetByteCount(chars);
        }
        public override int GetByteCount(string s)
        {
            return BaseEncoding.GetByteCount(s);
        }
        public override byte[] GetBytes(char[] chars)
        {
            return BaseEncoding.GetBytes(chars);
        }
        public override byte[] GetBytes(char[] chars, int index, int count)
        {
            return BaseEncoding.GetBytes(chars, index, count);
        }
        public override byte[] GetBytes(string s)
        {
            return BaseEncoding.GetBytes(s);
        }
        public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            return BaseEncoding.GetBytes(s, charIndex, charCount, bytes, byteIndex);
        }
        public override int GetCharCount(byte[] bytes)
        {
            return BaseEncoding.GetCharCount(bytes);
        }
        public override char[] GetChars(byte[] bytes)
        {
            return BaseEncoding.GetChars(bytes);
        }
        public override char[] GetChars(byte[] bytes, int index, int count)
        {
            return BaseEncoding.GetChars(bytes, index, count);
        }
        public override string GetString(byte[] bytes)
        {
            return BaseEncoding.GetString(bytes);
        }
        public override string GetString(byte[] bytes, int index, int count)
        {
            return BaseEncoding.GetString(bytes, index, count);
        }
        public override string HeaderName
        {
            get
            {
                return BaseEncoding.HeaderName;
            }
        }
        public override bool IsAlwaysNormalized(NormalizationForm form)
        {
            return BaseEncoding.IsAlwaysNormalized(form);
        }
        public override bool IsBrowserDisplay
        {
            get
            {
                return BaseEncoding.IsBrowserDisplay;
            }
        }
        public override bool IsBrowserSave
        {
            get
            {
                return BaseEncoding.IsBrowserSave;
            }
        }
        public override bool IsMailNewsDisplay
        {
            get
            {
                return BaseEncoding.IsMailNewsDisplay;
            }
        }
        public override bool IsMailNewsSave
        {
            get
            {
                return BaseEncoding.IsMailNewsSave;
            }
        }
        public override bool IsSingleByte
        {
            get
            {
                return BaseEncoding.IsSingleByte;
            }
        }
        public override string WebName
        {
            get
            {
                return BaseEncoding.WebName;
            }
        }
        public override int WindowsCodePage
        {
            get
            {
                return BaseEncoding.WindowsCodePage;
            }
        }
    }

    public static class TextEncoding
    {
        private static Encoding DefaultValue;
        public static Encoding Default
        {
            get
            {
                if (DefaultValue is null)
                {
                    DefaultValue = Encoding.Default;
                    if (ReferenceEquals(DefaultValue, GB2312))
                    {
                        try
                        {
                            DefaultValue = GB18030;
                        }
                        catch
                        {
                        }
                    }
                }
                return DefaultValue;
            }
            set
            {
                DefaultValue = value;
            }
        }

        private static Encoding WritingDefaultValue;
        public static Encoding WritingDefault
        {
            get
            {
                if (WritingDefaultValue is null)
                {
                    WritingDefaultValue = UTF16;
                }
                return WritingDefaultValue;
            }
            set
            {
                WritingDefaultValue = value;
            }
        }
        private static Encoding _ASCII_e = null;

        public static Encoding ASCII
        {
            get
            {
                if (_ASCII_e is null)
                    _ASCII_e = Encoding.GetEncoding("ASCII", new EncoderExceptionFallback(), new DecoderExceptionFallback()); // 20127
                return _ASCII_e;
            }
        }
        private static Encoding _UTF8_e = null;

        public static Encoding UTF8
        {
            get
            {
                if (_UTF8_e is null)
                    _UTF8_e = Encoding.GetEncoding("UTF-8"); // 65001
                return _UTF8_e;
            }
        }
        private static Encoding _UTF16_e = null;

        public static Encoding UTF16
        {
            get
            {
                if (_UTF16_e is null)
                    _UTF16_e = Encoding.GetEncoding("UTF-16"); // 1200
                return _UTF16_e;
            }
        }
        private static Encoding _UTF16B_e = null;

        public static Encoding UTF16B
        {
            get
            {
                if (_UTF16B_e is null)
                    _UTF16B_e = Encoding.GetEncoding("UTF-16BE"); // 1201
                return _UTF16B_e;
            }
        }
        private static Encoding _UTF32_e = null;

        public static Encoding UTF32
        {
            get
            {
                if (_UTF32_e is null)
                    _UTF32_e = Encoding.GetEncoding("UTF-32"); // 12000
                return _UTF32_e;
            }
        }
        private static Encoding _UTF32B_e = null;

        public static Encoding UTF32B
        {
            get
            {
                if (_UTF32B_e is null)
                    _UTF32B_e = Encoding.GetEncoding("UTF-32BE"); // 12001
                return _UTF32B_e;
            }
        }
        private static Encoding _GB18030_e = null;

        public static Encoding GB18030
        {
            get
            {
                if (_GB18030_e is null)
                    _GB18030_e = Encoding.GetEncoding("GB18030"); // 54936
                return _GB18030_e;
            }
        }
        private static Encoding _GB2312_e = null;

        public static Encoding GB2312
        {
            get
            {
                if (_GB2312_e is null)
                    _GB2312_e = Encoding.GetEncoding("GB2312"); // 936
                return _GB2312_e;
            }
        }
        private static Encoding _Big5_e = null;

        public static Encoding Big5
        {
            get
            {
                if (_Big5_e is null)
                    _Big5_e = Encoding.GetEncoding("Big5"); // 950
                return _Big5_e;
            }
        }
        private static Encoding _ShiftJIS_e = null;

        public static Encoding ShiftJIS
        {
            get
            {
                if (_ShiftJIS_e is null)
                    _ShiftJIS_e = Encoding.GetEncoding("Shift-JIS"); // 932
                return _ShiftJIS_e;
            }
        }
        private static Encoding _ISO8859_1_e = null;

        public static Encoding ISO8859_1
        {
            get
            {
                if (_ISO8859_1_e is null)
                    _ISO8859_1_e = Encoding.GetEncoding("ISO-8859-1"); // 28591
                return _ISO8859_1_e;
            }
        }
        private static Encoding _Windows1252_e = null;

        public static Encoding Windows1252
        {
            get
            {
                if (_Windows1252_e is null)
                    _Windows1252_e = Encoding.GetEncoding("Windows-1252"); // 1252
                return _Windows1252_e;
            }
        }

        /// <summary>
        /// 将指定字节数组中的所有字节解码为一组字符。
        /// </summary>
        /// <param name="This">编码。</param>
        /// <param name="Bytes">包含要解码的字节序列的字节数组。</param>
        /// <returns>一个字节数组，包含对指定的字节序列进行解码的结果。</returns>
        public static Char32[] GetString32(this Encoding This, byte[] Bytes)
        {
            return String32.FromUTF16B(Conversions.ToString(This.GetChars(Bytes)));
        }

        /// <summary>
        /// 映射。
        /// </summary>
        /// <typeparam name="D">定义域。</typeparam>
        /// <typeparam name="R">值域。</typeparam>
        public delegate R Mapping<D, R>(D d);
    }
}