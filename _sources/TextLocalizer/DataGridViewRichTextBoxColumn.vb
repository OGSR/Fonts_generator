'==========================================================================

'  File:        DataGridViewRichTextBoxColumn.vb
'  Description: 数据表富文本框列
'               此代码是由该文章中的代码而来
'               mrwisdom http://www.codeproject.com/KB/grid/RtfInDataGridView.aspx?display=Print
'  Version:     2009.10.05.

'==========================================================================

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Drawing
Imports System.Drawing.Printing
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.IO
Imports System.ComponentModel
Imports Firefly
Imports Firefly.TextEncoding

''' <summary> 
''' http://support.microsoft.com/default.aspx?scid=kb;en-us;812425 
''' The RichTextBox control does not provide any method to print the content of the RichTextBox. 
''' You can extend the RichTextBox class to use EM_FORMATRANGE message 
''' to send the content of a RichTextBox control to an output device such as printer. 
''' </summary> 
Public Class RichTextBoxPrinter
    'Convert the unit used by the .NET framework (1/100 inch) 
    'and the unit used by Win32 API calls (twips 1/1440 inch) 
    Private Const anInch As Double = 14.4

    <StructLayout(LayoutKind.Sequential)> _
    Private Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Private Structure CHARRANGE
        Public cpMin As Integer
        'First character of range (0 for start of doc) 
        Public cpMax As Integer
        'Last character of range (-1 for end of doc) 
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Private Structure FORMATRANGE
        Public hdc As IntPtr
        'Actual DC to draw on 
        Public hdcTarget As IntPtr
        'Target DC for determining text formatting 
        Public rc As RECT
        'Region of the DC to draw to (in twips) 
        Public rcPage As RECT
        'Region of the whole DC (page size) (in twips) 
        Public chrg As CHARRANGE
        'Range of text to draw (see earlier declaration) 
    End Structure

    Private Const WM_USER As Integer = &H400
    Private Const EM_FORMATRANGE As Integer = WM_USER + 57

    <DllImport("USER32.dll")> _
    Private Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal msg As Integer, ByVal wp As IntPtr, ByVal lp As IntPtr) As IntPtr
    End Function

    ' Render the contents of the RichTextBox for printing 
    ' Return the last character printed + 1 (printing start from this point for next page) 
    Public Shared Function Print(ByVal richTextBoxHandle As IntPtr, ByVal charFrom As Integer, ByVal charTo As Integer, ByVal e As PrintPageEventArgs) As Integer
        'Calculate the area to render and print 
        Dim rectToPrint As RECT
        rectToPrint.Top = CInt((e.MarginBounds.Top * anInch))
        rectToPrint.Bottom = CInt((e.MarginBounds.Bottom * anInch))
        rectToPrint.Left = CInt((e.MarginBounds.Left * anInch))
        rectToPrint.Right = CInt((e.MarginBounds.Right * anInch))

        'Calculate the size of the page 
        Dim rectPage As RECT
        rectPage.Top = CInt((e.PageBounds.Top * anInch))
        rectPage.Bottom = CInt((e.PageBounds.Bottom * anInch))
        rectPage.Left = CInt((e.PageBounds.Left * anInch))
        rectPage.Right = CInt((e.PageBounds.Right * anInch))

        Dim hdc As IntPtr = e.Graphics.GetHdc()

        Dim fmtRange As FORMATRANGE
        fmtRange.chrg.cpMax = charTo
        'Indicate character from to character to 
        fmtRange.chrg.cpMin = charFrom
        fmtRange.hdc = hdc
        'Use the same DC for measuring and rendering 
        fmtRange.hdcTarget = hdc
        'Point at printer hDC 
        fmtRange.rc = rectToPrint
        'Indicate the area on page to print 
        fmtRange.rcPage = rectPage
        'Indicate size of page 
        Dim res As IntPtr = IntPtr.Zero

        Dim wparam As IntPtr = IntPtr.Zero
        wparam = New IntPtr(1)

        'Get the pointer to the FORMATRANGE structure in memory 
        Dim lparam As IntPtr = IntPtr.Zero
        lparam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fmtRange))
        Marshal.StructureToPtr(fmtRange, lparam, False)

        'Send the rendered data for printing 
        res = SendMessage(richTextBoxHandle, EM_FORMATRANGE, wparam, lparam)

        'Free the block of memory allocated 
        Marshal.FreeCoTaskMem(lparam)

        'Release the device context handle obtained by a previous call 
        e.Graphics.ReleaseHdc(hdc)

        ' Release and cached info 
        SendMessage(richTextBoxHandle, EM_FORMATRANGE, IntPtr.Zero, IntPtr.Zero)

        'Return last + 1 character printer 
        Return res.ToInt32()
    End Function

    Public Shared Function Print(ByVal ctl As RichTextBox, ByVal width As Integer, ByVal height As Integer) As Image
        Dim img As Image = New Bitmap(width, height)
        Dim scale As Single

        Using g As Graphics = Graphics.FromImage(img)
            g.Clear(ctl.BackColor)
            ' --- Begin code addition D_Kondrad

            ' HorizontalResolution is measured in pix/inch 
            scale = CSng((width * 100)) / img.HorizontalResolution
            width = CInt(scale)

            ' VerticalResolution is measured in pix/inch 
            scale = CSng((height * 100)) / img.VerticalResolution
            height = CInt(scale)

            ' --- End code addition D_Kondrad 

            Dim marginBounds As New Rectangle(0, 0, width, height)
            Dim pageBounds As New Rectangle(0, 0, width, height)
            Dim args As New PrintPageEventArgs(g, marginBounds, pageBounds, Nothing)

            Print(ctl.Handle, 0, ctl.Text.Length, args)
        End Using

        Return img
    End Function

