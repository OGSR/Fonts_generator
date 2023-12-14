// ==========================================================================
// 
// File:        ISO.vb
// Location:    Firefly.Packaging <Visual Basic .Net>
// Description: ISO类
// Version:     2010.02.28.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualBasic.CompilerServices;

namespace Firefly.Packaging
{
    public class ISO : PackageDiscrete
    {

        protected int LogicalBlockSize = 0x800;
        protected IsoPrimaryDescriptor PrimaryDescriptor;

        protected Dictionary<FileDB, long> PhysicalAdressAddressOfFile = new Dictionary<FileDB, long>();
        protected Dictionary<FileDB, long> PhysicalLengthAddressOfFile = new Dictionary<FileDB, long>();

        protected long DataScanStart = 0L;

        public ISO(ZeroPositionStreamPasser sp) : base(sp)
        {
            var s = sp.GetStream();
            for (int n = 16; n <= int.MaxValue; n++)
            {
                BaseStream.Position = 0x800 * n;
                byte Type = BaseStream.ReadByte();
                BaseStream.Position -= 1L;
                bool exitFor = false;
                switch (Type)
                {
                    case 1:
                        {
                            if (PrimaryDescriptor is not null)
                                throw new InvalidDataException();
                            PrimaryDescriptor = new IsoPrimaryDescriptor(BaseStream);
                            break;
                        }
                    case 255:
                        {
                            exitFor = true;
                            break;
                        }
                }

                if (exitFor)
                {
                    break;
                }
            }
            LogicalBlockSize = PrimaryDescriptor.LogicalBlockSize;
            DataScanStart = s.Position;
            RootValue = ToFileDB(PrimaryDescriptor.RootDirectoryRecord, LogicalBlockSize);
            DataScanStart = GetSpace(DataScanStart);
            ScanHoles(DataScanStart);
        }

        public static string Filter
        {
            get
            {
                return "ISO(*.ISO)|*.ISO";
            }
        }

        public static PackageBase OpenWithStream(ZeroPositionStreamPasser sp)
        {
            return new ISO(sp);
        }

        public static PackageBase Open(string Path)
        {
            StreamEx s;
            try
            {
                s = new StreamEx(Path, FileMode.Open, FileAccess.ReadWrite);
            }
            catch
            {
                s = new StreamEx(Path, FileMode.Open, FileAccess.Read);
            }
            return new ISO(s);
        }

        public override long get_FileAddressInPhysicalFileDB(FileDB File)
        {
            BaseStream.Position = PhysicalAdressAddressOfFile[File];
            return BaseStream.ReadInt32() * (long)LogicalBlockSize;
        }

        public override void set_FileAddressInPhysicalFileDB(FileDB File, long value)
        {
            BaseStream.Position = PhysicalAdressAddressOfFile[File];
            int ExtentLocation = DirectIntConvert.CID(value / LogicalBlockSize);
            BaseStream.WriteInt32(ExtentLocation);
            BaseStream.WriteInt32BigEndian(ExtentLocation);
        }

        public override long get_FileLengthInPhysicalFileDB(FileDB File)
        {
            BaseStream.Position = PhysicalLengthAddressOfFile[File];
            return BaseStream.ReadInt32();
        }

        public override void set_FileLengthInPhysicalFileDB(FileDB File, long value)
        {
            BaseStream.Position = PhysicalLengthAddressOfFile[File];
            int DataLength = DirectIntConvert.CID(value);
            BaseStream.WriteInt32(DataLength);
            BaseStream.WriteInt32BigEndian(DataLength);
        }

        protected sealed override long GetSpace(long Length)
        {
            return (Length + LogicalBlockSize - 1L) / LogicalBlockSize * LogicalBlockSize;
        }

        protected FileDB ToFileDB(IsoDirectoryRecord dr, int LogicalBlockSize)
        {
            var s = BaseStream;
            long CurrentPosition = s.Position;
            string FileName = dr.FileId;
            // 处理含有revision号的文件名
            if (dr.FileId.ToString().IndexOf(";") >= 0)
                FileName = dr.FileId.ToString().Substring(0, dr.FileId.ToString().IndexOf(";"));
            var ret = new FileDB(FileName, (FileDB.FileType)((dr.FileFlags & 2) >> 1), dr.DataLength, dr.ExtentLocation * (long)LogicalBlockSize);

            PhysicalAdressAddressOfFile.Add(ret, dr.PhysicalAdressAddress);
            PhysicalLengthAddressOfFile.Add(ret, dr.PhysicalLengthAddress);

            ret.ParentFileDB = null;
            if (dr.IsDirectory())
            {
                s.Position = dr.ExtentLocation * LogicalBlockSize;
                byte CurrentDirectoryLength = s.ReadByte();
                s.Position += CurrentDirectoryLength - 1;
                byte ParentDirectoryLength = s.ReadByte();
                s.Position += ParentDirectoryLength - 1;

                byte Length = s.ReadByte();
                s.Position -= 1L;
                while (Length != 0)
                {
                    var r = new IsoDirectoryRecord(s);
                    var f = ToFileDB(r, LogicalBlockSize);
                    PushFile(f, ret);
                    Length = s.ReadByte();
                    s.Position -= 1L;
                }
                DataScanStart = NumericOperations.Max(s.Position, DataScanStart);
            }
            s.Position = CurrentPosition;
            return ret;
        }
    }

    public class IsoPrimaryDescriptor
    {
        public byte Type;
        public IsoAnsiString Id;
        public byte Version;

