'==========================================================================
'
'  File:        ExceptionHandler.vb
'  Location:    Firefly.Core <Visual Basic .Net>
'  Description: 异常字符化与错误记录
'  Version:     2010.02.07.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports System.Diagnostics
Imports System.Reflection
Imports System.Threading
Imports Microsoft.VisualBasic

Public NotInheritable Class ExceptionHandler
    Private Sub New()
    End Sub

    Public Shared Sub PopInfo(ByVal s As String)
        Microsoft.VisualBasic.MsgBox(s, MsgBoxStyle.Information Or MsgBoxStyle.OkOnly, My.Application.Info.Description)
    End Sub
    Public Shared Sub PopupException(ByVal ex As Exception)
        Dim Info As String = GetExceptionInfo(ex, New StackTrace(2, True))
        Dim r As MsgBoxResult = Microsoft.VisualBasic.MsgBox(DebugTip & Environment.NewLine & Environment.NewLine & Info, MsgBoxStyle.Critical Or MsgBoxStyle.YesNo, My.Application.Info.ProductName)
        If r = MsgBoxResult.Yes Then
            My.Computer.Clipboard.SetText(Info)
        End If
    End Sub
    Public Shared Sub PopupException(ByVal ex As Exception, ByVal ParentTrace As StackTrace)
        Dim Info As String = GetExceptionInfo(ex, ParentTrace)
        Dim r As MsgBoxResult = Microsoft.VisualBasic.MsgBox(DebugTip & Environment.NewLine & Environment.NewLine & Info, MsgBoxStyle.Critical Or MsgBoxStyle.YesNo, My.Application.Info.ProductName)
        If r = MsgBoxResult.Yes Then
            My.Computer.Clipboard.SetText(Info)
        End If
    End Sub
    Public Shared Sub PopupException(ByVal ex As Exception, ByVal DebugTip As String, ByVal Title As String)
        Dim Info As String = GetExceptionInfo(ex, New StackTrace(2, True))
        Dim r As MsgBoxResult = Microsoft.VisualBasic.MsgBox(DebugTip & Environment.NewLine & Environment.NewLine & Info, MsgBoxStyle.Critical Or MsgBoxStyle.YesNo, Title)
        If r = MsgBoxResult.Yes Then
            My.Computer.Clipboard.SetText(Info)
        End If
    End Sub
    Public Shared Sub PopupException(ByVal ex As Exception, ByVal ParentTrace As StackTrace, ByVal DebugTip As String, ByVal Title As String)
        Dim Info As String = GetExceptionInfo(ex, ParentTrace)
        Dim r As MsgBoxResult = Microsoft.VisualBasic.MsgBox(DebugTip & Environment.NewLine & Environment.NewLine & Info, MsgBoxStyle.Critical Or MsgBoxStyle.YesNo, Title)
        If r = MsgBoxResult.Yes Then
            My.Computer.Clipboard.SetText(Info)
        End If
    End Sub
    Private Shared Sub GetExceptionInfoWithoutParent(ByVal ex As Exception, ByVal msg As StringBuilder, ByVal Level As Integer)
        If ex.InnerException IsNot Nothing AndAlso ex.InnerException IsNot ex AndAlso Level < 3 Then
            GetExceptionInfoWithoutParent(ex.InnerException, msg, Level + 1)
            msg.AppendLine(New String("-"c, 20))
        End If
        msg.AppendLine(String.Format("{0}:" & System.Environment.NewLine & "{1}", ex.GetType.FullName, ex.Message))
        msg.AppendLine()
        msg.Append(GetStackTrace(New StackTrace(ex, True)))
    End Sub
    Public Shared Function GetExceptionInfo(ByVal ex As Exception) As String
        Return GetExceptionInfo(ex, New StackTrace(2, True))
    End Function
    Public Shared Function GetExceptionInfo(ByVal ex As Exception, ByVal ParentTrace As StackTrace) As String
        Dim msg As New StringBuilder
        GetExceptionInfoWithoutParent(ex, msg, 0)
        If ParentTrace IsNot Nothing Then msg.AppendLine(GetStackTrace(ParentTrace))
        Return msg.ToString
    End Function
    Public Shared Function GetStackTrace(ByVal ex As Exception, Optional ByVal ParentTrace As StackTrace = Nothing) As String
        Return GetStackTrace(New StackTrace(ex, True)) & GetStackTrace(ParentTrace)
    End Function
    Public Shared Function GetStackTrace(ByVal Trace As StackTrace) As String
        If Trace Is Nothing Then Return Nothing
        If Trace.FrameCount = 0 Then Return ""
        Dim sb As StringBuilder = New StringBuilder()
        For Each Frame In Trace.GetFrames
            sb.AppendLine(StackFrameToString(Frame))
        Next
        Return sb.ToString()
    End Function
    Public Shared Function StackFrameToString(ByVal Frame As StackFrame) As String
        Dim mi As MemberInfo = Frame.GetMethod()
        Dim Params As New List(Of String)
        For Each param In DirectCast(mi, MethodBase).GetParameters()
            If param.Name = "" Then
                Params.Add(param.ParameterType.Name)
            Else
                Params.Add(param.ParameterType.Name & " " & param.Name)
            End If
        Next

        Dim Pos As New List(Of String)
        If Frame.GetFileLineNumber > 0 Then Pos.Add(String.Format("Line {0}", Frame.GetFileLineNumber))
        If Frame.GetFileColumnNumber > 0 Then Pos.Add(String.Format("Column {0}", Frame.GetFileColumnNumber))
        If Frame.GetILOffset <> StackFrame.OFFSET_UNKNOWN Then Pos.Add(String.Format("IL {0:X4}", Frame.GetILOffset))
        If Frame.GetNativeOffset <> StackFrame.OFFSET_UNKNOWN Then Pos.Add(String.Format("N {0:X6}", Frame.GetNativeOffset))

        If Frame.GetFileName <> "" Then
            Return String.Format("{0}.{1}({2}) {3} : {4}", mi.DeclaringType.FullName, mi.Name, String.Join(", ", Params.ToArray), Frame.GetFileName, String.Join(", ", Pos.ToArray))
        Else
            Return String.Format("{0}.{1}({2}) : {3}", mi.DeclaringType.FullName, mi.Name, String.Join(", ", Params.ToArray), String.Join(", ", Pos.ToArray))
        End If
    End Function

    Public Shared DebugTip As String = "程序出现错误" & Environment.NewLine & "是否将错误信息复制到剪贴板？"
    Public Shared LogPath As String = My.Application.Info.AssemblyName & ".log"
    Public Shared CurrentFilePath As String = ""
    Public Shared CurrentSection As String = ""
    Private Shared sw As TextWriter
    Private Shared Sub WriteLineDirect(ByVal s As String)
        System.Diagnostics.Debug.WriteLine(s)
        If sw Is Nothing Then sw = TextWriter.Synchronized(Texting.Txt.CreateTextWriter(LogPath))
        sw.WriteLine(s)
        sw.Flush()
    End Sub
    Public Shared Sub WriteLine(ByVal s As String)
        s = GetIndexedText(s)
        WriteLineDirect(s)
    End Sub
    Public Shared Sub WriteWarning(ByVal s As String)
        s = GetIndexedText(s)
        WriteLineDirect(s)
    End Sub
    Public Shared Sub WriteWarning(ByVal ex As Exception)
        WriteWarning(ex.ToString)
    End Sub
    Public Shared Sub WriteError(ByVal s As String)
        s = GetIndexedText(s)
        WriteLineDirect(s)
    End Sub
    Public Shared Sub WriteError(ByVal ex As Exception)
        WriteError(GetExceptionInfo(ex, New StackTrace(2, True)))
    End Sub
    Private Shared Function GetIndexedText(ByVal s As String) As String
        If CurrentFilePath = "" AndAlso CurrentSection = "" Then
            Return s
        ElseIf CurrentSection = "" Then
            Return String.Format("{0} {1}", CurrentFilePath, s)
        Else
            Return String.Format("{0}:{1} {2}", CurrentFilePath, CurrentSection, s)
        End If
    End Function
End Class
