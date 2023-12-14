// ==========================================================================
// 
// File:        BitOperations.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 位与32位整数转换
// Version:     2009.10.11.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using Microsoft.VisualBasic.CompilerServices;

namespace Firefly
{

    /// <summary>位操作</summary>
    public static class BitOperations
    {

        /// <summary>安全的左移位操作，原来的操作由于Intel的原因，会自动对移的位数模8，导致左移8位错为左移0位。</summary>
    /// <remarks></remarks>
        public static byte SHL(this byte This, int n)
        {
            if (n >= 8)
                return 0;
            if (n < 0)
                return This.SHR(-n);
            return (byte)(This << n);
        }

        /// <summary>安全的右移位操作，原来的操作由于Intel的原因，会自动对移的位数模8，导致右移8位错为右移0位。</summary>
    /// <remarks></remarks>
        public static byte SHR(this byte This, int n)
        {
            if (n >= 8)
                return 0;
            if (n < 0)
                return This.SHL(-n);
            return (byte)(This >> n);
        }

        /// <summary>安全的左移位操作，原来的操作由于Intel的原因，会自动对移的位数模16，导致左移16位错为左移0位。</summary>
    /// <remarks></remarks>
        public static ushort SHL(this ushort This, int n)
        {
            if (n >= 16)
                return 0;
            if (n < 0)
                return This.SHR(-n);
            return (ushort)(This << n);
        }

        /// <summary>安全的右移位操作，原来的操作由于Intel的原因，会自动对移的位数模16，导致右移16位错为右移0位。</summary>
    /// <remarks></remarks>
        public static ushort SHR(this ushort This, int n)
        {
            if (n >= 16)
                return 0;
            if (n < 0)
                return This.SHL(-n);
            return (ushort)(This >> n);
        }

        /// <summary>安全的左移位操作，原来的操作由于Intel的原因，会自动对移的位数模32，导致左移32位错为左移0位。</summary>
    /// <remarks></remarks>
        public static uint SHL(this uint This, int n)
        {
            if (n >= 32)
                return 0U;
            if (n < 0)
                return This.SHR(-n);
            return This << n;
        }

        /// <summary>安全的右移位操作，原来的操作由于Intel的原因，会自动对移的位数模32，导致右移32位错为右移0位。</summary>
    /// <remarks></remarks>
        public static uint SHR(this uint This, int n)
        {
            if (n >= 32)
                return 0U;
            if (n < 0)
                return This.SHL(-n);
            return This >> n;
        }

        /// <summary>安全的左移位操作，原来的操作由于Intel的原因，会自动对移的位数模64，导致左移64位错为左移0位。</summary>
    /// <remarks></remarks>
        public static ulong SHL(this ulong This, int n)
        {
            if (n >= 64)
                return 0UL;
            if (n < 0)
                return This.SHR(-n);
            return This << n;
        }

        /// <summary>安全的右移位操作，原来的操作由于Intel的原因，会自动对移的位数模64，导致右移64位错为右移0位。</summary>
    /// <remarks></remarks>
        public static ulong SHR(this ulong This, int n)
        {
            if (n >= 64)
                return 0UL;
            if (n < 0)
                return This.SHL(-n);
            return This >> n;
        }

        /// <summary>安全的左移位操作，原来的操作由于Intel的原因，会自动对移的位数模8，导致左移8位错为左移0位。</summary>
    /// <remarks></remarks>
        public static sbyte SAL(this sbyte This, int n)
        {
            if (n >= 8)
                return 0;
            if (n < 0)
                return This.SAR(-n);
            return (sbyte)(This << n);
        }

