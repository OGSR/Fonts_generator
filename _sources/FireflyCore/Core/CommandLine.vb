'==========================================================================
'
'  File:        CommandLine.vb
'  Location:    Firefly.Core <Visual Basic .Net>
'  Description: 控制台
'  Version:     2010.01.31.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text.RegularExpressions

Public NotInheritable Class CommandLine
    Private Sub New()
    End Sub

    Public Class CommandLineOption
        Public Name As String
        Public Arguments As String()
    End Class

    Public Class CommandLineArguments
        Public Arguments As String()
        Public Options As CommandLineOption()
    End Class

    Private Shared Function DescapeQuota(ByVal s As String) As String
        If s = "" Then Return ""
        If s.StartsWith("""") AndAlso s.EndsWith("""") Then
            Return s.Substring(1, s.Length - 2).Replace("""""", """")
        Else
            Return s
        End If
    End Function

    Public Shared Function GetCmdLine() As CommandLineArguments
        Dim argv As New List(Of String)
        Dim SuppressedFirst As Boolean = False
        Dim NextStart = 0
        Dim CmdLine = Environment.CommandLine
        For Each arg As Match In Regex.Matches(CmdLine, "(""[^""]*""|([^"" ])+)+", RegexOptions.ExplicitCapture)
            If arg.Index <> NextStart Then
                If Not CmdLine.Substring(NextStart, arg.Index - NextStart).All(Function(c) c = " ") Then Throw New InvalidOperationException
            End If
            NextStart = arg.Index + arg.Length
            If Not SuppressedFirst Then
                SuppressedFirst = True
                Continue For
            End If
            If arg.Success Then
                Dim m = arg.Value
                argv.Add(DescapeQuota(m))
            Else
                Throw New InvalidOperationException
            End If
        Next
        If CmdLine.Length <> NextStart Then
            If Not CmdLine.Substring(NextStart, CmdLine.Length - NextStart).All(Function(c) c = " ") Then Throw New InvalidOperationException
        End If

        Dim Arguments As New List(Of String)
        Dim Options As New List(Of CommandLineOption)

        For Each arg In argv
            If arg.StartsWith("/") Then
                Dim OptionLine = arg.Substring(1)
                Dim Name As String
                Dim ParameterLine As String
                Dim Index = OptionLine.IndexOf(":")
                If Index >= 0 Then
                    Name = DescapeQuota(OptionLine.Substring(0, Index))
                    ParameterLine = OptionLine.Substring(Index + 1)
                Else
                    Name = DescapeQuota(OptionLine)
                    ParameterLine = ""
                End If
                Options.Add(New CommandLineOption With {.Name = Name, .Arguments = (From t In ParameterLine.Split(","c) Select DescapeQuota(t.Trim(" "c))).ToArray})
            Else
                Arguments.Add(arg)
            End If
        Next

        Return New CommandLineArguments With {.Arguments = Arguments.ToArray, .Options = Options.ToArray}
    End Function
End Class
