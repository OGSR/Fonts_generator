// ==========================================================================
// 
// File:        Tokenizer.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 词法分析器
// Version:     2009.11.21.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace Firefly
{

    public class Automaton<TState, TSymbol>
    {
        public TState State;
        public Func<TState, TSymbol, TState> Transition;

        public void Feed(TSymbol Symbol)
        {
            State = Transition(State, Symbol);
        }
    }

    public class TokenizerState<TToken, TSymbol>
    {
        public StringEx<TToken> BestValue = null;
        public int BestLength = 0;
        public List<TSymbol> Matched = new List<TSymbol>();
        public ListPartStringEx<TSymbol> MatchedStr;
        public LinkedList<TSymbol> Buffer = new LinkedList<TSymbol>();

        public TokenizerState()
        {
            MatchedStr = new ListPartStringEx<TSymbol>(Matched);
        }
    }

    public class Tokenizer<TSymbol, TToken>
    {
        private Dictionary<StringEx<TSymbol>, StringEx<TToken>> Dict;
        private Dictionary<StringEx<TSymbol>, StringEx<TToken>> DirectDict;
        private int Num;
        private int MaxTokenPerSymbolValue;

        public Tokenizer(Dictionary<StringEx<TSymbol>, StringEx<TToken>> Dict)
        {
            this.Dict = Dict;

            Num = Dict.Select(p => p.Key.Count).Max();
            DirectDict = new Dictionary<StringEx<TSymbol>, StringEx<TToken>>();
            for (int n = 1, loopTo = Num; n <= loopTo; n++)
            {
                int k = n;
                KeyValuePair<StringEx<TSymbol>, StringEx<TToken>>[] sd = (from p in Dict
                                                                          where p.Key.Count == k
                                                                          select p).ToArray();
                foreach (var p in sd)
                {
                    if (k > 1)
                    {
                        TSymbol[] a = p.Key.ToArray();
                        var s = new ListPartStringEx<TSymbol>(a, 0, 1);
                        if (DirectDict.ContainsKey(s))
                            DirectDict.Remove(s);
                        for (int i = 2, loopTo1 = k - 1; i <= loopTo1; i++)
                        {
                            s = new ListPartStringEx<TSymbol>(s, 1);
                            if (DirectDict.ContainsKey(s))
                                DirectDict.Remove(s);
                        }
                    }
                    DirectDict.Add(p.Key, p.Value);
                }
            }
            MaxTokenPerSymbolValue = Dict.Select(p => p.Value.Count).Max();
        }

        public int MaxSymbolPerToken
        {
            get
            {
                return Num;
            }
        }

        public int MaxTokenPerSymbol
        {
            get
            {
                return MaxTokenPerSymbolValue;
            }
        }

        public TokenizerState<TToken, TSymbol> GetDefaultState()
        {
            return new TokenizerState<TToken, TSymbol>();
        }

        public TokenizerState<TToken, TSymbol> Transit(TokenizerState<TToken, TSymbol> State, Action<TToken> WriteOutput, Action<TSymbol, int> ThrowException)
        {
            while (State.Buffer.Count > 0)
            {
                State.Matched.Add(State.Buffer.First.Value);
                State.MatchedStr = new ListPartStringEx<TSymbol>(State.MatchedStr, 1);
                State.Buffer.RemoveFirst();
                while (State.Matched.Count > 0)
                {
                    StringEx<TToken> DValue = null;
                    if (DirectDict.TryGetValue(State.MatchedStr, out DValue))
                    {
                        // Reduce
                        foreach (var v in DValue)
                            WriteOutput(v);
                        State.BestValue = null;
                        State.BestLength = 0;
                        State.Matched.Clear();
                        State.MatchedStr = new ListPartStringEx<TSymbol>(State.Matched);
                        break;
                    }
                    if (State.Matched.Count >= Num)
                    {
                        // 强行结束，如果无法Reduce则出错
                        StringEx<TToken> Value = null;
                        if (Dict.TryGetValue(State.MatchedStr, out Value))
                        {
                            // Reduce
                            foreach (var v in Value)
                                WriteOutput(v);
                            State.BestValue = null;
                            State.BestLength = 0;
                            State.Matched.Clear();
                            State.MatchedStr = new ListPartStringEx<TSymbol>(State.Matched);
                            break;
                        }
                        else if (State.BestValue is not null)
                        {
                            // Reduce
                            foreach (var v in State.BestValue)
                                WriteOutput(v);
                            State.BestValue = null;
                            State.BestLength = 0;
                            State.Matched.RemoveRange(0, State.BestLength);
                            State.MatchedStr = new ListPartStringEx<TSymbol>(State.Matched);
                            break;
                        }
                        else
                        {
                            // Fallback
                            for (int n = State.Matched.Count - 1; n >= 1; n -= 1)
                                State.Buffer.AddFirst(State.Matched[n]);
                            State.Matched.RemoveRange(1, State.Matched.Count - 1);
                            State.MatchedStr = new ListPartStringEx<TSymbol>(State.Matched);
                            ThrowException(State.Matched[0], -(State.Matched.Count + State.Buffer.Count));
                            State.BestValue = null;
                            State.BestLength = 0;
                            State.Matched.Clear();
                            State.MatchedStr = new ListPartStringEx<TSymbol>(State.Matched);
                        }
                    }
                    else
                    {
                        StringEx<TToken> Value = null;
                        if (Dict.TryGetValue(State.MatchedStr, out Value))
                        {
                            // Shift
                            State.BestValue = Value;
                            State.BestLength = State.MatchedStr.Count;
                            break;
                        }
                        else
                        {
                            // Shift
                            break;
                        }
                    }
                }
            }
            return State;
        }

        public TokenizerState<TToken, TSymbol> Finish(TokenizerState<TToken, TSymbol> State, Action<TToken> WriteOutput, Action<TSymbol, int> ThrowException)
        {
            while (State.Buffer.Count > 0 || State.Matched.Count > 0)
            {
                if (State.Buffer.Count > 0)
                {
                    State.Matched.Add(State.Buffer.First.Value);
                    State.MatchedStr = new ListPartStringEx<TSymbol>(State.MatchedStr, 1);
                    State.Buffer.RemoveFirst();
                }
                while (State.Matched.Count > 0)
                {
                    StringEx<TToken> DValue = null;
                    if (DirectDict.TryGetValue(State.MatchedStr, out DValue))
                    {
                        // Reduce
                        foreach (var v in DValue)
                            WriteOutput(v);
                        State.BestValue = null;
                        State.BestLength = 0;
                        State.Matched.Clear();
                        State.MatchedStr = new ListPartStringEx<TSymbol>(State.Matched);
                        break;
                    }
                    if (State.Matched.Count >= Num || State.Buffer.Count == 0)
                    {
                        // 强行结束，如果无法Reduce则出错
                        StringEx<TToken> Value = null;
                        if (Dict.TryGetValue(State.MatchedStr, out Value))
                        {
                            // Reduce
                            foreach (var v in Value)
                                WriteOutput(v);
                            State.BestValue = null;
                            State.BestLength = 0;
                            State.Matched.Clear();
                            State.MatchedStr = new ListPartStringEx<TSymbol>(State.Matched);
                            break;
                        }
                        else if (State.BestValue is not null)
                        {
                            // Reduce
                            foreach (var v in State.BestValue)
                                WriteOutput(v);
                            State.BestValue = null;
                            State.BestLength = 0;
                            State.Matched.RemoveRange(0, State.BestLength);
                            State.MatchedStr = new ListPartStringEx<TSymbol>(State.Matched);
                            break;
                        }
                        else
                        {
                            // Fallback
                            for (int n = State.Matched.Count - 1; n >= 1; n -= 1)
                                State.Buffer.AddFirst(State.Matched[n]);
                            State.Matched.RemoveRange(1, State.Matched.Count - 1);
                            State.MatchedStr = new ListPartStringEx<TSymbol>(State.Matched);
                            ThrowException(State.Matched[0], -(State.Matched.Count + State.Buffer.Count));
                            State.BestValue = null;
                            State.BestLength = 0;
                            State.Matched.Clear();
                            State.MatchedStr = new ListPartStringEx<TSymbol>(State.Matched);
                        }
                    }
                    else
                    {
                        StringEx<TToken> Value = null;
                        if (Dict.TryGetValue(State.MatchedStr, out Value))
                        {
                            // Shift
                            State.BestValue = Value;
                            State.BestLength = State.MatchedStr.Count;
                            break;
                        }
                        else
                        {
                            // Shift
                            break;
                        }
                    }
                }
            }
            return State;
        }

        public TokenizerState<TToken, TSymbol> Feed(TokenizerState<TToken, TSymbol> State, TSymbol Symbol)
        {
            State.Buffer.AddLast(Symbol);
            return State;
        }

        public bool IsStateFinished(TokenizerState<TToken, TSymbol> State)
        {
            return State.Matched.Count == 0 && State.Buffer.Count == 0;
        }
    }
}