// ==========================================================================
// 
// File:        UniHanDatabase.vb
// Location:    Firefly.Texting <Visual Basic .Net>
// Description: UniHan数据库
// Version:     2009.10.20.
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

namespace Firefly
{

    /// <summary>
/// 本类用于遍历UniHan数据库。
/// </summary>
/// <remarks>
/// UniHan数据库可从如下地址获取。
/// ftp://ftp.unicode.org/Public/UNIDATA/Unihan.zip
/// 
/// Unicode委员会的技术报告可从如下地址获取。
/// Unicode Standard Annex #38
/// http://www.unicode.org/reports/tr38/
/// </remarks>
    public class UniHanDatabase
    {
        private Dictionary<Char32, List<UniHanDouble>> CharDict = new Dictionary<Char32, List<UniHanDouble>>();

        public UniHanDatabase()
        {
        }

        public void Load(string Path, Predicate<UniHanTriple> TriplePredicate)
        {
            var r = new Regex(@"^U\+(?<Unicode>[0-9A-F]{4,5})\t(?<FieldType>[0-9A-Za-z_]+)\t(?<Value>.*)$", RegexOptions.ExplicitCapture);
            using (var sr = Txt.CreateTextReader(Path, TextEncoding.TextEncoding.UTF8))
            {
                while (!sr.EndOfStream)
                {
                    string Line = sr.ReadLine().TrimStart(' ');
                    if (Line.StartsWith("#"))
                        continue;
                    if (string.IsNullOrEmpty(Line))
                        continue;

                    var m = r.Match(Line);
                    if (!m.Success)
                        throw new InvalidDataException("{0}: {1}".Formats(Path, Line));

                    var Unicode = new Char32(int.Parse(m.Result("${Unicode}"), System.Globalization.NumberStyles.HexNumber));
                    string FieldType = m.Result("${FieldType}");
                    string Value = m.Result("${Value}");

                    var t = new UniHanTriple(Unicode, FieldType, Value);
                    if (TriplePredicate(t))
                    {
                        if (CharDict.ContainsKey(Unicode))
                        {
                            CharDict[Unicode].Add(new UniHanDouble(FieldType, Value));
                        }
                        else
                        {
                            CharDict.Add(Unicode, new List<UniHanDouble>(new UniHanDouble[] { new UniHanDouble(FieldType, Value) }));
                        }
                    }
                }
            }
        }
        public void Load(string Path, string FirstFieldType, params string[] FieldTypes)
        {
            var r = new Regex(@"^U\+(?<Unicode>2?[0-9A-F]{4})\t(?<FieldType>[0-9A-Za-z_]+)\t(?<Value>.*)$", RegexOptions.ExplicitCapture);
            var ft = new HashSet<string>();
            ft.Add(FirstFieldType);
            foreach (var f in FieldTypes)
                ft.Add(f);
            using (var sr = Txt.CreateTextReader(Path, TextEncoding.TextEncoding.UTF8))
            {
                while (!sr.EndOfStream)
                {
                    string Line = sr.ReadLine().TrimStart(' ');
                    if (Line.StartsWith("#"))
                        continue;
                    if (string.IsNullOrEmpty(Line))
                        continue;

                    var m = r.Match(Line);
                    if (!m.Success)
                        throw new InvalidDataException("{0}: {1}".Formats(Path, Line));

                    string FieldType = m.Result("${FieldType}");
                    if (!ft.Contains(FieldType))
                        continue;

                    var Unicode = new Char32(int.Parse(m.Result("${Unicode}"), System.Globalization.NumberStyles.HexNumber));
                    string Value = m.Result("${Value}");

                    if (CharDict.ContainsKey(Unicode))
                    {
                        var l = CharDict[Unicode];
                        l.Add(new UniHanDouble(FieldType, Value));
                    }
                    else
                    {
                        var l = new List<UniHanDouble>();
                        l.Add(new UniHanDouble(FieldType, Value));
                        CharDict.Add(Unicode, l);
                    }
                }
            }

        }
        public void LoadAll(string Path)
        {
            var r = new Regex(@"U+(?<Unicode>2?[0-9A-F]{4})\t(?<FieldType>[0-9A-Za-z]+)\t(?<Value>.*)", RegexOptions.ExplicitCapture);
            using (var sr = Txt.CreateTextReader(Path, TextEncoding.TextEncoding.UTF8))
            {
                while (!sr.EndOfStream)
                {
                    string Line = sr.ReadLine().TrimStart(' ');
                    if (Line.StartsWith("#"))
                        continue;
                    if (string.IsNullOrEmpty(Line))
                        continue;

                    var m = r.Match(Line);
                    if (!m.Success)
                        throw new InvalidDataException("{0}: {1}".Formats(Path, Line));

                    var Unicode = new Char32(int.Parse(m.Result("${Unicode}"), System.Globalization.NumberStyles.HexNumber));
                    string FieldType = m.Result("${FieldType}");
                    string Value = m.Result("${Value}");

                    if (CharDict.ContainsKey(Unicode))
                    {
                        var l = CharDict[Unicode];
                        l.Add(new UniHanDouble(FieldType, Value));
                    }
                    else
                    {
                        var l = new List<UniHanDouble>();
                        l.Add(new UniHanDouble(FieldType, Value));
                        CharDict.Add(Unicode, l);
                    }
                }
            }
        }
        public IEnumerable<UniHanChar> GetChars()
        {
            return from p in CharDict
                   select new UniHanChar(p.Key, p.Value);
        }
        public Dictionary<Char32, IEnumerable<UniHanDouble>> GetDoubleDict()
        {
            return (from p in CharDict
                    select new { p.Key, Value = p.Value.AsEnumerable() }).ToDictionary(p => p.Key, p => p.Value);
        }
        public IEnumerable<UniHanTriple> GetTriples()
        {
            var Triples = new List<UniHanTriple>();
            foreach (var p in CharDict)
            {
                foreach (var v in p.Value)
                    Triples.Add(new UniHanTriple(p.Key, v.FieldType, v.Value));
            }
            return Triples;
        }

