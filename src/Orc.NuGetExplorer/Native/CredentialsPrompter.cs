// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialsPrompter.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Native
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using Catel;
    using Catel.Configuration;
    using Catel.Logging;

    internal class CredentialsPrompter
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IConfigurationService _configurationService;

        #region Fields
        private bool _isSaveChecked;
        #endregion

        public CredentialsPrompter(IConfigurationService configurationService)
        {
            Argument.IsNotNull(() => configurationService);

            _configurationService = configurationService;
        }

        #region Properties
        public string Target { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool AllowStoredCredentials { get; set; }
        public bool ShowSaveCheckBox { get; set; }

        public bool IsSaveChecked => _isSaveChecked;

        public string WindowTitle { get; set; }
        public string MainInstruction { get; set; }
        public string Content { get; set; }
        public DownlevelTextMode DownlevelTextMode { get; set; }
        public bool IsAuthenticationRequired { get; set; }
        #endregion

        #region Methods
        public bool ShowDialog()
        {
            var windowHandle = User32.GetActiveWindow();
            return PromptForCredentialsCredUiWin(windowHandle, true);
        }

        private bool PromptForCredentialsCredUiWin(IntPtr owner, bool storedCredentials)
        {
            if (TryReadStoredCredentials())
            {
                return true;
            }

            if (!IsAuthenticationRequired)
            {
                Log.Debug("No authentication is required, no need to prompt for credentials");
                return false;
            }

            var inBuffer = IntPtr.Zero;
            var outBuffer = IntPtr.Zero;

            try
            {
                return PromptForCredentials(owner, storedCredentials, ref inBuffer, ref outBuffer);
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

        private bool TryReadStoredCredentials()
        {
            if (!AllowStoredCredentials)
            {
                return false;
            }

            Log.Debug("Stored credentials are allowed");

            var credentials = ReadCredential(Target, true);
            if (credentials == null)
            {
                return false;
            }

            Log.Debug("Successfully read stored credentials: '{0}'", credentials);

            UserName = credentials.UserName;
            Password = credentials.Password;
            return true;

        }

        private bool PromptForCredentials(IntPtr owner, bool storedCredentials, ref IntPtr inBuffer, ref IntPtr outBuffer)
        {
            var info = CreateCredUIInfo(owner, false);
            var flags = CredUi.CredUiWinFlags.Generic;
            if (ShowSaveCheckBox)
            {
                flags |= CredUi.CredUiWinFlags.Checkbox;
            }

            uint inBufferSize = 0;
            if (UserName.Length > 0)
            {
                // First call is only to get the required buffer size
                CredUi.CredPackAuthenticationBuffer(0, UserName, Password, IntPtr.Zero, ref inBufferSize);
                if (inBufferSize > 0)
                {
                    inBuffer = Marshal.AllocCoTaskMem((int)inBufferSize);
                    if (!CredUi.CredPackAuthenticationBuffer(0, UserName, Password, inBuffer, ref inBufferSize))
                    {
                        throw Log.ErrorAndCreateException(x => new CredentialException(Marshal.GetLastWin32Error()),
                            "Failed to create the authentication buffer before prompting");
                    }
                }
            }

            uint package = 0;

            Log.Debug("Prompting user for credentials");

            var result = CredUi.CredUIPromptForWindowsCredentials(ref info, 0, ref package, inBuffer, inBufferSize,
                out outBuffer, out var outBufferSize, ref _isSaveChecked, flags);

            switch (result)
            {
                case CredUi.CredUiReturnCodes.NO_ERROR:
                    ManageCredentialsStorage(outBuffer, outBufferSize, storedCredentials);
                    return true;

                case CredUi.CredUiReturnCodes.ERROR_CANCELLED:
                    Log.Debug("User canceled the credentials prompt");
                    return false;

                default:
                    throw Log.ErrorAndCreateException(x => new CredentialException((int)result),
                        "Failed to prompt for credentials, error code '{0}'", result);
            }
        }

        private void ManageCredentialsStorage(IntPtr outBuffer, uint outBufferSize, bool storedCredentials)
        {
            var userName = new StringBuilder(CredUi.CREDUI_MAX_USERNAME_LENGTH);
            var password = new StringBuilder(CredUi.CREDUI_MAX_PASSWORD_LENGTH);
            var userNameSize = (uint)userName.Capacity;
            var passwordSize = (uint)password.Capacity;
            uint domainSize = 0;
            if (!CredUi.CredUnPackAuthenticationBuffer(0, outBuffer, outBufferSize, userName, ref userNameSize, null, ref domainSize, password, ref passwordSize))
            {
                throw Log.ErrorAndCreateException(x => new CredentialException(Marshal.GetLastWin32Error()),
                    "Failed to create the authentication buffer after prompting");
            }

            UserName = userName.ToString();
            Password = password.ToString();

            Log.Debug("User entered credentials with username '{0}'", UserName);

            if (!ShowSaveCheckBox)
            {
                return;
            }

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

        private static string GetEncryptionKey(string key, string username)
        {
            // Note: slightly different than the configuration key so it's not exactly the same
            var encryptionKey = $"{key}_{username}";
            encryptionKey = EncryptionHelper.GetMd5Hash(encryptionKey);

            return encryptionKey;
        }

        private static string GetPasswordConfigurationKey(string key, string username)
        {
            var configurationKeyPostfix = $"{key}__{username}";
            configurationKeyPostfix = EncryptionHelper.GetMd5Hash(configurationKeyPostfix);
            
            var configurationKey = $"NuGet.FeedInfo.{configurationKeyPostfix}";
            return configurationKey;
        }

        private CredUi.SimpleCredentials ReadCredential(string key, bool allowConfigurationFallback)
        {
            Log.Debug("Trying to read credentials for key '{0}'", key);

            var read = CredUi.CredRead(key, CredUi.CredTypes.CRED_TYPE_GENERIC, 0, out var nCredPtr);
            var lastError = Marshal.GetLastWin32Error();

            if (!read)
            {
                if (lastError == (int)CredUi.CredUIReturnCodes.ERROR_NOT_FOUND)
                {
                    Log.Debug("Failed to read credentials, credentials are not found");
                    return null;
                }

                throw Log.ErrorAndCreateException(x => new CredentialException(lastError), "Failed to read credentials, error code is '{0}'", lastError);
            }

            var credential = new CredUi.SimpleCredentials();

            using (var criticalCredentialHandle = new CredUi.CriticalCredentialHandle(nCredPtr))
            {
                var cred = criticalCredentialHandle.GetCredential();

                Log.Debug("Retrieved credentials: {0}", cred);

                credential.UserName = cred.UserName;
                credential.Password = cred.CredentialBlob;

                // Some company policies don't allow us reading the credentials, so
                // that results in an empty password being returned
                if (string.IsNullOrWhiteSpace(credential.Password))
                {
                    if (allowConfigurationFallback)
                    {
                        try
                        {
                            var configurationKey = GetPasswordConfigurationKey(key, credential.UserName);
                            var encryptionKey = GetEncryptionKey(key, credential.UserName);

                            Log.Debug("Failed to read credentials from vault, probably a company policy. Falling back to reading configuration key '{0}'", configurationKey);

                            var encryptedPassword = _configurationService.GetRoamingValue(configurationKey, string.Empty);
                            if (!string.IsNullOrWhiteSpace(encryptedPassword))
                            {
                                var decryptedPassword = EncryptionHelper.Decrypt(encryptedPassword, encryptionKey);
                                credential.Password = decryptedPassword;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Failed to read credentials from alternative configuration");
                        }
                    }

                    if (string.IsNullOrWhiteSpace(credential.Password))
                    {
                        // We failed to read credentials from both vault and configuration
                        return null;
                    }
                }
            }

            return credential;
        }

        private void WriteCredential(string key, string userName, string secret)
        {
            var byteArray = Encoding.Unicode.GetBytes(secret);
            if (byteArray.Length > 512)
            {
                throw Log.ErrorAndCreateException(x => new ArgumentOutOfRangeException(nameof(secret), x), "The secret message has exceeded 512 bytes.");
            }

            Log.Debug("Writing credentials with username '{0}' for key '{1}'", userName, key);

            var cred = new CredUi.Credential
            {
                TargetName = key,
                UserName = userName,
                CredentialBlob = secret,
                CredentialBlobSize = (uint)Encoding.Unicode.GetBytes(secret).Length,
                AttributeCount = 0,
                Attributes = IntPtr.Zero,
                Comment = null,
                TargetAlias = null,
                Type = CredUi.CredTypes.CRED_TYPE_GENERIC,
                Persist = CredUi.IsWindowsVistaOrEarlier ? CredUi.CredPersistance.Session : CredUi.CredPersistance.LocalMachine
            };

            Log.Debug("Persisting credentials as '{0}'", cred.Persist);

            var ncred = CredUi.NativeCredential.GetNativeCredential(cred);
            var written = CredUi.CredWrite(ref ncred, 0);
            var lastError = Marshal.GetLastWin32Error();
            if (!written)
            {
                throw Log.ErrorAndCreateException(x => new CredentialException(lastError, x), "CredWrite failed with the error code '{0}'", lastError);
            }

            // Note: immediately read it for ORCOMP-229
            var credential = ReadCredential(key, false);
            if (credential == null || string.IsNullOrWhiteSpace(credential.Password))
            {
                var configurationKey = GetPasswordConfigurationKey(key, cred.UserName);
                var encryptionKey = GetEncryptionKey(key, cred.UserName);

                Log.Debug("Failed to write credentials to vault, probably a company policy. Falling back to writing configuration key '{0}'", configurationKey);

                var encryptedPassword = EncryptionHelper.Encrypt(secret, encryptionKey);
                _configurationService.SetRoamingValue(configurationKey, encryptedPassword);
            }

            Log.Debug("Successfully written credentials for key '{0}'", key);
        }

        private void DeleteCredential(string key)
        {
            Argument.IsNotNullOrWhitespace(() => key);

            Log.Debug("Deleting credentials with key '{0}'", key);

            if (CredUi.CredDelete(key, CredUi.CredTypes.CRED_TYPE_GENERIC, 0))
            {
                Log.Debug("Successfully deleted credentials");
            }
            else
            {
                var error = Marshal.GetLastWin32Error();
                if (error != (int)CredUi.CredUiReturnCodes.ERROR_NOT_FOUND)
                {
                    throw Log.ErrorAndCreateException(x => new CredentialException(error),
                        "Failed to delete credentials, error code '{0}'", error);
                }
            }
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
