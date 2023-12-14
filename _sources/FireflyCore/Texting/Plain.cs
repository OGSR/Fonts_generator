// ==========================================================================
// 
// File:        Plain.vb
// Location:    Firefly.Texting <Visual Basic .Net>
// Description: 纯文本格式
// Version:     2009.10.31.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System.Collections.Generic;
using Firefly.TextEncoding;

namespace Firefly.Texting
{
    public sealed class Plain
    {
        private Plain()
        {
        }

        public static string[] ReadFile(string Path, System.Text.Encoding Encoding)
        {
            using (var s = Txt.CreateTextReader(Path, Encoding, true))
            {
                var l = new List<string>();
                while (!s.EndOfStream)
                {
                    string Line = s.ReadLine();
                    if (!string.IsNullOrEmpty(Line))
                    {
                        Line = Line.Replace(@"\n", String32.ChrQ(13) + String32.ChrQ(10));
                        Line = Line.Replace(@"\x5C", @"\");
                    }
                    l.Add(Line);
                }
                return l.ToArray();
            }
        }
        public static void WriteFile(string Path, System.Text.Encoding Encoding, IEnumerable<string> Value)
        {
            using (var s = Txt.CreateTextWriter(Path, Encoding, true))
            {
                int n = 0;
                foreach (var v in Value)
                {
                    if (!string.IsNullOrEmpty(v))
                    {
                        string Line = v;
                        if (!string.IsNullOrEmpty(Line))
                            Line = Line.Replace(@"\", @"\x5C");
                        if (!string.IsNullOrEmpty(Line))
                            Line = Line.Replace(String32.ChrQ(13) + String32.ChrQ(10), String32.ChrQ(10)).Replace(String32.ChrQ(10), @"\n");
                        s.WriteLine(Line);
                    }
                    else
                    {
                        s.WriteLine();
                    }
                    n += 1;
                }
            }
        }
        public static string[] ReadFile(string Path)
        {
            return ReadFile(Path, TextEncoding.TextEncoding.Default);
        }
        public static void WriteFile(string Path, IEnumerable<string> Value)
        {
            WriteFile(Path, TextEncoding.TextEncoding.WritingDefault, Value);
        }
    }
}