// ==========================================================================
// 
// File:        CompressorSelector.vb
// Location:    Firefly.Compressing <Visual Basic .Net>
// Description: 压缩方法选择器
// Version:     2008.11.08.
// Copyright(C) F.R.C.
// 
// ==========================================================================


using System;
using System.Runtime.InteropServices;

namespace Firefly.Compressing
{
    /// <summary>
    /// 压缩委托
    /// 本接口仅用于短数据，即所有数据均可放于内存中。
    /// </summary>
    public delegate byte[] Compress(byte[] Data);

    /// <summary>
    /// 解压委托
    /// 本接口仅用于短数据，即所有数据均可放于内存中。
    /// </summary>
    public delegate byte[] Decompress(byte[] Data);

    /// <summary>
    /// 压缩方法选择器
    /// 能够对数据尝试输入的一组压缩方法，返回最小的或第一个小于指定大小的压缩数据。
    /// </summary>
    public class CompressorSelector
    {
        protected Compress[] Compressors;

        /// <summary>
        /// 靠前的压缩方法会被优先使用。
        /// </summary>
        public CompressorSelector(Compress[] CompressMethods)
        {
            if (CompressMethods is null || CompressMethods.Length == 0)
                throw new ArgumentNullException();
            Compressors = CompressMethods;
        }

        /// <summary>
        /// 逐次尝试，选取最佳压缩率的压缩方法。
        /// </summary>
        public byte[] Compress(byte[] Data, [Optional, DefaultParameterValue(-1)] ref int Method)
        {
            byte[] BestCompressedData = null;
            int BestMethodLength = int.MaxValue;
            int BestMethod = -1;
            for (int n = 0, loopTo = Compressors.Length - 1; n <= loopTo; n++)
            {
                byte[] CompressedData = Compressors[n](Data);
                if (CompressedData.Length < BestMethodLength)
                {
                    BestCompressedData = CompressedData;
                    BestMethodLength = CompressedData.Length;
                    BestMethod = n;
                }
            }
            Method = BestMethod;
            return BestCompressedData;
        }

        /// <summary>
        /// 逐次尝试，选取第一个能压缩到指定大小的压缩方法。
        /// </summary>
        public byte[] CompressAndFitIn(byte[] Data, int Size, [Optional, DefaultParameterValue(-1)] ref int Method, bool OutputNothingIfNoFit = false)
        {
            if (OutputNothingIfNoFit)
            {
                byte[] BestCompressedData = null;
                int BestMethodLength = int.MaxValue;
                int BestMethod = -1;
                for (int n = 0, loopTo = Compressors.Length - 1; n <= loopTo; n++)
                {
                    byte[] CompressedData = Compressors[n](Data);
                    if (CompressedData.Length <= Size)
                    {
                        Method = n;
                        return CompressedData;
                    }
                    else if (CompressedData.Length < BestMethodLength)
                    {
                        BestCompressedData = CompressedData;
                        BestMethodLength = CompressedData.Length;
                        BestMethod = n;
                    }
                }
                Method = BestMethod;
                return BestCompressedData;
            }
            else
            {
                for (int n = 0, loopTo1 = Compressors.Length - 1; n <= loopTo1; n++)
                {
                    byte[] CompressedData = Compressors[n](Data);
                    if (CompressedData.Length <= Size)
                    {
                        Method = n;
                        return CompressedData;
                    }
                }
                return null;
            }
        }
    }
}