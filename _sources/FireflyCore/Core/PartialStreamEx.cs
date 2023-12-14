// ==========================================================================
// 
// File:        PartialStreamEx.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 局部扩展流类
// Version:     2009.11.21.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.IO;

namespace Firefly
{

    /// <summary>
/// 局部扩展流类，用于表示一个流上的固定开始位置和长度的流，可以递归表示
/// </summary>
/// <remarks>注意：一切的异常都由你来处理。</remarks>
    public class PartialStreamEx : StreamEx
    {

        protected long BasePositionValue;
        protected long BaseLengthValue;
        protected bool BaseStreamClose;
        protected long LengthValue;

        /// <summary>已重载。初始化新实例。</summary>
    /// <param name="BaseLength">文件的最大大小</param>
    /// <remarks>BaseLength不能小于Length。</remarks>
        public PartialStreamEx(StreamEx BaseStream, long BasePosition, long BaseLength, bool BaseStreamClose = false)
        {
            this.BaseStream = BaseStream;
            BasePositionValue = BasePosition;
            BaseLengthValue = BaseLength;
            LengthValue = BaseLength;
            base.Position = BasePosition;
            this.BaseStreamClose = BaseStreamClose;
        }

        /// <summary>已重载。初始化新实例。</summary>
    /// <param name="BaseLength">文件的最大大小</param>
    /// <param name="Length">初始大小</param>
    /// <remarks>BaseLength不能小于Length。</remarks>
        public PartialStreamEx(StreamEx BaseStream, long BasePosition, long BaseLength, long Length, bool BaseStreamClose = false)
        {
            this.BaseStream = BaseStream;
            BasePositionValue = BasePosition;
            if (BaseLength < Length)
                throw new ArgumentOutOfRangeException();
            BaseLengthValue = BaseLength;
            LengthValue = Length;
            base.Position = BasePosition;
            this.BaseStreamClose = BaseStreamClose;
        }

        /// <summary>读取字节。</summary>
        public override byte ReadByte()
        {
            if (Position >= BaseLength)
                throw new EndOfStreamException();
            return base.ReadByte();
        }
        /// <summary>写入字节。</summary>
        public override void WriteByte(byte b)
        {
            if (Position >= BaseLength)
                throw new EndOfStreamException();
            base.WriteByte(b);
        }
        /// <summary>用字节表示的流的长度。</summary>
        public override long Length
        {
            get
            {
                return LengthValue;
            }
        }
        /// <summary>用字节表示的流在基流中的的偏移位置。</summary>
        public virtual long BasePosition
        {
            get
            {
                return BasePositionValue;
            }
        }
        /// <summary>用字节表示的流的最大长度。</summary>
        public virtual long BaseLength
        {
            get
            {
                return BaseLengthValue;
            }
        }
        /// <summary>流的当前位置。</summary>
        public override long Position
        {
            get
            {
                return base.Position - BasePositionValue;
            }
            set
            {
                base.Position = BasePositionValue + value;
            }
        }
        /// <summary>设置流的长度，不得大于最大大小。</summary>
        public override void SetLength(long Value)
        {
            if (Value < 0L)
                throw new ArgumentOutOfRangeException();
            if (Value > BaseLength)
                throw new ArgumentOutOfRangeException();
            if (BasePositionValue + Value > base.Length)
                base.SetLength(BasePositionValue + Value);
            LengthValue = Value;
        }
        /// <summary>已重载。读取到字节数组。</summary>
    /// <param name="Offset">Buffer 中的从零开始的字节偏移量，从此处开始存储从当前流中读取的数据。</param>
        public override void Read(byte[] Buffer, int Offset, int Count)
        {
            if (Position + Count > Length)
                throw new EndOfStreamException();
            base.Read(Buffer, Offset, Count);
        }
        /// <summary>已重载。写入字节数组。</summary>
    /// <param name="Offset">Buffer 中的从零开始的字节偏移量，从此处开始将字节复制到当前流。</param>
        public override void Write(byte[] Buffer, int Offset, int Count)
        {
            if (Position + Count > BaseLength)
                throw new EndOfStreamException();
            base.Write(Buffer, Offset, Count);
            if (Position > Length)
                LengthValue = Position;
        }

        protected override void DisposeManagedResource()
        {
            if (BaseStreamClose)
            {
                BaseStream.Dispose();
            }
            else
            {
                BaseStream.Flush();
            }
            BaseStream = null;
            base.DisposeManagedResource();
        }
    }
}