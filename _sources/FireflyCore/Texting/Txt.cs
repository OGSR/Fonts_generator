// ==========================================================================
// 
// File:        Txt.vb
// Location:    Firefly.Texting <Visual Basic .Net>
// Description: Text file format
// Version:     2009.11.21.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System.IO;
using System.Text;
using Firefly.TextEncoding;

namespace Firefly.Texting
{
    public sealed class Txt
    {
        private Txt()
        {
        }

        /// <summary>Reloaded. Check UTF-16(FF FE), GB18030(84 31 95 33), UTF-8(EF BB BF), UTF-32(FF FE 00 00), UTF-16B(FE FF), UTF-32B(00 00 FE FF) BOM of these six encodings. If it fails, it will return to the system default encoding (GB2312 will be replaced by GB18030). </summary>
        public static Encoding GetEncoding(string Path)
        {
            using (var s = new StreamEx(Path, FileMode.Open, FileAccess.Read))
            {
                if (s.Length >= 4L)
                {
                    s.Position = 0L;
                    int BOM = s.ReadInt32BigEndian();
                    if (BOM == int.MinValue + 0x7FFE0000)
                        return TextEncoding.TextEncoding.UTF32;
                    if (BOM == 0xFEFF)
                        return TextEncoding.TextEncoding.UTF32B;
                    if (BOM == int.MinValue + 0x04319533)
                        return TextEncoding.TextEncoding.GB18030;
                }
                if (s.Length >= 3L)
                {
                    s.Position = 0L;
                    int BOM = s.ReadUInt16BigEndian();
                    BOM = BOM << 8 | s.ReadByte();
                    if (BOM == 0xEFBBBF)
                        return TextEncoding.TextEncoding.UTF8;
                }
                if (s.Length >= 2L)
                {
                    s.Position = 0L;
                    ushort BOM = s.ReadUInt16BigEndian();
                    if (BOM == 0xFFFE)
                        return TextEncoding.TextEncoding.UTF16;
                    if (BOM == 0xFEFF)
                        return TextEncoding.TextEncoding.UTF16B;
                }
                return TextEncoding.TextEncoding.Default;
            }
        }

        /// <summary>Reloaded. Check UTF-16(FF FE), GB18030(84 31 95 33), UTF-8(EF BB BF), UTF-32(FF FE 00 00), UTF-16B(FE FF), UTF-32B(00 00 FE FF) BOM of these six encodings, if failed, return to the default encoding. </summary>
        public static Encoding GetEncoding(string Path, Encoding DefaultEncoding)
        {
            using (var s = new StreamEx(Path, FileMode.Open, FileAccess.Read))
            {
                if (s.Length >= 4L)
                {
                    s.Position = 0L;
                    int BOM = s.ReadInt32BigEndian();
                    if (BOM == int.MinValue + 0x7FFE0000)
                        return TextEncoding.TextEncoding.UTF32;
                    if (BOM == 0xFEFF)
                        return TextEncoding.TextEncoding.UTF32B;
                    if (BOM == int.MinValue + 0x04319533)
                        return TextEncoding.TextEncoding.GB18030;
                }
                if (s.Length >= 3L)
                {
                    s.Position = 0L;
                    int BOM = s.ReadUInt16BigEndian();
                    BOM = BOM << 8 | s.ReadByte();
                    if (BOM == 0xEFBBBF)
                        return TextEncoding.TextEncoding.UTF8;
                }
                if (s.Length >= 2L)
                {
                    s.Position = 0L;
                    ushort BOM = s.ReadUInt16BigEndian();
                    if (BOM == 0xFFFE)
                        return TextEncoding.TextEncoding.UTF16;
                    if (BOM == 0xFEFF)
                        return TextEncoding.TextEncoding.UTF16B;
                }
                return DefaultEncoding;
            }
        }

        /// <param name="Detect Encoding From Byte Order Marks">If true, UTF-16 (FF FE), GB18030 (84 31 95 33), UTF-8 (EF BB BF), UTF-32 will be checked (FF FE 00 00), UTF-16B (FE FF), UTF-32B (00 00 FE FF). </param>
        public static StreamReader CreateTextReader(string Path, Encoding Encoding, bool DetectEncodingFromByteOrderMarks = true)
        {
            if (DetectEncodingFromByteOrderMarks)
            {
                var s = new StreamEx(Path, FileMode.Open, FileAccess.Read);
                if (s.Length >= 4L)
                {
                    s.Position = 0L;
                    int BOM = s.ReadInt32BigEndian();
                    if (BOM == int.MinValue + 0x7FFE0000)
                        return new StreamReader(new PartialStreamEx(s, 4L, s.Length - 4L, true).ToUnsafeStream(), TextEncoding.TextEncoding.UTF32, false);
                    if (BOM == 0xFEFF)
                        return new StreamReader(new PartialStreamEx(s, 4L, s.Length - 4L, true).ToUnsafeStream(), TextEncoding.TextEncoding.UTF32B, false);
                    if (BOM == int.MinValue + 0x04319533)
                        return new StreamReader(new PartialStreamEx(s, 4L, s.Length - 4L, true).ToUnsafeStream(), TextEncoding.TextEncoding.GB18030, false);
                }
                if (s.Length >= 3L)
                {
                    s.Position = 0L;
                    int BOM = s.ReadUInt16BigEndian();
                    BOM = BOM << 8 | s.ReadByte();
                    if (BOM == 0xEFBBBF)
                        return new StreamReader(new PartialStreamEx(s, 3L, s.Length - 3L, true).ToUnsafeStream(), TextEncoding.TextEncoding.UTF8, false);
                }
                if (s.Length >= 2L)
                {
                    s.Position = 0L;
                    ushort BOM = s.ReadUInt16BigEndian();
                    if (BOM == 0xFFFE)
                        return new StreamReader(new PartialStreamEx(s, 2L, s.Length - 2L, true).ToUnsafeStream(), TextEncoding.TextEncoding.UTF16, false);
                    if (BOM == 0xFEFF)
                        return new StreamReader(new PartialStreamEx(s, 2L, s.Length - 2L, true).ToUnsafeStream(), TextEncoding.TextEncoding.UTF16B, false);
                }
                s.Dispose();
                return new StreamReader(Path, Encoding, true);
            }
            else
            {
                return new StreamReader(Path, Encoding, false);
            }
        }

