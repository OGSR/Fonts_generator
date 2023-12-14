// ==========================================================================
// 
// File:        INI.vb
// Location:    Firefly.Setting <Visual Basic .Net>
// Description: INI控制类及相关
// Created:     2004.10.31.09:33:47(GMT+08:00)
// Version:     0.6 2010.02.22.
// Copyright(C) F.R.C.
// 
// ==========================================================================


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Firefly.TextEncoding;
using Firefly.Texting;

namespace Firefly.Setting
{
    /// <summary>INI控制类</summary>
    /// <remarks>
    /// 本类管理INI文件
    /// 注意 本类初始化时会从文件读取数据,但没有文件也可
    /// 注意 相同的键只保留后者
    /// 注意 写函数
    /// 
    /// 注意 本类的字符串支持字符转义
    /// @ 在字符串前可取消字符转义
    /// { } 可用表示多行文字 此时自动禁止转义 {必须在有等号那行 }必须是那行的最后一个除空格以外的字符
    /// $ 在字符串前表示字符串的值从后面的外部文件得到 此时自动禁止转义
    /// 若@{$连用只有前面的起作用
    /// ; # // 用作单行注释
    /// /* */ 用作多行注释
    /// 没有等号和节格式的行不处理 不推荐作为注释
    /// \a 与响铃（警报）\u0007 匹配 
    /// \b 与退格符 \u0008 匹配
    /// \t 与 Tab 符 \u0009 匹配 
    /// \r 与回车符 \u000D 匹配
    /// \v 与垂直 Tab 符 \u000B 匹配
    /// \f 与换页符 \u000C 匹配
    /// \n 与换行符 \u000A 匹配
    /// \x?? 与 Chr(??) 匹配
    /// \x2F 与 左斜杠符 / 匹配
    /// \x5C 与 右斜杠符 \ 匹配
    /// 
    /// 本类使用ReadValue来读值 如果没有读出返回False  用New INI(FILE_NAME)得到的实例会自动调用此函数
    /// 本类使用WriteValue来写入值到内存
    /// 本类使用ReadFromFile将从文件添入值 如果没有文件可用返回False
    /// 本类使用WriteToFile将所有改变写入文件 如果没有写入返回False
    /// </remarks>
    public class Ini
    {
        private string FilePath;
        private System.Text.Encoding Encoding;

        private List<string> Sections = new List<string>();
        private Dictionary<string, Section> SectionDict = new Dictionary<string, Section>(StringComparer.OrdinalIgnoreCase);

