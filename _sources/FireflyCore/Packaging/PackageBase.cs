// ==========================================================================
// 
// File:        PackageBase.vb
// Location:    Firefly.Packaging <Visual Basic .Net>
// Description: 文件包基类
// Version:     2010.03.03.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Firefly.TextEncoding;
using Microsoft.VisualBasic.CompilerServices;

namespace Firefly.Packaging
{
    /// <summary>
    /// 文件包基类
    /// 
    /// 
    /// 给继承者的说明：
    /// 
    /// 如果文件包支持写入，应
    /// (1)在加入一个FileDB时，调用PushFile方法，使得它被加入到FileList、IndexOfFile中，以及PushFileToDir到根目录FileDB中，若根目录FileDB不存在，则空的根目录会自动创建
    /// 
    /// 请使用PackageRegister来注册文件包类型。
    /// 应提供一个返回"ISO(*.ISO)|*.ISO"形式字符串的Filter属性，
    /// 并按照PackageRegister中的委托类型提供一个Open函数、一个Create函数(如果支持创建)。
    /// </summary>
    public abstract class PackageBase : IDisposable
    {
        protected StreamEx BaseStream;

        /// <summary>全部文件的集合。</summary>
        protected List<FileDB> FileList = new List<FileDB>();
        /// <summary>文件到文件编号的映射。</summary>
        protected Dictionary<FileDB, int> IndexOfFile = new Dictionary<FileDB, int>();

        /// <summary>已重载。默认构照函数。请手动初始化BaseStream。</summary>
        protected PackageBase()
        {
        }
        /// <summary>已重载。打开或创建文件包。</summary>
        public PackageBase(ZeroPositionStreamPasser sp)
        {
            BaseStream = sp;
        }

        protected FileDB RootValue = new FileDB("", FileDB.FileType.Directory, -1, 0L);
        /// <summary>文件根。</summary>
        public FileDB Root
        {
            get
            {
                return RootValue;
            }
        }

        /// <summary>全部文件的集合。</summary>
        public IEnumerable<FileDB> Files
        {
            get
            {
                return FileList;
            }
        }

        /// <summary>把文件FileDB放入根目录FileDB。若根目录FileDB不存在，则空的根目录会自动创建。在加入一个FileDB时，调用该方法，使得它被加入到FileList、IndexOfFile中，以及PushFileToDir到根目录FileDB中。</summary>
        protected void PushFile(FileDB f)
        {
            PushFile(f, RootValue);
        }
        /// <summary>将文件FileDB放入文件夹的FileDB。在加入一个FileDB时，调用该方法，使得它被加入到FileList、IndexOfFile中，以及PushFileToDir到文件夹FileDB中。</summary>
        protected virtual void PushFile(FileDB f, FileDB Directory)
        {
            PushFileToDir(f, Directory);

            int n = FileList.Count;
            FileList.Add(f);
            IndexOfFile.Add(f, n);
        }

