// ==========================================================================
// 
// File:        FdGlyphDescriptionFile.vb
// Location:    Firefly.TextEncoding <Visual Basic .Net>
// Description: fd字形描述文件
// Version:     2010.04.08.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Firefly.Imaging;
using Firefly.TextEncoding;

namespace Firefly.Glyphing
{
    public sealed class FdGlyphDescriptionFile
    {
        private FdGlyphDescriptionFile()
        {
        }

        public static IEnumerable<GlyphDescriptor> ReadFile(string Path, Encoding Encoding)
        {
            var d = new List<GlyphDescriptor>();
            using (var s = Texting.Txt.CreateTextReader(Path, Encoding, true))
            {
                var r = new Regex(@"^U\+(?<Unicode>[0-9A-Fa-f]+)$", RegexOptions.ExplicitCapture);
                int LineNumber = 1;
                while (!s.EndOfStream)
                {
                    string Line = s.ReadLine();
                    if (!string.IsNullOrEmpty(Line.Trim()))
                    {
                        string[] Values = Line.Split(',');
                        if (Values.Length != 10)
                            throw new InvalidDataException(string.Format("{0}({1}) : 格式错误。", Path, LineNumber));

                        var Unicodes = new List<Char32>();
                        if (!Regex.Match(Values[0], "^ *$").Success)
                        {
                            foreach (var p in Regex.Split(Values[0], " +"))
                            {
                                var m = r.Match(p);
                                if (!m.Success)
                                    throw new InvalidDataException(string.Format("{0}({1}) : 格式错误。", Path, LineNumber));
                                int Unicode = int.Parse(m.Result("${Unicode}"), System.Globalization.NumberStyles.HexNumber);
                                Unicodes.Add(Unicode);
                            }
                        }
                        string Code = Values[1];
                        StringCode c;
                        if (!string.IsNullOrEmpty(Code))
                        {
                            c = new StringCode(Unicodes.ToArray().ToUTF16B(), int.Parse(Code, System.Globalization.NumberStyles.HexNumber));
                        }
                        else
                        {
                            c = StringCode.FromUniStr(Unicodes.ToArray().ToUTF16B());
                        }

                        var PhysicalBox = new Rectangle(int.Parse(Values[2], System.Globalization.NumberStyles.Integer), int.Parse(Values[3], System.Globalization.NumberStyles.Integer), int.Parse(Values[4], System.Globalization.NumberStyles.Integer), int.Parse(Values[5], System.Globalization.NumberStyles.Integer));
                        var VirtualBox = new Rectangle(int.Parse(Values[6], System.Globalization.NumberStyles.Integer), int.Parse(Values[7], System.Globalization.NumberStyles.Integer), int.Parse(Values[8], System.Globalization.NumberStyles.Integer), int.Parse(Values[9], System.Globalization.NumberStyles.Integer));

                        d.Add(new GlyphDescriptor() { c = c, PhysicalBox = PhysicalBox, VirtualBox = VirtualBox });
                    }
                    LineNumber += 1;
                }
            }
            return d;
        }
        public static IEnumerable<GlyphDescriptor> ReadFile(string Path)
        {
            return ReadFile(Path, TextEncoding.TextEncoding.Default);
        }
        public static void WriteFile(string Path, Encoding Encoding, IEnumerable<GlyphDescriptor> GlyphDescriptors)
        {
            using (var s = Texting.Txt.CreateTextWriter(Path, Encoding, true))
            {
                foreach (var d in GlyphDescriptors)
                {
                    string Unicode = "";
                    if (d.c.HasUnicode)
                        Unicode = string.Join(" ", (from u in d.c.Unicode.ToUTF32()
                                                    select "U+{0:X4}".Formats(u.Value)).ToArray());
                    string Code = "";
                    if (d.c.HasCode)
                        Code = d.c.CodeString;

                    s.WriteLine(string.Join(",", (from o in new object[] { Unicode, Code, d.PhysicalBox.X, d.PhysicalBox.Y, d.PhysicalBox.Width, d.PhysicalBox.Height, d.VirtualBox.X, d.VirtualBox.Y, d.VirtualBox.Width, d.VirtualBox.Height }

                                                  select (o.ToString())).ToArray()));
                }
            }
        }
        public static void WriteFile(string Path, IEnumerable<GlyphDescriptor> GlyphDescriptors)
        {
            WriteFile(Path, TextEncoding.TextEncoding.WritingDefault, GlyphDescriptors);
        }

