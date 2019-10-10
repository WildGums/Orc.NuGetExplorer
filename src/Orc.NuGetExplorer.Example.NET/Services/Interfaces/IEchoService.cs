// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEchoService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer.Example
{
    using Models;

    public interface IEchoService
    {
        #region Methods
        PackageManagementEcho GetPackageManagementEcho();
        #endregion
    }
}