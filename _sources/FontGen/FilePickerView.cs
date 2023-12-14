// ==========================================================================
// 
// File:        FilePickerView.vb
// Location:    Firefly.GUI <Visual Basic .Net>
// Description: 文件选取对话框 - 界面
// Version:     2010.02.04.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Firefly;
using Microsoft.VisualBasic.CompilerServices;

namespace FontGen
{

    public partial class FilePicker
    {
        private class Item
        {
            public string Name;
            public int Type;
        }

        private List<string> DirectoryList = new List<string>();
        private void RefreshDirectoryList()
        {
            string DirectoryText = ComboBox_Directory.Text;
            DirectoryList.Clear();
            ComboBox_Directory.Items.Clear();
            DirectoryList.Add(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            ComboBox_Directory.Items.Add("桌面");
            foreach (var d in DriveInfo.GetDrives())
            {
                DirectoryList.Add(d.Name);
                if (d.IsReady)
                {
                    ComboBox_Directory.Items.Add("{0} ({1})".Formats(FileNameHandling.GetDirectoryPathWithoutTailingSeparator(d.Name), d.VolumeLabel));
                }
                else
                {
                    ComboBox_Directory.Items.Add(FileNameHandling.GetDirectoryPathWithoutTailingSeparator(d.Name));
                }
            }
            ComboBox_Directory.Text = DirectoryText;
        }

        private void ComboBox_Directory_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox_Directory.Text = DirectoryList[ComboBox_Directory.SelectedIndex];
            CurrentDirectory = ComboBox_Directory.Text;
        }

        private ImageList ImageList = new ImageList();
        private Dictionary<int, Icon> IconDict = new Dictionary<int, Icon>();
        private List<ListViewItem> Sorter = new List<ListViewItem>();
        private void FillItem(int i)
        {
            var Item = Sorter[i];
            if (IconDict.ContainsKey(i))
                return;

            string f = FileNameHandling.GetAbsolutePath(Item.SubItems[0].Text, CurrentDirectory);
            Icon Icon = null;
            Icon = FilePickerInterop.GetAssociatedIcon(f, false);
            if (Icon is not null)
            {
                int Index;
                ImageList.Images.Add(Icon);
                Index = IconDict.Count;
                IconDict.Add(i, Icon);
                Item.ImageIndex = Index;
            }
            string TypeName = FilePickerInterop.GetTypeName(f);
            Item.SubItems[2].Text = TypeName;
        }

        private void RefreshList()
        {
            if (!Directory.Exists(CurrentDirectory))
                return;
            foreach (var p in IconDict)
                p.Value.Dispose();
            ImageList = new ImageList();
            IconDict = new Dictionary<int, Icon>();
            ImageList.ColorDepth = ColorDepth.Depth32Bit;
            FileListView.SmallImageList = ImageList;

            FileListView.VirtualListSize = 0;

            Item[] RootItems = [new Item() { Name = "..", Type = -1 }];
            var DirectoryItems = Directory.GetDirectories(CurrentDirectory).OrderBy(f => f, StringComparer.OrdinalIgnoreCase).Select(f => new Item() { Name = f, Type = 0 });
            string[] FileMasks = [];
            if (FilterIndexValue >= 0 && FilterIndexValue < FilterValues.Count)
            {
                FileMasks = FilterValues[FilterIndexValue].Value;
            }
            bool FileNamePredicate(string FileName) => FileMasks.Any(m => FileNameHandling.IsMatchFileMask(FileName, m));
            var FileItems = Directory.GetFiles(CurrentDirectory).Where(FilePath => FileNamePredicate(FileNameHandling.GetFileName(FilePath))).OrderBy(f => f, StringComparer.OrdinalIgnoreCase).Select(f => new Item() { Name = f, Type = 1 });
            Item[] Items = RootItems.Concat(DirectoryItems).Concat(FileItems).ToArray();

            int n = 0;
            Sorter = new List<ListViewItem>();
            foreach (var p in Items)
            {
                string f = FileNameHandling.GetAbsolutePath(p.Name, CurrentDirectory);
                if (string.IsNullOrEmpty(f))
                    continue;
                var fi = new FileInfo(f);
                string TypeName = "";
                if (Directory.Exists(f))
                {
                    var Item = new ListViewItem(new string[] { FileNameHandling.GetFileName(p.Name), "", TypeName, Conversions.ToString(fi.LastWriteTime), Conversions.ToString(fi.CreationTime), n.ToString(), p.Type.ToString() });
                    Sorter.Add(Item);
                }
                else
                {
                    var Item = new ListViewItem(new string[] { FileNameHandling.GetFileName(p.Name), fi.Length.ToString(), TypeName, Conversions.ToString(fi.LastWriteTime), Conversions.ToString(fi.CreationTime), n.ToString(), p.Type.ToString() });
                    Sorter.Add(Item);
                }
                n += 1;
            }
            if (FileListViewMajorCompareeIndex != -1)
                Sorter.Sort(Comparison);

            FileListView.VirtualListSize = Sorter.Count;
        }

