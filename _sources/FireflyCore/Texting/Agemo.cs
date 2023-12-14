// ==========================================================================
// 
// File:        Agemo.vb
// Location:    Firefly.Texting <Visual Basic .Net>
// Description: Agemo文本格式
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
    public sealed class Agemo
    {
        private Agemo()
        {
        }

        public static string[] ReadFile(string Path, Encoding Encoding)
        {
            var l = new LinkedList<string>();
            var sb = new List<Char32>();
            bool NotNull = false;
            int LineNumber = 1;
            using (var s = Txt.CreateTextReader(Path, Encoding, true))
            {
                var r = new Regex(@"^#### *(?<index>\d+) *####$", RegexOptions.ExplicitCapture);
                int PreIndex = 0;
                while (!s.EndOfStream)
                {
                    var LineBuilder = new List<Char32>();
                    string LineSeperator = "";
                    while (!s.EndOfStream)
                    {
                        var c = String32.ChrQ(s.Read());
                        if (c == ControlChars.Cr)
                        {
                            if (String32.ChrQ(s.Peek()) == ControlChars.Lf)
                            {
                                LineSeperator = ControlChars.CrLf;
                                s.Read();
                            }
                            else
                            {
                                LineSeperator = ControlChars.Cr;
                            }
                            break;
                        }
                        else if (c == ControlChars.Lf)
                        {
                            LineSeperator = ControlChars.Lf;
                            break;
                        }
                        else
                        {
                            LineBuilder.Add(c);
                        }
                    }
                    Char32[] Line = LineBuilder.ToArray();
                    var Match = r.Match(Line.ToUTF16B());
                    if (Match.Success)
                    {
                        NotNull = true;
                        RemoveLast(sb, ControlChars.Lf);
                        RemoveLast(sb, ControlChars.Cr);
                        RemoveLast(sb, ControlChars.Lf);
                        RemoveLast(sb, ControlChars.Cr);
                        l.AddLast(sb.ToArray().ToUTF16B());
                        sb = new List<Char32>();
                        int Index;
                        if (!int.TryParse(Match.Result("${index}").Trim(), out Index))
                        {
                            throw new InvalidDataException(string.Format("{0}({1}) : 格式错误。", Path, LineNumber));
                        }
                        if (Index != PreIndex + 1)
                        {
                            throw new InvalidDataException(string.Format("{0}({1}) : 格式错误。", Path, LineNumber));
                        }
                        PreIndex = Index;
                    }
                    else
                    {
                        sb.AddRange(Line);
                        sb.AddRange(LineSeperator.ToUTF32());
                    }
                    LineNumber += 1;
                }
                if (l.Count > 0)
                    l.RemoveFirst();
                RemoveLast(sb, ControlChars.Lf);
                RemoveLast(sb, ControlChars.Cr);
                RemoveLast(sb, ControlChars.Lf);
                RemoveLast(sb, ControlChars.Cr);
                if (!NotNull)
                {
                    if (sb.Count > 0)
                        throw new InvalidDataException(string.Format("{0}({1}) : 格式错误或编码错误。", Path, LineNumber));
                    return new string[] { };
                }
                l.AddLast(sb.ToArray().ToUTF16B());
                string[] ret = new string[l.Count];
                l.CopyTo(ret, 0);
                return ret;
            }
        }
        public static bool VerifyFile(string Path, Encoding Encoding, out string LogText, bool EnforceEncoding = false)
        {
            var Log = new List<string>();
            var sb = new List<Char32>();
            bool NotNull = false;
            int LineNumber = 0;
            using (var s = Txt.CreateTextReader(Path, Encoding, !EnforceEncoding))
            {
                var r0 = new Regex(@"^#### *(?<index>\d+) *####", RegexOptions.ExplicitCapture);
                var r = new Regex(@"^#### *(?<index>\d+) *####$", RegexOptions.ExplicitCapture);
                int PreIndex = 0;
                while (!s.EndOfStream)
                {
                    LineNumber += 1;
                    var LineBuilder = new List<Char32>();
                    string LineSeperator = "";
                    while (!s.EndOfStream)
                    {
                        var c = String32.ChrQ(s.Read());
                        if (c == ControlChars.Cr)
                        {
                            if (String32.ChrQ(s.Peek()) == ControlChars.Lf)
                            {
                                LineSeperator = ControlChars.CrLf;
                                s.Read();
                            }
                            else
                            {
                                LineSeperator = ControlChars.Cr;
                            }
                            break;
                        }
                        else if (c == ControlChars.Lf)
                        {
                            LineSeperator = ControlChars.Lf;
                            break;
                        }
                        else
                        {
                            LineBuilder.Add(c);
                        }
                    }
                    Char32[] Line = LineBuilder.ToArray();
                    var Match = r.Match(Line.ToUTF16B());
                    if (Match.Success)
                    {
                        NotNull = true;
                        RemoveLast(sb, ControlChars.Lf);
                        RemoveLast(sb, ControlChars.Cr);
                        RemoveLast(sb, ControlChars.Lf);
                        RemoveLast(sb, ControlChars.Cr);
                        sb = new List<Char32>();
                        int Index;
                        if (!int.TryParse(Match.Result("${index}").Trim(), out Index))
                        {
                            Log.Add(string.Format("{0}({1}) : 格式错误。", Path, LineNumber));
                        }
                        else
                        {
                            if (Index != PreIndex + 1)
                            {
                                Log.Add(string.Format("{0}({1}) : 格式错误。", Path, LineNumber));
                            }
                            PreIndex = Index;
                        }
                    }
                    else
                    {
                        var Match0 = r0.Match(Line.ToUTF16B());
                        if (Match0.Success)
                        {
                            int Index;
                            if (!int.TryParse(Match0.Result("${index}").Trim(), out Index))
                            {
                                Log.Add(string.Format("{0}({1}) : 格式错误。", Path, LineNumber));
                            }
                            else
                            {
                                Log.Add(string.Format("{0}({1}) : 格式错误。", Path, LineNumber));
                                PreIndex = Index;
                            }
                        }
                        else
                        {
                            sb.AddRange(Line);
                            sb.AddRange(LineSeperator.ToUTF32());
                        }
                    }
                }
                RemoveLast(sb, ControlChars.Lf);
                RemoveLast(sb, ControlChars.Cr);
                RemoveLast(sb, ControlChars.Lf);
                RemoveLast(sb, ControlChars.Cr);
                if (!NotNull)
                {
                    if (sb.Count > 0)
                        Log.Add(string.Format("{0}({1}) : 格式错误或编码错误。", Path, LineNumber));
                }
                LogText = string.Join(Environment.NewLine, Log.ToArray());
                return Log.Count == 0;
            }
        }
        public static void WriteFile(string Path, Encoding Encoding, IEnumerable<string> Value)
        {
            using (var s = Txt.CreateTextWriter(Path, Encoding, true))
            {
                int n = 0;
                foreach (var v in Value)
                {
                    s.WriteLine(string.Format("#### {0} ####", n + 1));
                    s.WriteLine(v);
                    s.WriteLine();
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
        private static void RemoveLast(List<Char32> sb, Char32 c)
        {
            if (sb.Count >= 1 && sb[sb.Count - 1] == c)
                sb.RemoveAt(sb.Count - 1);
        }
    }
}