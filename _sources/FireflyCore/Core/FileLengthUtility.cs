// ==========================================================================
// 
// File:        FileLengthUtility.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 文件长度辅助函数库
// Version:     2009.07.28.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;

namespace Firefly
{

    public static class FileLengthUtility
    {
        /// <summary>已重载。得到数组的差分，用Sum参数放在Value最后来凑齐</summary>
        public static short[] GetDifference(short[] Value, short Sum)
        {
            short[] ret = new short[Value.Length];
            short Upper = Sum;
            for (int n = Value.Length - 1; n >= 0; n -= 1)
            {
                ret[n] = (short)(Upper - Value[n]);
                Upper = Value[n];
            }
            return ret;
        }
        /// <summary>已重载。得到数组的差分，用Sum参数放在Value最后来凑齐</summary>
        public static ushort[] GetDifference(ushort[] Value, ushort Sum)
        {
            ushort[] ret = new ushort[Value.Length];
            ushort Upper = Sum;
            for (int n = Value.Length - 1; n >= 0; n -= 1)
            {
                ret[n] = (ushort)(Upper - Value[n]);
                Upper = Value[n];
            }
            return ret;
        }
        /// <summary>已重载。得到数组的差分，用Sum参数放在Value最后来凑齐</summary>
        public static int[] GetDifference(int[] Value, int Sum)
        {
            int[] ret = new int[Value.Length];
            int Upper = Sum;
            for (int n = Value.Length - 1; n >= 0; n -= 1)
            {
                ret[n] = Upper - Value[n];
                Upper = Value[n];
            }
            return ret;
        }
        /// <summary>已重载。得到数组的差分，用Sum参数放在Value最后来凑齐</summary>
        public static long[] GetDifference(long[] Value, long Sum)
        {
            long[] ret = new long[Value.Length];
            long Upper = Sum;
            for (int n = Value.Length - 1; n >= 0; n -= 1)
            {
                ret[n] = Upper - Value[n];
                Upper = Value[n];
            }
            return ret;
        }
        /// <summary>已重载。得到数组的求和，是GetDifference的逆运算</summary>
        public static short[] GetSummation(short Intial, short[] Value)
        {
            short[] ret = new short[Value.Length];
            short Address = Intial;
            for (int n = 0, loopTo = Value.Length - 1; n <= loopTo; n++)
            {
                ret[n] = Address;
                Address += Value[n];
            }
            return ret;
        }
        /// <summary>已重载。得到数组的求和，是GetDifference的逆运算</summary>
        public static ushort[] GetSummation(ushort Intial, ushort[] Value)
        {
            ushort[] ret = new ushort[Value.Length];
            ushort Address = Intial;
            for (int n = 0, loopTo = Value.Length - 1; n <= loopTo; n++)
            {
                ret[n] = Address;
                Address += Value[n];
            }
            return ret;
        }
        /// <summary>已重载。得到数组的求和，是GetDifference的逆运算</summary>
        public static int[] GetSummation(int Intial, int[] Value)
        {
            int[] ret = new int[Value.Length];
            int Address = Intial;
            for (int n = 0, loopTo = Value.Length - 1; n <= loopTo; n++)
            {
                ret[n] = Address;
                Address += Value[n];
            }
            return ret;
        }
        /// <summary>已重载。得到数组的求和，是GetDifference的逆运算</summary>
        public static long[] GetSummation(long Intial, long[] Value)
        {
            long[] ret = new long[Value.Length];
            long Address = Intial;
            for (int n = 0, loopTo = Value.Length - 1; n <= loopTo; n++)
            {
                ret[n] = Address;
                Address += Value[n];
            }
            return ret;
        }


