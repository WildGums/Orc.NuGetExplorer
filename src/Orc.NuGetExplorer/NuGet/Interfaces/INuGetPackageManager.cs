// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INuGetPackageManager.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using NuGet;

    internal interface INuGetPackageManager : IPackageManager, INuGetPackageManagerNotifier
    {
        
    }
}