// ==========================================================================
// 
// File:        FontGen.vb
// Location:    Firefly.FontGen <Visual Basic .Net>
// Description: 字库图片生成器
// Version:     2010.04.08.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Firefly;
using Firefly.Glyphing;
using Firefly.Imaging;
using Firefly.TextEncoding;
using Firefly.Texting;
using Microsoft.VisualBasic.CompilerServices;

namespace FontGen
{

    public partial class FontGen
    {
        public FontGen()
        {
            InitializeComponent();
        }
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        public static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ExceptionHandler.PopupException(e.Exception, new StackTrace(4, true));
        }

        public static int Main()
        {
            if (Debugger.IsAttached)
            {
                return MainInner();
            }
            else
            {
                try
                {
                    return MainInner();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.PopupException(ex);
                    return -1;
                }
            }
        }

        public static int MainInner()
        {
            var CmdLine = CommandLine.GetCmdLine();
            string[] argv = CmdLine.Arguments;
            CommandLine.CommandLineOption[] opt = CmdLine.Options;

            if (argv.Length == 0 && opt.Length == 0)
            {
                if (Debugger.IsAttached)
                {
                    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
                    return MainWindow();
                }
                else
                {
                    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                    try
                    {
                        Application.ThreadException += Application_ThreadException;
                        return MainWindow();
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.PopupException(ex);
                        return -1;
                    }
                    finally
                    {
                        Application.ThreadException -= Application_ThreadException;
                    }
                }
            }
            else
            {
                return MainConsole();
            }
        }

        public static int MainWindow()
        {
            FreeConsole();
            Application.EnableVisualStyles();
            Application.Run(new FontGen());
            return 0;
        }