End Class

Public Class DataGridViewRichTextBoxColumn
    Inherits DataGridViewColumn
    Public Sub New()
        MyBase.New(New DataGridViewRichTextBoxCell())
    End Sub

    Public Overloads Overrides Property CellTemplate() As DataGridViewCell
        Get
            Return MyBase.CellTemplate
        End Get
        Set(ByVal value As DataGridViewCell)
            If Not (TypeOf value Is DataGridViewRichTextBoxCell) Then
                Throw New InvalidCastException("CellTemplate must be a DataGridViewRichTextBoxCell")
            End If

            MyBase.CellTemplate = value
        End Set
    End Property
End Class

Public Class DataGridViewRichTextBoxCell
    Inherits DataGridViewImageCell

    Public Overloads Overrides ReadOnly Property EditType() As Type
        Get
            Return GetType(DataGridViewRichTextBoxEditingControl)
        End Get
    End Property

    Public Overloads Overrides Property ValueType() As Type
        Get
            Return GetType(String)
        End Get
        Set(ByVal value As Type)
            MyBase.ValueType = value
        End Set
    End Property

    Public Overloads Overrides ReadOnly Property FormattedValueType() As Type
        Get
            Return GetType(String)
        End Get
    End Property

    Private Shared ReadOnly Property InnerRichTextBoxForText() As RichTextBox
        Get
            Static c As RichTextBox = Nothing
            If c Is Nothing Then
                c = New RichTextBox
                c.BorderStyle = BorderStyle.None
            End If
            Return c
        End Get
    End Property
    Private Shared ReadOnly Property InnerRichTextBoxForSize() As RichTextBox
        Get
            Static c As RichTextBox = Nothing
            If c Is Nothing Then
                c = New RichTextBox
                c.BorderStyle = BorderStyle.None
                AddHandler c.ContentsResized, AddressOf ContentsResized
            End If
            Return c
        End Get
    End Property

    Public Property Rtf() As String
        Get
            Return Value
        End Get
        Set(ByVal Value As String)
            Me.Value = Value
        End Set
    End Property

    Public Property Text() As String
        Get
            InnerRichTextBoxForText.Rtf = Value
            Return InnerRichTextBoxForText.Text
        End Get
        Set(ByVal Value As String)
            InnerRichTextBoxForText.Clear()
            InnerRichTextBoxForText.Text = Value
            Me.Value = InnerRichTextBoxForText.Rtf
        End Set
    End Property

    Private Shared EnablePreferredHeight As Boolean = False
    Private Shared PreferredWidth As Integer = 0
    Private Shared PreferredHeight As Integer = 0
    Protected Overrides Function GetPreferredSize(ByVal graphics As System.Drawing.Graphics, ByVal cellStyle As System.Windows.Forms.DataGridViewCellStyle, ByVal rowIndex As Integer, ByVal constraintSize As System.Drawing.Size) As System.Drawing.Size
        Dim cellSize As Size = GetSize(rowIndex)
        Dim Width = cellSize.Width

        InnerRichTextBoxForSize.Size = New Size(Width, 0)
        PreferredWidth = 0
        PreferredHeight = 0
        InnerRichTextBoxForSize.Rtf = GetValue(rowIndex)
        Dim Rtf = InnerRichTextBoxForSize.Rtf
        InnerRichTextBoxForSize.Text = ""
        EnablePreferredHeight = True
        InnerRichTextBoxForSize.Rtf = Rtf
        EnablePreferredHeight = False

        Dim Height = PreferredHeight + 1
        Return New Size(Width, Height)
    End Function

    Private Shared Sub ContentsResized(ByVal sender As Object, ByVal e As System.Windows.Forms.ContentsResizedEventArgs)
        If Not EnablePreferredHeight Then Return
        PreferredWidth = e.NewRectangle.Width
        PreferredHeight = e.NewRectangle.Height
    End Sub

    Private Function GetRtfImage(ByVal rowIndex As Integer, ByVal value As Object, ByVal selected As Boolean) As Image
        Dim cellSize As Size = GetSize(rowIndex)

        If cellSize.Width < 1 OrElse cellSize.Height < 1 Then
            Return Nothing
        End If

        Dim ctl As RichTextBox = InnerRichTextBoxForText
        ctl.Text = ""
        ctl.Size = cellSize
        ctl.Rtf = value

        If ctl IsNot Nothing Then
            ' Print the content of RichTextBox to an image. 
            Dim imgSize As New Size(cellSize.Width - 1, cellSize.Height - 1)
            Dim rtfImg As Image = Nothing

            If selected Then
                ' Selected cell state 
                ctl.BackColor = Color.LightBlue
                ctl.ForeColor = DataGridView.DefaultCellStyle.ForeColor
                'ctl.BackColor = DataGridView.DefaultCellStyle.SelectionBackColor
                'ctl.ForeColor = DataGridView.DefaultCellStyle.SelectionForeColor

                ' Print image 
                rtfImg = RichTextBoxPrinter.Print(ctl, imgSize.Width, imgSize.Height)
            Else
                ' Restore RichTextBox 
                ctl.BackColor = DataGridView.DefaultCellStyle.BackColor
                ctl.ForeColor = DataGridView.DefaultCellStyle.ForeColor
                rtfImg = RichTextBoxPrinter.Print(ctl, imgSize.Width, imgSize.Height)
            End If

            Return rtfImg
        End If

        Return Nothing
    End Function

    Public Overloads Overrides Sub InitializeEditingControl(ByVal rowIndex As Integer, ByVal initialFormattedValue As Object, ByVal dataGridViewCellStyle As DataGridViewCellStyle)
        MyBase.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle)

        Dim ctl As RichTextBox = TryCast(DataGridView.EditingControl, RichTextBox)

        If ctl IsNot Nothing Then
            ctl.Rtf = Value
        End If
    End Sub

    Protected Overloads Overrides Function GetFormattedValue(ByVal value As Object, ByVal rowIndex As Integer, ByRef cellStyle As DataGridViewCellStyle, ByVal valueTypeConverter As TypeConverter, ByVal formattedValueTypeConverter As TypeConverter, ByVal context As DataGridViewDataErrorContexts) As Object
        InnerRichTextBoxForText.Rtf = value
        Return InnerRichTextBoxForText.Text
    End Function

    Protected Overloads Overrides Sub Paint(ByVal graphics As Graphics, ByVal clipBounds As Rectangle, ByVal cellBounds As Rectangle, ByVal rowIndex As Integer, ByVal cellState As DataGridViewElementStates, ByVal value As Object, ByVal formattedValue As Object, ByVal errorText As String, ByVal cellStyle As DataGridViewCellStyle, ByVal advancedBorderStyle As DataGridViewAdvancedBorderStyle, ByVal paintParts As DataGridViewPaintParts)
        MyBase.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, Nothing, Nothing, errorText, cellStyle, advancedBorderStyle, paintParts)

        Dim img As Image = GetRtfImage(rowIndex, value, MyBase.Selected)
        If img IsNot Nothing Then
            graphics.DrawImage(img, cellBounds.Left, cellBounds.Top)
        End If
    End Sub