        /// <summary>安全的右移位操作，原来的操作由于Intel的原因，会自动对移的位数模8，导致右移8位错为右移0位。</summary>
    /// <remarks></remarks>
        public static sbyte SAR(this sbyte This, int n)
        {
            if (n >= 8)
            {
                if (Conversions.ToBoolean(This & 0x80))
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
            if (n < 0)
                return This.SAL(-n);
            return (sbyte)(This >> n);
        }

        /// <summary>安全的左移位操作，原来的操作由于Intel的原因，会自动对移的位数模16，导致左移16位错为左移0位。</summary>
    /// <remarks></remarks>
        public static short SAL(this short This, int n)
        {
            if (n >= 16)
                return 0;
            if (n < 0)
                return This.SAR(-n);
            return (short)(This << n);
        }

        /// <summary>安全的右移位操作，原来的操作由于Intel的原因，会自动对移的位数模16，导致右移16位错为右移0位。</summary>
    /// <remarks></remarks>
        public static short SAR(this short This, int n)
        {
            if (n >= 16)
            {
                if (Conversions.ToBoolean(This & 0x8000))
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
            if (n < 0)
                return This.SAL(-n);
            return (short)(This >> n);
        }

        /// <summary>安全的左移位操作，原来的操作由于Intel的原因，会自动对移的位数模32，导致左移32位错为左移0位。</summary>
    /// <remarks></remarks>
        public static int SAL(this int This, int n)
        {
            if (n >= 32)
                return 0;
            if (n < 0)
                return This.SAR(-n);
            return This << n;
        }

        /// <summary>安全的右移位操作，原来的操作由于Intel的原因，会自动对移的位数模32，导致右移32位错为右移0位。</summary>
    /// <remarks></remarks>
        public static int SAR(this int This, int n)
        {
            if (n >= 32)
            {
                if (Conversions.ToBoolean(This & int.MinValue + 0x00000000))
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
            if (n < 0)
                return This.SAL(-n);
            return This >> n;
        }

        /// <summary>安全的左移位操作，原来的操作由于Intel的原因，会自动对移的位数模64，导致左移64位错为左移0位。</summary>
    /// <remarks></remarks>
        public static long SAL(this long This, int n)
        {
            if (n >= 64)
                return 0L;
            if (n < 0)
                return This.SAR(-n);
            return This << n;
        }

        /// <summary>安全的右移位操作，原来的操作由于Intel的原因，会自动对移的位数模64，导致右移64位错为右移0位。</summary>
    /// <remarks></remarks>
        public static long SAR(this long This, int n)
        {
            if (n >= 64)
            {
                if (Conversions.ToBoolean(This & long.MinValue))
                {
                    return -1;
                }
                else
                {
                    return 0L;
                }
            }
            if (n < 0)
                return This.SAL(-n);
            return This >> n;
        }


        /// <summary>
    /// 获得整数的特定位。
    /// </summary>
    /// <param name="This">Byte</param>
    /// <param name="U">高位索引(7-0)</param>
    /// <param name="L">低位索引(7-0)</param>
        public static byte Bits(this byte This, int U, int L)
        {
            int NumBits = U - L + 1;
            byte Mask;
            if (NumBits <= 0)
            {
                Mask = 0;
            }
            else if (NumBits >= 8)
            {
                Mask = byte.MaxValue;
            }
            else
            {
                Mask = (byte)(((byte)1).SHL(NumBits) - 1);
            }
            return (byte)(This.SHR(L) & Mask);
        }

        /// <summary>
    /// 获得整数的特定位。
    /// </summary>
    /// <param name="This">UInt16</param>
    /// <param name="U">高位索引(15-0)</param>
    /// <param name="L">低位索引(15-0)</param>
        public static ushort Bits(this ushort This, int U, int L)
        {
            int NumBits = U - L + 1;
            ushort Mask;
            if (NumBits <= 0)
            {
                Mask = 0;
            }
            else if (NumBits >= 16)
            {
                Mask = ushort.MaxValue;
            }
            else
            {
                Mask = (ushort)(((byte)1).SHL(NumBits) - (ushort)1);
            }
            return (ushort)(This.SHR(L) & Mask);
        }

        /// <summary>
    /// 获得整数的特定位。
    /// </summary>
    /// <param name="This">Int32</param>
    /// <param name="U">高位索引(31-0)</param>
    /// <param name="L">低位索引(31-0)</param>
        public static uint Bits(this uint This, int U, int L)
        {
            int NumBits = U - L + 1;
            uint Mask;
            if (NumBits <= 0)
            {
                Mask = 0U;
            }
            else if (NumBits >= 32)
            {
                Mask = uint.MaxValue;
            }
            else
            {
                Mask = 1U.SHL(NumBits) - 1U;
            }
            return This.SHR(L) & Mask;
        }

        /// <summary>
    /// 获得整数的特定位。
    /// </summary>
    /// <param name="This">Int32</param>
    /// <param name="U">高位索引(63-0)</param>
    /// <param name="L">低位索引(63-0)</param>
        public static ulong Bits(this ulong This, int U, int L)
        {
            int NumBits = U - L + 1;
            ulong Mask;
            if (NumBits <= 0)
            {
                Mask = 0UL;
            }
            else if (NumBits >= 64)
            {
                Mask = ulong.MaxValue;
            }
            else
            {
                Mask = 1UL.SHL(NumBits) - 1UL;
            }
            return This.SHR(L) & Mask;
        }

        /// <summary>
    /// 获得整数的特定位。
    /// </summary>
    /// <param name="This">Byte</param>
    /// <param name="U">高位索引(7-0)</param>
    /// <param name="L">低位索引(7-0)</param>
        public static sbyte Bits(this sbyte This, int U, int L)
        {
            return DirectIntConvert.CUS(DirectIntConvert.CSU(This).Bits(U, L));
        }

        /// <summary>
    /// 获得整数的特定位。
    /// </summary>
    /// <param name="This">UInt16</param>
    /// <param name="U">高位索引(15-0)</param>
    /// <param name="L">低位索引(15-0)</param>
        public static short Bits(this short This, int U, int L)
        {
            return DirectIntConvert.CUS(DirectIntConvert.CSU(This).Bits(U, L));
        }

        /// <summary>
    /// 获得整数的特定位。
    /// </summary>
    /// <param name="This">Int32</param>
    /// <param name="U">高位索引(31-0)</param>
    /// <param name="L">低位索引(31-0)</param>
        public static int Bits(this int This, int U, int L)
        {
            return DirectIntConvert.CUS(DirectIntConvert.CSU(This).Bits(U, L));
        }

        /// <summary>
    /// 获得整数的特定位。
    /// </summary>
    /// <param name="This">Int32</param>
    /// <param name="U">高位索引(63-0)</param>
    /// <param name="L">低位索引(63-0)</param>
        public static long Bits(this long This, int U, int L)
        {
            return DirectIntConvert.CUS(DirectIntConvert.CSU(This).Bits(U, L));
        }


        /// <summary>
    /// 已重载。从位构成整数。
    /// </summary>
    /// <param name="H">首字节</param>
    /// <param name="HU">首字节高位索引(7-0)</param>
    /// <param name="HL">首字节低位索引(7-0)</param>
    /// <param name="S">次字节</param>
    /// <param name="SU">次字节高位索引(7-0)</param>
    /// <param name="SL">次字节低位索引(7-0)</param>
    /// <param name="T">第三字节</param>
    /// <param name="TU">第三字节高位索引(7-0)</param>
    /// <param name="TL">第三字节低位索引(7-0)</param>
    /// <param name="Q">第四字节</param>
    /// <param name="QU">第四字节高位索引(7-0)</param>
    /// <param name="QL">第四字节低位索引(7-0)</param>
    /// <returns>由这些字节的这些位依次从高到低连接得到的整数。</returns>
        public static int ComposeBits(byte H, int HU, int HL, byte S, int SU, int SL, byte T, int TU, int TL, byte Q, int QU, int QL)
        {
            int HPart = H.Bits(HU, HL);
            int SPart = S.Bits(SU, SL);
            int TPart = T.Bits(TU, TL);
            int QPart = Q.Bits(QU, QL);
            return HPart.SAL(SU - SL + 1 + TU - TL + 1 + QU - QL + 1) | SPart.SAL(TU - TL + 1 + QU - QL + 1) | TPart.SAL(QU - QL + 1) | QPart;
        }
        /// <summary>
    /// 已重载。从位构成整数。
    /// </summary>
    /// <param name="H">首字节</param>
    /// <param name="HU">首字节高位索引(7-0)</param>
    /// <param name="HL">首字节低位索引(7-0)</param>
    /// <param name="S">次字节</param>
    /// <param name="SU">次字节高位索引(7-0)</param>
    /// <param name="SL">次字节低位索引(7-0)</param>
    /// <param name="T">第三字节</param>
    /// <param name="TU">第三字节高位索引(7-0)</param>
    /// <param name="TL">第三字节低位索引(7-0)</param>
    /// <returns>由这些字节的这些位依次从高到低连接得到的整数。</returns>
        public static int ComposeBits(byte H, int HU, int HL, byte S, int SU, int SL, byte T, int TU, int TL)
        {
            int HPart = H.Bits(HU, HL);
            int SPart = S.Bits(SU, SL);
            int TPart = T.Bits(TU, TL);
            return HPart.SAL(SU - SL + 1 + TU - TL + 1) | SPart.SAL(TU - TL + 1) | TPart;
        }
        /// <summary>
    /// 已重载。从位构成整数。
    /// </summary>
    /// <param name="H">首字节</param>
    /// <param name="HU">首字节高位索引(7-0)</param>
    /// <param name="HL">首字节低位索引(7-0)</param>
    /// <param name="S">次字节</param>
    /// <param name="SU">次字节高位索引(7-0)</param>
    /// <param name="SL">次字节低位索引(7-0)</param>
    /// <returns>由这些字节的这些位依次从高到低连接得到的整数。</returns>
        public static int ComposeBits(byte H, int HU, int HL, byte S, int SU, int SL)
        {
            int HPart = H.Bits(HU, HL);
            int SPart = S.Bits(SU, SL);
            return HPart.SAL(SU - SL + 1) | SPart;
        }
        /// <summary>
    /// 已重载。从位构成整数。
    /// </summary>
    /// <param name="H">首字节</param>
    /// <param name="HU">首字节高位索引(7-0)</param>
    /// <param name="HL">首字节低位索引(7-0)</param>
    /// <returns>由这些字节的这些位依次从高到低连接得到的整数。</returns>
        public static int ComposeBits(byte H, int HU, int HL)
        {
            return H.Bits(HU, HL);
        }
        /// <summary>
    /// 已重载。从位构成整数。
    /// </summary>
    /// <param name="H">首Int32</param>
    /// <param name="HU">首Int32高位索引(31-0)</param>
    /// <param name="HL">首Int32低位索引(31-0)</param>
    /// <param name="S">次Int32</param>
    /// <param name="SU">次Int32高位索引(31-0)</param>
    /// <param name="SL">次Int32低位索引(31-0)</param>
    /// <returns>由这些Int32的这些位依次从高到低连接得到的整数。</returns>
        public static int ComposeBits(int H, int HU, int HL, int S, int SU, int SL)
        {
            int HPart = H.Bits(HU, HL);
            int SPart = S.Bits(SU, SL);
            return HPart.SAL(SU - SL + 1) | SPart;
        }
        /// <summary>
    /// 已重载。从位构成整数。
    /// </summary>
    /// <param name="H">首Int32</param>
    /// <param name="HU">首Int32高位索引(31-0)</param>
    /// <param name="HL">首Int32低位索引(31-0)</param>
    /// <returns>由这些Int32的这些位依次从高到低连接得到的整数。</returns>
        public static int ComposeBits(int H, int HU, int HL)
        {
            return H.Bits(HU, HL);
        }

        /// <summary>
    /// 已重载。将整数分解到位。
    /// </summary>
    /// <param name="H">首字节</param>
    /// <param name="HU">首字节高位索引(7-0)</param>
    /// <param name="HL">首字节低位索引(7-0)</param>
    /// <param name="S">次字节</param>
    /// <param name="SU">次字节高位索引(7-0)</param>
    /// <param name="SL">次字节低位索引(7-0)</param>
    /// <param name="T">第三字节</param>
    /// <param name="TU">第三字节高位索引(7-0)</param>
    /// <param name="TL">第三字节低位索引(7-0)</param>
    /// <param name="Q">第四字节</param>
    /// <param name="QU">第四字节高位索引(7-0)</param>
    /// <param name="QL">第四字节低位索引(7-0)</param>
    /// <param name="Value">待分解的整数。</param>
        public static void DecomposeBits(ref byte H, int HU, int HL, ref byte S, int SU, int SL, ref byte T, int TU, int TL, ref byte Q, int QU, int QL, int Value)
        {
            int HPart = Value.SAR(SU - SL + 1 + TU - TL + 1 + QU - QL + 1) & 1.SAL(HU - HL + 1) - 1;
            H = (byte)(H & ~(byte)(1.SAL(HU - HL + 1) - 1).SAL(HL));
            H = (byte)(H | (byte)HPart.SAL(HL));
            int SPart = Value.SAR(TU - TL + 1 + QU - QL + 1) & 1.SAL(SU - SL + 1) - 1;
            S = (byte)(S & ~(byte)(1.SAL(SU - SL + 1) - 1).SAL(SL));
            S = (byte)(S | (byte)SPart.SAL(SL));
            int TPart = Value.SAR(QU - QL + 1) & 1.SAL(TU - TL + 1) - 1;
            T = (byte)(T & ~(byte)(1.SAL(TU - TL + 1) - 1).SAL(TL));
            T = (byte)(T | (byte)TPart.SAL(TL));
            int QPart = Value & 1.SAL(QU - QL + 1) - 1;
            Q = (byte)(Q & ~(byte)(1.SAL(QU - QL + 1) - 1).SAL(QL));
            Q = (byte)(Q | (byte)QPart.SAL(QL));
        }
        /// <summary>
    /// 已重载。将整数分解到位。
    /// </summary>
    /// <param name="H">首字节</param>
    /// <param name="HU">首字节高位索引(7-0)</param>
    /// <param name="HL">首字节低位索引(7-0)</param>
    /// <param name="S">次字节</param>
    /// <param name="SU">次字节高位索引(7-0)</param>
    /// <param name="SL">次字节低位索引(7-0)</param>
    /// <param name="T">第三字节</param>
    /// <param name="TU">第三字节高位索引(7-0)</param>
    /// <param name="TL">第三字节低位索引(7-0)</param>
    /// <param name="Value">待分解的整数。</param>
        public static void DecomposeBits(ref byte H, int HU, int HL, ref byte S, int SU, int SL, ref byte T, int TU, int TL, int Value)
        {
            int HPart = Value.SAR(SU - SL + 1 + TU - TL + 1) & 1.SAL(HU - HL + 1) - 1;
            H = (byte)(H & ~(byte)(1.SAL(HU - HL + 1) - 1).SAL(HL));
            H = (byte)(H | (byte)HPart.SAL(HL));
            int SPart = Value.SAR(TU - TL + 1) & 1.SAL(SU - SL + 1) - 1;
            S = (byte)(S & ~(byte)(1.SAL(SU - SL + 1) - 1).SAL(SL));
            S = (byte)(S | (byte)SPart.SAL(SL));
            int TPart = Value & 1.SAL(TU - TL + 1) - 1;
            T = (byte)(T & ~(byte)(1.SAL(TU - TL + 1) - 1).SAL(TL));
            T = (byte)(T | (byte)TPart.SAL(TL));
        }
        /// <summary>
    /// 已重载。将整数分解到位。
    /// </summary>
    /// <param name="H">首字节</param>
    /// <param name="HU">首字节高位索引(7-0)</param>
    /// <param name="HL">首字节低位索引(7-0)</param>
    /// <param name="S">次字节</param>
    /// <param name="SU">次字节高位索引(7-0)</param>
    /// <param name="SL">次字节低位索引(7-0)</param>
    /// <param name="Value">待分解的整数。</param>
        public static void DecomposeBits(ref byte H, int HU, int HL, ref byte S, int SU, int SL, int Value)
        {
            int HPart = Value.SAR(SU - SL + 1) & 1.SAL(HU - HL + 1) - 1;
            H = (byte)(H & ~(byte)(1.SAL(HU - HL + 1) - 1).SAL(HL));
            H = (byte)(H | (byte)HPart.SAL(HL));
            int SPart = Value & 1.SAL(SU - SL + 1) - 1;
            S = (byte)(S & ~(byte)(1.SAL(SU - SL + 1) - 1).SAL(SL));
            S = (byte)(S | (byte)SPart.SAL(SL));
        }
        /// <summary>
    /// 已重载。将整数分解到位。
    /// </summary>
    /// <param name="H">首字节</param>
    /// <param name="HU">首字节高位索引(7-0)</param>
    /// <param name="HL">首字节低位索引(7-0)</param>
    /// <param name="Value">待分解的整数。</param>
        public static void DecomposeBits(ref byte H, int HU, int HL, int Value)
        {
            int HPart = Value & 1.SAL(HU - HL + 1) - 1;
            H = (byte)(H & ~(byte)(1.SAL(HU - HL + 1) - 1).SAL(HL));
            H = (byte)(H | (byte)HPart.SAL(HL));
        }
        /// <summary>
    /// 已重载。将整数分解到位。
    /// </summary>
    /// <param name="H">首Int32</param>
    /// <param name="HU">首Int32高位索引(31-0)</param>
    /// <param name="HL">首Int32低位索引(31-0)</param>
    /// <param name="S">次Int32</param>
    /// <param name="SU">次Int32高位索引(31-0)</param>
    /// <param name="SL">次Int32低位索引(31-0)</param>
    /// <param name="Value">待分解的整数。</param>
        public static void DecomposeBits(ref int H, int HU, int HL, ref int S, int SU, int SL, int Value)
        {
            int HPart = Value.SAR(SU - SL + 1) & 1.SAL(HU - HL + 1) - 1;
            H = H & ~(1.SAL(HU - HL + 1) - 1).SAL(HL);
            H = H | HPart.SAL(HL);
            int SPart = Value & 1.SAL(SU - SL + 1) - 1;
            S = S & ~(1.SAL(SU - SL + 1) - 1).SAL(SL);
            S = S | SPart.SAL(SL);
        }
        /// <summary>
    /// 已重载。将整数分解到位。
    /// </summary>
    /// <param name="H">首Int32</param>
    /// <param name="HU">首Int32高位索引(31-0)</param>
    /// <param name="HL">首Int32低位索引(31-0)</param>
    /// <param name="Value">待分解的整数。</param>
        public static void DecomposeBits(ref int H, int HU, int HL, int Value)
        {
            int HPart = Value & 1.SAL(HU - HL + 1) - 1;
            H = H & ~(1.SAL(HU - HL + 1) - 1).SAL(HL);
            H = H | HPart.SAL(HL);
        }


        /// <summary>
    /// 已重载。从位连接整数。
    /// </summary>
    /// <param name="H">首字节</param>
    /// <param name="HW">首字节宽度(8-0)</param>
    /// <param name="S">次字节</param>
    /// <param name="SW">次字节宽度(8-0)</param>
    /// <param name="T">第三字节</param>
    /// <param name="TW">第三字节宽度(8-0)</param>
    /// <param name="Q">第四字节</param>
    /// <param name="QW">第四字节宽度(8-0)</param>
    /// <returns>由这些字节的这些位依次从高到低连接得到的整数。</returns>
        public static int ConcatBits(byte H, int HW, byte S, int SW, byte T, int TW, byte Q, int QW)
        {
            int HPart = H.Bits(HW - 1, 0);
            int SPart = S.Bits(SW - 1, 0);
            int TPart = T.Bits(TW - 1, 0);
            int QPart = Q.Bits(QW - 1, 0);
            return HPart.SAL(SW + TW + QW) | SPart.SAL(TW + QW) | TPart.SAL(QW) | QPart;
        }
        /// <summary>
    /// 已重载。从位连接整数。
    /// </summary>
    /// <param name="H">首字节</param>
    /// <param name="HW">首字节宽度(8-0)</param>
    /// <param name="S">次字节</param>
    /// <param name="SW">次字节宽度(8-0)</param>
    /// <param name="T">第三字节</param>
    /// <param name="TW">第三字节宽度(8-0)</param>
    /// <returns>由这些字节的这些位依次从高到低连接得到的整数。</returns>
        public static int ConcatBits(byte H, int HW, byte S, int SW, byte T, int TW)
        {
            int HPart = H.Bits(HW - 1, 0);
            int SPart = S.Bits(SW - 1, 0);
            int TPart = T.Bits(TW - 1, 0);
            return HPart.SAL(SW + TW) | SPart.SAL(TW) | TPart;
        }
        /// <summary>
    /// 已重载。从位连接整数。
    /// </summary>
    /// <param name="H">首字节</param>
    /// <param name="HW">首字节宽度(8-0)</param>
    /// <param name="S">次字节</param>
    /// <param name="SW">次字节宽度(8-0)</param>
    /// <returns>由这些字节的这些位依次从高到低连接得到的整数。</returns>
        public static int ConcatBits(byte H, int HW, byte S, int SW)
        {
            int HPart = H.Bits(HW - 1, 0);
            int SPart = S.Bits(SW - 1, 0);
            return HPart.SAL(SW) | SPart;
        }
        /// <summary>
    /// 已重载。从位连接整数。
    /// </summary>
    /// <param name="H">首Int32</param>
    /// <param name="HW">首Int32宽度(32-0)</param>
    /// <param name="S">次Int32</param>
    /// <param name="SW">次Int32宽度(32-0)</param>
    /// <returns>由这些Int32的这些位依次从高到低连接得到的整数。</returns>
        public static int ConcatBits(int H, int HW, int S, int SW)
        {
            int HPart = H.Bits(HW - 1, 0);
            int SPart = S.Bits(SW - 1, 0);
            return HPart.SAL(SW) | SPart;
        }

        /// <summary>
    /// 已重载。将整数拆分到位。
    /// </summary>
    /// <param name="H">首字节</param>
    /// <param name="HW">首字节宽度(8-0)</param>
    /// <param name="S">次字节</param>
    /// <param name="SW">次字节宽度(8-0)</param>
    /// <param name="T">第三字节</param>
    /// <param name="TW">第三字节宽度(8-0)</param>
    /// <param name="Q">第四字节</param>
    /// <param name="QW">第四字节宽度(8-0)</param>
    /// <param name="Value">待拆分的整数。</param>
        public static void SplitBits(ref byte H, int HW, ref byte S, int SW, ref byte T, int TW, ref byte Q, int QW, int Value)
        {
            H = (byte)(Value.SAR(SW + TW + QW) & 1.SAL(HW) - 1);
            S = (byte)(Value.SAR(TW + QW) & 1.SAL(SW) - 1);
            T = (byte)(Value.SAR(QW) & 1.SAL(TW) - 1);
            Q = (byte)(Value & 1.SAL(QW) - 1);
        }
        /// <summary>
    /// 已重载。将整数拆分到位。
    /// </summary>
    /// <param name="H">首字节</param>
    /// <param name="HW">首字节宽度(8-0)</param>
    /// <param name="S">次字节</param>
    /// <param name="SW">次字节宽度(8-0)</param>
    /// <param name="T">第三字节</param>
    /// <param name="TW">第三字节宽度(8-0)</param>
    /// <param name="Value">待拆分的整数。</param>
        public static void SplitBits(ref byte H, int HW, ref byte S, int SW, ref byte T, int TW, int Value)
        {
            H = (byte)(Value.SAR(SW + TW) & 1.SAL(HW) - 1);
            S = (byte)(Value.SAR(TW) & 1.SAL(SW) - 1);
            T = (byte)(Value & 1.SAL(TW) - 1);
        }
        /// <summary>
    /// 已重载。将整数拆分到位。
    /// </summary>
    /// <param name="H">首字节</param>
    /// <param name="HW">首字节宽度(8-0)</param>
    /// <param name="S">次字节</param>
    /// <param name="SW">次字节宽度(8-0)</param>
    /// <param name="Value">待拆分的整数。</param>
        public static void SplitBits(ref byte H, int HW, ref byte S, int SW, int Value)
        {
            H = (byte)(Value.SAR(SW) & 1.SAL(HW) - 1);
            S = (byte)(Value & 1.SAL(SW) - 1);
        }
        /// <summary>
    /// 已重载。将整数拆分到位。
    /// </summary>
    /// <param name="H">首Int32</param>
    /// <param name="HW">首Int32宽度(32-0)</param>
    /// <param name="S">次Int32</param>
    /// <param name="SW">次Int32宽度(32-0)</param>
    /// <param name="Value">待拆分的整数。</param>
        public static void SplitBits(ref int H, int HW, ref int S, int SW, int Value)
        {
            H = Value.SAR(SW) & 1.SAL(HW) - 1;
            S = Value & 1.SAL(SW) - 1;
        }
    }
}