// ==========================================================================
// 
// File:        ListPart.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 列表片
// Version:     2009.11.21.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Firefly
{

    /// <summary>
/// 列表片，列表的一部分
/// </summary>
    public sealed class ListPart<T> : IList<T>, ICloneable
    {

        private IList<T> Internal;
        private int InternalOffset;
        private int InternalLength;

        /// <summary>已重载。考虑到效率原因，不会复制数据，而是直接引用数据，因此传入的数组不得改变</summary>
        public ListPart(IList<T> Data)
        {
            if (Data is null)
                throw new ArgumentNullException();
            Internal = Data;
            InternalOffset = 0;
            InternalLength = Data.Count;
        }
        /// <summary>已重载。考虑到效率原因，不会复制数据，而是直接引用数据，因此传入的数组不得改变</summary>
        public ListPart(IList<T> Data, int Offset, int Length)
        {
            if (Data is null)
                throw new ArgumentNullException();
            if (Offset < 0 || Offset >= Data.Count)
                throw new ArgumentOutOfRangeException();
            Internal = Data;
            InternalOffset = Offset;
            InternalLength = Length;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new MappedEnumerator<int, T>(Enumerable.Range(InternalOffset, InternalLength).GetEnumerator(), i => Internal[i]);
        }

        private IEnumerator GetEnumeratorNonGeneric()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorNonGeneric();

        public object Clone()
        {
            return MemberwiseClone();
        }

        private bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        bool ICollection<T>.IsReadOnly { get => IsReadOnly; }
        private void Add(T Item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Add(T Item) => Add(Item);
        private void Clear()
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear() => Clear();
        private bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item) => Remove(item);
        private void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index) => RemoveAt(index);
        private void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.Insert(int index, T item) => Insert(index, item);

        public bool Contains(T Item)
        {
            return IndexOf(Item) >= 0;
        }

        public void CopyTo(T[] Array, int ArrayIndex)
        {
            for (int n = 0, loopTo = InternalLength - 1; n <= loopTo; n++)
                Array[ArrayIndex + n] = Internal[InternalOffset + n];
        }

        public int Count
        {
            get
            {
                return InternalLength;
            }
        }

        public T this[int Index]
        {
            get
            {
                if (Index < 0 || Index >= InternalLength)
                    throw new ArgumentOutOfRangeException();
                return Internal[InternalOffset + Index];
            }
            set
            {
                if (Index < 0 || Index >= InternalLength)
                    throw new ArgumentOutOfRangeException();
                Internal[InternalOffset + Index] = value;
            }
        }

        public void Reverse()
        {
            Reverse(0, InternalLength);
        }

        public void Reverse(int Index, int Count)
        {
            if (Index < 0 || Index >= InternalLength)
                throw new ArgumentOutOfRangeException();
            if (Index + Count > InternalLength)
                throw new ArgumentOutOfRangeException();
            for (int i = 0, loopTo = Count / 2 - 1; i <= loopTo; i++)
            {
                var tmp = Internal;
                var arga = tmp[Index + i];
                var tmp1 = Internal;
                var argb = tmp1[Index + Count - 1 - i];
                NumericOperations.Exchange(ref arga, ref argb);
                tmp[Index + i] = arga;
                tmp1[Index + Count - 1 - i] = argb;
            }
        }

        public int IndexOf(T Value)
        {
            return IndexOf(Value, 0, InternalLength);
        }

        public int IndexOf(T Value, int Index)
        {
            return IndexOf(Value, Index, InternalLength - Index);
        }

        public int IndexOf(T Value, int Index, int Count)
        {
            if (Index < 0 || Index >= InternalLength)
                throw new ArgumentOutOfRangeException();
            if (Index + Count > InternalLength)
                throw new ArgumentOutOfRangeException();
            var ec = EqualityComparer<T>.Default;
            for (int p = Index, loopTo = Index + Count - 1; p <= loopTo; p++)
            {
                if (ec.Equals(this[p], Value))
                {
                    return p;
                }
            }
            return -1;
        }

        public int IndexOf(IList<T> Value)
        {
            return IndexOf(Value, 0, InternalLength);
        }

        public int IndexOf(IList<T> Value, int Index)
        {
            return IndexOf(Value, Index, InternalLength - Index);
        }

        public int IndexOf(IList<T> Value, int Index, int Count)
        {
            if (Index < 0 || Index >= InternalLength)
                throw new ArgumentOutOfRangeException();
            if (Index + Count > InternalLength)
                throw new ArgumentOutOfRangeException();
            var ec = EqualityComparer<T>.Default;
            for (int p = Index, loopTo = Index + Count - Value.Count; p <= loopTo; p++)
            {
                bool Flag = true;
                for (int k = 0, loopTo1 = Value.Count - 1; k <= loopTo1; k++)
                {
                    if (!ec.Equals(this[p + k], Value[k]))
                    {
                        Flag = false;
                        break;
                    }
                }
                if (Flag)
                    return p;
            }
            return -1;
        }

        public int LastIndexOf(T Value)
        {
            return LastIndexOf(Value, InternalLength - 1, InternalLength);
        }

        public int LastIndexOf(T Value, int Index)
        {
            return LastIndexOf(Value, Index, Index + 1);
        }

        public int LastIndexOf(T Value, int Index, int Count)
        {
            if (Index < 0 || Index >= InternalLength)
                throw new ArgumentOutOfRangeException();
            if (Index >= InternalLength)
                throw new ArgumentOutOfRangeException();
            var ec = EqualityComparer<T>.Default;
            for (int p = Index, loopTo = Index - Count + 1; p >= loopTo; p -= 1)
            {
                if (ec.Equals(this[p], Value))
                {
                    return p;
                }
            }
            return -1;
        }

        public int LastIndexOf(IList<T> Value)
        {
            return LastIndexOf(Value, InternalLength - 1, InternalLength);
        }

        public int LastIndexOf(IList<T> Value, int Index)
        {
            return LastIndexOf(Value, Index, Index + 1);
        }

        public int LastIndexOf(IList<T> Value, int Index, int Count)
        {
            if (Index < 0 || Index >= InternalLength)
                throw new ArgumentOutOfRangeException();
            if (Index >= InternalLength)
                throw new ArgumentOutOfRangeException();
            var ec = EqualityComparer<T>.Default;
            for (int p = Index - Value.Count + 1, loopTo = Index - Count + 1; p >= loopTo; p -= 1)
            {
                bool Flag = true;
                for (int k = 0, loopTo1 = Value.Count - 1; k <= loopTo1; k++)
                {
                    if (!ec.Equals(this[p + k], Value[k]))
                    {
                        Flag = false;
                        break;
                    }
                }
                if (Flag)
                    return p;
            }
            return -1;
        }
    }
}