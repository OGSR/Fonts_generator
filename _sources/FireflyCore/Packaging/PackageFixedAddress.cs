// ==========================================================================
// 
// File:        PackageFixedAddress.vb
// Location:    Firefly.Packaging <Visual Basic .Net>
// Description: 固定地址文件包
// Version:     2010.02.03.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;

namespace Firefly.Packaging
{
    /// <summary>
    /// 固定地址文件包，通常用于无需改变文件地址索引的文件包
    /// 若需要数据顺序和索引一致，请使用PackageContinuous
    /// 若需要改变文件地址索引，请使用PackageDiscrete
    /// 若不需要改变文件地址索引，请使用PackageFixed
    /// 
    /// 
    /// 给继承者的说明：
    /// 
    /// 如果文件包支持写入，应
    /// (1)重写FileLengthInPhysicalFileDB方法
    /// (2)在加入一个FileDB时，调用PushFile方法，使得它被加入到FileList、IndexOfFile、FileSetAddressSorted中，以及PushFileToDir到根目录FileDB中，若根目录FileDB不存在，则空的根目录会自动创建
    /// 
    /// 请使用PackageRegister来注册文件包类型。
    /// 应提供一个返回"ISO(*.ISO)|*.ISO"形式字符串的Filter属性，
    /// 并按照PackageRegister中的委托类型提供一个Open函数、一个Create函数(如果支持创建)。
    /// </summary>
    public abstract class PackageFixedAddress : PackageBase
    {

        /// <summary>按照地址排序的文件集。</summary>
        protected SortedList<FileDB, long> FileSetAddressSorted = new SortedList<FileDB, long>(FileDBAddressComparer.Default);

        /// <summary>已重载。默认构照函数。请手动初始化BaseStream。</summary>
        protected PackageFixedAddress() : base()
        {
        }
        /// <summary>已重载。打开或创建文件包。</summary>
        public PackageFixedAddress(ZeroPositionStreamPasser sp) : base(sp)
        {
        }

        /// <summary>把文件FileDB放入根目录FileDB。若根目录FileDB不存在，则空的根目录会自动创建。在加入一个FileDB时，调用该方法，使得它被加入到FileList、IndexOfFile、FileSetAddressSorted中，以及PushFileToDir到根目录FileDB中。</summary>
        protected override void PushFile(FileDB f, FileDB Directory)
        {
            base.PushFile(f, Directory);
            FileSetAddressSorted.Add(f, f.Address);
        }

        /// <summary>从文件包读取FileDB文件长度和写入文件长度到文件包。用于替换文件包时使用。</summary>
        public abstract long get_FileLengthInPhysicalFileDB(FileDB File);
        public abstract void set_FileLengthInPhysicalFileDB(FileDB File, long value);

        /// <summary>已重载。替换包中的一个文件。</summary>
        public override void Replace(FileDB File, ZeroPositionStreamPasser sp)
        {
            var s = sp.GetStream();

            if (FileSetAddressSorted.Count == 0)
                throw new InvalidOperationException("NullFileSetAddressSorted");

            s.Position = 0L;
            long MaxSize = BaseStream.Length - File.Address;
            int NextIndex = FileSetAddressSorted.IndexOfKey(File) + 1;
            if (NextIndex < FileSetAddressSorted.Count)
            {
                MaxSize = FileSetAddressSorted.Keys[NextIndex].Address - File.Address;
            }
            if (MaxSize < 0L)
                throw new IOException(string.Format("NotEnoughSpace: {0}", File.Name));
            if (s.Length > MaxSize)
                throw new IOException(string.Format("NotEnoughSpace: {0}", File.Name));

            if (this.get_FileLengthInPhysicalFileDB(File) != File.Length)
                throw new InvalidOperationException(string.Format("OriginalFileLenghtNotMatch: {0}", File.Name));

            using (var f = new PartialStreamEx(BaseStream, File.Address, MaxSize))
            {
                f.Position = 0L;
                f.WriteFromStream(s, s.Length);
            }

            this.set_FileLengthInPhysicalFileDB(File, s.Length);
            File.Length = s.Length;
        }
    }
}