        public static IEnumerable<IGlyph> ReadFont(string FdPath, Encoding Encoding, IImageReader ImageReader)
        {
            var GlyphDescriptors = ReadFile(FdPath, Encoding);
            ImageReader.Load();
            var l = new List<IGlyph>();
            foreach (var d in GlyphDescriptors)
                l.Add(new Glyph() { c = d.c, Block = ImageReader.GetRectangleAsARGB(d.PhysicalBox.X, d.PhysicalBox.Y, d.PhysicalBox.Width, d.PhysicalBox.Height), VirtualBox = d.VirtualBox });
            return l;
        }
        public static IEnumerable<IGlyph> ReadFont(string FdPath, IImageReader ImageReader)
        {
            return ReadFont(FdPath, TextEncoding.TextEncoding.Default, ImageReader);
        }
        public static IEnumerable<IGlyph> ReadFont(string FdPath)
        {
            var Encoding = TextEncoding.TextEncoding.Default;
            string BmpPath = FileNameHandling.ChangeExtension(FdPath, "bmp");
            using (var ImageReader = new BmpFontImageReader(BmpPath))
            {
                return ReadFont(FdPath, Encoding, ImageReader);
            }
        }
        public static void WriteFont(string FdPath, Encoding Encoding, IEnumerable<IGlyph> Glyphs, IEnumerable<GlyphDescriptor> GlyphDescriptors, IImageWriter ImageWriter, int Width, int Height)
        {
            IGlyph[] gl = Glyphs.ToArray();
            GlyphDescriptor[] gdl = GlyphDescriptors.ToArray();
            if (gl.Length != gdl.Length)
                throw new ArgumentException("GlyphsAndGlyphDescriptorsCountNotMatch");
            int PicWidth = Width;
            int PicHeight = Height;

            ImageWriter.Create(PicWidth, PicHeight);
            for (int GlyphIndex = 0, loopTo = gl.Count() - 1; GlyphIndex <= loopTo; GlyphIndex++)
            {
                var g = gl[GlyphIndex];
                var gd = gdl[GlyphIndex];
                int x = gd.PhysicalBox.X;
                int y = gd.PhysicalBox.Y;
                ImageWriter.SetRectangleFromARGB(x, y, g.Block);
            }
            WriteFile(FdPath, Encoding, GlyphDescriptors);
        }
        public static void WriteFont(string FdPath, Encoding Encoding, IEnumerable<IGlyph> Glyphs, IImageWriter ImageWriter, IGlyphArranger GlyphArranger, int Width, int Height)
        {
            IGlyph[] gl = Glyphs.ToArray();
            int PicWidth = Width;
            int PicHeight = Height;

            var GlyphDescriptors = GlyphArranger.GetGlyphArrangement(gl, PicWidth, PicHeight);
            GlyphDescriptor[] gdl = GlyphDescriptors.ToArray();
            if (gl.Length != gdl.Length)
                throw new InvalidOperationException("NumGlyphTooMuch: NumGlyph={0} MaxNumGlyph={1}".Formats(gl.Count(), GlyphDescriptors.Count()));

            WriteFont(FdPath, Encoding, Glyphs, GlyphDescriptors, ImageWriter, Width, Height);
        }
        public static void WriteFont(string FdPath, Encoding Encoding, IEnumerable<IGlyph> Glyphs, IImageWriter ImageWriter, int Width, int Height)
        {
            IGlyph[] gl = Glyphs.ToArray();
            int PhysicalWidth = (from g in gl
                                 select g.PhysicalWidth).Max();
            int PhysicalHeight = (from g in gl
                                  select g.PhysicalHeight).Max();
            var ga = new GlyphArranger(PhysicalWidth, PhysicalHeight);
            int PicWidth = Width;
            int PicHeight = Height;

            WriteFont(FdPath, Encoding, gl, ImageWriter, ga, PicWidth, PicHeight);
        }
        public static void WriteFont(string FdPath, Encoding Encoding, IEnumerable<IGlyph> Glyphs, IImageWriter ImageWriter, int Width)
        {
            IGlyph[] gl = Glyphs.ToArray();
            int PhysicalWidth = (from g in gl
                                 select g.PhysicalWidth).Max();
            int PhysicalHeight = (from g in gl
                                  select g.PhysicalHeight).Max();
            var ga = new GlyphArranger(PhysicalWidth, PhysicalHeight);
            int PicWidth = Width;
            int PicHeight = ga.GetPreferredHeight(gl, Width);

            WriteFont(FdPath, Encoding, gl, ImageWriter, ga, PicWidth, PicHeight);
        }
        public static void WriteFont(string FdPath, Encoding Encoding, IEnumerable<IGlyph> Glyphs, IImageWriter ImageWriter)
        {
            IGlyph[] gl = Glyphs.ToArray();
            int PhysicalWidth = (from g in gl
                                 select g.PhysicalWidth).Max();
            int PhysicalHeight = (from g in gl
                                  select g.PhysicalHeight).Max();
            var ga = new GlyphArranger(PhysicalWidth, PhysicalHeight);
            var Size = ga.GetPreferredSize(gl);
            int PicWidth = Size.Width;
            int PicHeight = Size.Height;

            WriteFont(FdPath, Encoding, gl, ImageWriter, ga, PicWidth, PicHeight);
        }
        public static void WriteFont(string FdPath, IEnumerable<IGlyph> Glyphs, int BitPerPixel, int[] Palette, Func<int, byte> Quantize)
        {
            string BmpPath = FileNameHandling.ChangeExtension(FdPath, "bmp");
            var Encoding = TextEncoding.TextEncoding.WritingDefault;
            using (var ImageWriter = new BmpFontImageWriter(BmpPath, (short)BitPerPixel, Palette, Quantize))
            {
                WriteFont(FdPath, Encoding, Glyphs, ImageWriter);
            }
        }
        public static void WriteFont(string FdPath, IEnumerable<IGlyph> Glyphs, int BitPerPixel)
        {
            string BmpPath = FileNameHandling.ChangeExtension(FdPath, "bmp");
            var Encoding = TextEncoding.TextEncoding.WritingDefault;
            using (var ImageWriter = new BmpFontImageWriter(BmpPath, (short)BitPerPixel))
            {
                WriteFont(FdPath, Encoding, Glyphs, ImageWriter);
            }
        }
        public static void WriteFont(string FdPath, IEnumerable<IGlyph> Glyphs)
        {
            string BmpPath = FileNameHandling.ChangeExtension(FdPath, "bmp");
            var Encoding = TextEncoding.TextEncoding.WritingDefault;
            using (var ImageWriter = new BmpFontImageWriter(BmpPath))
            {
                WriteFont(FdPath, Encoding, Glyphs, ImageWriter);
            }
        }
    }

