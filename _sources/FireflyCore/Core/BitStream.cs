// ==========================================================================
// 
// File:        BitStream.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 位流
// Version:     2009.12.01.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;

namespace Firefly
{

    /// <summary>
/// 位流。字节内部的位的顺序，低位在前。字节间，低位在前。返回的数据的内部，低位在前。
/// </summary>
/// <remarks>
/// 请显式调用Close或Dispose来关闭流。
/// </remarks>
    public class BitStream : IDisposable
    {
        private byte[] BaseArray;
        private int PositionValue;
        private int LengthValue;

        /// <summary>已重载。初始化新实例。</summary>
        public BitStream(int Length)
        {
            if (Length < 0)
                throw new ArgumentOutOfRangeException();
            BaseArray = new byte[Length];
            PositionValue = 0;
            LengthValue = Length;
        }
        /// <summary>已重载。初始化新实例。</summary>
        public BitStream(byte[] BaseArray)
        {
            if (BaseArray is null)
                throw new ArgumentNullException();
            this.BaseArray = BaseArray;
            PositionValue = 0;
            LengthValue = BaseArray.Length * 8;
        }

        /// <summary>读取到Byte。</summary>
        public virtual byte ReadToByte(int i)
        {
            if (i < 0)
                throw new ArgumentOutOfRangeException();
            if (i == 0)
                return 0;

            int n = PositionValue / 8;
            int p = PositionValue % 8;
            int r = 8 - p;
            byte v = BaseArray[n].Bits(7, p);
            if (r < i)
            {
                n += 1;
                v = (byte)BitOperations.ConcatBits(BaseArray[n], 8, v, r).Bits(i - 1, 0);
                r += 8;
            }
            else
            {
                v = v.Bits(i - 1, 0);
            }
            PositionValue += i;
            return v;
        }
        /// <summary>从Byte写入。</summary>
        public virtual void WriteFromByte(byte v, int i)
        {
            if (i < 0)
                throw new ArgumentOutOfRangeException();
            if (i == 0)
                return;

            v = v.Bits(i - 1, 0);
            int n = PositionValue / 8;
            int p = PositionValue % 8;
            int r = 8 - p;
            if (r >= i)
            {
                BaseArray[n] = (byte)BitOperations.ConcatBits(BaseArray[n].Bits(7, p + i), 8 - p - i, v, i, BaseArray[n], p);
                PositionValue += i;
                return;
            }
            else
            {
                BaseArray[n] = (byte)BitOperations.ConcatBits(v, r, BaseArray[n], p);
            }
            if (r < i)
            {
                n += 1;
                BaseArray[n] = (byte)BitOperations.ConcatBits(BaseArray[n], 8 - i + r, v.Bits(i - 1, r), i - r);
            }
            PositionValue += i;
        }

        /// <summary>读取到Int32。</summary>
        public virtual int ReadToInt32(int i)
        {
            if (i < 0)
                throw new ArgumentOutOfRangeException();
            if (i == 0)
                return 0;

            int n = PositionValue / 8;
            int p = PositionValue % 8;
            int r = 8 - p;
            int v = BaseArray[n].Bits(7, p);
            while (r < i)
            {
                n += 1;
                v = BitOperations.ConcatBits(BaseArray[n], 8, v, r);
                r += 8;
            }
            v = v.Bits(i - 1, 0);
            PositionValue += i;
            return v;
        }
        /// <summary>从Int32写入。</summary>
        public virtual void WriteFromInt32(int v, int i)
        {
            if (i < 0)
                throw new ArgumentOutOfRangeException();
            if (i == 0)
                return;

            v = v.Bits(i - 1, 0);
            int n = PositionValue / 8;
            int p = PositionValue % 8;
            int r = 8 - p;
            if (r >= i)
            {
                BaseArray[n] = (byte)BitOperations.ConcatBits(BaseArray[n].Bits(7, p + i), 8 - p - i, (byte)v, i, BaseArray[n], p);
                PositionValue += i;
                return;
            }
            else
            {
                BaseArray[n] = (byte)BitOperations.ConcatBits(v, r, BaseArray[n], p);
            }
            while (r + 8 <= i)
            {
                n += 1;
                BaseArray[n] = (byte)v.Bits(r + 8 - 1, r);
                r += 8;
            }
            if (r < i)
            {
                n += 1;
                BaseArray[n] = (byte)BitOperations.ConcatBits(BaseArray[n], 8 - i + r, v.Bits(i - 1, r), i - r);
            }
            PositionValue += i;
        }

        /// <summary>强制同步缓冲数据。</summary>
        public virtual void Flush()
        {
        }
        private bool _Close_Closed = false;
        /// <summary>关闭流。</summary>
    /// <remarks>对继承者的说明：该方法调用Dispose()，不要覆盖该方法，而应覆盖Dispose(Boolean)</remarks>
        public virtual void Close()
        {
            if (_Close_Closed)
                throw new InvalidOperationException();
            Dispose();
            _Close_Closed = true;
        }
        /// <summary>用字节表示的流的长度。</summary>
        public virtual long Length
        {
            get
            {
                return LengthValue;
            }
        }
        /// <summary>流的当前位置。</summary>
        public virtual int Position
        {
            get
            {
                return PositionValue;
            }
            set
            {
                PositionValue = value;
            }
        }

        #region  IDisposable 支持 
        private bool DisposedValue = false; // 检测冗余的调用
                                            /// <summary>释放流的资源。</summary>
    /// <remarks>对继承者的说明：不要调用基类的Dispose()，而应调用Dispose(True)，否则会出现无限递归。</remarks>
        protected virtual void Dispose(bool Disposing)
        {
            if (DisposedValue)
                return;
            if (Disposing)
            {
                // 释放其他状态(托管对象)。
            }

            // 释放您自己的状态(非托管对象)。
            // 将大型字段设置为 null。
            BaseArray = null;
            DisposedValue = true;
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