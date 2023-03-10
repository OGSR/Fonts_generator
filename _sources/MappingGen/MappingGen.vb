'==========================================================================
'
'  File:        MappingGen.vb
'  Location:    Firefly.MappingGen <Visual Basic .Net>
'  Description: 字符映射表生成器
'  Version:     2010.03.05.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.IO
Imports Firefly
Imports Firefly.TextEncoding
Imports Firefly.Texting

Public Module MappingGen

    Public Sub Main()
        If System.Diagnostics.Debugger.IsAttached Then
            MainInner()
        Else
            Try
                MainInner()
            Catch ex As Exception
                ExceptionHandler.PopupException(ex)
            End Try
        End If
    End Sub

    Public Sub MainInner()
        Dim CmdLine = CommandLine.GetCmdLine()
        Dim argv = CmdLine.Arguments
        Dim PriorG As Boolean = False
        Dim NoEmpty As Boolean = False
        Dim RemoveCodeRanges As New List(Of Range)
        For Each opt In CmdLine.Options
            Select Case opt.Name.ToLower
                Case "?", "help"
                    DisplayInfo()
                    Return
                Case "g"
                    PriorG = True
                Case "n"
                    NoEmpty = True
                Case "removecode"
                    Dim arg = opt.Arguments
                    Select Case arg.Length
                        Case 2
                            Dim l = Int32.Parse(arg(0), Globalization.NumberStyles.HexNumber)
                            Dim u = Int32.Parse(arg(1), Globalization.NumberStyles.HexNumber)
                            RemoveCodeRanges.Add(New Range(l, u))
                        Case Else
                            Throw New ArgumentException(opt.Name & ":" & String.Join(",", opt.Arguments))
                    End Select
                Case Else
                    Throw New ArgumentException(opt.Name)
            End Select
        Next
        Select Case argv.Length
            Case 3
                GenerateMapping(argv(0), argv(1), argv(2), PriorG, NoEmpty, RemoveCodeRanges)
            Case Else
                DisplayInfo()
        End Select
    End Sub

    Public Sub DisplayInfo()
        Console.WriteLine("字符映射表生成器")
        Console.WriteLine("Firefly.MappingGen，按BSD许可证分发")
        Console.WriteLine("F.R.C.")
        Console.WriteLine("")
        Console.WriteLine("本字符映射表生成器按照一个原始的日文字符映射表和一个简体汉文字符表，生成最接近的新字符映射表。")
        Console.WriteLine("最接近是指：")
        Console.WriteLine("如果新映射表中有原映射表中的字符，那么码点一致。")
        Console.WriteLine("如果新映射表中有原映射表中的字符的简体形式而没有原始字符，那么码点一致。")
        Console.WriteLine("")
        Console.WriteLine("用法:")
        Console.WriteLine("MappingGen <SourceTblFile> <Charfile> <TargetTblFile> [/G] [/N] (RemoveCode)*")
        Console.WriteLine("RemoveCode ::= /removecode:<Lower:Hex>,<Upper:Hex>")
        Console.WriteLine("SourceTblFile 原始日文字符映射表，tbl文件。")
        Console.WriteLine("CharFile 字符文件，包含所有新字符编码表中所需要的字符。")
        Console.WriteLine("TargetTblFile 目标字符映射表，tbl文件，UTF-16编码。")
        Console.WriteLine("/G 表明简体字比原字更优先。")
        Console.WriteLine("/N 表明不占用原始映射表的无字符码位。")
        Console.WriteLine("/removecode 移除该编码范围内(包含两边界)字符。")
        Console.WriteLine("编码文件本身，应以UTF-16、GB18030、UTF-8、UTF-32、UTF-16B、UTF-32B之一编码。")
        Console.WriteLine("注意：请事先备份原文件，本程序会直接修改原文件。")
        Console.WriteLine("")
        Console.WriteLine("示例:")
        Console.WriteLine("MappingGen Shift-JIS.tbl Char.txt FakeShift-JIS.tbl")
        Console.WriteLine("将Shift-JIS.tbl中的日文编码，以最接近的方式加入Char.txt中的内容，然后生成新的字符映射表FakeShift-JIS.tbl。")
    End Sub

    Private Function LookupWithUnicodeInDict(ByVal u As String, ByVal Dict As Dictionary(Of String, StringCode)) As StringCode
        If Dict.ContainsKey(u) Then Return Dict(u)
        Return Nothing
    End Function

    Public Sub GenerateMapping(ByVal SourceTblFilePath As String, ByVal CharFile As String, ByVal TargetTblFilePath As String, ByVal PriorG As Boolean, ByVal NoEmpty As Boolean, ByVal RemoveCodeRanges As IEnumerable(Of Range))
        Dim Source = TblCharMappingFile.ReadFile(SourceTblFilePath, GB18030)
        Dim SourceDict As New Dictionary(Of String, StringCode)
        For Each c In Source
            If Not SourceDict.ContainsKey(c.Unicode) Then
                SourceDict.Add(c.Unicode, c)
            End If
        Next
        Dim LookupWithUnicode = Function(u As String) LookupWithUnicodeInDict(u, SourceDict)

        Dim Target As New List(Of StringCode)

        Dim Chars = (From c In Txt.ReadFile(CharFile, GB18030, True).ToUTF32 Select CStr(c)).ToArray
        Dim UsedStringCodes As New HashSet(Of StringCode)
        Dim NewChars As New List(Of String)

        For Each r In RemoveCodeRanges
            For Each StringCode In Source
                If Not UsedStringCodes.Contains(StringCode) AndAlso StringCode.HasCode AndAlso r.Contain(StringCode.Code) Then
                    Target.Add(StringCode)
                    UsedStringCodes.Add(StringCode)
                End If
            Next
        Next
        For Each c In Chars
            Dim StringCode = LookupWithUnicode(c)
            If StringCode IsNot Nothing AndAlso UsedStringCodes.Contains(StringCode) Then Continue For
            NewChars.Add(c)
        Next
        Chars = NewChars.ToArray
        NewChars.Clear()

        If NoEmpty Then
            For Each StringCode In Source
                If Not UsedStringCodes.Contains(StringCode) AndAlso Not StringCode.HasUnicode Then
                    Target.Add(StringCode)
                    UsedStringCodes.Add(StringCode)
                End If
            Next
        End If

        If PriorG Then
            For Each c In Chars
                Dim StringCodeJ = LookupWithUnicode(HanziConverter.G2JOneOnOne(c))
                If StringCodeJ IsNot Nothing AndAlso Not UsedStringCodes.Contains(StringCodeJ) Then
                    Dim NewStringCode As New StringCode(c, StringCodeJ.Code)
                    Target.Add(NewStringCode)
                    UsedStringCodes.Add(StringCodeJ)
                    Continue For
                End If
                NewChars.Add(c)
            Next
            Chars = NewChars.ToArray
            NewChars.Clear()

            For Each c In Chars
                Dim StringCode = LookupWithUnicode(c)
                If StringCode IsNot Nothing AndAlso Not UsedStringCodes.Contains(StringCode) Then
                    Target.Add(StringCode)
                    UsedStringCodes.Add(StringCode)
                    Continue For
                End If
                NewChars.Add(c)
            Next
            Chars = NewChars.ToArray
            NewChars.Clear()
        Else
            For Each c In Chars
                Dim StringCode = LookupWithUnicode(c)
                If StringCode IsNot Nothing AndAlso Not UsedStringCodes.Contains(StringCode) Then
                    Target.Add(StringCode)
                    UsedStringCodes.Add(StringCode)
                    Continue For
                End If
                NewChars.Add(c)
            Next
            Chars = NewChars.ToArray
            NewChars.Clear()

            For Each c In Chars
                Dim StringCodeJ = LookupWithUnicode(HanziConverter.G2JOneOnOne(c))
                If StringCodeJ IsNot Nothing AndAlso Not UsedStringCodes.Contains(StringCodeJ) Then
                    Dim NewStringCode As New StringCode(c, StringCodeJ.Code)
                    Target.Add(NewStringCode)
                    UsedStringCodes.Add(StringCodeJ)
                    Continue For
                End If
                NewChars.Add(c)
            Next
            Chars = NewChars.ToArray
            NewChars.Clear()
        End If

        For Each c In Chars
            Dim StringCodeJ = LookupWithUnicode(HanziConverter.T2JOneOnOne(c))
            If StringCodeJ IsNot Nothing AndAlso Not UsedStringCodes.Contains(StringCodeJ) Then
                Dim NewStringCode As New StringCode(c, StringCodeJ.Code)
                Target.Add(NewStringCode)
                UsedStringCodes.Add(StringCodeJ)
                Continue For
            End If
            NewChars.Add(c)
        Next
        Chars = NewChars.ToArray
        NewChars.Clear()

        For Each c In Chars
            Dim StringCodeJ = LookupWithUnicode(HanziConverter.J2GOneOnOne(c))
            If StringCodeJ IsNot Nothing AndAlso Not UsedStringCodes.Contains(StringCodeJ) Then
                Dim NewStringCode As New StringCode(c, StringCodeJ.Code)
                Target.Add(NewStringCode)
                UsedStringCodes.Add(StringCodeJ)
                Continue For
            End If
            NewChars.Add(c)
        Next
        Chars = NewChars.ToArray
        NewChars.Clear()

        For Each c In Chars
            Dim StringCodeJ = LookupWithUnicode(HanziConverter.J2TOneOnOne(c))
            If StringCodeJ IsNot Nothing AndAlso Not UsedStringCodes.Contains(StringCodeJ) Then
                Dim NewStringCode As New StringCode(c, StringCodeJ.Code)
                Target.Add(NewStringCode)
                UsedStringCodes.Add(StringCodeJ)
                Continue For
            End If
            NewChars.Add(c)
        Next
        Chars = NewChars.ToArray
        NewChars.Clear()

        Dim LeftCount As Integer = (From c In Source Where Not UsedStringCodes.Contains(c)).Count

        If Chars.Length > LeftCount Then
            Console.WriteLine("字符数超出{0}个".Formats(Chars.Length - LeftCount))
            Return
        End If

        Dim k = Source.Count - 1
        For Each c In Chars.Reverse
            While UsedStringCodes.Contains(Source(k))
                k -= 1
            End While
            Dim StringCode = Source(k)
            Dim NewStringCode As New StringCode(c, StringCode.Code)
            Target.Add(NewStringCode)
            k -= 1
        Next

        Dim Diff = From code In (From c In Source Select c.Code).Except(From c In Target Select c.Code) Join c In Source On code Equals c.Code Select c
        For Each c In Diff
            Target.Add(c)
        Next

        Target.Sort(Function(Left, Right) Left.Code - Right.Code)

        TblCharMappingFile.WriteFile(TargetTblFilePath, UTF16, Target)
    End Sub
End Module
