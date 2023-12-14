// ==========================================================================
// 
// File:        DirectIntConvert.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 直接整数转换
// Version:     2009.03.29.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using Microsoft.VisualBasic.CompilerServices;

namespace Firefly
{

    /// <summary>
/// 直接整数转换
/// </summary>
/// <remarks></remarks>
    public static class DirectIntConvert
    {

        /// <summary>Int32->Int16</summary>
        public static short CID(int i)
        {
            if (Conversions.ToBoolean(i & 0x8000))
            {
                return (short)(i & 0xFFFF | int.MinValue + 0x7FFF0000);
            }
            else
            {
                return (short)i;
            }
        }
        /// <summary>Int64->Int32</summary>
        public static int CID(long i)
        {
            if (Conversions.ToBoolean(i & int.MinValue + 0x00000000))
            {
                return (int)(i & int.MinValue + 0x7FFFFFFF | int.MinValue + 0x00000000);
            }
            else
            {
                return (int)i;
            }
        }
        /// <summary>Int16->Int32</summary>
        public static int EID(short i)
        {
            if (Conversions.ToBoolean(i & 0x8000))
            {
                return i & 0x7FFF | 0x8000;
            }
            else
            {
                return i;
            }
        }
        /// <summary>Int32->Int64</summary>
        public static long EID(int i)
        {
            if (Conversions.ToBoolean(i & int.MinValue + 0x00000000))
            {
                return (long)(i & 0x7FFFFFFF) | int.MinValue + 0x00000000;
            }
            else
            {
                return i;
            }
        }
        /// <summary>SByte->Byte</summary>
        public static byte CSU(sbyte i)
        {
            if (Conversions.ToBoolean(i & 0x80))
            {
                return (byte)((byte)(i & 0x7F) | 0x80);
            }
            else
            {
                return (byte)i;
            }
        }
        /// <summary>Int16->UInt16</summary>
        public static ushort CSU(short i)
        {
            if (Conversions.ToBoolean(i & 0x8000))
            {
                return (ushort)((ushort)(i & 0x7FFF) | 0x8000);
            }
            else
            {
                return (ushort)i;
            }
        }
        /// <summary>Int32->UInt32</summary>
        public static uint CSU(int i)
        {
            if (Conversions.ToBoolean(i & int.MinValue + 0x00000000))
            {
                return (uint)((uint)(i & 0x7FFFFFFF) | int.MinValue + 0x00000000);
            }
            else
            {
                return (uint)i;
            }
        }
        /// <summary>Int64->UInt64</summary>
        public static ulong CSU(long i)
        {
            if (Conversions.ToBoolean(i & long.MinValue + 0x00000000))
            {
                return (ulong)((i & 0x7FFFFFFFFFFFFFFF) | long.MinValue + 0x00000000);
            }
            else
            {
                return (ulong)i;
            }
        }
        /// <summary>Byte->SByte</summary>
        public static sbyte CUS(byte i)
        {
            if (Conversions.ToBoolean(i & 0x80))
            {
                return (sbyte)((sbyte)(i & 0x7F) | int.MinValue + 0x7FFFFF80);
            }
            else
            {
                return (sbyte)i;
            }
        }
        /// <summary>UInt16->Int16</summary>
        public static short CUS(ushort i)
        {
            if (Conversions.ToBoolean(i & 0x8000))
            {
                return (short)((short)(i & 0x7FFF) | 0x8000);
            }
            else
            {
                return (short)i;
            }
        }
        /// <summary>UInt32->Int32</summary>
        public static int CUS(uint i)
        {
            if (Conversions.ToBoolean(i & int.MinValue + 0x00000000))
            {
                return (int)(i & 0x7FFFFFFFU) | int.MinValue + 0x00000000;
            }
            else
            {
                return (int)i;
            }
        }
        /// <summary>UInt64->Int64</summary>
        public static long CUS(ulong i)
        {
            if (Conversions.ToBoolean(i & ulong.MinValue + 0x00000000))
            {
                return (long)((ulong)(i & 0x7FFFFFFFU) | ulong.MinValue + 0x00000000);
            }
            else
            {
                return (long)i;
            }
        }
    }
}