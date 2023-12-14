// ==========================================================================
// 
// File:        FileNameHandling.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 文件名操作函数模块
// Version:     2009.11.29.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Firefly
{

    public static class FileNameHandling
    {

        /// <summary>获得文件名</summary>
        public static string GetFileName(string FilePath)
        {
            if (string.IsNullOrEmpty(FilePath))
                return "";
            var NameS = default(int);
            int NameS2 = FilePath.Replace("/", @"\").IndexOf('\\', NameS);
            while (NameS2 != -1)
            {
                NameS = NameS2 + 1;
                NameS2 = FilePath.Replace("/", @"\").IndexOf('\\', NameS);
            }
            return FilePath.Substring(NameS);
        }
        /// <summary>获得主文件名</summary>
        public static string GetMainFileName(string FilePath)
        {
            if (string.IsNullOrEmpty(FilePath))
                return "";
            var NameS = default(int);
            int NameS2 = FilePath.Replace("/", @"\").IndexOf('\\', NameS);
            while (NameS2 != -1)
            {
                NameS = NameS2 + 1;
                NameS2 = FilePath.Replace("/", @"\").IndexOf('\\', NameS);
            }
            int NameE = FilePath.Length - 1;
            int NameE2 = FilePath.LastIndexOf('.', NameE);
            if (NameE2 != -1)
            {
                NameE = NameE2 - 1;
            }
            return FilePath.Substring(NameS, NameE - NameS + 1);
        }
        /// <summary>获得扩展文件名</summary>
        public static string GetExtendedFileName(string FilePath)
        {
            if (string.IsNullOrEmpty(FilePath))
                return "";
            if (!FilePath.Contains("."))
                return "";
            return FilePath.Substring(FilePath.LastIndexOf(".") + 1);
        }
        /// <summary>获得文件路径中的文件夹部分</summary>
        public static string GetFileDirectory(string FilePath)
        {
            if (string.IsNullOrEmpty(FilePath))
                return "";
            var NameE = default(int);
            var NameE2 = default(int);
            while (NameE2 != -1)
            {
                NameE = NameE2 + 1;
                NameE2 = FilePath.Replace("/", @"\").IndexOf('\\', NameE);
            }
            return FilePath.Substring(0, NameE - 1);
        }
        /// <summary>获得相对路径</summary>
        public static string GetRelativePath(string FilePath, string BaseDirectory)
        {
            if (string.IsNullOrEmpty(FilePath) || string.IsNullOrEmpty(BaseDirectory))
                return FilePath;
            string a = FilePath.TrimEnd('\\').TrimEnd('/');
            string b = BaseDirectory.TrimEnd('\\').TrimEnd('/');
            string c;
            string d;

            c = PopFirstDir(ref a);
            d = PopFirstDir(ref b);
            if (CultureInfo.CurrentCulture.CompareInfo.Compare(c ?? "", d ?? "", CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) != 0)
                return FilePath;
            while (CultureInfo.CurrentCulture.CompareInfo.Compare(c ?? "", d ?? "", CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0)
            {
                if (string.IsNullOrEmpty(c))
                    return ".";
                c = PopFirstDir(ref a);
                d = PopFirstDir(ref b);
            }

            a = (c + @"\" + a).TrimEnd('\\').TrimEnd('/');
            b = (d + @"\" + b).TrimEnd('\\').TrimEnd('/');

            while (!string.IsNullOrEmpty(PopFirstDir(ref b)))
                a = @"..\" + a;
            return a.Replace(@"\", Conversions.ToString(System.IO.Path.DirectorySeparatorChar));
        }
        /// <summary>获得精简路径</summary>
        public static string GetReducedPath(string Path)
        {
            var l = new Stack<string>();
            if (!string.IsNullOrEmpty(Path))
            {
                foreach (var d in Regex.Split(Path, @"\\|/"))
                {
                    if (CultureInfo.CurrentCulture.CompareInfo.Compare(d, ".", CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0)
                        continue;
                    if (CultureInfo.CurrentCulture.CompareInfo.Compare(d, "..", CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0)
                    {
                        if (l.Count > 0)
                        {
                            string p = l.Pop();
                            if (CultureInfo.CurrentCulture.CompareInfo.Compare(p, "..", CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0)
                            {
                                l.Push(p);
                                l.Push(d);
                            }
                        }
                        else
                        {
                            l.Push(d);
                        }
                        continue;
                    }
                    if (d.Contains(":"))
                        l.Clear();
                    l.Push(d);
                }
            }
            return string.Join(Conversions.ToString(System.IO.Path.DirectorySeparatorChar), l.Reverse().ToArray());
        }
        /// <summary>获得没有结尾分隔符的文件夹路径</summary>
        public static string GetDirectoryPathWithoutTailingSeparator(string DirectoryPath)
        {
            if (string.IsNullOrEmpty(DirectoryPath))
                return "";
            return DirectoryPath.TrimEnd('\\').TrimEnd('/');
        }
        /// <summary>获得有结尾分隔符的文件夹路径，如果文件夹为空，则返回空</summary>
        public static string GetDirectoryPathWithTailingSeparator(string DirectoryPath)
        {
            string d = GetDirectoryPathWithoutTailingSeparator(DirectoryPath);
            if (string.IsNullOrEmpty(d))
                return "";
            return d + System.IO.Path.DirectorySeparatorChar;
        }
        /// <summary>获得绝对路径</summary>
        public static string GetAbsolutePath(string FilePath, string BaseDirectory)
        {
            BaseDirectory = GetDirectoryPathWithoutTailingSeparator(BaseDirectory);
            if (!string.IsNullOrEmpty(FilePath))
                FilePath = FilePath.TrimStart('\\').TrimStart('/');
            var s = new Stack<string>();
            if (!string.IsNullOrEmpty(BaseDirectory))
            {
                foreach (var d in Regex.Split(BaseDirectory, @"\\|/"))
                {
                    if (CultureInfo.CurrentCulture.CompareInfo.Compare(d, ".", CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0)
                        continue;
                    if (CultureInfo.CurrentCulture.CompareInfo.Compare(d, "..", CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0)
                    {
                        if (s.Count > 0)
                        {
                            string p = s.Pop();
                            if (CultureInfo.CurrentCulture.CompareInfo.Compare(p, "..", CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0)
                            {
                                s.Push(p);
                                s.Push(d);
                            }
                        }
                        else
                        {
                            s.Push(d);
                        }
                        continue;
                    }
                    if (d.Contains(":"))
                        s.Clear();
                    s.Push(d);
                }
            }
            if (!string.IsNullOrEmpty(FilePath))
            {
                foreach (var d in Regex.Split(FilePath, @"\\|/"))
                {
                    if (CultureInfo.CurrentCulture.CompareInfo.Compare(d, ".", CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0)
                        continue;
                    if (CultureInfo.CurrentCulture.CompareInfo.Compare(d, "..", CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0)
                    {
                        if (s.Count > 0)
                        {
                            string p = s.Pop();
                            if (CultureInfo.CurrentCulture.CompareInfo.Compare(p, "..", CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0)
                            {
                                s.Push(p);
                                s.Push(d);
                            }
                        }
                        else
                        {
                            s.Push(d);
                        }
                        continue;
                    }
                    if (d.Contains(":"))
                        s.Clear();
                    s.Push(d);
                }
            }
            return string.Join(Conversions.ToString(System.IO.Path.DirectorySeparatorChar), s.Reverse().ToArray());
        }

        /// <summary>取出路径的第一个文件夹名</summary>
        public static string PopFirstDir(ref string Path)
        {
            string ret;
            if (string.IsNullOrEmpty(Path))
                return "";
            var NameS = default(int);
            NameS = Path.Replace("/", @"\").IndexOf('\\', NameS);
            if (NameS < 0)
            {
                ret = Path;
                Path = "";
                return ret;
            }
            else
            {
                ret = Path.Substring(0, NameS);
                Path = Path.Substring(NameS + 1);
                return ret;
            }
        }
        /// <summary>构成路径</summary>
        public static string GetPath(string Directory, string FileName)
        {
            if (string.IsNullOrEmpty(Directory))
                return FileName;
            Directory = Directory.TrimEnd('\\').TrimEnd('/');
            return (Directory + @"\" + FileName).Replace(@"\", Conversions.ToString(System.IO.Path.DirectorySeparatorChar));
        }
        /// <summary>更换扩展名</summary>
        public static string ChangeExtension(string FilePath, string Extension)
        {
            return System.IO.Path.ChangeExtension(FilePath, Extension);
        }

        /// <summary>判断文件名是否符合通配符</summary>
        public static bool IsMatchFileMask(string FileName, string Mask)
        {
            return LikeOperator.LikeString(FileName, Mask, CompareMethod.Text);
        }
    }
}