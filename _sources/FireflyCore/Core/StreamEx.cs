// ==========================================================================
// 
// File:        StreamEx.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 扩展流类
// Version:     2010.02.04.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using Firefly.TextEncoding;
using Microsoft.VisualBasic.CompilerServices;

namespace Firefly
{

    /// <summary>
/// 扩展流类
/// </summary>
/// <remarks>
/// 请显式调用Close或Dispose来关闭流。
/// 如果调用了ToStream或转换到了Stream，并放弃了StreamEx，StreamEx也不会消失，因为使用了一个继承自Stream的Adapter来持有StreamEx的引用。
/// 本类与System.IO.StreamReader等类不兼容。这些类使用了ReadByte返回的结束标志-1等。本类会在位置超过文件长度时读取会抛出异常。
/// 本类主要用于封装System.IO.MemoryStream和System.IO.FileStream，对其他流可能抛出无法预期的异常。
/// 一切的异常都由调用者来处理。
/// </remarks>
    public class StreamEx : IDisposable
    {
        protected Stream BaseStream;

        /// <summary>已重载。初始化新实例。</summary>
        public StreamEx()
        {
            BaseStream = new MemoryStream();
        }
        /// <summary>已重载。初始化新实例。</summary>
        public StreamEx(string Path, FileMode Mode, FileAccess Access, FileShare Share)
        {
            BaseStream = new FileStream(Path, Mode, Access, Share);
        }
        /// <summary>已重载。初始化新实例。</summary>
        public StreamEx(string Path, FileMode Mode, FileAccess Access = FileAccess.ReadWrite)
        {
            BaseStream = new FileStream(Path, Mode, Access, FileShare.Read);
        }
        /// <summary>已重载。初始化新实例。</summary>
        public StreamEx(Stream BaseStream)
        {
            this.BaseStream = BaseStream;
        }
        public static implicit operator StreamEx(Stream s)
        {
            StreamAdapter sa = s as StreamAdapter;
            if (sa is not null)
                return sa.BaseStream;
            return new StreamEx(s);
        }
        public static implicit operator Stream(StreamEx s)
        {
            return new StreamAdapter(s);
        }
        public Stream ToStream()
        {
            return new StreamAdapter(this);
        }
        public Stream ToUnsafeStream()
        {
            return new UnsafeStreamAdapter(this);
        }

        /// <summary>读取字节。</summary>
        public virtual byte ReadByte()
        {
            int b = BaseStream.ReadByte();
            if (b == -1)
                throw new EndOfStreamException();
            return (byte)b;
        }
        /// <summary>写入字节。</summary>
        public virtual void WriteByte(byte b)
        {
            BaseStream.WriteByte(b);
        }

        /// <summary>读取Int16。</summary>
        public short ReadInt16()
        {
            short o;
            o = ReadByte();
            o = (short)(o | ReadByte() << 8);
            return o;
        }
        /// <summary>读取Int32。</summary>
        public int ReadInt32()
        {
            int o;
            o = ReadByte();
            o = o | ReadByte() << 8;
            o = o | ReadByte() << 16;
            o = o | ReadByte() << 24;
            return o;
        }
        /// <summary>读取Int64。</summary>
        public long ReadInt64()
        {
            long o;
            o = ReadByte();
            o = o | (long)ReadByte() << 8;
            o = o | (long)ReadByte() << 16;
            o = o | (long)ReadByte() << 24;
            o = o | (long)ReadByte() << 32;
            o = o | (long)ReadByte() << 40;
            o = o | (long)ReadByte() << 48;
            o = o | (long)ReadByte() << 56;
            return o;
        }

        /// <summary>写入Int16。</summary>
        public void WriteInt16(short i)
        {
            WriteByte((byte)(i & 0xFF));
            i = (short)(i >> 8);
            WriteByte((byte)(i & 0xFF));
        }
        /// <summary>写入Int32。</summary>
        public void WriteInt32(int i)
        {
            WriteByte((byte)(i & 0xFF));
            i = i >> 8;
            WriteByte((byte)(i & 0xFF));
            i = i >> 8;
            WriteByte((byte)(i & 0xFF));
            i = i >> 8;
            WriteByte((byte)(i & 0xFF));
        }
        /// <summary>写入Int64。</summary>
        public void WriteInt64(long i)
        {
            WriteByte((byte)(i & 0xFFL));
            i = i >> 8;
            WriteByte((byte)(i & 0xFFL));
            i = i >> 8;
            WriteByte((byte)(i & 0xFFL));
            i = i >> 8;
            WriteByte((byte)(i & 0xFFL));
            i = i >> 8;
            WriteByte((byte)(i & 0xFFL));
            i = i >> 8;
            WriteByte((byte)(i & 0xFFL));
            i = i >> 8;
            WriteByte((byte)(i & 0xFFL));
            i = i >> 8;
            WriteByte((byte)(i & 0xFFL));
        }

