// ==========================================================================
// 
// File:        ExceptionHandler.vb
// Location:    Firefly.Core <Visual Basic .Net>
// Description: 异常字符化与错误记录
// Version:     2010.02.07.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualBasic;

namespace Firefly
{

    public sealed class ExceptionHandler
    {
        private ExceptionHandler()
        {
        }

        public static void PopInfo(string s)
        {
            Interaction.MsgBox(s, MsgBoxStyle.Information | MsgBoxStyle.OkOnly, My.MyProject.Application.Info.Description);
        }
        public static void PopupException(Exception ex)
        {
            string Info = GetExceptionInfo(ex, new StackTrace(2, true));
            var r = Interaction.MsgBox(DebugTip + Environment.NewLine + Environment.NewLine + Info, MsgBoxStyle.Critical | MsgBoxStyle.YesNo, My.MyProject.Application.Info.ProductName);
            if (r == MsgBoxResult.Yes)
            {
                My.MyProject.Computer.Clipboard.SetText(Info);
            }
        }
        public static void PopupException(Exception ex, StackTrace ParentTrace)
        {
            string Info = GetExceptionInfo(ex, ParentTrace);
            var r = Interaction.MsgBox(DebugTip + Environment.NewLine + Environment.NewLine + Info, MsgBoxStyle.Critical | MsgBoxStyle.YesNo, My.MyProject.Application.Info.ProductName);
            if (r == MsgBoxResult.Yes)
            {
                My.MyProject.Computer.Clipboard.SetText(Info);
            }
        }
        public static void PopupException(Exception ex, string DebugTip, string Title)
        {
            string Info = GetExceptionInfo(ex, new StackTrace(2, true));
            var r = Interaction.MsgBox(DebugTip + Environment.NewLine + Environment.NewLine + Info, MsgBoxStyle.Critical | MsgBoxStyle.YesNo, Title);
            if (r == MsgBoxResult.Yes)
            {
                My.MyProject.Computer.Clipboard.SetText(Info);
            }
        }
        public static void PopupException(Exception ex, StackTrace ParentTrace, string DebugTip, string Title)
        {
            string Info = GetExceptionInfo(ex, ParentTrace);
            var r = Interaction.MsgBox(DebugTip + Environment.NewLine + Environment.NewLine + Info, MsgBoxStyle.Critical | MsgBoxStyle.YesNo, Title);
            if (r == MsgBoxResult.Yes)
            {
                My.MyProject.Computer.Clipboard.SetText(Info);
            }
        }
        private static void GetExceptionInfoWithoutParent(Exception ex, StringBuilder msg, int Level)
        {
            if (ex.InnerException is not null && !ReferenceEquals(ex.InnerException, ex) && Level < 3)
            {
                GetExceptionInfoWithoutParent(ex.InnerException, msg, Level + 1);
                msg.AppendLine(new string('-', 20));
            }
            msg.AppendLine(string.Format("{0}:" + Environment.NewLine + "{1}", ex.GetType().FullName, ex.Message));
            msg.AppendLine();
            msg.Append(GetStackTrace(new StackTrace(ex, true)));
        }
        public static string GetExceptionInfo(Exception ex)
        {
            return GetExceptionInfo(ex, new StackTrace(2, true));
        }
        public static string GetExceptionInfo(Exception ex, StackTrace ParentTrace)
        {
            var msg = new StringBuilder();
            GetExceptionInfoWithoutParent(ex, msg, 0);
            if (ParentTrace is not null)
                msg.AppendLine(GetStackTrace(ParentTrace));
            return msg.ToString();
        }
        public static string GetStackTrace(Exception ex, StackTrace ParentTrace = null)
        {
            return GetStackTrace(new StackTrace(ex, true)) + GetStackTrace(ParentTrace);
        }
        public static string GetStackTrace(StackTrace Trace)
        {
            if (Trace is null)
                return null;
            if (Trace.FrameCount == 0)
                return "";
            var sb = new StringBuilder();
            foreach (var Frame in Trace.GetFrames())
                sb.AppendLine(StackFrameToString(Frame));
            return sb.ToString();
        }
        public static string StackFrameToString(StackFrame Frame)
        {
            MemberInfo mi = Frame.GetMethod();
            var Params = new List<string>();
            foreach (var param in ((MethodBase)mi).GetParameters())
            {
                if (string.IsNullOrEmpty(param.Name))
                {
                    Params.Add(param.ParameterType.Name);
                }
                else
                {
                    Params.Add(param.ParameterType.Name + " " + param.Name);
                }
            }

            var Pos = new List<string>();
            if (Frame.GetFileLineNumber() > 0)
                Pos.Add(string.Format("Line {0}", Frame.GetFileLineNumber()));
            if (Frame.GetFileColumnNumber() > 0)
                Pos.Add(string.Format("Column {0}", Frame.GetFileColumnNumber()));
            if (Frame.GetILOffset() != StackFrame.OFFSET_UNKNOWN)
                Pos.Add(string.Format("IL {0:X4}", Frame.GetILOffset()));
            if (Frame.GetNativeOffset() != StackFrame.OFFSET_UNKNOWN)
                Pos.Add(string.Format("N {0:X6}", Frame.GetNativeOffset()));

            if (!string.IsNullOrEmpty(Frame.GetFileName()))
            {
                return string.Format("{0}.{1}({2}) {3} : {4}", mi.DeclaringType.FullName, mi.Name, string.Join(", ", Params.ToArray()), Frame.GetFileName(), string.Join(", ", Pos.ToArray()));
            }
            else
            {
                return string.Format("{0}.{1}({2}) : {3}", mi.DeclaringType.FullName, mi.Name, string.Join(", ", Params.ToArray()), string.Join(", ", Pos.ToArray()));
            }
        }

        public static string DebugTip = "程序出现错误" + Environment.NewLine + "是否将错误信息复制到剪贴板？";
        public static string LogPath = My.MyProject.Application.Info.AssemblyName + ".log";
        public static string CurrentFilePath = "";
        public static string CurrentSection = "";
        private static TextWriter sw;
        private static void WriteLineDirect(string s)
        {
            Debug.WriteLine(s);
            if (sw is null)
                sw = TextWriter.Synchronized(Texting.Txt.CreateTextWriter(LogPath));
            sw.WriteLine(s);
            sw.Flush();
        }
        public static void WriteLine(string s)
        {
            s = GetIndexedText(s);
            WriteLineDirect(s);
        }
        public static void WriteWarning(string s)
        {
            s = GetIndexedText(s);
            WriteLineDirect(s);
        }
        public static void WriteWarning(Exception ex)
        {
            WriteWarning(ex.ToString());
        }
        public static void WriteError(string s)
        {
            s = GetIndexedText(s);
            WriteLineDirect(s);
        }
        public static void WriteError(Exception ex)
        {
            WriteError(GetExceptionInfo(ex, new StackTrace(2, true)));
        }
        private static string GetIndexedText(string s)
        {
            if (string.IsNullOrEmpty(CurrentFilePath) && string.IsNullOrEmpty(CurrentSection))
            {
                return s;
            }
            else if (string.IsNullOrEmpty(CurrentSection))
            {
                return string.Format("{0} {1}", CurrentFilePath, s);
            }
            else
            {
                return string.Format("{0}:{1} {2}", CurrentFilePath, CurrentSection, s);
            }
        }
    }
}