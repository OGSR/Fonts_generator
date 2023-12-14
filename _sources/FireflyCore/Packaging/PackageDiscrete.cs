// ==========================================================================
// 
// File:        PackageDiscrete.vb
// Location:    Firefly.Packaging <Visual Basic .Net>
// Description: 离散数据文件包
// Version:     2010.03.17.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;

namespace Firefly.Packaging
{
    /// <summary>
    /// 离散数据文件包，通常用于有完整地址索引和长度索引的文件包，能够在文件末尾和文件空白区扩充文件数据
    /// 若需要数据顺序和索引一致，请使用PackageContinuous，虽然这通常意味着要增加修改包所需的时间
    /// 
    /// 
    /// 给继承者的说明：
    /// 
    /// 文件包支持写入，应
    /// (1)重写FileAddressInPhysicalFileDB、FileLengthInPhysicalFileDB、GetSpace方法
    /// (2)在加入一个FileDB时，调用PushFile方法，使得它被加入到FileList、IndexOfFile、FileSetAddressSorted中，以及PushFileToDir到根目录FileDB中，若根目录FileDB不存在，则空的根目录会自动创建
    /// (3)最后执行ScanHoles，扫描出可以放置文件数据的洞
    /// 
    /// 请使用PackageRegister来注册文件包类型。
    /// 应提供一个返回"ISO(*.ISO)|*.ISO"形式字符串的Filter属性，
    /// 并按照PackageRegister中的委托类型提供一个Open函数、一个Create函数(如果支持创建)。
    /// </summary>
    public abstract class PackageDiscrete : PackageBase
    {

        /// <summary>文件包中的数据块(地址->文件数)。</summary>
        private SortedList<long, int> Blocks = new SortedList<long, int>();
        /// <summary>文件包中的空洞。</summary>
        private SortedList<Hole, int> Holes = new SortedList<Hole, int>(HoleComparer.Default);
        /// <summary>文件包中的空洞。</summary>
        private Dictionary<long, Hole> HoleMap = new Dictionary<long, Hole>();

        public abstract long get_FileAddressInPhysicalFileDB(FileDB File);

        public abstract void set_FileAddressInPhysicalFileDB(FileDB File, long value);

        public abstract long get_FileLengthInPhysicalFileDB(FileDB File);

        public abstract void set_FileLengthInPhysicalFileDB(FileDB File, long value);

        /// <summary>默认的文件数据起始地址</summary>
        private long DataStart = 0L;
        /// <summary>默认的文件对齐大小</summary>
        private long AlignmentBlockSize = 0L;

        private static long GCD(long a, long b)
        {
            if (a < 0L)
                a = -a;
            if (b < 0L)
                b = -b;
            if (a == 0L)
                return b;
            if (b == 0L)
                return a;

            if (a < b)
                NumericOperations.Exchange(ref a, ref b);

            while (true)
            {
                a = a % b;
                NumericOperations.Exchange(ref a, ref b);
                if (b == 0L)
                    return a;
            }
            throw new InvalidOperationException();
        }

        /// <summary>
        /// 返回一个地址的对齐位置，相对于开始位置。默认通过GetSpace与ScanHoles传入的DataStart计算。
        /// </summary>
        protected virtual long GetAlignedAddress(long Address)
        {
            return GetSpace(Address - DataStart) + DataStart;
        }

        /// <summary>
        /// 返回一个长度的文件所占的空间，通常用于对齐。
        /// 比如800h对齐的文件，应该返回((Length + 800h - 1) \ 800h) * 800h
        /// </summary>
        protected virtual long GetSpace(long Length)
        {
            if (AlignmentBlockSize <= 1L)
                return Length;
            return (Length + AlignmentBlockSize - 1L) / AlignmentBlockSize * AlignmentBlockSize;
        }


        /// <summary>已重载。默认构照函数。请手动初始化BaseStream。</summary>
        protected PackageDiscrete() : base()
        {
        }
        /// <summary>已重载。打开或创建文件包。</summary>
        public PackageDiscrete(ZeroPositionStreamPasser sp) : base(sp)
        {
        }

        private void SetBlockFileCount(long Address, int FileCount)
        {
            int OldFileCount = Blocks[Address];
            if (FileCount < 0)
                throw new ArgumentOutOfRangeException();
            if (OldFileCount < 0)
                throw new InvalidOperationException();
            if (OldFileCount == 0 && FileCount > 0)
            {
                if (HoleMap.ContainsKey(Address))
                {
                    var Hole = HoleMap[Address];
                    Holes.Remove(Hole);
                    HoleMap.Remove(Address);
                }
            }
            else if (OldFileCount > 0 && FileCount == 0)
            {
                long EndAddress = GetAlignedAddress(BaseStream.Length);
                int i = Blocks.IndexOfKey(Address);
                if (i + 1 < Blocks.Count)
                    EndAddress = Blocks.Keys[i + 1];
                if (EndAddress > Address)
                {
                    var Hole = new Hole() { Address = Address, Length = EndAddress - Address };
                    Holes.Add(Hole, 0);
                    HoleMap.Add(Address, Hole);
                }
            }
            Blocks[Address] = FileCount;
        }