        /// <summary>读取Int16，高位优先字节序。</summary>
        public short ReadInt16BigEndian()
        {
            short o;
            o = (short)(ReadByte() << 8);
            o = (short)(o | ReadByte());
            return o;
        }
        /// <summary>读取Int32，高位优先字节序。</summary>
        public int ReadInt32BigEndian()
        {
            int o;
            o = ReadByte() << 24;
            o = o | ReadByte() << 16;
            o = o | ReadByte() << 8;
            o = o | ReadByte();
            return o;
        }
        /// <summary>读取Int64，高位优先字节序。</summary>
        public long ReadInt64BigEndian()
        {
            long o;
            o = (long)ReadByte() << 56;
            o = o | (long)ReadByte() << 48;
            o = o | (long)ReadByte() << 40;
            o = o | (long)ReadByte() << 32;
            o = o | (long)ReadByte() << 24;
            o = o | (long)ReadByte() << 16;
            o = o | (long)ReadByte() << 8;
            o = o | ReadByte();
            return o;
        }

        /// <summary>写入Int16，高位优先字节序。</summary>
        public void WriteInt16BigEndian(short i)
        {
            WriteByte((byte)(DirectIntConvert.CSU(i) >> 8 & 0xFF));
            WriteByte((byte)(i & 0xFF));
        }
        /// <summary>写入Int32，高位优先字节序。</summary>
        public void WriteInt32BigEndian(int i)
        {
            WriteByte((byte)(DirectIntConvert.CSU(i) >> 24 & 0xFFL));
            WriteByte((byte)(DirectIntConvert.CSU(i) >> 16 & 0xFFL));
            WriteByte((byte)(DirectIntConvert.CSU(i) >> 8 & 0xFFL));
            WriteByte((byte)(i & 0xFF));
        }
        /// <summary>写入Int64，高位优先字节序。</summary>
        public void WriteInt64BigEndian(long i)
        {
            WriteByte((byte)((long)(DirectIntConvert.CSU(i) >> 56) & 0xFFL));
            WriteByte((byte)((long)(DirectIntConvert.CSU(i) >> 48) & 0xFFL));
            WriteByte((byte)((long)(DirectIntConvert.CSU(i) >> 40) & 0xFFL));
            WriteByte((byte)((long)(DirectIntConvert.CSU(i) >> 32) & 0xFFL));
            WriteByte((byte)((long)(DirectIntConvert.CSU(i) >> 24) & 0xFFL));
            WriteByte((byte)((long)(DirectIntConvert.CSU(i) >> 16) & 0xFFL));
            WriteByte((byte)((long)(DirectIntConvert.CSU(i) >> 8) & 0xFFL));
            WriteByte((byte)(i & 0xFFL));
        }

        /// <summary>读取UInt16。</summary>
        public ushort ReadUInt16()
        {
            return DirectIntConvert.CSU(ReadInt16());
        }
        /// <summary>写入UInt16。</summary>
        public void WriteUInt16(ushort i)
        {
            WriteInt16(DirectIntConvert.CUS(i));
        }
        /// <summary>读取UInt16，高位优先字节序。</summary>
        public ushort ReadUInt16BigEndian()
        {
            return DirectIntConvert.CSU(ReadInt16BigEndian());
        }
        /// <summary>写入UInt16，高位优先字节序。</summary>
        public void WriteUInt16BigEndian(ushort i)
        {
            WriteInt16BigEndian(DirectIntConvert.CUS(i));
        }

        /// <summary>读取\0字节结尾的字符串(UTF-16等不适用)。</summary>
        public string ReadString(int Count, System.Text.Encoding Encoding)
        {
            var Bytes = new List<byte>();
            for (int n = 0, loopTo = Count - 1; n <= loopTo; n++)
            {
                byte b = ReadByte();
                if (b == ControlChars.Nul)
                {
                    Position += Count - 1 - n;
                    break;
                }
                else
                {
                    Bytes.Add(b);
                }
            }
            return Conversions.ToString(Encoding.GetChars(Bytes.ToArray()));
        }
        /// <summary>写入\0字节结尾的字符串(UTF-16等不适用)。</summary>
        public void WriteString(string s, int Count, System.Text.Encoding Encoding)
        {
            if (string.IsNullOrEmpty(s))
            {
                for (int n = 0, loopTo = Count - 1; n <= loopTo; n++)
                    WriteByte(0);
            }
            else
            {
                byte[] Bytes = Encoding.GetBytes(s);
                if (Bytes.Length > Count)
                    throw new InvalidDataException();
                Write(Bytes);
                for (int n = Bytes.Length, loopTo1 = Count - 1; n <= loopTo1; n++)
                    WriteByte(0);
            }
        }
        /// <summary>读取包括\0字节的字符串(如UTF-16)。</summary>
        public string ReadStringWithNull(int Count, System.Text.Encoding Encoding)
        {
            return Conversions.ToString(Encoding.GetChars(Read(Count)));
        }

