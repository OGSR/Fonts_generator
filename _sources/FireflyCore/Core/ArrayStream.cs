// ==========================================================================
// 
// File:        ArrayStream.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 数组流
// Version:     2009.08.04.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;

namespace Firefly
{

    /// <summary>
/// 数组流
/// </summary>
/// <remarks>
/// 请显式调用Close或Dispose来关闭流。
/// </remarks>
    public class ArrayStream<T> : IDisposable
    {
        private T[] BaseArray;
        private int BasePositionValue;
        private int PositionValue;
        private int LengthValue;

        /// <summary>已重载。初始化新实例。</summary>
        public ArrayStream(int Length)
        {
            if (Length < 0)
                throw new ArgumentOutOfRangeException();
            BaseArray = new T[Length];
            BasePositionValue = 0;
            PositionValue = 0;
            LengthValue = Length;
        }
        /// <summary>已重载。初始化新实例。</summary>
        public ArrayStream(T[] BaseArray, int BasePosition = 0)
        {
            if (BaseArray is null)
                throw new ArgumentNullException();
            if (BasePosition < 0 || BasePosition > BaseArray.Length)
                throw new ArgumentOutOfRangeException();
            this.BaseArray = BaseArray;
            BasePositionValue = BasePosition;
            PositionValue = BasePosition;
            LengthValue = BaseArray.Length - BasePosition;
        }

        /// <summary>已重载。初始化新实例。</summary>
        public ArrayStream(T[] BaseArray, int BasePosition, int Length)
        {
            if (BaseArray is null)
                throw new ArgumentNullException();
            if (Length < 0)
                throw new ArgumentOutOfRangeException();
            if (BasePosition < 0 || BasePosition + Length > BaseArray.Length)
                throw new ArgumentOutOfRangeException();
            this.BaseArray = BaseArray;
            BasePositionValue = BasePosition;
            PositionValue = BasePosition;
            LengthValue = Length;
        }

        /// <summary>读取元素。</summary>
        public virtual T ReadElement()
        {
            var t = BaseArray[PositionValue];
            PositionValue += 1;
            return t;
        }
        /// <summary>写入元素。</summary>
        public virtual void WriteElement(T b)
        {
            BaseArray[PositionValue] = b;
            PositionValue += 1;
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
                return PositionValue - BasePositionValue;
            }
            set
            {
                PositionValue = BasePositionValue + value;
            }
        }
        /// <summary>已重载。读取到元素数组。</summary>
    /// <param name="Offset">Buffer 中的从零开始的字节偏移量，从此处开始存储从当前流中读取的数据。</param>
        public virtual void Read(T[] Buffer, int Offset, int Count)
        {
            if (Count < 0 || PositionValue + Count > BasePositionValue + LengthValue)
                throw new ArgumentOutOfRangeException();
            Array.Copy(BaseArray, PositionValue, Buffer, Offset, Count);
            PositionValue += Count;
        }
        /// <summary>已重载。读取到元素数组。</summary>
        public void Read(T[] Buffer)
        {
            Read(Buffer, 0, Buffer.Length);
        }
        /// <summary>已重载。读取元素数组。</summary>
        public T[] Read(int Count)
        {
            T[] d = new T[Count];
            Read(d, 0, Count);
            return d;
        }
        /// <summary>已重载。写入元素数组。</summary>
    /// <param name="Offset">Buffer 中的从零开始的字节偏移量，从此处开始将字节复制到当前流。</param>
        public virtual void Write(T[] Buffer, int Offset, int Count)
        {
            if (Count < 0 || PositionValue + Count > BasePositionValue + LengthValue)
                throw new ArgumentOutOfRangeException();
            Array.Copy(Buffer, Offset, BaseArray, PositionValue, Count);
            PositionValue += Count;
        }
        /// <summary>已重载。写入元素数组。</summary>
        public void Write(T[] Buffer)
        {
            Write(Buffer, 0, Buffer.Length);
        }

        /// <summary>读取到外部流。</summary>
        public void ReadToStream(ArrayStream<T> s, int Count)
        {
            if (Count <= 0)
                return;
            T[] Buffer = new T[NumericOperations.Min(Count, 4 * (1 << 10)) - 1 + 1];
            for (int n = 0, loopTo = Count - Buffer.Length; Buffer.Length >= 0 ? n <= loopTo : n >= loopTo; n += Buffer.Length)
            {
                Read(Buffer);
                s.Write(Buffer);
            }
            int LeftLength = Count % Buffer.Length;
            Read(Buffer, 0, LeftLength);
            s.Write(Buffer, 0, LeftLength);
        }
        /// <summary>从外部流写入。</summary>
        public void WriteFromStream(ArrayStream<T> s, int Count)
        {
            if (Count <= 0)
                return;
            T[] Buffer = new T[NumericOperations.Min(Count, 4 * (1 << 10)) - 1 + 1];
            for (int n = 0, loopTo = Count - Buffer.Length; Buffer.Length >= 0 ? n <= loopTo : n >= loopTo; n += Buffer.Length)
            {
                s.Read(Buffer);
                Write(Buffer);
            }
            int LeftLength = Count % Buffer.Length;
            s.Read(Buffer, 0, LeftLength);
            Write(Buffer, 0, LeftLength);
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