        private class Section
        {
            public List<string> Entries = new List<string>();
            public Dictionary<string, string> EntryDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public Ini()
        {
            FilePath = "";
            Encoding = TextEncoding.TextEncoding.WritingDefault;
        }
        public Ini(string Path) : this(Path, true, null)
        {
        }
        public Ini(string Path, bool Read) : this(Path, Read, null)
        {
        }
        public Ini(string Path, bool Read, System.Text.Encoding Encoding)
        {
            FilePath = Path;
            this.Encoding = Encoding;
            if (Read)
                ReadFromFile(Path);
        }
        public bool ReadFromFile(string Path)
        {
            return ReadFromFile(Path, null);
        }
        public bool ReadFromFile(string Path, System.Text.Encoding Encoding)
        {
            if (Encoding is null)
                Encoding = TextEncoding.TextEncoding.Default;

            string[] Line;
            try
            {
                Line = Regex.Split(Txt.ReadFile(Path, Encoding), @"\r\n|\n");
            }
            catch
            {
                return false;
            }

            Section CurrentSection = null;
            for (int n = 0, loopTo = Line.GetUpperBound(0); n <= loopTo; n++)
            {
                // 处理节和键
                string[] TempLine = Line[n].Split(new char[] { '=' }, 2);
                TempLine[0] = TempLine[0].Trim(' ');
                if (TempLine.Length > 1)
                {
                    TempLine[1] = TempLine[1].TrimStart(' ');
                }
                if (TempLine.GetLength(0) == 1)
                {
                    // 处理节
                    if (TempLine[0].StartsWith("[") && TempLine[0].EndsWith("]"))
                    {
                        string SectionName = TempLine[0].TrimStart('[').TrimEnd(']');
                        if (!SectionDict.ContainsKey(SectionName))
                        {
                            Sections.Add(SectionName);
                            CurrentSection = new Section();
                            SectionDict.Add(SectionName, CurrentSection);
                        }
                        else
                        {
                            CurrentSection = SectionDict[SectionName];
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(TempLine[0]))
                        continue;
                    if (TempLine[0].Contains(";"))
                        continue;
                    if (TempLine[0].Contains("#"))
                        continue;
                    if (TempLine[0].Contains("//"))
                        continue;
                    if (TempLine[0].Contains("/*"))
                        continue;
                    if (TempLine[0].Contains("*/"))
                        continue;
                    if (TempLine[1].StartsWith("{"))
                    {
                        string s = TempLine[1].Remove(0, 1);
                        if (!string.IsNullOrEmpty(s))
                            s = s + Environment.NewLine;
                        while (!Line[n].EndsWith("}"))
                        {
                            n += 1;
                            if (n > Line.GetUpperBound(0))
                                break;
                            s += Line[n] + Environment.NewLine;
                        }
                        if (s.EndsWith("}" + Environment.NewLine))
                            s = s.Substring(0, s.Length - 1 - Environment.NewLine.Length);
                        if (s.EndsWith(Environment.NewLine))
                            s = s.Substring(0, s.Length - Environment.NewLine.Length);
                        TempLine[1] = "@" + s;
                    }
                    else if (TempLine[1].StartsWith("$"))
                    {
                        try
                        {
                            TempLine[1] = Txt.ReadFile(TempLine[1].Remove(0, 1).Trim(' '), Encoding);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    else
                    {
                        // 除去"/*"到"*/"的注释
                        int Index = TempLine[1].IndexOf("/*");
                        if (!(Index < 0))
                        {
                            int Index2 = TempLine[1].IndexOf("*/");
                            if (Index2 > Index)
                            {
                                TempLine[1] = TempLine[1].Substring(0, Index - 1 + 1) + TempLine[1].Substring(Index2 + 2);
                            }
                            else
                            {
                                TempLine[1] = TempLine[1].Substring(0, Index - 1 + 1);
                                n += 1;
                                bool exitFor = false;
                                while (Line[n].IndexOf("*/") < 0)
                                {
                                    n += 1;
                                    if (n > Line.GetUpperBound(0))
                                    {
                                        exitFor = true;
                                        break;
                                    }
                                }

                                if (exitFor)
                                {
                                    break;
                                }
                                TempLine[1] += Line[n].Substring(Line[n].IndexOf("*/") + 2);
                            }
                        }
                        // 除去单行注释
                        TempLine[1] = TempLine[1].Split(';')[0];
                        TempLine[1] = TempLine[1].Split('#')[0];
                        Index = TempLine[1].IndexOf("//");
                        if (!(Index < 0))
                            TempLine[1] = TempLine[1].Substring(0, Index);
                    }
                    if (!TempLine[1].StartsWith("@"))
                        TempLine[1] = TempLine[1].Trim(' ');

                    // 处理键
                    string Key = TempLine[0];
                    string Value = TempLine[1];
                    if (CurrentSection.EntryDict.ContainsKey(Key))
                    {
                        CurrentSection.EntryDict[Key] = Value;
                    }
                    else
                    {
                        CurrentSection.Entries.Add(Key);
                        CurrentSection.EntryDict.Add(Key, Value);
                    }
                }
            }
            return true;
        }

        public bool ReadValue(string SectionName, string Key, ref string Value)
        {
            if (!SectionDict.ContainsKey(SectionName))
                return false;
            var CurrentSection = SectionDict[SectionName];

            if (!CurrentSection.EntryDict.ContainsKey(Key))
                return false;
            string TempString = CurrentSection.EntryDict[Key];

            if (!TempString.StartsWith("@"))
            {
                TempString = TempString.Descape();
                Value = TempString;
            }
            else
            {
                Value = TempString.Remove(0, 1);
            }
            return true;
        }
        public bool ReadValue(string SectionName, string Key, ref decimal Value)
        {
            string TempString = null;
            if (!ReadValue(SectionName, Key, ref TempString))
                return false;
            try
            {
                Value = decimal.Parse(TempString, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public bool ReadValue(string SectionName, string Key, ref float Value)
        {
            string TempString = null;
            if (!ReadValue(SectionName, Key, ref TempString))
                return false;
            try
            {
                Value = float.Parse(TempString, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public bool ReadValue(string SectionName, string Key, ref double Value)
        {
            string TempString = null;
            if (!ReadValue(SectionName, Key, ref TempString))
                return false;
            try
            {
                Value = double.Parse(TempString, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public bool ReadValue(string SectionName, string Key, ref bool Value)
        {
            string TempString = null;
            if (!ReadValue(SectionName, Key, ref TempString))
                return false;
            try
            {
                Value = bool.Parse(TempString);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public bool ReadValue(string SectionName, string Key, ref byte Value)
        {
            string TempString = null;
            if (!ReadValue(SectionName, Key, ref TempString))
                return false;
            try
            {
                Value = byte.Parse(TempString, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public bool ReadValue(string SectionName, string Key, ref sbyte Value)
        {
            string TempString = null;
            if (!ReadValue(SectionName, Key, ref TempString))
                return false;
            try
            {
                Value = sbyte.Parse(TempString, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public bool ReadValue(string SectionName, string Key, ref short Value)
        {
            string TempString = null;
            if (!ReadValue(SectionName, Key, ref TempString))
                return false;
            try
            {
                Value = short.Parse(TempString, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public bool ReadValue(string SectionName, string Key, ref ushort Value)
        {
            string TempString = null;
            if (!ReadValue(SectionName, Key, ref TempString))
                return false;
            try
            {
                Value = ushort.Parse(TempString, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public bool ReadValue(string SectionName, string Key, ref int Value)
        {
            string TempString = null;
            if (!ReadValue(SectionName, Key, ref TempString))
                return false;
            try
            {
                Value = int.Parse(TempString, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public bool ReadValue(string SectionName, string Key, ref uint Value)
        {
            string TempString = null;
            if (!ReadValue(SectionName, Key, ref TempString))
                return false;
            try
            {
                Value = uint.Parse(TempString, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public bool ReadValue(string SectionName, string Key, ref long Value)
        {
            string TempString = null;
            if (!ReadValue(SectionName, Key, ref TempString))
                return false;
            try
            {
                Value = long.Parse(TempString, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public bool ReadValue(string SectionName, string Key, ref ulong Value)
        {
            string TempString = null;
            if (!ReadValue(SectionName, Key, ref TempString))
                return false;
            try
            {
                Value = ulong.Parse(TempString, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public bool ReadValue(string SectionName, string Key, ref string[] Value)
        {
            string TempString = null;
            if (!ReadValue(SectionName, Key, ref TempString))
                return false;
            string[] Temp;
            if (!TempString.StartsWith("@"))
            {
                if (string.IsNullOrEmpty(TempString))
                    return true;
                Temp = TempString.Split(',');
                if (Value is null || Value.GetUpperBound(0) != Temp.GetUpperBound(0))
                    Value = new string[Temp.GetUpperBound(0) + 1];
                for (int n = 0, loopTo = Math.Min(Value.GetUpperBound(0), Temp.GetUpperBound(0)); n <= loopTo; n++)
                {
                    Temp[n] = Temp[n].Descape();
                    Value[n] = Temp[n];
                }
            }
            else
            {
                TempString = TempString.Remove(0, 1);
                if (string.IsNullOrEmpty(TempString))
                    return true;
                Temp = TempString.Split(',');
                if (Value is null || Value.GetUpperBound(0) != Temp.GetUpperBound(0))
                    Value = new string[Temp.GetUpperBound(0) + 1];
                for (int n = 0, loopTo1 = Math.Min(Value.GetUpperBound(0), Temp.GetUpperBound(0)); n <= loopTo1; n++)
                    Value[n] = Temp[n].TrimStart(String32.ChrQ(13)).TrimStart(String32.ChrQ(10)).TrimEnd(String32.ChrQ(10)).TrimEnd(String32.ChrQ(13));
            }
            return true;
        }
        public bool ReadValue(string SectionName, string Key, ref string[,] Value)
        {
            string TempString = null;
            if (!ReadValue(SectionName, Key, ref TempString))
                return false;
            if (Value is null)
                return false;
            string[] TempLine;
            string[] Temp;
            if (!TempString.StartsWith("@"))
            {
                if (string.IsNullOrEmpty(TempString))
                    return true;
                TempLine = Regex.Split(TempString, @"\r\n|\n");
                for (int n = 0, loopTo = Math.Min(Value.GetUpperBound(1), TempLine.GetUpperBound(0)); n <= loopTo; n++)
                {
                    Temp = TempLine[n].Split(',');
                    for (int m = 0, loopTo1 = Math.Min(Value.GetUpperBound(0), Temp.GetUpperBound(0)); m <= loopTo1; m++)
                    {
                        Temp[m] = Temp[m].Descape();
                        Value[m, n] = Temp[m];
                    }
                }
            }
            else
            {
                TempString = TempString.Remove(0, 1);
                if (string.IsNullOrEmpty(TempString))
                    return true;
                TempLine = Regex.Split(TempString, @"\r\n|\n");
                for (int n = 0, loopTo2 = Math.Min(Value.GetUpperBound(1), TempLine.GetUpperBound(0)); n <= loopTo2; n++)
                {
                    Temp = TempLine[n].Split(',');
                    for (int m = 0, loopTo3 = Math.Min(Value.GetUpperBound(0), Temp.GetUpperBound(0)); m <= loopTo3; m++)
                        Value[m, n] = Temp[m].TrimStart(String32.ChrQ(13)).TrimStart(String32.ChrQ(10)).TrimEnd(String32.ChrQ(10)).TrimEnd(String32.ChrQ(13));
                }
            }
            return true;
        }
        public void WriteValue(string SectionName, string Key, string Value, bool TransferMeaning)
        {
            Section CurrentSection;
            if (SectionDict.ContainsKey(SectionName))
            {
                CurrentSection = SectionDict[SectionName];
            }
            else
            {
                Sections.Add(SectionName);
                CurrentSection = new Section();
                SectionDict.Add(SectionName, CurrentSection);
            }

            string TempString = "";
            if (!string.IsNullOrEmpty(Value))
                TempString = Value;
            if (TransferMeaning)
            {
                TempString = TempString.Replace(";", @"\x3B");
                TempString = TempString.Replace("#", @"\x23");
                TempString = TempString.Replace("//", @"\x2F\x2F");
                TempString = TempString.Replace("/*", @"\x2F\x2A");
                TempString = TempString.Replace("*/", @"\x2A\x2F");
                TempString = TempString.Replace("{", @"\x7B");
                TempString = TempString.Replace("}", @"\x7D");
                TempString = TempString.Replace("@", @"\x40");
                TempString = TempString.Replace("$", @"\x24");
                TempString = TempString.Replace(@"\", @"\x5C");
                TempString = TempString.Replace(@"\a".Descape(), @"\a");
                TempString = TempString.Replace(@"\b".Descape(), @"\b");
                TempString = TempString.Replace(@"\t".Descape(), @"\t");
                TempString = TempString.Replace(@"\r".Descape(), @"\r");
                TempString = TempString.Replace(@"\v".Descape(), @"\v");
                TempString = TempString.Replace(@"\f".Descape(), @"\f");
                TempString = TempString.Replace(@"\n".Descape(), @"\n");
                TempString = TempString.Replace(@"\b".Descape(), @"\b");
            }
            else
            {
                TempString = "@" + TempString;
            }

            Key = Key.Trim(' ');
            if (CurrentSection.EntryDict.ContainsKey(Key))
            {
                CurrentSection.EntryDict[Key] = TempString;
            }
            else
            {
                CurrentSection.Entries.Add(Key);
                CurrentSection.EntryDict.Add(Key, TempString);
            }
        }
        public void WriteValue(string SectionName, string Key, string Value)
        {
            if (string.IsNullOrEmpty(Value) || !(Value.Contains(@"\") || Value.Contains("/")))
            {
                WriteValue(SectionName, Key, Value, true);
            }
            else
            {
                WriteValue(SectionName, Key, Value, false);
            }
        }
        public void WriteValue(string SectionName, string Key, string[] Value)
        {
            if (Value is null)
                return;
            WriteValue(SectionName, Key, string.Join(",", Value));
        }
        public void WriteValue(string SectionName, string Key, string[,] Value)
        {
            if (Value is null)
                return;
            WriteValue(SectionName, Key, string.Join(Environment.NewLine, (from y in Enumerable.Range(0, Value.GetLength(1))
                                                                           select string.Join(",", (from x in Enumerable.Range(0, Value.GetLength(0))
                                                                                                    select Value[x, y]).ToArray())).ToArray()), false);
        }
        public bool WriteToFile(string Comment = "")
        {
            var Lines = new List<string>();
            foreach (var SectionName in Sections)
            {
                var CurrentSection = SectionDict[SectionName];
                Lines.Add("[" + SectionName + "]");
                foreach (var Key in CurrentSection.Entries)
                {
                    string Temp = CurrentSection.EntryDict[Key];
                    if (Temp.StartsWith("@") && Temp.Contains(ControlChars.Lf))
                    {
                        Lines.Add(Key + " = " + "{" + Environment.NewLine + Temp.Remove(0, 1) + Environment.NewLine + "}");
                    }
                    else
                    {
                        Lines.Add(Key + " = " + Temp);
                    }
                }
                Lines.Add("");
            }

            string StringToWrite = string.Join(Environment.NewLine, Lines.ToArray());

            if (!string.IsNullOrEmpty(Comment))
            {
                string TempString = Comment;
                TempString = TempString.Replace(@"\a".Descape(), @"\a");
                TempString = TempString.Replace(@"\b".Descape(), @"\b");
                TempString = TempString.Replace(@"\t".Descape(), @"\t");
                TempString = TempString.Replace(@"\v".Descape(), @"\v");
                TempString = TempString.Replace(@"\f".Descape(), @"\f");
                TempString = TempString.Replace(@"\b".Descape(), @"\b");
                StringToWrite = Comment + Environment.NewLine + StringToWrite;
            }

            var Encoding = this.Encoding;
            if (Encoding is null)
                Encoding = TextEncoding.TextEncoding.WritingDefault;
            try
            {
                string dir = FileNameHandling.GetFileDirectory(FilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                Txt.WriteFile(FilePath, Encoding, StringToWrite);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public void SetFileName(string Path)
        {
            FilePath = Path;
        }
    }

    /// <summary>INI本地化类</summary>
    /// <remarks>
    /// 本类管理以INI形式存储的本地化字符串
    /// 需要的本地化文件格式样例如下：
    /// 
    /// Program.en.ini
    /// [en]
    /// Text1 = Hello
    /// Text2 = Welldone
    /// Text3 = Color
    /// 
    /// [en-GB]
    /// Text3 = Colour
    /// 
    /// Program.zh-CN.ini
    /// [zh-CN]
    /// Text1 = 你好
    /// Text2 = 棒极了
    /// Text3 = 颜色
    /// 
    /// </remarks>
    public class IniLocalization
    {
        protected Ini LanRes;
        protected string LanFull;
        public string LanguageIndentiySignFull
        {
            get
            {
                return LanFull;
            }
        }
        protected string LanParent;
        public string LanguageIndentiySignParent
        {
            get
            {
                return LanParent;
            }
        }
        protected string LanNative;
        public string LanguageIndentiySignNative
        {
            get
            {
                return LanNative;
            }
        }
        protected string LanDefault;
        public string DefaultLanguageIndentiySign
        {
            get
            {
                return LanDefault;
            }
        }
        public IniLocalization(string LanguageFileMainName, string Language, string DefaultLanguage = "en", System.Text.Encoding Encoding = null)
        {
            LanFull = Language;
            LanDefault = DefaultLanguage;

            if (string.IsNullOrEmpty(LanFull))
                LanFull = System.Globalization.CultureInfo.InstalledUICulture.Name;
            int Index = LanFull.IndexOf("-");
            if (Index < 0)
            {
                LanParent = LanFull;
                LanNative = "";
                LanParent = LanParent.ToLower();
                LanFull = LanParent;
            }
            else
            {
                LanParent = LanFull.Substring(0, Index);
                LanNative = LanFull.Substring(Index + 1);
                LanParent = LanParent.ToLower();
                LanNative = LanNative.ToUpper();
                LanFull = LanParent + "-" + LanNative;
            }

            LanRes = new Ini();
            LanRes.ReadFromFile(LanguageFileMainName + "." + DefaultLanguage + ".ini", Encoding);
            LanRes.ReadFromFile(LanguageFileMainName + "." + LanParent + ".ini", Encoding);
            LanRes.ReadFromFile(LanguageFileMainName + "." + LanFull + ".ini", Encoding);
        }
        public void ReadValue(string Key, ref string Value)
        {
            LanRes.ReadValue(LanDefault, Key, ref Value);
            LanRes.ReadValue(LanParent, Key, ref Value);
            LanRes.ReadValue(LanFull, Key, ref Value);
        }
    }
}