    public class BmpFontImageReader : IImageReader
    {

        private string BmpPath;
        private Bmp b;

        public BmpFontImageReader(string Path)
        {
            BmpPath = Path;
        }

        public void Load()
        {
            if (b is not null)
                throw new InvalidOperationException();
            b = Bmp.Open(BmpPath);
        }
        public int[,] GetRectangleAsARGB(int x, int y, int w, int h)
        {
            return b.GetRectangleAsARGB(x, y, w, h);
        }

        #region  IDisposable 支持 
        /// <summary>释放托管对象或间接非托管对象(Stream等)。可在这里将大型字段设置为 null。</summary>
        protected virtual void DisposeManagedResource()
        {
            if (b is not null)
            {
                b.Dispose();
                b = null;
            }
        }

        /// <summary>释放直接非托管对象(Handle等)。可在这里将大型字段设置为 null。</summary>
        protected virtual void DisposeUnmanagedResource()
        {
        }

        // 检测冗余的调用
        private bool DisposedValue = false;
        /// <summary>释放流的资源。请优先覆盖DisposeManagedResource、DisposeUnmanagedResource、DisposeNullify方法。如果你直接保存非托管对象(Handle等)，请覆盖Finalize方法，并在其中调用Dispose(False)。</summary>
        protected virtual void Dispose(bool disposing)
        {
            if (DisposedValue)
                return;
            DisposedValue = true;
            if (disposing)
            {
                DisposeManagedResource();
            }
            DisposeUnmanagedResource();
        }

