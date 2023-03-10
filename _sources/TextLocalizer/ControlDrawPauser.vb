Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Public Module ControlDrawPauser
    <DllImport("user32", CharSet:=CharSet.Auto)> _
    Private Function SendMessage(ByVal hWnd As HandleRef, ByVal msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    End Function

    Private Const WM_SETREDRAW As Integer = &HB
    Private Const EM_SETEVENTMASK As Integer = &H431

    Private OldEventMasks As New Dictionary(Of Control, IntPtr)
    <Extension()> Public Sub SuspendDraw(ByVal This As Control)
        If OldEventMasks.ContainsKey(This) Then Return
        OldEventMasks.Add(This, SendMessage(New HandleRef(This, This.Handle), EM_SETEVENTMASK, IntPtr.Zero, IntPtr.Zero))

        If Not This.Visible Then Return

        SendMessage(New HandleRef(This, This.Handle), WM_SETREDRAW, 0, 0)
    End Sub

    <Extension()> Public Sub ResumeDraw(ByVal This As Control)
        If Not OldEventMasks.ContainsKey(This) Then Return
        If This.Visible Then SendMessage(New HandleRef(This, This.Handle), WM_SETREDRAW, 1, 0)

        SendMessage(New HandleRef(This, This.Handle), EM_SETEVENTMASK, IntPtr.Zero, OldEventMasks(This))
        OldEventMasks.Remove(This)
    End Sub
End Module
