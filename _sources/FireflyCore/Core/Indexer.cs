// ==========================================================================
// 
// File:        Indexer.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 离散索引器
// Version:     2010.02.18.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;

namespace Firefly
{

    /// <summary>
/// 离散索引器，用于表示离散整数区间，并提供遍历离散整数区间的函数与枚举器支持。
/// 支持使用For Each语法遍历区间内的所有点。
/// </summary>
    public class Indexer : IEnumerator<int>, IEnumerable<int>
    {
        protected SortedList<int, Range> Descriptor = new SortedList<int, Range>();
        protected int Value;
        protected int Position;

        public Indexer(ICollection<Range> Descriptors)
        {
            foreach (Range d in Descriptors)
            {
                if (d.Lower == int.MinValue)
                    throw new InvalidDataException();
                Descriptor.Add(d.Lower, d);
            }
            Value = int.MinValue;
            Position = 0;
        }
        public void AddDescriptor(Range d)
        {
            if (d.Lower == int.MinValue)
                throw new InvalidDataException();
            Descriptor.Add(d.Lower, d);
            Position = 0;
        }
        public void RemoveDescriptor(Range d)
        {
            Descriptor.Remove(d.Lower);
            Position = 0;
        }

        public bool Contain(int i)
        {
            if (Descriptor.Count == 0)
                return false;
            int U = Descriptor.Count - 1;
            int M = U / 2;
            while (U > 0)
            {
                if (Descriptor.Keys[M] > i)
                {
                    U = M;
                    M = U / 2;
                }
                else
                {
                    break;
                }
            }
            U = M;
            for (int n = U; n >= 0; n -= 1)
            {
                if (Descriptor[Descriptor.Keys[n]].Contain(i))
                    return true;
            }
            return false;
        }

        public int Current
        {
            get
            {
                return Value;
            }
        }
        private object CurrentNonGeneric
        {
            get
            {
                return Value;
            }
        }

        object System.Collections.IEnumerator.Current { get => CurrentNonGeneric; }

        public bool MoveNext()
        {
            if (Descriptor.Count == 0)
                return false;
            int v = Value + 1;
            while (v >= Descriptor.Values[Position].Lower + Descriptor.Values[Position].Count)
            {
                Position += 1;
                if (Position >= Descriptor.Count)
                    return false;
            }
            if (v < Descriptor.Values[Position].Lower)
                v = Descriptor.Values[Position].Lower;
            Value = v;
            return true;
        }

        public void SetBefore(int Index)
        {
            Value = Index - 1;
        }

        public void Reset()
        {
            if (Descriptor.Count < 0)
                throw new InvalidOperationException();
            Value = int.MinValue;
            Position = 0;
        }

        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Descriptor = null;
            }
            disposedValue = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerator<int> GetEnumerator()
        {
            return this;
        }

        private System.Collections.IEnumerator GetEnumeratorNonGeneric()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumeratorNonGeneric();
    }

    /// <summary>范围，离散索引器描述器，用于表示离散索引器中的一段连续整数区间</summary>
    public class Range
    {
        public int Lower;
        public int Upper;
        public int Count
        {
            get
            {
                return Upper - Lower + 1;
            }
            set
            {
                Upper = Lower + value - 1;
            }
        }
        public Range(int Lower, int Upper)
        {
            if (Upper < Lower)
                throw new ArgumentOutOfRangeException();
            this.Lower = Lower;
            this.Upper = Upper;
        }
        public bool Contain(int i)
        {
            return i >= Lower && i <= Upper;
        }
    }
}