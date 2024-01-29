// ==========================================================================
// 
// File:        FontGen.vb
// Location:    Firefly.FontGen <Visual Basic .Net>
// Description: Font image generator
// Version:     2010.04.08.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Firefly;
using Firefly.Glyphing;
using Firefly.Imaging;
using Firefly.TextEncoding;
using Firefly.Texting;

[assembly: AssemblyTitle("Fonts generator for the X-Ray Engine")]
[assembly: AssemblyDescription("Fonts generator for the X-Ray Engine")]
[assembly: AssemblyCompany("OGSR")]
[assembly: AssemblyProduct("Fonts Generator")]
[assembly: AssemblyCopyright("OGSR © 2024")]
[assembly: AssemblyVersion("1.0")]

namespace FontGen
{
    public partial class FontGenForm
    {
        PrivateFontCollection Collection = new PrivateFontCollection();

        static string[] _testStrings = [
            "йцукенгшщзхъфывапролджэячсмитьбюё",
            "ЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮЁ",
            "qwertyuiop[]asdfghjklzxcvbnm",
            "QWERTYUIOP{}ASDFGHJKLZXCVBNM",
            "ҐґЄєІіЇї",
            " 1234567890-=/*,!\"№;%:?()_.`|'\\+~@#$^&<>",
        ];

        public FontGenForm()
        {
            InitializeComponent();
        }

        public static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ExceptionHandler.PopupException(e.Exception, new StackTrace(4, true));
        }

