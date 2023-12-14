// ==========================================================================
// 
// File:        ImageInterface.vb
// Location:    Firefly.Imaging <Visual Basic .Net>
// Description: 图片调用接口和实现
// Version:     2010.04.04.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;

namespace Firefly
{

    public interface IImageReader : IDisposable
    {

        void Load();
        int[,] GetRectangleAsARGB(int x, int y, int w, int h);
    }

    public interface IImageWriter : IDisposable
    {

        void Create(int w, int h);
        void SetRectangleFromARGB(int x, int y, int[,] a);
    }
}