        public static void DisplayInfo()
        {
            Console.WriteLine("字库图片生成器");
            Console.WriteLine("Firefly.FontGen，按BSD许可证分发");
            Console.WriteLine("F.R.C.");
            Console.WriteLine("");
            Console.WriteLine("本生成器用于从tbl编码文件、fd字库描述文件或者字符文件生成字库图片和对应的fd字库描述文件。");
            Console.WriteLine("");
            Console.WriteLine("用法:");
            Console.WriteLine("FontGen <Source tbl/fd/CharFile> <Target fd> <FontName> <FontStyle> <FontSize> <PhysicalWidth> <PhysicalHeight> <DrawOffsetX> <DrawOffsetY> [<VirtualOffsetX> <VirtualOffsetY> <VirtualDeltaWidth> <VirtualDeltaHeight> [<PicWidth> <PicHeight>]] [/x2] [/left]");
            Console.WriteLine("Source tbl/fd/CharFile 输入tbl编码文件、fd字库描述文件或者字符文件路径。");
            Console.WriteLine("Target fd 输出fd字库描述文件路径。输出的字库图片(bmp)也使用该路径。");
            Console.WriteLine("FontName 字体名称。");
            Console.WriteLine("FontStyle 字体风格，加粗 1 斜体 2 下划线 4 删除线 8，可叠加。");
            Console.WriteLine("FontSize 字体大小。");
            Console.WriteLine("PhysicalWidth 物理宽度，字符格子宽度。");
            Console.WriteLine("PhysicalHeight 物理高度，字符格子高度。");
            Console.WriteLine("DrawOffsetX 绘制X偏移。");
            Console.WriteLine("DrawOffsetY 绘制Y偏移。");
            Console.WriteLine("VirtualOffsetX 虚拟X偏移，字符的显示部分的X偏移。");
            Console.WriteLine("VirtualOffsetY 虚拟Y偏移，字符的显示部分的Y偏移。");
            Console.WriteLine("VirtualDeltaWidth 虚拟宽度差，字符的显示部分的宽度相对于默认值的差。");
            Console.WriteLine("VirtualDeltaHeight 虚拟高度差，字符的显示部分的高度相对于默认值的差。");
            Console.WriteLine("PicWidth 图片宽度。");
            Console.WriteLine("PicHeight 图片高度。");
            Console.WriteLine("如果不指定图片宽度和高度，将会自动选择最小的能容纳所有字符的2的幂的宽和高。");
            Console.WriteLine("如果指定图片宽度和高度，将会生成多张图片。");
            Console.WriteLine("/x2 2x超采样。");
            Console.WriteLine("/left 左对齐。");
            Console.WriteLine("");
            Console.WriteLine("示例:");
            Console.WriteLine("FontGen FakeShiftJIS.tbl FakeShiftJIS.fd 宋体 0 16 16 16 0 0");
            Console.WriteLine("FontGen FakeShiftJIS.tbl FakeShiftJIS.fd 宋体 0 16 16 16 0 0 0 0 0 0 1024 1024");
            Console.WriteLine("");
            Console.WriteLine("高级用法：");
            Console.WriteLine("FontGen (Add|AddNew|RemoveUnicode|RemoveCode|Save)*");
            Console.WriteLine("Add ::= [/x2] [/left] [/argb:<Pattern>=1xxx] /add:<Source tbl/fd/CharFile>[,<FontName>,<FontStyle>,<FontSize>,<PhysicalWidth>,<PhysicalHeight>,<DrawOffsetX>,<DrawOffsetY>[,<VirtualOffsetX>,<VirtualOffsetY>,<VirtualDeltaWidth>,<VirtualDeltaHeight>]]");
            Console.WriteLine("AddNew ::= [/x2] [/left] [/argb:<Pattern>=1xxx] /addnew:<Source tbl/fd/CharFile>[,<FontName>,<FontStyle>,<FontSize>,<PhysicalWidth>,<PhysicalHeight>,<DrawOffsetX>,<DrawOffsetY>[,<VirtualOffsetX>,<VirtualOffsetY>,<VirtualDeltaWidth>,<VirtualDeltaHeight>]]");
            Console.WriteLine("RemoveUnicode ::= /removeunicode:<Lower:Hex>,<Upper:Hex>");
            Console.WriteLine("RemoveCode ::= /removecode:<Lower:Hex>,<Upper:Hex>");
            Console.WriteLine("Save ::= [/bpp:<BitPerPixel>=8] [/size:<PicWidth>,<PicHeight>] [/multiple] [/compact] /save:<Target fd>");
            Console.WriteLine("/argb 指定颜色的形式");
            Console.WriteLine("Pattern Pattern由4位组成，分别对应A、R、G、B通道，每位可以是0、1或x，其中0表示为0，1表示为最大值，x表示为绘图值");
            Console.WriteLine("/add 添加字形源，可以指定参数由本程序生成，或指定由fd文件加载");
            Console.WriteLine("/addnew 添加字形源，但仅当字符不存在时才添加");
            Console.WriteLine("/removeunicode 移除该Unicode范围内(包含两边界)字符的字形，Unicode的范围包括扩展平面");
            Console.WriteLine("/removecode 移除该编码范围内(包含两边界)字符的字形");
            Console.WriteLine("/bpp 指定位深度");
            Console.WriteLine("BitPerPixel 位深度：1、2、4、8、16、32");
            Console.WriteLine("/size 指定图片大小");
            Console.WriteLine("PicWidth 图片宽度");
            Console.WriteLine("PicHeight 图片高度");
            Console.WriteLine("/multiple 指定保存为多个文件");
            Console.WriteLine("/compact 紧凑存储，列不对齐");
            Console.WriteLine("/save 保存字形到fd文件");
            Console.WriteLine("");
            Console.WriteLine("示例:");
            Console.WriteLine("FontGen /add:Original.fd /removecode:100,10000 /x2 /left /addnew:FakeShiftJIS.tbl,宋体,0,16,16,16,0,0 /save:FakeShiftJIS.fd");
            Console.WriteLine("该例子表明：从Original.fd加载字库，删去0x100到0x10000的部分，然後将FakeShiftJIS.tbl生成字形，将其中新增的字形加入，并将结果保存到FakeShiftJIS.fd");
        }

        public class GlyphComparer : EqualityComparer<IGlyph>
        {

            public override bool Equals(IGlyph x, IGlyph y)
            {
                if (x.c.HasCode && y.c.HasCode)
                    return x.c.Code == y.c.Code;
                if (x.c.HasUnicode && y.c.HasUnicode)
                    return (x.c.Unicode ?? "") == (y.c.Unicode ?? "");
                return x.c.Equals(y.c);
            }

            public override int GetHashCode(IGlyph obj)
            {
                if (obj.c.HasCode)
                    return obj.c.Code;
                if (obj.c.HasUnicode)
                    return obj.c.Code;
                return obj.c.GetHashCode();
            }
        }