        public struct UniHanDouble
        {
            private string FieldTypeValue;
            private string ValueValue;
            public string FieldType
            {
                get
                {
                    return FieldTypeValue;
                }
            }
            public string Value
            {
                get
                {
                    return ValueValue;
                }
            }

            public UniHanDouble(string FieldType, string Value)
            {
                FieldTypeValue = FieldType;
                ValueValue = Value;
            }
        }

        public struct UniHanTriple
        {
            private Char32 UnicodeValue;
            private string FieldTypeValue;
            private string ValueValue;
            public Char32 Unicode
            {
                get
                {
                    return UnicodeValue;
                }
            }
            public string FieldType
            {
                get
                {
                    return FieldTypeValue;
                }
            }
            public string Value
            {
                get
                {
                    return ValueValue;
                }
            }

            public UniHanTriple(Char32 Unicode, string FieldType, string Value)
            {
                UnicodeValue = Unicode;
                FieldTypeValue = FieldType;
                ValueValue = Value;
            }
        }

        public class UniHanChar
        {
            private Char32 UnicodeValue;
            private Dictionary<string, string> Dict = new Dictionary<string, string>();
            public UniHanChar(Char32 Unicode, IEnumerable<UniHanDouble> Fields)
            {
                UnicodeValue = Unicode;
                Dict = Fields.ToDictionary(d => d.FieldType, d => d.Value);
            }
            public Char32 Unicode
            {
                get
                {
                    return UnicodeValue;
                }
            }
            public string this[string FieldName]
            {
                get
                {
                    return Dict[FieldName];
                }
                set
                {
                    if (!Dict.ContainsKey(FieldName))
                    {
                        Dict.Add(FieldName, value);
                    }
                    else
                    {
                        Dict[FieldName] = value;
                    }
                }
            }
            public bool get_HasField(string FieldName)
            {
                return Dict.ContainsKey(FieldName);
            }
            public void AddField(string FieldName, string Value)
            {
                Dict.Add(FieldName, Value);
            }
            public void RemoveField(string FieldName)
            {
                Dict.Remove(FieldName);
            }
            public IEnumerable<UniHanDouble> GetFields()
            {
                return from p in Dict
                       select new UniHanDouble(p.Key, p.Value);
            }
        }
    }
}