// ==========================================================================
// 
// File:        FilePickerInterop.vb
// Location:    Firefly.GUI <Visual Basic .Net>
// Description: File selection dialog - Win32 call wrapper
// Version:     2009.11.30.
// Copyright(C) F.R.C.
// 
// ==========================================================================

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace FontGen
{

    public sealed class FilePickerInterop
    {
        public static Icon GetAssociatedIcon(string Path, bool large)
        {
            var info = new SHFILEINFO(true);
            int cbFileInfo = Marshal.SizeOf(info);
            SHGFI flags;

            if (large)
            {
                flags = SHGFI.Icon | SHGFI.LargeIcon;
            }
            else
            {
                flags = SHGFI.Icon | SHGFI.SmallIcon;
            }

            SHGetFileInfo(Path, 0U, out info, (uint)cbFileInfo, flags);

            if (info.hIcon == IntPtr.Zero)
                return null;
            var i = Icon.FromHandle(info.hIcon).Clone();
            DestroyIcon(info.hIcon);

            return (Icon)i;
        }
        public static string GetTypeName(string Path)
        {
            var info = new SHFILEINFO(true);
            int cbFileInfo = Marshal.SizeOf(info);
            SHGFI flags;
            flags = SHGFI.TypeName;

            SHGetFileInfo(Path, 0U, out info, (uint)cbFileInfo, flags);

            return info.szTypeName;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Ansi)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbfileInfo, SHGFI uFlags);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        private const int MAX_PATH = 260;
        private const int MAX_TYPE = 80;

        private struct SHFILEINFO
        {
            public SHFILEINFO(bool b)
            {
                hIcon = IntPtr.Zero;
                iIcon = 0;
                dwAttributes = 0U;
                szDisplayName = "";
                szTypeName = "";
            }

            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_TYPE)]
            public string szTypeName;
        }

        [Flags()]
        public enum SHGFI : uint
        {
            /// <summary>get icon</summary>
            Icon = 0x100U,

            /// <summary>get display name</summary>
            DisplayName = 0x200U,

            /// <summary>get type name</summary>
            TypeName = 0x400U,

            /// <summary>get attributes</summary>
            Attributes = 0x800U,

            /// <summary>get icon location</summary>
            IconLocation = 0x1000U,

            /// <summary>return exe type</summary>
            ExeType = 0x2000U,

            /// <summary>get system icon index</summary>
            SysIconIndex = 0x4000U,

            /// <summary>put a link overlay on icon</summary>
            LinkOverlay = 0x8000U,

            /// <summary>show icon in selected state</summary>
            Selected = 0x10000U,

            /// <summary>get only specified attributes</summary>
            Attr_Specified = 0x20000U,

            /// <summary>get large icon</summary>
            LargeIcon = 0x0U,

            /// <summary>get small icon</summary>
            SmallIcon = 0x1U,

            /// <summary>get open icon</summary>
            OpenIcon = 0x2U,

            /// <summary>get shell size icon</summary>
            ShellIconize = 0x4U,

            /// <summary>pszPath is a pidl</summary>
            PIDL = 0x8U,

            /// <summary>use passed dwFileAttribute</summary>
            UseFileAttributes = 0x10U,

            /// <summary>apply the appropriate overlays</summary>
            AddOverlays = 0x20U,

            /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
            OverlayIndex = 0x40U
        }
    }
}