// ==========================================================================
// 
// File:        ColorSpace.vb
// Location:    Firefly.Imaging <Visual Basic .Net>
// Description: 颜色空间变换
// Version:     2009.10.31.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using static System.Math;

namespace Firefly.Imaging
{

    /// <summary>
    /// 各颜色空间中点的互相转换。
    /// </summary>
    public static class ColorSpace
    {
        public static int YCbCr2RGB(byte Y, byte Cb, byte Cr)
        {
            int R, G, B;

            R = (int)Round(Y + 1.402d * (Cr - 128));
            G = (int)Round(Y - 0.34414d * (Cb - 128) - 0.71414d * (Cr - 128));
            B = (int)Round(Y + 1.772d * (Cb - 128));

            // R = (Y * 256 + Cr * 358 - 45875) >> 8
            // G = (Y * 256 - Cb * 87 - Cr * 183 + 34667) >> 8
            // B = (Y * 256 + Cb * 454 - 58129) >> 8

            if (R < 0)
            {
                R = 0;
            }
            else if (R > 0xFF)
            {
                R = 0xFF;
            }
            if (G < 0)
            {
                G = 0;
            }
            else if (G > 0xFF)
            {
                G = 0xFF;
            }
            if (B < 0)
            {
                B = 0;
            }
            else if (B > 0xFF)
            {
                B = 0xFF;
            }

            return int.MinValue + 0x7F000000 | R << 16 | G << 8 | B;
        }
        public static void YCbCr2RGB(byte Y, byte Cb, byte Cr, ref byte R, ref byte G, ref byte B)
        {
            int tR, tG, tB;

            tR = (int)Round(Y + 1.402d * (Cr - 128));
            tG = (int)Round(Y - 0.34414d * (Cb - 128) - 0.71414d * (Cr - 128));
            tB = (int)Round(Y + 1.772d * (Cb - 128));

            if (tR < 0)
            {
                tR = 0;
            }
            else if (tR > 0xFF)
            {
                tR = 0xFF;
            }
            if (tG < 0)
            {
                tG = 0;
            }
            else if (tG > 0xFF)
            {
                tG = 0xFF;
            }
            if (tB < 0)
            {
                tB = 0;
            }
            else if (tB > 0xFF)
            {
                tB = 0xFF;
            }

            R = (byte)tR;
            G = (byte)tG;
            B = (byte)tB;
        }
        public static int RGB2YCbCr(byte R, byte G, byte B)
        {
            int Y, Cb, Cr;

            Y = (int)Round(0.299d * R + 0.587d * G + 0.114d * B);
            Cb = (int)Round(-0.1687d * R - 0.3313d * G + 0.5d * B + 128d);
            Cr = (int)Round(0.5d * R - 0.4187d * G - 0.0813d * B + 128d);
            if (Y < 0)
            {
                Y = 0;
            }
            else if (Y > 255)
            {
                Y = 255;
            }
            if (Cb < 0)
            {
                Cb = 0;
            }
            else if (Cb > 255)
            {
                Cb = 255;
            }
            if (Cr < 0)
            {
                Cr = 0;
            }
            else if (Cr > 255)
            {
                Cr = 255;
            }

            return Y << 16 | Cb << 8 | Cr;
        }
        public static void RGB2YCbCr(byte R, byte G, byte B, ref byte Y, ref byte Cb, ref byte Cr)
        {
            int tY, tCb, tCr;

            tY = (int)Round(0.299d * R + 0.587d * G + 0.114d * B);
            tCb = (int)Round(-0.1687d * R - 0.3313d * G + 0.5d * B + 128d);
            tCr = (int)Round(0.5d * R - 0.4187d * G - 0.0813d * B + 128d);
            if (tY < 0)
            {
                tY = 0;
            }
            else if (tY > 255)
            {
                tY = 255;
            }
            if (tCb < 0)
            {
                tCb = 0;
            }
            else if (tCb > 255)
            {
                tCb = 255;
            }
            if (tCr < 0)
            {
                tCr = 0;
            }
            else if (tCr > 255)
            {
                tCr = 255;
            }

            Y = (byte)tY;
            Cb = (byte)tCb;
            Cr = (byte)tCr;
        }
        public static short RGB32To16(int ARGB)
        {
            return DirectIntConvert.CID((ARGB & 0xF80000) >> 8 | (ARGB & 0xFC00) >> 5 | (ARGB & 0xF8) >> 3);
        }
        public static short RGB32To15(int ARGB)
        {
            return DirectIntConvert.CID((ARGB & 0xF80000) >> 9 | (ARGB & 0xF800) >> 6 | (ARGB & 0xF8) >> 3);
        }
        public static int RGB16To32(short RGB16)
        {
            int r = (DirectIntConvert.EID(RGB16) & 0xF800) >> 8;
            int g = (DirectIntConvert.EID(RGB16) & 0x7E0) >> 3;
            int b = (DirectIntConvert.EID(RGB16) & 0x1F) << 3;
            r = r | r >> 5;
            g = g | g >> 6;
            b = b | b >> 5;
            return int.MinValue + 0x7F000000 | r << 16 | g << 8 | b;
        }
        public static int RGB15To32(short RGB15)
        {
            int r = (DirectIntConvert.EID(RGB15) & 0x7C00) >> 7;
            int g = (DirectIntConvert.EID(RGB15) & 0x3E0) >> 2;
            int b = (DirectIntConvert.EID(RGB15) & 0x1F) << 3;
            r = r | r >> 5;
            g = g | g >> 5;
            b = b | b >> 5;
            return int.MinValue + 0x7F000000 | r << 16 | g << 8 | b;
        }
        public static byte RGB32ToL8(int ARGB)
        {
            int L = (int)Round(0.299d * ((ARGB & 0xFF0000) >> 16) + 0.587d * ((ARGB & 0xFF00) >> 8) + 0.114d * (ARGB & 0xFF));
            if (L < 0)
            {
                L = 0;
            }
            else if (L > 255)
            {
                L = 255;
            }
            return (byte)L;
        }
        public static int L8ToRGB32(byte L)
        {
            int g = L;
            return int.MinValue + 0x7F000000 | g << 16 | g << 8 | g;
        }
        public static void HSL2RGB(double H, double S, double L, ref byte R, ref byte G, ref byte B)
        {
            // H in [0,1)
            // S in [0,1)
            // L in [0,1)
            if (S == 0d)
            {
                R = (byte)Round(L * 255d);
                G = (byte)Round(L * 255d);
                B = (byte)Round(L * 255d);
            }
            else
            {
                double temp1, temp2;
                if (L < 0.5d)
                {
                    temp2 = L * (1d + S);
                }
                else
                {
                    temp2 = L + S - L * S;
                }
                temp1 = 2d * L - temp2;

                R = (byte)Round(255d * Hue2RGB(temp1, temp2, H + 1d / 3d));
                G = (byte)Round(255d * Hue2RGB(temp1, temp2, H));
                B = (byte)Round(255d * Hue2RGB(temp1, temp2, H - 1d / 3d));
            }
        }
        public static double Hue2RGB(double v1, double v2, double vH)
        {
            if (vH < 0d)
                vH += 1d;
            if (vH > 1d)
                vH -= 1d;
            if (6d * vH < 1d)
                return v1 + (v2 - v1) * 6d * vH;
            if (2d * vH < 1d)
                return v2;
            if (3d * vH < 2d)
                return v1 + (v2 - v1) * (2d / 3d - vH) * 6d;
            return v1;
        }
        public static void RGB2HSL(byte R, byte G, byte B, ref double H, ref double S, ref double L)
        {
            double var_R, var_G, var_B, var_Min, var_Max, del_Max, del_R, del_G, del_B;
            var_R = R / 255d;
            var_G = G / 255d;
            var_B = B / 255d;

            var_Min = NumericOperations.Min(var_R, NumericOperations.Min(var_G, var_B));
            var_Max = NumericOperations.Max(var_R, NumericOperations.Max(var_G, var_B));
            del_Max = var_Max - var_Min;

            L = (var_Max + var_Min) / 2d;

            if (del_Max == 0d)
            {
                H = 0d;
                S = 0d;
            }
            else
            {
                if (L < 0.5d)
                {
                    S = del_Max / (var_Max + var_Min);
                }
                else
                {
                    S = del_Max / (2d - var_Max - var_Min);
                }

                del_R = ((var_Max - var_R) / 6d + del_Max / 2d) / del_Max;
                del_G = ((var_Max - var_G) / 6d + del_Max / 2d) / del_Max;
                del_B = ((var_Max - var_B) / 6d + del_Max / 2d) / del_Max;

                if (var_R == var_Max)
                {
                    H = del_B - del_G;
                }
                else if (var_G == var_Max)
                {
                    H = 1d / 3d + del_R - del_B;
                }
                else if (var_B == var_Max)
                {
                    H = 2d / 3d + del_G - del_R;
                }
                if (H < 0d)
                    H += 1d;
                if (H > 1d)
                    H -= 1d;
            }
        }
        public static void HSV2RGB(double H, double S, double V, ref byte R, ref byte G, ref byte B)
        {
            // H in [0,1)
            // S in [0,1)
            // L in [0,1)
            if (S == 0d)
            {
                R = (byte)Round(V * 255d);
                G = (byte)Round(V * 255d);
                B = (byte)Round(V * 255d);
            }
            else
            {
                double var_h, var_i, var_1, var_2, var_3, var_r, var_g, var_b;
                var_h = H * 6d;
                if (var_h == 6d)
                    var_h = 0d;
                var_i = Floor(var_h);
                var_1 = V * (1d - S);
                var_2 = V * (1d - S * (var_h - var_i));
                var_3 = V * (1d - S * (1d - (var_h - var_i)));

                if (var_i == 0d)
                {
                    var_r = V;
                    var_g = var_3;
                    var_b = var_1;
                }
                else if (var_i == 1d)
                {
                    var_r = var_2;
                    var_g = V;
                    var_b = var_1;
                }
                else if (var_i == 2d)
                {
                    var_r = var_1;
                    var_g = V;
                    var_b = var_3;
                }
                else if (var_i == 3d)
                {
                    var_r = var_1;
                    var_g = var_2;
                    var_b = V;
                }
                else if (var_i == 4d)
                {
                    var_r = var_3;
                    var_g = var_1;
                    var_b = V;
                }
                else
                {
                    var_r = V;
                    var_g = var_1;
                    var_b = var_2;
                }

                R = (byte)Round(var_r * 255d);
                G = (byte)Round(var_g * 255d);
                B = (byte)Round(var_b * 255d);
            }
        }
        public static void RGB2HSV(byte R, byte G, byte B, ref double H, ref double S, ref double V)
        {
            double var_R, var_G, var_B, var_Min, var_Max, del_Max, del_R, del_G, del_B;
            var_R = R / 255d;
            var_G = G / 255d;
            var_B = B / 255d;

            var_Min = NumericOperations.Min(var_R, NumericOperations.Min(var_G, var_B));
            var_Max = NumericOperations.Max(var_R, NumericOperations.Max(var_G, var_B));
            del_Max = var_Max - var_Min;

            V = var_Max;

            if (del_Max == 0d)
            {
                H = 0d;
                S = 0d;
            }
            else
            {
                S = del_Max / var_Max;

                del_R = ((var_Max - var_R) / 6d + del_Max / 2d) / del_Max;
                del_G = ((var_Max - var_G) / 6d + del_Max / 2d) / del_Max;
                del_B = ((var_Max - var_B) / 6d + del_Max / 2d) / del_Max;

                if (var_R == var_Max)
                {
                    H = del_B - del_G;
                }
                else if (var_G == var_Max)
                {
                    H = 1d / 3d + del_R - del_B;
                }
                else if (var_B == var_Max)
                {
                    H = 2d / 3d + del_G - del_R;
                }
                if (H < 0d)
                    H += 1d;
                if (H > 1d)
                    H -= 1d;
            }
        }

