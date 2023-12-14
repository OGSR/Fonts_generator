// ==========================================================================
// 
// File:        MultiByteEncoding.vb
// Location:    Firefly.TextEncoding <Visual Basic .Net>
// Description: 多字节编码
// Version:     2009.11.28.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Firefly.TextEncoding
{
    public sealed class MultiByteEncoding : Encoding
    {

        private Dictionary<StringEx<byte>, StringEx<char>> BytesToChars;
        private Dictionary<StringEx<char>, StringEx<byte>> CharsToBytes;
        private Tokenizer<byte, char> CharTokenizer;
        private Tokenizer<char, byte> ByteTokenizer;

        public MultiByteEncoding(IEnumerable<StringCode> l)
        {
            BytesToChars = new Dictionary<StringEx<byte>, StringEx<char>>();
            CharsToBytes = new Dictionary<StringEx<char>, StringEx<byte>>();
            string ReplacementString = "";
            foreach (var sc in l)
            {
                if (!sc.HasUnicode)
                    continue;
                if (!sc.HasCode)
                    continue;
                if (sc.Character == "?")
                    ReplacementString = "?";

                StringEx<byte> bl;
                if (sc.CodeLength == 1 || sc.CodeLength == -1 && (sc.Code & int.MinValue + 0x7FFFFF00) == 0)
                {
                    bl = new StringEx<byte>(new byte[] { (byte)sc.Code });
                }
                else if (sc.CodeLength == 2 || sc.CodeLength == -1 && (sc.Code & int.MinValue + 0x7FFF0000) == 0)
                {
                    bl = new StringEx<byte>(new byte[] { (byte)sc.Code.Bits(15, 8), (byte)sc.Code.Bits(7, 0) });
                }
                else if (sc.CodeLength == 3 || sc.CodeLength == -1 && (sc.Code & int.MinValue + 0x7F000000) == 0)
                {
                    bl = new StringEx<byte>(new byte[] { (byte)sc.Code.Bits(23, 16), (byte)sc.Code.Bits(15, 8), (byte)sc.Code.Bits(7, 0) });
                }
                else if (sc.CodeLength == 4 || sc.CodeLength == -1)
                {
                    bl = new StringEx<byte>(new byte[] { (byte)sc.Code.Bits(31, 24), (byte)sc.Code.Bits(23, 16), (byte)sc.Code.Bits(15, 8), (byte)sc.Code.Bits(7, 0) });
                }
                else
                {
                    throw new InvalidDataException();
                }
                var cl = new StringEx<char>(sc.Character.ToCharArray());

                if (!BytesToChars.ContainsKey(bl))
                    BytesToChars.Add(bl, cl);
                if (!CharsToBytes.ContainsKey(cl))
                    CharsToBytes.Add(cl, bl);
            }

            CharTokenizer = new Tokenizer<byte, char>(BytesToChars);
            ByteTokenizer = new Tokenizer<char, byte>(CharsToBytes);

            var fi = typeof(Encoding).GetField("m_isReadOnly", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetField);
            fi.SetValue(this, false);

            EncoderFallback = new EncoderReplacementFallback(ReplacementString);
            DecoderFallback = new DecoderReplacementFallback(ReplacementString);
        }

        public override int GetMaxByteCount(int charCount)
        {
            return charCount * ByteTokenizer.MaxTokenPerSymbol;
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount * CharTokenizer.MaxTokenPerSymbol;
        }

        public override Encoder GetEncoder()
        {
            return new InternalEncoder(this);
        }

        public override Decoder GetDecoder()
        {
            return new InternalDecoder(this);
        }

        private class Counter<T>
        {
            public int Tick = 0;
            public void Count(T Value)
            {
                Tick += 1;
            }
        }

        private bool DoEncoderCountFallback(char CharUnknown, int Index, Counter<byte> Counter)
        {
            var Buffer = EncoderFallback.CreateFallbackBuffer();
            bool ret = Buffer.Fallback(CharUnknown, Index);
            if (Buffer.Remaining > 0)
            {
                Counter.Tick += Buffer.Remaining;
            }
            return ret;
        }

        public override int GetByteCount(char[] chars, int index, int count)
        {
            var State = ByteTokenizer.GetDefaultState();
            var outputCounter = new Counter<byte>() { Tick = 0 };
            using (var input = new ArrayStream<char>(chars, index, count))
            {
                while (input.Position < input.Length)
                {
                    State = ByteTokenizer.Feed(State, input.ReadElement());
                    State = ByteTokenizer.Transit(State, outputCounter.Count, (c, Offset) => DoEncoderCountFallback(c, input.Position + Offset, outputCounter));
                }
                State = ByteTokenizer.Finish(State, outputCounter.Count, (c, Offset) => DoEncoderCountFallback(c, input.Position + Offset, outputCounter));
            }
            return outputCounter.Tick;
        }

        private bool DoEncoderFallback(char CharUnknown, int Index, Action<byte> WriteOutput)
        {
            var Buffer = EncoderFallback.CreateFallbackBuffer();
            bool ret = Buffer.Fallback(CharUnknown, Index);
            var l = new List<char>();
            while (Buffer.Remaining > 0)
                l.Add(Buffer.GetNextChar());
            foreach (var v in GetBytes(l.ToArray()))
                WriteOutput(v);
            return ret;
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            var State = ByteTokenizer.GetDefaultState();
            using (var output = new ArrayStream<byte>(bytes, byteIndex))
            {
                using (var input = new ArrayStream<char>(chars, charIndex, charCount))
                {
                    while (input.Position < input.Length)
                    {
                        State = ByteTokenizer.Feed(State, input.ReadElement());
                        State = ByteTokenizer.Transit(State, output.WriteElement, (c, Offset) => DoEncoderFallback(c, input.Position + Offset, output.WriteElement));
                    }
                    State = ByteTokenizer.Finish(State, output.WriteElement, (c, Offset) => DoEncoderFallback(c, input.Position + Offset, output.WriteElement));
                }
                return output.Position;
            }
        }

        private bool DoDecoderCountFallback(byte ByteUnknown, int Index, Counter<char> Counter)
        {
            var Buffer = DecoderFallback.CreateFallbackBuffer();
            bool ret = Buffer.Fallback(new byte[] { ByteUnknown }, Index);
            if (Buffer.Remaining > 0)
            {
                Counter.Tick += Buffer.Remaining;
            }
            return ret;
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            var State = CharTokenizer.GetDefaultState();
            var outputCounter = new Counter<char>() { Tick = 0 };
            using (var input = new ArrayStream<byte>(bytes, index, count))
            {
                while (input.Position < input.Length)
                {
                    State = CharTokenizer.Feed(State, input.ReadElement());
                    State = CharTokenizer.Transit(State, outputCounter.Count, (c, Offset) => DoDecoderCountFallback(c, input.Position + Offset, outputCounter));
                }
                State = CharTokenizer.Finish(State, outputCounter.Count, (c, Offset) => DoDecoderCountFallback(c, input.Position + Offset, outputCounter));
            }
            return outputCounter.Tick;
        }

        private bool DoDecoderFallback(byte ByteUnknown, int Index, Action<char> WriteOutput)
        {
            var Buffer = DecoderFallback.CreateFallbackBuffer();
            bool ret = Buffer.Fallback(new byte[] { ByteUnknown }, Index);
            while (Buffer.Remaining > 0)
                WriteOutput(Buffer.GetNextChar());
            return ret;
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            var State = CharTokenizer.GetDefaultState();
            using (var output = new ArrayStream<char>(chars, charIndex))
            {
                using (var input = new ArrayStream<byte>(bytes, byteIndex, byteCount))
                {
                    while (input.Position < input.Length)
                    {
                        State = CharTokenizer.Feed(State, input.ReadElement());
                        State = CharTokenizer.Transit(State, output.WriteElement, (c, Offset) => DoDecoderFallback(c, input.Position + Offset, output.WriteElement));
                    }
                    State = CharTokenizer.Finish(State, output.WriteElement, (c, Offset) => DoDecoderFallback(c, input.Position + Offset, output.WriteElement));
                }
                return output.Position;
            }
        }

        /// <summary>辅助类，仅仅是为了是使用跨缓冲区多次GetBytes的.Net内部的类正常。</summary>
        private class InternalEncoder : Encoder
        {

            public MultiByteEncoding Encoding;
            public TokenizerState<byte, char> State;

            public InternalEncoder(MultiByteEncoding Encoding)
            {
                this.Encoding = Encoding;
                State = Encoding.ByteTokenizer.GetDefaultState();
            }

            public override void Convert(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, int byteCount, bool flush, out int charsUsed, out int bytesUsed, out bool completed)
            {
                using (var output = new ArrayStream<byte>(bytes, byteIndex, byteCount))
                {
                    using (var input = new ArrayStream<char>(chars, charIndex, charCount))
                    {
                        while (input.Position < input.Length && output.Position < output.Length)
                        {
                            State = Encoding.ByteTokenizer.Feed(State, input.ReadElement());
                            State = Encoding.ByteTokenizer.Transit(State, output.WriteElement, (c, Offset) => Encoding.DoEncoderFallback(c, input.Position + Offset, output.WriteElement));
                        }
                        if (flush)
                        {
                            if (output.Position < output.Length)
                            {
                                State = Encoding.ByteTokenizer.Finish(State, output.WriteElement, (c, Offset) => Encoding.DoEncoderFallback(c, input.Position + Offset, output.WriteElement));
                            }
                        }
                        charsUsed = input.Position;
                        bytesUsed = output.Position;
                        completed = input.Position == input.Length && Encoding.ByteTokenizer.IsStateFinished(State);
                    }
                }
            }

            public override int GetByteCount(char[] chars, int index, int count, bool flush)
            {
                var outputCounter = new Counter<byte>() { Tick = 0 };
                using (var input = new ArrayStream<char>(chars, index, count))
                {
                    while (input.Position < input.Length)
                    {
                        State = Encoding.ByteTokenizer.Feed(State, input.ReadElement());
                        State = Encoding.ByteTokenizer.Transit(State, outputCounter.Count, (c, Offset) => Encoding.DoEncoderCountFallback(c, input.Position + Offset, outputCounter));
                    }
                    if (flush)
                        State = Encoding.ByteTokenizer.Finish(State, outputCounter.Count, (c, Offset) => Encoding.DoEncoderCountFallback(c, input.Position + Offset, outputCounter));
                }
                return outputCounter.Tick;
            }

            public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, bool flush)
            {
                int byteCount = bytes.Length - byteIndex;
                int bytesUsed = 0;
                int charsUsed = 0;
                bool completed = false;
                Convert(chars, charIndex, charCount, bytes, byteIndex, byteCount, flush, out charsUsed, out bytesUsed, out completed);
                return bytesUsed;
            }

            public override void Reset()
            {
                State = Encoding.ByteTokenizer.GetDefaultState();
            }
        }

        /// <summary>辅助类，仅仅是为了是使用跨缓冲区多次GetChars的.Net内部的类正常。</summary>
        private class InternalDecoder : Decoder
        {

            public MultiByteEncoding Encoding;
            public TokenizerState<char, byte> State;

            public InternalDecoder(MultiByteEncoding Encoding)
            {
                this.Encoding = Encoding;
                State = Encoding.CharTokenizer.GetDefaultState();
            }

            public override void Convert(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, int charCount, bool flush, out int bytesUsed, out int charsUsed, out bool completed)
            {
                using (var output = new ArrayStream<char>(chars, charIndex, charCount))
                {
                    using (var input = new ArrayStream<byte>(bytes, byteIndex, byteCount))
                    {
                        while (input.Position < input.Length && output.Position < output.Length)
                        {
                            State = Encoding.CharTokenizer.Feed(State, input.ReadElement());
                            State = Encoding.CharTokenizer.Transit(State, output.WriteElement, (c, Offset) => Encoding.DoDecoderFallback(c, input.Position + Offset, output.WriteElement));
                        }
                        if (flush)
                        {
                            if (output.Position < output.Length)
                            {
                                State = Encoding.CharTokenizer.Finish(State, output.WriteElement, (c, Offset) => Encoding.DoDecoderFallback(c, input.Position + Offset, output.WriteElement));
                            }
                        }
                        bytesUsed = input.Position;
                        charsUsed = output.Position;
                        completed = input.Position == input.Length && Encoding.CharTokenizer.IsStateFinished(State);
                    }
                }
            }

            public override int GetCharCount(byte[] bytes, int index, int count, bool flush)
            {
                var outputCounter = new Counter<char>() { Tick = 0 };
                using (var input = new ArrayStream<byte>(bytes, index, count))
                {
                    while (input.Position < input.Length)
                    {
                        State = Encoding.CharTokenizer.Feed(State, input.ReadElement());
                        State = Encoding.CharTokenizer.Transit(State, outputCounter.Count, (c, Offset) => Encoding.DoDecoderCountFallback(c, input.Position + Offset, outputCounter));
                    }
                    if (flush)
                        State = Encoding.CharTokenizer.Finish(State, outputCounter.Count, (c, Offset) => Encoding.DoDecoderCountFallback(c, input.Position + Offset, outputCounter));
                }
                return outputCounter.Tick;
            }

            public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, bool flush)
            {
                int charCount = chars.Length - charIndex;
                int bytesUsed = 0;
                int charsUsed = 0;
                bool completed = false;
                Convert(bytes, byteIndex, byteCount, chars, charIndex, charCount, flush, out bytesUsed, out charsUsed, out completed);
                return charsUsed;
            }

            public override int GetCharCount(byte[] bytes, int index, int count)
            {
                return GetCharCount(bytes, index, count, false);
            }

            public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
            {
                return GetChars(bytes, byteIndex, byteCount, chars, charIndex, false);
            }

            public override void Reset()
            {
                State = Encoding.CharTokenizer.GetDefaultState();
            }
        }
    }
}