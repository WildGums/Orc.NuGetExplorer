// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredUi.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if DEBUG
// Double protection, only allowed in debug mode
//#define LOG_SENSITIVE_INFO
#endif

namespace Orc.NuGetExplorer.Native
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Text;
    using Microsoft.Win32.SafeHandles;

    internal static class CredUi
    {
        #region Delegates
        [Flags]
        public enum CredUiFlags
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

        public enum CredUIReturnCodes
        {
            NO_ERROR = 0,
            ERROR_CANCELLED = 1223,
            ERROR_NO_SUCH_LOGON_SESSION = 1312,
            ERROR_NOT_FOUND = 1168,
            ERROR_INVALID_ACCOUNT_NAME = 1315,
            ERROR_INSUFFICIENT_BUFFER = 122,
            ERROR_INVALID_PARAMETER = 87,
            ERROR_INVALID_FLAGS = 1004,
        }

        [Flags]
        public enum CredUiWinFlags
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

        #region Properties
        public static bool IsWindowsVistaOrEarlier
        {
            get { return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version <= new Version(6, 0, 6000); }
        }
        #endregion

        #region Methods
        public static string DecryptPassword(byte[] encrypted)
        {
            try
            {
                var unprotectedBytes = System.Security.Cryptography.ProtectedData.Unprotect(encrypted, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(unprotectedBytes);
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                return string.Empty;
            }
        }

        public static byte[] EncryptPassword(string password)
        {
            var unprotectedBytes = Encoding.UTF8.GetBytes(password);
            var protectedBytes = System.Security.Cryptography.ProtectedData.Protect(unprotectedBytes, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
            return protectedBytes;
        }

        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        public static extern CredUiReturnCodes CredUIPromptForWindowsCredentials(
            ref CredUiInfo pUiInfo,
            int dwAuthError,
            ref uint pulAuthPackage,
            IntPtr pvInAuthBuffer,
            uint ulInAuthBufferSize,
            out IntPtr ppvOutAuthBuffer,
            out uint pulOutAuthBufferSize,
            [MarshalAs(UnmanagedType.Bool)] ref bool pfSave,
            CredUiWinFlags dwFlags);

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
        internal static extern bool CredWrite(ref NativeCredential Credential, int Flags);

        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CredPackAuthenticationBuffer(int dwFlags, string pszUserName, string pszPassword, IntPtr pPackedCredentials,
            ref uint pcbPackedCredentials);

        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CredUnPackAuthenticationBuffer(uint dwFlags, IntPtr pAuthBuffer, uint cbAuthBuffer, StringBuilder pszUserName, ref uint pcchMaxUserName, StringBuilder pszDomainName, ref uint pcchMaxDomainName, StringBuilder pszPassword, ref uint pcchMaxPassword);
        #endregion

        internal sealed class CriticalCredentialHandle : CriticalHandleZeroOrMinusOneIsInvalid
        {
            #region Constructors
            internal CriticalCredentialHandle(IntPtr preExistingHandle)
            {
                SetHandle(preExistingHandle);
            }
            #endregion

            #region Methods
            internal Credential GetCredential()
            {
                if (IsInvalid)
                {
                    throw new InvalidOperationException("Invalid CriticalHandle!");
                }

                // Get the Credential from the mem location
                var ncred = (NativeCredential)Marshal.PtrToStructure(handle, typeof(NativeCredential));

                // Create a managed Credential type and fill it with data from the native counterpart.
                var cred = new Credential();
                cred.CredentialBlobSize = ncred.CredentialBlobSize;
                cred.UserName = Marshal.PtrToStringUni(ncred.UserName);
                cred.TargetName = Marshal.PtrToStringUni(ncred.TargetName);
                cred.TargetAlias = Marshal.PtrToStringUni(ncred.TargetAlias);
                cred.Type = ncred.Type;
                cred.Flags = ncred.Flags;
                cred.Persist = (CredPersistance)ncred.Persist;

                byte[] encryptedPassword = new byte[ncred.CredentialBlobSize];
                Marshal.Copy(ncred.CredentialBlob, encryptedPassword, 0, encryptedPassword.Length);
                cred.CredentialBlob = DecryptPassword(encryptedPassword);

                return cred;
            }

            // Perform any specific actions to release the handle in the ReleaseHandle method.
            // Often, you need to use Pinvoke to make a call into the Win32 API to release the 
            // handle. In this case, however, we can use the Marshal class to release the unmanaged memory.

            protected override bool ReleaseHandle()
            {
                // If the handle was set, free it. Return success.
                if (!IsInvalid)
                {
                    // NOTE: We should also ZERO out the memory allocated to the handle, before free'ing it
                    // so there are no traces of the sensitive data left in memory.
                    CredFree(handle);

                    // Mark the handle as invalid for future users.
                    SetHandleAsInvalid();

                    return true;
                }

                return false;
            }
            #endregion
        }

        internal class SimpleCredentials
        {
            #region Properties
            public string UserName { get; set; }
            public string Password { get; set; }
            #endregion

            public override string ToString()
            {
                var value = $"Username = '{UserName}'";

#if LOG_SENSITIVE_INFO
                value += string.Format(", Password = '{0}'", Password);
#endif

                return value;
            }
        }

        internal enum CredUiReturnCodes
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

        internal enum CredPersistance
        {
            Session = 1,
            LocalMachine = 2,
            Enterprise = 3
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct CredUiInfo
        {
            #region Fields
            public int cbSize;
            public IntPtr hwndParent;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszMessageText;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszCaptionText;
            public IntPtr hbmBanner;
            #endregion
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct NativeCredential
        {
            public UInt32 Flags;
            public CredTypes Type;
            public IntPtr TargetName;
            public IntPtr Comment;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
            public UInt32 CredentialBlobSize;
            public IntPtr CredentialBlob;
            public UInt32 Persist;
            public UInt32 AttributeCount;
            public IntPtr Attributes;
            public IntPtr TargetAlias;
            public IntPtr UserName;

            /// <summary>
            /// This method derives a NativeCredential instance from a given Credential instance.
            /// </summary>
            /// <param name="cred">The managed Credential counterpart containing data to be stored.</param>
            /// <returns>A NativeCredential instance that is derived from the given Credential
            /// instance.</returns>
            internal static NativeCredential GetNativeCredential(Credential cred)
            {
                var ncred = new NativeCredential();

                ncred.AttributeCount = 0;
                ncred.Attributes = IntPtr.Zero;
                ncred.Comment = IntPtr.Zero;
                ncred.TargetAlias = IntPtr.Zero;
                ncred.Type = CredTypes.CRED_TYPE_GENERIC;
                ncred.Persist = (UInt32)cred.Persist;
                ncred.TargetName = Marshal.StringToCoTaskMemUni(cred.TargetName);

                var encryptedPassword = EncryptPassword(cred.CredentialBlob);

                try
                {
                    ncred.CredentialBlob = Marshal.AllocHGlobal(encryptedPassword.Length);
                    Marshal.Copy(encryptedPassword, 0, ncred.CredentialBlob, encryptedPassword.Length);
                    ncred.CredentialBlobSize = (uint)encryptedPassword.Length;
                    ncred.Type = CredTypes.CRED_TYPE_GENERIC;
                }
                finally
                {
                    Marshal.FreeCoTaskMem(ncred.CredentialBlob);
                }

                ncred.UserName = Marshal.StringToCoTaskMemUni(cred.UserName);

                return ncred;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct Credential
        {
            public UInt32 Flags;
            public CredTypes Type;
            public string TargetName;
            public string Comment;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
            public UInt32 CredentialBlobSize;
            public string CredentialBlob;
            public CredPersistance Persist;
            public UInt32 AttributeCount;
            public IntPtr Attributes;
            public string TargetAlias;
            public string UserName;

            public override string ToString()
            {
                var result = "[Credential info] ";

                result += string.Join(", ", new[]
                {
                    string.Format("User name = '{0}'", UserName),
                    string.Format("Target name = '{0}'", TargetName),
                    string.Format("Type = '{0}'", Type),
                    string.Format("Blob size = '{0}'", CredentialBlobSize),
                });

#if LOG_SENSITIVE_INFO
                result += string.Join(", ", new[]
                {
                    string.Format("Blob = '{0}'", CredentialBlob),
                });
#endif

                return result;
            }
        }
    }
}
