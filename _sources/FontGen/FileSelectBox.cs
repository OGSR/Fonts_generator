// ==========================================================================
// 
// File:        FileSelectBox.vb
// Location:    Firefly.GUI <Visual Basic .Net>
// Description: File selection box
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

        // User Control overrides Dispose to clean up the component list.
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
        // Required for Windows Forms Designer
        private IContainer components;

        // NOTE: The following procedure is required for Windows Forms Designer
        // This can be modified using Windows Forms Designer.
        // Don't use the code editor to modify it.
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.Button = new System.Windows.Forms.Button();
            this.TextBox = new System.Windows.Forms.TextBox();
            this.Label = new System.Windows.Forms.Label();
            this._SplitContainer = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this._SplitContainer)).BeginInit();
            this._SplitContainer.Panel1.SuspendLayout();
            this._SplitContainer.Panel2.SuspendLayout();
            this._SplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // Button
            // 
            this.Button.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Button.Location = new System.Drawing.Point(141, 2);
            this.Button.Name = "Button";
            this.Button.Size = new System.Drawing.Size(34, 25);
            this.Button.TabIndex = 0;
            this.Button.Text = "...";
            this.Button.UseVisualStyleBackColor = true;
            this.Button.Click += new System.EventHandler(this.Button_Click);
            // 
            // TextBox
            // 
            this.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.TextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.TextBox.Location = new System.Drawing.Point(3, 5);
            this.TextBox.Margin = new System.Windows.Forms.Padding(0);
            this.TextBox.Name = "TextBox";
            this.TextBox.Size = new System.Drawing.Size(135, 20);
            this.TextBox.TabIndex = 1;
            this.TextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            // 
            // Label
            // 
            this.Label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Label.AutoSize = true;
            this.Label.Location = new System.Drawing.Point(3, 8);
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(33, 13);
            this.Label.TabIndex = 2;
            this.Label.Text = "Label";
            // 
            // _SplitContainer
            // 
            this._SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._SplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this._SplitContainer.IsSplitterFixed = true;
            this._SplitContainer.Location = new System.Drawing.Point(0, 0);
            this._SplitContainer.Name = "_SplitContainer";
            // 
            // _SplitContainer.Panel1
            // 
            this._SplitContainer.Panel1.Controls.Add(this.Label);
            this._SplitContainer.Panel1MinSize = 200;
            // 
            // _SplitContainer.Panel2
            // 
            this._SplitContainer.Panel2.Controls.Add(this.TextBox);
            this._SplitContainer.Panel2.Controls.Add(this.Button);
            this._SplitContainer.Size = new System.Drawing.Size(379, 29);
            this._SplitContainer.SplitterDistance = 200;
            this._SplitContainer.SplitterWidth = 1;
            this._SplitContainer.TabIndex = 3;
            // 
            // FileSelectBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this._SplitContainer);
            this.Name = "FileSelectBox";
            this.Size = new System.Drawing.Size(379, 29);
            this._SplitContainer.Panel1.ResumeLayout(false);
            this._SplitContainer.Panel1.PerformLayout();
            this._SplitContainer.Panel2.ResumeLayout(false);
            this._SplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._SplitContainer)).EndInit();
            this._SplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

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