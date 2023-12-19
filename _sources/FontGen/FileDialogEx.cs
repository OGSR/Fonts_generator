// ==========================================================================
// 
// File:        FileDialogEx.vb
// Location:    Firefly.GUI <Visual Basic .Net>
// Description: The extended file dialog class has compatibility issues under Win7 and is obsolete. Please use File Picker.
// Version:     2009.11.30.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace FontGen
{

    public partial class FileDialogEx
    {

        #region  modified properties

        protected bool IsSaveDialogValue;
        public bool IsSaveDialog
        {
            get
            {
                return IsSaveDialogValue;
            }
        }

        public enum ModeSelectionEnum
        {
            File = 1,
            Folder = 2,
            FileWithFolder = 3
        }

        protected string InnerFilter;
        protected ModeSelectionEnum ModeSelectionValue = ModeSelectionEnum.FileWithFolder;
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
                        {
                            if (ModeSelectionValue == ModeSelectionEnum.Folder)
                            {
                                Dialog.Filter = InnerFilter;
                            }
                            ModeSelectionValue = value;
                            break;
                        }
                    case ModeSelectionEnum.Folder:
                        {
                            InnerFilter = Dialog.Filter;
                            Dialog.Filter = "-| ";
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

        private bool MultiselectValue;
        public bool Multiselect
        {
            get
            {
                return MultiselectValue;
            }
            set
            {
                MultiselectValue = value;
                if (IsSaveDialog)
                {
                    var mi = typeof(FileDialog).GetMethod("SetOption", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public);
                    // Friend Sub FileDialog.SetOption(ByVal [option] As Integer, ByVal value As Boolean)
                    mi.Invoke(SaveDialogValue, [0x200, value]);
                }
                // If Value Then
                // Throw New NotSupportedException("Win7 has disabled this ability.")
                // End If
                else
                {
                    OpenDialogValue.Multiselect = value;
                }
            }
        }

        public string FileName
        {
            get
            {
                return Dialog.FileName;
            }
            set
            {
                Dialog.FileName = value;
            }
        }

        private List<string> FileNamesValue = new List<string>();
        public string[] FileNames
        {
            get
            {
                return FileNamesValue.ToArray();
            }
        }

        /// <summary>It is only valid when used as an Open File Dialog, otherwise a Not Supported Exception will be thrown.</summary>
        public string SafeFileName
        {
            get
            {
                if (IsSaveDialog)
                    throw new NotSupportedException();
                return OpenDialogValue.SafeFileName;
            }
        }

        private List<string> SafeFileNamesValue = new List<string>();
        /// <summary>It is only valid when used as an Open File Dialog, otherwise a Not Supported Exception will be thrown.</summary>
        public string[] SafeFileNames
        {
            get
            {
                if (IsSaveDialog)
                    throw new NotSupportedException();
                return SafeFileNamesValue.ToArray();
            }
        }

        #endregion

        #region  FileDialogpublic method

        public bool AddExtension
        {
            get
            {
                return Dialog.AddExtension;
            }
            set
            {
                Dialog.AddExtension = value;
            }
        }

        public bool AutoUpgradeEnabled
        {
            get
            {
                return Dialog.AutoUpgradeEnabled;
            }
            set
            {
                Dialog.AutoUpgradeEnabled = value;
            }
        }

        public bool CheckFileExists
        {
            get
            {
                return Dialog.CheckFileExists;
            }
            set
            {
                Dialog.CheckFileExists = value;
            }
        }

        public bool CheckPathExists
        {
            get
            {
                return Dialog.CheckPathExists;
            }
            set
            {
                Dialog.CheckPathExists = value;
            }
        }

        public FileDialogCustomPlacesCollection CustomPlaces
        {
            get
            {
                return Dialog.CustomPlaces;
            }
        }

        public string DefaultExt
        {
            get
            {
                return Dialog.DefaultExt;
            }
            set
            {
                Dialog.DefaultExt = value;
            }
        }

        public bool DereferenceLinks
        {
            get
            {
                return Dialog.DereferenceLinks;
            }
            set
            {
                Dialog.DereferenceLinks = value;
            }
        }

        public string Filter
        {
            get
            {
                if (CanSelectFolders && !CanSelectFiles)
                {
                    throw new InvalidOperationException();
                }
                return Dialog.Filter;
            }
            set
            {
                if (CanSelectFolders && !CanSelectFiles)
                {
                    throw new InvalidOperationException();
                }
                Dialog.Filter = value;
            }
        }

        public int FilterIndex
        {
            get
            {
                return Dialog.FilterIndex;
            }
            set
            {
                Dialog.FilterIndex = value;
            }
        }

        public string InitialDirectory
        {
            get
            {
                return Dialog.InitialDirectory;
            }
            set
            {
                Dialog.InitialDirectory = value;
            }
        }

        public System.IO.Stream OpenFile()
        {
            if (IsSaveDialog)
            {
                return SaveDialogValue.OpenFile();
            }
            else
            {
                return OpenDialogValue.OpenFile();
            }
        }

        public void Reset()
        {
            Dialog.Reset();
            Multiselect = false;
        }

        public bool RestoreDirectory
        {
            get
            {
                return Dialog.RestoreDirectory;
            }
            set
            {
                Dialog.RestoreDirectory = value;
            }
        }

        public bool ShowHelp
        {
            get
            {
                return Dialog.ShowHelp;
            }
            set
            {
                Dialog.ShowHelp = value;
            }
        }

        public bool SupportMultiDottedExtensions
        {
            get
            {
                return Dialog.SupportMultiDottedExtensions;
            }
            set
            {
                Dialog.SupportMultiDottedExtensions = value;
            }
        }

        public string Title
        {
            get
            {
                return Dialog.Title;
            }
            set
            {
                Dialog.Title = value;
            }
        }

        public bool ValidateNames
        {
            get
            {
                return Dialog.ValidateNames;
            }
            set
            {
                Dialog.ValidateNames = value;
            }
        }

        #endregion

        #region  event parameters

        public class SelectionChangedEventArgs : EventArgs
        {

            public SelectionChangedEventArgs(string FolderPath, string FilePath, string[] FileNames)
            {
                FolderPathValue = FolderPath;
                FilePathValue = FilePath;
                FileNamesValue = FileNames;
            }

            protected string FolderPathValue;
            public string FolderPath
            {
                get
                {
                    return FolderPathValue;
                }
            }

            protected string FilePathValue;
            public string FilePath
            {
                get
                {
                    return FilePathValue;
                }
            }

            protected string[] FileNamesValue;
            public string[] FileNames
            {
                get
                {
                    return FileNamesValue;
                }
            }
        }

        public class ButtonOpenClickEventArgs : SelectionChangedEventArgs
        {

            public ButtonOpenClickEventArgs(string FolderPath, string FilePath, string[] FileNames, bool Cancel) : base(FolderPath, FilePath, FileNames)
            {
                CancelValue = Cancel;
            }

            protected bool CancelValue;
            public bool Cancel
            {
                get
                {
                    return CancelValue;
                }
                set
                {
                    CancelValue = value;
                }
            }
        }
        #endregion

        #region  event 

        private bool FileOKValue = false;
        protected void mNativeDialog_ButtonOpenClick(object sender, ButtonOpenClickEventArgs e)
        {
            // If Not (EnableSelectFolders Or (IsSaveDialog AndAlso MultiselectValue)) Then Return

            Dialog.FileName = e.FilePath;
            FileNamesValue.Clear();
            foreach (var f in e.FileNames)
                FileNamesValue.Add(System.IO.Path.Combine(e.FolderPath, f));
            SafeFileNamesValue.Clear();
            SafeFileNamesValue.AddRange(e.FileNames);

            FileOKValue = true;

            var b = new System.ComponentModel.CancelEventArgs();
            FileOk?.Invoke(this, b);
            e.Cancel = b.Cancel;

            if (b.Cancel)
            {
                FileOKValue = false;
            }
        }

        public event SelectionChangedClickEventHandler SelectionChangedClick;

        public delegate void SelectionChangedClickEventHandler(object sender, SelectionChangedEventArgs e);
        protected void mNativeDialog_SelectionChangedClick(object sender, SelectionChangedEventArgs e)
        {
            Dialog.FileName = e.FilePath;
            FileNamesValue.Clear();
            foreach (var f in e.FileNames)
                FileNamesValue.Add(System.IO.Path.Combine(e.FolderPath, f));
            SafeFileNamesValue.Clear();
            SafeFileNamesValue.AddRange(e.FileNames);

            SelectionChangedClick?.Invoke(sender, e);
        }

        public event FileOkEventHandler FileOk;

        public delegate void FileOkEventHandler(object sender, System.ComponentModel.CancelEventArgs e);
        protected bool CancelValue;
        protected void OpenDialogValue_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CanSelectFolders)
            {
                e.Cancel = CancelValue;
            }
            else
            {
                FileOk?.Invoke(this, e);
            }
        }
        protected void SaveDialogValue_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CanSelectFolders || IsSaveDialog && MultiselectValue)
            {
                e.Cancel = CancelValue;
            }
            else
            {
                FileOk?.Invoke(this, e);
            }
        }

        public event HelpRequestEventHandler HelpRequest;

        public delegate void HelpRequestEventHandler(object sender, EventArgs e);
        protected void OpenDialogValue_HelpRequest(object sender, EventArgs e)
        {
            HelpRequest?.Invoke(this, e);
        }
        protected void SaveDialogValue_HelpRequest(object sender, EventArgs e)
        {
            HelpRequest?.Invoke(this, e);
        }

        #endregion

    }
}