        /// <summary>读取ASCII字符串。</summary>
        public string ReadSimpleString(int Count)
        {
            return ReadString(Count, TextEncoding.TextEncoding.ASCII);
        }
        /// <summary>写入ASCII字符串。</summary>
        public void WriteSimpleString(string s, int Count)
        {
            WriteString(s, Count, TextEncoding.TextEncoding.ASCII);
        }
        /// <summary>写入ASCII字符串。</summary>
        public void WriteSimpleString(string s)
        {
            WriteSimpleString(s, s.Length);
        }
        /// <summary>读取ASCII字符串(包括\0)。</summary>
        public string ReadSimpleStringWithNull(int Count)
        {
            return ReadStringWithNull(Count, TextEncoding.TextEncoding.ASCII);
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
        private struct SingleInt32
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public float SingleValue;
            [System.Runtime.InteropServices.FieldOffset(0)]
            public int Int32Value;
        }
        /// <summary>读取单精度浮点数。</summary>
        public float ReadSingle()
        {
            var a = default(SingleInt32);
            a.Int32Value = ReadInt32();
            return a.SingleValue;
        }
        /// <summary>写入单精度浮点数。</summary>
        public void WriteSingle(float i)
        {
            var a = default(SingleInt32);
            a.SingleValue = i;
            WriteInt32(a.Int32Value);
        }
        /// <summary>读取单精度浮点数。</summary>
        public float ReadFloat32()
        {
            var a = default(SingleInt32);
            a.Int32Value = ReadInt32();
            return a.SingleValue;
        }
        /// <summary>写入单精度浮点数。</summary>
        public void WriteFloat32(float i)
        {
            var a = default(SingleInt32);
            a.SingleValue = i;
            WriteInt32(a.Int32Value);
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
        private struct DoubleInt64
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public double DoubleValue;
            [System.Runtime.InteropServices.FieldOffset(0)]
            public long Int64Value;
        }
        /// <summary>读取双精度浮点数。</summary>
        public double ReadDouble()
        {
            var a = default(DoubleInt64);
            a.Int64Value = ReadInt64();
            return a.DoubleValue;
        }
        /// <summary>写入双精度浮点数。</summary>
        public void WriteDouble(double i)
        {
            var a = default(DoubleInt64);
            a.DoubleValue = i;
            WriteInt64(a.Int64Value);
        }
        /// <summary>读取双精度浮点数。</summary>
        public double ReadFloat64()
        {
            var a = default(DoubleInt64);
            a.Int64Value = ReadInt64();
            return a.DoubleValue;
        }
        /// <summary>写入双精度浮点数。</summary>
        public void WriteFloat64(double i)
        {
            var a = default(DoubleInt64);
            a.DoubleValue = i;
            WriteInt64(a.Int64Value);
        }

