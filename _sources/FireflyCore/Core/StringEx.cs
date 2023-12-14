// ==========================================================================
// 
// File:        StringEx.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 泛型串
// Version:     2009.11.29.
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
/// 泛型串，特性类似于字符串，但是类型参数不是字符，创建后不可改变，可作为容器类的键使用
/// </summary>
    public class StringEx<T> : ICollection<T>, IEquatable<StringEx<T>>, IComparable<StringEx<T>>, ICloneable
    {

        private IEnumerable<T> Internal;
        private int HashCode;

        /// <summary>已重载。考虑到效率原因，不会复制数据，而是直接引用数据，因此传入的数组不得改变</summary>
        public StringEx(IEnumerable<T> Data)
        {
            if (Data is null)
                throw new ArgumentNullException();
            Internal = Data;

            int hash = 1315423911;
            foreach (var v in Internal)
                hash = hash << 5 ^ v.GetHashCode() ^ hash >> 2 & 0x3FFFFFFF;
            HashCode = hash;
        }

        /// <summary>已重载。考虑到效率原因，不会复制数据，而是直接引用数据，因此传入的数组不得改变</summary>
        public StringEx(IEnumerable<T> Data, int HashCode)
        {
            if (Data is null)
                throw new ArgumentNullException();
            Internal = Data;

            this.HashCode = HashCode;
        }

        public bool Equals(StringEx<T> other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other is null)
                return false;
            if (Internal.Count() != other.Internal.Count())
                return false;
            var ec = EqualityComparer<T>.Default;
            var e = new ZippedEnumerator<T, T, bool>(GetEnumerator(), other.GetEnumerator(), (a, b) => ec.Equals(a, b));
            return new EnumeratorEnumerable<bool>(e).All(b => b);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj is null)
                return false;
            StringEx<T> s = obj as StringEx<T>;
            if (s is null)
                return false;
            return Equals(s);
        }

        public override int GetHashCode()
        {
            return HashCode;
        }

        public int CompareTo(StringEx<T> other)
        {
            if (other is null)
                return int.MinValue;
            IEnumerable<T> Left = this;
            IEnumerable<T> Right = other;
            int LeftCount = Left.Count();
            int RightCount = Right.Count();
            int r = LeftCount - RightCount;
            if (r < 0)
            {
                Right = Right.Take(LeftCount);
            }
            else if (r > 0)
            {
                Left = Left.Take(RightCount);
            }
            var c = Comparer<T>.Default;
            var e = new ZippedEnumerator<T, T, int>(Left.GetEnumerator(), Right.GetEnumerator(), (a, b) => c.Compare(a, b));
            int r2 = new EnumeratorEnumerable<int>(e).FirstOrDefault(i => i != 0);
            if (r2 != 0)
                return r2;
            return r;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Internal.GetEnumerator();
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

        public bool Contains(T Item)
        {
            return Internal.Contains(Item);
        }

        public void CopyTo(T[] Array, int ArrayIndex)
        {
            for (int n = 0, loopTo = Internal.Count() - 1; n <= loopTo; n++)
                Array[ArrayIndex + n] = Internal.ElementAtOrDefault(n);
        }

        public int Count
        {
            get
            {
                return Internal.Count();
            }
        }

        public StringEx<T> Reverse()
        {
            return Reverse(0, Internal.Count());
        }

        public StringEx<T> Reverse(int Index, int Count)
        {
            if (Index < 0 || Index >= Internal.Count())
                throw new ArgumentOutOfRangeException();
            if (Index + Count > Internal.Count())
                throw new ArgumentOutOfRangeException();
            T[] Result = Internal.ToArray();
            for (int i = 0, loopTo = Count / 2 - 1; i <= loopTo; i++)
            {
                var argb = Internal.ElementAtOrDefault(Index + Count - 1 - i);
                NumericOperations.Exchange(ref Result[Index + i], ref argb);
            }
            return new StringEx<T>(Result);
        }

        public static bool operator ==(StringEx<T> Left, StringEx<T> Right)
        {
            if (Left is null && Right is null)
                return true;
            if (Left is null || Right is null)
                return false;
            return Left.Equals(Right);
        }

        public static bool operator !=(StringEx<T> Left, StringEx<T> Right)
        {
            if (Left is null && Right is null)
                return false;
            if (Left is null || Right is null)
                return true;
            return !Left.Equals(Right);
        }

        public static bool operator >=(StringEx<T> Left, StringEx<T> Right)
        {
            if (Right is null)
                return true;
            if (Left is null)
                return false;
            return Left.CompareTo(Right) >= 0;
        }

        public static bool operator <=(StringEx<T> Left, StringEx<T> Right)
        {
            if (Left is null)
                return true;
            if (Right is null)
                return false;
            return Left.CompareTo(Right) <= 0;
        }

        public static bool operator >(StringEx<T> Left, StringEx<T> Right)
        {
            return !(Left <= Right);
        }

        public static bool operator <(StringEx<T> Left, StringEx<T> Right)
        {
            return !(Left >= Right);
        }
    }
}