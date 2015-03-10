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
    using System.Windows;
    using Catel.Windows;

    internal class CredentialsPrompter
    {
        #region Fields
        private string _confirmTarget;
        private bool _isSaveChecked;
        #endregion

        #region Properties
        public bool ShowSaveCheckBox { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Target { get; set; }

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
            var creduiInfo = new CredUi.CREDUI_INFO();
            creduiInfo.pszCaptionText = "lklkl";
            creduiInfo.pszMessageText = "message";

            var windowHandle = Application.Current.MainWindow.GetWindowHandle();
            return PromptForCredentialsCredUIWin(windowHandle, true);
        }

        private bool PromptForCredentialsCredUIWin(IntPtr owner, bool storedCredentials)
        {
            var info = CreateCredUIInfo(owner, false);
            var flags = CredUi.CredUIWinFlags.Generic;
            if (ShowSaveCheckBox)
            {
                flags |= CredUi.CredUIWinFlags.Checkbox;
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
                    case CredUi.CredUIReturnCodes.NO_ERROR:
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
                            // If the credential was stored previously but the user has now cleared the save checkbox,
                            // we want to delete the credential.
                            if (storedCredentials && !IsSaveChecked)
                            {
                                /* DeleteCredential(Target);*/
                            }
                        }
                        return true;

                    case CredUi.CredUIReturnCodes.ERROR_CANCELLED:
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

        /*public static bool DeleteCredential(string target)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (target.Length == 0)
                throw new ArgumentException(Properties.Resources.CredentialEmptyTargetError, "target");

            bool found = false;
            lock (_applicationInstanceCredentialCache)
            {
                found = _applicationInstanceCredentialCache.Remove(target);
            }

            if (CredUi.CredDelete(target, CredUi.CredTypes.CRED_TYPE_GENERIC, 0))
            {
                found = true;
            }
            else
            {
                int error = Marshal.GetLastWin32Error();
                if (error != (int)CredUi.CredUIReturnCodes.ERROR_NOT_FOUND)
                    throw new CredentialException(error);
            }
            return found;
        }*/

        private CredUi.CREDUI_INFO CreateCredUIInfo(IntPtr owner, bool downlevelText)
        {
            var info = new CredUi.CREDUI_INFO();
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

    internal enum DownlevelTextMode
    {
        /// <summary>
        /// The text of the <see cref="CredentialDialog.MainInstruction"/> and <see cref="CredentialDialog.Content"/> properties is
        /// concatenated together, separated by an empty line.
        /// </summary>
        MainInstructionAndContent,

        /// <summary>
        /// Only the text of the <see cref="CredentialDialog.MainInstruction"/> property is shown.
        /// </summary>
        MainInstructionOnly,

        /// <summary>
        /// Only the text of the <see cref="CredentialDialog.Content"/> property is shown.
        /// </summary>
        ContentOnly
    }
}