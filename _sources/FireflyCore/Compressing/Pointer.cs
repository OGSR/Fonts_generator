
// ==========================================================================
// 
// File:        Pointer.vb
// Location:    Firefly.Compressing <Visual Basic .Net>
// Description: 压缩匹配指针
// Version:     2009.01.20.
// Copyright(C) F.R.C.
// 
// ==========================================================================

namespace Firefly.Compressing
{
    /// <summary>压缩匹配指针</summary>
    public interface Pointer
    {
        int Length { get; }
    }

    /// <summary>字面量</summary>
    public class Literal : Pointer
    {

        public Literal()
        {
        }

        /// <summary>长度</summary>
        public int Length
        {
            get
            {
                return 1;
            }
        }
    }
}