// ==========================================================================
// 
// File:        FileSelectBox.vb
// Location:    Firefly.GUI <Visual Basic .Net>
// Description: 文件选取框
// Version:     2009.11.30.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Firefly;
using static FontGen.FilePicker;

namespace FontGen
{

    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public class FileSelectBox : UserControl
    {
        public FileSelectBox()
        {
            InitializeComponent();
            SplitContainer = _SplitContainer;
            _SplitContainer.Name = "SplitContainer";
            //SplitterDistance = _SplitterDistance;
        }

        // UserControl 重写 Dispose，以清理组件列表。
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
        internal Button Button;
        internal TextBox TextBox;
        internal Label Label;
        private SplitContainer _SplitContainer;

        public virtual SplitContainer SplitContainer
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _SplitContainer;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _SplitContainer = value;
            }
        }
        // Windows 窗体设计器所必需的
        private IContainer components;

        // 注意: 以下过程是 Windows 窗体设计器所必需的
        // 可以使用 Windows 窗体设计器修改它。
        // 不要使用代码编辑器修改它。
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            Button = new Button();
            Button.Click += new EventHandler(Button_Click);
            TextBox = new TextBox();
            TextBox.KeyUp += new KeyEventHandler(TextBox_KeyUp);
            Label = new Label();
            _SplitContainer = new SplitContainer();
            _SplitContainer.Panel1.SuspendLayout();
            _SplitContainer.Panel2.SuspendLayout();
            _SplitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // Button
            // 
            Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            Button.Location = new System.Drawing.Point(296, 2);
            Button.Name = "Button";
            Button.Size = new System.Drawing.Size(34, 23);
            Button.TabIndex = 0;
            Button.Text = "...";
            Button.UseVisualStyleBackColor = true;
            // 
            // TextBox
            // 
            TextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            TextBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            TextBox.AutoCompleteSource = AutoCompleteSource.FileSystemDirectories;
            TextBox.Location = new System.Drawing.Point(3, 3);
            TextBox.Margin = new Padding(0);
            TextBox.Name = "TextBox";
            TextBox.Size = new System.Drawing.Size(290, 21);
            TextBox.TabIndex = 1;
            // 
            // Label
            // 
            Label.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            Label.AutoSize = true;
            Label.Location = new System.Drawing.Point(3, 7);
            Label.Name = "Label";
            Label.Size = new System.Drawing.Size(35, 12);
            Label.TabIndex = 2;
            Label.Text = "Label";
            // 
            // SplitContainer
            // 
            _SplitContainer.Dock = DockStyle.Fill;
            _SplitContainer.FixedPanel = FixedPanel.Panel1;
            _SplitContainer.IsSplitterFixed = true;
            _SplitContainer.Location = new System.Drawing.Point(0, 0);
            _SplitContainer.Name = "_SplitContainer";
            // 
            // SplitContainer.Panel1
            // 
            _SplitContainer.Panel1.Controls.Add(Label);
            // 
            // SplitContainer.Panel2
            // 
            _SplitContainer.Panel2.Controls.Add(TextBox);
            _SplitContainer.Panel2.Controls.Add(Button);
            _SplitContainer.Size = new System.Drawing.Size(379, 27);
            _SplitContainer.SplitterDistance = 45;
            _SplitContainer.SplitterWidth = 1;
            _SplitContainer.TabIndex = 3;
            // 
            // FileSelectBox
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6.0f, 12.0f);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            Controls.Add(_SplitContainer);
            Name = "FileSelectBox";
            Size = new System.Drawing.Size(379, 27);
            _SplitContainer.Panel1.ResumeLayout(false);
            _SplitContainer.Panel1.PerformLayout();
            _SplitContainer.Panel2.ResumeLayout(false);
            _SplitContainer.Panel2.PerformLayout();
            _SplitContainer.ResumeLayout(false);
            ResumeLayout(false);

        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                EnterPressed?.Invoke(this, new EventArgs());
            }
        }

        private FilePicker _PopupDialog_d = default;

        public void PopupDialog()
        {
            string CurrentDirectory = System.IO.Directory.GetCurrentDirectory();
            {
                var withBlock = TextBox;
                if (_PopupDialog_d is null)
                    _PopupDialog_d = new FilePicker(IsSaveDialogValue);
                if (_PopupDialog_d.IsSaveDialog != IsSaveDialogValue)
                    _PopupDialog_d = new FilePicker(IsSaveDialogValue);
                _PopupDialog_d.Multiselect = Multiselect;
                _PopupDialog_d.ModeSelection = ModeSelectionValue;

                string dir = FileNameHandling.GetFileDirectory(withBlock.Text);
                if (System.IO.Directory.Exists(dir))
                {
                    _PopupDialog_d.FilePath = withBlock.Text.TrimEnd('\\').TrimEnd('/');
                }
                _PopupDialog_d.Filter = FilterValue;
                if (_PopupDialog_d.ShowDialog() == DialogResult.OK)
                {
                    string T = FileNameHandling.GetRelativePath(_PopupDialog_d.FilePath, CurrentDirectory);
                    if (!string.IsNullOrEmpty(T) && !string.IsNullOrEmpty(_PopupDialog_d.FilePath) && T.Length < _PopupDialog_d.FilePath.Length)
                    {
                        withBlock.Text = T;
                    }
                    else
                    {
                        withBlock.Text = _PopupDialog_d.FilePath;
                    }
                }
            }
            System.IO.Directory.SetCurrentDirectory( CurrentDirectory);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            PopupDialog();
        }

        private string FilterValue = "(*.*)|*.*";
        [Category("Appearance")]
        public string Filter
        {
            get
            {
                return FilterValue;
            }
            set
            {
                if (value is null)
                    throw new ArgumentException();
                var n = default(int);
                var i = default(int);
                while (true)
                {
                    i = value.IndexOf("|", i + 1);
                    if (i < 0)
                        break;
                    n += 1;
                }
                if ((n & 1) == 0)
                    throw new ArgumentException();
                FilterValue = value;
            }
        }

        [Category("Appearance")]
        public string LabelText
        {
            get
            {
                return Label.Text;
            }
            set
            {
                Label.Text = value;
            }
        }

        [Category("Appearance")]
        public string Path
        {
            get
            {
                return TextBox.Text;
            }
            set
            {
                TextBox.Text = value;
            }
        }

        [Category("Appearance")]
        public int SplitterDistance
        {
            get
            {
                return SplitContainer.SplitterDistance;
            }
            set
            {
                SplitContainer.SplitterDistance = value;
            }
        }

        private bool IsSaveDialogValue = false;
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool IsSaveDialog
        {
            get
            {
                return IsSaveDialogValue;
            }
            set
            {
                IsSaveDialogValue = value;
            }
        }

        private bool MultiselectValue = false;
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool Multiselect
        {
            get
            {
                return MultiselectValue;
            }
            set
            {
                MultiselectValue = value;
            }
        }

        protected ModeSelectionEnum ModeSelectionValue = ModeSelectionEnum.FileWithFolder;
        [Category("Behavior")]
        [DefaultValue(typeof(ModeSelectionEnum), "FileWithFolder")]
        public ModeSelectionEnum ModeSelection
        {
            get
            {
                return ModeSelectionValue;
            }
            set
            {
                switch (value)
                {
                    case ModeSelectionEnum.File:
                    case ModeSelectionEnum.Folder:
                    case ModeSelectionEnum.FileWithFolder:
                        {
                            ModeSelectionValue = value;
                            break;
                        }

                    default:
                        {
                            throw new System.IO.InvalidDataException();
                        }
                }
            }
        }

        public event EnterPressedEventHandler EnterPressed;

        public delegate void EnterPressedEventHandler(object sender, EventArgs e);
    }
}