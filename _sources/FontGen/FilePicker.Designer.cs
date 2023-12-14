
namespace FontGen
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class FilePicker : System.Windows.Forms.Form
    {

        // Form 重写 Dispose，以清理组件列表。
        [System.Diagnostics.DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components is not null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Windows 窗体设计器所必需的
        private System.ComponentModel.IContainer components;

        // 注意: 以下过程是 Windows 窗体设计器所必需的
        // 可以使用 Windows 窗体设计器修改它。
        // 不要使用代码编辑器修改它。
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            Button_Select = new System.Windows.Forms.Button();
            Button_Select.Click += new System.EventHandler(Button_Select_Click);
            Button_Cancel = new System.Windows.Forms.Button();
            Button_Cancel.Click += new System.EventHandler(Button_Cancel_Click);
            Button_Enter = new System.Windows.Forms.Button();
            Button_Enter.Click += new System.EventHandler(Button_Enter_Click);
            FileListView = new System.Windows.Forms.ListView();
            FileListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(FileListView_RetrieveVirtualItem);
            FileListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(FileListView_ColumnClick);
            FileListView.Enter += new System.EventHandler(FileListView_Enter);
            FileListView.ItemActivate += new System.EventHandler(FileListView_ItemActivate);
            FileListView.KeyDown += new System.Windows.Forms.KeyEventHandler(FileListView_KeyDown);
            FileListView.SelectedIndexChanged += new System.EventHandler(FileListView_SelectedIndexChanged);
            ColumnHeader_Name = new System.Windows.Forms.ColumnHeader();
            ColumnHeader_Length = new System.Windows.Forms.ColumnHeader();
            ColumnHeader_Type = new System.Windows.Forms.ColumnHeader();
            ColumnHeader_ModifyTime = new System.Windows.Forms.ColumnHeader();
            ColumnHeader_CreateTime = new System.Windows.Forms.ColumnHeader();
            ComboBox_FileName = new System.Windows.Forms.ComboBox();
            ComboBox_FileName.Enter += new System.EventHandler(ComboBox_FileName_Enter);
            ComboBox_FileName.TextChanged += new System.EventHandler(ComboBox_FileName_TextChanged);
            Label_FileName = new System.Windows.Forms.Label();
            ComboBox_Filter = new System.Windows.Forms.ComboBox();
            ComboBox_Filter.SelectedIndexChanged += new System.EventHandler(ComboBox_Filter_SelectedIndexChanged);
            Label_Filter = new System.Windows.Forms.Label();
            ComboBox_Directory = new System.Windows.Forms.ComboBox();
            ComboBox_Directory.SelectedIndexChanged += new System.EventHandler(ComboBox_Directory_SelectedIndexChanged);
            ComboBox_Directory.Enter += new System.EventHandler(ComboBox_Directory_Enter);
            Label_Directory = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // Button_Select
            // 
            Button_Select.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            Button_Select.Location = new System.Drawing.Point(415, 216);
            Button_Select.Name = "Button_Select";
            Button_Select.Size = new System.Drawing.Size(67, 21);
            Button_Select.TabIndex = 6;
            Button_Select.Text = "选定(&S)";
            // 
            // Button_Cancel
            // 
            Button_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Button_Cancel.Location = new System.Drawing.Point(415, 242);
            Button_Cancel.Name = "Button_Cancel";
            Button_Cancel.Size = new System.Drawing.Size(67, 21);
            Button_Cancel.TabIndex = 9;
            Button_Cancel.Text = "取消";
            // 
            // Button_Enter
            // 
            Button_Enter.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            Button_Enter.Location = new System.Drawing.Point(415, 11);
            Button_Enter.Name = "Button_Enter";
            Button_Enter.Size = new System.Drawing.Size(67, 21);
            Button_Enter.TabIndex = 2;
            Button_Enter.Text = "进入(&E)";
            // 
            // FileListView
            // 
            FileListView.AllowColumnReorder = true;
            FileListView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;

            FileListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { ColumnHeader_Name, ColumnHeader_Length, ColumnHeader_Type, ColumnHeader_ModifyTime, ColumnHeader_CreateTime });
            FileListView.FullRowSelect = true;
            FileListView.Location = new System.Drawing.Point(14, 38);
            FileListView.Name = "FileListView";
            FileListView.ShowGroups = false;
            FileListView.Size = new System.Drawing.Size(468, 171);
            FileListView.TabIndex = 3;
            FileListView.UseCompatibleStateImageBehavior = false;
            FileListView.View = System.Windows.Forms.View.Details;
            FileListView.VirtualMode = true;
            // 
            // ColumnHeader_Name
            // 
            ColumnHeader_Name.Text = "名称";
            ColumnHeader_Name.Width = 300;
            // 
            // ColumnHeader_Length
            // 
            ColumnHeader_Length.Text = "大小";
            ColumnHeader_Length.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ColumnHeader_Type
            // 
            ColumnHeader_Type.Text = "类型";
            ColumnHeader_Type.Width = 110;
            // 
            // ColumnHeader_ModifyTime
            // 
            ColumnHeader_ModifyTime.Text = "修改时间";
            ColumnHeader_ModifyTime.Width = 110;
            // 
            // ColumnHeader_CreateTime
            // 
            ColumnHeader_CreateTime.Text = "创建时间";
            ColumnHeader_CreateTime.Width = 110;
            // 
            // ComboBox_FileName
            // 
            ComboBox_FileName.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            ComboBox_FileName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            ComboBox_FileName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            ComboBox_FileName.FormattingEnabled = true;
            ComboBox_FileName.Location = new System.Drawing.Point(112, 216);
            ComboBox_FileName.Name = "ComboBox_FileName";
            ComboBox_FileName.Size = new System.Drawing.Size(297, 20);
            ComboBox_FileName.TabIndex = 5;
            // 
            // Label_FileName
            // 
            Label_FileName.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            Label_FileName.AutoSize = true;
            Label_FileName.Location = new System.Drawing.Point(12, 219);
            Label_FileName.Name = "Label_FileName";
            Label_FileName.Size = new System.Drawing.Size(65, 12);
            Label_FileName.TabIndex = 4;
            Label_FileName.Text = "文件名(&N):";
            // 
            // ComboBox_Filter
            // 
            ComboBox_Filter.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            ComboBox_Filter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            ComboBox_Filter.FormattingEnabled = true;
            ComboBox_Filter.Location = new System.Drawing.Point(112, 243);
            ComboBox_Filter.Name = "ComboBox_Filter";
            ComboBox_Filter.Size = new System.Drawing.Size(297, 20);
            ComboBox_Filter.TabIndex = 8;
            // 
            // Label_Filter
            // 
            Label_Filter.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            Label_Filter.AutoSize = true;
            Label_Filter.Location = new System.Drawing.Point(12, 246);
            Label_Filter.Name = "Label_Filter";
            Label_Filter.Size = new System.Drawing.Size(77, 12);
            Label_Filter.TabIndex = 7;
            Label_Filter.Text = "文件类型(&T):";
            // 
            // ComboBox_Directory
            // 
            ComboBox_Directory.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            ComboBox_Directory.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            ComboBox_Directory.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            ComboBox_Directory.FormattingEnabled = true;
            ComboBox_Directory.Location = new System.Drawing.Point(112, 12);
            ComboBox_Directory.Name = "ComboBox_Directory";
            ComboBox_Directory.Size = new System.Drawing.Size(297, 20);
            ComboBox_Directory.TabIndex = 1;
            // 
            // Label_Directory
            // 
            Label_Directory.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            Label_Directory.AutoSize = true;
            Label_Directory.Location = new System.Drawing.Point(12, 15);
            Label_Directory.Name = "Label_Directory";
            Label_Directory.Size = new System.Drawing.Size(77, 12);
            Label_Directory.TabIndex = 0;
            Label_Directory.Text = "查找范围(&I):";
            // 
            // FilePicker
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6.0f, 12.0f);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = Button_Cancel;
            ClientSize = new System.Drawing.Size(494, 275);
            Controls.Add(Button_Enter);
            Controls.Add(Label_Filter);
            Controls.Add(Label_Directory);
            Controls.Add(Label_FileName);
            Controls.Add(ComboBox_Filter);
            Controls.Add(ComboBox_Directory);
            Controls.Add(ComboBox_FileName);
            Controls.Add(FileListView);
            Controls.Add(Button_Cancel);
            Controls.Add(Button_Select);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FilePicker";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "FilePicker";
            Load += new System.EventHandler(FilePicker_Load);
            Shown += new System.EventHandler(FilePicker_Shown);
            KeyDown += new System.Windows.Forms.KeyEventHandler(FilePicker_KeyDown);
            ResumeLayout(false);
            PerformLayout();

        }
        internal System.Windows.Forms.Button Button_Select;
        internal System.Windows.Forms.Button Button_Cancel;
        internal System.Windows.Forms.Button Button_Enter;
        internal System.Windows.Forms.ListView FileListView;
        internal System.Windows.Forms.ComboBox ComboBox_FileName;
        internal System.Windows.Forms.Label Label_FileName;
        internal System.Windows.Forms.ComboBox ComboBox_Filter;
        internal System.Windows.Forms.Label Label_Filter;
        internal System.Windows.Forms.ComboBox ComboBox_Directory;
        internal System.Windows.Forms.Label Label_Directory;
        internal System.Windows.Forms.ColumnHeader ColumnHeader_Name;
        internal System.Windows.Forms.ColumnHeader ColumnHeader_Length;
        internal System.Windows.Forms.ColumnHeader ColumnHeader_Type;
        internal System.Windows.Forms.ColumnHeader ColumnHeader_ModifyTime;
        internal System.Windows.Forms.ColumnHeader ColumnHeader_CreateTime;

    }
}