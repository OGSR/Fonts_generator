
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
            this.CheckBox_AnchorLeft = new System.Windows.Forms.CheckBox();
            this.chkCopMode = new System.Windows.Forms.CheckBox();
            this.btnCustomFont = new System.Windows.Forms.Button();
            this.txtTargetPath = new System.Windows.Forms.TextBox();
            this.chkUTF8 = new System.Windows.Forms.CheckBox();
            this.btnEditText = new System.Windows.Forms.Button();
            this.btnAutosize = new System.Windows.Forms.Button();
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
            // label2
            // 
            label2.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            label2.Location = new System.Drawing.Point(291, 3);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(203, 34);
            label2.TabIndex = 15;
            label2.Text = "Target Directory:";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_FontName
            // 
            this.Label_FontName.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Label_FontName.Location = new System.Drawing.Point(8, 3);
            this.Label_FontName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label_FontName.Name = "Label_FontName";
            this.Label_FontName.Size = new System.Drawing.Size(269, 34);
            this.Label_FontName.TabIndex = 0;
            this.Label_FontName.Text = "Font name";
            this.Label_FontName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ComboBox_FontName
            // 
            this.ComboBox_FontName.DisplayMember = "Name";
            this.ComboBox_FontName.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.ComboBox_FontName.DropDownHeight = 600;
            this.ComboBox_FontName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_FontName.DropDownWidth = 400;
            this.ComboBox_FontName.Font = new System.Drawing.Font("Arial", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ComboBox_FontName.FormattingEnabled = true;
            this.ComboBox_FontName.IntegralHeight = false;
            this.ComboBox_FontName.Location = new System.Drawing.Point(8, 38);
            this.ComboBox_FontName.Margin = new System.Windows.Forms.Padding(4);
            this.ComboBox_FontName.MaxDropDownItems = 20;
            this.ComboBox_FontName.Name = "ComboBox_FontName";
            this.ComboBox_FontName.Size = new System.Drawing.Size(269, 40);
            this.ComboBox_FontName.TabIndex = 1;
            this.ComboBox_FontName.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ComboBox_FontName_DrawItem);
            this.ComboBox_FontName.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.ComboBox_FontName_MeasureItem);
            this.ComboBox_FontName.SelectedValueChanged += new System.EventHandler(this.ComboBox_FontName_SelectedValueChanged);
            // 
            // CheckBox_Bold
            // 
            this.CheckBox_Bold.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CheckBox_Bold.Location = new System.Drawing.Point(8, 186);
            this.CheckBox_Bold.Margin = new System.Windows.Forms.Padding(4);
            this.CheckBox_Bold.Name = "CheckBox_Bold";
            this.CheckBox_Bold.Size = new System.Drawing.Size(120, 35);
            this.CheckBox_Bold.TabIndex = 2;
            this.CheckBox_Bold.Text = "Bold";
            this.CheckBox_Bold.UseVisualStyleBackColor = true;
            this.CheckBox_Bold.CheckedChanged += new System.EventHandler(this.CheckBox_Bold_CheckedChanged);
            // 
            // CheckBox_Italic
            // 
            this.CheckBox_Italic.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CheckBox_Italic.Location = new System.Drawing.Point(157, 186);
            this.CheckBox_Italic.Margin = new System.Windows.Forms.Padding(4);
            this.CheckBox_Italic.Name = "CheckBox_Italic";
            this.CheckBox_Italic.Size = new System.Drawing.Size(120, 35);
            this.CheckBox_Italic.TabIndex = 2;
            this.CheckBox_Italic.Text = "Italic";
            this.CheckBox_Italic.UseVisualStyleBackColor = true;
            this.CheckBox_Italic.CheckedChanged += new System.EventHandler(this.CheckBox_Italic_CheckedChanged);
            // 
            // CheckBox_Underline
            // 
            this.CheckBox_Underline.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CheckBox_Underline.Location = new System.Drawing.Point(8, 229);
            this.CheckBox_Underline.Margin = new System.Windows.Forms.Padding(4);
            this.CheckBox_Underline.Name = "CheckBox_Underline";
            this.CheckBox_Underline.Size = new System.Drawing.Size(120, 36);
            this.CheckBox_Underline.TabIndex = 2;
            this.CheckBox_Underline.Text = "Underline";
            this.CheckBox_Underline.UseVisualStyleBackColor = true;
            this.CheckBox_Underline.CheckedChanged += new System.EventHandler(this.CheckBox_Underline_CheckedChanged);
            // 
            // CheckBox_Strikeout
            // 
            this.CheckBox_Strikeout.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CheckBox_Strikeout.Location = new System.Drawing.Point(157, 229);
            this.CheckBox_Strikeout.Margin = new System.Windows.Forms.Padding(4);
            this.CheckBox_Strikeout.Name = "CheckBox_Strikeout";
            this.CheckBox_Strikeout.Size = new System.Drawing.Size(120, 36);
            this.CheckBox_Strikeout.TabIndex = 2;
            this.CheckBox_Strikeout.Text = "Strikeout";
            this.CheckBox_Strikeout.UseVisualStyleBackColor = true;
            this.CheckBox_Strikeout.CheckedChanged += new System.EventHandler(this.CheckBox_Strikeout_CheckedChanged);
            // 
            // NumericUpDown_Size
            // 
            this.NumericUpDown_Size.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NumericUpDown_Size.Location = new System.Drawing.Point(90, 150);
            this.NumericUpDown_Size.Margin = new System.Windows.Forms.Padding(4);
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
            this.NumericUpDown_Size.Size = new System.Drawing.Size(79, 27);
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
            this.Label_Size.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Label_Size.Location = new System.Drawing.Point(8, 150);
            this.Label_Size.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label_Size.Name = "Label_Size";
            this.Label_Size.Size = new System.Drawing.Size(74, 22);
            this.Label_Size.TabIndex = 0;
            this.Label_Size.Text = "Size:";
            this.Label_Size.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CheckBox_DoubleSample
            // 
            this.CheckBox_DoubleSample.Checked = true;
            this.CheckBox_DoubleSample.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_DoubleSample.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CheckBox_DoubleSample.Location = new System.Drawing.Point(8, 273);
            this.CheckBox_DoubleSample.Margin = new System.Windows.Forms.Padding(4);
            this.CheckBox_DoubleSample.Name = "CheckBox_DoubleSample";
            this.CheckBox_DoubleSample.Size = new System.Drawing.Size(120, 34);
            this.CheckBox_DoubleSample.TabIndex = 2;
            this.CheckBox_DoubleSample.Text = "2xsampling";
            this.CheckBox_DoubleSample.UseVisualStyleBackColor = true;
            this.CheckBox_DoubleSample.CheckedChanged += new System.EventHandler(this.CheckBox_DoubleSample_CheckedChanged);
            // 
            // Label_PhysicalWidth
            // 
            this.Label_PhysicalWidth.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Label_PhysicalWidth.Location = new System.Drawing.Point(8, 305);
            this.Label_PhysicalWidth.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label_PhysicalWidth.Name = "Label_PhysicalWidth";
            this.Label_PhysicalWidth.Size = new System.Drawing.Size(120, 33);
            this.Label_PhysicalWidth.TabIndex = 0;
            this.Label_PhysicalWidth.Text = "PhysicalWidth";
            this.Label_PhysicalWidth.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NumericUpDown_PhysicalWidth
            // 
            this.NumericUpDown_PhysicalWidth.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NumericUpDown_PhysicalWidth.Location = new System.Drawing.Point(8, 342);
            this.NumericUpDown_PhysicalWidth.Margin = new System.Windows.Forms.Padding(4);
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
            this.NumericUpDown_PhysicalWidth.Size = new System.Drawing.Size(120, 27);
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
            this.Label_PhysicalHeight.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Label_PhysicalHeight.Location = new System.Drawing.Point(157, 305);
            this.Label_PhysicalHeight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label_PhysicalHeight.Name = "Label_PhysicalHeight";
            this.Label_PhysicalHeight.Size = new System.Drawing.Size(120, 33);
            this.Label_PhysicalHeight.TabIndex = 0;
            this.Label_PhysicalHeight.Text = "PhysicalHeight";
            this.Label_PhysicalHeight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NumericUpDown_PhysicalHeight
            // 
            this.NumericUpDown_PhysicalHeight.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NumericUpDown_PhysicalHeight.Location = new System.Drawing.Point(157, 342);
            this.NumericUpDown_PhysicalHeight.Margin = new System.Windows.Forms.Padding(4);
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
            this.NumericUpDown_PhysicalHeight.Size = new System.Drawing.Size(120, 27);
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
            this.SplitContainer_Main.Location = new System.Drawing.Point(295, 38);
            this.SplitContainer_Main.Margin = new System.Windows.Forms.Padding(4);
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
            this.SplitContainer_Main.Size = new System.Drawing.Size(1103, 819);
            this.SplitContainer_Main.SplitterDistance = 262;
            this.SplitContainer_Main.SplitterWidth = 5;
            this.SplitContainer_Main.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.PictureBox_Preview);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1103, 262);
            this.panel1.TabIndex = 0;
            // 
            // PictureBox_Preview
            // 
            this.PictureBox_Preview.BackColor = System.Drawing.Color.White;
            this.PictureBox_Preview.Location = new System.Drawing.Point(0, 0);
            this.PictureBox_Preview.Margin = new System.Windows.Forms.Padding(4);
            this.PictureBox_Preview.Name = "PictureBox_Preview";
            this.PictureBox_Preview.Size = new System.Drawing.Size(1103, 259);
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
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1103, 552);
            this.panel2.TabIndex = 0;
            // 
            // PictureBox_Preview2x
            // 
            this.PictureBox_Preview2x.BackColor = System.Drawing.Color.White;
            this.PictureBox_Preview2x.Location = new System.Drawing.Point(0, 0);
            this.PictureBox_Preview2x.Margin = new System.Windows.Forms.Padding(4);
            this.PictureBox_Preview2x.Name = "PictureBox_Preview2x";
            this.PictureBox_Preview2x.Size = new System.Drawing.Size(1103, 552);
            this.PictureBox_Preview2x.TabIndex = 5;
            this.PictureBox_Preview2x.TabStop = false;
            // 
            // Label_DrawOffsetX
            // 
            this.Label_DrawOffsetX.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Label_DrawOffsetX.Location = new System.Drawing.Point(8, 368);
            this.Label_DrawOffsetX.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label_DrawOffsetX.Name = "Label_DrawOffsetX";
            this.Label_DrawOffsetX.Size = new System.Drawing.Size(120, 31);
            this.Label_DrawOffsetX.TabIndex = 0;
            this.Label_DrawOffsetX.Text = "DrawOffsetX";
            this.Label_DrawOffsetX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_DrawOffsetY
            // 
            this.Label_DrawOffsetY.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Label_DrawOffsetY.Location = new System.Drawing.Point(157, 368);
            this.Label_DrawOffsetY.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label_DrawOffsetY.Name = "Label_DrawOffsetY";
            this.Label_DrawOffsetY.Size = new System.Drawing.Size(120, 31);
            this.Label_DrawOffsetY.TabIndex = 0;
            this.Label_DrawOffsetY.Text = "DrawOffsetY";
            this.Label_DrawOffsetY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NumericUpDown_DrawOffsetX
            // 
            this.NumericUpDown_DrawOffsetX.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NumericUpDown_DrawOffsetX.Location = new System.Drawing.Point(8, 403);
            this.NumericUpDown_DrawOffsetX.Margin = new System.Windows.Forms.Padding(4);
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
            this.NumericUpDown_DrawOffsetX.Size = new System.Drawing.Size(120, 27);
            this.NumericUpDown_DrawOffsetX.TabIndex = 3;
            this.NumericUpDown_DrawOffsetX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_DrawOffsetX.ValueChanged += new System.EventHandler(this.NumericUpDowns_ValueChanged);
            // 
            // NumericUpDown_DrawOffsetY
            // 
            this.NumericUpDown_DrawOffsetY.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NumericUpDown_DrawOffsetY.Location = new System.Drawing.Point(157, 403);
            this.NumericUpDown_DrawOffsetY.Margin = new System.Windows.Forms.Padding(4);
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
            this.NumericUpDown_DrawOffsetY.Size = new System.Drawing.Size(120, 27);
            this.NumericUpDown_DrawOffsetY.TabIndex = 3;
            this.NumericUpDown_DrawOffsetY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_DrawOffsetY.ValueChanged += new System.EventHandler(this.NumericUpDowns_ValueChanged);
            // 
            // Label_VirtualOffsetX
            // 
            this.Label_VirtualOffsetX.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Label_VirtualOffsetX.Location = new System.Drawing.Point(8, 429);
            this.Label_VirtualOffsetX.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label_VirtualOffsetX.Name = "Label_VirtualOffsetX";
            this.Label_VirtualOffsetX.Size = new System.Drawing.Size(120, 31);
            this.Label_VirtualOffsetX.TabIndex = 0;
            this.Label_VirtualOffsetX.Text = "VirtualOffsetX";
            this.Label_VirtualOffsetX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_VirtualOffsetY
            // 
            this.Label_VirtualOffsetY.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Label_VirtualOffsetY.Location = new System.Drawing.Point(157, 429);
            this.Label_VirtualOffsetY.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label_VirtualOffsetY.Name = "Label_VirtualOffsetY";
            this.Label_VirtualOffsetY.Size = new System.Drawing.Size(120, 31);
            this.Label_VirtualOffsetY.TabIndex = 0;
            this.Label_VirtualOffsetY.Text = "VirtualOffsetY";
            this.Label_VirtualOffsetY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NumericUpDown_VirtualOffsetX
            // 
            this.NumericUpDown_VirtualOffsetX.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NumericUpDown_VirtualOffsetX.Location = new System.Drawing.Point(8, 464);
            this.NumericUpDown_VirtualOffsetX.Margin = new System.Windows.Forms.Padding(4);
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
            this.NumericUpDown_VirtualOffsetX.Size = new System.Drawing.Size(120, 27);
            this.NumericUpDown_VirtualOffsetX.TabIndex = 3;
            this.NumericUpDown_VirtualOffsetX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_VirtualOffsetX.ValueChanged += new System.EventHandler(this.NumericUpDowns_ValueChanged);
            // 
            // NumericUpDown_VirtualOffsetY
            // 
            this.NumericUpDown_VirtualOffsetY.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NumericUpDown_VirtualOffsetY.Location = new System.Drawing.Point(157, 464);
            this.NumericUpDown_VirtualOffsetY.Margin = new System.Windows.Forms.Padding(4);
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
            this.NumericUpDown_VirtualOffsetY.Size = new System.Drawing.Size(120, 27);
            this.NumericUpDown_VirtualOffsetY.TabIndex = 3;
            this.NumericUpDown_VirtualOffsetY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_VirtualOffsetY.ValueChanged += new System.EventHandler(this.NumericUpDowns_ValueChanged);
            // 
            // Label_VirtualDeltaWidth
            // 
            this.Label_VirtualDeltaWidth.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Label_VirtualDeltaWidth.Location = new System.Drawing.Point(8, 500);
            this.Label_VirtualDeltaWidth.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label_VirtualDeltaWidth.Name = "Label_VirtualDeltaWidth";
            this.Label_VirtualDeltaWidth.Size = new System.Drawing.Size(120, 29);
            this.Label_VirtualDeltaWidth.TabIndex = 0;
            this.Label_VirtualDeltaWidth.Text = "Virtual Width";
            this.Label_VirtualDeltaWidth.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_VirtualDeltaHeight
            // 
            this.Label_VirtualDeltaHeight.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Label_VirtualDeltaHeight.Location = new System.Drawing.Point(157, 500);
            this.Label_VirtualDeltaHeight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label_VirtualDeltaHeight.Name = "Label_VirtualDeltaHeight";
            this.Label_VirtualDeltaHeight.Size = new System.Drawing.Size(120, 29);
            this.Label_VirtualDeltaHeight.TabIndex = 0;
            this.Label_VirtualDeltaHeight.Text = "Virtual Height";
            this.Label_VirtualDeltaHeight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NumericUpDown_VirtualDeltaWidth
            // 
            this.NumericUpDown_VirtualDeltaWidth.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NumericUpDown_VirtualDeltaWidth.Location = new System.Drawing.Point(8, 533);
            this.NumericUpDown_VirtualDeltaWidth.Margin = new System.Windows.Forms.Padding(4);
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
            this.NumericUpDown_VirtualDeltaWidth.Size = new System.Drawing.Size(120, 27);
            this.NumericUpDown_VirtualDeltaWidth.TabIndex = 3;
            this.NumericUpDown_VirtualDeltaWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_VirtualDeltaWidth.ValueChanged += new System.EventHandler(this.NumericUpDowns_ValueChanged);
            // 
            // NumericUpDown_VirtualDeltaHeight
            // 
            this.NumericUpDown_VirtualDeltaHeight.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NumericUpDown_VirtualDeltaHeight.Location = new System.Drawing.Point(157, 533);
            this.NumericUpDown_VirtualDeltaHeight.Margin = new System.Windows.Forms.Padding(4);
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
            this.NumericUpDown_VirtualDeltaHeight.Size = new System.Drawing.Size(120, 27);
            this.NumericUpDown_VirtualDeltaHeight.TabIndex = 3;
            this.NumericUpDown_VirtualDeltaHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_VirtualDeltaHeight.ValueChanged += new System.EventHandler(this.NumericUpDowns_ValueChanged);
            // 
            // Button_Generate
            // 
            this.Button_Generate.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Button_Generate.Location = new System.Drawing.Point(8, 815);
            this.Button_Generate.Margin = new System.Windows.Forms.Padding(4);
            this.Button_Generate.Name = "Button_Generate";
            this.Button_Generate.Size = new System.Drawing.Size(269, 41);
            this.Button_Generate.TabIndex = 7;
            this.Button_Generate.Text = "Generate";
            this.Button_Generate.UseVisualStyleBackColor = true;
            this.Button_Generate.Click += new System.EventHandler(this.Button_Generate_Click);
            // 
            // CheckBox_AnchorLeft
            // 
            this.CheckBox_AnchorLeft.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CheckBox_AnchorLeft.Location = new System.Drawing.Point(157, 273);
            this.CheckBox_AnchorLeft.Margin = new System.Windows.Forms.Padding(4);
            this.CheckBox_AnchorLeft.Name = "CheckBox_AnchorLeft";
            this.CheckBox_AnchorLeft.Size = new System.Drawing.Size(120, 34);
            this.CheckBox_AnchorLeft.TabIndex = 2;
            this.CheckBox_AnchorLeft.Text = "Left align";
            this.CheckBox_AnchorLeft.UseVisualStyleBackColor = true;
            this.CheckBox_AnchorLeft.CheckedChanged += new System.EventHandler(this.CheckBox_DoubleSample_CheckedChanged);
            // 
            // chkCopMode
            // 
            this.chkCopMode.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkCopMode.Location = new System.Drawing.Point(8, 629);
            this.chkCopMode.Margin = new System.Windows.Forms.Padding(4);
            this.chkCopMode.Name = "chkCopMode";
            this.chkCopMode.Size = new System.Drawing.Size(269, 111);
            this.chkCopMode.TabIndex = 12;
            this.chkCopMode.Text = "COP-style font texture (not recommended, SHOC-style has better quality, but needs" +
    " use \'font\' shader in \'fonts.ltx\')";
            this.chkCopMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkCopMode.UseVisualStyleBackColor = true;
            this.chkCopMode.CheckedChanged += new System.EventHandler(this.chkDrawAlpha_CheckedChanged);
            // 
            // btnCustomFont
            // 
            this.btnCustomFont.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnCustomFont.Location = new System.Drawing.Point(8, 87);
            this.btnCustomFont.Margin = new System.Windows.Forms.Padding(4);
            this.btnCustomFont.Name = "btnCustomFont";
            this.btnCustomFont.Size = new System.Drawing.Size(269, 41);
            this.btnCustomFont.TabIndex = 13;
            this.btnCustomFont.Text = "CustomFont";
            this.btnCustomFont.UseVisualStyleBackColor = true;
            this.btnCustomFont.Click += new System.EventHandler(this.btnCustomFont_Click);
            // 
            // txtTargetPath
            // 
            this.txtTargetPath.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtTargetPath.Location = new System.Drawing.Point(502, 3);
            this.txtTargetPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtTargetPath.Name = "txtTargetPath";
            this.txtTargetPath.Size = new System.Drawing.Size(896, 27);
            this.txtTargetPath.TabIndex = 16;
            // 
            // chkUTF8
            // 
            this.chkUTF8.Checked = true;
            this.chkUTF8.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUTF8.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkUTF8.Location = new System.Drawing.Point(8, 748);
            this.chkUTF8.Margin = new System.Windows.Forms.Padding(4);
            this.chkUTF8.Name = "chkUTF8";
            this.chkUTF8.Size = new System.Drawing.Size(269, 59);
            this.chkUTF8.TabIndex = 17;
            this.chkUTF8.Text = "UTF-8 font (full support only in OGSR Engine)";
            this.chkUTF8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkUTF8.UseVisualStyleBackColor = true;
            // 
            // btnEditText
            // 
            this.btnEditText.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnEditText.Location = new System.Drawing.Point(8, 580);
            this.btnEditText.Margin = new System.Windows.Forms.Padding(4);
            this.btnEditText.Name = "btnEditText";
            this.btnEditText.Size = new System.Drawing.Size(269, 41);
            this.btnEditText.TabIndex = 18;
            this.btnEditText.Text = "Edit symbols";
            this.btnEditText.UseVisualStyleBackColor = true;
            this.btnEditText.Click += new System.EventHandler(this.btnEditText_Click);
            // 
            // btnAutosize
            // 
            this.btnAutosize.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnAutosize.Location = new System.Drawing.Point(177, 146);
            this.btnAutosize.Margin = new System.Windows.Forms.Padding(4);
            this.btnAutosize.Name = "btnAutosize";
            this.btnAutosize.Size = new System.Drawing.Size(100, 32);
            this.btnAutosize.TabIndex = 19;
            this.btnAutosize.Text = "Autosize";
            this.btnAutosize.UseVisualStyleBackColor = true;
            this.btnAutosize.Click += new System.EventHandler(this.btnAutosize_Click);
            // 
            // FontGenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1413, 869);
            this.Controls.Add(this.btnAutosize);
            this.Controls.Add(this.chkUTF8);
            this.Controls.Add(this.btnEditText);
            this.Controls.Add(this.txtTargetPath);
            this.Controls.Add(label2);
            this.Controls.Add(this.btnCustomFont);
            this.Controls.Add(this.chkCopMode);
            this.Controls.Add(this.Button_Generate);
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
            this.Margin = new System.Windows.Forms.Padding(4);
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
        internal System.Windows.Forms.Button Button_Generate;
        internal System.Windows.Forms.CheckBox CheckBox_AnchorLeft;
        private System.Windows.Forms.CheckBox chkCopMode;
        private System.Windows.Forms.Button btnCustomFont;
        private System.Windows.Forms.TextBox txtTargetPath;
        private System.Windows.Forms.Panel panel1;
        internal System.Windows.Forms.PictureBox PictureBox_Preview;
        private System.Windows.Forms.Panel panel2;
        internal System.Windows.Forms.PictureBox PictureBox_Preview2x;
        private System.Windows.Forms.CheckBox chkUTF8;
        private System.Windows.Forms.Button btnEditText;
        private System.Windows.Forms.Button btnAutosize;
    }
}