
namespace FontGen
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class FontGen : System.Windows.Forms.Form
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(FontGen));
            Label_FontName = new System.Windows.Forms.Label();
            ComboBox_FontName = new System.Windows.Forms.ComboBox();
            ComboBox_FontName.TextChanged += new System.EventHandler(ComboBox_FontName_TextChanged);
            CheckBox_Bold = new System.Windows.Forms.CheckBox();
            CheckBox_Bold.CheckedChanged += new System.EventHandler(CheckBox_Bold_CheckedChanged);
            CheckBox_Italic = new System.Windows.Forms.CheckBox();
            CheckBox_Italic.CheckedChanged += new System.EventHandler(CheckBox_Italic_CheckedChanged);
            CheckBox_Underline = new System.Windows.Forms.CheckBox();
            CheckBox_Underline.CheckedChanged += new System.EventHandler(CheckBox_Underline_CheckedChanged);
            CheckBox_Strikeout = new System.Windows.Forms.CheckBox();
            CheckBox_Strikeout.CheckedChanged += new System.EventHandler(CheckBox_Strikeout_CheckedChanged);
            NumericUpDown_Size = new System.Windows.Forms.NumericUpDown();
            NumericUpDown_Size.ValueChanged += new System.EventHandler(NumericUpDown_Size_ValueChanged);
            Label_Size = new System.Windows.Forms.Label();
            PictureBox_Preview = new System.Windows.Forms.PictureBox();
            CheckBox_DoubleSample = new System.Windows.Forms.CheckBox();
            CheckBox_DoubleSample.CheckedChanged += new System.EventHandler(CheckBox_DoubleSample_CheckedChanged);
            Label_PhysicalWidth = new System.Windows.Forms.Label();
            NumericUpDown_PhysicalWidth = new System.Windows.Forms.NumericUpDown();
            NumericUpDown_PhysicalWidth.ValueChanged += new System.EventHandler(NumericUpDowns_ValueChanged);
            Label_PhysicalHeight = new System.Windows.Forms.Label();
            NumericUpDown_PhysicalHeight = new System.Windows.Forms.NumericUpDown();
            NumericUpDown_PhysicalHeight.ValueChanged += new System.EventHandler(NumericUpDowns_ValueChanged);
            PictureBox_Preview2x = new System.Windows.Forms.PictureBox();
            SplitContainer_Main = new System.Windows.Forms.SplitContainer();
            Label_DrawOffsetX = new System.Windows.Forms.Label();
            Label_DrawOffsetY = new System.Windows.Forms.Label();
            NumericUpDown_DrawOffsetX = new System.Windows.Forms.NumericUpDown();
            NumericUpDown_DrawOffsetX.ValueChanged += new System.EventHandler(NumericUpDowns_ValueChanged);
            NumericUpDown_DrawOffsetY = new System.Windows.Forms.NumericUpDown();
            NumericUpDown_DrawOffsetY.ValueChanged += new System.EventHandler(NumericUpDowns_ValueChanged);
            Label_VirtualOffsetX = new System.Windows.Forms.Label();
            Label_VirtualOffsetY = new System.Windows.Forms.Label();
            NumericUpDown_VirtualOffsetX = new System.Windows.Forms.NumericUpDown();
            NumericUpDown_VirtualOffsetX.ValueChanged += new System.EventHandler(NumericUpDowns_ValueChanged);
            NumericUpDown_VirtualOffsetY = new System.Windows.Forms.NumericUpDown();
            NumericUpDown_VirtualOffsetY.ValueChanged += new System.EventHandler(NumericUpDowns_ValueChanged);
            Label_VirtualDeltaWidth = new System.Windows.Forms.Label();
            Label_VirtualDeltaHeight = new System.Windows.Forms.Label();
            NumericUpDown_VirtualDeltaWidth = new System.Windows.Forms.NumericUpDown();
            NumericUpDown_VirtualDeltaWidth.ValueChanged += new System.EventHandler(NumericUpDowns_ValueChanged);
            NumericUpDown_VirtualDeltaHeight = new System.Windows.Forms.NumericUpDown();
            NumericUpDown_VirtualDeltaHeight.ValueChanged += new System.EventHandler(NumericUpDowns_ValueChanged);
            Button_Generate = new System.Windows.Forms.Button();
            Button_Generate.Click += new System.EventHandler(Button_Generate_Click);
            FileSelectBox_File = new FileSelectBox();
            Button_CmdToClipboard = new System.Windows.Forms.Button();
            Button_CmdToClipboard.Click += new System.EventHandler(Button_CmdToClipboard_Click);
            CheckBox_AnchorLeft = new System.Windows.Forms.CheckBox();
            CheckBox_AnchorLeft.CheckedChanged += new System.EventHandler(CheckBox_DoubleSample_CheckedChanged);
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_Size).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PictureBox_Preview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_PhysicalWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_PhysicalHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PictureBox_Preview2x).BeginInit();
            SplitContainer_Main.Panel1.SuspendLayout();
            SplitContainer_Main.Panel2.SuspendLayout();
            SplitContainer_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_DrawOffsetX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_DrawOffsetY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_VirtualOffsetX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_VirtualOffsetY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_VirtualDeltaWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_VirtualDeltaHeight).BeginInit();
            SuspendLayout();
            // 
            // Label_FontName
            // 
            Label_FontName.AutoSize = true;
            Label_FontName.Location = new System.Drawing.Point(13, 13);
            Label_FontName.Name = "Label_FontName";
            Label_FontName.Size = new System.Drawing.Size(53, 12);
            Label_FontName.TabIndex = 0;
            Label_FontName.Text = "字体名称";
            // 
            // ComboBox_FontName
            // 
            ComboBox_FontName.FormattingEnabled = true;
            ComboBox_FontName.Location = new System.Drawing.Point(15, 29);
            ComboBox_FontName.Name = "ComboBox_FontName";
            ComboBox_FontName.Size = new System.Drawing.Size(120, 20);
            ComboBox_FontName.TabIndex = 1;
            ComboBox_FontName.Text = "宋体";
            // 
            // CheckBox_Bold
            // 
            CheckBox_Bold.AutoSize = true;
            CheckBox_Bold.Location = new System.Drawing.Point(15, 94);
            CheckBox_Bold.Name = "CheckBox_Bold";
            CheckBox_Bold.Size = new System.Drawing.Size(48, 16);
            CheckBox_Bold.TabIndex = 2;
            CheckBox_Bold.Text = "加粗";
            CheckBox_Bold.UseVisualStyleBackColor = true;
            // 
            // CheckBox_Italic
            // 
            CheckBox_Italic.AutoSize = true;
            CheckBox_Italic.Location = new System.Drawing.Point(78, 94);
            CheckBox_Italic.Name = "CheckBox_Italic";
            CheckBox_Italic.Size = new System.Drawing.Size(48, 16);
            CheckBox_Italic.TabIndex = 2;
            CheckBox_Italic.Text = "斜体";
            CheckBox_Italic.UseVisualStyleBackColor = true;
            // 
            // CheckBox_Underline
            // 
            CheckBox_Underline.AutoSize = true;
            CheckBox_Underline.Location = new System.Drawing.Point(15, 116);
            CheckBox_Underline.Name = "CheckBox_Underline";
            CheckBox_Underline.Size = new System.Drawing.Size(60, 16);
            CheckBox_Underline.TabIndex = 2;
            CheckBox_Underline.Text = "下划线";
            CheckBox_Underline.UseVisualStyleBackColor = true;
            // 
            // CheckBox_Strikeout
            // 
            CheckBox_Strikeout.AutoSize = true;
            CheckBox_Strikeout.Location = new System.Drawing.Point(78, 116);
            CheckBox_Strikeout.Name = "CheckBox_Strikeout";
            CheckBox_Strikeout.Size = new System.Drawing.Size(60, 16);
            CheckBox_Strikeout.TabIndex = 2;
            CheckBox_Strikeout.Text = "删除线";
            CheckBox_Strikeout.UseVisualStyleBackColor = true;
            // 
            // NumericUpDown_Size
            // 
            NumericUpDown_Size.Location = new System.Drawing.Point(15, 67);
            NumericUpDown_Size.Maximum = new decimal(new int[] { 65536, 0, 0, 0 });
            NumericUpDown_Size.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NumericUpDown_Size.Name = "NumericUpDown_Size";
            NumericUpDown_Size.Size = new System.Drawing.Size(120, 21);
            NumericUpDown_Size.TabIndex = 3;
            NumericUpDown_Size.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            NumericUpDown_Size.Value = new decimal(new int[] { 16, 0, 0, 0 });
            // 
            // Label_Size
            // 
            Label_Size.AutoSize = true;
            Label_Size.Location = new System.Drawing.Point(13, 52);
            Label_Size.Name = "Label_Size";
            Label_Size.Size = new System.Drawing.Size(53, 12);
            Label_Size.TabIndex = 0;
            Label_Size.Text = "字体大小";
            // 
            // PictureBox_Preview
            // 
            PictureBox_Preview.BackColor = System.Drawing.Color.White;
            PictureBox_Preview.Dock = System.Windows.Forms.DockStyle.Fill;
            PictureBox_Preview.Location = new System.Drawing.Point(0, 0);
            PictureBox_Preview.Name = "PictureBox_Preview";
            PictureBox_Preview.Size = new System.Drawing.Size(479, 129);
            PictureBox_Preview.TabIndex = 4;
            PictureBox_Preview.TabStop = false;
            // 
            // CheckBox_DoubleSample
            // 
            CheckBox_DoubleSample.AutoSize = true;
            CheckBox_DoubleSample.Location = new System.Drawing.Point(15, 138);
            CheckBox_DoubleSample.Name = "CheckBox_DoubleSample";
            CheckBox_DoubleSample.Size = new System.Drawing.Size(60, 16);
            CheckBox_DoubleSample.TabIndex = 2;
            CheckBox_DoubleSample.Text = "2x采样";
            CheckBox_DoubleSample.UseVisualStyleBackColor = true;
            // 
            // Label_PhysicalWidth
            // 
            Label_PhysicalWidth.AutoSize = true;
            Label_PhysicalWidth.Location = new System.Drawing.Point(13, 157);
            Label_PhysicalWidth.Name = "Label_PhysicalWidth";
            Label_PhysicalWidth.Size = new System.Drawing.Size(53, 12);
            Label_PhysicalWidth.TabIndex = 0;
            Label_PhysicalWidth.Text = "物理宽度";
            // 
            // NumericUpDown_PhysicalWidth
            // 
            NumericUpDown_PhysicalWidth.Location = new System.Drawing.Point(15, 171);
            NumericUpDown_PhysicalWidth.Maximum = new decimal(new int[] { 65536, 0, 0, 0 });
            NumericUpDown_PhysicalWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NumericUpDown_PhysicalWidth.Name = "NumericUpDown_PhysicalWidth";
            NumericUpDown_PhysicalWidth.Size = new System.Drawing.Size(57, 21);
            NumericUpDown_PhysicalWidth.TabIndex = 3;
            NumericUpDown_PhysicalWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            NumericUpDown_PhysicalWidth.Value = new decimal(new int[] { 16, 0, 0, 0 });
            // 
            // Label_PhysicalHeight
            // 
            Label_PhysicalHeight.AutoSize = true;
            Label_PhysicalHeight.Location = new System.Drawing.Point(76, 157);
            Label_PhysicalHeight.Name = "Label_PhysicalHeight";
            Label_PhysicalHeight.Size = new System.Drawing.Size(53, 12);
            Label_PhysicalHeight.TabIndex = 0;
            Label_PhysicalHeight.Text = "物理高度";
            // 
            // NumericUpDown_PhysicalHeight
            // 
            NumericUpDown_PhysicalHeight.Location = new System.Drawing.Point(78, 172);
            NumericUpDown_PhysicalHeight.Maximum = new decimal(new int[] { 65536, 0, 0, 0 });
            NumericUpDown_PhysicalHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NumericUpDown_PhysicalHeight.Name = "NumericUpDown_PhysicalHeight";
            NumericUpDown_PhysicalHeight.Size = new System.Drawing.Size(57, 21);
            NumericUpDown_PhysicalHeight.TabIndex = 3;
            NumericUpDown_PhysicalHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            NumericUpDown_PhysicalHeight.Value = new decimal(new int[] { 16, 0, 0, 0 });
            // 
            // PictureBox_Preview2x
            // 
            PictureBox_Preview2x.BackColor = System.Drawing.Color.White;
            PictureBox_Preview2x.Dock = System.Windows.Forms.DockStyle.Fill;
            PictureBox_Preview2x.Location = new System.Drawing.Point(0, 0);
            PictureBox_Preview2x.Name = "PictureBox_Preview2x";
            PictureBox_Preview2x.Size = new System.Drawing.Size(479, 258);
            PictureBox_Preview2x.TabIndex = 4;
            PictureBox_Preview2x.TabStop = false;
            // 
            // SplitContainer_Main
            // 
            SplitContainer_Main.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;

            SplitContainer_Main.Location = new System.Drawing.Point(141, 46);
            SplitContainer_Main.Name = "SplitContainer_Main";
            SplitContainer_Main.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainer_Main.Panel1
            // 
            SplitContainer_Main.Panel1.Controls.Add(PictureBox_Preview);
            // 
            // SplitContainer_Main.Panel2
            // 
            SplitContainer_Main.Panel2.Controls.Add(PictureBox_Preview2x);
            SplitContainer_Main.Size = new System.Drawing.Size(479, 391);
            SplitContainer_Main.SplitterDistance = 129;
            SplitContainer_Main.TabIndex = 5;
            // 
            // Label_DrawOffsetX
            // 
            Label_DrawOffsetX.AutoSize = true;
            Label_DrawOffsetX.Location = new System.Drawing.Point(13, 196);
            Label_DrawOffsetX.Name = "Label_DrawOffsetX";
            Label_DrawOffsetX.Size = new System.Drawing.Size(59, 12);
            Label_DrawOffsetX.TabIndex = 0;
            Label_DrawOffsetX.Text = "绘制X偏移";
            // 
            // Label_DrawOffsetY
            // 
            Label_DrawOffsetY.AutoSize = true;
            Label_DrawOffsetY.Location = new System.Drawing.Point(76, 196);
            Label_DrawOffsetY.Name = "Label_DrawOffsetY";
            Label_DrawOffsetY.Size = new System.Drawing.Size(59, 12);
            Label_DrawOffsetY.TabIndex = 0;
            Label_DrawOffsetY.Text = "绘制Y偏移";
            // 
            // NumericUpDown_DrawOffsetX
            // 
            NumericUpDown_DrawOffsetX.Location = new System.Drawing.Point(15, 211);
            NumericUpDown_DrawOffsetX.Maximum = new decimal(new int[] { 65536, 0, 0, 0 });
            NumericUpDown_DrawOffsetX.Minimum = new decimal(new int[] { 65535, 0, 0, (int)-2147483648L });
            NumericUpDown_DrawOffsetX.Name = "NumericUpDown_DrawOffsetX";
            NumericUpDown_DrawOffsetX.Size = new System.Drawing.Size(57, 21);
            NumericUpDown_DrawOffsetX.TabIndex = 3;
            NumericUpDown_DrawOffsetX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // NumericUpDown_DrawOffsetY
            // 
            NumericUpDown_DrawOffsetY.Location = new System.Drawing.Point(78, 211);
            NumericUpDown_DrawOffsetY.Maximum = new decimal(new int[] { 65536, 0, 0, 0 });
            NumericUpDown_DrawOffsetY.Minimum = new decimal(new int[] { 65535, 0, 0, (int)-2147483648L });
            NumericUpDown_DrawOffsetY.Name = "NumericUpDown_DrawOffsetY";
            NumericUpDown_DrawOffsetY.Size = new System.Drawing.Size(57, 21);
            NumericUpDown_DrawOffsetY.TabIndex = 3;
            NumericUpDown_DrawOffsetY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Label_VirtualOffsetX
            // 
            Label_VirtualOffsetX.AutoSize = true;
            Label_VirtualOffsetX.Location = new System.Drawing.Point(13, 235);
            Label_VirtualOffsetX.Name = "Label_VirtualOffsetX";
            Label_VirtualOffsetX.Size = new System.Drawing.Size(59, 12);
            Label_VirtualOffsetX.TabIndex = 0;
            Label_VirtualOffsetX.Text = "虚拟X偏移";
            // 
            // Label_VirtualOffsetY
            // 
            Label_VirtualOffsetY.AutoSize = true;
            Label_VirtualOffsetY.Location = new System.Drawing.Point(76, 235);
            Label_VirtualOffsetY.Name = "Label_VirtualOffsetY";
            Label_VirtualOffsetY.Size = new System.Drawing.Size(59, 12);
            Label_VirtualOffsetY.TabIndex = 0;
            Label_VirtualOffsetY.Text = "虚拟Y偏移";
            // 
            // NumericUpDown_VirtualOffsetX
            // 
            NumericUpDown_VirtualOffsetX.Location = new System.Drawing.Point(15, 250);
            NumericUpDown_VirtualOffsetX.Maximum = new decimal(new int[] { 65536, 0, 0, 0 });
            NumericUpDown_VirtualOffsetX.Minimum = new decimal(new int[] { 65535, 0, 0, (int)-2147483648L });
            NumericUpDown_VirtualOffsetX.Name = "NumericUpDown_VirtualOffsetX";
            NumericUpDown_VirtualOffsetX.Size = new System.Drawing.Size(57, 21);
            NumericUpDown_VirtualOffsetX.TabIndex = 3;
            NumericUpDown_VirtualOffsetX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // NumericUpDown_VirtualOffsetY
            // 
            NumericUpDown_VirtualOffsetY.Location = new System.Drawing.Point(78, 250);
            NumericUpDown_VirtualOffsetY.Maximum = new decimal(new int[] { 65536, 0, 0, 0 });
            NumericUpDown_VirtualOffsetY.Minimum = new decimal(new int[] { 65535, 0, 0, (int)-2147483648L });
            NumericUpDown_VirtualOffsetY.Name = "NumericUpDown_VirtualOffsetY";
            NumericUpDown_VirtualOffsetY.Size = new System.Drawing.Size(57, 21);
            NumericUpDown_VirtualOffsetY.TabIndex = 3;
            NumericUpDown_VirtualOffsetY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Label_VirtualDeltaWidth
            // 
            Label_VirtualDeltaWidth.AutoSize = true;
            Label_VirtualDeltaWidth.Location = new System.Drawing.Point(13, 274);
            Label_VirtualDeltaWidth.Name = "Label_VirtualDeltaWidth";
            Label_VirtualDeltaWidth.Size = new System.Drawing.Size(65, 12);
            Label_VirtualDeltaWidth.TabIndex = 0;
            Label_VirtualDeltaWidth.Text = "虚拟宽度差";
            // 
            // Label_VirtualDeltaHeight
            // 
            Label_VirtualDeltaHeight.AutoSize = true;
            Label_VirtualDeltaHeight.Location = new System.Drawing.Point(76, 274);
            Label_VirtualDeltaHeight.Name = "Label_VirtualDeltaHeight";
            Label_VirtualDeltaHeight.Size = new System.Drawing.Size(65, 12);
            Label_VirtualDeltaHeight.TabIndex = 0;
            Label_VirtualDeltaHeight.Text = "虚拟高度差";
            // 
            // NumericUpDown_VirtualDeltaWidth
            // 
            NumericUpDown_VirtualDeltaWidth.Location = new System.Drawing.Point(15, 289);
            NumericUpDown_VirtualDeltaWidth.Maximum = new decimal(new int[] { 65536, 0, 0, 0 });
            NumericUpDown_VirtualDeltaWidth.Minimum = new decimal(new int[] { 65535, 0, 0, (int)-2147483648L });
            NumericUpDown_VirtualDeltaWidth.Name = "NumericUpDown_VirtualDeltaWidth";
            NumericUpDown_VirtualDeltaWidth.Size = new System.Drawing.Size(57, 21);
            NumericUpDown_VirtualDeltaWidth.TabIndex = 3;
            NumericUpDown_VirtualDeltaWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // NumericUpDown_VirtualDeltaHeight
            // 
            NumericUpDown_VirtualDeltaHeight.Location = new System.Drawing.Point(78, 289);
            NumericUpDown_VirtualDeltaHeight.Maximum = new decimal(new int[] { 65536, 0, 0, 0 });
            NumericUpDown_VirtualDeltaHeight.Minimum = new decimal(new int[] { 65535, 0, 0, (int)-2147483648L });
            NumericUpDown_VirtualDeltaHeight.Name = "NumericUpDown_VirtualDeltaHeight";
            NumericUpDown_VirtualDeltaHeight.Size = new System.Drawing.Size(57, 21);
            NumericUpDown_VirtualDeltaHeight.TabIndex = 3;
            NumericUpDown_VirtualDeltaHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Button_Generate
            // 
            Button_Generate.Location = new System.Drawing.Point(15, 345);
            Button_Generate.Name = "Button_Generate";
            Button_Generate.Size = new System.Drawing.Size(120, 23);
            Button_Generate.TabIndex = 7;
            Button_Generate.Text = "生成";
            Button_Generate.UseVisualStyleBackColor = true;
            // 
            // FileSelectBox_File
            // 
            FileSelectBox_File.AutoSize = true;
            FileSelectBox_File.Filter = "tbl码表文件(*.tbl)|*.tbl|fd字符描述文件(*.fd)|*.fd|字符文件(*.txt)|*.txt";
            FileSelectBox_File.LabelText = "tbl/fd/字符文件";
            FileSelectBox_File.Location = new System.Drawing.Point(141, 13);
            FileSelectBox_File.Name = "FileSelectBox_File";
            FileSelectBox_File.Path = "";
            FileSelectBox_File.Size = new System.Drawing.Size(479, 27);
            FileSelectBox_File.SplitterDistance = 100;
            FileSelectBox_File.TabIndex = 6;
            // 
            // Button_CmdToClipboard
            // 
            Button_CmdToClipboard.Location = new System.Drawing.Point(15, 316);
            Button_CmdToClipboard.Name = "Button_CmdToClipboard";
            Button_CmdToClipboard.Size = new System.Drawing.Size(120, 23);
            Button_CmdToClipboard.TabIndex = 8;
            Button_CmdToClipboard.Text = "传命令行到剪贴板";
            Button_CmdToClipboard.UseVisualStyleBackColor = true;
            // 
            // CheckBox_AnchorLeft
            // 
            CheckBox_AnchorLeft.AutoSize = true;
            CheckBox_AnchorLeft.Location = new System.Drawing.Point(78, 138);
            CheckBox_AnchorLeft.Name = "CheckBox_AnchorLeft";
            CheckBox_AnchorLeft.Size = new System.Drawing.Size(60, 16);
            CheckBox_AnchorLeft.TabIndex = 2;
            CheckBox_AnchorLeft.Text = "左对齐";
            CheckBox_AnchorLeft.UseVisualStyleBackColor = true;
            // 
            // FontGen
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6.0f, 12.0f);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(632, 446);
            Controls.Add(Button_CmdToClipboard);
            Controls.Add(Button_Generate);
            Controls.Add(FileSelectBox_File);
            Controls.Add(SplitContainer_Main);
            Controls.Add(NumericUpDown_VirtualDeltaHeight);
            Controls.Add(NumericUpDown_VirtualDeltaWidth);
            Controls.Add(NumericUpDown_VirtualOffsetY);
            Controls.Add(NumericUpDown_VirtualOffsetX);
            Controls.Add(NumericUpDown_DrawOffsetY);
            Controls.Add(NumericUpDown_DrawOffsetX);
            Controls.Add(NumericUpDown_PhysicalHeight);
            Controls.Add(NumericUpDown_PhysicalWidth);
            Controls.Add(NumericUpDown_Size);
            Controls.Add(Label_VirtualDeltaHeight);
            Controls.Add(CheckBox_Strikeout);
            Controls.Add(Label_VirtualOffsetY);
            Controls.Add(CheckBox_Underline);
            Controls.Add(Label_DrawOffsetY);
            Controls.Add(Label_VirtualDeltaWidth);
            Controls.Add(CheckBox_Italic);
            Controls.Add(Label_VirtualOffsetX);
            Controls.Add(Label_PhysicalHeight);
            Controls.Add(Label_DrawOffsetX);
            Controls.Add(CheckBox_Bold);
            Controls.Add(Label_PhysicalWidth);
            Controls.Add(ComboBox_FontName);
            Controls.Add(Label_Size);
            Controls.Add(Label_FontName);
            Controls.Add(CheckBox_AnchorLeft);
            Controls.Add(CheckBox_DoubleSample);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "FontGen";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "字库图片生成器";
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_Size).EndInit();
            ((System.ComponentModel.ISupportInitialize)PictureBox_Preview).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_PhysicalWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_PhysicalHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)PictureBox_Preview2x).EndInit();
            SplitContainer_Main.Panel1.ResumeLayout(false);
            SplitContainer_Main.Panel2.ResumeLayout(false);
            SplitContainer_Main.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_DrawOffsetX).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_DrawOffsetY).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_VirtualOffsetX).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_VirtualOffsetY).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_VirtualDeltaWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_VirtualDeltaHeight).EndInit();
            Shown += new System.EventHandler(FontGen_Shown);
            SizeChanged += new System.EventHandler(FontGen_SizeChanged);
            ResumeLayout(false);
            PerformLayout();

        }
        internal System.Windows.Forms.Label Label_FontName;
        internal System.Windows.Forms.ComboBox ComboBox_FontName;
        internal System.Windows.Forms.CheckBox CheckBox_Bold;
        internal System.Windows.Forms.CheckBox CheckBox_Italic;
        internal System.Windows.Forms.CheckBox CheckBox_Underline;
        internal System.Windows.Forms.CheckBox CheckBox_Strikeout;
        internal System.Windows.Forms.NumericUpDown NumericUpDown_Size;
        internal System.Windows.Forms.Label Label_Size;
        internal System.Windows.Forms.PictureBox PictureBox_Preview;
        internal System.Windows.Forms.CheckBox CheckBox_DoubleSample;
        internal System.Windows.Forms.Label Label_PhysicalWidth;
        internal System.Windows.Forms.NumericUpDown NumericUpDown_PhysicalWidth;
        internal System.Windows.Forms.Label Label_PhysicalHeight;
        internal System.Windows.Forms.NumericUpDown NumericUpDown_PhysicalHeight;
        internal System.Windows.Forms.PictureBox PictureBox_Preview2x;
        internal System.Windows.Forms.SplitContainer SplitContainer_Main;
        internal System.Windows.Forms.Label Label_DrawOffsetX;
        internal System.Windows.Forms.Label Label_DrawOffsetY;
        internal System.Windows.Forms.NumericUpDown NumericUpDown_DrawOffsetX;
        internal System.Windows.Forms.NumericUpDown NumericUpDown_DrawOffsetY;
        internal System.Windows.Forms.Label Label_VirtualOffsetX;
        internal System.Windows.Forms.Label Label_VirtualOffsetY;
        internal System.Windows.Forms.NumericUpDown NumericUpDown_VirtualOffsetX;
        internal System.Windows.Forms.NumericUpDown NumericUpDown_VirtualOffsetY;
        internal System.Windows.Forms.Label Label_VirtualDeltaWidth;
        internal System.Windows.Forms.Label Label_VirtualDeltaHeight;
        internal System.Windows.Forms.NumericUpDown NumericUpDown_VirtualDeltaWidth;
        internal System.Windows.Forms.NumericUpDown NumericUpDown_VirtualDeltaHeight;
        internal FileSelectBox FileSelectBox_File;
        internal System.Windows.Forms.Button Button_Generate;
        internal System.Windows.Forms.Button Button_CmdToClipboard;
        internal System.Windows.Forms.CheckBox CheckBox_AnchorLeft;
    }
}