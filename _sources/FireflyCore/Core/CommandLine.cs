// ==========================================================================
// 
// File:        CommandLine.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 控制台
// Version:     2010.01.31.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.CompilerServices;

namespace Firefly
{

    public sealed class CommandLine
    {
        private CommandLine()
        {
        }

        public class CommandLineOption
        {
            public string Name;
            public string[] Arguments;
        }

        public class CommandLineArguments
        {
            public string[] Arguments;
            public CommandLineOption[] Options;
        }

        private static string DescapeQuota(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            if (s.StartsWith("\"") && s.EndsWith("\""))
            {
                return s.Substring(1, s.Length - 2).Replace("\"\"", "\"");
            }
            else
            {
                return s;
            }
        }

        public static CommandLineArguments GetCmdLine()
        {
            var argv = new List<string>();
            bool SuppressedFirst = false;
            int NextStart = 0;
            string CmdLine = Environment.CommandLine;
            foreach (Match arg in Regex.Matches(CmdLine, "(\"[^\"]*\"|([^\" ])+)+", RegexOptions.ExplicitCapture))
            {
                if (arg.Index != NextStart)
                {
                    if (!CmdLine.Substring(NextStart, arg.Index - NextStart).All(c => Conversions.ToString(c) == " "))
                        throw new InvalidOperationException();
                }
                NextStart = arg.Index + arg.Length;
                if (!SuppressedFirst)
                {
                    SuppressedFirst = true;
                    continue;
                }
                if (arg.Success)
                {
                    string m = arg.Value;
                    argv.Add(DescapeQuota(m));
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            if (CmdLine.Length != NextStart)
            {
                if (!CmdLine.Substring(NextStart, CmdLine.Length - NextStart).All(c => Conversions.ToString(c) == " "))
                    throw new InvalidOperationException();
            }

            var Arguments = new List<string>();
            var Options = new List<CommandLineOption>();

            foreach (var arg in argv)
            {
                if (arg.StartsWith("/"))
                {
                    string OptionLine = arg.Substring(1);
                    string Name;
                    string ParameterLine;
                    int Index = OptionLine.IndexOf(":");
                    if (Index >= 0)
                    {
                        Name = DescapeQuota(OptionLine.Substring(0, Index));
                        ParameterLine = OptionLine.Substring(Index + 1);
                    }
                    else
                    {
                        Name = DescapeQuota(OptionLine);
                        ParameterLine = "";
                    }
                    Options.Add(new CommandLineOption() { Name = Name, Arguments = (from t in ParameterLine.Split(',') select DescapeQuota(t.Trim(' '))).ToArray() });
                }
                else
                {
                    Arguments.Add(arg);
                }
            }

            return new CommandLineArguments() { Arguments = Arguments.ToArray(), Options = Options.ToArray() };
        }
    }
}