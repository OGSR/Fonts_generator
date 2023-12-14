// ==========================================================================
// 
// File:        ByteArrayStream.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 字节数组流
// Version:     2009.11.02.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;

namespace Firefly
{

    /// <summary>
/// 字节数组流
/// </summary>
/// <remarks>
/// 请显式调用Close或Dispose来关闭流。
/// </remarks>
    public class ByteArrayStream : StreamEx
    {
        private byte[] BaseArray;
        private int BasePositionValue;
        private int PositionValue;
        private int LengthValue;

        /// <summary>已重载。初始化新实例。</summary>
        public ByteArrayStream(int Length)
        {
            if (Length < 0)
                throw new ArgumentOutOfRangeException();
            BaseArray = new byte[Length];
            BasePositionValue = 0;
            PositionValue = 0;
            LengthValue = Length;
        }
        /// <summary>已重载。初始化新实例。</summary>
        public ByteArrayStream(byte[] BaseArray, int BasePosition = 0)
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
        public ByteArrayStream(byte[] BaseArray, int BasePosition, int Length)
        {
            if (BaseArray is null)
                throw new ArgumentNullException();
            if (Length < 0)
                throw new ArgumentOutOfRangeException();
            if (BasePosition < 0 || BasePosition + Length > BaseArray.LongLength)
                throw new ArgumentOutOfRangeException();
            this.BaseArray = BaseArray;
            BasePositionValue = BasePosition;
            PositionValue = BasePosition;
            LengthValue = Length;
        }

        /// <summary>读取元素。</summary>
        public override byte ReadByte()
        {
            byte t = BaseArray[PositionValue];
            PositionValue += 1;
            return t;
        }
        /// <summary>写入元素。</summary>
        public override void WriteByte(byte b)
        {
            BaseArray[PositionValue] = b;
            PositionValue += 1;
        }

        /// <summary>指示当前流是否支持读取。</summary>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }
        /// <summary>指示当前流是否支持定位。</summary>
        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }
        /// <summary>指示当前流是否支持写入。</summary>
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }
        /// <summary>强制同步缓冲数据。</summary>
        public override void Flush()
        {
        }
        /// <summary>用字节表示的流的长度。</summary>
        public override long Length
        {
            get
            {
                return LengthValue;
            }
        }
        /// <summary>流的当前位置。</summary>
        public override long Position
        {
            get
            {
                return PositionValue - BasePositionValue;
            }
            set
            {
                PositionValue = (int)(BasePositionValue + value);
            }
        }
        /// <summary>已重载。读取到元素数组。</summary>
    /// <param name="Offset">Buffer 中的从零开始的字节偏移量，从此处开始存储从当前流中读取的数据。</param>
        public override void Read(byte[] Buffer, int Offset, int Count)
        {
            if (Count < 0 || PositionValue + Count > BasePositionValue + LengthValue)
                throw new ArgumentOutOfRangeException();
            Array.Copy(BaseArray, PositionValue, Buffer, Offset, Count);
            PositionValue += Count;
        }
        /// <summary>已重载。写入元素数组。</summary>
    /// <param name="Offset">Buffer 中的从零开始的字节偏移量，从此处开始将字节复制到当前流。</param>
        public override void Write(byte[] Buffer, int Offset, int Count)
        {
            if (Count < 0 || PositionValue + Count > BasePositionValue + LengthValue)
                throw new ArgumentOutOfRangeException();
            Array.Copy(Buffer, Offset, BaseArray, PositionValue, Count);
            PositionValue += Count;
        }

        protected override void DisposeManagedResource()
        {
            BaseArray = null;
            base.DisposeManagedResource();
        }
    }
}