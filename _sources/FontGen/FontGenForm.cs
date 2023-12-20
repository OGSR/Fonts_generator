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
using System.Threading.Tasks;
using System.Windows.Forms;
using Firefly;
using Firefly.Glyphing;
using Firefly.Imaging;
using Firefly.TextEncoding;
using Firefly.Texting;

namespace FontGen
{
    public partial class FontGenForm
    {
        PrivateFontCollection collection = new PrivateFontCollection();

        static string[] TestStrings = [
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

        public static IEnumerable<IGlyph> GenerateFont(string SourcePath, string FontName, FontStyle FontStyle, int FontSize, int PhysicalWidth, int PhysicalHeight, int DrawOffsetX, int DrawOffsetY, int VirtualOffsetX, int VirtualOffsetY, int VirtualDeltaWidth, int VirtualDeltaHeight, bool EnableDoubleSample, bool AnchorLeft, ChannelPattern[] ChannelPatterns)
        {
            StringCode[] StringCodes;

            if (!string.IsNullOrWhiteSpace(SourcePath) && File.Exists(SourcePath))
            {
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
            }
            else
            {
                StringCodes = TestStrings.SelectMany(l => l.ToUTF32()).Select(c => StringCode.FromUniChar(c))
                    .Distinct()
                    .ToArray();
            }

            IGlyphProvider gg;
            if (EnableDoubleSample)
            {
                gg = new GlyphGeneratorDoubleSample(
                    FontName,
                    FontStyle,
                    FontSize,
                    PhysicalWidth,
                    PhysicalHeight,
                    DrawOffsetX,
                    DrawOffsetY,
                    VirtualOffsetX,
                    VirtualOffsetY,
                    VirtualDeltaWidth,
                    VirtualDeltaHeight,
                    AnchorLeft,
                    ChannelPatterns);
            }
            else
            {
                gg = new GlyphGenerator(
                    FontName,
                    FontStyle,
                    FontSize,
                    PhysicalWidth,
                    PhysicalHeight,
                    DrawOffsetX,
                    DrawOffsetY,
                    VirtualOffsetX,
                    VirtualOffsetY,
                    VirtualDeltaWidth,
                    VirtualDeltaHeight,
                    AnchorLeft,
                    ChannelPatterns);
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
                    using (BmpFontImageWriter imageWriter = new BmpFontImageWriter(FileNameHandling.ChangeExtension(FdPath, "bmp"), BitPerPixel))
                    {
                        FdGlyphDescriptionFile.WriteFont(FdPath, TextEncoding.WritingDefault, pgl, pgd, imageWriter, PicWidth, PicHeight);
                    }
                    GlyphIndex += pgd.Length;
                }
            }
            else
            {
                using (BmpFontImageWriter imageWriter = new BmpFontImageWriter(FileNameHandling.ChangeExtension(TargetPath, "bmp"), BitPerPixel))
                {
                    FdGlyphDescriptionFile.WriteFont(TargetPath, TextEncoding.WritingDefault, gl, imageWriter, ga, PicWidth, PicHeight);
                }
            }
        }

        private bool Initialized = false;