        private void FileListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            FillItem(e.ItemIndex);
            e.Item = Sorter[e.ItemIndex];
        }

        private int FileListViewMajorCompareeIndex = -1;

        private int Comparison(ListViewItem x, ListViewItem y)
        {
            var OrderSeq = new List<int>();
            OrderSeq.Add(FileListView.Columns.Count + 1);
            var r = Enumerable.Range(0, FileListView.Columns.Count);
            if (r.Contains(FileListViewMajorCompareeIndex))
            {
                OrderSeq.Add(FileListViewMajorCompareeIndex);
                OrderSeq.AddRange(r.Except(new int[] { FileListViewMajorCompareeIndex }));
            }
            else
            {
                OrderSeq.AddRange(r);
            }
            OrderSeq.Add(FileListView.Columns.Count);

            foreach (var c in OrderSeq)
            {
                switch (c)
                {
                    case 1:
                        {
                            if (x.SubItems[c].Text.Length < y.SubItems[c].Text.Length)
                                return -1;
                            if (x.SubItems[c].Text.Length > y.SubItems[c].Text.Length)
                                return 1;
                            break;
                        }
                }
                if (Operators.CompareString(x.SubItems[c].Text, y.SubItems[c].Text, false) < 0)
                    return -1;
                if (Operators.CompareString(x.SubItems[c].Text, y.SubItems[c].Text, false) > 0)
                    return 1;
            }
            return 0;
        }

