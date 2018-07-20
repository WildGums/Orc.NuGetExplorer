// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetSettingsCredentialProvider.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using NuGet;

    public class NuGetSettingsCredentialProvider : SettingsCredentialProvider
    {
        #region Constructors
        public NuGetSettingsCredentialProvider(ICredentialProvider credentialProvider, IPackageSourceProvider packageSourceProvider)
            : base(credentialProvider, packageSourceProvider)
        {
        }

        public NuGetSettingsCredentialProvider(ICredentialProvider credentialProvider, IPackageSourceProvider packageSourceProvider, ILogger logger)
            : base(credentialProvider, packageSourceProvider, logger)
        {
        }
        #endregion
    }
}