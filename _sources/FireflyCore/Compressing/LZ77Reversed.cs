// ==========================================================================
// 
// File:        LZ77Reversed.vb
// Location:    Firefly.Compressing <Visual Basic .Net>
// Description: 从后向前的LZ77算法类
// Version:     2009.11.21.
// Copyright(C) F.R.C.
// 
// ==========================================================================


using System;
using System.Collections.Generic;

namespace Firefly.Compressing
{
    /// <summary>
    /// 从后向前的LZ77算法类
    /// 完成一个完整压缩的时间复杂度为O(n * MaxHashStringLength)，空间复杂度为O(n)
    /// 主要用于完成完全LZ和RLE下的绝对最优压缩率的压缩。内存占用很大。
    /// 压缩时，逐次调用Match获得当前匹配，调用Proceed向左移动数据指针。
    /// </summary>
    /// <remarks>
    /// 本类不用于较长数据的压缩，如需进行较长数据的压缩，需要对数据进行分段处理。
    /// </remarks>
    public class LZ77Reversed
    {
        private byte[] Data;
        private int Offset;
        private int LowerOffset;
        private Queue<List<ListPartStringEx<byte>>> IndexTable;
        private Dictionary<ListPartStringEx<byte>, LinkedList<int>> InvertTable;
        private ushort SlideWindowLength;
        private ushort MinMatchLength;
        private ushort MaxMatchLength;
        private ushort MinHashStringLength;
        private ushort MaxHashStringLength;
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
        public LZ77Reversed(byte[] OriginalData, ushort SlideWindowLength, ushort MaxMatchLength, ushort MinMatchLength = 1, ushort MaxHashStringLength = 10) : this(OriginalData, SlideWindowLength, MaxMatchLength, MinMatchLength, MaxHashStringLength, MinMatchLength, SlideWindowLength)
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
        public LZ77Reversed(byte[] OriginalData, ushort SlideWindowLength, ushort MaxMatchLength, ushort MinMatchLength, ushort MaxHashStringLength, ushort MinHashStringLength, int MaxItemForEachItem)
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
            IndexTable = new Queue<List<ListPartStringEx<byte>>>();
            InvertTable = new Dictionary<ListPartStringEx<byte>, LinkedList<int>>(SlideWindowLength * MaxMatchLength);
            Offset = OriginalData.Length - 1;
            LowerOffset = OriginalData.Length - 1;
            IndexTableNode = new List<ListPartStringEx<byte>>();
            Update();
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

        /// <summary>已重载。向左移动</summary>
        public void Proceed(int n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException();
            for (int i = 0, loopTo = n - 1; i <= loopTo; i++)
                Proceed();
        }

        /// <summary>已重载。向左移动</summary>
        public void Proceed()
        {
            Offset -= 1;
            Delete();
            Update();
        }

        /// <summary>更新查找表</summary>
        /// <remarks>无副作用</remarks>
        private void Update()
        {
            if (Offset < -1)
                throw new InvalidOperationException();
            while (Offset - LowerOffset < SlideWindowLength && LowerOffset > 0)
            {
                LowerOffset -= 1;
                var LowerIndexTableNode = new List<ListPartStringEx<byte>>();
                ListPartStringEx<byte> s = null;
                for (int i = MinHashStringLength, loopTo = NumericOperations.Min(MaxHashStringLength, Data.Length - LowerOffset); i <= loopTo; i++)
                {
                    if (s is not null)
                    {
                        s = new ListPartStringEx<byte>(s, 1);
                    }
                    else
                    {
                        s = new ListPartStringEx<byte>(Data, LowerOffset, i);
                    }
                    LowerIndexTableNode.Add(s);
                    LinkedList<int> Index = null;
                    if (InvertTable.TryGetValue(s, out Index))
                    {
                        Index.AddLast(LowerOffset);
                    }
                    else
                    {
                        Index = new LinkedList<int>();
                        Index.AddLast(LowerOffset);
                        InvertTable.Add(s, Index);
                    }
                }
                IndexTable.Enqueue(LowerIndexTableNode);
            }
        }

        /// <summary>去除窗口外内容</summary>
        private void Delete()
        {
            if (IndexTable.Count > 0)
            {
                IndexTableNode = IndexTable.Dequeue();
                foreach (var s in IndexTableNode)
                {
                    var r = InvertTable[s];
                    System.Diagnostics.Debug.Assert(r.First.Value == Offset);
                    r.RemoveFirst();
                    if (r.Count == 0)
                        InvertTable.Remove(s);
                }
            }
            else
            {
                IndexTableNode = new List<ListPartStringEx<byte>>();
            }
        }