        /// <summary>将文件FileDB放入文件夹的FileDB。请只在PushFile中调用。</summary>
        protected void PushFileToDir(FileDB File, FileDB Directory)
        {
            string Dir = "";
            if (File.Name.Contains(@"\") || File.Name.Contains("/"))
            {
                string argPath = File.Name;
                Dir = FileNameHandling.PopFirstDir(ref argPath);
                File.Name = argPath;
            }
            if (string.IsNullOrEmpty(Dir))
            {
                Directory.SubFile.Add(File);
                Directory.SubFileNameRef.Add(File.Name, File);
                File.ParentFileDB = Directory;
            }
            else
            {
                if (!Directory.SubFileNameRef.ContainsKey(Dir))
                {
                    var DirDB = FileDB.CreateDirectory(Dir);
                    Directory.SubFile.Add(DirDB);
                    Directory.SubFileNameRef.Add(DirDB.Name, DirDB);
                    DirDB.ParentFileDB = Directory;
                }
                PushFileToDir(File, Directory.SubFileNameRef[Dir]);
            }
        }

        /// <summary>尝试按路径取得FileDB。</summary>
        public FileDB TryGetFileDB(string Path)
        {
            string p = Path;
            var ret = Root;
            string d = "";
            if (string.IsNullOrEmpty(p))
                return Root;
            if (!string.IsNullOrEmpty(Root.Name))
            {
                d = FileNameHandling.PopFirstDir(ref p);
                if (CultureInfo.CurrentCulture.CompareInfo.Compare(d ?? "", Root.Name ?? "", CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) != 0)
                    p = Path;
            }
            while (ret is not null)
            {
                d = FileNameHandling.PopFirstDir(ref p);
                if (string.IsNullOrEmpty(d))
                    return ret;
                if (ret.SubFileNameRef.ContainsKey(d))
                {
                    ret = ret.SubFileNameRef[d];
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>从文件头标识符猜测文件扩展名。</summary>
        public static string GuessExtensionFromMagicIdentifier(int MagicIdentifier, string DefaultExt)
        {
            string Ext = (new Char32[] { String32.ChrQ(MagicIdentifier & 0xFF), String32.ChrQ(MagicIdentifier >> 8 & 0xFF), String32.ChrQ(MagicIdentifier >> 16 & 0xFF), String32.ChrQ(MagicIdentifier >> 24 & 0xFF) }).ToUTF16B();
            var Match = Regex.Match(Ext, "[A-Za-z][0-9A-Za-z]{2,}");
            if (Match.Success)
                return Match.Value;
            Match = Regex.Match(Ext.Substring(0, 2), "[A-Za-z]{2}");
            if (Match.Success)
                return Match.Value;
            return DefaultExt;
        }

        /// <summary>从包中解出一个文件。</summary>
        protected virtual void ExtractInner(FileDB File, ZeroPositionStreamPasser sp)
        {
            var s = sp.GetStream();
            using (var f = new PartialStreamEx(BaseStream, File.Address, File.Length))
            {
                s.WriteFromStream(f, f.Length);
            }
        }
        /// <summary>已重载。从包中解出一个文件。该函数不再可覆盖，请覆盖ExtractInner。调用ExtractInner。</summary>
        public void Extract(FileDB File, ZeroPositionStreamPasser sp)
        {
            if (File.Type != FileDB.FileType.File)
                throw new ArgumentException();
            ExtractInner(File, sp);
        }
        /// <summary>已重载。从包中解出一个文件。调用Extract(ByVal File As FileDB, ByVal sp As ZeroPositionStreamPasser)。</summary>
        public byte[] Extract(FileDB File)
        {
            using (var s = new StreamEx())
            {
                Extract(File, s);
                s.Position = 0L;
                return s.Read((int)s.Length);
            }
        }
        /// <summary>从包中解出一个文件。应优先考虑覆盖ExtractInner。默认实现调用Extract(ByVal File As FileDB, ByVal sp As ZeroPositionStreamPasser)。</summary>
        protected virtual void ExtractSingleInner(FileDB File, string Path)
        {
            using (var t = new StreamEx(Path, FileMode.Create))
            {
                Extract(File, t);
            }
        }
        /// <summary>从包中解出一个文件。默认实现调用ExtractSingleInner。</summary>
        public void ExtractSingle(FileDB File, string Path)
        {
            string Dir = FileNameHandling.GetFileDirectory(Path);
            if (!string.IsNullOrEmpty(Dir) && !Directory.Exists(Dir))
                Directory.CreateDirectory(Dir);
            ExtractSingleInner(File, Path);
        }
        /// <summary>已重载。从包中解出一个文件或一个文件夹。调用ExtractSingle和ExtractMultiple。</summary>
        public void Extract(FileDB File, string Path, string Mask = "*")
        {
            switch (File.Type)
            {
                case FileDB.FileType.File:
                    {
                        if (FileNameHandling.IsMatchFileMask(File.Name, Mask))
                        {
                            ExtractSingle(File, Path);
                        }

                        break;
                    }
                case FileDB.FileType.Directory:
                    {
                        var Paths = new List<string>();
                        foreach (FileDB f in File.SubFile)
                            Paths.Add(FileNameHandling.GetPath(Path, f.Name));
                        ExtractMultiple(File.SubFile.ToArray(), Paths.ToArray(), Mask);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }
        /// <summary>从包中解出多个文件或文件夹。应优先考虑覆盖ExtractInner或ExtractSingleInner。默认实现调用Extract(ByVal File As FileDB, ByVal Path As String, Optional ByVal Mask As String = "*")。</summary>
        protected virtual void ExtractMultipleInner(FileDB[] Files, string[] Paths)
        {
            for (int n = 0, loopTo = Files.Length - 1; n <= loopTo; n++)
                Extract(Files[n], Paths[n]);
        }
        /// <summary>已重载。从包中解出多个文件或文件夹。调用ExtractMultipleInner。</summary>
        public void ExtractMultiple(FileDB[] Files, string[] Paths)
        {
            if (Files.Length != Paths.Length)
                throw new ArgumentException();
            for (int n = 0, loopTo = Files.Length - 1; n <= loopTo; n++)
            {
                string Dir = FileNameHandling.GetFileDirectory(Paths[n]);
                if (!string.IsNullOrEmpty(Dir) && !Directory.Exists(Dir))
                    Directory.CreateDirectory(Dir);
            }
            ExtractMultipleInner(Files, Paths);
        }
        /// <summary>已重载。从包中解出多个文件或文件夹。调用ExtractMultipleInner。</summary>
        public void ExtractMultiple(FileDB[] Files, string[] Paths, string Mask)
        {
            if (Files.Length != Paths.Length)
                throw new ArgumentException();
            var FilesMatchMask = new List<FileDB>();
            var PathsOfMatched = new List<string>();
            for (int n = 0, loopTo = Files.Length - 1; n <= loopTo; n++)
            {
                var File = Files[n];
                if (FileNameHandling.IsMatchFileMask(File.Name, Mask))
                {
                    string Dir = FileNameHandling.GetFileDirectory(Paths[n]);
                    if (!string.IsNullOrEmpty(Dir) && !Directory.Exists(Dir))
                        Directory.CreateDirectory(Dir);
                    FilesMatchMask.Add(File);
                    PathsOfMatched.Add(Paths[n]);
                }
            }
            ExtractMultipleInner(FilesMatchMask.ToArray(), PathsOfMatched.ToArray());
        }

        /// <summary>已重载。替换包中的一个文件。</summary>
        public abstract void Replace(FileDB File, ZeroPositionStreamPasser sp);
        /// <summary>已重载。替换包中的一个文件。Replace(ByVal File As FileDB, ByVal sp As ZeroPositionStreamPasser)。</summary>
        public void Replace(FileDB File, string Path)
        {
            using (var s = new StreamEx(Path, FileMode.Open, FileAccess.Read))
            {
                Replace(File, s);
            }
        }
        /// <summary>已重载。替换包中的一个文件。调用Replace(ByVal File As FileDB, ByVal sp As ZeroPositionStreamPasser)。</summary>
        public void Replace(FileDB File, byte[] Bin)
        {
            using (var s = new ByteArrayStream(Bin))
            {
                Replace(File, s);
            }
        }
        /// <summary>替换包中的多个文件。默认实现调用Replace(ByVal File As FileDB, ByVal Path As String)。</summary>
        protected virtual void ReplaceMultipleInner(FileDB[] Files, string[] Paths)
        {
            for (int n = 0, loopTo = Files.Length - 1; n <= loopTo; n++)
                Replace(Files[n], Paths[n]);
        }
        /// <summary>替换包中的多个文件。调用ReplaceMultipleInner。</summary>
        public void ReplaceMultiple(FileDB[] Files, string[] Paths)
        {
            if (Files.Length != Paths.Length)
                throw new ArgumentException();
            ReplaceMultipleInner(Files, Paths);
        }

        /// <summary>关闭包。</summary>
        public void Close()
        {
            Dispose(true);
        }

        #region  IDisposable 支持 
        /// <summary>释放托管对象或间接非托管对象(Stream等)。可在这里将大型字段设置为 null。</summary>
        protected virtual void DisposeManagedResource()
        {
            if (BaseStream is not null)
                BaseStream.Dispose();
            BaseStream = null;
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

    /// <summary>文件信息</summary>
    public class FileDB
    {
        protected string NameValue;
        protected FileType TypeValue;
        protected long LengthValue;
        protected long AddressValue;
        protected string TitleNameValue;

        /// <summary>文件名</summary>
        public virtual string Name
        {
            get
            {
                return NameValue;
            }
            set
            {
                NameValue = value;
            }
        }
        /// <summary>文件类型：文件 文件夹</summary>
        public virtual FileType Type
        {
            get
            {
                return TypeValue;
            }
            set
            {
                TypeValue = value;
            }
        }
        /// <summary>文件长度</summary>
        public virtual long Length
        {
            get
            {
                return LengthValue;
            }
            set
            {
                LengthValue = value;
            }
        }
        /// <summary>文件地址</summary>
        public virtual long Address
        {
            get
            {
                return AddressValue;
            }
            set
            {
                AddressValue = value;
            }
        }
        /// <summary>显示用文件名，如果为空会返回Name</summary>
        public virtual string TitleName
        {
            get
            {
                if (string.IsNullOrEmpty(TitleNameValue))
                    return Name;
                return TitleNameValue;
            }
            set
            {
                TitleNameValue = value;
            }
        }

        public FileDB ParentFileDB;
        public List<FileDB> SubFile = new List<FileDB>();
        public Dictionary<string, FileDB> SubFileNameRef = new Dictionary<string, FileDB>(StringComparer.CurrentCultureIgnoreCase);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FileDB(string Name, FileType Type, long Length, long Address, string TitleName = "")
        {
            this.Name = Name;
            this.Type = Type;
            this.Length = Length;
            this.Address = Address;
            this.TitleName = TitleName;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FileDB()
        {
            Type = FileType.File;
        }
        public string Path
        {
            get
            {
                var Ancestor = ParentFileDB;
                string ret = Name;
                while (Ancestor is not null)
                {
                    ret = FileNameHandling.GetPath(Ancestor.Name, ret);
                    Ancestor = Ancestor.ParentFileDB;
                }
                return ret;
            }
        }
        public enum FileType : int
        {
            File = 0,
            Directory = 1
        }
        public static FileDB CreateFile(string Name, long Length, long Address, string TitleName = "")
        {
            return new FileDB(Name, FileType.File, Length, Address, TitleName);
        }
        public static FileDB CreateDirectory(string Name, string TitleName = "")
        {
            return new FileDB(Name, FileType.Directory, 0L, -1, TitleName);
        }
    }

    /// <summary>文件信息开始地址比较器</summary>
    public class FileDBAddressComparer : IComparer<FileDB>
    {
        private static FileDBAddressComparer _def = new FileDBAddressComparer();
        public static FileDBAddressComparer Default
        {
            get
            {
                return _def;
            }
        }
        public int Compare(FileDB x, FileDB y)
        {
            if (x.Address < y.Address)
                return -1;
            if (x.Address > y.Address)
                return 1;
            if (x.Length < y.Length)
                return -1;
            if (x.Length > y.Length)
                return 1;
            return string.Compare(x.Name, y.Name);
        }
    }

    /// <summary>
    /// 注册文件包类型
    /// 至少应该包含打开
    /// </summary>
    public sealed class PackageRegister
    {
        private PackageRegister()
        {
        }

        /// <summary>打开文件包。</summary>
        public delegate PackageBase PackageOpen(ZeroPositionStreamPasser s);
        public delegate PackageBase PackageOpenWithPath(string Path);
        /// <summary>创建新的文件包。</summary>
        public delegate PackageBase PackageCreate(ZeroLengthStreamPasser s, string Directory);
        public delegate PackageBase PackageCreateWithPath(string Path, string Directory);

        private class PackageInfo
        {
            public string Filter;
            public PackageOpen Open;
            public PackageOpenWithPath OpenWithPath;
            public PackageCreate Create;
            public PackageCreateWithPath CreateWithPath;
        }

        private static Dictionary<string, PackageInfo> Packages = new Dictionary<string, PackageInfo>();
        private static List<PackageInfo> Readables = new List<PackageInfo>();
        private static List<PackageInfo> Writables = new List<PackageInfo>();

        /// <summary>
        /// 注册一个包类型
        /// </summary>
        /// <param name="Filter">
        /// 文件包文件名筛选器，应按照“Package File(*.Package)|*.Package”的格式书写。
        /// </param>
        public static void Register(string Filter, PackageOpen Open, PackageCreate Create = null)
        {
            PackageInfo p;
            if (!Packages.ContainsKey(Filter))
            {
                if (Open is null)
                    throw new ArgumentNullException("OpenFunctionNull");
                p = new PackageInfo() { Filter = Filter, Open = Open, Create = Create };
                Packages.Add(Filter, p);
            }
            else
            {
                p = Packages[Filter];
                if (Open is null && p.Open is null && p.OpenWithPath is null)
                    throw new ArgumentNullException("OpenFunctionNull");
                p.Open = Open;
                if (Create is not null)
                    p.Create = Create;
            }
            if (!Readables.Contains(p))
                Readables.Add(p);
            if (Create is not null && !Writables.Contains(p))
                Writables.Add(p);
        }
        public static void Register(string Filter, PackageOpenWithPath Open, PackageCreateWithPath Create = null)
        {
            PackageInfo p;
            if (!Packages.ContainsKey(Filter))
            {
                if (Open is null)
                    throw new ArgumentNullException("OpenFunctionNull");
                p = new PackageInfo() { Filter = Filter, OpenWithPath = Open, CreateWithPath = Create };
                Packages.Add(Filter, p);
            }
            else
            {
                p = Packages[Filter];
                if (Open is null && p.Open is null && p.OpenWithPath is null)
                    throw new ArgumentNullException("OpenFunctionNull");
                p.OpenWithPath = Open;
                if (Create is not null)
                    p.CreateWithPath = Create;
            }
            if (!Readables.Contains(p))
                Readables.Add(p);
            if (Create is not null && !Writables.Contains(p))
                Writables.Add(p);
        }
        public static void Unregister(string Filter)
        {
            var p = Packages[Filter];
            Packages.Remove(Filter);
            Readables.Remove(p);
            if (Writables.Contains(p))
                Writables.Remove(p);
        }
        public static PackageBase Open(int Index, ZeroPositionStreamPasser sp)
        {
            if (Readables[Index].Open is not null)
            {
                return Readables[Index].Open(sp);
            }
            else
            {
                throw new InvalidOperationException("NoOpenWithStreamFunction");
            }
        }
        public static PackageBase Open(int Index, string Path)
        {
            if (Readables[Index].OpenWithPath is null)
            {
                if (Conversions.ToBoolean(File.GetAttributes(Path) & FileAttributes.ReadOnly))
                {
                    return Open(Index, new StreamEx(Path, FileMode.Open, FileAccess.Read));
                }
                else
                {
                    return Open(Index, new StreamEx(Path, FileMode.Open, FileAccess.ReadWrite));
                }
            }
            else
            {
                return Readables[Index].OpenWithPath(Path);
            }
        }
        public static PackageBase Create(int WritableIndex, ZeroLengthStreamPasser sp, string Directory)
        {
            if (Writables[WritableIndex].Create is not null)
            {
                return Writables[WritableIndex].Create(sp, Directory);
            }
            else
            {
                throw new InvalidOperationException("NoCreateWithStreamFunction");
            }
        }
        public static PackageBase Create(int WritableIndex, string Path, string Directory)
        {
            if (Writables[WritableIndex].OpenWithPath is null)
            {
                return Create(WritableIndex, new StreamEx(Path, FileMode.Create, FileAccess.ReadWrite), Directory);
            }
            else
            {
                return Writables[WritableIndex].CreateWithPath(Path, Directory);
            }
        }
        public static string GetFilter()
        {
            var sb = new System.Text.StringBuilder();
            foreach (var p in Readables)
            {
                sb.Append(p.Filter);
                sb.Append("|");
            }
            return sb.ToString().TrimEnd('|');
        }
        public static string GetWritableFilter()
        {
            var sb = new System.Text.StringBuilder();
            foreach (var p in Writables)
            {
                sb.Append(p.Filter);
                sb.Append("|");
            }
            return sb.ToString().TrimEnd('|');
        }
        public static int PackageTypeCount
        {
            get
            {
                return Packages.Count;
            }
        }
    }
}