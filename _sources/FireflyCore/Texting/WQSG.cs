// ==========================================================================
// 
// File:        WQSG.vb
// Location:    Firefly.Texting <Visual Basic .Net>
// Description: WQSGText文本格式
// Version:     2009.10.31.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Firefly.TextEncoding;

namespace Firefly.Texting
{
    public static class WQSGExt
    {
        public static string[] GetTexts(this WQSG.Triple[] This)
        {
            return WQSG.GetTexts(This);
        }
    }
    public sealed class WQSG
    {
        private WQSG()
        {
        }

        public class Triple
        {
            public int Offset;
            public int Length;
            public string Text;
        }
        public static string[] GetTexts(Triple[] This)
        {
            string[] Texts = new string[This.Length];
            for (int n = 0, loopTo = This.Length - 1; n <= loopTo; n++)
                Texts[n] = This[n].Text;
            return Texts;
        }
        public static Triple[] ReadFile(string Path, Encoding Encoding)
        {
            var l = new List<Triple>();
            int LineNumber = 1;
            using (var s = Txt.CreateTextReader(Path, Encoding, true))
            {
                var r = new Regex("^(?<offset>.*?),(?<length>.*?),(?<text>.*)$", RegexOptions.ExplicitCapture);
                while (!s.EndOfStream)
                {
                    string Line = s.ReadLine();
                    if (string.IsNullOrEmpty(Line))
                    {
                        LineNumber += 1;
                        continue;
                    }
                    var Match = r.Match(Line);
                    if (Match.Success)
                    {
                        var t = new Triple();
                        if (!int.TryParse(Match.Result("${offset}"), System.Globalization.NumberStyles.HexNumber, null, out t.Offset))
                            throw new InvalidDataException(string.Format("{0}({1}) : 格式错误。", Path, LineNumber));
                        if (!int.TryParse(Match.Result("${length}"), System.Globalization.NumberStyles.Integer, null, out t.Length))
                            throw new InvalidDataException(string.Format("{0}({1}) : 格式错误。", Path, LineNumber));
                        t.Text = Match.Result("${text}").Replace(@"\n", ControlChars.CrLf);
                        l.Add(t);
                    }
                    else
                    {
                        throw new InvalidDataException(string.Format("{0}({1}) : 格式错误。", Path, LineNumber));
                    }
                    LineNumber += 1;
                }
            }
            return l.ToArray();
        }
        public static bool VerifyFile(string Path, Encoding Encoding, out string LogText)
        {
            var Log = new List<string>();
            int LineNumber = 1;
            using (var s = Txt.CreateTextReader(Path, Encoding, true))
            {
                var r = new Regex("^(?<offset>.*?),(?<length>.*?),(?<text>.*)$", RegexOptions.ExplicitCapture);
                while (!s.EndOfStream)
                {
                    string Line = s.ReadLine();
                    if (string.IsNullOrEmpty(Line.Trim()))
                    {
                        LineNumber += 1;
                        continue;
                    }
                    var Match = r.Match(Line);
                    if (Match.Success)
                    {
                        var t = new Triple();
                        if (!int.TryParse(Match.Result("${offset}"), System.Globalization.NumberStyles.HexNumber, null, out t.Offset))
                            Log.Add(string.Format("{0}({1}) : 格式错误。", Path, LineNumber));
                        if (!int.TryParse(Match.Result("${length}"), System.Globalization.NumberStyles.Integer, null, out t.Length))
                            Log.Add(string.Format("{0}({1}) : 格式错误。", Path, LineNumber));
                        t.Text = Match.Result("${text}").Replace(@"\n", ControlChars.CrLf);
                    }
                    else
                    {
                        Log.Add(string.Format("{0}({1}) : 格式错误。", Path, LineNumber));
                    }
                    LineNumber += 1;
                }
            }
            LogText = string.Join(Environment.NewLine, Log.ToArray());
            return Log.Count == 0;
        }
        public static void WriteFile(string Path, Encoding Encoding, IEnumerable<Triple> Value)
        {
            using (var s = Txt.CreateTextWriter(Path, Encoding, true))
            {
                int n = 0;
                foreach (var v in Value)
                {
                    s.WriteLine(string.Format("{0},{1},{2}", v.Offset.ToString("X8"), v.Length, v.Text.Replace(ControlChars.CrLf, ControlChars.Lf).Replace(ControlChars.Lf, @"\n")));
                    s.WriteLine();
                    n += 1;
                }
            }
        }
        public static Triple[] ReadFile(string Path)
        {
            return ReadFile(Path, TextEncoding.TextEncoding.Default);
        }
        public static void WriteFile(string Path, IEnumerable<Triple> Value)
        {
            WriteFile(Path, TextEncoding.TextEncoding.WritingDefault, Value);
        }
    }
}