        /// <summary>已重载。得到地址列的差分，用Length参数放在Address最后来凑齐，Address为0表示长度为0</summary>
        public static short[] GetAddressDifference(short[] Address, short Length)
        {
            short[] ret = new short[Address.Length];
            short Upper = Length;
            for (int n = Address.Length - 1; n >= 0; n -= 1)
            {
                if (Address[n] == 0)
                {
                    ret[n] = 0;
                }
                else
                {
                    ret[n] = (short)(Upper - Address[n]);
                    Upper = Address[n];
                }
            }
            return ret;
        }
        /// <summary>已重载。得到地址列的差分，用Length参数放在Address最后来凑齐，Address为0表示长度为0</summary>
        public static ushort[] GetAddressDifference(ushort[] Address, ushort Length)
        {
            ushort[] ret = new ushort[Address.Length];
            ushort Upper = Length;
            for (int n = Address.Length - 1; n >= 0; n -= 1)
            {
                if (Address[n] == 0)
                {
                    ret[n] = 0;
                }
                else
                {
                    ret[n] = (ushort)(Upper - Address[n]);
                    Upper = Address[n];
                }
            }
            return ret;
        }
        /// <summary>已重载。得到地址列的差分，用Length参数放在Address最后来凑齐，Address为0表示长度为0</summary>
        public static int[] GetAddressDifference(int[] Address, int Length)
        {
            int[] ret = new int[Address.Length];
            int Upper = Length;
            for (int n = Address.Length - 1; n >= 0; n -= 1)
            {
                if (Address[n] == 0)
                {
                    ret[n] = 0;
                }
                else
                {
                    ret[n] = Upper - Address[n];
                    Upper = Address[n];
                }
            }
            return ret;
        }
        /// <summary>已重载。得到地址列的差分，用Length参数放在Address最后来凑齐，Address为0表示长度为0</summary>
        public static long[] GetAddressDifference(long[] Address, long Length)
        {
            long[] ret = new long[Address.Length];
            long Upper = Length;
            for (int n = Address.Length - 1; n >= 0; n -= 1)
            {
                if (Address[n] == 0L)
                {
                    ret[n] = 0L;
                }
                else
                {
                    ret[n] = Upper - Address[n];
                    Upper = Address[n];
                }
            }
            return ret;
        }
        /// <summary>已重载。得到地址列的求和，是GetAddressDifference的逆运算，长度为0则Address置为0</summary>
        public static short[] GetAddressSummation(short BaseAddress, short[] Length)
        {
            short[] ret = new short[Length.Length];
            short Address = BaseAddress;
            for (int n = 0, loopTo = Length.Length - 1; n <= loopTo; n++)
            {
                if (Length[n] == 0)
                {
                    ret[n] = 0;
                }
                else
                {
                    ret[n] = Address;
                    Address += Length[n];
                }
            }
            return ret;
        }
        /// <summary>已重载。得到地址列的求和，是GetAddressDifference的逆运算，长度为0则Address置为0</summary>
        public static ushort[] GetAddressSummation(ushort BaseAddress, ushort[] Length)
        {
            ushort[] ret = new ushort[Length.Length];
            ushort Address = BaseAddress;
            for (int n = 0, loopTo = Length.Length - 1; n <= loopTo; n++)
            {
                if (Length[n] == 0)
                {
                    ret[n] = 0;
                }
                else
                {
                    ret[n] = Address;
                    Address += Length[n];
                }
            }
            return ret;
        }
        /// <summary>已重载。得到地址列的求和，是GetAddressDifference的逆运算，长度为0则Address置为0</summary>
        public static int[] GetAddressSummation(int BaseAddress, int[] Length)
        {
            int[] ret = new int[Length.Length];
            int Address = BaseAddress;
            for (int n = 0, loopTo = Length.Length - 1; n <= loopTo; n++)
            {
                if (Length[n] == 0)
                {
                    ret[n] = 0;
                }
                else
                {
                    ret[n] = Address;
                    Address += Length[n];
                }
            }
            return ret;
        }
        /// <summary>已重载。得到地址列的求和，是GetAddressDifference的逆运算，长度为0则Address置为0</summary>
        public static long[] GetAddressSummation(long BaseAddress, long[] Length)
        {
            long[] ret = new long[Length.Length];
            long Address = BaseAddress;
            for (int n = 0, loopTo = Length.Length - 1; n <= loopTo; n++)
            {
                if (Length[n] == 0L)
                {
                    ret[n] = 0L;
                }
                else
                {
                    ret[n] = Address;
                    Address += Length[n];
                }
            }
            return ret;
        }

