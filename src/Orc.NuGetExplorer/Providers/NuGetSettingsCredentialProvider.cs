// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetSettingsCredentialProvider.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using NuGet;

    internal class NuGetSettingsCredentialProvider : SettingsCredentialProvider
    {
        #region Constructors
        public NuGetSettingsCredentialProvider(ICredentialProvider credentialProvider, IPackageSourceProvider packageSourceProvider) : base(credentialProvider, packageSourceProvider)
        {
        }

        public NuGetSettingsCredentialProvider(ICredentialProvider credentialProvider, IPackageSourceProvider packageSourceProvider, ILogger logger) : base(credentialProvider, packageSourceProvider, logger)
        {
        }
        #endregion
    }
}