        private void SplitBlockAt(long Address)
        {
            Blocks.Add(Address, 0);
            int i = Blocks.IndexOfKey(Address);
            if (i - 1 < 0)
                throw new InvalidOperationException();
            long PreviousAddress = Blocks.Keys[i - 1];
            int FileCount = Blocks[PreviousAddress];
            Blocks[Address] = FileCount;
            if (FileCount < 0)
                throw new InvalidOperationException();
            if (FileCount == 0)
            {
                if (HoleMap.ContainsKey(PreviousAddress))
                {
                    var PreviousHole = HoleMap[PreviousAddress];
                    Holes.Remove(PreviousHole);
                    HoleMap.Remove(PreviousAddress);
                }
                var NewPreviousHole = new Hole() { Address = PreviousAddress, Length = Address - PreviousAddress };
                Holes.Add(NewPreviousHole, 0);
                HoleMap.Add(NewPreviousHole.Address, NewPreviousHole);
                var NewHole = new Hole() { Address = Address, Length = GetAlignedAddress(BaseStream.Length) - Address };
                Holes.Add(NewHole, 0);
                HoleMap.Add(NewHole.Address, NewHole);
            }
        }

        private void CombineHoleAt(long Address)
        {
            int FileCount = Blocks[Address];
            if (FileCount > 0)
                return;
            if (FileCount < 0)
                throw new InvalidOperationException();
            int i = Blocks.IndexOfKey(Address);
            if (i - 1 < 0)
                return;
            long PreviousAddress = Blocks.Keys[i - 1];
            int PreviousFileCount = Blocks[PreviousAddress];
            if (PreviousFileCount > 0)
                return;
            if (PreviousFileCount < 0)
                throw new InvalidOperationException();

            var PreviousHole = HoleMap[PreviousAddress];
            Holes.Remove(PreviousHole);
            HoleMap.Remove(PreviousAddress);
            if (i < Blocks.Count - 1)
            {
                var Hole = HoleMap[Address];
                Holes.Remove(Hole);
                HoleMap.Remove(Address);
                var NewHole = new Hole() { Address = PreviousAddress, Length = PreviousHole.Length + Hole.Length };
                Holes.Add(NewHole, 0);
                HoleMap.Add(PreviousAddress, NewHole);
            }
            Blocks.Remove(Address);
        }

        /// <summary>在增加文件时用于更新文件包占用数据块信息。</summary>
        protected void AddFileToBlocks(FileDB File)
        {
            long EndAddress = File.Address + GetSpace(File.Length);
            if (File.Address >= EndAddress)
                return;
            if (!Blocks.ContainsKey(File.Address))
            {
                SplitBlockAt(File.Address);
            }
            int i;
            var loopTo = Blocks.Count - 1;
            for (i = Blocks.IndexOfKey(File.Address); i <= loopTo; i++)
            {
                long CurrentBlockAddress = Blocks.Keys[i];
                if (EndAddress <= CurrentBlockAddress)
                    break;
                SetBlockFileCount(CurrentBlockAddress, Blocks[CurrentBlockAddress] + 1);
            }
            if (i >= Blocks.Count || EndAddress != Blocks.Keys[i])
            {
                SplitBlockAt(EndAddress);
                SetBlockFileCount(Blocks.Keys[i], Blocks[Blocks.Keys[i]] - 1);
            }
        }

        /// <summary>在去除文件时用于更新文件包占用数据块信息。</summary>
        protected void RemoveFileFromBlocks(FileDB File)
        {
            long EndAddress = File.Address + GetSpace(File.Length);
            if (File.Address >= EndAddress)
                return;
            int i;
            var loopTo = Blocks.Count - 1;
            for (i = Blocks.IndexOfKey(File.Address); i <= loopTo; i++)
            {
                long CurrentBlockAddress = Blocks.Keys[i];
                if (EndAddress <= CurrentBlockAddress)
                    break;
                SetBlockFileCount(CurrentBlockAddress, Blocks[CurrentBlockAddress] - 1);
                if (Blocks[CurrentBlockAddress] == 0)
                {
                    // 清空原始数据
                    long Offset = CurrentBlockAddress - File.Address;
                    using (var s = new PartialStreamEx(BaseStream, Blocks.Keys[i], NumericOperations.Min(GetSpace(File.Length) - Offset, BaseStream.Length - CurrentBlockAddress)))
                    {
                        s.Position = 0L;
                        for (long n = 0L, loopTo1 = File.Length - 1L; n <= loopTo1; n++)
                            s.WriteByte(0);
                    }
                }
            }
            if (i >= Blocks.Count || EndAddress != Blocks.Keys[i])
                throw new InvalidOperationException();
            CombineHoleAt(File.Address);
            CombineHoleAt(EndAddress);
        }