        public IsoAnsiString SystemId;
        public IsoAnsiString VolumeId;

        public int VolumeSpaceSize;

        public short VolumeSetSize;
        public short VolumeSequenceNumber;
        public short LogicalBlockSize;
        public int PathTableSize;
        public short TypeLPathTable;
        public short OptTypeLPathTable;
        public short TypeMPathTable;
        public short OptTypeMPathTable;
        public IsoDirectoryRecord RootDirectoryRecord;
        public IsoAnsiString VolumeSetId;
        public IsoAnsiString PublisherId;
        public IsoAnsiString PreparerId;
        public IsoAnsiString ApplicationId;
        public IsoAnsiString CopyrightFileId;
        public IsoAnsiString AbstractFileId;
        public IsoAnsiString BibliographicFileId;
        public IsoAnsiString CreationDate;
        public IsoAnsiString ModificationDate;
        public IsoAnsiString ExptrationDate;
        public IsoAnsiString EffectiveDate;
        public byte FileStructureVersion;

        public IsoAnsiString ApplicationData;


        public IsoPrimaryDescriptor(PositionedStreamPasser sp)
        {
            var s = sp.GetStream();
            Type = s.ReadByte();
            Id = s.Read(5);
            Version = s.ReadByte();
            s.Position += 1L;
            SystemId = s.Read(32);
            VolumeId = s.Read(32);
            s.Position += 8L;
            VolumeSpaceSize = s.ReadInt32();
            s.ReadInt32BigEndian();
            s.Position += 32L;
            VolumeSetSize = s.ReadInt16();
            s.ReadInt16BigEndian();
            VolumeSequenceNumber = s.ReadInt16();
            s.ReadInt16BigEndian();
            LogicalBlockSize = s.ReadInt16();
            s.ReadInt16BigEndian();
            PathTableSize = s.ReadInt32();
            s.ReadInt32BigEndian();
            TypeLPathTable = s.ReadInt16();
            s.ReadInt16BigEndian();
            OptTypeLPathTable = s.ReadInt16();
            s.ReadInt16BigEndian();
            TypeMPathTable = s.ReadInt16();
            s.ReadInt16BigEndian();
            OptTypeMPathTable = s.ReadInt16();
            s.ReadInt16BigEndian();
            RootDirectoryRecord = new IsoDirectoryRecord(s);
            VolumeSetId = s.Read(128);
            PublisherId = s.Read(128);
            PreparerId = s.Read(128);
            ApplicationId = s.Read(128);
            CopyrightFileId = s.Read(37);
            AbstractFileId = s.Read(37);
            BibliographicFileId = s.Read(37);
            CreationDate = s.Read(17);
            ModificationDate = s.Read(17);
            ExptrationDate = s.Read(17);
            EffectiveDate = s.Read(17);
            FileStructureVersion = s.ReadByte();
            s.Position += 1L;
            ApplicationData = s.Read(512);
            s.Position += 653L;
        }
    }

    public class IsoDirectoryRecord
    {
        public byte Length;
        public byte ExtAttrLength;
        public int ExtentLocation;
        public int DataLength;
        public IsoAnsiString RecordingDateAndTime;
        public byte FileFlags;
        public byte FileUnitSize;
        public byte InterleaveGapSize;
        public short VolumeSequenceNumber;
        public byte FileIdLen;
        public IsoAnsiString FileId;

        public long PhysicalAdressAddress;
        public long PhysicalLengthAddress;
        public IsoDirectoryRecord(PositionedStreamPasser sp)
        {
            var s = sp.GetStream();
            long CurrentPosition = s.Position;
            Length = s.ReadByte();
            ExtAttrLength = s.ReadByte();

            PhysicalAdressAddress = s.Position;
            ExtentLocation = s.ReadInt32();
            s.ReadInt32BigEndian();

            PhysicalLengthAddress = s.Position;
            DataLength = s.ReadInt32();
            s.ReadInt32BigEndian();

            RecordingDateAndTime = s.Read(7);
            FileFlags = s.ReadByte();
            FileUnitSize = s.ReadByte();
            InterleaveGapSize = s.ReadByte();
            VolumeSequenceNumber = s.ReadInt16();
            s.ReadInt16BigEndian();
            FileIdLen = s.ReadByte();
            FileId = s.Read(FileIdLen);
            if ((FileIdLen & 1) == 0)
                s.Position += 1L;
            s.Position = CurrentPosition + Length;
        }
        public bool IsDirectory()
        {
            return Conversions.ToBoolean(FileFlags & 2);
        }
    }

    [DebuggerDisplay("{ToString()}")]
    public class IsoAnsiString
    {
        public byte[] Data;

        public IsoAnsiString(byte[] Data)
        {
            this.Data = Data;
        }

        public override string ToString()
        {
            var d = new List<byte>();
            foreach (var b in Data)
            {
                if (b == 0)
                    break;
                d.Add(b);
            }
            return Conversions.ToString(TextEncoding.TextEncoding.Default.GetChars(d.ToArray())).TrimEnd(' ');
        }

        public static implicit operator string(IsoAnsiString s)
        {
            return s.ToString();
        }

        public static implicit operator IsoAnsiString(byte[] b)
        {
            return new IsoAnsiString(b);
        }

        public static implicit operator IsoAnsiString(string s)
        {
            return new IsoAnsiString(TextEncoding.TextEncoding.Default.GetBytes(s));
        }
    }
}