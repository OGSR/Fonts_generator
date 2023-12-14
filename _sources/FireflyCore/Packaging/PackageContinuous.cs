// ==========================================================================
// 
// File:        PackageContinuous.vb
// Location:    Firefly.Packaging <Visual Basic .Net>
// Description: 连续数据文件包
// Version:     2010.03.17.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;

namespace Firefly.Packaging
{
    /// <summary>
    /// 连续数据文件包，通常用于需改变文件索引的文件包，且文件包的数据顺序需要保持和索引一致
    /// 若无需数据顺序和索引一致，请考虑PackageDiscrete，这通常能够减少修改包所需的时间
    /// 当前实现假设索引在文件包头部或外部，因此无需备份。如果不是这样，则需要修改某些行为
    /// 
    /// 
    /// 给继承者的说明：
    /// 
    /// 文件包支持写入，应
    /// (1)重写FileLengthInPhysicalFileDB、GetSpace方法
    /// (2)在加入一个FileDB时，调用PushFile方法，使得它被加入到FileList、IndexOfFile、FileSetAddressSorted中，以及PushFileToDir到根目录FileDB中，若根目录FileDB不存在，则空的根目录会自动创建
    /// 
    /// 如果需要启用进度通知功能，请设置委托函数NotifyProgress，参数值在[0, 1]上。
    /// 
    /// 请使用PackageRegister来注册文件包类型。
    /// 应提供一个返回"ISO(*.ISO)|*.ISO"形式字符串的Filter属性，
    /// 并按照PackageRegister中的委托类型提供一个Open函数、一个Create函数(如果支持创建)。
    /// </summary>
    public abstract class PackageContinuous : PackageBase
    {

        /// <summary>按照地址排序的文件集。</summary>
        protected SortedList<FileDB, long> FileSetAddressSorted = new SortedList<FileDB, long>(FileDBAddressComparer.Default);
        /// <summary>设置该委托函数，可以启用进度通知功能，参数值在[0, 1]上。</summary>
        public Action<double> NotifyProgress;

        /// <summary>从文件包读取FileDB文件长度和写入文件长度到文件包。用于替换文件包时使用。</summary>
        public abstract long get_FileLengthInPhysicalFileDB(FileDB File);
        public abstract void set_FileLengthInPhysicalFileDB(FileDB File, long value);

        /// <summary>
        /// 返回一个长度的文件所占的空间，通常用于对齐。
        /// 比如800h对齐的文件，应该返回((Length + 800h - 1) \ 800h) * 800h
        /// </summary>
        protected abstract long GetSpace(long Length);


        /// <summary>已重载。默认构照函数。请手动初始化BaseStream。</summary>
        protected PackageContinuous() : base()
        {
        }
        /// <summary>已重载。打开或创建文件包。</summary>
        public PackageContinuous(ZeroPositionStreamPasser sp) : base(sp)
        {
        }

        /// <summary>把文件FileDB放入根目录FileDB。若根目录FileDB不存在，则空的根目录会自动创建。在加入一个FileDB时，调用该方法，使得它被加入到FileList、IndexOfFile、FileSetAddressSorted中，以及PushFileToDir到根目录FileDB中。</summary>
        protected override void PushFile(FileDB f, FileDB Directory)
        {
            base.PushFile(f, Directory);
            FileSetAddressSorted.Add(f, f.Address);
        }

        /// <summary>已重载。替换包中的一个文件。</summary>
        public override void Replace(FileDB File, ZeroPositionStreamPasser sp)
        {
            var s = sp.GetStream();
            string TempFileName = Path.GetTempFileName();
            using (var Temp = new StreamEx(TempFileName, FileMode.Create, FileAccess.ReadWrite))
            {
                Temp.WriteFromStream(s, s.Length);
            }

            ReplaceMultipleInner(new FileDB[] { File }, new string[] { TempFileName });

            System.IO.File.Delete(TempFileName);
        }

        /// <summary>替换包中的多个文件。默认实现调用ReplaceMultipleInner。</summary>
        public void ReplaceMultiple(int[] FileNumbers, string Directory)
        {
            var Files = new List<FileDB>();
            var Paths = new List<string>();
            foreach (var n in FileNumbers)
            {
                var f = FileList[n - 1];
                Files.Add(f);
                Paths.Add(FileNameHandling.GetPath(Directory, f.Path));
            }
            ReplaceMultipleInner(Files.ToArray(), Paths.ToArray());
        }

