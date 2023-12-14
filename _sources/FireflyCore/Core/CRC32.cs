// ==========================================================================
// 
// File:        CRC32.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: CRC32计算
// Version:     2008.10.31.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using Microsoft.VisualBasic.CompilerServices;

namespace Firefly
{

    /// <summary>CRC32类</summary>
/// <remarks>
/// 按照IEEE-802标准，参考RFC3385。
/// </remarks>
    public class CRC32
    {
        private int[] Table;
        private int Result;

        public void Reset()
        {
            Result = int.MinValue + 0x7FFFFFFF;
        }
        public void PushData(byte b)
        {
            int iLookup = Result & 0xFF ^ b;
            Result = Result >> 8 & 0xFFFFFF;
            Result = Result ^ Table[iLookup];
        }
        public int GetCRC32()
        {
            return ~Result;
        }

        public CRC32()
        {
            // g(x) = x^32 + x^26 + x^23 + x^22 + x^16 + x^12 + x^11 + x^10 + x^8 + x^7 + x^5 + x^4 + x^2 + x + 1
            // 多项式系数的位数组表示104C11DB7
            int Coefficients = int.MinValue + 0x6DB88320; // 反向表示

            Table = new int[256];

            for (int i = 0; i <= 255; i++)
            {
                int CRC = i;
                for (int j = 0; j <= 7; j++)
                {
                    if (Conversions.ToBoolean(CRC & 1))
                    {
                        CRC = CRC >> 1 & 0x7FFFFFFF;
                        CRC = CRC ^ Coefficients;
                    }
                    else
                    {
                        CRC = CRC >> 1 & 0x7FFFFFFF;
                    }
                }
                Table[i] = CRC;
            }

            Reset();
        }
    }
}