
namespace FontGen
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class FontGenForm : System.Windows.Forms.Form
    {

        // Form overrides Dispose to clean up the component list.
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

        // Required for Windows Forms Designer
        private System.ComponentModel.IContainer components;

        // NOTE: The following procedure is required for Windows Forms Designer
        // This can be modified using Windows Forms Designer.
        // Don't use the code editor to modify it.
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FontGenForm));
            this.Label_FontName = new System.Windows.Forms.Label();
            this.ComboBox_FontName = new System.Windows.Forms.ComboBox();
            this.CheckBox_Bold = new System.Windows.Forms.CheckBox();
            this.CheckBox_Italic = new System.Windows.Forms.CheckBox();
            this.CheckBox_Underline = new System.Windows.Forms.CheckBox();
            this.CheckBox_Strikeout = new System.Windows.Forms.CheckBox();
            this.NumericUpDown_Size = new System.Windows.Forms.NumericUpDown();
            this.Label_Size = new System.Windows.Forms.Label();
            this.CheckBox_DoubleSample = new System.Windows.Forms.CheckBox();
            this.Label_PhysicalWidth = new System.Windows.Forms.Label();
            this.NumericUpDown_PhysicalWidth = new System.Windows.Forms.NumericUpDown();
            this.Label_PhysicalHeight = new System.Windows.Forms.Label();
            this.NumericUpDown_PhysicalHeight = new System.Windows.Forms.NumericUpDown();
            this.SplitContainer_Main = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.PictureBox_Preview = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.PictureBox_Preview2x = new System.Windows.Forms.PictureBox();
            this.Label_DrawOffsetX = new System.Windows.Forms.Label();
            this.Label_DrawOffsetY = new System.Windows.Forms.Label();
            this.NumericUpDown_DrawOffsetX = new System.Windows.Forms.NumericUpDown();
            this.NumericUpDown_DrawOffsetY = new System.Windows.Forms.NumericUpDown();
            this.Label_VirtualOffsetX = new System.Windows.Forms.Label();
            this.Label_VirtualOffsetY = new System.Windows.Forms.Label();
            this.NumericUpDown_VirtualOffsetX = new System.Windows.Forms.NumericUpDown();
            this.NumericUpDown_VirtualOffsetY = new System.Windows.Forms.NumericUpDown();
            this.Label_VirtualDeltaWidth = new System.Windows.Forms.Label();
            this.Label_VirtualDeltaHeight = new System.Windows.Forms.Label();
            this.NumericUpDown_VirtualDeltaWidth = new System.Windows.Forms.NumericUpDown();
            this.NumericUpDown_VirtualDeltaHeight = new System.Windows.Forms.NumericUpDown();
            this.Button_Generate = new System.Windows.Forms.Button();
            this.Button_CmdToClipboard = new System.Windows.Forms.Button();
            this.CheckBox_AnchorLeft = new System.Windows.Forms.CheckBox();
            this.ddlBPP = new System.Windows.Forms.ComboBox();
            this.chkDrawAlpha = new System.Windows.Forms.CheckBox();
            this.btnCustomFont = new System.Windows.Forms.Button();
            this.lblCustomFontName = new System.Windows.Forms.Label();
            this.txtTargetPath = new System.Windows.Forms.TextBox();
            this.FileSelectBox_File = new FontGen.FileSelectBox();
            this.chkDelTemp = new System.Windows.Forms.CheckBox();
            this.btnEditText = new System.Windows.Forms.Button();
            this.btnAutosize = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Size)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_PhysicalWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_PhysicalHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer_Main)).BeginInit();
            this.SplitContainer_Main.Panel1.SuspendLayout();
            this.SplitContainer_Main.Panel2.SuspendLayout();
            this.SplitContainer_Main.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Preview)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Preview2x)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_DrawOffsetX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_DrawOffsetY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_VirtualOffsetX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_VirtualOffsetY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_VirtualDeltaWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_VirtualDeltaHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 424);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(57, 13);
            label1.TabIndex = 10;
            label1.Text = "BitPerPixel";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(225, 58);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(86, 13);
            label2.TabIndex = 15;
            label2.Text = "Target Directory:";
            // 
            // Label_FontName
            // 
            this.Label_FontName.AutoSize = true;
            this.Label_FontName.Location = new System.Drawing.Point(13, 14);
            this.Label_FontName.Name = "Label_FontName";
            this.Label_FontName.Size = new System.Drawing.Size(57, 13);
            this.Label_FontName.TabIndex = 0;
            this.Label_FontName.Text = "Font name";
            // 
            // ComboBox_FontName
            // 
            this.ComboBox_FontName.DisplayMember = "Name";
            this.ComboBox_FontName.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.ComboBox_FontName.DropDownHeight = 600;
            this.ComboBox_FontName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_FontName.DropDownWidth = 400;
            this.ComboBox_FontName.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ComboBox_FontName.FormattingEnabled = true;
            this.ComboBox_FontName.IntegralHeight = false;
            this.ComboBox_FontName.Location = new System.Drawing.Point(15, 31);
            this.ComboBox_FontName.MaxDropDownItems = 20;
            this.ComboBox_FontName.Name = "ComboBox_FontName";
            this.ComboBox_FontName.Size = new System.Drawing.Size(194, 33);
            this.ComboBox_FontName.TabIndex = 1;
            this.ComboBox_FontName.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ComboBox_FontName_DrawItem);
            this.ComboBox_FontName.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.ComboBox_FontName_MeasureItem);
            this.ComboBox_FontName.SelectedValueChanged += new System.EventHandler(this.ComboBox_FontName_SelectedValueChanged);
            // 
            // CheckBox_Bold
            // 
            this.CheckBox_Bold.AutoSize = true;
            this.CheckBox_Bold.Location = new System.Drawing.Point(14, 152);
            this.CheckBox_Bold.Name = "CheckBox_Bold";
            this.CheckBox_Bold.Size = new System.Drawing.Size(47, 17);
            this.CheckBox_Bold.TabIndex = 2;
            this.CheckBox_Bold.Text = "Bold";
            this.CheckBox_Bold.UseVisualStyleBackColor = true;
            this.CheckBox_Bold.CheckedChanged += new System.EventHandler(this.CheckBox_Bold_CheckedChanged);
            // 
            // CheckBox_Italic
            // 
            this.CheckBox_Italic.AutoSize = true;
            this.CheckBox_Italic.Location = new System.Drawing.Point(102, 152);
            this.CheckBox_Italic.Name = "CheckBox_Italic";
            this.CheckBox_Italic.Size = new System.Drawing.Size(48, 17);
            this.CheckBox_Italic.TabIndex = 2;
            this.CheckBox_Italic.Text = "Italic";
            this.CheckBox_Italic.UseVisualStyleBackColor = true;
            this.CheckBox_Italic.CheckedChanged += new System.EventHandler(this.CheckBox_Italic_CheckedChanged);
            // 
            // CheckBox_Underline
            // 
            this.CheckBox_Underline.AutoSize = true;
            this.CheckBox_Underline.Location = new System.Drawing.Point(14, 176);
            this.CheckBox_Underline.Name = "CheckBox_Underline";
            this.CheckBox_Underline.Size = new System.Drawing.Size(71, 17);
            this.CheckBox_Underline.TabIndex = 2;
            this.CheckBox_Underline.Text = "Underline";
            this.CheckBox_Underline.UseVisualStyleBackColor = true;
            this.CheckBox_Underline.CheckedChanged += new System.EventHandler(this.CheckBox_Underline_CheckedChanged);
            // 
            // CheckBox_Strikeout
            // 
            this.CheckBox_Strikeout.AutoSize = true;
            this.CheckBox_Strikeout.Location = new System.Drawing.Point(102, 176);
            this.CheckBox_Strikeout.Name = "CheckBox_Strikeout";
            this.CheckBox_Strikeout.Size = new System.Drawing.Size(68, 17);
            this.CheckBox_Strikeout.TabIndex = 2;
            this.CheckBox_Strikeout.Text = "Strikeout";
            this.CheckBox_Strikeout.UseVisualStyleBackColor = true;
            this.CheckBox_Strikeout.CheckedChanged += new System.EventHandler(this.CheckBox_Strikeout_CheckedChanged);
            // 
            // NumericUpDown_Size
            // 
            this.NumericUpDown_Size.Location = new System.Drawing.Point(14, 122);
            this.NumericUpDown_Size.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.NumericUpDown_Size.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDown_Size.Name = "NumericUpDown_Size";
            this.NumericUpDown_Size.Size = new System.Drawing.Size(59, 20);
            this.NumericUpDown_Size.TabIndex = 3;
            this.NumericUpDown_Size.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_Size.Value = new decimal(new int[] {
            48,
            0,
            0,
            0});
            this.NumericUpDown_Size.ValueChanged += new System.EventHandler(this.NumericUpDown_Size_ValueChanged);
            // 
            // Label_Size
            // 
            this.Label_Size.AutoSize = true;
            this.Label_Size.Location = new System.Drawing.Point(12, 106);
            this.Label_Size.Name = "Label_Size";
            this.Label_Size.Size = new System.Drawing.Size(27, 13);
            this.Label_Size.TabIndex = 0;
            this.Label_Size.Text = "Size";
            // 
            // CheckBox_DoubleSample
            // 
            this.CheckBox_DoubleSample.AutoSize = true;
            this.CheckBox_DoubleSample.Checked = true;
            this.CheckBox_DoubleSample.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_DoubleSample.Location = new System.Drawing.Point(14, 228);
            this.CheckBox_DoubleSample.Name = "CheckBox_DoubleSample";
            this.CheckBox_DoubleSample.Size = new System.Drawing.Size(78, 17);
            this.CheckBox_DoubleSample.TabIndex = 2;
            this.CheckBox_DoubleSample.Text = "2xsampling";
            this.CheckBox_DoubleSample.UseVisualStyleBackColor = true;
            this.CheckBox_DoubleSample.CheckedChanged += new System.EventHandler(this.CheckBox_DoubleSample_CheckedChanged);
            // 
            // Label_PhysicalWidth
            // 
            this.Label_PhysicalWidth.AutoSize = true;
            this.Label_PhysicalWidth.Location = new System.Drawing.Point(12, 248);
            this.Label_PhysicalWidth.Name = "Label_PhysicalWidth";
            this.Label_PhysicalWidth.Size = new System.Drawing.Size(74, 13);
            this.Label_PhysicalWidth.TabIndex = 0;
            this.Label_PhysicalWidth.Text = "PhysicalWidth";
            // 
            // NumericUpDown_PhysicalWidth
            // 
            this.NumericUpDown_PhysicalWidth.Location = new System.Drawing.Point(14, 263);
            this.NumericUpDown_PhysicalWidth.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.NumericUpDown_PhysicalWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDown_PhysicalWidth.Name = "NumericUpDown_PhysicalWidth";
            this.NumericUpDown_PhysicalWidth.Size = new System.Drawing.Size(57, 20);
            this.NumericUpDown_PhysicalWidth.TabIndex = 3;
            this.NumericUpDown_PhysicalWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_PhysicalWidth.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.NumericUpDown_PhysicalWidth.ValueChanged += new System.EventHandler(this.NumericUpDowns_ValueChanged);
            // 
            // Label_PhysicalHeight
            // 
            this.Label_PhysicalHeight.AutoSize = true;
            this.Label_PhysicalHeight.Location = new System.Drawing.Point(100, 248);
            this.Label_PhysicalHeight.Name = "Label_PhysicalHeight";
            this.Label_PhysicalHeight.Size = new System.Drawing.Size(77, 13);
            this.Label_PhysicalHeight.TabIndex = 0;
            this.Label_PhysicalHeight.Text = "PhysicalHeight";
            // 
            // NumericUpDown_PhysicalHeight
            // 
            this.NumericUpDown_PhysicalHeight.Location = new System.Drawing.Point(102, 264);
            this.NumericUpDown_PhysicalHeight.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.NumericUpDown_PhysicalHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDown_PhysicalHeight.Name = "NumericUpDown_PhysicalHeight";
            this.NumericUpDown_PhysicalHeight.Size = new System.Drawing.Size(57, 20);
            this.NumericUpDown_PhysicalHeight.TabIndex = 3;
            this.NumericUpDown_PhysicalHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_PhysicalHeight.Value = new decimal(new int[] {
            42,
            0,
            0,
            0});
            this.NumericUpDown_PhysicalHeight.ValueChanged += new System.EventHandler(this.NumericUpDowns_ValueChanged);
            // 
            // SplitContainer_Main
            // 
            this.SplitContainer_Main.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitContainer_Main.Location = new System.Drawing.Point(221, 136);
            this.SplitContainer_Main.Name = "SplitContainer_Main";
            this.SplitContainer_Main.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainer_Main.Panel1
            // 
            this.SplitContainer_Main.Panel1.AutoScroll = true;
            this.SplitContainer_Main.Panel1.Controls.Add(this.panel1);
            // 
            // SplitContainer_Main.Panel2
            // 
            this.SplitContainer_Main.Panel2.AutoScroll = true;
            this.SplitContainer_Main.Panel2.Controls.Add(this.panel2);
            this.SplitContainer_Main.Size = new System.Drawing.Size(827, 535);
            this.SplitContainer_Main.SplitterDistance = 173;
            this.SplitContainer_Main.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.PictureBox_Preview);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(827, 173);
            this.panel1.TabIndex = 0;
            // 
            // PictureBox_Preview
            // 
            this.PictureBox_Preview.BackColor = System.Drawing.Color.White;
            this.PictureBox_Preview.Location = new System.Drawing.Point(0, 0);
            this.PictureBox_Preview.Name = "PictureBox_Preview";
            this.PictureBox_Preview.Size = new System.Drawing.Size(827, 190);
            this.PictureBox_Preview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.PictureBox_Preview.TabIndex = 5;
            this.PictureBox_Preview.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.PictureBox_Preview2x);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(827, 358);
            this.panel2.TabIndex = 0;
            // 
            // PictureBox_Preview2x
            // 
            this.PictureBox_Preview2x.BackColor = System.Drawing.Color.White;
            this.PictureBox_Preview2x.Location = new System.Drawing.Point(0, 0);
            this.PictureBox_Preview2x.Name = "PictureBox_Preview2x";
            this.PictureBox_Preview2x.Size = new System.Drawing.Size(827, 389);
            this.PictureBox_Preview2x.TabIndex = 5;
            this.PictureBox_Preview2x.TabStop = false;
            // 
            // Label_DrawOffsetX
            // 
            this.Label_DrawOffsetX.AutoSize = true;
            this.Label_DrawOffsetX.Location = new System.Drawing.Point(12, 290);
            this.Label_DrawOffsetX.Name = "Label_DrawOffsetX";
            this.Label_DrawOffsetX.Size = new System.Drawing.Size(67, 13);
            this.Label_DrawOffsetX.TabIndex = 0;
            this.Label_DrawOffsetX.Text = "DrawOffsetX";
            // 
            // Label_DrawOffsetY
            // 
            this.Label_DrawOffsetY.AutoSize = true;
            this.Label_DrawOffsetY.Location = new System.Drawing.Point(100, 290);
            this.Label_DrawOffsetY.Name = "Label_DrawOffsetY";
            this.Label_DrawOffsetY.Size = new System.Drawing.Size(67, 13);
            this.Label_DrawOffsetY.TabIndex = 0;
            this.Label_DrawOffsetY.Text = "DrawOffsetY";
            // 
            // NumericUpDown_DrawOffsetX
            // 
            this.NumericUpDown_DrawOffsetX.Location = new System.Drawing.Point(14, 307);
            this.NumericUpDown_DrawOffsetX.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.NumericUpDown_DrawOffsetX.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.NumericUpDown_DrawOffsetX.Name = "NumericUpDown_DrawOffsetX";
            this.NumericUpDown_DrawOffsetX.Size = new System.Drawing.Size(57, 20);
            this.NumericUpDown_DrawOffsetX.TabIndex = 3;
            this.NumericUpDown_DrawOffsetX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_DrawOffsetX.ValueChanged += new System.EventHandler(this.NumericUpDowns_ValueChanged);
            // 
            // NumericUpDown_DrawOffsetY
            // 
            this.NumericUpDown_DrawOffsetY.Location = new System.Drawing.Point(102, 307);
            this.NumericUpDown_DrawOffsetY.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.NumericUpDown_DrawOffsetY.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.NumericUpDown_DrawOffsetY.Name = "NumericUpDown_DrawOffsetY";
            this.NumericUpDown_DrawOffsetY.Size = new System.Drawing.Size(57, 20);
            this.NumericUpDown_DrawOffsetY.TabIndex = 3;
            this.NumericUpDown_DrawOffsetY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_DrawOffsetY.ValueChanged += new System.EventHandler(this.NumericUpDowns_ValueChanged);
            // 
            // Label_VirtualOffsetX
            // 
            this.Label_VirtualOffsetX.AutoSize = true;
            this.Label_VirtualOffsetX.Location = new System.Drawing.Point(12, 333);
            this.Label_VirtualOffsetX.Name = "Label_VirtualOffsetX";
            this.Label_VirtualOffsetX.Size = new System.Drawing.Size(71, 13);
            this.Label_VirtualOffsetX.TabIndex = 0;
            this.Label_VirtualOffsetX.Text = "VirtualOffsetX";
            // 
            // Label_VirtualOffsetY
            // 
            this.Label_VirtualOffsetY.AutoSize = true;
            this.Label_VirtualOffsetY.Location = new System.Drawing.Point(100, 333);
            this.Label_VirtualOffsetY.Name = "Label_VirtualOffsetY";
            this.Label_VirtualOffsetY.Size = new System.Drawing.Size(71, 13);
            this.Label_VirtualOffsetY.TabIndex = 0;
            this.Label_VirtualOffsetY.Text = "VirtualOffsetY";
            // 
            // NumericUpDown_VirtualOffsetX
            // 
            this.NumericUpDown_VirtualOffsetX.Location = new System.Drawing.Point(14, 349);
            this.NumericUpDown_VirtualOffsetX.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.NumericUpDown_VirtualOffsetX.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.NumericUpDown_VirtualOffsetX.Name = "NumericUpDown_VirtualOffsetX";
            this.NumericUpDown_VirtualOffsetX.Size = new System.Drawing.Size(57, 20);
            this.NumericUpDown_VirtualOffsetX.TabIndex = 3;
            this.NumericUpDown_VirtualOffsetX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_VirtualOffsetX.ValueChanged += new System.EventHandler(this.NumericUpDowns_ValueChanged);
            // 
            // NumericUpDown_VirtualOffsetY
            // 
            this.NumericUpDown_VirtualOffsetY.Location = new System.Drawing.Point(102, 349);
            this.NumericUpDown_VirtualOffsetY.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.NumericUpDown_VirtualOffsetY.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.NumericUpDown_VirtualOffsetY.Name = "NumericUpDown_VirtualOffsetY";
            this.NumericUpDown_VirtualOffsetY.Size = new System.Drawing.Size(57, 20);
            this.NumericUpDown_VirtualOffsetY.TabIndex = 3;
            this.NumericUpDown_VirtualOffsetY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_VirtualOffsetY.ValueChanged += new System.EventHandler(this.NumericUpDowns_ValueChanged);
            // 
            // Label_VirtualDeltaWidth
            // 
            this.Label_VirtualDeltaWidth.AutoSize = true;
            this.Label_VirtualDeltaWidth.Location = new System.Drawing.Point(12, 375);
            this.Label_VirtualDeltaWidth.Name = "Label_VirtualDeltaWidth";
            this.Label_VirtualDeltaWidth.Size = new System.Drawing.Size(67, 13);
            this.Label_VirtualDeltaWidth.TabIndex = 0;
            this.Label_VirtualDeltaWidth.Text = "Virtual Width";
            // 
            // Label_VirtualDeltaHeight
            // 
            this.Label_VirtualDeltaHeight.AutoSize = true;
            this.Label_VirtualDeltaHeight.Location = new System.Drawing.Point(100, 375);
            this.Label_VirtualDeltaHeight.Name = "Label_VirtualDeltaHeight";
            this.Label_VirtualDeltaHeight.Size = new System.Drawing.Size(70, 13);
            this.Label_VirtualDeltaHeight.TabIndex = 0;
            this.Label_VirtualDeltaHeight.Text = "Virtual Height";
            // 
            // NumericUpDown_VirtualDeltaWidth
            // 
            this.NumericUpDown_VirtualDeltaWidth.Location = new System.Drawing.Point(14, 391);
            this.NumericUpDown_VirtualDeltaWidth.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.NumericUpDown_VirtualDeltaWidth.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.NumericUpDown_VirtualDeltaWidth.Name = "NumericUpDown_VirtualDeltaWidth";
            this.NumericUpDown_VirtualDeltaWidth.Size = new System.Drawing.Size(57, 20);
            this.NumericUpDown_VirtualDeltaWidth.TabIndex = 3;
            this.NumericUpDown_VirtualDeltaWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_VirtualDeltaWidth.ValueChanged += new System.EventHandler(this.NumericUpDowns_ValueChanged);
            // 
            // NumericUpDown_VirtualDeltaHeight
            // 
            this.NumericUpDown_VirtualDeltaHeight.Location = new System.Drawing.Point(102, 391);
            this.NumericUpDown_VirtualDeltaHeight.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.NumericUpDown_VirtualDeltaHeight.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.NumericUpDown_VirtualDeltaHeight.Name = "NumericUpDown_VirtualDeltaHeight";
            this.NumericUpDown_VirtualDeltaHeight.Size = new System.Drawing.Size(57, 20);
            this.NumericUpDown_VirtualDeltaHeight.TabIndex = 3;
            this.NumericUpDown_VirtualDeltaHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_VirtualDeltaHeight.ValueChanged += new System.EventHandler(this.NumericUpDowns_ValueChanged);
            // 
            // Button_Generate
            // 
            this.Button_Generate.Location = new System.Drawing.Point(12, 500);
            this.Button_Generate.Name = "Button_Generate";
            this.Button_Generate.Size = new System.Drawing.Size(202, 25);
            this.Button_Generate.TabIndex = 7;
            this.Button_Generate.Text = "Generate";
            this.Button_Generate.UseVisualStyleBackColor = true;
            this.Button_Generate.Click += new System.EventHandler(this.Button_Generate_Click);
            // 
            // Button_CmdToClipboard
            // 
            this.Button_CmdToClipboard.Location = new System.Drawing.Point(12, 468);
            this.Button_CmdToClipboard.Name = "Button_CmdToClipboard";
            this.Button_CmdToClipboard.Size = new System.Drawing.Size(201, 25);
            this.Button_CmdToClipboard.TabIndex = 8;
            this.Button_CmdToClipboard.Text = "Commandline to the clipboard";
            this.Button_CmdToClipboard.UseVisualStyleBackColor = true;
            this.Button_CmdToClipboard.Click += new System.EventHandler(this.Button_CmdToClipboard_Click);
            // 
            // CheckBox_AnchorLeft
            // 
            this.CheckBox_AnchorLeft.AutoSize = true;
            this.CheckBox_AnchorLeft.Location = new System.Drawing.Point(102, 228);
            this.CheckBox_AnchorLeft.Name = "CheckBox_AnchorLeft";
            this.CheckBox_AnchorLeft.Size = new System.Drawing.Size(92, 17);
            this.CheckBox_AnchorLeft.TabIndex = 2;
            this.CheckBox_AnchorLeft.Text = "Left alignment";
            this.CheckBox_AnchorLeft.UseVisualStyleBackColor = true;
            this.CheckBox_AnchorLeft.CheckedChanged += new System.EventHandler(this.CheckBox_DoubleSample_CheckedChanged);
            // 
            // ddlBPP
            // 
            this.ddlBPP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlBPP.FormattingEnabled = true;
            this.ddlBPP.Items.AddRange(new object[] {
            "8",
            "32"});
            this.ddlBPP.Location = new System.Drawing.Point(15, 440);
            this.ddlBPP.Name = "ddlBPP";
            this.ddlBPP.Size = new System.Drawing.Size(58, 21);
            this.ddlBPP.TabIndex = 11;
            // 
            // chkDrawAlpha
            // 
            this.chkDrawAlpha.AutoSize = true;
            this.chkDrawAlpha.Checked = true;
            this.chkDrawAlpha.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDrawAlpha.Location = new System.Drawing.Point(14, 555);
            this.chkDrawAlpha.Name = "chkDrawAlpha";
            this.chkDrawAlpha.Size = new System.Drawing.Size(134, 17);
            this.chkDrawAlpha.TabIndex = 12;
            this.chkDrawAlpha.Text = "DrawAlpha (NON CoP)";
            this.chkDrawAlpha.UseVisualStyleBackColor = true;
            this.chkDrawAlpha.CheckedChanged += new System.EventHandler(this.chkDrawAlpha_CheckedChanged);
            // 
            // btnCustomFont
            // 
            this.btnCustomFont.Location = new System.Drawing.Point(16, 71);
            this.btnCustomFont.Name = "btnCustomFont";
            this.btnCustomFont.Size = new System.Drawing.Size(75, 23);
            this.btnCustomFont.TabIndex = 13;
            this.btnCustomFont.Text = "CustomFont";
            this.btnCustomFont.UseVisualStyleBackColor = true;
            this.btnCustomFont.Click += new System.EventHandler(this.btnCustomFont_Click);
            // 
            // lblCustomFontName
            // 
            this.lblCustomFontName.AutoSize = true;
            this.lblCustomFontName.Location = new System.Drawing.Point(225, 81);
            this.lblCustomFontName.Name = "lblCustomFontName";
            this.lblCustomFontName.Size = new System.Drawing.Size(101, 13);
            this.lblCustomFontName.TabIndex = 14;
            this.lblCustomFontName.Text = "lblCustomFontName";
            // 
            // txtTargetPath
            // 
            this.txtTargetPath.Location = new System.Drawing.Point(425, 51);
            this.txtTargetPath.Name = "txtTargetPath";
            this.txtTargetPath.Size = new System.Drawing.Size(410, 20);
            this.txtTargetPath.TabIndex = 16;
            // 
            // FileSelectBox_File
            // 
            this.FileSelectBox_File.AutoSize = true;
            this.FileSelectBox_File.Filter = "TBL code watch file (*.tbl) |*.tbl | fd character description file (*.fd) |*.fd |" +
    " character file (*.txt) |*.txt";
            this.FileSelectBox_File.LabelText = "TBL/FD/character source file";
            this.FileSelectBox_File.Location = new System.Drawing.Point(221, 14);
            this.FileSelectBox_File.Name = "FileSelectBox_File";
            this.FileSelectBox_File.Path = "";
            this.FileSelectBox_File.Size = new System.Drawing.Size(614, 29);
            this.FileSelectBox_File.SplitterDistance = 200;
            this.FileSelectBox_File.TabIndex = 6;
            // 
            // chkDelTemp
            // 
            this.chkDelTemp.AutoSize = true;
            this.chkDelTemp.Checked = true;
            this.chkDelTemp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDelTemp.Location = new System.Drawing.Point(14, 532);
            this.chkDelTemp.Name = "chkDelTemp";
            this.chkDelTemp.Size = new System.Drawing.Size(113, 17);
            this.chkDelTemp.TabIndex = 17;
            this.chkDelTemp.Text = "Remove temp files";
            this.chkDelTemp.UseVisualStyleBackColor = true;
            // 
            // btnEditText
            // 
            this.btnEditText.Location = new System.Drawing.Point(973, 59);
            this.btnEditText.Name = "btnEditText";
            this.btnEditText.Size = new System.Drawing.Size(75, 23);
            this.btnEditText.TabIndex = 18;
            this.btnEditText.Text = "Edit content";
            this.btnEditText.UseVisualStyleBackColor = true;
            this.btnEditText.Click += new System.EventHandler(this.btnEditText_Click);
            // 
            // btnAutosize
            // 
            this.btnAutosize.Location = new System.Drawing.Point(102, 122);
            this.btnAutosize.Name = "btnAutosize";
            this.btnAutosize.Size = new System.Drawing.Size(75, 23);
            this.btnAutosize.TabIndex = 19;
            this.btnAutosize.Text = "Autosize";
            this.btnAutosize.UseVisualStyleBackColor = true;
            this.btnAutosize.Click += new System.EventHandler(this.btnAutosize_Click);
            // 
            // FontGenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 680);
            this.Controls.Add(this.btnAutosize);
            this.Controls.Add(this.btnEditText);
            this.Controls.Add(this.chkDelTemp);
            this.Controls.Add(this.txtTargetPath);
            this.Controls.Add(label2);
            this.Controls.Add(this.lblCustomFontName);
            this.Controls.Add(this.btnCustomFont);
            this.Controls.Add(this.chkDrawAlpha);
            this.Controls.Add(this.ddlBPP);
            this.Controls.Add(label1);
            this.Controls.Add(this.Button_CmdToClipboard);
            this.Controls.Add(this.Button_Generate);
            this.Controls.Add(this.FileSelectBox_File);
            this.Controls.Add(this.SplitContainer_Main);
            this.Controls.Add(this.NumericUpDown_VirtualDeltaHeight);
            this.Controls.Add(this.NumericUpDown_VirtualDeltaWidth);
            this.Controls.Add(this.NumericUpDown_VirtualOffsetY);
            this.Controls.Add(this.NumericUpDown_VirtualOffsetX);
            this.Controls.Add(this.NumericUpDown_DrawOffsetY);
            this.Controls.Add(this.NumericUpDown_DrawOffsetX);
            this.Controls.Add(this.NumericUpDown_PhysicalHeight);
            this.Controls.Add(this.NumericUpDown_PhysicalWidth);
            this.Controls.Add(this.NumericUpDown_Size);
            this.Controls.Add(this.Label_VirtualDeltaHeight);
            this.Controls.Add(this.CheckBox_Strikeout);
            this.Controls.Add(this.Label_VirtualOffsetY);
            this.Controls.Add(this.CheckBox_Underline);
            this.Controls.Add(this.Label_DrawOffsetY);
            this.Controls.Add(this.Label_VirtualDeltaWidth);
            this.Controls.Add(this.CheckBox_Italic);
            this.Controls.Add(this.Label_VirtualOffsetX);
            this.Controls.Add(this.Label_PhysicalHeight);
            this.Controls.Add(this.Label_DrawOffsetX);
            this.Controls.Add(this.CheckBox_Bold);
            this.Controls.Add(this.Label_PhysicalWidth);
            this.Controls.Add(this.ComboBox_FontName);
            this.Controls.Add(this.Label_Size);
            this.Controls.Add(this.Label_FontName);
            this.Controls.Add(this.CheckBox_AnchorLeft);
            this.Controls.Add(this.CheckBox_DoubleSample);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FontGenForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Font image generator";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FontGen_Load);
            this.Shown += new System.EventHandler(this.FontGen_Shown);
            this.SizeChanged += new System.EventHandler(this.FontGen_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Size)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_PhysicalWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_PhysicalHeight)).EndInit();
            this.SplitContainer_Main.Panel1.ResumeLayout(false);
            this.SplitContainer_Main.Panel1.PerformLayout();
            this.SplitContainer_Main.Panel2.ResumeLayout(false);
            this.SplitContainer_Main.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer_Main)).EndInit();
            this.SplitContainer_Main.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Preview)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Preview2x)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_DrawOffsetX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_DrawOffsetY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_VirtualOffsetX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_VirtualOffsetY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_VirtualDeltaWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_VirtualDeltaHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.Label Label_FontName;
        internal System.Windows.Forms.ComboBox ComboBox_FontName;
        internal System.Windows.Forms.CheckBox CheckBox_Bold;
        internal System.Windows.Forms.CheckBox CheckBox_Italic;
        internal System.Windows.Forms.CheckBox CheckBox_Underline;
        internal System.Windows.Forms.CheckBox CheckBox_Strikeout;
        internal System.Windows.Forms.NumericUpDown NumericUpDown_Size;
        internal System.Windows.Forms.Label Label_Size;
        internal System.Windows.Forms.CheckBox CheckBox_DoubleSample;
        internal System.Windows.Forms.Label Label_PhysicalWidth;
        internal System.Windows.Forms.NumericUpDown NumericUpDown_PhysicalWidth;
        internal System.Windows.Forms.Label Label_PhysicalHeight;
        internal System.Windows.Forms.NumericUpDown NumericUpDown_PhysicalHeight;
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
        private System.Windows.Forms.ComboBox ddlBPP;
        private System.Windows.Forms.CheckBox chkDrawAlpha;
        private System.Windows.Forms.Button btnCustomFont;
        private System.Windows.Forms.Label lblCustomFontName;
        private System.Windows.Forms.TextBox txtTargetPath;
        private System.Windows.Forms.Panel panel1;
        internal System.Windows.Forms.PictureBox PictureBox_Preview;
        private System.Windows.Forms.Panel panel2;
        internal System.Windows.Forms.PictureBox PictureBox_Preview2x;
        private System.Windows.Forms.CheckBox chkDelTemp;
        private System.Windows.Forms.Button btnEditText;
        private System.Windows.Forms.Button btnAutosize;
    }
}