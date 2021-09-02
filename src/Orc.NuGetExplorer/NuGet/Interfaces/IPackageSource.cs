// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageSource.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    public interface IPackageSource
    {
        bool IsEnabled { get; }
        [ObsoleteEx(TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        bool IsOfficial { get; }
        string Name { get; }
        string Source { get; }
        bool IsAccessible { get; }
        bool IsVerified { get; }
        bool IsSelected { get; set; }

        PackageSourceWrapper GetPackageSource();
    }
}