        /// <summary>指示当前流是否支持读取。</summary>
        public virtual bool CanRead
        {
            get
            {
                return BaseStream.CanRead;
            }
        }
        /// <summary>指示当前流是否支持定位。</summary>
        public virtual bool CanSeek
        {
            get
            {
                return BaseStream.CanSeek;
            }
        }
        /// <summary>指示当前流是否支持写入。</summary>
        public virtual bool CanWrite
        {
            get
            {
                return BaseStream.CanWrite;
            }
        }
        /// <summary>强制同步缓冲数据。</summary>
        public virtual void Flush()
        {
            BaseStream.Flush();
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
                return BaseStream.Length;
            }
        }
        /// <summary>流的当前位置。</summary>
        public virtual long Position
        {
            get
            {
                return BaseStream.Position;
            }
            set
            {
                BaseStream.Position = value;
            }
        }
        /// <summary>设置流的当前位置。</summary>
        public long Seek(long Offset, SeekOrigin Origin)
        {
            switch (Origin)
            {
                case SeekOrigin.Begin:
                    {
                        Position = Offset;
                        break;
                    }
                case SeekOrigin.Current:
                    {
                        Position += Offset;
                        break;
                    }
                case SeekOrigin.End:
                    {
                        Position = Length - Offset;
                        break;
                    }
            }
            return Position;
        }
        /// <summary>设置流的长度。</summary>
        public virtual void SetLength(long Value)
        {
            BaseStream.SetLength(Value);
        }
        /// <summary>已重载。读取到字节数组。</summary>
    /// <param name="Offset">Buffer 中的从零开始的字节偏移量，从此处开始存储从当前流中读取的数据。</param>
        public virtual void Read(byte[] Buffer, int Offset, int Count)
        {
            int c = BaseStream.Read(Buffer, Offset, Count);
            if (c != Count)
                throw new EndOfStreamException();
        }
        /// <summary>已重载。读取到字节数组。</summary>
        public void Read(byte[] Buffer)
        {
            Read(Buffer, 0, Buffer.Length);
        }
        /// <summary>已重载。读取字节数组。</summary>
        public byte[] Read(int Count)
        {
            byte[] d = new byte[Count];
            Read(d, 0, Count);
            return d;
        }
        /// <summary>已重载。写入字节数组。</summary>
    /// <param name="Offset">Buffer 中的从零开始的字节偏移量，从此处开始将字节复制到当前流。</param>
        public virtual void Write(byte[] Buffer, int Offset, int Count)
        {
            BaseStream.Write(Buffer, Offset, Count);
        }
        /// <summary>已重载。写入字节数组。</summary>
        public void Write(byte[] Buffer)
        {
            Write(Buffer, 0, Buffer.Length);
        }

        /// <summary>读取Int32数组。</summary>
        public int[] ReadInt32Array(int Count)
        {
            int[] d = new int[Count];
            for (int n = 0, loopTo = Count - 1; n <= loopTo; n++)
                d[n] = ReadInt32();
            return d;
        }
        /// <summary>写入Int32数组。</summary>
        public void WriteInt32Array(int[] Buffer)
        {
            foreach (var i in Buffer)
                WriteInt32(i);
        }

        /// <summary>读取到外部流。</summary>
        public void ReadToStream(StreamEx s, long Count)
        {
            if (Count <= 0L)
                return;
            byte[] Buffer = new byte[(int)(NumericOperations.Min(Count, 4 * (1 << 20)) - 1L) + 1];
            for (long n = 0L, loopTo = Count - Buffer.Length; (long)Buffer.Length >= 0 ? n <= loopTo : n >= loopTo; n += Buffer.Length)
            {
                Read(Buffer);
                s.Write(Buffer);
            }
            int LeftLength = (int)(Count % Buffer.Length);
            Read(Buffer, 0, LeftLength);
            s.Write(Buffer, 0, LeftLength);
        }
        /// <summary>从外部流写入。</summary>
        public void WriteFromStream(StreamEx s, long Count)
        {
            if (Count <= 0L)
                return;
            byte[] Buffer = new byte[(int)(NumericOperations.Min(Count, 4 * (1 << 20)) - 1L) + 1];
            for (long n = 0L, loopTo = Count - Buffer.Length; (long)Buffer.Length >= 0 ? n <= loopTo : n >= loopTo; n += Buffer.Length)
            {
                s.Read(Buffer);
                Write(Buffer);
            }
            int LeftLength = (int)(Count % Buffer.Length);
            s.Read(Buffer, 0, LeftLength);
            Write(Buffer, 0, LeftLength);
        }
        /// <summary>保存到文件。</summary>
        public void SaveAs(string Path)
        {
            var s = new StreamEx(Path, FileMode.Create, FileAccess.ReadWrite);
            long Current = Position;
            Position = 0L;
            ReadToStream(s, Length);
            s.Close();
            Position = Current;
        }

        #region  IDisposable 支持 
        /// <summary>释放托管对象或间接非托管对象(Stream等)。可在这里将大型字段设置为 null。</summary>
        protected virtual void DisposeManagedResource()
        {
            if (BaseStream is not null)
                BaseStream.Dispose();
            BaseStream = null;
        }

        /// <summary>释放直接非托管对象(Handle等)。可在这里将大型字段设置为 null。</summary>
        protected virtual void DisposeUnmanagedResource()
        {
        }

        // 检测冗余的调用
        private bool DisposedValue = false;
        /// <summary>释放流的资源。请优先覆盖DisposeManagedResource、DisposeUnmanagedResource、DisposeNullify方法。如果你直接保存非托管对象(Handle等)，请覆盖Finalize方法，并在其中调用Dispose(False)。</summary>
        protected virtual void Dispose(bool disposing)
        {
            if (DisposedValue)
                return;
            DisposedValue = true;
            if (disposing)
            {
                DisposeManagedResource();
            }
            DisposeUnmanagedResource();
        }