        /// <summary>释放流的资源。</summary>
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入上面的 Dispose(ByVal disposing As Boolean) 中。
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class BmpFontImageWriter : IImageWriter
    {

        private string BmpPath;
        private Bmp b;
        private int BitPerPixel;
        private int[] Palette;
        private Func<int, byte> Quantize;

        public BmpFontImageWriter(string Path) : this(Path, 8)
        {
        }
        public BmpFontImageWriter(string Path, int BitPerPixel)
        {
            BmpPath = Path;
            this.BitPerPixel = BitPerPixel;
            switch (BitPerPixel)
            {
                case 2:
                    {
                        this.BitPerPixel = 4;
                        byte GetGray(int ARGB) => (byte)((((ARGB & 0xFF0000) >> 16) + ((ARGB & 0xFF00) >> 8) + (ARGB & 0xFF) + 2) / 3);
                        int r = 255 / ((1 << BitPerPixel) - 1);
                        Palette = (from i in Enumerable.Range(0, 1 << BitPerPixel)
                                   select BitOperations.ConcatBits(0xFF, 8, (byte)(r * i), 8, (byte)(r * i), 8, (byte)(r * i), 8)).ToArray().Extend(16, 0);
                        Quantize = new Func<int, byte>((ARGB) => (byte)(GetGray(ARGB) >> 8 - BitPerPixel));
                        break;
                    }
                case 8:
                    {
                        byte GetGray(int ARGB) => (byte)((((ARGB & 0xFF0000) >> 16) + ((ARGB & 0xFF00) >> 8) + (ARGB & 0xFF) + 2) / 3);
                        Palette = (from i in Enumerable.Range(0, 1 << BitPerPixel)
                                   select BitOperations.ConcatBits(0xFF, 8, (byte)i, 8, (byte)i, 8, (byte)i, 8)).ToArray();
                        Quantize = GetGray;
                        break;
                    }
                case var @case when @case <= 8:
                    {
                        byte GetGray(int ARGB) => (byte)((((ARGB & 0xFF0000) >> 16) + ((ARGB & 0xFF00) >> 8) + (ARGB & 0xFF) + 2) / 3);
                        int r = 255 / ((1 << BitPerPixel) - 1);
                        Palette = (from i in Enumerable.Range(0, 1 << BitPerPixel)
                                   select BitOperations.ConcatBits(0xFF, 8, (byte)(r * i), 8, (byte)(r * i), 8, (byte)(r * i), 8)).ToArray();
                        Quantize = new Func<int, byte>((ARGB) => (byte)(GetGray(ARGB) >> 8 - BitPerPixel));
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }
        public BmpFontImageWriter(string Path, int BitPerPixel, int[] Palette)
        {
            BmpPath = Path;
            this.BitPerPixel = BitPerPixel;
            this.Palette = Palette;
        }
        public BmpFontImageWriter(string Path, int BitPerPixel, int[] Palette, Func<int, byte> Quantize)
        {
            BmpPath = Path;
            this.BitPerPixel = BitPerPixel;
            this.Palette = Palette;
            this.Quantize = Quantize;
        }

        public void Create(int w, int h)
        {
            b = new Bmp(BmpPath, w, h, (short)BitPerPixel);
            if (Palette is not null)
                b.Palette = Palette;
        }

        public void SetRectangleFromARGB(int x, int y, int[,] a)
        {
            if (Quantize is null)
            {
                b.SetRectangleFromARGB(x, y, a);
            }
            else
            {
                b.SetRectangleFromARGB(x, y, a, Quantize);
            }
        }

        #region  IDisposable 支持 
        /// <summary>释放托管对象或间接非托管对象(Stream等)。可在这里将大型字段设置为 null。</summary>
        protected virtual void DisposeManagedResource()
        {
            if (b is not null)
            {
                b.Dispose();
                b = null;
            }
        }

        /// <summary>释放直接非托管对象(Handle等)。可在这里将大型字段设置为 null。</summary>
        protected virtual void DisposeUnmanagedResource()
        {
        }

        // 检测冗余的调用
        private bool DisposedValue = false;
        /// <summary>释放流的资源。请优先覆盖DisposeManagedResource、DisposeUnmanagedResource、DisposeNullify方法。如果你直接保存非托管对象(Handle等)，请覆盖Finalize方法，并在其中调用Dispose(False)。</summary>
        protected virtual void Dispose(bool disposing)
        {
            if (DisposedValue)
                return;
            DisposedValue = true;
            if (disposing)
            {
                DisposeManagedResource();
            }
            DisposeUnmanagedResource();
        }

        /// <summary>释放流的资源。</summary>
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入上面的 Dispose(ByVal disposing As Boolean) 中。
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}