// ==========================================================================
// 
// File:        Counter.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 计数器
// Version:     2009.11.21.
// Copyright(C) F.R.C.
// 
// ==========================================================================


namespace Firefly
{

    public class Counter
    {
        public int Tick = 0;
        public void Count()
        {
            Tick += 1;
        }
    }
}