        /// <summary>释放流的资源。</summary>
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入上面的 Dispose(ByVal disposing As Boolean) 中。
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region  Stream 兼容支持 
        /// <summary>扩展流适配器类</summary>
    /// <remarks>用于安全保存StreamEx的Stream形式。</remarks>
        protected class StreamAdapter : Stream
        {
            protected internal StreamEx BaseStream;

            public StreamAdapter(StreamEx s)
            {
                BaseStream = s;
            }
            public override bool CanRead
            {
                get
                {
                    return BaseStream.CanRead;
                }
            }
            public override bool CanSeek
            {
                get
                {
                    return BaseStream.CanSeek;
                }
            }
            public override bool CanWrite
            {
                get
                {
                    return BaseStream.CanWrite;
                }
            }
            public override void Flush()
            {
                BaseStream.Flush();
            }
            public override long Length
            {
                get
                {
                    return BaseStream.Length;
                }
            }
            public override long Position
            {
                get
                {
                    return BaseStream.Position;
                }
                set
                {
                    BaseStream.Position = value;
                }
            }
            public override long Seek(long Offset, SeekOrigin Origin)
            {
                return BaseStream.Seek(Offset, Origin);
            }
            public override void SetLength(long Value)
            {
                BaseStream.SetLength(Value);
            }
            public override int ReadByte()
            {
                return BaseStream.ReadByte();
            }
            public override void WriteByte(byte Value)
            {
                BaseStream.WriteByte(Value);
            }
            public override int Read(byte[] Buffer, int Offset, int Count)
            {
                BaseStream.Read(Buffer, Offset, Count);
                return Count;
            }
            public override void Write(byte[] Buffer, int Offset, int Count)
            {
                BaseStream.Write(Buffer, Offset, Count);
            }
            protected override void Dispose(bool disposing)
            {
                if (BaseStream is not null)
                {
                    BaseStream.Dispose();
                    BaseStream = null;
                }
                base.Dispose(disposing);
            }
        }

        /// <summary>扩展流适配器类-适配非安全流</summary>
    /// <remarks>用于安全保存StreamEx的Stream形式。</remarks>
        protected class UnsafeStreamAdapter : Stream
        {
            protected internal StreamEx BaseStream;

            public UnsafeStreamAdapter(StreamEx s)
            {
                BaseStream = s;
            }
            public override bool CanRead
            {
                get
                {
                    return BaseStream.CanRead;
                }
            }
            public override bool CanSeek
            {
                get
                {
                    return BaseStream.CanSeek;
                }
            }
            public override bool CanWrite
            {
                get
                {
                    return BaseStream.CanWrite;
                }
            }
            public override void Flush()
            {
                BaseStream.Flush();
            }
            public override long Length
            {
                get
                {
                    return BaseStream.Length;
                }
            }
            public override long Position
            {
                get
                {
                    return BaseStream.Position;
                }
                set
                {
                    BaseStream.Position = value;
                }
            }
            public override long Seek(long Offset, SeekOrigin Origin)
            {
                return BaseStream.Seek(Offset, Origin);
            }
            public override void SetLength(long Value)
            {
                BaseStream.SetLength(Value);
            }
            public override int ReadByte()
            {
                try
                {
                    return BaseStream.ReadByte();
                }
                catch (EndOfStreamException ex)
                {
                    return -1;
                }
            }
            public override void WriteByte(byte Value)
            {
                BaseStream.WriteByte(Value);
            }
            public override int Read(byte[] Buffer, int Offset, int Count)
            {
                if (BaseStream.Position >= BaseStream.Length)
                {
                    return 0;
                }
                else if (BaseStream.Position + Count > BaseStream.Length)
                {
                    int NewCount = (int)(BaseStream.Length - BaseStream.Position);
                    BaseStream.Read(Buffer, Offset, NewCount);
                    return NewCount;
                }
                else
                {
                    BaseStream.Read(Buffer, Offset, Count);
                    return Count;
                }
            }
            public override void Write(byte[] Buffer, int Offset, int Count)
            {
                BaseStream.Write(Buffer, Offset, Count);
            }
            protected override void Dispose(bool disposing)
            {
                if (BaseStream is not null)
                {
                    BaseStream.Dispose();
                    BaseStream = null;
                }
                base.Dispose(disposing);
            }
        }
        #endregion

    }
}