        [STAThread]
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
            Application.Run(new FontGenForm());
            return 0;
        }

        public static IEnumerable<IGlyph> GenerateFont(string sourcePath, string fontName, FontStyle fontStyle, int fontSize, int physicalWidth, int physicalHeight, int drawOffsetX, int drawOffsetY, int virtualOffsetX, int virtualOffsetY, int virtualDeltaWidth, int virtualDeltaHeight, bool enableDoubleSample, bool anchorLeft, ChannelPattern[] channelPatterns)
        {
            StringCode[] StringCodes;

            if (!string.IsNullOrWhiteSpace(sourcePath) && File.Exists(sourcePath))
            {
                string Ext = FileNameHandling.GetExtendedFileName(sourcePath);
                if (Ext.Equals("tbl", StringComparison.OrdinalIgnoreCase))
                {
                    StringCodes = TblCharMappingFile.ReadFile(sourcePath).ToArray();
                }
                else if (Ext.Equals("fd", StringComparison.OrdinalIgnoreCase))
                {
                    StringCodes = (from d in FdGlyphDescriptionFile.ReadFile(sourcePath)
                                   select d.c).ToArray();
                }
                else if (Ext.Equals("txt", StringComparison.OrdinalIgnoreCase))
                {
                    StringCodes = (from c in Txt.ReadFile(sourcePath).ToUTF32()
                                   select StringCode.FromUniChar(c)).ToArray();
                }
                else
                {
                    throw new InvalidDataException();
                }
            }
            else
            {
                StringCodes = _testStrings.SelectMany(l => l.ToUTF32()).Select(c => StringCode.FromUniChar(c))
                    .Distinct()
                    .ToArray();
            }

            IGlyphProvider gg;
            if (enableDoubleSample)
            {
                gg = new GlyphGeneratorDoubleSample(
                    fontName,
                    fontStyle,
                    fontSize,
                    physicalWidth,
                    physicalHeight,
                    drawOffsetX,
                    drawOffsetY,
                    virtualOffsetX,
                    virtualOffsetY,
                    virtualDeltaWidth,
                    virtualDeltaHeight,
                    anchorLeft,
                    channelPatterns);
            }
            else
            {
                gg = new GlyphGenerator(
                    fontName,
                    fontStyle,
                    fontSize,
                    physicalWidth,
                    physicalHeight,
                    drawOffsetX,
                    drawOffsetY,
                    virtualOffsetX,
                    virtualOffsetY,
                    virtualDeltaWidth,
                    virtualDeltaHeight,
                    anchorLeft,
                    channelPatterns);
            }

            using (gg)
                return (from c in StringCodes
                        select gg.GetGlyph(c)).ToArray();
        }

        public static void SaveFont(IEnumerable<IGlyph> glyphs, string targetPath, int picWidth, int picHeight, int bitPerPixel, bool multiple, bool compact)
        {
            IGlyph[] gl = glyphs.ToArray();
            int PhysicalWidth = (from g in gl
                                 select g.PhysicalWidth).Max();
            int PhysicalHeight = (from g in gl
                                  select g.PhysicalHeight).Max();
            IGlyphArranger ga;
            if (compact)
            {
                ga = new GlyphArrangerCompact(PhysicalWidth, PhysicalHeight);
            }
            else
            {
                ga = new GlyphArranger(PhysicalWidth, PhysicalHeight);
            }

            if (picWidth < 0 || picHeight < 0)
            {
                var Size = ga.GetPreferredSize(gl);
                picWidth = Size.Width;
                picHeight = ga.GetPreferredHeight(gl, Size.Width);
            }

            if (multiple)
            {
                int n = 0;
                int GlyphIndex = 0;
                while (GlyphIndex < gl.Length)
                {
                    string FdPath = FileNameHandling.ChangeExtension(targetPath, "{0}.{1}".Formats(n, FileNameHandling.GetExtendedFileName(targetPath)));
                    var PartGlyphDescriptors = ga.GetGlyphArrangement(gl, picWidth, picHeight);
                    GlyphDescriptor[] pgd = PartGlyphDescriptors.ToArray();
                    if (pgd.Length == 0)
                        throw new InvalidDataException("PicSizeTooSmallForGlyphOfChar:{0}".Formats(gl[GlyphIndex].c.ToString()));
                    IGlyph[] pgl = gl.SubArray(GlyphIndex, pgd.Length);
                    using (BmpFontImageWriter imageWriter = new BmpFontImageWriter(FileNameHandling.ChangeExtension(FdPath, "bmp"), bitPerPixel))
                    {
                        FdGlyphDescriptionFile.WriteFont(FdPath, TextEncoding.WritingDefault, pgl, pgd, imageWriter, picWidth, picHeight);
                    }
                    GlyphIndex += pgd.Length;
                }
            }
            else
            {
                using (BmpFontImageWriter imageWriter = new BmpFontImageWriter(FileNameHandling.ChangeExtension(targetPath, "bmp"), bitPerPixel))
                {
                    FdGlyphDescriptionFile.WriteFont(targetPath, TextEncoding.WritingDefault, gl, imageWriter, ga, picWidth, picHeight);
                }
            }
        }

        private bool Initialized;

        private void ReDraw()
        {
            if (!Initialized)
                return;

            int PhysicalWidth = (int)Math.Round(NumericUpDown_PhysicalWidth.Value);
            int PhysicalHeight = (int)Math.Round(NumericUpDown_PhysicalHeight.Value);

            // Try
            var Image = new Bitmap(PhysicalWidth * 64 + 32 * 8, PhysicalHeight * 32 + 32 * 8, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var image2X = new Bitmap(PhysicalWidth * 64 * 2 + 32 * 8 * 2, PhysicalHeight * 32 * 2 + 32 * 8 * 2, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            // Try
            using (var g = Graphics.FromImage(Image))
            using (var g2X = Graphics.FromImage(image2X))
            {
                g.Clear(Color.White);
                g2X.Clear(Color.LightGray);

                var (lines, glyphs) = GetChars(PhysicalWidth, PhysicalHeight);

                List<(int y0, int x0)> range = Enumerable
                    .Range(0, PhysicalHeight)
                    .SelectMany(y0 => Enumerable.Range(0, PhysicalWidth).Select(x0 => (y0, x0)))
                    .ToList();

                using (var b = new Bmp(PhysicalWidth, PhysicalHeight, 32))
                using (var b2X = new Bmp(PhysicalWidth * 2, PhysicalHeight * 2, 32))
                {
                    int[,] block2X = new int[(PhysicalWidth * 2), (PhysicalHeight * 2)];
                    //int[,] Block = new int[(PhysicalWidth), (PhysicalHeight)];

                    int gIndex = 0;

                    int l = 0;
                    foreach (var t in lines)
                    {
                        int k = 0;
                        foreach (Char32 c in t)
                        {
                            IGlyph glyph = glyphs[gIndex++];

                            int x = k * (PhysicalWidth + 4);
                            int y = l * (PhysicalHeight + 4);

                            int[,] Block = glyph.Block;

                            Parallel.ForEach(range, pair =>
                            {
                                var x0 = pair.x0;
                                var y0 = pair.y0;

                                Block[x0, y0] = Block[x0, y0] ^ 0xFFFFFF;

                                if (!glyph.IsValid)
                                {
                                    Block[x0, y0] = Color.Red.ToArgb();
                                }
                            });

                            //for (int y0 = 0; y0 <= PhysicalHeight - 1; y0++)                                
                            //for (int x0 = 0; x0 <= PhysicalWidth - 1; x0++)
                            //{
                            //    Block[x0, y0] = Block[x0, y0] ^ 0xFFFFFF;

                            //    if (!glyph.IsValid)
                            //    {
                            //        //int ARGB = BitOperations.ConcatBits(GetChannel(ChannelPatterns[0], L), 8, GetChannel(ChannelPatterns[1], L), 8, GetChannel(ChannelPatterns[2], L), 8, GetChannel(ChannelPatterns[3], L), 8);

                            //        Block[x0, y0] = Color.Red.ToArgb();
                            //    }
                            //}                                

                            b.SetRectangle(0, 0, Block);
                            using (var bb = b.ToBitmap())
                            {
                                var PhysicalRect = new Rectangle(x, y, PhysicalWidth, PhysicalHeight);
                                g.DrawImage(bb, PhysicalRect);
                            }

                            Parallel.ForEach(range, pair =>
                            {
                                var x0 = pair.x0;
                                var y0 = pair.y0;

                                block2X[x0 * 2, y0 * 2] = Block[x0, y0];
                                block2X[x0 * 2 + 1, y0 * 2] = Block[x0, y0];
                                block2X[x0 * 2, y0 * 2 + 1] = Block[x0, y0];
                                block2X[x0 * 2 + 1, y0 * 2 + 1] = Block[x0, y0];
                            });

                            //for (int y0 = 0; y0 <= PhysicalHeight - 1; y0++)                                
                            //for (int x0 = 0; x0 <= PhysicalWidth - 1; x0++)
                            //{
                            //    Block2x[x0 * 2, y0 * 2] = Block[x0, y0];
                            //    Block2x[x0 * 2 + 1, y0 * 2] = Block[x0, y0];
                            //    Block2x[x0 * 2, y0 * 2 + 1] = Block[x0, y0];
                            //    Block2x[x0 * 2 + 1, y0 * 2 + 1] = Block[x0, y0];
                            //}                                

                            b2X.SetRectangle(0, 0, block2X);
                            using (var bb2X = b2X.ToBitmap())
                            {
                                var physicalRect2X = new Rectangle(x * 2, y * 2, PhysicalWidth * 2, PhysicalHeight * 2);
                                g2X.DrawImage(bb2X, physicalRect2X);
                            }

                            var VirtualRect = glyph.VirtualBox;
                            g2X.DrawRectangle(Pens.Red, new Rectangle(x * 2 + VirtualRect.X * 2, y * 2 + VirtualRect.Y * 2, VirtualRect.Width * 2 - 1, VirtualRect.Height * 2 - 1));

                            k += 1;
                        }

                        l += 1;
                    }
                }


            }

            // Catch
            // End Try
            PictureBox_Preview.Image = Image;
            PictureBox_Preview.ClientSize = Image.Size;
            PictureBox_Preview2x.Image = image2X;
            PictureBox_Preview2x.ClientSize = image2X.Size;
            PictureBox_Preview.Invalidate();
            PictureBox_Preview2x.Invalidate();
            // Catch
            // End Try
        }

        private (IEnumerable<Char32[]> lines, IGlyph[] glyphs) GetChars(int physicalWidth, int physicalHeight)
        {
            FontStyle Style = GetSelectedStyles();

            bool EnableDoubleSample = CheckBox_DoubleSample.Checked;
            bool AnchorLeft = CheckBox_AnchorLeft.Checked;

            ChannelPattern[] ChannelPatterns = [ChannelPattern.Draw, ChannelPattern.Draw, ChannelPattern.Draw, ChannelPattern.Draw];

            if (chkCopMode.Checked)
                ChannelPatterns[0] = ChannelPattern.One;

            IGlyphProvider gg;
            if (EnableDoubleSample)
            {
                gg = new GlyphGeneratorDoubleSample(
                    GetSelectedFont(),
                    Style,
                    (int)Math.Round(NumericUpDown_Size.Value),
                    physicalWidth,
                    physicalHeight,
                    (int)Math.Round(NumericUpDown_DrawOffsetX.Value),
                    (int)Math.Round(NumericUpDown_DrawOffsetY.Value),
                    (int)Math.Round(NumericUpDown_VirtualOffsetX.Value),
                    (int)Math.Round(NumericUpDown_VirtualOffsetY.Value),
                    (int)Math.Round(NumericUpDown_VirtualDeltaWidth.Value),
                    (int)Math.Round(NumericUpDown_VirtualDeltaHeight.Value),
                    AnchorLeft,
                    ChannelPatterns);
            }
            else
            {
                gg = new GlyphGenerator(
                    GetSelectedFont(), Style,
                    (int)Math.Round(NumericUpDown_Size.Value),
                    physicalWidth,
                    physicalHeight,
                    (int)Math.Round(NumericUpDown_DrawOffsetX.Value),
                    (int)Math.Round(NumericUpDown_DrawOffsetY.Value),
                    (int)Math.Round(NumericUpDown_VirtualOffsetX.Value),
                    (int)Math.Round(NumericUpDown_VirtualOffsetY.Value),
                    (int)Math.Round(NumericUpDown_VirtualDeltaWidth.Value),
                    (int)Math.Round(NumericUpDown_VirtualDeltaHeight.Value),
                    AnchorLeft,
                    ChannelPatterns);
            }

            IEnumerable<Char32[]> lines = _testStrings
                .Select(l => l.ToUTF32()).ToArray();

            IGlyph[] glyphs;

            using (gg)
            {
                glyphs = lines
                    .SelectMany(l => l)
                    //.AsParallel()
                    .Select(c => gg.GetGlyph(StringCode.FromUniChar(c)))
                    .ToArray();
            }

            return (lines, glyphs);
        }

        private FontStyle GetSelectedStyles()
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
            return Style;
        }

        private string CustomFontPath;

        private string GetSelectedFont()
        {
            if (!string.IsNullOrEmpty(CustomFontPath))
            {
                return CustomFontPath;
            }

            return ((FontFamily)ComboBox_FontName.SelectedItem).Name;
        }

        private void FontGen_Load(object sender, EventArgs e)
        {
            Initialized = true;
            ComboBox_FontName.DataSource = FontFamily.Families.ToList();
            ComboBox_FontName.SelectedIndex = 0;
            ReDraw();
        }

        private void FontGen_Shown(object sender, EventArgs e)
        {
        }
        private void ComboBox_FontName_SelectedValueChanged(object sender, EventArgs e)
        {
            if (ComboBox_FontName.SelectedIndex >= 0)
            {
                CustomFontPath = string.Empty;

                ReDraw();

                FontFamily f = (FontFamily)ComboBox_FontName.SelectedItem;
            }
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
        private void chkDrawAlpha_CheckedChanged(object sender, EventArgs e)
        {
            //ReDraw();
        }

        private string Esc(string parameter)
        {
            if (string.IsNullOrEmpty(parameter))
                return "\"\"";
            if (parameter.Contains(" "))
                return "\"" + parameter + "\"";
            if (parameter.Contains("　"))
                return "\"" + parameter + "\"";
            return parameter;
        }

        private string FormatEsc(string format, params object[] args)
        {
            return format.Formats(args.Select(arg => Esc(arg.ToString())).ToArray());
        }

        private void Button_CmdToClipboard_Click(object sender, EventArgs e)
        {
            FontStyle Style = GetSelectedStyles();

            int PhysicalWidth = (int)Math.Round(NumericUpDown_PhysicalWidth.Value);
            int PhysicalHeight = (int)Math.Round(NumericUpDown_PhysicalHeight.Value);

            var Options = new List<string>();
            if (CheckBox_DoubleSample.Checked)
                Options.Add("/x2");
            if (CheckBox_AnchorLeft.Checked)
                Options.Add("/left");

            string path = "";

            string[] AddParameters = [
                FileNameHandling.GetFileName(path),
                GetSelectedFont(),
                ((int)Style).ToString(),
                NumericUpDown_Size.Value.ToString(),
                PhysicalWidth.ToString(),
                PhysicalHeight.ToString(),
                NumericUpDown_DrawOffsetX.Value.ToString(),
                NumericUpDown_DrawOffsetY.Value.ToString(),
                NumericUpDown_VirtualOffsetX.Value.ToString(),
                NumericUpDown_VirtualOffsetY.Value.ToString(),
                NumericUpDown_VirtualDeltaWidth.Value.ToString(),
                NumericUpDown_VirtualDeltaHeight.Value.ToString()];

            Options.Add("/add:" + string.Join(",", (from p in AddParameters select Esc(p)).ToArray()));
            Options.Add("/save:" + Esc(FileNameHandling.ChangeExtension(FileNameHandling.GetFileName(path), "fd")));

            string Cmd = FormatEsc("FontGen " + string.Join(" ", Options.ToArray()));
            Clipboard.SetText(Cmd);

            MessageBox.Show(Cmd, Text);
        }

        private void RunAndWait(string workDir, string exe, string args)
        {
            ProcessStartInfo myProcess = new ProcessStartInfo(exe, args);
            myProcess.WorkingDirectory = workDir;
            myProcess.UseShellExecute = true;
            myProcess.Verb = "runas";

            Process.Start(myProcess).WaitForExit();
        }

        private void Button_Generate_Click(object sender, EventArgs e)
        {
            FontStyle Style = GetSelectedStyles();

            int PhysicalWidth = (int)Math.Round(NumericUpDown_PhysicalWidth.Value);
            int PhysicalHeight = (int)Math.Round(NumericUpDown_PhysicalHeight.Value);
            bool EnableDoubleSample = CheckBox_DoubleSample.Checked;
            bool AnchorLeft = CheckBox_AnchorLeft.Checked;

            ChannelPattern[] ChannelPatterns = [ChannelPattern.Draw, ChannelPattern.Draw, ChannelPattern.Draw, ChannelPattern.Draw];

            if (chkCopMode.Checked)
                ChannelPatterns[0] = ChannelPattern.One;

            IEnumerable<IGlyph> glyphs = GenerateFont(
            "",
            GetSelectedFont(), Style,
            (int)Math.Round(NumericUpDown_Size.Value),
            PhysicalWidth,
            PhysicalHeight,
            (int)Math.Round(NumericUpDown_DrawOffsetX.Value),
            (int)Math.Round(NumericUpDown_DrawOffsetY.Value),
            (int)Math.Round(NumericUpDown_VirtualOffsetX.Value),
            (int)Math.Round(NumericUpDown_VirtualOffsetY.Value),
            (int)Math.Round(NumericUpDown_VirtualDeltaWidth.Value),
            (int)Math.Round(NumericUpDown_VirtualDeltaHeight.Value),
            EnableDoubleSample,
            AnchorLeft,
            ChannelPatterns);

            string wortDir = txtTargetPath.Text;

            if (string.IsNullOrWhiteSpace(wortDir))
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        wortDir = fbd.SelectedPath;

                        txtTargetPath.Text = wortDir;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(wortDir))
            {
                return;
            }

            string path = Path.Combine(wortDir, Path.GetFileName(GetSelectedFont()));

            string fdFilePath = FileNameHandling.ChangeExtension(path, "fd");
            string bmpFilePath = FileNameHandling.ChangeExtension(path, "bmp");

            int bpp = chkCopMode.Checked ? 8 : 32;

            SaveFont(glyphs, fdFilePath, -1, -1, bpp, false, false);

            string toolDir = AppDomain.CurrentDomain.BaseDirectory + "\\Bins";

            string ddsFormat = bpp == 32 ? "-32" : "-8";
            ddsFormat += " ";
            ddsFormat += chkCopMode.Checked ? "A8" : "u8888";

            FbToIni(fdFilePath);

            RunAndWait(wortDir, Path.Combine(toolDir, "nvdxt.exe"), $"-file \"{Path.GetFileName(bmpFilePath)}\" -outdir \"{wortDir}\" -nomipmap {ddsFormat}");

            //if (chkDelTemp.Checked)
            {
                string iniFilePath = FileNameHandling.ChangeExtension(path, "ini");
                string ddsFilePath = FileNameHandling.ChangeExtension(path, "dds");

                // if all good
                if (File.Exists(iniFilePath) && File.Exists(ddsFilePath))
                {
                    File.Delete(fdFilePath);
                    File.Delete(bmpFilePath);
                }
            }

            MessageBox.Show("Generation completed!", Text);
        }

        private void FbToIni(string fdFilePath)
        {
            string iniFilePath = FileNameHandling.ChangeExtension(fdFilePath, "ini");

            if (File.Exists(fdFilePath))
            {
                string[] lines = File.ReadAllLines(fdFilePath);
                Dictionary<int, bool> chars_map = new Dictionary<int, bool>();
                List<string> result = new List<string>();
                int height = -1;

                foreach (string l in lines)
                {
                    string[] parts = l.Split(',');

                    int charCode = int.Parse(parts[0].Substring(2), System.Globalization.NumberStyles.HexNumber);

                    if (!chkUTF8.Checked)
                    {
                        switch (charCode)
                        {
                            case 0x401: // Ё
                                {
                                    charCode = 0xA8;
                                    break;
                                }
                            case 0x451: // ё
                                {
                                    charCode = 0xB8;
                                    break;
                                }
                            case 0x2116: // №
                                {
                                    charCode = 0xB9;
                                    break;
                                }
                            case 0x2014: // —
                                {
                                    charCode = 0x97;
                                    break;
                                }
                            case 0x490: // Ґ
                                {
                                    charCode = 0xA5;
                                    break;
                                }
                            case 0x491: // ґ
                                {
                                    charCode = 0xB4;
                                    break;
                                }
                            case 0x404: // Є
                                {
                                    charCode = 0xAA;
                                    break;
                                }
                            case 0x454: // є
                                {
                                    charCode = 0xBA;
                                    break;
                                }
                            case 0x406: // І
                                {
                                    charCode = 0xB2;
                                    break;
                                }
                            case 0x456: // і
                                {
                                    charCode = 0xB3;
                                    break;
                                }
                            case 0x407: // Ї
                                {
                                    charCode = 0xAF;
                                    break;
                                }
                            case 0x457: // ї
                                {
                                    charCode = 0xBF;
                                    break;
                                }
                            default:
                                {
                                    if (charCode >= 0x410 && charCode < (0x410 + 256))
                                    {
                                        charCode -= 848;
                                    }
                                    break;
                                }
                        }
                    }

                    int x1, y1;

                    x1 = int.Parse(parts[2]);
                    y1 = int.Parse(parts[3]);

                    int i_x1, i_y1;

                    i_x1 = int.Parse(parts[6]);
                    i_y1 = int.Parse(parts[7]);

                    int i_x2, i_y2;

                    i_x2 = int.Parse(parts[8]);
                    i_y2 = int.Parse(parts[9]);

                    height = i_y2;

                    if (chkUTF8.Checked || charCode <= 0xff)
                    {
                        if (chkUTF8.Checked)
                            result.Add($"{charCode:D5} = {x1 + i_x1}, {y1 + i_y1}, {x1 + i_x1 + i_x2}, {y1 + i_y1 + i_y2}");
                        else
                            result.Add($"{charCode:D3} = {x1 + i_x1}, {y1 + i_y1}, {x1 + i_x1 + i_x2}, {y1 + i_y1 + i_y2}");
                        chars_map[charCode] = true;
                    }
                }

                if (!chkUTF8.Checked)
                {
                    for (int i = 0; i <= 0xff; i++)
                    {
                        if (!chars_map.ContainsKey(i))
                            result.Add($"{i:D3} = {0}, {0}, {0}, {0}");
                    }
                }

                result.Insert(0, $"height = {height}");
                result.Insert(0, chkUTF8.Checked ? "[mb_symbol_coords]" : "[symbol_coords]");

                if (File.Exists(iniFilePath))
                {
                    File.SetAttributes(iniFilePath, FileAttributes.Normal);
                    File.Delete(iniFilePath);
                }

                File.WriteAllLines(iniFilePath, result);
            }
        }

        private void btnCustomFont_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fd = new OpenFileDialog())
            {
                fd.Filter = "(*.ttf)|*.ttf";

                if (fd.ShowDialog() == DialogResult.OK)
                {
                    CustomFontPath = fd.FileName;
                    if (ComboBox_FontName.SelectedIndex != -1)
                    {
                        ComboBox_FontName.SelectedIndex = -1;
                    }

                    ReDraw();

                    Collection.AddFontFile(CustomFontPath);
                }
            }
        }

        private void ComboBox_FontName_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index != -1)
            {
                var comboBox = (ComboBox)sender;
                var fontFamily = (FontFamily)comboBox.Items[e.Index];
                var font = new Font(fontFamily, 12);

                e.DrawBackground();
                e.Graphics.DrawString(font.Name, font, Brushes.Black, e.Bounds.X, e.Bounds.Y);
            }
        }

        private void ComboBox_FontName_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index != -1)
            {
                var comboBox = (ComboBox)sender;
                var fontFamily = (FontFamily)comboBox.Items[e.Index];
                var font = new Font(fontFamily, 12);

                var DrawedRectangle = e.Graphics.MeasureStringRectangle(font.Name, font);

                e.ItemHeight = (int)Math.Floor(DrawedRectangle.Height);
            }
        }

        private FontGenContent FontGenContent = new FontGenContent()
        {
        };

        private void btnEditText_Click(object sender, EventArgs e)
        {
            FontGenContent.Tag = this;

            if (FontGenContent.Visible)
            {
                FontGenContent.Hide();
            }
            else
            {
                FontGenContent.Show();
                FontGenContent.SetText(_testStrings);
            }
        }

        public void SetContent(string[] lines)
        {
            _testStrings = lines;

            ReDraw();
        }

        private void btnAutosize_Click(object sender, EventArgs e)
        {
            int PhysicalWidth = (int)Math.Round(NumericUpDown_PhysicalWidth.Value);
            int PhysicalHeight = (int)Math.Round(NumericUpDown_PhysicalHeight.Value);

            var (_, gl) = GetChars(PhysicalWidth, PhysicalHeight);

            PhysicalWidth = (from g in gl
                             select g.VirtualBox.Width + 4).Max();
            PhysicalHeight = (from g in gl
                              select g.VirtualBox.Height + 4).Max();

            NumericUpDown_PhysicalWidth.Value = PhysicalWidth;
            NumericUpDown_PhysicalHeight.Value = PhysicalHeight;
        }
    }
}