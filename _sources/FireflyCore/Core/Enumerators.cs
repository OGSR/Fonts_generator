// ==========================================================================
// 
// File:        Enumerators.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 枚举器
// Version:     2009.11.21.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections;
using System.Collections.Generic;

namespace Firefly
{

    /// <summary>映射式枚举器</summary>
    public class MappedEnumerator<TKey, TValue> : IEnumerator<TValue>
    {

        private IEnumerator<TKey> BaseEnumerator;
        private Func<TKey, TValue> Mapping;

        public MappedEnumerator(IEnumerator<TKey> BaseEnumerator, Func<TKey, TValue> Mapping)
        {
            this.BaseEnumerator = BaseEnumerator;
            this.Mapping = Mapping;
        }

        public TValue Current
        {
            get
            {
                return Mapping(BaseEnumerator.Current);
            }
        }

        private object GetEnumeratorNonGeneric
        {
            get
            {
                return Current;
            }
        }

        object IEnumerator.Current { get => GetEnumeratorNonGeneric; }

        public bool MoveNext()
        {
            return BaseEnumerator.MoveNext();
        }

        public void Reset()
        {
            BaseEnumerator.Reset();
        }

        #region  IDisposable 支持 
        private bool disposedValue = false; // 检测冗余的调用
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放其他状态(托管对象)。
                    BaseEnumerator.Dispose();
                }

                // 释放您自己的状态(非托管对象)。
                // 将大型字段设置为 null。
            }
            disposedValue = true;
        }
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入上面的 Dispose(ByVal disposing As Boolean) 中。
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }

    /// <summary>Zip式枚举器</summary>
    public class ZippedEnumerator<TKeyA, TKeyB, TValue> : IEnumerator<TValue>
    {

        private IEnumerator<TKeyA> BaseEnumeratorA;
        private IEnumerator<TKeyB> BaseEnumeratorB;
        private Func<TKeyA, TKeyB, TValue> Zipping;

        public ZippedEnumerator(IEnumerator<TKeyA> BaseEnumeratorA, IEnumerator<TKeyB> BaseEnumeratorB, Func<TKeyA, TKeyB, TValue> Zipping)
        {
            this.BaseEnumeratorA = BaseEnumeratorA;
            this.BaseEnumeratorB = BaseEnumeratorB;
            this.Zipping = Zipping;
        }

        public TValue Current
        {
            get
            {
                return Zipping(BaseEnumeratorA.Current, BaseEnumeratorB.Current);
            }
        }

        private object GetEnumeratorNonGeneric
        {
            get
            {
                return Current;
            }
        }

        object IEnumerator.Current { get => GetEnumeratorNonGeneric; }

        public bool MoveNext()
        {
            bool ResultA = BaseEnumeratorA.MoveNext();
            bool ResultB = BaseEnumeratorB.MoveNext();
            if (ResultA != ResultB)
                throw new InvalidOperationException();
            return ResultA;
        }

        public void Reset()
        {
            BaseEnumeratorA.Reset();
            BaseEnumeratorB.Reset();
        }

        #region  IDisposable 支持 
        private bool disposedValue = false; // 检测冗余的调用
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放其他状态(托管对象)。
                    BaseEnumeratorA.Dispose();
                    BaseEnumeratorB.Dispose();
                }

                // 释放您自己的状态(非托管对象)。
                // 将大型字段设置为 null。
            }
            disposedValue = true;
        }
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入上面的 Dispose(ByVal disposing As Boolean) 中。
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }

    /// <summary>用于转换泛型Enumerator到泛型IEnumerable</summary>
    public class EnumeratorEnumerable<T> : IEnumerable<T>
    {

        private IEnumerator<T> BaseEnumerator;

        public EnumeratorEnumerable(IEnumerator<T> BaseEnumerator)
        {
            this.BaseEnumerator = BaseEnumerator;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return BaseEnumerator;
        }

        private IEnumerator GetEnumeratorNonGeneric()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorNonGeneric();
    }
}