// ==========================================================================
// 
// File:        CharAdder.vb
// Location:    Firefly.CharAdder <Visual Basic .Net>
// Description: Character in the library
// Version:     2010.03.05.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Firefly;
using Firefly.TextEncoding;
using static Firefly.TextEncoding.EncodingString;
using Firefly.Texting;

namespace CharAdder
{

    public static class CharAdder
    {

        public static void Main()
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                MainInner();
            }
            else
            {
                try
                {
                    MainInner();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.PopupException(ex);
                }
            }
        }

        public static void MainInner()
        {
            var CmdLine = CommandLine.GetCmdLine();
            string[] argv = CmdLine.Arguments;
            var RemoveUnicodeRanges = new List<Range>();
            bool IgnoreExistingBaseFile = false;
            foreach (var opt in CmdLine.Options)
            {
                switch (opt.Name.ToLower() ?? "")
                {
                    case "?":
                    case "help":
                        {
                            DisplayInfo();
                            return;
                        }
                    case "removeunicode":
                        {
                            string[] arg = opt.Arguments;
                            switch (arg.Length)
                            {
                                case 2:
                                    {
                                        int l = int.Parse(arg[0], System.Globalization.NumberStyles.HexNumber);
                                        int u = int.Parse(arg[1], System.Globalization.NumberStyles.HexNumber);
                                        RemoveUnicodeRanges.Add(new Range(l, u));
                                        break;
                                    }

                                default:
                                    {
                                        throw new ArgumentException(opt.Name + ":" + string.Join(",", opt.Arguments));
                                    }
                            }

                            break;
                        }
                    case "i":
                        {
                            IgnoreExistingBaseFile = true;
                            break;
                        }

                    default:
                        {
                            throw new ArgumentException(opt.Name);
                        }
                }
            }
            switch (argv.Length)
            {
                case 2:
                    {
                        AddChar(argv[0], argv[1], "", RemoveUnicodeRanges, IgnoreExistingBaseFile);
                        break;
                    }
                case 3:
                    {
                        AddChar(argv[0], argv[1], argv[2], RemoveUnicodeRanges, IgnoreExistingBaseFile);
                        break;
                    }

                default:
                    {
                        DisplayInfo();
                        break;
                    }
            }
        }

        public static void DisplayInfo()
        {
           Console.WriteLine("Character library");
           Console.WriteLine("Firefly.CharAdder, distributed under BSD license");
           Console.WriteLine("F.R.C.");
           Console.WriteLine("");
           Console.WriteLine("Usage:");
           Console.WriteLine("CharAdder <Pattern> <Char File> [<Exclude File>] (Remove Unicode)* [/I]");
           Console.WriteLine("RemoveUnicode ::= /removeunicode:<Lower:Hex>,<Upper:Hex>");
           Console.WriteLine("Pattern text file name pattern, refer to MSDN - Regular Expressions [.NET Framework]");
           Console.WriteLine("CharFile character library file");
           Console.WriteLine("ExcludeFile character exclusion library file");
           Console.WriteLine("/removeunicode removes characters within the Unicode range (including both boundaries). The range of Unicode includes the extended plane");
           Console.WriteLine("/I ignore characters in existing character library files");
           Console.WriteLine("Note: Text file encoding only supports GB18030 (GB2312) and Unicode encoding with BOM. The generated results are saved as UTF-16 encoding.");
           Console.WriteLine("");
           Console.WriteLine("Example:");
           Console.WriteLine(@"CharAdder "".*?\.txt"" Char.txt Exclude.txt");
           Console.WriteLine("Extract characters from all files with the extension txt, exclude the characters in Exclude.txt, and add them to the end of Char.txt.");
        }

        public static void AddChar(string Pattern, string BaseFile, string ExcludeFile, List<Range> RemoveUnicodeRanges, bool IgnoreExistingBaseFile)
        {
            var g = new EncodingStringGenerator();
            g.PushExclude(ControlChars.Cr);
            g.PushExclude(ControlChars.Lf);

            if (File.Exists(ExcludeFile))
            {
                g.PushExclude(Txt.ReadFile(ExcludeFile, TextEncoding.Default));
            }

            string LibString = "";
            if (!IgnoreExistingBaseFile)
            {
                if (File.Exists(BaseFile))
                {
                    LibString = Txt.ReadFile(BaseFile, TextEncoding.Default);
                    g.PushExclude(LibString);
                }
            }

            foreach (var c in new Indexer(RemoveUnicodeRanges))
                g.PushExclude(c);

            int Count = 0;

            var Regex = new Regex("^" + Pattern + "$", RegexOptions.ExplicitCapture);
            foreach (var f1 in Directory.GetFiles(".", "*.*", SearchOption.AllDirectories))
            {
                string f = f1;
                if (f.StartsWith(@".\") || f.StartsWith("./"))
                    f = f.Substring(2);
                var Match = Regex.Match(Path.GetFileName(f));
                if (Match.Success)
                {
                    g.PushText(Txt.ReadFile(f, TextEncoding.Default));
                    Count += 1;
                }
            }

            LibString += g.GetLibString();

            using (var BaseWriter = new StreamWriter(BaseFile, false, System.Text.Encoding.Unicode))
            {
                BaseWriter.Write(LibString.ToString());
            }

            Console.WriteLine("A total of {0} file is processed.".Formats(Count));
        }
    }
}