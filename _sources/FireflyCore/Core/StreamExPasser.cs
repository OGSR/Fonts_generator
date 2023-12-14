// ==========================================================================
// 
// File:        StreamExPasser.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 扩展流传递器，用于显式确定函数传参时的流是否包含长度位置信息。
// Version:     2010.03.28.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;

namespace Firefly
{

    /// <summary>零长度零位置扩展流传递器。用于保证在函数传参时传递零长度零位置的流。</summary>
    public class ZeroLengthStreamPasser
    {
        protected StreamEx BaseStream;

        public ZeroLengthStreamPasser(StreamEx s)
        {
            if (s.Length != 0L)
                throw new ArgumentException("LengthNotZero");
            if (s.Position != 0L)
                throw new ArgumentException("PositionNotZero");
            BaseStream = s;
        }

        public StreamEx GetStream()
        {
            if (BaseStream.Length != 0L)
                throw new ArgumentException("LengthNotZero");
            if (BaseStream.Position != 0L)
                throw new ArgumentException("PositionNotZero");
            return BaseStream;
        }

        public static implicit operator ZeroLengthStreamPasser(StreamEx s)
        {
            return new ZeroLengthStreamPasser(s);
        }

        public static implicit operator StreamEx(ZeroLengthStreamPasser p)
        {
            return p.GetStream();
        }
    }

    /// <summary>零位置扩展流传递器。用于保证在函数传参时传递零位置的流。</summary>
    public class ZeroPositionStreamPasser
    {
        protected StreamEx BaseStream;

        public ZeroPositionStreamPasser(StreamEx s)
        {
            if (s.Position != 0L)
                throw new ArgumentException("PositionNotZero");
            BaseStream = s;
        }

        public StreamEx GetStream()
        {
            if (BaseStream.Position != 0L)
                throw new ArgumentException("PositionNotZero");
            return BaseStream;
        }

        public static implicit operator ZeroPositionStreamPasser(StreamEx s)
        {
            return new ZeroPositionStreamPasser(s);
        }

        public static implicit operator StreamEx(ZeroPositionStreamPasser p)
        {
            return p.GetStream();
        }

        public static implicit operator ZeroPositionStreamPasser(ZeroLengthStreamPasser p)
        {
            return new ZeroPositionStreamPasser(p.GetStream());
        }
    }

    /// <summary>有位置的扩展流传递器。用于显式申明函数传参时传递的流有位置信息。</summary>
    public class PositionedStreamPasser
    {
        protected StreamEx BaseStream;

        public PositionedStreamPasser(StreamEx s)
        {
            BaseStream = s;
        }

        public StreamEx GetStream()
        {
            return BaseStream;
        }

        public static implicit operator PositionedStreamPasser(StreamEx s)
        {
            return new PositionedStreamPasser(s);
        }

        public static implicit operator StreamEx(PositionedStreamPasser p)
        {
            return p.GetStream();
        }
    }
}