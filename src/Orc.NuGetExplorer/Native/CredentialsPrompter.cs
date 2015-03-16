// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialsPrompter.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Native
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using Catel;

    internal class CredentialsPrompter
    {
        #region Fields
        private string _confirmTarget;
        private bool _isSaveChecked;
        #endregion

        #region Properties
        public string Target { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool AllowStoredCredentials { get; set; }
        public bool ShowSaveCheckBox { get; set; }

        public bool IsSaveChecked
        {
            get { return _isSaveChecked; }
            set { _isSaveChecked = value; }
        }

        public string WindowTitle { get; set; }
        public string MainInstruction { get; set; }
        public string Content { get; set; }
        public DownlevelTextMode DownlevelTextMode { get; set; }
        #endregion

        #region Methods
        public bool ShowDialog()
        {
            var credUiInfo = new CredUi.CredUiInfo();
            credUiInfo.pszCaptionText = "";
            credUiInfo.pszMessageText = "";

            var windowHandle = User32.GetActiveWindow();
            return PromptForCredentialsCredUIWin(windowHandle, true);
        }

        private bool PromptForCredentialsCredUIWin(IntPtr owner, bool storedCredentials)
        {
            if (AllowStoredCredentials)
            {
                var credentials = ReadCredential(Target);
                if (credentials != null)
                {
                    UserName = credentials.UserName;
                    Password = credentials.Password;
                    return true;
                }
            }

            var info = CreateCredUIInfo(owner, false);
            var flags = CredUi.CredUiWinFlags.Generic;
            if (ShowSaveCheckBox)
            {
                flags |= CredUi.CredUiWinFlags.Checkbox;
            }

            var inBuffer = IntPtr.Zero;
            var outBuffer = IntPtr.Zero;

            try
            {
                uint inBufferSize = 0;
                if (UserName.Length > 0)
                {
                    CredUi.CredPackAuthenticationBuffer(0, UserName, Password, IntPtr.Zero, ref inBufferSize);
                    if (inBufferSize > 0)
                    {
                        inBuffer = Marshal.AllocCoTaskMem((int) inBufferSize);
                        if (!CredUi.CredPackAuthenticationBuffer(0, UserName, Password, inBuffer, ref inBufferSize))
                        {
                            throw new CredentialException(Marshal.GetLastWin32Error());
                        }
                    }
                }

                uint outBufferSize = 0;
                uint package = 0;

                var result = CredUi.CredUIPromptForWindowsCredentials(ref info, 0, ref package, inBuffer, inBufferSize,
                    out outBuffer, out outBufferSize, ref _isSaveChecked, flags);
                switch (result)
                {
                    case CredUi.CredUiReturnCodes.NO_ERROR:
                        var userName = new StringBuilder(CredUi.CREDUI_MAX_USERNAME_LENGTH);
                        var password = new StringBuilder(CredUi.CREDUI_MAX_PASSWORD_LENGTH);
                        var userNameSize = (uint) userName.Capacity;
                        var passwordSize = (uint) password.Capacity;
                        uint domainSize = 0;
                        if (!CredUi.CredUnPackAuthenticationBuffer(0, outBuffer, outBufferSize, userName, ref userNameSize, null, ref domainSize, password, ref passwordSize))
                        {
                            throw new CredentialException(Marshal.GetLastWin32Error());
                        }

                        UserName = userName.ToString();
                        Password = password.ToString();

                        if (ShowSaveCheckBox)
                        {
                            _confirmTarget = Target;

                            // If the NativeCredential was stored previously but the user has now cleared the save checkbox,
                            // we want to delete the NativeCredential.
                            if (storedCredentials && !IsSaveChecked)
                            {
                                DeleteCredential(Target);
                            }

                            if (IsSaveChecked)
                            {
                                WriteCredential(Target, UserName, Password);
                            }
                        }
                        return true;

                    case CredUi.CredUiReturnCodes.ERROR_CANCELLED:
                        return false;

                    default:
                        throw new CredentialException((int) result);
                }
            }
            finally
            {
                if (inBuffer != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(inBuffer);
                }

                if (outBuffer != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(outBuffer);
                }
            }
        }

        private static CredUi.SimpleCredentials ReadCredential(string key)
        {
            IntPtr nCredPtr;

            var read = CredUi.CredRead(key, CredUi.CredTypes.CRED_TYPE_GENERIC, 0, out nCredPtr);
            var lastError = Marshal.GetLastWin32Error();

            if (!read)
            {
                if (lastError == (int)CredUi.CredUIReturnCodes.ERROR_NOT_FOUND)
                    return null;
                else
                    throw new CredentialException(lastError);
            }

            var credential = new CredUi.SimpleCredentials();

            using (var criticalCredentialHandle = new CredUi.CriticalCredentialHandle(nCredPtr))
            {
                var cred = criticalCredentialHandle.GetCredential();

                credential.UserName = cred.UserName;
                credential.Password = cred.CredentialBlob;
            }

            return credential;
        }

        private static bool WriteCredential(string key, string userName, string secret)
        {
            var byteArray = Encoding.Unicode.GetBytes(secret);
            if (byteArray.Length > 512)
            {
                throw new ArgumentOutOfRangeException("secret", "The secret message has exceeded 512 bytes.");
            }

            var cred = new CredUi.Credential();
            cred.TargetName = key;
            cred.UserName = userName;
            cred.CredentialBlob = secret;
            cred.CredentialBlobSize = (UInt32) Encoding.Unicode.GetBytes(secret).Length;
            cred.AttributeCount = 0;
            cred.Attributes = IntPtr.Zero;
            cred.Comment = null;
            cred.TargetAlias = null;
            cred.Type = CredUi.CredTypes.CRED_TYPE_GENERIC;
            cred.Persist = CredUi.IsWindowsVistaOrEarlier ? CredUi.CredPersistance.Session : CredUi.CredPersistance.LocalMachine;

            var ncred = CredUi.NativeCredential.GetNativeCredential(cred);
            var written = CredUi.CredWrite(ref ncred, 0);
            var lastError = Marshal.GetLastWin32Error();
            if (!written)
            {
                var message = string.Format("CredWrite failed with the error code {0}.", lastError);
                throw new CredentialException(lastError, message);
            }

            return true;
        }

        private static bool DeleteCredential(string key)
        {
            Argument.IsNotNullOrWhitespace(() => key);

            var found = false;

            if (CredUi.CredDelete(key, CredUi.CredTypes.CRED_TYPE_GENERIC, 0))
            {
                found = true;
            }
            else
            {
                var error = Marshal.GetLastWin32Error();
                if (error != (int) CredUi.CredUiReturnCodes.ERROR_NOT_FOUND)
                {
                    throw new CredentialException(error);
                }
            }

            return found;
        }

        private CredUi.CredUiInfo CreateCredUIInfo(IntPtr owner, bool downlevelText)
        {
            var info = new CredUi.CredUiInfo();
            info.cbSize = Marshal.SizeOf(info);
            info.hwndParent = owner;

            if (downlevelText)
            {
                info.pszCaptionText = WindowTitle;
                switch (DownlevelTextMode)
                {
                    case DownlevelTextMode.MainInstructionAndContent:
                        if (MainInstruction.Length == 0)
                        {
                            info.pszMessageText = Content;
                        }
                        else if (Content.Length == 0)
                        {
                            info.pszMessageText = MainInstruction;
                        }
                        else
                        {
                            info.pszMessageText = MainInstruction + Environment.NewLine + Environment.NewLine + Content;
                        }
                        break;

                    case DownlevelTextMode.MainInstructionOnly:
                        info.pszMessageText = MainInstruction;
                        break;

                    case DownlevelTextMode.ContentOnly:
                        info.pszMessageText = Content;
                        break;
                }
            }
            else
            {
                // Vista and later don't use the window title.
                info.pszMessageText = Content;
                info.pszCaptionText = MainInstruction;
            }

            return info;
        }
        #endregion
    }
}