#Region "Handlers of edit events, copyied from DataGridViewTextBoxCell"
    Private flagsState As Byte

    Protected Overloads Overrides Sub OnEnter(ByVal rowIndex As Integer, ByVal throughMouseClick As Boolean)
        MyBase.OnEnter(rowIndex, throughMouseClick)

        If (MyBase.DataGridView IsNot Nothing) AndAlso throughMouseClick Then
            Me.flagsState = CByte((Me.flagsState Or 1))
        End If
    End Sub

    Protected Overloads Overrides Sub OnLeave(ByVal rowIndex As Integer, ByVal throughMouseClick As Boolean)
        MyBase.OnLeave(rowIndex, throughMouseClick)

        If MyBase.DataGridView IsNot Nothing Then
            Me.flagsState = CByte((Me.flagsState And -2))
        End If
    End Sub

    Protected Overloads Overrides Sub OnMouseClick(ByVal e As DataGridViewCellMouseEventArgs)
        MyBase.OnMouseClick(e)
        If MyBase.DataGridView IsNot Nothing Then
            Dim currentCellAddress As Point = MyBase.DataGridView.CurrentCellAddress

            If ((currentCellAddress.X = e.ColumnIndex) AndAlso (currentCellAddress.Y = e.RowIndex)) AndAlso (e.Button = MouseButtons.Left) Then
                If (Me.flagsState And 1) <> 0 Then
                    Me.flagsState = CByte((Me.flagsState And -2))
                ElseIf MyBase.DataGridView.EditMode <> DataGridViewEditMode.EditProgrammatically Then
                    MyBase.DataGridView.BeginEdit(False)
                End If
            End If
        End If
    End Sub

    Public Overloads Overrides Function KeyEntersEditMode(ByVal e As KeyEventArgs) As Boolean
        Return (((((Char.IsLetterOrDigit(New Char32(e.KeyCode)) AndAlso ((e.KeyCode < Keys.F1) OrElse (e.KeyCode > Keys.F24))) OrElse ((e.KeyCode >= Keys.NumPad0) AndAlso (e.KeyCode <= Keys.Divide))) OrElse (((e.KeyCode >= Keys.OemSemicolon) AndAlso (e.KeyCode <= Keys.OemBackslash)) OrElse ((e.KeyCode = Keys.Space) AndAlso Not e.Shift))) AndAlso (Not e.Alt AndAlso Not e.Control)) OrElse MyBase.KeyEntersEditMode(e))
    End Function