        public static int MainConsole()
        {
            Application.EnableVisualStyles();

            var CmdLine = CommandLine.GetCmdLine();

            foreach (var opt in CmdLine.Options)
            {
                switch (opt.Name.ToLower() ?? "")
                {
                    case "?":
                    case "help":
                        {
                            DisplayInfo();
                            return 0;
                        }
                }
            }

            switch (CmdLine.Arguments.Count())
            {
                case 0:
                    {
                        IEnumerable<IGlyph> Glyphs = new Glyph[] { };
                        ChannelPattern[] ChannelPatterns = [ChannelPattern.One, ChannelPattern.Draw, ChannelPattern.Draw, ChannelPattern.Draw];
                        bool EnableDoubleSample = false;
                        bool AnchorLeft = false;
                        int BitPerPixel = 8;
                        int PicWidth = -1;
                        int PicHeight = -1;
                        bool Multiple = false;
                        bool Compact = false;
                        foreach (var opt in CmdLine.Options)
                        {
                            switch (opt.Name.ToLower() ?? "")
                            {
                                case "argb":
                                    {
                                        string[] argv = opt.Arguments;
                                        switch (argv.Length)
                                        {
                                            case 1:
                                                {
                                                    Char32[] s = argv[0].ToLower().ToUTF32();
                                                    if (s.Length != 4)
                                                        throw new ArgumentException(string.Join(",", opt.Arguments));
                                                    for (int n = 0; n <= 3; n++)
                                                    {
                                                        switch (n.ToString())
                                                        {
                                                            case "0":
                                                                {
                                                                    ChannelPatterns[n] = ChannelPattern.Zero;
                                                                    break;
                                                                }
                                                            case "x":
                                                                {
                                                                    ChannelPatterns[n] = ChannelPattern.Draw;
                                                                    break;
                                                                }
                                                            case "1":
                                                                {
                                                                    ChannelPatterns[n] = ChannelPattern.One;
                                                                    break;
                                                                }

                                                            default:
                                                                {
                                                                    throw new ArgumentException(string.Join(",", opt.Arguments));
                                                                }
                                                        }
                                                    }

                                                    break;
                                                }

                                            default:
                                                {
                                                    throw new ArgumentException(string.Join(",", opt.Arguments));
                                                }
                                        }

                                        break;
                                    }
                                case "x2":
                                    {
                                        EnableDoubleSample = true;
                                        break;
                                    }
                                case "left":
                                    {
                                        AnchorLeft = true;
                                        break;
                                    }
                                case "bpp":
                                    {
                                        string[] argv = opt.Arguments;
                                        switch (argv.Length)
                                        {
                                            case 1:
                                                {
                                                    BitPerPixel = int.Parse(argv[0]);
                                                    break;
                                                }

                                            default:
                                                {
                                                    throw new ArgumentException(string.Join(",", opt.Arguments));
                                                }
                                        }

                                        break;
                                    }
                                case "size":
                                    {
                                        string[] argv = opt.Arguments;
                                        switch (argv.Length)
                                        {
                                            case 2:
                                                {
                                                    PicWidth = int.Parse(argv[0]);
                                                    PicHeight = int.Parse(argv[1]);
                                                    break;
                                                }

                                            default:
                                                {
                                                    throw new ArgumentException(string.Join(",", opt.Arguments));
                                                }
                                        }

                                        break;
                                    }
                                case "multiple":
                                    {
                                        Multiple = true;
                                        break;
                                    }
                                case "compact":
                                    {
                                        Compact = true;
                                        break;
                                    }

                                default:
                                    {
                                        string[] argv = opt.Arguments;
                                        switch (opt.Name.ToLower() ?? "")
                                        {
                                            case "add":
                                                {
                                                    switch (argv.Length)
                                                    {
                                                        case 1:
                                                            {
                                                                var g = FdGlyphDescriptionFile.ReadFont(argv[0]);
                                                                Glyphs = Glyphs.Except(g, new GlyphComparer()).Concat(g);
                                                                break;
                                                            }
                                                        case 8:
                                                            {
                                                                var g = GenerateFont(argv[0], argv[1], (FontStyle)Conversions.ToInteger(argv[2]), Conversions.ToInteger(argv[3]), Conversions.ToInteger(argv[4]), Conversions.ToInteger(argv[5]), Conversions.ToInteger(argv[6]), Conversions.ToInteger(argv[7]), 0, 0, 0, 0, EnableDoubleSample, AnchorLeft, ChannelPatterns);
                                                                Glyphs = Glyphs.Except(g, new GlyphComparer()).Concat(g);
                                                                break;
                                                            }
                                                        case 12:
                                                            {
                                                                var g = GenerateFont(argv[0], argv[1], (FontStyle)Conversions.ToInteger(argv[2]), Conversions.ToInteger(argv[3]), Conversions.ToInteger(argv[4]), Conversions.ToInteger(argv[5]), Conversions.ToInteger(argv[6]), Conversions.ToInteger(argv[7]), Conversions.ToInteger(argv[8]), Conversions.ToInteger(argv[9]), Conversions.ToInteger(argv[10]), Conversions.ToInteger(argv[11]), EnableDoubleSample, AnchorLeft, ChannelPatterns);
                                                                Glyphs = Glyphs.Except(g, new GlyphComparer()).Concat(g);
                                                                break;
                                                            }

                                                        default:
                                                            {
                                                                throw new ArgumentException(string.Join(",", opt.Arguments));
                                                            }
                                                    }

                                                    break;
                                                }
                                            case "addnew":
                                                {
                                                    switch (argv.Length)
                                                    {
                                                        case 1:
                                                            {
                                                                var g = FdGlyphDescriptionFile.ReadFont(argv[0]);
                                                                Glyphs = Glyphs.Concat(g.Except(Glyphs, new GlyphComparer()));
                                                                break;
                                                            }
                                                        case 8:
                                                            {
                                                                var g = GenerateFont(argv[0], argv[1], (FontStyle)Conversions.ToInteger(argv[2]), Conversions.ToInteger(argv[3]), Conversions.ToInteger(argv[4]), Conversions.ToInteger(argv[5]), Conversions.ToInteger(argv[6]), Conversions.ToInteger(argv[7]), 0, 0, 0, 0, EnableDoubleSample, AnchorLeft, ChannelPatterns);
                                                                Glyphs = Glyphs.Concat(g.Except(Glyphs, new GlyphComparer()));
                                                                break;
                                                            }
                                                        case 12:
                                                            {
                                                                var g = GenerateFont(argv[0], argv[1], (FontStyle)Conversions.ToInteger(argv[2]), Conversions.ToInteger(argv[3]), Conversions.ToInteger(argv[4]), Conversions.ToInteger(argv[5]), Conversions.ToInteger(argv[6]), Conversions.ToInteger(argv[7]), Conversions.ToInteger(argv[8]), Conversions.ToInteger(argv[9]), Conversions.ToInteger(argv[10]), Conversions.ToInteger(argv[11]), EnableDoubleSample, AnchorLeft, ChannelPatterns);
                                                                Glyphs = Glyphs.Concat(g.Except(Glyphs, new GlyphComparer()));
                                                                break;
                                                            }

                                                        default:
                                                            {
                                                                throw new ArgumentException(string.Join(",", opt.Arguments));
                                                            }
                                                    }

                                                    break;
                                                }
                                            case "removeunicode":
                                                {
                                                    switch (argv.Length)
                                                    {
                                                        case 2:
                                                            {
                                                                int l = int.Parse(argv[0], System.Globalization.NumberStyles.HexNumber);
                                                                int u = int.Parse(argv[1], System.Globalization.NumberStyles.HexNumber);
                                                                Glyphs = Glyphs.Where(g => !g.c.HasUnicode || Conversions.ToDouble(g.c.Unicode) < l || Conversions.ToDouble(g.c.Unicode) > u);
                                                                break;
                                                            }

                                                        default:
                                                            {
                                                                throw new ArgumentException(string.Join(",", opt.Arguments));
                                                            }
                                                    }

                                                    break;
                                                }
                                            case "removecode":
                                                {
                                                    switch (argv.Length)
                                                    {
                                                        case 2:
                                                            {
                                                                int l = int.Parse(argv[0], System.Globalization.NumberStyles.HexNumber);
                                                                int u = int.Parse(argv[1], System.Globalization.NumberStyles.HexNumber);
                                                                Glyphs = Glyphs.Where(g => !g.c.HasCode || g.c.Code < l || g.c.Code > u);
                                                                break;
                                                            }

                                                        default:
                                                            {
                                                                throw new ArgumentException(string.Join(",", opt.Arguments));
                                                            }
                                                    }

                                                    break;
                                                }
                                            case "save":
                                                {
                                                    switch (argv.Length)
                                                    {
                                                        case 1:
                                                            {
                                                                SaveFont(Glyphs, argv[0], PicWidth, PicHeight, BitPerPixel, Multiple, Compact);
                                                                break;
                                                            }

                                                        default:
                                                            {
                                                                throw new ArgumentException(string.Join(",", opt.Arguments));
                                                            }
                                                    }

                                                    break;
                                                }

                                            default:
                                                {
                                                    throw new ArgumentException(opt.Name);
                                                }
                                        }
                                        ChannelPatterns = [ChannelPattern.One, ChannelPattern.Draw, ChannelPattern.Draw, ChannelPattern.Draw];
                                        EnableDoubleSample = false;
                                        AnchorLeft = false;
                                        BitPerPixel = 8;
                                        PicWidth = -1;
                                        PicHeight = -1;
                                        Multiple = false;
                                        Compact = false;
                                        break;
                                    }
                            }
                        }

                        break;
                    }
                case 9:
                case 13:
                case 15:
                    {
                        string[] argv = CmdLine.Arguments;
                        ChannelPattern[] ChannelPatterns = [ChannelPattern.One, ChannelPattern.Draw, ChannelPattern.Draw, ChannelPattern.Draw];
                        bool EnableDoubleSample = false;
                        bool AnchorLeft = false;
                        foreach (var opt in CmdLine.Options)
                        {
                            switch (opt.Name.ToLower() ?? "")
                            {
                                case "x2":
                                    {
                                        EnableDoubleSample = true;
                                        break;
                                    }
                                case "left":
                                    {
                                        AnchorLeft = true;
                                        break;
                                    }

                                default:
                                    {
                                        throw new ArgumentException();
                                    }
                            }
                        }
                        switch (argv.Count())
                        {
                            case 9:
                                {
                                    SaveFont(GenerateFont(argv[0], argv[2], (FontStyle)Conversions.ToInteger(argv[3]), Conversions.ToInteger(argv[4]), Conversions.ToInteger(argv[5]), Conversions.ToInteger(argv[6]), Conversions.ToInteger(argv[7]), Conversions.ToInteger(argv[8]), 0, 0, 0, 0, EnableDoubleSample, AnchorLeft, ChannelPatterns), argv[1], -1, -1, 8, false, false);
                                    break;
                                }
                            case 13:
                                {
                                    SaveFont(GenerateFont(argv[0], argv[2], (FontStyle)Conversions.ToInteger(argv[3]), Conversions.ToInteger(argv[4]), Conversions.ToInteger(argv[5]), Conversions.ToInteger(argv[6]), Conversions.ToInteger(argv[7]), Conversions.ToInteger(argv[8]), Conversions.ToInteger(argv[9]), Conversions.ToInteger(argv[10]), Conversions.ToInteger(argv[11]), Conversions.ToInteger(argv[12]), EnableDoubleSample, AnchorLeft, ChannelPatterns), argv[1], -1, -1, 8, false, false);
                                    break;
                                }
                            case 15:
                                {
                                    SaveFont(GenerateFont(argv[0], argv[2], (FontStyle)Conversions.ToInteger(argv[3]), Conversions.ToInteger(argv[4]), Conversions.ToInteger(argv[5]), Conversions.ToInteger(argv[6]), Conversions.ToInteger(argv[7]), Conversions.ToInteger(argv[8]), Conversions.ToInteger(argv[9]), Conversions.ToInteger(argv[10]), Conversions.ToInteger(argv[11]), Conversions.ToInteger(argv[12]), EnableDoubleSample, AnchorLeft, ChannelPatterns), argv[1], Conversions.ToInteger(argv[13]), Conversions.ToInteger(argv[14]), 8, true, false);
                                    break;
                                }
                        }

                        break;
                    }

                default:
                    {
                        DisplayInfo();
                        return -1;
                    }
            }
            return 0;
        }

