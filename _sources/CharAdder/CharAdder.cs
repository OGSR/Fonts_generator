// ==========================================================================
// 
// File:        CharAdder.vb
// Location:    Firefly.CharAdder <Visual Basic .Net>
// Description: 字符入库器
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
            Console.WriteLine("字符入库器");
            Console.WriteLine("Firefly.CharAdder，按BSD许可证分发");
            Console.WriteLine("F.R.C.");
            Console.WriteLine("");
            Console.WriteLine("用法:");
            Console.WriteLine("CharAdder <Pattern> <CharFile> [<ExcludeFile>] (RemoveUnicode)* [/I]");
            Console.WriteLine("RemoveUnicode ::= /removeunicode:<Lower:Hex>,<Upper:Hex>");
            Console.WriteLine("Pattern 文本的文件名模式，参考 MSDN - 正则表达式 [.NET Framework]");
            Console.WriteLine("CharFile 字符库文件");
            Console.WriteLine("ExcludeFile 字符排除库文件");
            Console.WriteLine("/removeunicode 移除该Unicode范围内(包含两边界)字符，Unicode的范围包括扩展平面");
            Console.WriteLine("/I 忽略已有字符库文件中的字符");
            Console.WriteLine("注意：文本文件编码仅支持GB18030(GB2312)和带BOM的Unicode系编码。生成的结果保存为UTF-16编码。");
            Console.WriteLine("");
            Console.WriteLine("示例:");
            Console.WriteLine(@"CharAdder "".*?\.txt"" Char.txt Exclude.txt");
            Console.WriteLine("从所有扩展名为txt的文件中提取字符，排除掉Exclude.txt中的字符，加到Char.txt的最后。");
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

            Console.WriteLine("共处理了{0}个文件。".Formats(Count));
        }
    }
}