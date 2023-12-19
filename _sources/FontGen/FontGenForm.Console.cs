using Firefly.Glyphing;
using Firefly.TextEncoding;
using Firefly;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace FontGen
{
    public partial class FontGenForm
    {
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

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        public static void DisplayInfo()
        {
            Console.WriteLine("Font Image Generator");
            Console.WriteLine("Firefly.FontGen, distributed under BSD license");
            Console.WriteLine("F.R.C.");
            Console.WriteLine("");
            Console.WriteLine("This generator is used to generate font images and corresponding fd font description files from tbl encoding files, fd font description files or character files.");
            Console.WriteLine("");
            Console.WriteLine("Usage:");
            Console.WriteLine("FontGen <Source tbl/fd/CharFile> <Target fd> <FontName> <FontStyle> <FontSize> <PhysicalWidth> <PhysicalHeight> <DrawOffsetX> <DrawOffsetY> [<VirtualOffsetX> <VirtualOffsetY> <VirtualDeltaWidth> < VirtualDeltaHeight> [<PicWidth> <PicHeight>]] [/x2] [/left]");
            Console.WriteLine("Source tbl/fd/CharFile Enter the tbl encoding file, fd font description file or character file path.");
            Console.WriteLine("Target fd outputs the fd font description file path. The output font image (bmp) also uses this path.");
            Console.WriteLine("FontName font name.");
            Console.WriteLine("FontStyle font style, bold 1 italic 2 underline 4 strikethrough 8, can be superimposed.");
            Console.WriteLine("FontSize font size.");
            Console.WriteLine("PhysicalWidth physical width, character grid width.");
            Console.WriteLine("PhysicalHeight physical height, character grid height.");
            Console.WriteLine("DrawOffsetX draws X offset.");
            Console.WriteLine("DrawOffsetY draws Y offset.");
            Console.WriteLine("VirtualOffsetX virtual X offset, X offset of the displayed part of the character.");
            Console.WriteLine("VirtualOffsetY virtual Y offset, Y offset of the displayed part of the character.");
            Console.WriteLine("VirtualDeltaWidth virtual width difference, the difference between the width of the displayed part of the character and the default value.");
            Console.WriteLine("VirtualDeltaHeight virtual height difference, the difference between the height of the displayed part of the character and the default value.");
            Console.WriteLine("PicWidth picture width.");
            Console.WriteLine("PicHeight picture height.");
            Console.WriteLine("If you do not specify the image width and height, the smallest power of 2 width and height that can accommodate all characters will be automatically selected.");
            Console.WriteLine("If you specify the image width and height, multiple images will be generated.");
            Console.WriteLine("/x2 2x supersampling.");
            Console.WriteLine("/left align left.");
            Console.WriteLine("");
            Console.WriteLine("Example:");
            Console.WriteLine("FontGen FakeShiftJIS.tbl FakeShiftJIS.fd 宋体 0 16 16 16 0 0");
            Console.WriteLine("FontGen FakeShiftJIS.tbl FakeShiftJIS.fd 宋体 0 16 16 16 0 0 0 0 0 0 1024 1024");
            Console.WriteLine("");
            Console.WriteLine("Advanced usage:");
            Console.WriteLine("FontGen (Add|AddNew|RemoveUnicode|RemoveCode|Save)*");
            Console.WriteLine("Add ::= [/x2] [/left] [/argb:<Pattern>=1xxx] /add:<Source tbl/fd/CharFile>[,<FontName>,<FontStyle>,<FontSize >,<PhysicalWidth>,<PhysicalHeight>,<DrawOffsetX>,<DrawOffsetY>[,<VirtualOffsetX>,<VirtualOffsetY>,<VirtualDeltaWidth>,<VirtualDeltaHeight>]]");
            Console.WriteLine("AddNew ::= [/x2] [/left] [/argb:<Pattern>=1xxx] /addnew:<Source tbl/fd/CharFile>[,<FontName>,<FontStyle>,<FontSize >,<PhysicalWidth>,<PhysicalHeight>,<DrawOffsetX>,<DrawOffsetY>[,<VirtualOffsetX>,<VirtualOffsetY>,<VirtualDeltaWidth>,<VirtualDeltaHeight>]]");
            Console.WriteLine("RemoveUnicode ::= /removeunicode:<Lower:Hex>,<Upper:Hex>");
            Console.WriteLine("RemoveCode ::= /removecode:<Lower:Hex>,<Upper:Hex>");
            Console.WriteLine("Save ::= [/bpp:<BitPerPixel>=8] [/size:<PicWidth>,<PicHeight>] [/multiple] [/compact] /save:<Target fd>");
            Console.WriteLine("/argb specifies the color form");
            Console.WriteLine("Pattern Pattern consists of 4 bits, corresponding to the A, R, G, and B channels respectively. Each bit can be 0, 1 or x, where 0 represents 0, 1 represents the maximum value, and x represents the drawing value ");
            Console.WriteLine("/add adds glyph source, you can specify parameters to be generated by this program, or specified to be loaded from fd file");
            Console.WriteLine("/addnew adds a glyph source, but only if the character does not exist");
            Console.WriteLine("/removeunicode removes the glyphs of characters within the Unicode range (including both boundaries). The Unicode range includes the extended plane");
            Console.WriteLine("/removecode removes the glyphs of characters within the encoding range (including both boundaries)");
            Console.WriteLine("/bpp specifies bit depth");
            Console.WriteLine("BitPerPixel bit depth: 1, 2, 4, 8, 16, 32");
            Console.WriteLine("/size specifies the image size");
            Console.WriteLine("PicWidth picture width");
            Console.WriteLine("PicHeight picture height");
            Console.WriteLine("/multiple specifies saving as multiple files");
            Console.WriteLine("/compact compact storage, columns are not aligned");
            Console.WriteLine("/save save glyphs to fd file");
            Console.WriteLine("");
            Console.WriteLine("Example:");
            Console.WriteLine("FontGen /add:Original.fd /removecode:100,10000 /x2 /left /addnew:FakeShiftJIS.tbl,宋体,0,16,16,16,0,0 /save:FakeShiftJIS.fd");
            Console.WriteLine("This example shows: load the font library from Original.fd, delete the part from 0x100 to 0x10000, then generate glyphs from FakeShiftJIS.tbl, add the new glyphs, and save the result to FakeShiftJIS.fd");
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
    }
}