        public static IEnumerable<IGlyph> GenerateFont(string SourcePath, string FontName, FontStyle FontStyle, int FontSize, int PhysicalWidth, int PhysicalHeight, int DrawOffsetX, int DrawOffsetY, int VirtualOffsetX, int VirtualOffsetY, int VirtualDeltaWidth, int VirtualDeltaHeight, bool EnableDoubleSample, bool AnchorLeft, ChannelPattern[] ChannelPatterns)
        {
            StringCode[] StringCodes;

            string Ext = FileNameHandling.GetExtendedFileName(SourcePath);
            if (Ext.Equals("tbl", StringComparison.OrdinalIgnoreCase))
            {
                StringCodes = TblCharMappingFile.ReadFile(SourcePath).ToArray();
            }
            else if (Ext.Equals("fd", StringComparison.OrdinalIgnoreCase))
            {
                StringCodes = (from d in FdGlyphDescriptionFile.ReadFile(SourcePath)
                               select d.c).ToArray();
            }
            else if (Ext.Equals("txt", StringComparison.OrdinalIgnoreCase))
            {
                StringCodes = (from c in Txt.ReadFile(SourcePath).ToUTF32()
                               select StringCode.FromUniChar(c)).ToArray();
            }
            else
            {
                throw new InvalidDataException();
            }

            IGlyphProvider gg;
            if (EnableDoubleSample)
            {
                gg = new GlyphGeneratorDoubleSample(FontName, FontStyle, FontSize, PhysicalWidth, PhysicalHeight, DrawOffsetX, DrawOffsetY, VirtualOffsetX, VirtualOffsetY, VirtualDeltaWidth, VirtualDeltaHeight, AnchorLeft, ChannelPatterns);
            }
            else
            {
                gg = new GlyphGenerator(FontName, FontStyle, FontSize, PhysicalWidth, PhysicalHeight, DrawOffsetX, DrawOffsetY, VirtualOffsetX, VirtualOffsetY, VirtualDeltaWidth, VirtualDeltaHeight, AnchorLeft, ChannelPatterns);
            }

            using (gg)
                return (from c in StringCodes
                        select gg.GetGlyph(c)).ToArray();
        }

