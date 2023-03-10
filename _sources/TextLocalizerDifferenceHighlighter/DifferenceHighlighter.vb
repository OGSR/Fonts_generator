'==========================================================================
'
'  File:        DifferenceHighlighter.vb
'  Location:    Firefly.TextLocalizer.DifferenceHighlighter <Visual Basic .Net>
'  Description: 文本本地化工具差异比较高亮插件
'  Version:     2009.10.08.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Drawing
Imports System.IO
Imports Firefly
Imports Firefly.TextEncoding
Imports Firefly.Setting
Imports Firefly.Project

Public Class Config
    Public ComparePairs As ComparePair()
End Class

Public Class ComparePair
    Public Source As String
    Public Target As String
    Public SourceForeColor As String = "FFFF0000"
    Public SourceBackColor As String = "FFBFFFBF"
    Public TargetForeColor As String = "FFFF0000"
    Public TargetBackColor As String = "FFBFFFBF"
End Class

Public Class DifferenceHighlighter
    Inherits TextLocalizerBase
    Implements ITextLocalizerTextHighlighter

    Private SettingPath As String = "DifferenceHighlighter.locplugin"
    Private Config As Config

    Public Sub New()
        If File.Exists(SettingPath) Then
            Config = Xml.ReadFile(Of Config)(SettingPath)
        Else
            Config = New Config With {.ComparePairs = New ComparePair() {}}
        End If
    End Sub
    Protected Overrides Sub DisposeManagedResource()
        Try
            Xml.WriteFile(SettingPath, UTF16, Config)
        Catch
        End Try
        MyBase.DisposeManagedResource()
    End Sub

    Public Function GetTextStyles(ByVal TextName As String, ByVal TextIndex As Integer, ByVal FormatedTexts As IEnumerable(Of String)) As IEnumerable(Of TextStyle()) Implements Firefly.Project.ITextLocalizerTextHighlighter.GetTextStyles
        Dim TextStyles = (From i In Enumerable.Range(0, Columns.Count) Select New List(Of TextStyle)).ToArray

        If Config.ComparePairs IsNot Nothing Then
            For Each p In Config.ComparePairs
                If p Is Nothing Then Continue For
                If Not NameToColumn.ContainsKey(p.Source) Then Continue For
                If Not NameToColumn.ContainsKey(p.Target) Then Continue For
                Dim SourceText = FormatedTexts(NameToColumn(p.Source))
                Dim TargetText = FormatedTexts(NameToColumn(p.Target))
                If SourceText Is Nothing Then SourceText = ""
                If TargetText Is Nothing Then TargetText = ""
                Dim SourceForeColor = Color.FromArgb(Integer.Parse(p.SourceForeColor, Globalization.NumberStyles.HexNumber))
                Dim SourceBackColor = Color.FromArgb(Integer.Parse(p.SourceBackColor, Globalization.NumberStyles.HexNumber))
                Dim TargetForeColor = Color.FromArgb(Integer.Parse(p.TargetForeColor, Globalization.NumberStyles.HexNumber))
                Dim TargetBackColor = Color.FromArgb(Integer.Parse(p.TargetBackColor, Globalization.NumberStyles.HexNumber))
                Dim Diff = StringDiff.Compare(SourceText.ToCharArray, TargetText.ToCharArray)

                Dim Source = TextStyles(NameToColumn(p.Source))
                Dim Target = TextStyles(NameToColumn(p.Target))
                For Each d In Diff
                    If d.SourceLength <> d.TargetLength Then
                        If d.SourceLength <> 0 Then Source.Add(New TextStyle With {.Index = d.SourceIndex, .Length = d.SourceLength, .ForeColor = SourceForeColor, .BackColor = SourceBackColor})
                        If d.TargetLength <> 0 Then Target.Add(New TextStyle With {.Index = d.TargetIndex, .Length = d.TargetLength, .ForeColor = TargetForeColor, .BackColor = TargetBackColor})
                    End If
                Next
            Next
        End If

        Return (From l In TextStyles Select l.ToArray).ToArray
    End Function
End Class
