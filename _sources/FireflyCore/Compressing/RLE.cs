// ==========================================================================
// 
// File:        RLE.vb
// Location:    Firefly.Compressing <Visual Basic .Net>
// Description: RLE算法类
// Version:     2008.11.08.
// Copyright(C) F.R.C.
// 
// ==========================================================================


using System;

namespace Firefly.Compressing
{
    /// <summary>
    /// RLE算法类
    /// 完成一个完整压缩的时间复杂度为O(n)，空间复杂度为O(1)
    /// </summary>
    public class RLE
    {
        private byte[] Data;
        private int Offset;
        private ushort MinMatchLength;
        private ushort MaxMatchLength;

        public RLE(byte[] OriginalData, ushort MaxMatchLength, ushort MinMatchLength = 1)
        {
            if (OriginalData is null)
                throw new ArgumentNullException();
            if (MinMatchLength <= 0)
                throw new ArgumentOutOfRangeException();
            if (MaxMatchLength < MinMatchLength)
                throw new ArgumentException();
            Data = OriginalData;
            this.MinMatchLength = MinMatchLength;
            this.MaxMatchLength = MaxMatchLength;
            Offset = 0;
        }

        /// <summary>原始数据</summary>
        public byte[] OriginalData
        {
            get
            {
                return Data;
            }
        }

        /// <summary>位置</summary>
        public int Position
        {
            get
            {
                return Offset;
            }
        }

        /// <summary>已重载。前进</summary>
        public void Proceed(int n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException();
            for (int i = 0, loopTo = n - 1; i <= loopTo; i++)
                Proceed();
        }

        /// <summary>已重载。前进</summary>
        public void Proceed()
        {
            Offset += 1;
        }

        /// <summary>匹配</summary>
        /// <remarks>无副作用</remarks>
        public RLEPointer Match()
        {
            byte d = Data[Offset];
            ushort Max = (ushort)NumericOperations.Min(MaxMatchLength, Data.Length - Offset);
            ushort Count = Max;
            for (ushort l = 1, loopTo = (ushort)(Max - 1); l <= loopTo; l++)
            {
                if (Data[Offset + l] != d)
                {
                    Count = l;
                }
            }
            if (Count < MinMatchLength)
                return null;
            return new RLEPointer(d, Count);
        }

        /// <summary>RLE匹配指针，表示一个RLE匹配</summary>
        public class RLEPointer : Pointer
        {

            /// <summary>重复值</summary>
            public readonly byte Value;
            private readonly ushort Count;

            public RLEPointer(byte Value, ushort Count)
            {
                this.Value = Value;
                this.Count = Count;
            }

            /// <summary>长度</summary>
            public int Length
            {
                get
                {
                    return Count;
                }
            }
        }
    }
}