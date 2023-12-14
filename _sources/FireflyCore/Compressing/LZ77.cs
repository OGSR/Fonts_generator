// ==========================================================================
// 
// File:        LZ77.vb
// Location:    Firefly.Compressing <Visual Basic .Net>
// Description: LZ77算法类
// Version:     2009.11.21.
// Copyright(C) F.R.C.
// 
// ==========================================================================


using System;
using System.Collections.Generic;

namespace Firefly.Compressing
{
    /// <summary>
    /// LZ77算法类
    /// 完成一个完整压缩的时间复杂度为O(n * MaxHashStringLength)，空间复杂度为O(MaxHashStringLength)
    /// 压缩时，逐次调用Match获得当前匹配，调用Proceed移动数据指针。
    /// </summary>
    /// <remarks>
    /// 本类不用于较长数据的压缩，如需进行较长数据的压缩，需要将本类用到的一些数组修改成缓存流。
    /// </remarks>
    public class LZ77
    {
        private byte[] Data;
        private int Offset;
        private int LowerOffset;
        private Queue<List<ListPartStringEx<byte>>> IndexTable;
        private Dictionary<ListPartStringEx<byte>, LinkedList<int>> InvertTable;
        private int SlideWindowLength;
        private int MinMatchLength;
        private int MaxMatchLength;
        private int MinHashStringLength;
        private int MaxHashStringLength;
        private int MaxItemForEachItem;
        private List<ListPartStringEx<byte>> IndexTableNode;

        /// <summary>
        /// 已重载。构造函数。
        /// </summary>
        /// <param name="OriginalData">原始数据</param>
        /// <param name="SlideWindowLength">滑动窗口大小</param>
        /// <param name="MaxMatchLength">最大匹配长度</param>
        /// <param name="MinMatchLength">最小匹配长度</param>
        /// <param name="MaxHashStringLength">最大散列匹配长度</param>
        public LZ77(byte[] OriginalData, int SlideWindowLength, int MaxMatchLength, int MinMatchLength = 1, int MaxHashStringLength = 10) : this(OriginalData, SlideWindowLength, MaxMatchLength, MinMatchLength, MaxHashStringLength, MinMatchLength, SlideWindowLength)
        {
        }

        /// <summary>
        /// 已重载。构造函数。
        /// </summary>
        /// <param name="OriginalData">原始数据</param>
        /// <param name="SlideWindowLength">滑动窗口大小</param>
        /// <param name="MaxMatchLength">最大匹配长度</param>
        /// <param name="MinMatchLength">最小匹配长度</param>
        /// <param name="MaxHashStringLength">最大散列匹配长度</param>
        /// <param name="MinHashStringLength">最小散列匹配长度</param>
        /// <param name="MaxItemForEachItem">最大非散列匹配项数</param>
        /// <remarks></remarks>
        public LZ77(byte[] OriginalData, int SlideWindowLength, int MaxMatchLength, int MinMatchLength, int MaxHashStringLength, int MinHashStringLength, int MaxItemForEachItem)
        {
            if (OriginalData is null)
                throw new ArgumentNullException();
            if (SlideWindowLength <= 0)
                throw new ArgumentOutOfRangeException();
            if (MinMatchLength <= 0)
                throw new ArgumentOutOfRangeException();
            if (MaxHashStringLength <= 0)
                throw new ArgumentOutOfRangeException();
            if (MinHashStringLength <= 0)
                throw new ArgumentOutOfRangeException();
            if (MaxItemForEachItem <= 0)
                throw new ArgumentOutOfRangeException();
            if (MaxMatchLength < MinMatchLength)
                throw new ArgumentException();
            if (MaxHashStringLength < MinHashStringLength)
                throw new ArgumentException();
            if (MaxMatchLength < MaxHashStringLength)
                throw new ArgumentException();
            if (MaxHashStringLength < MinMatchLength)
                throw new ArgumentException();
            Data = OriginalData;
            this.SlideWindowLength = SlideWindowLength;
            this.MinMatchLength = MinMatchLength;
            this.MaxMatchLength = MaxMatchLength;
            this.MinHashStringLength = MinHashStringLength;
            this.MaxHashStringLength = MaxHashStringLength;
            this.MaxItemForEachItem = MaxItemForEachItem;
            IndexTable = new Queue<List<ListPartStringEx<byte>>>(SlideWindowLength + MaxMatchLength);
            InvertTable = new Dictionary<ListPartStringEx<byte>, LinkedList<int>>(SlideWindowLength * MaxMatchLength);
            Offset = 0;
            LowerOffset = 0;
            IndexTableNode = new List<ListPartStringEx<byte>>();
        }

        /// <summary>原始数据</summary>
        public byte[] OriginalData
        {
            get
            {
                return Data;
            }
        }

        /// <summary>位置</summary>
        public int Position
        {
            get
            {
                return Offset;
            }
        }

        /// <summary>已重载。前进</summary>
        public void Proceed(int n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException();
            for (int i = 0, loopTo = n - 1; i <= loopTo; i++)
                Proceed();
        }

        /// <summary>已重载。前进</summary>
        public void Proceed()
        {
            Update();
            IndexTable.Enqueue(IndexTableNode);
            IndexTableNode = new List<ListPartStringEx<byte>>();
            Offset += 1;
            Delete();
        }