        private void FileListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            FileListViewMajorCompareeIndex = e.Column;
            RefreshList();
        }

        private void RefreshFilterList()
        {
            ComboBox_Filter.Items.Clear();
            if (FilterValues.Count > 0)
            {
                ComboBox_Filter.Items.AddRange((from f in FilterValues
                                                select f.Key).ToArray());
                if (FilterIndexValue >= 0 && FilterIndexValue < FilterValues.Count)
                {
                    ComboBox_Filter.SelectedIndex = FilterIndexValue;
                }
            }
        }

        private void ComboBox_Filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBox_Filter.SelectedIndex != FilterIndexValue)
            {
                FilterIndexValue = ComboBox_Filter.SelectedIndex;
                RefreshList();
            }
        }

        public FilePicker(bool IsSaveDialog = false)
        {

            Timer = new Timer();
            // 此调用是 Windows 窗体设计器所必需的。
            InitializeComponent();

            // 在 InitializeComponent() 调用之后添加任何初始化。

            // FileListView.ContextMenu = ContextMenu

            InitialDirectory = Environment.CurrentDirectory;
            CurrentDirectory = InitialDirectory;
            Filter = "所有文件(*.*)|*.*";
            this.IsSaveDialog = IsSaveDialog;
            ModeSelection = ModeSelectionEnum.File;
            Multiselect = false;
            CheckFileExists = true;
            CheckPathExists = true;
            ValidateNames = true;
        }

        private void FilePicker_Load(object sender, EventArgs e)
        {
            RefreshDirectoryList();
            RefreshFilterList();
            RefreshList();
        }

        private void FilePicker_Shown(object sender, EventArgs e)
        {
            LastSourceControl = ComboBox_FileName;
            ComboBox_FileName.Focus();
        }

        public new void Hide()
        {
            Owner = null;
            base.Hide();
        }

        private void HideSelf()
        {
            Hide();
            if (Parent is not null)
                Parent.Focus();
        }

        private Control LastSourceControl;

        private void Button_Enter_Click(object sender, EventArgs e)
        {
            if (ReferenceEquals(LastSourceControl, ComboBox_Directory))
            {
                CurrentDirectory = ComboBox_Directory.Text;
            }
            else if (ReferenceEquals(LastSourceControl, ComboBox_FileName))
            {
                CurrentDirectory = ComboBox_FileName.Text;
            }
            else if (ReferenceEquals(LastSourceControl, FileListView))
            {
                var FocusedItem = FileListView.FocusedItem;
                if (FocusedItem is null)
                    return;
                if (FocusedItem.SubItems.Count != FileListView.Columns.Count + 2)
                    return;
                switch (Conversions.ToInteger(FocusedItem.SubItems[FileListView.Columns.Count + 1].Text))
                {
                    case -1:
                        {
                            CurrentDirectory = FileNameHandling.GetFileDirectory(FileNameHandling.GetDirectoryPathWithoutTailingSeparator(CurrentDirectory));
                            break;
                        }
                    case 0:
                        {
                            CurrentDirectory = FileNameHandling.GetAbsolutePath(FocusedItem.SubItems[0].Text, CurrentDirectory);
                            break;
                        }
                }
            }
        }

        private bool ExistNode(string Path)
        {
            if (Conversions.ToBoolean(ModeSelection & ModeSelectionEnum.File))
            {
                if (File.Exists(Path))
                    return true;
            }
            if (Conversions.ToBoolean(ModeSelection & ModeSelectionEnum.Folder))
            {
                if (Directory.Exists(Path))
                    return true;
            }
            return false;
        }

        private bool CheckComboBox_FileName(bool PopCheckFileExistBox)
        {
            string Path = FileNameHandling.GetAbsolutePath(ComboBox_FileName.Text, CurrentDirectory);
            if (CheckPathExists)
            {
                if (!Directory.Exists(FileNameHandling.GetFileDirectory(Path)))
                    return false;
            }
            if (CheckFileExists)
            {
                if (IsSaveDialog)
                {
                    if (ExistNode(Path))
                    {
                        if (PopCheckFileExistBox)
                        {
                            var dr = MessageBox.Show(@"{0} 已存在。\r\n要替换吗？".Descape().Formats(Path), "确认另存为", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (dr != DialogResult.Yes)
                                return false;
                        }
                    }
                    else if (ValidateNames)
                    {
                        foreach (var c in System.IO.Path.GetInvalidPathChars())
                        {
                            if (Path.Contains(Conversions.ToString(c)))
                                return false;
                        }
                    }
                }
                else if (!ExistNode(Path))
                    return false;
            }
            return true;
        }

        private HashSet<string> FileNameHistorySet = new HashSet<string>();
        private void Button_Select_Click(object sender, EventArgs e)
        {
            if (ReferenceEquals(LastSourceControl, ComboBox_FileName))
            {
                if (CheckComboBox_FileName(true))
                {
                    if (!FileNameHistorySet.Contains(ComboBox_FileName.Text))
                    {
                        FileNameHistorySet.Add(ComboBox_FileName.Text);
                        ComboBox_Directory.Items.Add(ComboBox_FileName.Text);
                    }
                    string Path = FileNameHandling.GetAbsolutePath(ComboBox_FileName.Text, CurrentDirectory);
                    FilePathValue = Path;
                    FileNamesValue = [FileNameHandling.GetRelativePath(Path, CurrentDirectory)];
                    DialogResult = DialogResult.OK;
                    HideSelf();
                }
            }
            else if (ReferenceEquals(LastSourceControl, FileListView))
            {
                var l = new List<string>();
                string f = "";

                foreach (ListViewItem r in Sorter)
                {
                    if (!r.Selected)
                        continue;
                    if (r.SubItems.Count == FileListView.Columns.Count + 2)
                    {
                        switch (Conversions.ToInteger(r.SubItems[FileListView.Columns.Count + 1].Text))
                        {
                            case -1:
                                {
                                    break;
                                }
                            case 0:
                                {
                                    if (Conversions.ToBoolean(ModeSelection & ModeSelectionEnum.File))
                                    {
                                        l.Add(r.SubItems[0].Text);
                                    }

                                    break;
                                }

                            default:
                                {
                                    if (Conversions.ToBoolean(ModeSelection & ModeSelectionEnum.Folder))
                                    {
                                        l.Add(r.SubItems[0].Text);
                                    }

                                    break;
                                }
                        }
                    }
                    if (r.Focused)
                    {
                        f = r.SubItems[0].Text;
                    }
                }
                if (string.IsNullOrEmpty(f) && l.Count > 0)
                    f = l[0];
                FilePathValue = FileNameHandling.GetAbsolutePath(f, CurrentDirectory);
                FileNamesValue = l.ToArray();
                DialogResult = DialogResult.OK;
                HideSelf();
            }
        }

        private void ProcessReturn(object sender, EventArgs e)
        {
            if (ComboBox_Directory.Focused && Button_Enter.Enabled)
            {
                Button_Enter_Click(sender, e);
            }
            else if (ComboBox_FileName.Focused && Button_Select.Enabled)
            {
                Button_Select_Click(sender, e);
            }
            else if (ComboBox_FileName.Focused && Button_Enter.Enabled)
            {
                Button_Enter_Click(sender, e);
            }
            else if (FileListView.Focused && Button_Enter.Enabled)
            {
                Button_Enter_Click(sender, e);
            }
            else if (FileListView.Focused && Button_Select.Enabled)
            {
                Button_Select_Click(sender, e);
            }
        }

        private void FileListView_Enter(object sender, EventArgs e)
        {
            LastSourceControl = FileListView;
        }

        private void FileListView_ItemActivate(object sender, EventArgs e)
        {
            LastSourceControl = FileListView;
            ProcessReturn(sender, e);
        }

        private void ComboBox_Directory_Enter(object sender, EventArgs e)
        {
            LastSourceControl = ComboBox_Directory;
            Button_Enter.Enabled = true;
        }

        private void ComboBox_FileName_Enter(object sender, EventArgs e)
        {
            LastSourceControl = ComboBox_FileName;
            ComboBox_FileName_TextChanged(sender, e);
        }

        private void FileListView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Control | Keys.A:
                    {
                        if (Multiselect)
                        {
                            foreach (ListViewItem r in Sorter)
                            {
                                if (r.SubItems.Count == FileListView.Columns.Count + 2)
                                {
                                    if (Conversions.ToInteger(r.SubItems[FileListView.Columns.Count + 1].Text) == -1)
                                    {
                                        r.Selected = false;
                                    }
                                    else
                                    {
                                        r.Selected = true;
                                    }
                                }
                            }
                        }

                        break;
                    }
                case Keys.Back:
                    {
                        CurrentDirectory = FileNameHandling.GetFileDirectory(FileNameHandling.GetDirectoryPathWithoutTailingSeparator(CurrentDirectory));
                        break;
                    }

                default:
                    {
                        return;
                    }
            }
            e.Handled = true;
        }

        private void FileListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            Timer.Stop();
            var FocusedItem = FileListView.FocusedItem;
            if (FocusedItem is null)
                return;
            if (FocusedItem.SubItems.Count != FileListView.Columns.Count + 2)
                return;
            switch (Conversions.ToInteger(FocusedItem.SubItems[FileListView.Columns.Count + 1].Text))
            {
                case -1:
                case 0:
                    {
                        Button_Enter.Enabled = true;
                        break;
                    }

                default:
                    {
                        Button_Enter.Enabled = false;
                        break;
                    }
            }
            bool Exist = false;
            foreach (ListViewItem r in Sorter)
            {
                if (!r.Selected)
                    continue;
                if (r.SubItems.Count == FileListView.Columns.Count + 2)
                {
                    Exist = true;
                    switch (Conversions.ToInteger(r.SubItems[FileListView.Columns.Count + 1].Text))
                    {
                        case -1:
                            {
                                Button_Select.Enabled = false;
                                return;
                            }
                        case 0:
                            {
                                if (ModeSelection == ModeSelectionEnum.File)
                                {
                                    Button_Select.Enabled = false;
                                    return;
                                }

                                break;
                            }

                        default:
                            {
                                if (ModeSelection == ModeSelectionEnum.Folder)
                                {
                                    Button_Select.Enabled = false;
                                    return;
                                }

                                break;
                            }
                    }
                }
            }
            Button_Select.Enabled = Exist;
        }

        private void ComboBox_FileName_TextChanged(object sender, EventArgs e)
        {
            Box_TextChanged(sender, e);
        }

        private Timer Timer;
        internal int IMECompositing = 0;
        private int Block = 0;
        private void Box_Tick(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(System.Threading.Interlocked.CompareExchange(ref IMECompositing, -1, -1)))
                return;
            System.Threading.Interlocked.Exchange(ref Block, -1);
            Timer.Stop();
            if (ComboBox_FileName.Focused)
            {
                Timer.Interval = 500;
                Timer.Start();
            }
            Button_Select.Enabled = CheckComboBox_FileName(false);
            System.Threading.Interlocked.Exchange(ref Block, 0);
        }
        private void Box_TextChanged(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(System.Threading.Interlocked.CompareExchange(ref Block, -1, -1)))
                return;
            Timer.Stop();
            Timer.Interval = 500;
            Timer.Start();
        }

        private void FilePicker_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Return:
                    {
                        ProcessReturn(sender, e);
                        break;
                    }

                default:
                    {
                        return;
                    }
            }
            e.Handled = true;
        }

        private void Button_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            HideSelf();
        }
    }
}