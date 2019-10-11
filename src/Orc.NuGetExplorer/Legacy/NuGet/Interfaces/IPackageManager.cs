// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageManager.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer
{
    internal interface IPackageManager : NuGet.IPackageManager, IPackageOperationNotificationService
    {
    }
}