#End Region
End Class

Public Class DataGridViewRichTextBoxEditingControl
    Inherits RichTextBox
    Implements IDataGridViewEditingControl
    Private _dataGridView As DataGridView
    Private _rowIndex As Integer
    Private _valueChanged As Boolean

    Public Sub New()
        Me.BorderStyle = BorderStyle.None
    End Sub

    Protected Overloads Overrides Sub OnTextChanged(ByVal e As EventArgs)
        MyBase.OnTextChanged(e)

        _valueChanged = True
        EditingControlDataGridView.NotifyCurrentCellDirty(True)
    End Sub

    Protected Overloads Overrides Function IsInputKey(ByVal keyData As Keys) As Boolean
        Dim keys__1 As Keys = keyData And Keys.KeyCode
        If keys__1 = Keys.[Return] Then
            Return Me.Multiline
        End If

        Return MyBase.IsInputKey(keyData)
    End Function

    Protected Overloads Overrides Sub OnKeyDown(ByVal e As KeyEventArgs)
        MyBase.OnKeyDown(e)

        If e.Control Then
            Select Case e.KeyCode
                ' Control + B = Bold 
                Case Keys.B
                    If Me.SelectionFont.Bold Then
                        Me.SelectionFont = New Font(Me.Font.FontFamily, Me.Font.Size, Not FontStyle.Bold And Me.Font.Style)
                    Else
                        Me.SelectionFont = New Font(Me.Font.FontFamily, Me.Font.Size, FontStyle.Bold Or Me.Font.Style)
                    End If
                    Exit Select
                    ' Control + U = Underline 
                Case Keys.U
                    If Me.SelectionFont.Underline Then
                        Me.SelectionFont = New Font(Me.Font.FontFamily, Me.Font.Size, Not FontStyle.Underline And Me.Font.Style)
                    Else
                        Me.SelectionFont = New Font(Me.Font.FontFamily, Me.Font.Size, FontStyle.Underline Or Me.Font.Style)
                    End If
                    Exit Select
                Case Else
                    ' Control + I = Italic 
                    ' Conflicts with the default shortcut 
                    'case Keys.I: 
                    ' if (this.SelectionFont.Italic) 
                    ' { 
                    ' this.SelectionFont = new Font(this.Font.FontFamily, this.Font.Size, ~FontStyle.Italic & this.Font.Style); 
                    ' } 
                    ' else 
                    ' this.SelectionFont = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Italic | this.Font.Style); 
                    ' break; 
                    Exit Select
            End Select
        End If
    End Sub