        /// <summary>匹配</summary>
        /// <remarks>无副作用</remarks>
        public LZPointer Match(LinkedList<AccPointer> StatesAccLength)
        {
            if (StatesAccLength.Count != Data.Length - Offset - 1)
                throw new ArgumentException();
            ushort Max = (ushort)NumericOperations.Min(MaxMatchLength, Data.Length - Offset);
            LinkedList<int> PreviousMatches = null;
            int BestAccLength = 0;
            LZPointer BestMatch = null;
            var AccLength = StatesAccLength.First;
            if (Max < MinHashStringLength)
                return null;
            for (int l = 1, loopTo = MinHashStringLength - 1; l <= loopTo; l++)
            {
                AccLength = AccLength.Next;
                if (AccLength is null)
                    return null;
            }
            ListPartStringEx<byte> s = null;
            if (Max < MinMatchLength)
                return null;
            for (ushort l = MinHashStringLength, loopTo1 = NumericOperations.Min((ushort)(MinMatchLength - 1), MaxHashStringLength); l <= loopTo1; l++)
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
                AccLength = AccLength.Next;
                if (AccLength is null)
                    return null;
            }
            for (ushort l = MinMatchLength, loopTo2 = NumericOperations.Min(Max, MaxHashStringLength); l <= loopTo2; l++)
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
                    return BestMatch;
                PreviousMatches = InvertTable[s];
                UpdateBestMatch(ref BestAccLength, ref BestMatch, l, AccLength, (ushort)(Offset - PreviousMatches.Last.Value));
                AccLength = AccLength.Next;
                if (AccLength is null)
                    return BestMatch;
            }
            if (Max <= MaxHashStringLength)
                return BestMatch;

            if (PreviousMatches.Count > MaxItemForEachItem)
            {
                var LastNode = PreviousMatches.Last;
                PreviousMatches = new LinkedList<int>();
                for (int n = 0, loopTo3 = MaxItemForEachItem - 1; n <= loopTo3; n++)
                {
                    PreviousMatches.AddFirst(LastNode.Value);
                    LastNode = LastNode.Previous;
                }
            }

            var AccLengthInitial = AccLength;
            var Node = PreviousMatches.Last;
            while (Node is not null)
            {
                AccLength = AccLengthInitial;
                bool continueWhile = false;
                bool exitWhile = false;
                for (ushort l = (ushort)(MaxHashStringLength + 1), loopTo4 = Max; l <= loopTo4; l++)
                {
                    if (Data[Node.Value + l - 1] != Data[Offset + l - 1])
                    {
                        ushort CurrentLength = (ushort)(l - 1);
                        UpdateBestMatch(ref BestAccLength, ref BestMatch, CurrentLength, AccLength.Previous, (ushort)(Offset - Node.Value));
                        Node = Node.Previous;
                        continueWhile = true;
                        break;
                    }
                    if (l == Max || AccLength.Next is null)
                    {
                        UpdateBestMatch(ref BestAccLength, ref BestMatch, Max, AccLength, (ushort)(Offset - Node.Value));
                        exitWhile = true;
                        break;
                    }
                    AccLength = AccLength.Next;
                }

                if (continueWhile)
                {
                    continue;
                }

                if (exitWhile)
                {
                    break;
                }
            }
            return BestMatch;
        }

        private static void UpdateBestMatch(ref int BestAccLength, ref LZPointer BestMatch, ushort CurLength, LinkedListNode<AccPointer> CurTailAccLengthNode, ushort NumBack)
        {
            if (BestMatch is null)
            {
                if (CurTailAccLengthNode is not null)
                {
                    BestAccLength = CurTailAccLengthNode.Value.AccLength;
                }
                else
                {
                    BestAccLength = 0;
                }
                BestMatch = new LZPointer(NumBack, CurLength, BestAccLength);
            }
            else
            {
                int CurAccLength;
                if (CurTailAccLengthNode is not null)
                {
                    CurAccLength = CurTailAccLengthNode.Value.AccLength;
                }
                else
                {
                    CurAccLength = 0;
                }
                if (CurAccLength < BestAccLength)
                {
                    BestAccLength = CurAccLength;
                    BestMatch = new LZPointer(NumBack, CurLength, CurAccLength);
                }
            }
        }

        /// <summary>LZ匹配指针，表示一个LZ匹配</summary>
        public class LZPointer : AccPointer
        {

            /// <summary>回退量</summary>
            public readonly ushort NumBack;
            private readonly ushort LengthValue;
            private int AccLengthValue;

            public LZPointer(ushort NumBack, ushort Length, int AccLength)
            {
                this.NumBack = NumBack;
                LengthValue = Length;
                AccLengthValue = AccLength;
            }

            /// <summary>长度</summary>
            public int Length
            {
                get
                {
                    return LengthValue;
                }
            }

            /// <summary>后缀最优压缩长度</summary>
            public int AccLength
            {
                get
                {
                    return AccLengthValue;
                }
                set
                {
                    AccLengthValue = value;
                }
            }

            /// <summary>后缀最优压缩长度</summary>
            public int AccLengthReadOnly
            {
                get
                {
                    return AccLengthValue;
                }
            }

            int AccPointer.AccLength { get => AccLengthReadOnly; }
        }

        /// <summary>指针，表示一个匹配</summary>
        public interface AccPointer : Pointer
        {

            /// <summary>后缀最优压缩长度</summary>
            int AccLength { get; }
        }

        /// <summary>字面量指针，表示一个字面量匹配</summary>
        public class Literal : AccPointer
        {

            private readonly int AccLengthValue;

            public Literal(int AccLength)
            {
                AccLengthValue = AccLength;
            }

            /// <summary>长度</summary>
            public int Length
            {
                get
                {
                    return 1;
                }
            }

            /// <summary>后缀最优压缩长度</summary>
            public int AccLength
            {
                get
                {
                    return AccLengthValue;
                }
            }
        }
    }
}