        protected override void ReplaceMultipleInner(FileDB[] Files, string[] Paths)
        {
            var FileIndex = new List<int>();
            foreach (FileDB File in Files)
            {
                if (File.Length != this.get_FileLengthInPhysicalFileDB(File))
                    throw new ArgumentException("PhysicalFileLengthErrorPointing");
                FileIndex.Add(IndexOfFile[File]);
            }

            // 将数据分成头段、替换段、尾段
            // 头段不变，尾段移动，替换段替换
            // 寻找替换段起始与结束
            int Min = FileList.Count - 1;
            int Max = 0;
            var ReplaceBlockLengthList = new SortedList<int, long>();
            long TotalLengthDiff = 0L;
            for (int k = 0, loopTo = FileIndex.Count - 1; k <= loopTo; k++)
            {
                int n = FileIndex[k];
                if (n < Min)
                    Min = n;
                if (n > Max)
                    Max = n;
                var f = FileList[n];
                using (var fs = new StreamEx(Paths[k], FileMode.Open, FileAccess.ReadWrite))
                {
                    ReplaceBlockLengthList.Add(n, fs.Length);
                    TotalLengthDiff = TotalLengthDiff - GetSpace(f.Length) + GetSpace(fs.Length);
                }
            }

            // 构建替换文件表与替换段保留文件表
            var ReplaceList = new SortedList<int, FileDB>();
            var ReplacePathList = new SortedList<int, string>();
            for (int k = 0, loopTo1 = FileIndex.Count - 1; k <= loopTo1; k++)
            {
                int n = FileIndex[k];
                ReplaceList.Add(n, FileList[n]);
                ReplacePathList.Add(n, Paths[k]);
            }
            var PreserveList = new SortedList<int, FileDB>();
            for (int i = Min, loopTo2 = Max; i <= loopTo2; i++)
            {
                if (ReplaceList.ContainsKey(i))
                    continue;
                ReplaceBlockLengthList.Add(i, FileList[i].Length);
                PreserveList.Add(i, FileList[i]);
            }

            // 构建替换后地址表
            long ReplaceBlockStart = FileList[Min].Address;
            long TailBlockStart;
            if (Max == FileList.Count - 1)
            {
                TailBlockStart = BaseStream.Length;
            }
            else
            {
                TailBlockStart = FileList[Max + 1].Address;
            }
            long TailBlockLength = BaseStream.Length - TailBlockStart;
            // Dim ReplaceBlockLength As Int64 = TailBlockStart - ReplaceBlockStart

            var ReplaceBlockSpaceList = new List<long>();
            foreach (var v in ReplaceBlockLengthList.Values)
                ReplaceBlockSpaceList.Add(GetSpace(v));
            long[] ReplaceBlockAddressArray = FileLengthUtility.GetAddressSummation(ReplaceBlockStart, ReplaceBlockSpaceList.ToArray());

            // 生成保留文件临时文件
            string TempFileName = Path.GetTempFileName();
            using (var TempFile = new StreamEx(TempFileName, FileMode.Create, FileAccess.ReadWrite))
            {
                foreach (var Pair in PreserveList)
                {
                    {
                        var withBlock = Pair.Value;
                        using (var f = new PartialStreamEx(BaseStream, withBlock.Address, withBlock.Length))
                        {
                            TempFile.WriteFromStream(f, withBlock.Length);
                        }
                    }
                }

                // 移动尾段
                if (TotalLengthDiff > 0L)
                {
                    BaseStream.SetLength(BaseStream.Length + TotalLengthDiff);
                }
                MoveData(TailBlockStart, TailBlockLength, TailBlockStart + TotalLengthDiff);
                if (TotalLengthDiff < 0L)
                {
                    BaseStream.SetLength(BaseStream.Length + TotalLengthDiff);
                }

                // 更改尾段数据地址
                for (int i = Max + 1, loopTo3 = FileList.Count - 1; i <= loopTo3; i++)
                {
                    var f = FileList[i];
                    f.Address += TotalLengthDiff;
                }

                // 填入保留文件数据
                TempFile.Position = 0L;
                foreach (var Pair in PreserveList)
                {
                    {
                        var withBlock1 = Pair.Value;
                        using (var f = new PartialStreamEx(BaseStream, ReplaceBlockAddressArray[Pair.Key - Min], GetSpace(withBlock1.Length)))
                        {
                            TempFile.ReadToStream(f, withBlock1.Length);
                            while (f.Position < f.Length)
                                f.WriteByte(0);
                        }
                    }
                }
            }

            // 填入替换文件数据
            foreach (var Pair in ReplaceList)
            {
                string p = ReplacePathList[Pair.Key];
                using (var f = new PartialStreamEx(BaseStream, ReplaceBlockAddressArray[Pair.Key - Min], GetSpace(ReplaceBlockLengthList[Pair.Key])))
                {
                    using (var fs = new StreamEx(p, FileMode.Open, FileAccess.ReadWrite))
                    {
                        fs.ReadToStream(f, ReplaceBlockLengthList[Pair.Key]);
                        while (f.Position < f.Length)
                            f.WriteByte(0);
                    }
                }
            }

            // 更改替换段文件数据地址，更改Index中替换文件数据长度
            for (int i = Min, loopTo4 = Max; i <= loopTo4; i++)
            {
                var f = FileList[i];
                f.Address = ReplaceBlockAddressArray[i - Min];
                f.Length = ReplaceBlockLengthList[i];
                this.set_FileLengthInPhysicalFileDB(f, f.Length);
            }

            // 删除临时文件
            File.Delete(TempFileName);
        }