#Region "IDataGridViewEditingControl Members"

    Public Sub ApplyCellStyleToEditingControl(ByVal dataGridViewCellStyle As DataGridViewCellStyle) Implements IDataGridViewEditingControl.ApplyCellStyleToEditingControl
        Me.Font = dataGridViewCellStyle.Font
    End Sub

    Public Property EditingControlDataGridView() As DataGridView Implements IDataGridViewEditingControl.EditingControlDataGridView
        Get
            Return _dataGridView
        End Get
        Set(ByVal value As DataGridView)
            _dataGridView = value
        End Set
    End Property

    Public Property EditingControlFormattedValue() As Object Implements IDataGridViewEditingControl.EditingControlFormattedValue
        Get
            Return Me.Rtf
        End Get
        Set(ByVal value As Object)
            If TypeOf value Is String Then
                Me.Text = TryCast(value, String)
            End If
        End Set
    End Property

    Public Property EditingControlRowIndex() As Integer Implements IDataGridViewEditingControl.EditingControlRowIndex
        Get
            Return _rowIndex
        End Get
        Set(ByVal value As Integer)
            _rowIndex = value
        End Set
    End Property

    Public Property EditingControlValueChanged() As Boolean Implements IDataGridViewEditingControl.EditingControlValueChanged
        Get
            Return _valueChanged
        End Get
        Set(ByVal value As Boolean)
            _valueChanged = value
        End Set
    End Property

    Public Function EditingControlWantsInputKey(ByVal keyData As Keys, ByVal dataGridViewWantsInputKey As Boolean) As Boolean Implements IDataGridViewEditingControl.EditingControlWantsInputKey
        Select Case (keyData And Keys.KeyCode)
            Case Keys.[Return]
                If (((keyData And (Keys.Alt Or Keys.Control Or Keys.Shift)) = Keys.Shift) AndAlso Me.Multiline) Then
                    Return True
                End If
                Exit Select
            Case Keys.Left, Keys.Right, Keys.Up, Keys.Down
                Return True
        End Select

        Return Not dataGridViewWantsInputKey
    End Function

    Public ReadOnly Property EditingPanelCursor() As Cursor Implements IDataGridViewEditingControl.EditingPanelCursor
        Get
            Return Me.Cursor
        End Get
    End Property

    Public Function GetEditingControlFormattedValue(ByVal context As DataGridViewDataErrorContexts) As Object Implements IDataGridViewEditingControl.GetEditingControlFormattedValue
        Return Me.Rtf
    End Function

    Public Sub PrepareEditingControlForEdit(ByVal selectAll As Boolean) Implements IDataGridViewEditingControl.PrepareEditingControlForEdit
    End Sub

    Public ReadOnly Property RepositionEditingControlOnValueChange() As Boolean Implements IDataGridViewEditingControl.RepositionEditingControlOnValueChange
        Get
            Return False
        End Get
    End Property

#End Region
End Class