        /// <summary>扫描洞。</summary>
        protected void ScanHoles(long DataStart)
        {
            this.DataStart = DataStart;
            Blocks.Add(DataStart, 0);
            var InitialHole = new Hole() { Address = DataStart, Length = GetAlignedAddress(BaseStream.Length) - DataStart };
            Holes.Add(InitialHole, 0);
            HoleMap.Add(InitialHole.Address, InitialHole);
            foreach (var f in FileList)
                AlignmentBlockSize = GCD(f.Address, AlignmentBlockSize - DataStart);
            foreach (var f in FileList)
            {
                if (f.Address < DataStart)
                {
                    if (f.Address + f.Length > DataStart)
                        throw new InvalidOperationException();
                    continue;
                }
                AddFileToBlocks(f);
            }
        }

        /// <summary>已重载。替换包中的一个文件。</summary>
        public override void Replace(FileDB File, ZeroPositionStreamPasser sp)
        {
            var s = sp.GetStream();

            if (File.Address != this.get_FileAddressInPhysicalFileDB(File))
                throw new ArgumentException("PhysicalFileAddressErrorPointing");
            if (File.Length != this.get_FileLengthInPhysicalFileDB(File))
                throw new ArgumentException("PhysicalFileLengthErrorPointing");

            long EndAddress = File.Address + GetSpace(File.Length);
            long MaxSize = 0L;
            int i;
            var loopTo = Blocks.Count - 2;
            for (i = Blocks.IndexOfKey(File.Address); i <= loopTo; i++)
            {
                long CurrentBlockAddress = Blocks.Keys[i];
                if (EndAddress <= CurrentBlockAddress)
                    break;
                int FileCount = Blocks[CurrentBlockAddress];
                if (FileCount == 1)
                    MaxSize = Blocks.Keys[i + 1] - File.Address;
            }
            if (i >= Blocks.Count - 1)
                MaxSize = long.MaxValue;

            Hole Hole = null;

            // 如果可能，则原位导入
            if (s.Length <= MaxSize)
                Hole = new Hole() { Address = File.Address, Length = GetSpace(s.Length) };

            // 如果不能原位导入，则寻找洞
            if (Hole is null)
            {
                foreach (var h in Holes)
                {
                    if (s.Length <= h.Key.Length)
                    {
                        Hole = h.Key;
                        break;
                    }
                }
            }

            // 如果需要超过长度，则扩展空间
            long Address = Blocks.Keys[Blocks.Count - 1];
            if (Blocks[Address] != 0)
                throw new InvalidOperationException();
            if (Hole is not null)
            {
                if (Hole.Address + Hole.Length >= Address)
                {
                    BaseStream.SetLength(Hole.Address + Hole.Length);
                }
            }
            else
            {
                long Length = GetSpace(s.Length);
                BaseStream.SetLength(Address + Length);
                Hole = new Hole() { Address = Address, Length = Length };
            }

            // 此时空间足够，改变文件中存储的结构
            this.set_FileLengthInPhysicalFileDB(File, s.Length);
            if (Hole.Address != File.Address)
                this.set_FileAddressInPhysicalFileDB(File, Hole.Address);

            // 更新空洞信息，改变文件信息
            RemoveFileFromBlocks(File);
            File.Address = Hole.Address;
            File.Length = s.Length;
            AddFileToBlocks(File);

            // 改变文件数据
            using (var f = new PartialStreamEx(BaseStream, Hole.Address, Hole.Length))
            {
                f.Position = 0L;
                f.WriteFromStream(s, s.Length);
                for (long n = s.Length, loopTo1 = f.Length - 1L; n <= loopTo1; n++)
                    f.WriteByte(0);
            }
        }
    }

    /// <summary>洞</summary>
    public class Hole
    {
        public long Address;
        public long Length;
    }

    /// <summary>洞地址比较器</summary>
    public class HoleComparer : IComparer<Hole>
    {
        private static HoleComparer _def = new HoleComparer();
        public static HoleComparer Default
        {
            get
            {
                return _def;
            }
        }
        public int Compare(Hole x, Hole y)
        {
            if (x.Length < y.Length)
                return -1;
            if (x.Length > y.Length)
                return 1;
            if (x.Address < y.Address)
                return -1;
            if (x.Address > y.Address)
                return 1;
            return 0;
        }
    }
}