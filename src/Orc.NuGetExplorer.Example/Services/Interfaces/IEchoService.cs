// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEchoService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Example
{
    using Models;

    public interface IEchoService
    {
        #region Methods
        PackageManagementEcho GetPackageManagementEcho();
        #endregion
    }
}