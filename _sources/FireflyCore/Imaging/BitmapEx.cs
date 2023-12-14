// ==========================================================================
// 
// File:        BitmapEx.vb
// Location:    Firefly.Imaging <Visual Basic .Net>
// Description: Bitmap扩展函数
// Version:     2009.03.09.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Firefly.Imaging
{

    /// <summary>
    /// 用于扩展System.Drawing.Bitmap，使得可以将其中数据与Int32数组交换。
    /// </summary>
    public static class BitmapEx
    {
        /// <summary>
        /// 从Bitmap中获得颜色数组。
        /// 仅支持格式为PixelFormat.Format32bppArgb的Bitmap，且BitmapData.Stride必须为宽度的4倍。
        /// </summary>
        public static int[,] GetRectangle(this Bitmap Bitmap, int x, int y, int w, int h)
        {
            if (Bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new NotSupportedException();

            if (w < 0 || h < 0)
                return null;
            int[,] a = new int[w, h];
            if (w == 0)
                return a;
            if (h == 0)
                return a;
            int ox, oy;
            if (y < 0)
            {
                h = h + y;
                oy = 0;
            }
            else
            {
                oy = y;
            }
            if (oy + h > Bitmap.Height)
            {
                h = Bitmap.Height - oy;
            }
            if (x < 0)
            {
                ox = 0;
            }
            else
            {
                ox = x;
            }
            if (ox + w > Bitmap.Width)
            {
                w = Bitmap.Width - ox;
            }
            int xl = ox - x;
            int xu;
            if (x >= 0)
            {
                xu = w + ox - x - 1;
            }
            else
            {
                xu = w - 1;
            }

            if (h <= 0)
                return a;

            var Rect = new Rectangle(0, oy, Bitmap.Width, h);
            var BitmapData = Bitmap.LockBits(Rect, ImageLockMode.ReadOnly, Bitmap.PixelFormat);
            if (BitmapData.Stride != Bitmap.Width * 4)
                throw new NotSupportedException();

            var Ptr = BitmapData.Scan0;
            int NumPixels = BitmapData.Stride * h / 4;
            int[] Pixels = new int[NumPixels];
            Marshal.Copy(Ptr, Pixels, 0, NumPixels);
            Bitmap.UnlockBits(BitmapData);

            int o = oy - y;
            for (int m = 0, loopTo = h - 1; m <= loopTo; m++)
            {
                for (int n = xl, loopTo1 = xu; n <= loopTo1; n++)
                    a[n, o + m] = Pixels[ox + n + m * Bitmap.Width];
            }

            return a;
        }

        /// <summary>
        /// 将颜色数组放入Bitmap。
        /// 仅支持格式为PixelFormat.Format32bppArgb的Bitmap，且BitmapData.Stride必须为宽度的4倍。
        /// </summary>
        public static void SetRectangle(this Bitmap Bitmap, int x, int y, int[,] a)
        {
            if (Bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new NotSupportedException();

            if (a is null)
                return;
            int w = a.GetLength(0);
            int h = a.GetLength(1);
            if (w <= 0)
                return;
            if (h <= 0)
                return;
            int ox, oy;
            if (y < 0)
            {
                h = h + y;
                oy = 0;
            }
            else
            {
                oy = y;
            }
            if (oy + h > Bitmap.Height)
            {
                h = Bitmap.Height - oy;
            }
            if (x < 0)
            {
                ox = 0;
            }
            else
            {
                ox = x;
            }
            if (ox + w > Bitmap.Width)
            {
                w = Bitmap.Width - ox;
            }
            int xl = ox - x;
            int xu;
            if (x >= 0)
            {
                xu = w + ox - x - 1;
            }
            else
            {
                xu = w - 1;
            }

            if (h <= 0)
                return;

            var Rect = new Rectangle(0, oy, Bitmap.Width, h);
            var BitmapData = Bitmap.LockBits(Rect, ImageLockMode.ReadWrite, Bitmap.PixelFormat);
            if (BitmapData.Stride != Bitmap.Width * 4)
                throw new NotSupportedException();

            var Ptr = BitmapData.Scan0;
            int NumPixels = BitmapData.Stride * h / 4;
            int[] Pixels = new int[NumPixels];
            Marshal.Copy(Ptr, Pixels, 0, NumPixels);

            int o = oy - y;
            for (int m = 0, loopTo = h - 1; m <= loopTo; m++)
            {
                for (int n = xl, loopTo1 = xu; n <= loopTo1; n++)
                    Pixels[ox + n + m * Bitmap.Width] = a[n, o + m];
            }

            Marshal.Copy(Pixels, 0, Ptr, NumPixels);
            Bitmap.UnlockBits(BitmapData);
        }
    }
}