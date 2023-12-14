// ==========================================================================
// 
// File:        EncodingString.vb
// Location:    Firefly.TextEncoding <Visual Basic .Net>
// Description: 编码
// Version:     2008.11.28.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;

namespace Firefly.TextEncoding
{
    public static class EncodingString
    {
        /// <summary>已重载。得到编码文本，按第一次出现的位置排序。</summary>
        public static string GetEncodingStringFromText(string Text, string Exclude = "")
        {
            return GetEncodingString32FromText(Text.ToUTF32(), Exclude).ToUTF16B();
        }
        /// <summary>已重载。得到编码文本，按第一次出现的位置排序。</summary>
        public static string GetEncodingStringFromText(string[] Text, string Exclude = "")
        {
            Char32[][] Char32Text = new Char32[Text.Length][];
            for (int n = 0, loopTo = Text.Length - 1; n <= loopTo; n++)
                Char32Text[n] = Text[n].ToUTF32();
            return GetEncodingString32FromText(Char32Text, Exclude).ToUTF16B();
        }
        /// <summary>已重载。得到编码文本，按第一次出现的位置排序。</summary>
        public static Char32[] GetEncodingString32FromText(string Text, string Exclude = "")
        {
            return GetEncodingString32FromText(Text.ToUTF32(), Exclude);
        }
        /// <summary>已重载。得到编码文本，按第一次出现的位置排序。</summary>
        public static Char32[] GetEncodingString32FromText(string[] Text, string Exclude = "")
        {
            Char32[][] Char32Text = new Char32[Text.Length][];
            for (int n = 0, loopTo = Text.Length - 1; n <= loopTo; n++)
                Char32Text[n] = Text[n].ToUTF32();
            return GetEncodingString32FromText(Char32Text, Exclude);
        }
        /// <summary>已重载。得到编码文本，按第一次出现的位置排序。</summary>
        public static Char32[] GetEncodingString32FromText(Char32[] Text, string Exclude = "")
        {
            var s = new List<Char32>();
            var d = new Dictionary<Char32, int>();
            var dExclude = new Dictionary<Char32, int>();
            foreach (var c in Exclude)
            {
                if (!dExclude.ContainsKey(c))
                    dExclude.Add(c, 0);
            }
            foreach (var c in Text)
            {
                if (dExclude.ContainsKey(c))
                    continue;
                if (!d.ContainsKey(c))
                {
                    d.Add(c, 0);
                    s.Add(c);
                }
            }
            return s.ToArray();
        }
        /// <summary>已重载。得到编码文本，按第一次出现的位置排序。</summary>
        public static Char32[] GetEncodingString32FromText(Char32[][] Text, string Exclude = "")
        {
            var s = new List<Char32>();
            var d = new Dictionary<Char32, int>();
            var dExclude = new Dictionary<Char32, int>();
            foreach (var c in Exclude)
            {
                if (!dExclude.ContainsKey(c))
                    dExclude.Add(c, 0);
            }
            foreach (var t in Text)
            {
                foreach (var c in t)
                {
                    if (dExclude.ContainsKey(c))
                        continue;
                    if (!d.ContainsKey(c))
                    {
                        d.Add(c, 0);
                        s.Add(c);
                    }
                }
            }
            return s.ToArray();
        }

        /// <summary>编码文本生成器</summary>
        public class EncodingStringGenerator
        {
            protected List<int> l = new List<int>();
            protected List<Char32> s = new List<Char32>();
            protected Dictionary<Char32, int> d = new Dictionary<Char32, int>();
            protected Dictionary<Char32, int> dExclude = new Dictionary<Char32, int>();

            /// <summary>已重载。创建新实例。</summary>
            public EncodingStringGenerator()
            {
            }
            /// <summary>已重载。用排除列表创建新实例。</summary>
            public EncodingStringGenerator(string Exclude) : this(Exclude.ToUTF32())
            {
            }
            /// <summary>已重载。用排除列表创建新实例。</summary>
            public EncodingStringGenerator(Char32[] Exclude)
            {
                foreach (var c in Exclude)
                {
                    if (dExclude.ContainsKey(c))
                        continue;
                    dExclude.Add(c, 0);
                }
            }
            /// <summary>已重载。添加排除的字符列表。</summary>
            public void PushExclude(char c)
            {
                PushExclude((Char32)c);
            }
            /// <summary>已重载。添加排除的字符列表。</summary>
            public void PushExclude(Char32 c)
            {
                if (dExclude.ContainsKey(c))
                    return;
                dExclude.Add(c, 0);
                if (d.ContainsKey(c))
                {
                    l[d[c]] = 0;
                    d.Remove(c);
                }
            }
            /// <summary>已重载。添加排除的字符列表。</summary>
            public void PushExclude(string Exclude)
            {
                PushExclude(Exclude.ToUTF32());
            }
            /// <summary>已重载。添加排除的字符列表。</summary>
            public void PushExclude(Char32[] Exclude)
            {
                foreach (var c in Exclude)
                {
                    if (dExclude.ContainsKey(c))
                        continue;
                    dExclude.Add(c, 0);
                    if (d.ContainsKey(c))
                    {
                        l[d[c]] = 0;
                        d.Remove(c);
                    }
                }
            }
            /// <summary>已重载。从文本添加字符。</summary>
            public void PushText(string Text)
            {
                PushText(Text.ToUTF32());
            }
            /// <summary>已重载。从文本添加字符。</summary>
            public void PushText(Char32[] Text)
            {
                foreach (var c in Text)
                {
                    if (dExclude.ContainsKey(c))
                        continue;
                    if (!d.ContainsKey(c))
                    {
                        d.Add(c, l.Count);
                        s.Add(c);
                        l.Add(1);
                    }
                    else
                    {
                        l[d[c]] += 1;
                    }
                }
            }
            /// <summary>已重载。得到字库文字，频率高的在前。</summary>
            public string GetLibString()
            {
                return GetLibString32().ToUTF16B();
            }
            /// <summary>已重载。得到字库文字，频率高的在前。</summary>
            public Char32[] GetLibString32()
            {
                Char32[] ret = s.ToArray();
                int[] retl = l.ToArray();
                Array.Sort(retl, ret);
                Array.Reverse(ret);
                Array.Reverse(retl);
                for (int i = ret.Length - 1; i >= 0; i -= 1)
                {
                    if (retl[i] > 0)
                    {
                        Char32[] a = new Char32[i + 1];
                        Array.Copy(ret, a, i + 1);
                        return a;
                    }
                }
                return new Char32[] { };
            }
            /// <summary>清空。</summary>
            public void Clear()
            {
                l.Clear();
                s.Clear();
                d.Clear();
            }
        }
    }
}