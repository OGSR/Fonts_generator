<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormSearch
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Label_TextFind = New System.Windows.Forms.Label
        Me.Label_TextReplace = New System.Windows.Forms.Label
        Me.ComboBox_TextFind = New System.Windows.Forms.ComboBox
        Me.ComboBox_TextReplace = New System.Windows.Forms.ComboBox
        Me.Label_Scope = New System.Windows.Forms.Label
        Me.ComboBox_Scope = New System.Windows.Forms.ComboBox
        Me.CheckBox_Case = New System.Windows.Forms.CheckBox
        Me.CheckBox_Up = New System.Windows.Forms.CheckBox
        Me.CheckBox_Regex = New System.Windows.Forms.CheckBox
        Me.Button_Find = New System.Windows.Forms.Button
        Me.Button_Replace = New System.Windows.Forms.Button
        Me.Button_ReplaceAll = New System.Windows.Forms.Button
        Me.NumericUpDown_Column = New System.Windows.Forms.Label
        Me.Label_Row = New System.Windows.Forms.Label
        Me.CheckBox_SmartHanzi = New System.Windows.Forms.CheckBox
        Me.SuspendLayout()
        '
        'Label_TextFind
        '
        Me.Label_TextFind.AutoSize = True
        Me.Label_TextFind.Location = New System.Drawing.Point(12, 9)
        Me.Label_TextFind.Name = "Label_TextFind"
        Me.Label_TextFind.Size = New System.Drawing.Size(71, 12)
        Me.Label_TextFind.TabIndex = 0
        Me.Label_TextFind.Text = "查找内容(&N)"
        '
        'Label_TextReplace
        '
        Me.Label_TextReplace.AutoSize = True
        Me.Label_TextReplace.Location = New System.Drawing.Point(12, 52)
        Me.Label_TextReplace.Name = "Label_TextReplace"
        Me.Label_TextReplace.Size = New System.Drawing.Size(71, 12)
        Me.Label_TextReplace.TabIndex = 2
        Me.Label_TextReplace.Text = "替换内容(&P)"
        '
        'ComboBox_TextFind
        '
        Me.ComboBox_TextFind.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ComboBox_TextFind.Location = New System.Drawing.Point(14, 24)
        Me.ComboBox_TextFind.Name = "ComboBox_TextFind"
        Me.ComboBox_TextFind.Size = New System.Drawing.Size(266, 20)
        Me.ComboBox_TextFind.TabIndex = 1
        '
        'ComboBox_TextReplace
        '
        Me.ComboBox_TextReplace.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ComboBox_TextReplace.Location = New System.Drawing.Point(14, 67)
        Me.ComboBox_TextReplace.Name = "ComboBox_TextReplace"
        Me.ComboBox_TextReplace.Size = New System.Drawing.Size(266, 20)
        Me.ComboBox_TextReplace.TabIndex = 3
        '
        'Label_Scope
        '
        Me.Label_Scope.AutoSize = True
        Me.Label_Scope.Location = New System.Drawing.Point(12, 95)
        Me.Label_Scope.Name = "Label_Scope"
        Me.Label_Scope.Size = New System.Drawing.Size(71, 12)
        Me.Label_Scope.TabIndex = 4
        Me.Label_Scope.Text = "查找范围(&L)"
        '
        'ComboBox_Scope
        '
        Me.ComboBox_Scope.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ComboBox_Scope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox_Scope.Items.AddRange(New Object() {"当前文件当前栏", "所有文件当前栏", "当前文件所有栏", "所有文件所有栏"})
        Me.ComboBox_Scope.Location = New System.Drawing.Point(12, 110)
        Me.ComboBox_Scope.Name = "ComboBox_Scope"
        Me.ComboBox_Scope.Size = New System.Drawing.Size(266, 20)
        Me.ComboBox_Scope.TabIndex = 5
        '
        'CheckBox_Case
        '
        Me.CheckBox_Case.AutoSize = True
        Me.CheckBox_Case.Checked = True
        Me.CheckBox_Case.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBox_Case.Location = New System.Drawing.Point(14, 213)
        Me.CheckBox_Case.Name = "CheckBox_Case"
        Me.CheckBox_Case.Size = New System.Drawing.Size(102, 16)
        Me.CheckBox_Case.TabIndex = 9
        Me.CheckBox_Case.Text = "大小写匹配(&C)"
        Me.CheckBox_Case.UseVisualStyleBackColor = True
        '
        'CheckBox_Up
        '
        Me.CheckBox_Up.AutoSize = True
        Me.CheckBox_Up.Location = New System.Drawing.Point(14, 191)
        Me.CheckBox_Up.Name = "CheckBox_Up"
        Me.CheckBox_Up.Size = New System.Drawing.Size(90, 16)
        Me.CheckBox_Up.TabIndex = 8
        Me.CheckBox_Up.Text = "向上搜索(&U)"
        Me.CheckBox_Up.UseVisualStyleBackColor = True
        '
        'CheckBox_Regex
        '
        Me.CheckBox_Regex.AutoSize = True
        Me.CheckBox_Regex.Location = New System.Drawing.Point(14, 235)
        Me.CheckBox_Regex.Name = "CheckBox_Regex"
        Me.CheckBox_Regex.Size = New System.Drawing.Size(126, 16)
        Me.CheckBox_Regex.TabIndex = 10
        Me.CheckBox_Regex.Text = ".Net正则表达式(&E)"
        Me.CheckBox_Regex.UseVisualStyleBackColor = True
        '
        'Button_Find
        '
        Me.Button_Find.Location = New System.Drawing.Point(187, 187)
        Me.Button_Find.Name = "Button_Find"
        Me.Button_Find.Size = New System.Drawing.Size(93, 23)
        Me.Button_Find.TabIndex = 13
        Me.Button_Find.Text = "查找(&F)"
        Me.Button_Find.UseVisualStyleBackColor = True
        '
        'Button_Replace
        '
        Me.Button_Replace.Location = New System.Drawing.Point(187, 231)
        Me.Button_Replace.Name = "Button_Replace"
        Me.Button_Replace.Size = New System.Drawing.Size(93, 23)
        Me.Button_Replace.TabIndex = 14
        Me.Button_Replace.Text = "替换(&R)"
        Me.Button_Replace.UseVisualStyleBackColor = True
        '
        'Button_ReplaceAll
        '
        Me.Button_ReplaceAll.Location = New System.Drawing.Point(187, 275)
        Me.Button_ReplaceAll.Name = "Button_ReplaceAll"
        Me.Button_ReplaceAll.Size = New System.Drawing.Size(93, 23)
        Me.Button_ReplaceAll.TabIndex = 15
        Me.Button_ReplaceAll.Text = "全部替换(&A)"
        Me.Button_ReplaceAll.UseVisualStyleBackColor = True
        '
        'NumericUpDown_Column
        '
        Me.NumericUpDown_Column.AutoSize = True
        Me.NumericUpDown_Column.Location = New System.Drawing.Point(12, 156)
        Me.NumericUpDown_Column.Name = "NumericUpDown_Column"
        Me.NumericUpDown_Column.Size = New System.Drawing.Size(0, 12)
        Me.NumericUpDown_Column.TabIndex = 7
        '
        'Label_Row
        '
        Me.Label_Row.AutoSize = True
        Me.Label_Row.Location = New System.Drawing.Point(12, 141)
        Me.Label_Row.Name = "Label_Row"
        Me.Label_Row.Size = New System.Drawing.Size(35, 12)
        Me.Label_Row.TabIndex = 6
        Me.Label_Row.Text = "栏(&O)"
        '
        'CheckBox_SmartHanzi
        '
        Me.CheckBox_SmartHanzi.AutoSize = True
        Me.CheckBox_SmartHanzi.Checked = True
        Me.CheckBox_SmartHanzi.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBox_SmartHanzi.Location = New System.Drawing.Point(14, 257)
        Me.CheckBox_SmartHanzi.Name = "CheckBox_SmartHanzi"
        Me.CheckBox_SmartHanzi.Size = New System.Drawing.Size(108, 16)
        Me.CheckBox_SmartHanzi.TabIndex = 11
        Me.CheckBox_SmartHanzi.Text = "异体字模糊匹配"
        Me.CheckBox_SmartHanzi.UseVisualStyleBackColor = True
        '
        'FormSearch
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(292, 308)
        Me.Controls.Add(Me.CheckBox_SmartHanzi)
        Me.Controls.Add(Me.Label_Row)
        Me.Controls.Add(Me.NumericUpDown_Column)
        Me.Controls.Add(Me.Button_ReplaceAll)
        Me.Controls.Add(Me.Button_Replace)
        Me.Controls.Add(Me.Button_Find)
        Me.Controls.Add(Me.CheckBox_Regex)
        Me.Controls.Add(Me.CheckBox_Up)
        Me.Controls.Add(Me.CheckBox_Case)
        Me.Controls.Add(Me.ComboBox_Scope)
        Me.Controls.Add(Me.ComboBox_TextReplace)
        Me.Controls.Add(Me.ComboBox_TextFind)
        Me.Controls.Add(Me.Label_Scope)
        Me.Controls.Add(Me.Label_TextReplace)
        Me.Controls.Add(Me.Label_TextFind)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FormSearch"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "查找与替换"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label_TextFind As System.Windows.Forms.Label
    Friend WithEvents Label_TextReplace As System.Windows.Forms.Label
    Friend WithEvents ComboBox_TextFind As System.Windows.Forms.ComboBox
    Friend WithEvents ComboBox_TextReplace As System.Windows.Forms.ComboBox
    Friend WithEvents Label_Scope As System.Windows.Forms.Label
    Friend WithEvents ComboBox_Scope As System.Windows.Forms.ComboBox
    Friend WithEvents CheckBox_Case As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox_Up As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox_Regex As System.Windows.Forms.CheckBox
    Friend WithEvents Button_Find As System.Windows.Forms.Button
    Friend WithEvents Button_Replace As System.Windows.Forms.Button
    Friend WithEvents Button_ReplaceAll As System.Windows.Forms.Button
    Friend WithEvents NumericUpDown_Column As System.Windows.Forms.Label
    Friend WithEvents Label_Row As System.Windows.Forms.Label
    Friend WithEvents CheckBox_SmartHanzi As System.Windows.Forms.CheckBox
End Class
