// ==========================================================================
// 
// File:        ArrayOperations.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 数组操作
// Version:     2009.01.21.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;

namespace Firefly
{

    /// <summary>数组操作</summary>
    public static class ArrayOperations
    {
        /// <summary>
    /// 已重载。获取数组的子数组。
    /// </summary>
    /// <param name="This">数组</param>
    /// <param name="StartIndex">起始索引</param>
    /// <param name="Length">长度</param>
        public static T[] SubArray<T>(this T[] This, int StartIndex, int Length)
        {
            T[] s = new T[Length];
            Array.Copy(This, StartIndex, s, 0, Length);
            return s;
        }

        /// <summary>
    /// 已重载。获取数组的子数组。
    /// </summary>
    /// <param name="This">数组</param>
    /// <param name="StartIndex">起始索引</param>
        public static T[] SubArray<T>(this T[] This, int StartIndex)
        {
            return This.SubArray(StartIndex, This.Length - StartIndex);
        }

        /// <summary>
    /// 返回数组的扩展数组。
    /// </summary>
    /// <param name="This">数组</param>
    /// <param name="Length">新长度</param>
    /// <param name="Value">初始值</param>
        public static T[] Extend<T>(this T[] This, int Length, T Value)
        {
            if (This.Length > Length)
                throw new ArgumentOutOfRangeException();
            T[] newBytes = new T[Length];
            Array.Copy(This, newBytes, NumericOperations.Min(This.Length, Length));
            for (int n = NumericOperations.Min(This.Length, Length), loopTo = Length - 1; n <= loopTo; n++)
                newBytes[n] = Value;
            return newBytes;
        }

        /// <summary>
    /// 对指定数组的每个元素执行指定操作。
    /// </summary>
    /// <typeparam name="T">数组元素的类型。</typeparam>
    /// <param name="This">从零开始的一维 Array，要对其元素执行操作。</param>
    /// <param name="Action">要对 array 的每个元素执行的 Action(Of T)。</param>
        public static void ForEach<T>(this T[] This, Action<T> Action)
        {
            Array.ForEach(This, Action);
        }

        /// <summary>
    /// 判断数组是否元素完全相等。
    /// </summary>
        public static bool ArrayEqual<T>(this T[] a, T[] b)
        {
            if (a.Length != b.Length)
                return false;
            for (int n = 0, loopTo = a.Length - 1; n <= loopTo; n++)
            {
                if (!a[n].Equals(b[n]))
                    return false;
            }
            return true;
        }

        /// <summary>
    /// 判断数组是否元素完全相等。
    /// </summary>
        public static bool ArrayEqual<T>(this T[] a, T[] b, int bIndex, int bCount)
        {
            if (a.Length != bCount)
                return false;
            for (int n = 0, loopTo = a.Length - 1; n <= loopTo; n++)
            {
                if (!a[n].Equals(b[bIndex + n]))
                    return false;
            }
            return true;
        }
    }
}