        public static void SaveFont(IEnumerable<IGlyph> Glyphs, string TargetPath, int PicWidth, int PicHeight, int BitPerPixel, bool Multiple, bool Compact)
        {
            IGlyph[] gl = Glyphs.ToArray();
            int PhysicalWidth = (from g in gl
                                 select g.PhysicalWidth).Max();
            int PhysicalHeight = (from g in gl
                                  select g.PhysicalHeight).Max();
            IGlyphArranger ga;
            if (Compact)
            {
                ga = new GlyphArrangerCompact(PhysicalWidth, PhysicalHeight);
            }
            else
            {
                ga = new GlyphArranger(PhysicalWidth, PhysicalHeight);
            }
            if (PicWidth < 0 || PicHeight < 0)
            {
                var Size = ga.GetPreferredSize(gl);
                PicWidth = Size.Width;
                PicHeight = Size.Height;
            }

            if (Multiple)
            {
                int n = 0;
                int GlyphIndex = 0;
                while (GlyphIndex < gl.Length)
                {
                    string FdPath = FileNameHandling.ChangeExtension(TargetPath, "{0}.{1}".Formats(n, FileNameHandling.GetExtendedFileName(TargetPath)));
                    var PartGlyphDescriptors = ga.GetGlyphArrangement(gl, PicWidth, PicHeight);
                    GlyphDescriptor[] pgd = PartGlyphDescriptors.ToArray();
                    if (pgd.Length == 0)
                        throw new InvalidDataException("PicSizeTooSmallForGlyphOfChar:{0}".Formats(gl[GlyphIndex].c.ToString()));
                    IGlyph[] pgl = gl.SubArray(GlyphIndex, pgd.Length);
                    FdGlyphDescriptionFile.WriteFont(FdPath, TextEncoding.WritingDefault, pgl, pgd, new BmpFontImageWriter(FileNameHandling.ChangeExtension(FdPath, "bmp"), BitPerPixel), PicWidth, PicHeight);
                    GlyphIndex += pgd.Length;
                }
            }
            else
            {
                FdGlyphDescriptionFile.WriteFont(TargetPath, TextEncoding.WritingDefault, gl, new BmpFontImageWriter(FileNameHandling.ChangeExtension(TargetPath, "bmp"), BitPerPixel), ga, PicWidth, PicHeight);
            }
        }