        /// <summary>已重载。得到地址列的对应的长度，地址列可以乱序，但必须完备，用Length参数放在Address最后来凑齐，Address为0表示长度为0</summary>
        public static short[] GetAddressDifferenceUnordered(short[] Address, short Length)
        {
            Address = (short[])Address.Clone();
            short[] ret = new short[Address.Length];
            int[] Index = new int[Address.Length];
            for (int n = 0, loopTo = Address.Length - 1; n <= loopTo; n++)
                Index[n] = n;
            Array.Sort(Address, Index);
            short Upper = Length;
            for (int n = Address.Length - 1; n >= 0; n -= 1)
            {
                if (Address[n] == 0)
                {
                    ret[n] = 0;
                }
                else
                {
                    ret[n] = (short)(Upper - Address[n]);
                    Upper = Address[n];
                }
            }
            Array.Sort(Index, ret);
            return ret;
        }
        /// <summary>已重载。得到地址列的对应的长度，地址列可以乱序，但必须完备，用Length参数放在Address最后来凑齐，Address为0表示长度为0</summary>
        public static ushort[] GetAddressDifferenceUnordered(ushort[] Address, ushort Length)
        {
            Address = (ushort[])Address.Clone();
            ushort[] ret = new ushort[Address.Length];
            int[] Index = new int[Address.Length];
            for (int n = 0, loopTo = Address.Length - 1; n <= loopTo; n++)
                Index[n] = n;
            Array.Sort(Address, Index);
            ushort Upper = Length;
            for (int n = Address.Length - 1; n >= 0; n -= 1)
            {
                if (Address[n] == 0)
                {
                    ret[n] = 0;
                }
                else
                {
                    ret[n] = (ushort)(Upper - Address[n]);
                    Upper = Address[n];
                }
            }
            Array.Sort(Index, ret);
            return ret;
        }
        /// <summary>已重载。得到地址列的对应的长度，地址列可以乱序，但必须完备，用Length参数放在Address最后来凑齐，Address为0表示长度为0</summary>
        public static int[] GetAddressDifferenceUnordered(int[] Address, int Length)
        {
            Address = (int[])Address.Clone();
            int[] ret = new int[Address.Length];
            int[] Index = new int[Address.Length];
            for (int n = 0, loopTo = Address.Length - 1; n <= loopTo; n++)
                Index[n] = n;
            Array.Sort(Address, Index);
            int Upper = Length;
            for (int n = Address.Length - 1; n >= 0; n -= 1)
            {
                if (Address[n] == 0)
                {
                    ret[n] = 0;
                }
                else
                {
                    ret[n] = Upper - Address[n];
                    Upper = Address[n];
                }
            }
            Array.Sort(Index, ret);
            return ret;
        }
        /// <summary>已重载。得到地址列的对应的长度，地址列可以乱序，但必须完备，用Length参数放在Address最后来凑齐，Address为0表示长度为0</summary>
        public static long[] GetAddressDifferenceUnordered(long[] Address, long Length)
        {
            Address = (long[])Address.Clone();
            long[] ret = new long[Address.Length];
            int[] Index = new int[Address.Length];
            for (int n = 0, loopTo = Address.Length - 1; n <= loopTo; n++)
                Index[n] = n;
            Array.Sort(Address, Index);
            long Upper = Length;
            for (int n = Address.Length - 1; n >= 0; n -= 1)
            {
                if (Address[n] == 0L)
                {
                    ret[n] = 0L;
                }
                else
                {
                    ret[n] = Upper - Address[n];
                    Upper = Address[n];
                }
            }
            Array.Sort(Index, ret);
            return ret;
        }
    }
}