'==========================================================================
'
'  File:        StringDiff.vb
'  Location:    Firefly.TextLocalizer.DifferenceHighlighter <Visual Basic .Net>
'  Description: 序列比较
'  Version:     2009.10.06.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Math
Imports System.Collections.Generic
Imports System.Linq
Imports Firefly

Public Class TranslatePart
    Public SourceIndex As Integer
    Public SourceLength As Integer
    Public TargetIndex As Integer
    Public TargetLength As Integer
End Class

Public NotInheritable Class StringDiff
    Private Sub New()
    End Sub

    Private Enum DifferencePattern
        None = 0
        Del
        Add
        Same
        Replace
    End Enum

    Private Class StringDifference
        Public Pattern As DifferencePattern
        Public SourceIndex As Integer
        Public SourceLength As Integer
        Public TargetIndex As Integer
        Public TargetLength As Integer
    End Class

    Private Class Comparer(Of T As IEquatable(Of T))
        Private Source As T()
        Private Target As T()
        Private n As Integer
        Private m As Integer
        Private C As Integer(,)
        Private D As DifferencePattern(,)

        Public Sub New(ByVal Source As T(), ByVal Target As T())
            Me.Source = Source
            Me.Target = Target
            n = Source.Length
            m = Target.Length
            C = New Integer(n, m) {}
            D = New Integer(n, m) {}

            For i = 0 To n
                C(i, 0) = i
            Next
            For j = 0 To m
                C(0, j) = j
            Next
        End Sub

        Private Sub CalcDifference()
            For i = 1 To n
                For j = 1 To m
                    Dim s = C(i - 1, j) + 1
                    Dim dv = DifferencePattern.Del
                    If D(i - 1, j) <> DifferencePattern.Del AndAlso D(i - 1, j) <> DifferencePattern.Replace Then
                        s += 1
                    End If

                    Dim s2 = C(i, j - 1) + 1
                    If D(i, j - 1) <> DifferencePattern.Add AndAlso D(i, j - 1) <> DifferencePattern.Replace Then
                        s2 += 1
                    End If
                    If s2 < s Then
                        s = s2
                        dv = DifferencePattern.Add
                    End If

                    If Source(i - 1).Equals(Target(j - 1)) Then
                        Dim s3 = C(i - 1, j - 1)
                        If D(i - 1, j - 1) <> DifferencePattern.Same Then
                            s3 += 1
                        End If
                        If s3 < s Then
                            s = s3
                            dv = DifferencePattern.Same
                        End If
                    Else
                        Dim s4 = C(i - 1, j - 1) + 2
                        If D(i - 1, j - 1) = DifferencePattern.Same Then
                            s4 += 1
                        End If
                        If s4 < s Then
                            s = s4
                            dv = DifferencePattern.Replace
                        End If
                    End If

                    C(i, j) = s
                    D(i, j) = dv
                Next
            Next
        End Sub

        Public Function GetDifference() As TranslatePart()
            CalcDifference()

            Dim s As New Stack(Of StringDifference)
            Dim i = n
            Dim j = m
            While True
                If i <= 0 Then
                    If j <= 0 Then
                    Else
                        s.Push(New StringDifference With {.Pattern = DifferencePattern.Add, .SourceIndex = 0, .SourceLength = 0, .TargetIndex = 0, .TargetLength = j})
                    End If
                    Exit While
                ElseIf j <= 0 Then
                    s.Push(New StringDifference With {.Pattern = DifferencePattern.Del, .SourceIndex = 0, .SourceLength = i, .TargetIndex = 0, .TargetLength = 0})
                    Exit While
                Else
                    Dim Pattern = D(i, j)
                    Select Case Pattern
                        Case DifferencePattern.Del
                            i -= 1
                            s.Push(New StringDifference With {.Pattern = DifferencePattern.Del, .SourceIndex = i, .SourceLength = 1, .TargetIndex = j, .TargetLength = 0})
                        Case DifferencePattern.Add
                            j -= 1
                            s.Push(New StringDifference With {.Pattern = DifferencePattern.Add, .SourceIndex = i, .SourceLength = 0, .TargetIndex = j, .TargetLength = 1})
                        Case DifferencePattern.Replace, DifferencePattern.Same
                            i -= 1
                            j -= 1
                            s.Push(New StringDifference With {.Pattern = Pattern, .SourceIndex = i, .SourceLength = 1, .TargetIndex = j, .TargetLength = 1})
                    End Select
                End If
            End While

            If s.Count = 0 Then Return New TranslatePart() {}

            Dim l As New List(Of StringDifference)
            Dim c0 As StringDifference = Nothing
            Dim c As StringDifference = s.Pop
            If c.Pattern = DifferencePattern.Replace Then
                l.Add(New StringDifference With {.Pattern = DifferencePattern.Del, .SourceIndex = c.SourceIndex, .SourceLength = c.SourceLength, .TargetIndex = c.TargetIndex, .TargetLength = 0})
                c = New StringDifference With {.Pattern = DifferencePattern.Add, .SourceIndex = c.SourceIndex + c.SourceLength, .SourceLength = 0, .TargetIndex = c.TargetIndex, .TargetLength = c.TargetLength}
            End If
            While True
                If c0 IsNot Nothing Then
                    If c.Pattern = c0.Pattern Then
                        c0.SourceLength += c.SourceLength
                        c0.TargetLength += c.TargetLength
                        If s.Count > 0 Then
                            c = s.Pop
                        Else
                            Exit While
                        End If
                    ElseIf c.Pattern = DifferencePattern.Replace Then
                        If c0.Pattern = DifferencePattern.Del Then
                            c0.SourceLength += c.SourceLength
                            c = New StringDifference With {.Pattern = DifferencePattern.Add, .SourceIndex = c.SourceIndex + c.SourceLength, .SourceLength = 0, .TargetIndex = c.TargetIndex, .TargetLength = c.TargetLength}
                        ElseIf c0.Pattern = DifferencePattern.Add Then
                            c0.TargetLength += c.TargetLength
                            c = New StringDifference With {.Pattern = DifferencePattern.Del, .SourceIndex = c.SourceIndex, .SourceLength = c.SourceLength, .TargetIndex = c.TargetIndex + c.TargetLength, .TargetLength = 0}
                        Else
                            l.Add(c0)
                            c0 = New StringDifference With {.Pattern = DifferencePattern.Del, .SourceIndex = c.SourceIndex, .SourceLength = c.SourceLength, .TargetIndex = c.TargetIndex, .TargetLength = 0}
                            c = New StringDifference With {.Pattern = DifferencePattern.Add, .SourceIndex = c.SourceIndex + c.SourceLength, .SourceLength = 0, .TargetIndex = c.TargetIndex, .TargetLength = c.TargetLength}
                        End If
                        l.Add(c0)
                        c0 = c
                        If s.Count > 0 Then
                            c = s.Pop
                        Else
                            Exit While
                        End If
                    Else
                        l.Add(c0)
                        c0 = c
                        If s.Count > 0 Then
                            c = s.Pop
                        Else
                            Exit While
                        End If
                    End If
                Else
                    c0 = c
                    If s.Count > 0 Then
                        c = s.Pop
                    Else
                        Exit While
                    End If
                End If
            End While
            l.Add(c0)

            Return (From p In l Select New TranslatePart With {.SourceIndex = p.SourceIndex, .SourceLength = p.SourceLength, .TargetIndex = p.TargetIndex, .TargetLength = p.TargetLength}).ToArray
        End Function
    End Class

    Public Shared Function Compare(Of T As IEquatable(Of T))(ByVal Source As T(), ByVal Target As T()) As TranslatePart()
        Return (New Comparer(Of T)(Source, Target)).GetDifference()
    End Function
End Class
