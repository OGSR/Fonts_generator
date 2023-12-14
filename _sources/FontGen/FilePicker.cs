// ==========================================================================
// 
// File:        FilePicker.vb
// Location:    Firefly.GUI <Visual Basic .Net>
// Description: 文件选取对话框
// Version:     2009.12.04.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Firefly;
using Microsoft.VisualBasic.CompilerServices;

namespace FontGen
{


    /// <summary>文件选取对话框，用于统一打开和保存单个和多个文件和文件夹，可替代OpenFileDialog、SaveFileDialog、FolderBrowserDialog三个对话框。</summary>
    public partial class FilePicker
    {
        private string InitialDirectoryValue;
        public string InitialDirectory
        {
            get
            {
                return InitialDirectoryValue;
            }
            set
            {
                if (Directory.Exists(value))
                {
                    InitialDirectoryValue = value;
                    CurrentDirectory = value;
                }
            }
        }

        private string CurrentDirectoryValue;
        public string CurrentDirectory
        {
            get
            {
                return CurrentDirectoryValue;
            }
            set
            {
                value = FileNameHandling.GetDirectoryPathWithTailingSeparator(FileNameHandling.GetAbsolutePath(value, CurrentDirectoryValue));
                if (Directory.Exists(value))
                {
                    string Previous = CurrentDirectoryValue;
                    try
                    {
                        CurrentDirectoryValue = value;
                        ComboBox_Directory.Text = value;
                        RefreshList();
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        MessageBox.Show(@"无法访问{0}。\r\n拒绝访问。".Descape().Formats(CurrentDirectoryValue), "位置不可用", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        CurrentDirectoryValue = Previous;
                        ComboBox_Directory.Text = Previous;
                        RefreshList();
                    }
                }
            }
        }

        private List<KeyValuePair<string, string[]>> FilterValues = new List<KeyValuePair<string, string[]>>();
        private int FilterIndexValue = -1;
        public string Filter
        {
            get
            {
                return string.Join("|", (from p in FilterValues
                                         select (p.Key + "|" + string.Join(";", p.Value))).ToArray());
            }
            set
            {
                if (string.IsNullOrEmpty(value) || !value.Contains("|"))
                {
                    FilterValues.Clear();
                    FilterIndexValue = -1;
                    return;
                }
                string[] r = value.Split('|');
                if (r.Length % 2 != 0)
                    throw new ArgumentException();
                FilterValues = Enumerable.Range(0, r.Length / 2).Select(i => new KeyValuePair<string, string[]>(r[i * 2], r[i * 2 + 1].Split(';'))).ToList();
                FilterIndexValue = 0;
            }
        }

        public int CurrentFilterIndex
        {
            get
            {
                return FilterIndexValue;
            }
            set
            {
                FilterIndexValue = value;
                if (FilterIndexValue >= 0 && FilterIndexValue < FilterValues.Count)
                {
                    ComboBox_Filter.SelectedIndex = FilterIndexValue;
                }
            }
        }

        protected bool IsSaveDialogValue = false;
        public bool IsSaveDialog
        {
            get
            {
                return IsSaveDialogValue;
            }
            set
            {
                IsSaveDialogValue = value;
                if ((Title ?? "") != (Name ?? ""))
                    return;
                if (value)
                {
                    Title = "另存为..";
                }
                else
                {
                    Title = "打开";
                }
            }
        }

        public enum ModeSelectionEnum
        {
            File = 1,
            Folder = 2,
            FileWithFolder = 3
        }

        protected ModeSelectionEnum ModeSelectionValue = ModeSelectionEnum.File;
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
                    case ModeSelectionEnum.FileWithFolder:
                    case ModeSelectionEnum.Folder:
                        {
                            ModeSelectionValue = value;
                            break;
                        }

                    default:
                        {
                            throw new ArgumentException();
                        }
                }
            }
        }

        public bool CanSelectFiles
        {
            get
            {
                return Conversions.ToBoolean(ModeSelectionValue & ModeSelectionEnum.File);
            }
        }

        public bool CanSelectFolders
        {
            get
            {
                return Conversions.ToBoolean(ModeSelectionValue & ModeSelectionEnum.Folder);
            }
        }

        private bool MultiselectValue = false;
        public bool Multiselect
        {
            get
            {
                return MultiselectValue;
            }
            set
            {
                MultiselectValue = value;
                FileListView.MultiSelect = value;
            }
        }

        private string FilePathValue = "";
        public string FilePath
        {
            get
            {
                return FilePathValue;
            }
            set
            {
                FilePathValue = value;
                CurrentDirectory = FileNameHandling.GetFileDirectory(value);
                ComboBox_FileName.Text = FileNameHandling.GetFileName(value);
            }
        }

        private string[] FileNamesValue = [];
        public string[] FileNames
        {
            get
            {
                return FileNamesValue.ToArray();
            }
        }

        public string[] FilePaths
        {
            get
            {
                string d = CurrentDirectory;
                return (from f in FileNamesValue
                        select FileNameHandling.GetAbsolutePath(f, d)).ToArray();
            }
        }

        private bool CheckFileExistsValue = true;
        public bool CheckFileExists
        {
            get
            {
                return CheckFileExistsValue;
            }
            set
            {
                CheckFileExistsValue = value;
            }
        }

        private bool CheckPathExistsValue = true;
        public bool CheckPathExists
        {
            get
            {
                return CheckPathExistsValue;
            }
            set
            {
                CheckPathExistsValue = value;
            }
        }

        public string Title
        {
            get
            {
                return Text;
            }
            set
            {
                Text = value;
            }
        }

        private bool ValidateNamesValue = true;
        public bool ValidateNames
        {
            get
            {
                return ValidateNamesValue;
            }
            set
            {
                ValidateNamesValue = value;
            }
        }
    }
}