        public static StreamReader CreateTextReader(string Path)
        {
            return CreateTextReader(Path, TextEncoding.TextEncoding.Default, true);
        }

        /// <param name="Detect Encoding From Byte Order Marks">If true, UTF-16 (FF FE), GB18030 (84 31 95 33), UTF-8 (EF BB BF), UTF-32 will be checked (FF FE 00 00), UTF-16B (FE FF), UTF-32B (00 00 FE FF). </param>
        public static string ReadFile(string Path, Encoding Encoding, bool DetectEncodingFromByteOrderMarks = true)
        {
            using (var s = CreateTextReader(Path, Encoding, DetectEncodingFromByteOrderMarks))
            {
                if (!s.EndOfStream)
                    return s.ReadToEnd();
            }
            return "";
        }
        public static string ReadFile(string Path)
        {
            return ReadFile(Path, TextEncoding.TextEncoding.Default);
        }


        /// <param name="With Byte Order Marks">If true, it will be UTF-16(FF FE), GB18030(84 31 95 33), UTF-8(EF BB BF), UTF-32(FF Six encodings: FE 00 00), UTF-16B (FE FF), and UTF-32B (00 00 FE FF) are written into the BOM. </param>
        public static StreamWriter CreateTextWriter(string Path, Encoding Encoding, bool WithByteOrderMarks = true)
        {
            if (WithByteOrderMarks)
            {
                if (ReferenceEquals(Encoding, TextEncoding.TextEncoding.UTF16))
                {
                    using (var s = new StreamEx(Path, FileMode.Create, FileAccess.ReadWrite))
                    {
                        s.WriteByte(0xFF);
                        s.WriteByte(0xFE);
                    }
                }
                else if (ReferenceEquals(Encoding, TextEncoding.TextEncoding.GB18030))
                {
                    using (var s = new StreamEx(Path, FileMode.Create, FileAccess.ReadWrite))
                    {
                        s.WriteInt32BigEndian(int.MinValue + 0x04319533);
                    }
                }
                else if (ReferenceEquals(Encoding, TextEncoding.TextEncoding.UTF8))
                {
                    using (var s = new StreamEx(Path, FileMode.Create, FileAccess.ReadWrite))
                    {
                        s.WriteByte(0xEF);
                        s.WriteByte(0xBB);
                        s.WriteByte(0xBF);
                    }
                }
                else if (ReferenceEquals(Encoding, TextEncoding.TextEncoding.UTF32))
                {
                    using (var s = new StreamEx(Path, FileMode.Create, FileAccess.ReadWrite))
                    {
                        s.WriteByte(0xFF);
                        s.WriteByte(0xFE);
                        s.WriteByte(0);
                        s.WriteByte(0);
                    }
                }
                else if (ReferenceEquals(Encoding, TextEncoding.TextEncoding.UTF16B))
                {
                    using (var s = new StreamEx(Path, FileMode.Create, FileAccess.ReadWrite))
                    {
                        s.WriteByte(0xFE);
                        s.WriteByte(0xFF);
                    }
                }
                else if (ReferenceEquals(Encoding, TextEncoding.TextEncoding.UTF32B))
                {
                    using (var s = new StreamEx(Path, FileMode.Create, FileAccess.ReadWrite))
                    {
                        s.WriteByte(0);
                        s.WriteByte(0);
                        s.WriteByte(0xFE);
                        s.WriteByte(0xFF);
                    }
                }
                else
                {
                    using (var s = new StreamEx(Path, FileMode.Create, FileAccess.ReadWrite))
                    {
                    }
                }
            }
            else
            {
                using (var s = new StreamEx(Path, FileMode.Create, FileAccess.ReadWrite))
                {
                }
            }
            return new StreamWriter(Path, true, new EncodingNoPreambleWrapper(Encoding));
        }

        public static StreamWriter CreateTextWriter(string Path)
        {
            return CreateTextWriter(Path, TextEncoding.TextEncoding.WritingDefault, true);
        }

        /// <param name="With Byte Order Marks">If true, it will be UTF-16(FF FE), GB18030(84 31 95 33), UTF-8(EF BB BF), UTF-32(FF Six encodings: FE 00 00), UTF-16B (FE FF), and UTF-32B (00 00 FE FF) are written into the BOM. </param>
        public static void WriteFile(string Path, Encoding Encoding, string Value, bool WithByteOrderMarks = true)
        {
            using (var s = CreateTextWriter(Path, Encoding, WithByteOrderMarks))
            {
                s.Write(Value);
            }
        }
        public static void WriteFile(string Path, string Value)
        {
            WriteFile(Path, TextEncoding.TextEncoding.WritingDefault, Value);
        }
    }
}