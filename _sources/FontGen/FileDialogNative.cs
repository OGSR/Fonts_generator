// ==========================================================================
// 
// File:        FileDialogNative.vb
// Location: Firefly.GUI <Visual Basic .Net>
// Description: Extended file dialog class, there are compatibility issues under Win7, it is obsolete, please use File Picker
// This file uses the method mentioned in http://www.codeproject.com/KB/dialog/Open File Dialog Ex.aspx
// Known issue 1: Hidden extension files such as shortcuts cannot be processed correctly
// Known issue 2: Unable to automatically enter the folder when pressing Enter
// Version: 2009.11.30.
// Copyright(C) F.R.C.
//
// ==========================================================================


using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Firefly;
using Microsoft.VisualBasic.CompilerServices;

namespace FontGen
{

    public partial class FileDialogEx : Form
    {
        private OpenFileDialog _OpenDialogValue;

        protected virtual OpenFileDialog OpenDialogValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _OpenDialogValue;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_OpenDialogValue != null)
                {
                    _OpenDialogValue.FileOk -= OpenDialogValue_FileOk;
                    _OpenDialogValue.HelpRequest -= OpenDialogValue_HelpRequest;
                }

                _OpenDialogValue = value;
                if (_OpenDialogValue != null)
                {
                    _OpenDialogValue.FileOk += OpenDialogValue_FileOk;
                    _OpenDialogValue.HelpRequest += OpenDialogValue_HelpRequest;
                }
            }
        }
        private SaveFileDialog _SaveDialogValue;

        protected virtual SaveFileDialog SaveDialogValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _SaveDialogValue;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_SaveDialogValue != null)
                {
                    _SaveDialogValue.FileOk -= SaveDialogValue_FileOk;
                    _SaveDialogValue.HelpRequest -= SaveDialogValue_HelpRequest;
                }

                _SaveDialogValue = value;
                if (_SaveDialogValue != null)
                {
                    _SaveDialogValue.FileOk += SaveDialogValue_FileOk;
                    _SaveDialogValue.HelpRequest += SaveDialogValue_HelpRequest;
                }
            }
        }

        protected internal bool SetIntializeFileNameToNull = false;

        private FileDialogBase _mNativeDialog;

        protected virtual FileDialogBase mNativeDialog
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _mNativeDialog;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_mNativeDialog != null)
                {
                    _mNativeDialog.ButtonOpenClick -= mNativeDialog_ButtonOpenClick;
                    _mNativeDialog.SelectionChangedClick -= mNativeDialog_SelectionChangedClick;
                }

                _mNativeDialog = value;

                if (_mNativeDialog != null)
                {
                    _mNativeDialog.ButtonOpenClick += mNativeDialog_ButtonOpenClick;
                    _mNativeDialog.SelectionChangedClick += mNativeDialog_SelectionChangedClick;
                }
            }
        }
        protected IntPtr mDialogHandle = IntPtr.Zero;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)Msg.WM_ACTIVATE)
            {
                if (mDialogHandle != m.LParam)
                {
                    var ClassNameSB = new StringBuilder(256);
                    FileDialogInterop.GetClassName(new HandleRef(this, m.LParam), ClassNameSB, ClassNameSB.Capacity);
                    string ClassName = ClassNameSB.ToString();

                    if (ClassName == "#32770")
                    {
                        mDialogHandle = m.LParam;
                        if (mNativeDialog is not null)
                        {
                            SetIntializeFileNameToNull = mNativeDialog.SetIntializeFileNameToNull;
                            mNativeDialog.Dispose();
                        }
                        mNativeDialog = new FileDialogBase(mDialogHandle, this);
                        mNativeDialog.SetIntializeFileNameToNull = SetIntializeFileNameToNull;
                    }
                }
            }
            base.WndProc(ref m);
        }

        protected FileDialog Dialog
        {
            get
            {
                if (IsSaveDialog)
                    return SaveDialogValue;
                return OpenDialogValue;
            }
        }

        public new DialogResult ShowDialog()
        {
            var returnDialogResult = DialogResult.Cancel;
            FileOKValue = false;
            Show();
            int HideFileExt = Conversions.ToInteger(Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced").GetValue("HideFileExt", 1));
            if (HideFileExt != 0)
            {
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true).SetValue("HideFileExt", 0);
            }

            if (FileName.EndsWith(Conversions.ToString(Path.DirectorySeparatorChar)))
            {
                FileName = Path.Combine(FileName, Path.GetFileName(FileName.TrimEnd(Path.DirectorySeparatorChar)));
                mNativeDialog.SetIntializeFileNameToNull = true;
            }

            returnDialogResult = Dialog.ShowDialog(this);

            if (HideFileExt != 0)
            {
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true).SetValue("HideFileExt", 1);
            }

            Hide();

            if (FileOKValue)
                return DialogResult.OK;
            return returnDialogResult;
        }

        protected class FileDialogBase : NativeWindow, IDisposable
        {

            protected FileDialogEx mSourceControl;
            protected IntPtr mFileDialogHandle;
            protected IntPtr mListViewHandle;
            protected IntPtr mEditFileNameHandle;

            protected internal bool SetIntializeFileNameToNull = false;

            public FileDialogBase(IntPtr Handle, FileDialogEx SourceControl)
            {
                mFileDialogHandle = Handle;
                mSourceControl = SourceControl;
                AssignHandle(mFileDialogHandle);
            }

            public event ButtonOpenClickEventHandler ButtonOpenClick;

            public delegate void ButtonOpenClickEventHandler(object sender, ButtonOpenClickEventArgs e);
            public event SelectionChangedClickEventHandler SelectionChangedClick;

            public delegate void SelectionChangedClickEventHandler(object sender, SelectionChangedEventArgs e);

            protected bool FileDialogEnumWindowCallBack(IntPtr hWnd, int lParam)
            {
                var ClassNameSB = new StringBuilder(256);
                FileDialogInterop.GetClassName(new HandleRef(this, hWnd), ClassNameSB, ClassNameSB.Capacity);
                string ClassName = ClassNameSB.ToString();
                int ControlId = FileDialogInterop.GetDlgCtrlID(new HandleRef(this, hWnd));

                if (ClassName == "SysListView32")
                {
                    mListViewHandle = hWnd;
                }
                else if (ClassName == "Edit")
                {
                    mEditFileNameHandle = hWnd;
                }

                return true;
            }

            protected void FileDialogEnumWindow()
            {
                FileDialogInterop.EnumChildWindows(new HandleRef(this, mFileDialogHandle), FileDialogEnumWindowCallBack, new IntPtr(0));
            }

            protected string GetFilePath()
            {
                var Length = FileDialogInterop.SendMessage(new HandleRef(this, mFileDialogHandle), (uint)CommonDialogMessages.CDM_GETFILEPATH, IntPtr.Zero, IntPtr.Zero);
                if (Length.ToInt32() < 0)
                    return "";

                var FilePath = new StringBuilder(Length.ToInt32());
                FileDialogInterop.SendMessage(new HandleRef(this, mFileDialogHandle), (uint)CommonDialogMessages.CDM_GETFILEPATH, new IntPtr(FilePath.Capacity), FilePath);
                string t = FilePath.ToString();
                if (t.Contains("\""))
                {
                    int a = t.IndexOf("\"");
                    int b = t.IndexOf("\"", a + 1);
                    t = t.Substring(0, a) + t.Substring(a + 1, b - a - 1);
                }
                return t;
            }

            protected string GetFolderPath()
            {
                var Length = FileDialogInterop.SendMessage(new HandleRef(this, mFileDialogHandle), (uint)CommonDialogMessages.CDM_GETFOLDERPATH, IntPtr.Zero, IntPtr.Zero);
                if (Length.ToInt32() < 0)
                    return "";

                var FolderPath = new StringBuilder(Length.ToInt32());
                FileDialogInterop.SendMessage(new HandleRef(this, mFileDialogHandle), (uint)CommonDialogMessages.CDM_GETFOLDERPATH, new IntPtr(FolderPath.Capacity), FolderPath);
                return FolderPath.ToString();
            }

            protected string[] GetFileNames()
            {
                var Length = FileDialogInterop.SendMessage(new HandleRef(this, mListViewHandle), (uint)ListViewMessages.LVM_GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero);

                string FolderPath = "";
                if (!mSourceControl.CanSelectFolders)
                {
                    FolderPath = GetFolderPath();
                }

                var FileNames = new List<string>();
                string FileName = "";
                for (int n = 0, loopTo = (int)(Length.ToInt64() - 1L); n <= loopTo; n++)
                {
                    var NamePtr = Marshal.AllocHGlobal(2048 * 4);
                    var Item = new LVITEM() { mask =uint.MaxValue, iItem = n, iSubItem = 0, state = 0U, stateMask = uint.MaxValue, pszText = NamePtr, cchTextMax = 2048 };
                    var p = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(LVITEM)) * 4);
                    Marshal.StructureToPtr(Item, p, true);
                    var Successful = FileDialogInterop.SendMessage(new HandleRef(this, mListViewHandle), (int)ListViewMessages.LVM_GETITEM, new IntPtr(Marshal.SizeOf(typeof(LVITEM))), p);
                    Item = (LVITEM)Marshal.PtrToStructure(p, typeof(LVITEM));
                    Marshal.FreeHGlobal(p);
                    string Name = "";
                    if (Item.pszText != new IntPtr(-1))
                    {
                        Name = Marshal.PtrToStringUni(NamePtr);
                    }
                    Marshal.FreeHGlobal(NamePtr);

                    if (Successful != IntPtr.Zero)
                        continue;

                    // state
                    // LVIS_FOCUSED 0x0001
                    // LVIS_SELECTED 0x0002
                    bool Focused = Conversions.ToBoolean(Item.state & 1L);
                    bool Selected = Conversions.ToBoolean(Item.state & 2L);

                    if (!string.IsNullOrEmpty(Name))
                    {
                        // System.Console.WriteLine("Cap " & Name & " " & Focused & " " & Selected)
                        if (!mSourceControl.CanSelectFiles)
                        {
                            if (File.Exists(Path.Combine(FolderPath, Name)))
                                continue;
                        }
                        if (!mSourceControl.CanSelectFolders)
                        {
                            if (Directory.Exists(Path.Combine(FolderPath, Name)))
                                continue;
                        }

                        if (Selected)
                            FileNames.Add(Name);
                        if (Focused)
                            FileName = Name;
                    }
                    else
                    {
                        // System.Console.WriteLine("Cap " & Focused & " " & Selected)
                    }
                }

                if (!string.IsNullOrEmpty(FileName) && FileNames.Contains(FileName))
                {
                    FileNames.Remove(FileName);
                    FileNames.Insert(0, FileName);
                }

                // If FileNames.Count = 0 Then
                // Console.WriteLine("Cap " & FileNames.Count & " " & Length.ToInt64)
                // End If

                return FileNames.ToArray();
            }

            protected string GetPathFromFolderPath(string FilePath, string FolderPath)
            {
                if ((FilePath ?? "") == (FolderPath ?? "") && !FilePath.EndsWith(Conversions.ToString(Path.DirectorySeparatorChar)))
                {
                    if (string.IsNullOrEmpty(FilePath))
                        return FilePath;
                    return FilePath + Path.DirectorySeparatorChar;
                }
                return FilePath;
            }

            protected string GetEditFileNameText()
            {
                var TextLength = FileDialogInterop.SendMessage(new HandleRef(this, mEditFileNameHandle), (uint)Msg.WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero);
                var Text = new StringBuilder(TextLength.ToInt32() + 2);
                FileDialogInterop.SendMessage(new HandleRef(this, mEditFileNameHandle), (uint)Msg.WM_GETTEXT, new IntPtr(Text.Capacity), Text);

                return Text.ToString();
            }

            protected string[] GetEditFileNames()
            {
                string Text = GetEditFileNameText();
                if (Text is null || string.IsNullOrEmpty(Text.Trim()))
                    return [];
                if (!Text.Contains("\""))
                {
                    return [Text];
                }

                var FileNames = new List<string>();
                var FileName = new StringBuilder();
                bool Odd = false;
                foreach (var c in Text)
                {
                    if (Conversions.ToString(c) == "\"")
                    {
                        Odd = !Odd;
                        if (!Odd)
                        {
                            FileNames.Add(FileName.ToString());
                            FileName = new StringBuilder();
                        }
                    }
                    else if (Odd)
                    {
                        FileName.Append(c);
                    }
                }
                if (Odd)
                    throw new InvalidCastException();

                return FileNames.ToArray();
            }

            protected void SetEditFileNames(string[] FileNames)
            {
                StringBuilder sb;
                if (FileNames is not null)
                {
                    if (FileNames.Length == 1)
                    {
                        sb = new StringBuilder(FileNames[0]);
                    }
                    else
                    {
                        string[] Names = (string[])FileNames.Clone();
                        for (int n = 0, loopTo = Names.Length - 1; n <= loopTo; n++)
                            Names[n] = "\"" + Names[n] + "\"";

                        sb = new StringBuilder(string.Join(" ", Names));
                    }
                }
                else
                {
                    sb = new StringBuilder();
                }
                FileDialogInterop.SendMessage(new HandleRef(this, mEditFileNameHandle), (uint)Msg.WM_SETTEXT, IntPtr.Zero, sb);
            }

            protected enum AfterButtonOpenClicked
            {
                None = 0,
                RefreshSelection = 1,
                InternalMessageHandling = 2
            }

            protected AfterButtonOpenClicked OnButtonOpenClicked(ref Message m)
            {
                // If mSourceControl.EnableSelectFolders OrElse (mSourceControl.IsSaveDialog AndAlso mSourceControl.MultiselectValue) Then
                FileDialogEnumWindow();

                string FilePath = GetFilePath();
                string FolderPath = GetFolderPath();
                string[] FileNames = null;

                try
                {
                    FileNames = GetEditFileNames();
                }
                catch (Exception ex)
                {
                    return AfterButtonOpenClicked.RefreshSelection | AfterButtonOpenClicked.InternalMessageHandling;
                }

                if (FileNames.Length == 0)
                {
                    if (!mSourceControl.CanSelectFolders)
                        return AfterButtonOpenClicked.RefreshSelection | AfterButtonOpenClicked.InternalMessageHandling;

                    FilePath = GetPathFromFolderPath(FilePath, FolderPath);
                    FileNames = [FolderPath];
                    var b = new ButtonOpenClickEventArgs(FolderPath, FilePath, FileNames, false);
                    ButtonOpenClick?.Invoke(this, b);
                    mSourceControl.CancelValue = b.Cancel;
                    if (!b.Cancel)
                    {
                        var argm = new Message() { Msg = (int)Msg.WM_COMMAND, WParam = new IntPtr(2), LParam = m.LParam, HWnd = m.HWnd, Result = IntPtr.Zero };
                        base.WndProc(ref argm);
                        return AfterButtonOpenClicked.None;
                    }
                }

                FilePath = GetPathFromFolderPath(FilePath, FolderPath);

                char[] Invailds = Path.GetInvalidPathChars();
                foreach (var f in FileNames)
                {
                    foreach (var c in Invailds)
                    {
                        if (string.IsNullOrEmpty(f) || f.Contains(Conversions.ToString(c)))
                            return AfterButtonOpenClicked.None;
                    }
                }

                if (mSourceControl.CheckPathExists)
                {
                    foreach (var f in FileNames)
                    {
                        if (string.IsNullOrEmpty(f))
                            return AfterButtonOpenClicked.None;
                        if (!Directory.Exists(Path.Combine(FolderPath, Path.GetDirectoryName(f))))
                            return AfterButtonOpenClicked.None;
                    }
                }

                if (mSourceControl.CheckFileExists)
                {
                    switch (mSourceControl.ModeSelection)
                    {
                        case ModeSelectionEnum.File:
                            {
                                foreach (var f in FileNames)
                                {
                                    if (!File.Exists(Path.Combine(FolderPath, f)))
                                        return AfterButtonOpenClicked.None;
                                }

                                break;
                            }
                        case ModeSelectionEnum.Folder:
                            {
                                foreach (var f in FileNames)
                                {
                                    if (!Directory.Exists(Path.Combine(FolderPath, f)))
                                        return AfterButtonOpenClicked.None;
                                }

                                break;
                            }
                        case ModeSelectionEnum.FileWithFolder:
                            {
                                foreach (var f in FileNames)
                                {
                                    if (!(File.Exists(Path.Combine(FolderPath, f)) || Directory.Exists(Path.Combine(FolderPath, f))))
                                        return AfterButtonOpenClicked.None;
                                }

                                break;
                            }
                    }
                }

                var e = new ButtonOpenClickEventArgs(FolderPath, FilePath, FileNames, false);
                ButtonOpenClick?.Invoke(this, e);
                mSourceControl.CancelValue = e.Cancel;
                if (!e.Cancel)
                {
                    var argm1 = new Message() { Msg = (int)Msg.WM_COMMAND, WParam = new IntPtr(2), LParam = m.LParam, HWnd = m.HWnd, Result = IntPtr.Zero };
                    base.WndProc(ref argm1);
                    return AfterButtonOpenClicked.None;
                }
                // End If

                return AfterButtonOpenClicked.RefreshSelection | AfterButtonOpenClicked.InternalMessageHandling;
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == (int)Msg.WM_COMMAND)
                {
                    int ControlId = (int)(m.WParam.ToInt64() & 0xFFFFL);
                    int Notification = (int)(m.WParam.ToInt64() >> 16 & 0xFFFFL);
                    if (ControlId == 1 && Notification == 0) // ButtonOpen, BN_CLICKED
                    {
                        // System.Console.WriteLine("{0} : {1}, {2}, {3}", "FileDialogBase", DirectCast(m.Msg, Msg), m.LParam, m.WParam)
                        // System.Console.WriteLine(" {0}, {1}", ControlId, Notification)

                        var r = OnButtonOpenClicked(ref m);
                        switch (r)
                        {
                            case AfterButtonOpenClicked.None:
                                {
                                    return;
                                }
                            case AfterButtonOpenClicked.RefreshSelection | AfterButtonOpenClicked.InternalMessageHandling:
                                {
                                    break;
                                }
                            case AfterButtonOpenClicked.InternalMessageHandling:
                                {
                                    base.WndProc(ref m);
                                    return;
                                }

                            default:
                                {
                                    throw new InvalidDataException();
                                }
                        }
                    }
                }

                switch (m.Msg)
                {
                    case 1324:
                        {
                            FileDialogEnumWindow();

                            string FilePath;
                            string FolderPath;
                            string[] FileNames;
                            try
                            {
                                FilePath = GetFilePath();
                                FolderPath = GetFolderPath();
                                FileNames = GetFileNames();
                                if (FileNames.Length == 0 && (FilePath ?? "") != (FolderPath ?? ""))
                                {
                                    FileNames = [FileNameHandling.GetFileName(FilePath)];
                                }
                                FilePath = GetPathFromFolderPath(FilePath, FolderPath);
                            }
                            catch (Exception ex)
                            {
                                break;
                            }

                            FileDialogEnumWindow();

                            if (SetIntializeFileNameToNull)
                            {
                                SetEditFileNames([]);
                                SetIntializeFileNameToNull = false;
                            }
                            else if (mSourceControl.MultiselectValue)
                            {
                                SetEditFileNames(FileNames);
                            }
                            else if (FileNames.Length > 1)
                            {
                                SetEditFileNames([Path.GetFileName(FilePath)]);
                            }
                            else if (FileNames.Length > 0)
                            {
                                SetEditFileNames(FileNames);
                            }

                            var b = new SelectionChangedEventArgs(FolderPath, FilePath, FileNames);
                            SelectionChangedClick?.Invoke(this, b);
                            break;
                        }
                }
                base.WndProc(ref m);
            }

            private bool _Dispose_Disposed = false;

            public void Dispose()
            {
                if (!_Dispose_Disposed)
                {
                    ReleaseHandle();
                    _Dispose_Disposed = true;
                }
                GC.SuppressFinalize(this);
            }
        }
    }
}