        /// <summary>更新查找表</summary>
        private void Update()
        {
            if (Offset >= Data.Length)
                throw new InvalidOperationException();
            ListPartStringEx<byte> s = null;
            for (int i = MinHashStringLength, loopTo = NumericOperations.Min(MaxHashStringLength, Data.Length - 1 - Offset); i <= loopTo; i++)
            {
                if (i - MinHashStringLength < IndexTableNode.Count)
                {
                    s = IndexTableNode[i - MinHashStringLength];
                }
                else if (s is not null)
                {
                    s = new ListPartStringEx<byte>(s, 1);
                    IndexTableNode.Add(s);
                }
                else
                {
                    s = new ListPartStringEx<byte>(Data, Offset, i);
                    IndexTableNode.Add(s);
                }
                LinkedList<int> Index = null;
                if (InvertTable.TryGetValue(s, out Index))
                {
                    Index.AddLast(Offset);
                }
                else
                {
                    Index = new LinkedList<int>();
                    Index.AddLast(Offset);
                    InvertTable.Add(s, Index);
                }
            }
        }

        /// <summary>去除窗口外内容</summary>
        private void Delete()
        {
            while (Offset - LowerOffset > SlideWindowLength)
            {
                foreach (var s in IndexTable.Dequeue())
                {
                    var r = InvertTable[s];
                    System.Diagnostics.Debug.Assert(r.First.Value == LowerOffset);
                    r.RemoveFirst();
                    if (r.Count == 0)
                        InvertTable.Remove(s);
                }
                LowerOffset += 1;
            }
        }

        /// <summary>匹配</summary>
        /// <remarks>无副作用</remarks>
        public LZPointer Match()
        {
            int Max = NumericOperations.Min(MaxMatchLength, Data.Length - Offset);
            LinkedList<int> PreviousMatches = null;
            LZPointer PreviousMatch = null;
            ListPartStringEx<byte> s = null;
            if (Max < MinMatchLength)
                return null;
            for (int l = MinHashStringLength, loopTo = NumericOperations.Min(MinMatchLength - 1, MaxHashStringLength); l <= loopTo; l++)
            {
                if (l - MinHashStringLength < IndexTableNode.Count)
                {
                    s = IndexTableNode[l - MinHashStringLength];
                }
                else if (s is not null)
                {
                    s = new ListPartStringEx<byte>(s, 1);
                    IndexTableNode.Add(s);
                }
                else
                {
                    s = new ListPartStringEx<byte>(Data, Offset, l);
                    IndexTableNode.Add(s);
                }
                if (!InvertTable.ContainsKey(s))
                {
                    return null;
                }
                PreviousMatches = InvertTable[s];
            }
            for (int l = MinMatchLength, loopTo1 = NumericOperations.Min(MaxHashStringLength, Max); l <= loopTo1; l++)
            {
                if (l - MinHashStringLength < IndexTableNode.Count)
                {
                    s = IndexTableNode[l - MinHashStringLength];
                }
                else if (s is not null)
                {
                    s = new ListPartStringEx<byte>(s, 1);
                    IndexTableNode.Add(s);
                }
                else
                {
                    s = new ListPartStringEx<byte>(Data, Offset, l);
                    IndexTableNode.Add(s);
                }
                if (!InvertTable.ContainsKey(s))
                {
                    return PreviousMatch;
                }
                PreviousMatches = InvertTable[s];
                PreviousMatch = new LZPointer(Offset - PreviousMatches.Last.Value, l);
            }
            if (Max <= MaxHashStringLength)
                return PreviousMatch;

            if (PreviousMatches.Count > MaxItemForEachItem)
            {
                var LastNode = PreviousMatches.Last;
                PreviousMatches = new LinkedList<int>();
                for (int n = 0, loopTo2 = MaxItemForEachItem - 1; n <= loopTo2; n++)
                {
                    PreviousMatches.AddFirst(LastNode.Value);
                    LastNode = LastNode.Previous;
                }
            }

            int MaxLength = MaxHashStringLength;
            var BestNode = PreviousMatches.Last;
            var Node = PreviousMatches.Last;
            while (Node is not null)
            {
                int l;
                bool continueWhile = false;
                var loopTo3 = Max;
                for (l = MaxHashStringLength + 1; l <= loopTo3; l++)
                {
                    if (Data[Node.Value + l - 1] != Data[Offset + l - 1])
                    {
                        int CurrentLength = l - 1;
                        if (MaxLength < CurrentLength)
                        {
                            MaxLength = CurrentLength;
                            BestNode = Node;
                        }
                        Node = Node.Previous;
                        continueWhile = true;
                        break;
                    }
                }

                if (continueWhile)
                {
                    continue;
                }
                MaxLength = Max;
                BestNode = Node;
                break;
            }
            return new LZPointer(Offset - BestNode.Value, MaxLength);
        }

        /// <summary>LZ匹配指针，表示一个LZ匹配</summary>
        public class LZPointer : Pointer
        {

            /// <summary>回退量</summary>
            public readonly int NumBack;
            private readonly int LengthValue;

            public LZPointer(int NumBack, int Length)
            {
                this.NumBack = NumBack;
                LengthValue = Length;
            }

            /// <summary>长度</summary>
            public int Length
            {
                get
                {
                    return LengthValue;
                }
            }
        }
    }
}