        public delegate int ColorDistance(int L, int R);

        public static int ColourDistanceRGB(int x, int y)
        {
            int xr = x.Bits(23, 16);
            int xg = x.Bits(15, 8);
            int xb = x.Bits(7, 0);
            int yr = y.Bits(23, 16);
            int yg = y.Bits(15, 8);
            int yb = y.Bits(7, 0);

            int rmean = (xr + yr) / 2;
            int r = xr - yr;
            int g = xg - yg;
            int b = xb - yb;
            return (510 + rmean) * r * r + 1020 * g * g + (765 - rmean) * b * b;
        }

        public static int ColourDistanceARGB(int x, int y)
        {
            int xa = x.Bits(31, 24);
            int xr = x.Bits(23, 16);
            int xg = x.Bits(15, 8);
            int xb = x.Bits(7, 0);
            int ya = y.Bits(31, 24);
            int yr = y.Bits(23, 16);
            int yg = y.Bits(15, 8);
            int yb = y.Bits(7, 0);

            int rmean = (xr * xa + yr * ya) / 510;
            int r = (xr * xa - yr * ya) / 255;
            int g = (xg * xa - yg * ya) / 255;
            int b = (xb * xa - yb * ya) / 255;
            int a = xa - ya;
            return (510 + rmean) * r * r + 1020 * g * g + (765 - rmean) * b * b + 1530 * a * a;
        }
    }
}