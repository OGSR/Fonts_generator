// ==========================================================================
// 
// File:        PCK.vb
// Location:    Firefly.Packaging <Visual Basic .Net>
// Description: PCK文件流类(一个标准的文件包)
// Version:     2010.03.28.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System.Collections.Generic;
using System.IO;
using static System.Math;

namespace Firefly.Packaging
{
    /// <summary>PCK文件流类</summary>
    /// <remarks>
    /// 用于打开和创建盟军2的PCK文件
    /// </remarks>
    public class PCK : PackageFixedAddress
    {
        public PCK(ZeroPositionStreamPasser sp) : base(sp)
        {
            BaseStream.Position = 0L;
            RootValue = OpenFileDB();
        }

        protected FileDB OpenFileDB()
        {
            var ret = new FileDB();
            PhysicalPosition.Add(ret, (int)BaseStream.Position);
            var s = BaseStream;
            ret.Name = s.ReadSimpleString(36);
            ret.Type = (FileDB.FileType)s.ReadInt32();
            ret.Length = s.ReadInt32();
            ret.Address = s.ReadInt32();
            if (ret.Type == FileDB.FileType.Directory)
            {
                s.Position = ret.Address;
                while (true)
                {
                    var f = OpenFileDB();
                    if ((int)f.Type == 255)
                        break;
                    f.ParentFileDB = ret;
                    ret.SubFile.Add(f);
                    ret.SubFileNameRef.Add(f.Name, f);
                }
            }
            return ret;
        }

        protected const int DBLength = 48;
        public void WriteFileDB(FileDB File)
        {
            PhysicalPosition.Add(File, (int)BaseStream.Position);
            var s = BaseStream;
            s.WriteSimpleString(File.Name, 36);
            s.WriteInt32((int)File.Type);
            s.WriteInt32((int)File.Length);
            s.WriteInt32((int)File.Address);
        }

        public PCK(ZeroLengthStreamPasser sp, string Directory)
        {
            var s = sp.GetStream();
            BaseStream = s;

            var FileQueue = new Queue<FileDB>();
            var FileLengthAddressPointerQueue = new Queue<int>();
            var FilePathQueue = new Queue<string>();
            var FileLengthQueue = new Queue<int>();
            var FileAddressQueue = new Queue<int>();

            string RootName = FileNameHandling.GetFileName(Directory);
            if (RootName.Length > 36)
                throw new InvalidDataException(Directory);

            s.SetLength(16777216L);
            var cFileDB = CreateDirectory(RootName, DBLength);
            RootValue = cFileDB;
            WriteFileDB(cFileDB);
            ImportDirectory(FileNameHandling.GetFileDirectory(Directory), cFileDB, FileQueue, FileLengthAddressPointerQueue, FilePathQueue);
            WriteFileDB(CreateDirectoryEnd());

            foreach (string f in FilePathQueue)
            {
                using (var File = new StreamEx(f, FileMode.Open, FileAccess.Read))
                {
                    GotoNextFilePoint();
                    FileLengthQueue.Enqueue((int)File.Length);
                    FileAddressQueue.Enqueue((int)s.Position);
                    if (s.Length - s.Position < File.Length)
                    {
                        s.SetLength((long)Round(s.Length + NumericOperations.Max(16777216d, Ceiling(File.Length / 16777216d) * 16777216d)));
                    }
                    s.WriteFromStream(File, File.Length);
                }
            }
            GotoNextFilePoint();
            s.SetLength(s.Position);

            FileDB fn;
            int pl;
            int pa;
            foreach (int p in FileLengthAddressPointerQueue)
            {
                s.Position = p;
                fn = FileQueue.Dequeue();
                pl = FileLengthQueue.Dequeue();
                pa = FileAddressQueue.Dequeue();
                fn.Length = pl;
                fn.Address = pa;
                s.WriteInt32(pl);
                s.WriteInt32(pa);
            }
            s.Position = 0L;
        }
        private void ImportDirectory(string Dir, FileDB DirDB, Queue<FileDB> FileQueue, Queue<int> FileLengthAddressPointerQueue, Queue<string> FilePathQueue)
        {
            var s = BaseStream;
            FileDB cFileDB;
            string Name;
            foreach (string f in Directory.GetFiles(FileNameHandling.GetPath(Dir, DirDB.Name)))
            {
                Name = FileNameHandling.GetFileName(f);
                if (Name.Length > 36)
                    throw new InvalidDataException(f);
                cFileDB = FileDB.CreateFile(Name, -1, -1);
                WriteFileDB(cFileDB);
                cFileDB.ParentFileDB = DirDB;
                DirDB.SubFile.Add(cFileDB);
                DirDB.SubFileNameRef.Add(cFileDB.Name, cFileDB);
                FileQueue.Enqueue(cFileDB);
                FileLengthAddressPointerQueue.Enqueue((int)(s.Position - 8L));
                FilePathQueue.Enqueue(f);
            }
            foreach (string d in Directory.GetDirectories(FileNameHandling.GetPath(Dir, DirDB.Name)))
            {
                Name = FileNameHandling.GetFileName(d);
                if (Name.Length > 36)
                    throw new InvalidDataException(d);
                cFileDB = CreateDirectory(Name, (int)(s.Position + DBLength));
                WriteFileDB(cFileDB);
                cFileDB.ParentFileDB = DirDB;
                DirDB.SubFile.Add(cFileDB);
                DirDB.SubFileNameRef.Add(cFileDB.Name, cFileDB);
                ImportDirectory(FileNameHandling.GetFileDirectory(d), cFileDB, FileQueue, FileLengthAddressPointerQueue, FilePathQueue);
                WriteFileDB(CreateDirectoryEnd());
            }
        }
        public void GotoNextFilePoint()
        {
            long NewPosition = (BaseStream.Position / 0x800L + 1L) * 0x800L;
            while (BaseStream.Position < NewPosition)
                BaseStream.WriteByte(0);
        }
        public static FileDB CreateDirectory(string Name, int Address)
        {
            return new FileDB(Name, (FileDB.FileType)1, int.MinValue + 0x7FFFFFFF, Address);
        }
        public static FileDB CreateDirectoryEnd()
        {
            return new FileDB(null, (FileDB.FileType)255, int.MinValue + 0x7FFFFFFF, int.MinValue + 0x7FFFFFFF);
        }

        protected Dictionary<FileDB, int> PhysicalPosition = new Dictionary<FileDB, int>();
        public override long get_FileLengthInPhysicalFileDB(FileDB File)
        {
            BaseStream.Position = PhysicalPosition[File] + 40;
            return BaseStream.ReadInt32();
        }

        public override void set_FileLengthInPhysicalFileDB(FileDB File, long value)
        {
            BaseStream.Position = PhysicalPosition[File] + 40;
            BaseStream.WriteInt32((int)value);
        }

        public static string Filter
        {
            get
            {
                return "PCK(*.PCK)|*.PCK";
            }
        }
        public static PackageBase Open(ZeroPositionStreamPasser sp)
        {
            return new PCK(sp);
        }
        public static PackageBase Create(ZeroLengthStreamPasser sp, string Directory)
        {
            return new PCK(sp, Directory);
        }
    }
}