        private void ReDraw()
        {
            if (!Initialized)
                return;

            int PhysicalWidth = (int)Math.Round(NumericUpDown_PhysicalWidth.Value);
            int PhysicalHeight = (int)Math.Round(NumericUpDown_PhysicalHeight.Value);

            // Try
            var Image = new Bitmap(PhysicalWidth * 64 + 32 * 8, PhysicalHeight * 32 + 32 * 8, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var Image2x = new Bitmap(PhysicalWidth * 64 * 2 + 32 * 8 * 2, PhysicalHeight * 32 * 2 + 32 * 8 * 2, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            // Try
            using (var g = Graphics.FromImage(Image))
            using (var g2x = Graphics.FromImage(Image2x))
            {
                g.Clear(Color.White);
                g2x.Clear(Color.LightGray);

                FontStyle Style = GetSelectedStyles();

                bool EnableDoubleSample = CheckBox_DoubleSample.Checked;
                bool AnchorLeft = CheckBox_AnchorLeft.Checked;

                ChannelPattern[] ChannelPatterns = [ChannelPattern.Draw, ChannelPattern.Draw, ChannelPattern.Draw, ChannelPattern.Draw];

                if (!chkDrawAlpha.Checked)
                    ChannelPatterns[0] = ChannelPattern.One;

                IGlyphProvider gg;
                if (EnableDoubleSample)
                {
                    gg = new GlyphGeneratorDoubleSample(
                        GetSelectedFont(),
                        Style,
                        (int)Math.Round(NumericUpDown_Size.Value),
                        PhysicalWidth,
                        PhysicalHeight,
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
                        PhysicalWidth,
                        PhysicalHeight,
                        (int)Math.Round(NumericUpDown_DrawOffsetX.Value),
                        (int)Math.Round(NumericUpDown_DrawOffsetY.Value),
                        (int)Math.Round(NumericUpDown_VirtualOffsetX.Value),
                        (int)Math.Round(NumericUpDown_VirtualOffsetY.Value),
                        (int)Math.Round(NumericUpDown_VirtualDeltaWidth.Value),
                        (int)Math.Round(NumericUpDown_VirtualDeltaHeight.Value),
                        AnchorLeft,
                        ChannelPatterns);
                }

                var range = Enumerable
                    .Range(0, PhysicalHeight)
                    .SelectMany(y0 => Enumerable.Range(0, PhysicalWidth).Select(x0 => (y0, x0)))
                    .ToList();

                using (gg)
                {
                    using (var b = new Bmp(PhysicalWidth, PhysicalHeight, 32))
                    using (var b2x = new Bmp(PhysicalWidth * 2, PhysicalHeight * 2, 32))
                    {
                        int[,] Block2x = new int[(PhysicalWidth * 2), (PhysicalHeight * 2)];
                        //int[,] Block = new int[(PhysicalWidth), (PhysicalHeight)];

                        int l = 0;
                        foreach (string t in TestStrings)
                        {
                            int k = 0;
                            foreach (Char32 c in t.ToUTF32())
                            {
                                IGlyph glyph = gg.GetGlyph(StringCode.FromUniChar(c));

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

                                    Block2x[x0 * 2, y0 * 2] = Block[x0, y0];
                                    Block2x[x0 * 2 + 1, y0 * 2] = Block[x0, y0];
                                    Block2x[x0 * 2, y0 * 2 + 1] = Block[x0, y0];
                                    Block2x[x0 * 2 + 1, y0 * 2 + 1] = Block[x0, y0];
                                });

                                //for (int y0 = 0; y0 <= PhysicalHeight - 1; y0++)                                
                                //for (int x0 = 0; x0 <= PhysicalWidth - 1; x0++)
                                //{
                                //    Block2x[x0 * 2, y0 * 2] = Block[x0, y0];
                                //    Block2x[x0 * 2 + 1, y0 * 2] = Block[x0, y0];
                                //    Block2x[x0 * 2, y0 * 2 + 1] = Block[x0, y0];
                                //    Block2x[x0 * 2 + 1, y0 * 2 + 1] = Block[x0, y0];
                                //}                                

                                b2x.SetRectangle(0, 0, Block2x);
                                using (var bb2x = b2x.ToBitmap())
                                {
                                    var PhysicalRect2x = new Rectangle(x * 2, y * 2, PhysicalWidth * 2, PhysicalHeight * 2);
                                    g2x.DrawImage(bb2x, PhysicalRect2x);
                                }

                                var VirtualRect = glyph.VirtualBox;
                                g2x.DrawRectangle(Pens.Red, new Rectangle(x * 2 + VirtualRect.X * 2, y * 2 + VirtualRect.Y * 2, VirtualRect.Width * 2 - 1, VirtualRect.Height * 2 - 1));

                                k += 1;
                            }

                            l += 1;
                        }
                    }

                }
            }

            // Catch
            // End Try
            PictureBox_Preview.Image = Image;
            PictureBox_Preview.ClientSize = Image.Size;
            PictureBox_Preview2x.Image = Image2x;
            PictureBox_Preview2x.ClientSize = Image2x.Size;
            PictureBox_Preview.Invalidate();
            PictureBox_Preview2x.Invalidate();
            // Catch
            // End Try
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

        private string customFontPath;

        private string GetSelectedFont()
        {
            if (!string.IsNullOrEmpty(customFontPath))
            {
                return customFontPath;
            }

            return ((FontFamily)ComboBox_FontName.SelectedItem).Name;
        }

        private void FontGen_Load(object sender, EventArgs e)
        {
            Initialized = true;
            ComboBox_FontName.DataSource = FontFamily.Families.ToList();
            ComboBox_FontName.SelectedIndex = 0;
            ReDraw();

            ddlBPP.SelectedItem = "32";
        }

        private void FontGen_Shown(object sender, EventArgs e)
        {
        }
        private void ComboBox_FontName_SelectedValueChanged(object sender, EventArgs e)
        {
            if (ComboBox_FontName.SelectedIndex >= 0)
            {
                customFontPath = string.Empty;

                ReDraw();

                FontFamily f = (FontFamily)ComboBox_FontName.SelectedItem;

                lblCustomFontName.Text = f.Name;
                lblCustomFontName.Font = new Font(f, (int)Math.Round(NumericUpDown_Size.Value), GetSelectedStyles(), GraphicsUnit.Pixel);
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
            FontStyle Style = GetSelectedStyles();

            int PhysicalWidth = (int)Math.Round(NumericUpDown_PhysicalWidth.Value);
            int PhysicalHeight = (int)Math.Round(NumericUpDown_PhysicalHeight.Value);

            var Options = new List<string>();
            if (CheckBox_DoubleSample.Checked)
                Options.Add("/x2");
            if (CheckBox_AnchorLeft.Checked)
                Options.Add("/left");

            string path = FileSelectBox_File.Path;

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

            bool cop_mode = !chkDrawAlpha.Checked;

            if (cop_mode)
                ChannelPatterns[0] = ChannelPattern.One;

            int bpp = int.Parse(ddlBPP.SelectedItem.ToString());

            IEnumerable<IGlyph> glyphs = GenerateFont(
            FileSelectBox_File.Path,
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

            string fd_filePath = FileNameHandling.ChangeExtension(path, "fd");
            string bmp_filePath = FileNameHandling.ChangeExtension(path, "bmp");

            SaveFont(glyphs, fd_filePath, -1, -1, bpp, false, false);

            string toolDir = AppDomain.CurrentDomain.BaseDirectory + "\\Bins";

            string dds_format = bpp == 32 ? "-32" : "-8";
            dds_format+= cop_mode ? "A8" : "u8888";

            RunAndWait(wortDir, Path.Combine(toolDir, "FD2INI.exe"), $"\"{Path.GetFileName(fd_filePath)}\"");
            RunAndWait(wortDir, Path.Combine(toolDir, "BmpCuter.exe"), Path.GetFileName(bmp_filePath));
            RunAndWait(wortDir, Path.Combine(toolDir, "nvdxt.exe"), $"-file \"{Path.GetFileName(bmp_filePath)}\" -outdir \"{wortDir}\" -nomipmap {dds_format}");

            if (chkDelTemp.Checked)
            {
                string ini_filePath = FileNameHandling.ChangeExtension(path, "ini");
                string dds_filePath = FileNameHandling.ChangeExtension(path, "dds");

                // if all good
                if (File.Exists(ini_filePath) && File.Exists(dds_filePath))
                {
                    File.Delete(fd_filePath);
                    File.Delete(bmp_filePath);
                }
            }

            MessageBox.Show("Generation completed!", Text);
        }

        private void btnCustomFont_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fd = new OpenFileDialog())
            {
                fd.Filter = "(*.ttf)|*.ttf";

                if (fd.ShowDialog() == DialogResult.OK)
                {
                    customFontPath = fd.FileName;
                    if (ComboBox_FontName.SelectedIndex != -1)
                    {
                        ComboBox_FontName.SelectedIndex = -1;
                    }
                    
                    ReDraw();

                    lblCustomFontName.Text = Path.GetFileName(customFontPath);

                    collection.AddFontFile(customFontPath);
                    lblCustomFontName.Font = new Font(collection.Families[collection.Families.Length - 1], (int)Math.Round(NumericUpDown_Size.Value), GetSelectedStyles(), GraphicsUnit.Pixel);
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
                FontGenContent.SetText(TestStrings);
            }
        }

        public void SetContent(string[] lines)
        {
            TestStrings = lines;

            ReDraw();
        }
    }
}