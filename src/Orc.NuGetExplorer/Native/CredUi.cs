// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Native
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class CredUi
    {
        public static bool IsWindowsVistaOrLater
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(6, 0, 6000);
            }
        }

        #region Delegates
        [Flags]
        public enum CREDUI_FLAGS
        {
            INCORRECT_PASSWORD = 0x1,
            DO_NOT_PERSIST = 0x2,
            REQUEST_ADMINISTRATOR = 0x4,
            EXCLUDE_CERTIFICATES = 0x8,
            REQUIRE_CERTIFICATE = 0x10,
            SHOW_SAVE_CHECK_BOX = 0x40,
            ALWAYS_SHOW_UI = 0x80,
            REQUIRE_SMARTCARD = 0x100,
            PASSWORD_ONLY_OK = 0x200,
            VALIDATE_USERNAME = 0x400,
            COMPLETE_USERNAME = 0x800,
            PERSIST = 0x1000,
            SERVER_CREDENTIAL = 0x4000,
            EXPECT_CONFIRMATION = 0x20000,
            GENERIC_CREDENTIALS = 0x40000,
            USERNAME_TARGET_CREDENTIALS = 0x80000,
            KEEP_USERNAME = 0x100000
        }

        [Flags]
        public enum CredUIWinFlags
        {
            Generic = 0x1,
            Checkbox = 0x2,
            AuthPackageOnly = 0x10,
            InCredOnly = 0x20,
            EnumerateAdmins = 0x100,
            EnumerateCurrentUser = 0x200,
            SecurePrompt = 0x1000,
            Pack32Wow = 0x10000000
        }
        #endregion

        #region Fields
        internal const int CREDUI_MAX_USERNAME_LENGTH = 256 + 1 + 256;
        internal const int CREDUI_MAX_PASSWORD_LENGTH = 256;
        #endregion

        #region Methods
        //[DllImport("credui.dll", CharSet = CharSet.Unicode)]
        //internal static extern CredUIReturnCodes CredUIPromptForCredentials(
        //    ref CREDUI_INFO pUiInfo,
        //    string targetName,
        //    IntPtr Reserved,
        //    int dwAuthError,
        //    StringBuilder pszUserName,
        //    uint ulUserNameMaxChars,
        //    StringBuilder pszPassword,
        //    uint ulPaswordMaxChars,
        //    [MarshalAs(UnmanagedType.Bool), In(), Out()] ref bool pfSave,
        //    CREDUI_FLAGS dwFlags);

        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        public static extern CredUIReturnCodes CredUIPromptForWindowsCredentials(
            ref CREDUI_INFO pUiInfo,
            int dwAuthError,
            ref uint pulAuthPackage,
            IntPtr pvInAuthBuffer,
            uint ulInAuthBufferSize,
            out IntPtr ppvOutAuthBuffer,
            out uint pulOutAuthBufferSize,
            [MarshalAs(UnmanagedType.Bool)] ref bool pfSave,
            CredUIWinFlags dwFlags);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "CredReadW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CredRead(string TargetName, CredTypes Type, int Flags, out IntPtr Credential);

        [DllImport("advapi32.dll"), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal static extern void CredFree(IntPtr Buffer);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "CredDeleteW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CredDelete(string TargetName, CredTypes Type, int Flags);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "CredWriteW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CredWrite(ref CREDENTIAL Credential, int Flags);

        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CredPackAuthenticationBuffer(int dwFlags, string pszUserName, string pszPassword, IntPtr pPackedCredentials, 
            ref uint pcbPackedCredentials);

        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CredUnPackAuthenticationBuffer(uint dwFlags, IntPtr pAuthBuffer, uint cbAuthBuffer, StringBuilder pszUserName, ref uint pcchMaxUserName, StringBuilder pszDomainName, ref uint pcchMaxDomainName, StringBuilder pszPassword, ref uint pcchMaxPassword);
        #endregion

        internal enum CredUIReturnCodes
        {
            NO_ERROR = 0,
            ERROR_CANCELLED = 1223,
            ERROR_NO_SUCH_LOGON_SESSION = 1312,
            ERROR_NOT_FOUND = 1168,
            ERROR_INVALID_ACCOUNT_NAME = 1315,
            ERROR_INSUFFICIENT_BUFFER = 122,
            ERROR_INVALID_PARAMETER = 87,
            ERROR_INVALID_FLAGS = 1004
        }

        internal enum CredTypes
        {
            CRED_TYPE_GENERIC = 1,
            CRED_TYPE_DOMAIN_PASSWORD = 2,
            CRED_TYPE_DOMAIN_CERTIFICATE = 3,
            CRED_TYPE_DOMAIN_VISIBLE_PASSWORD = 4
        }

        internal enum CredPersist
        {
            Session = 1,
            LocalMachine = 2,
            Enterprise = 3
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct CREDUI_INFO
        {
            #region Fields
            public int cbSize;
            public IntPtr hwndParent;
            [MarshalAs(UnmanagedType.LPWStr)] public string pszMessageText;
            [MarshalAs(UnmanagedType.LPWStr)] public string pszCaptionText;
            public IntPtr hbmBanner;
            #endregion
        }

        public struct CREDENTIAL
        {
            #region Fields
            public int AttributeCount;
            public IntPtr Attributes;
            [MarshalAs(UnmanagedType.LPWStr)] public string Comment;
            public IntPtr CredentialBlob;
            public uint CredentialBlobSize;
            public int Flags;
            public long LastWritten;
            [MarshalAs(UnmanagedType.U4)] public CredPersist Persist;
            [MarshalAs(UnmanagedType.LPWStr)] public string TargetAlias;
            [MarshalAs(UnmanagedType.LPWStr)] public string TargetName;
            public CredTypes Type;
            [MarshalAs(UnmanagedType.LPWStr)] public string UserName;
            #endregion
        }
    }
}