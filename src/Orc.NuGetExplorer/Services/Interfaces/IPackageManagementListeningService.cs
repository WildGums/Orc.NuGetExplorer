// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageManagementListeningService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;

    public interface IPackageManagementListeningService
    {
        event EventHandler<NuGetPackageOperationEventArgs> PackageInstalling;
        event EventHandler<NuGetPackageOperationEventArgs> PackageInstalled;
        event EventHandler<NuGetPackageOperationEventArgs> PackageUninstalled;
        event EventHandler<NuGetPackageOperationEventArgs> PackageUninstalling;
    }
}