        /// <summary>物理移动数据。请勿轻易直接调用。</summary>
        protected void MoveData(long Address, long Length, long NewAddress)
        {
            if (NotifyProgress is not null)
                NotifyProgress(0d);

            long Diff = NewAddress - Address;
            if (Diff == 0L)
                return;
            if (Length == 0L)
                return;
            if (Length < 0L)
                throw new ArgumentException();

            int BufferSize = 4 * (1 << 20);
            byte[] Buffer = new byte[BufferSize];

            long FirstAddress = Address;
            long SecondAddress = (FirstAddress + BufferSize - 1L) / BufferSize * BufferSize;
            int FirstLength = (int)(SecondAddress - FirstAddress);
            int LastLength = (int)((Address + Length) % BufferSize);
            long LastAddress = Address + Length - LastLength;

            if (FirstAddress >= LastAddress)
            {
                FirstLength = (int)Length;
                LastLength = 0;
            }

            if (Diff > 0L)
            {
                if (LastLength > 0)
                {
                    BaseStream.Position = LastAddress;
                    BaseStream.Read(Buffer, 0, LastLength);
                    BaseStream.Position = LastAddress + Diff;
                    BaseStream.Write(Buffer, 0, LastLength);
                }

                for (long PieceAddress = LastAddress - BufferSize, loopTo = SecondAddress; (long)-BufferSize >= 0 ? PieceAddress <= loopTo : PieceAddress >= loopTo; PieceAddress += -BufferSize)
                {
                    if (NotifyProgress is not null)
                        NotifyProgress((LastAddress - PieceAddress) / (double)(LastAddress - SecondAddress));
                    BaseStream.Position = PieceAddress;
                    BaseStream.Read(Buffer, 0, BufferSize);
                    BaseStream.Position = PieceAddress + Diff;
                    BaseStream.Write(Buffer, 0, BufferSize);
                }

                if (FirstLength > 0)
                {
                    BaseStream.Position = FirstAddress;
                    BaseStream.Read(Buffer, 0, FirstLength);
                    BaseStream.Position = FirstAddress + Diff;
                    BaseStream.Write(Buffer, 0, FirstLength);
                }
            }
            else
            {
                if (FirstLength > 0)
                {
                    BaseStream.Position = FirstAddress;
                    BaseStream.Read(Buffer, 0, FirstLength);
                    BaseStream.Position = FirstAddress + Diff;
                    BaseStream.Write(Buffer, 0, FirstLength);
                }

                for (long PieceAddress = SecondAddress, loopTo1 = LastAddress - BufferSize; (long)BufferSize >= 0 ? PieceAddress <= loopTo1 : PieceAddress >= loopTo1; PieceAddress += BufferSize)
                {
                    if (NotifyProgress is not null)
                        NotifyProgress((PieceAddress - SecondAddress) / (double)(LastAddress - SecondAddress));
                    BaseStream.Position = PieceAddress;
                    BaseStream.Read(Buffer, 0, BufferSize);
                    BaseStream.Position = PieceAddress + Diff;
                    BaseStream.Write(Buffer, 0, BufferSize);
                }

                if (LastLength > 0)
                {
                    BaseStream.Position = LastAddress;
                    BaseStream.Read(Buffer, 0, LastLength);
                    BaseStream.Position = LastAddress + Diff;
                    BaseStream.Write(Buffer, 0, LastLength);
                }
            }

            if (NotifyProgress is not null)
                NotifyProgress(1d);
        }
    }
}