// ==========================================================================
// 
// File:        NumericOperations.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 数值操作
// Version:     2008.11.08.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;

namespace Firefly
{

    /// <summary>数值操作</summary>
    public static class NumericOperations
    {
        public static T Max<T>(T a, T b) where T : IComparable
        {
            if (a is not null)
            {
                if (a.CompareTo(b) >= 0)
                    return a;
            }
            return b;
        }
        public static T Min<T>(T a, T b) where T : IComparable
        {
            if (a is not null)
            {
                if (a.CompareTo(b) >= 0)
                    return b;
            }
            return a;
        }
        public static T Max<T>(T a, params T[] b) where T : IComparable
        {
            var ret = a;
            foreach (T x in b)
            {
                if (x is not null)
                {
                    if (x.CompareTo(ret) >= 0)
                        ret = x;
                }
            }
            return ret;
        }
        public static T Min<T>(T a, params T[] b) where T : IComparable
        {
            var ret = a;
            foreach (T x in b)
            {
                if (ret is not null)
                {
                    if (ret.CompareTo(x) >= 0)
                        ret = x;
                }
            }
            return ret;
        }
        public static T Exchange<T>(ref T a, ref T b)
        {
            T Temp;
            Temp = a;
            a = b;
            b = Temp;
            return default;
        }
    }
}