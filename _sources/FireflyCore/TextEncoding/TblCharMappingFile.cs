// ==========================================================================
// 
// File:        TblCharMappingFile.vb
// Location:    Firefly.TextEncoding <Visual Basic .Net>
// Description: tbl字符映射表文件
// Version:     2009.11.21.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Firefly.TextEncoding
{
    public sealed class TblCharMappingFile
    {
        private TblCharMappingFile()
        {
        }

        public static IEnumerable<KeyValuePair<string, string>> ReadRaw(string Path, Encoding Encoding)
        {
            var d = new List<KeyValuePair<string, string>>();
            using (var s = Texting.Txt.CreateTextReader(Path, Encoding, true))
            {
                var r = new Regex("^(?<left>.*?)=(?<right>.*)$", RegexOptions.ExplicitCapture);
                while (!s.EndOfStream)
                {
                    string Line = s.ReadLine();
                    var Match = r.Match(Line);
                    if (!Match.Success)
                        continue;
                    string Left = Match.Result("${left}");
                    string Right = Match.Result("${right}");
                    d.Add(new KeyValuePair<string, string>(Left, Right));
                }
            }
            return d;
        }
        public static IEnumerable<KeyValuePair<string, string>> ReadRaw(string Path)
        {
            return ReadRaw(Path, TextEncoding.Default);
        }
        public static IEnumerable<StringCode> ReadFile(string Path, Encoding Encoding)
        {
            var d = new List<StringCode>();
            using (var s = Texting.Txt.CreateTextReader(Path, Encoding, true))
            {
                var r = new Regex("^(?<left>.*?)=(?<right>.*)$", RegexOptions.ExplicitCapture);
                while (!s.EndOfStream)
                {
                    string Line = s.ReadLine();
                    var Match = r.Match(Line);
                    if (!Match.Success)
                        continue;
                    string Left = Match.Result("${left}").Trim(' ');
                    var c = StringCode.FromNothing();
                    if (!string.IsNullOrEmpty(Left))
                    {
                        c.CodeString = Left;
                    }
                    string Right = Match.Result("${right}");
                    if (Right.Trim(' ').Length >= 1)
                        Right = Right.Trim(' ');
                    if (Right.Trim(' ').Length >= 2)
                        Right = Right.Descape();
                    if (!string.IsNullOrEmpty(Right))
                    {
                        c.Unicode = Right;
                    }
                    d.Add(c);
                }
            }
            return d;
        }
        public static IEnumerable<StringCode> ReadFile(string Path)
        {
            return ReadFile(Path, TextEncoding.Default);
        }
        public static Encoding ReadAsEncoding(string Path, Encoding Encoding)
        {
            return new MultiByteEncoding(ReadFile(Path, Encoding));
        }
        public static Encoding ReadAsEncoding(string Path)
        {
            return new MultiByteEncoding(ReadFile(Path, TextEncoding.Default));
        }
        public static void WriteRaw(string Path, Encoding Encoding, IEnumerable<KeyValuePair<string, string>> l)
        {
            using (var s = Texting.Txt.CreateTextWriter(Path, Encoding, true))
            {
                foreach (var p in l)
                    s.WriteLine(p.Key + "=" + p.Value);
            }
        }
        public static void WriteRaw(string Path, IEnumerable<KeyValuePair<string, string>> l)
        {
            WriteRaw(Path, TextEncoding.WritingDefault, l);
        }
        public static void WriteFile(string Path, Encoding Encoding, IEnumerable<StringCode> l)
        {
            using (var s = Texting.Txt.CreateTextWriter(Path, Encoding, true))
            {
                foreach (var p in l)
                {
                    string Character = "";
                    if (p.HasUnicode)
                    {
                        Character = p.Character;
                        Character = Character.Escape();
                    }
                    s.WriteLine(p.CodeString + "=" + Character);
                }
            }
        }
        public static void WriteFile(string Path, IEnumerable<StringCode> l)
        {
            WriteFile(Path, TextEncoding.WritingDefault, l);
        }
    }
}