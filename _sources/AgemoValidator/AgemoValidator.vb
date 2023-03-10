'==========================================================================
'
'  File:        AgemoValidator.vb
'  Location:    Firefly.AgemoValidator <Visual Basic .Net>
'  Description: Agemo文本格式验证器
'  Version:     2009.06.05.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows.Forms
Imports Firefly
Imports Firefly.TextEncoding
Imports Firefly.Texting

Public Class AgemoValidator

    Private Sub ListBox_Files_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ListBox_Files.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Link
        End If
    End Sub

    Private Sub ListBox_Files_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ListBox_Files.DragDrop
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim dict As New HashSet(Of String)
            For Each str As String In ListBox_Files.Items
                dict.Add(str)
            Next
            Dim strs As String() = e.Data.GetData(DataFormats.FileDrop)
            For Each str As String In strs
                If dict.Contains(str) Then Continue For
                ListBox_Files.Items.Add(str)
            Next
        End If
    End Sub

    Private Sub Button_Validate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Validate.Click
        If Not Button_Validate.Enabled Then Return
        Button_Validate.Enabled = False
        Button_Validate.Update()
        TextBox_Output.Clear()

        Dim AutoRemove = CheckBox_AutoRemove.Checked
        Dim EnforceUTF16 = CheckBox_EnforceUTF16.Checked
        Dim ToRemove As New HashSet(Of String)

        For Each f As String In ListBox_Files.Items
            Try
                Dim Text As String = ""
                Dim Validated As Boolean
                If EnforceUTF16 Then
                    Validated = Agemo.VerifyFile(f, UTF16, Text)
                Else
                    Validated = Agemo.VerifyFile(f, GB18030, Text)
                End If
                If Validated Then
                    If AutoRemove Then
                        ToRemove.Add(f)
                    End If
                Else
                    System.Diagnostics.Debug.WriteLine(Text)
                    TextBox_Output.AppendText(Text)
                    TextBox_Output.AppendText(System.Environment.NewLine)
                End If
            Catch ex As Exception
                TextBox_Output.AppendText("{0}{1}{2}".Formats(f, System.Environment.NewLine, ex.ToString))
                TextBox_Output.AppendText(System.Environment.NewLine)
            End Try
            Application.DoEvents()
        Next

        If AutoRemove Then
            Dim NewLists As New List(Of String)
            For Each f As String In ListBox_Files.Items
                If ToRemove.Contains(f) Then Continue For
                NewLists.Add(f)
            Next
            ListBox_Files.Items.Clear()
            For Each f In NewLists
                ListBox_Files.Items.Add(f)
            Next
        End If

        TextBox_Output.AppendText("完毕。")
        TextBox_Output.AppendText(System.Environment.NewLine)
        Button_Validate.Enabled = True
    End Sub
End Class