        private bool Initialized = false;
        private void ReDraw()
        {
            if (!Initialized)
                return;
            // Try
            var Image = new Bitmap(PictureBox_Preview.Width, PictureBox_Preview.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var Image2x = new Bitmap(PictureBox_Preview2x.Width, PictureBox_Preview2x.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            // Try
            using (var g = Graphics.FromImage(Image))
            {
                using (var g2x = Graphics.FromImage(Image2x))
                {
                    g.Clear(Color.White);
                    g2x.Clear(Color.LightGray);

                    var Style = FontStyle.Regular;
                    if (CheckBox_Bold.Checked)
                        Style = Style | FontStyle.Bold;
                    if (CheckBox_Italic.Checked)
                        Style = Style | FontStyle.Italic;
                    if (CheckBox_Underline.Checked)
                        Style = Style | FontStyle.Underline;
                    if (CheckBox_Strikeout.Checked)
                        Style = Style | FontStyle.Strikeout;
                    int PhysicalWidth = (int)Math.Round(NumericUpDown_PhysicalWidth.Value);
                    int PhysicalHeight = (int)Math.Round(NumericUpDown_PhysicalHeight.Value);
                    bool EnableDoubleSample = CheckBox_DoubleSample.Checked;
                    bool AnchorLeft = CheckBox_AnchorLeft.Checked;
                    ChannelPattern[] ChannelPatterns = [ChannelPattern.One, ChannelPattern.Draw, ChannelPattern.Draw, ChannelPattern.Draw];
                    IGlyphProvider gg;
                    if (EnableDoubleSample)
                    {
                        gg = new GlyphGeneratorDoubleSample(ComboBox_FontName.Text, Style, (int)Math.Round(NumericUpDown_Size.Value), PhysicalWidth, PhysicalHeight, (int)Math.Round(NumericUpDown_DrawOffsetX.Value), (int)Math.Round(NumericUpDown_DrawOffsetY.Value), (int)Math.Round(NumericUpDown_VirtualOffsetX.Value), (int)Math.Round(NumericUpDown_VirtualOffsetY.Value), (int)Math.Round(NumericUpDown_VirtualDeltaWidth.Value), (int)Math.Round(NumericUpDown_VirtualDeltaHeight.Value), AnchorLeft, ChannelPatterns);
                    }
                    else
                    {
                        gg = new GlyphGenerator(ComboBox_FontName.Text, Style, (int)Math.Round(NumericUpDown_Size.Value), PhysicalWidth, PhysicalHeight, (int)Math.Round(NumericUpDown_DrawOffsetX.Value), (int)Math.Round(NumericUpDown_DrawOffsetY.Value), (int)Math.Round(NumericUpDown_VirtualOffsetX.Value), (int)Math.Round(NumericUpDown_VirtualOffsetY.Value), (int)Math.Round(NumericUpDown_VirtualDeltaWidth.Value), (int)Math.Round(NumericUpDown_VirtualDeltaHeight.Value), AnchorLeft, ChannelPatterns);
                    }

                    using (gg)
                    {
                        string[] TestStrings = ["012 AaBbCc", @"!""#$%&'()*+,-./:;<=>?@[\]^_`", "珍爱生命　远离汉化", "これはテストォーー！", "يادداشت هاي شخصي احمدي نژاد"];

                        using (var b = new Bmp(PhysicalWidth, PhysicalHeight, 32))
                        {
                            using (var b2x = new Bmp(PhysicalWidth * 2, PhysicalHeight * 2, 32))
                            {
                                int[,] Block2x = new int[(PhysicalWidth * 2), (PhysicalHeight * 2)];
                                int l = 0;
                                foreach (var t in TestStrings)
                                {
                                    int k = 0;
                                    foreach (var c in t.ToUTF32())
                                    {
                                        int x = k * (PhysicalWidth + 4);
                                        int y = l * (PhysicalHeight + 4);
                                        var PhysicalRect = new Rectangle(x, y, PhysicalWidth, PhysicalHeight);
                                        var glyph = gg.GetGlyph(StringCode.FromUniChar(c));
                                        var VirtualRect = glyph.VirtualBox;
                                        int[,] Block = glyph.Block;
                                        for (int y0 = 0, loopTo = PhysicalHeight - 1; y0 <= loopTo; y0++)
                                        {
                                            for (int x0 = 0, loopTo1 = PhysicalWidth - 1; x0 <= loopTo1; x0++)
                                                Block[x0, y0] = Block[x0, y0] ^ 0xFFFFFF;
                                        }
                                        b.SetRectangle(0, 0, Block);
                                        using (var bb = b.ToBitmap())
                                        {
                                            g.DrawImage(bb, PhysicalRect);
                                        }

                                        var PhysicalRect2x = new Rectangle(x * 2, y * 2, PhysicalWidth * 2, PhysicalHeight * 2);
                                        for (int y0 = 0, loopTo2 = PhysicalHeight - 1; y0 <= loopTo2; y0++)
                                        {
                                            for (int x0 = 0, loopTo3 = PhysicalWidth - 1; x0 <= loopTo3; x0++)
                                            {
                                                Block2x[x0 * 2, y0 * 2] = Block[x0, y0];
                                                Block2x[x0 * 2 + 1, y0 * 2] = Block[x0, y0];
                                                Block2x[x0 * 2, y0 * 2 + 1] = Block[x0, y0];
                                                Block2x[x0 * 2 + 1, y0 * 2 + 1] = Block[x0, y0];
                                            }
                                        }
                                        b2x.SetRectangle(0, 0, Block2x);
                                        using (var bb2x = b2x.ToBitmap())
                                        {
                                            g2x.DrawImage(bb2x, PhysicalRect2x);
                                        }

                                        g2x.DrawRectangle(Pens.Red, new Rectangle(x * 2 + VirtualRect.X * 2, y * 2 + VirtualRect.Y * 2, VirtualRect.Width * 2 - 1, VirtualRect.Height * 2 - 1));

                                        k += 1;
                                    }
                                    l += 1;
                                }
                            }
                        }
                    }
                }
            }
            // Catch
            // End Try
            PictureBox_Preview.Image = Image;
            PictureBox_Preview2x.Image = Image2x;
            PictureBox_Preview.Invalidate();
            PictureBox_Preview2x.Invalidate();
            // Catch
            // End Try
        }

        private void FontGen_Shown(object sender, EventArgs e)
        {
            Initialized = true;
            ComboBox_FontName.Items.AddRange((from f in FontFamily.Families
                                              select f.Name).ToArray());
            ReDraw();
        }
        private void ComboBox_FontName_TextChanged(object sender, EventArgs e)
        {
            ReDraw();
        }
        private void NumericUpDown_Size_ValueChanged(object sender, EventArgs e)
        {
            ReDraw();
        }
        private void CheckBox_Bold_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }
        private void CheckBox_Italic_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }
        private void CheckBox_Underline_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }
        private void CheckBox_Strikeout_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }
        private void CheckBox_DoubleSample_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }
        private void NumericUpDowns_ValueChanged(object sender, EventArgs e)
        {
            ReDraw();
        }
        private void FontGen_SizeChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private string Esc(string Parameter)
        {
            if (string.IsNullOrEmpty(Parameter))
                return "\"\"";
            if (Parameter.Contains(" "))
                return "\"" + Parameter + "\"";
            if (Parameter.Contains("　"))
                return "\"" + Parameter + "\"";
            return Parameter;
        }

        private string FormatEsc(string Format, params object[] args)
        {
            return Format.Formats((from arg in args
                                   select Esc(arg.ToString())).ToArray());
        }

        private void Button_CmdToClipboard_Click(object sender, EventArgs e)
        {
            var Style = FontStyle.Regular;
            if (CheckBox_Bold.Checked)
                Style = Style | FontStyle.Bold;
            if (CheckBox_Italic.Checked)
                Style = Style | FontStyle.Italic;
            if (CheckBox_Underline.Checked)
                Style = Style | FontStyle.Underline;
            if (CheckBox_Strikeout.Checked)
                Style = Style | FontStyle.Strikeout;
            int PhysicalWidth = (int)Math.Round(NumericUpDown_PhysicalWidth.Value);
            int PhysicalHeight = (int)Math.Round(NumericUpDown_PhysicalHeight.Value);
            var Options = new List<string>();
            if (CheckBox_DoubleSample.Checked)
                Options.Add("/x2");
            if (CheckBox_AnchorLeft.Checked)
                Options.Add("/left");
            string[] AddParameters = [FileNameHandling.GetFileName(FileSelectBox_File.Path), ComboBox_FontName.Text, ((int)Style).ToString(), NumericUpDown_Size.Value.ToString(), PhysicalWidth.ToString(), PhysicalHeight.ToString(), NumericUpDown_DrawOffsetX.Value.ToString(), NumericUpDown_DrawOffsetY.Value.ToString(), NumericUpDown_VirtualOffsetX.Value.ToString(), NumericUpDown_VirtualOffsetY.Value.ToString(), NumericUpDown_VirtualDeltaWidth.Value.ToString(), NumericUpDown_VirtualDeltaHeight.Value.ToString()];
            Options.Add("/add:" + string.Join(",", (from p in AddParameters
                                                    select Esc(p)).ToArray()));
            Options.Add("/save:" + Esc(FileNameHandling.ChangeExtension(FileNameHandling.GetFileName(FileSelectBox_File.Path), "fd")));
            string Cmd = FormatEsc("FontGen " + string.Join(" ", Options.ToArray()));
            Clipboard.SetText(Cmd);
            MessageBox.Show(Cmd, Text);
        }

        private void Button_Generate_Click(object sender, EventArgs e)
        {
            var Style = FontStyle.Regular;
            if (CheckBox_Bold.Checked)
                Style = Style | FontStyle.Bold;
            if (CheckBox_Italic.Checked)
                Style = Style | FontStyle.Italic;
            if (CheckBox_Underline.Checked)
                Style = Style | FontStyle.Underline;
            if (CheckBox_Strikeout.Checked)
                Style = Style | FontStyle.Strikeout;
            int PhysicalWidth = (int)Math.Round(NumericUpDown_PhysicalWidth.Value);
            int PhysicalHeight = (int)Math.Round(NumericUpDown_PhysicalHeight.Value);
            bool EnableDoubleSample = CheckBox_DoubleSample.Checked;
            bool AnchorLeft = CheckBox_AnchorLeft.Checked;
            ChannelPattern[] ChannelPatterns = [ChannelPattern.One, ChannelPattern.Draw, ChannelPattern.Draw, ChannelPattern.Draw];

            SaveFont(GenerateFont(FileSelectBox_File.Path, ComboBox_FontName.Text, Style, (int)Math.Round(NumericUpDown_Size.Value), PhysicalWidth, PhysicalHeight, (int)Math.Round(NumericUpDown_DrawOffsetX.Value), (int)Math.Round(NumericUpDown_DrawOffsetY.Value), (int)Math.Round(NumericUpDown_VirtualOffsetX.Value), (int)Math.Round(NumericUpDown_VirtualOffsetY.Value), (int)Math.Round(NumericUpDown_VirtualDeltaWidth.Value), (int)Math.Round(NumericUpDown_VirtualDeltaHeight.Value), EnableDoubleSample, AnchorLeft, ChannelPatterns), FileNameHandling.ChangeExtension(FileSelectBox_File.Path, "fd"), -1, -1, 8, false, false);
            MessageBox.Show("生成完毕。", Text);
        }
    }
}