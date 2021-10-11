﻿namespace Orc.NuGetExplorer
{
    using System.Collections.Immutable;
    using NuGet.Frameworks;
    using NuGet.Packaging;
    using NuGet.Packaging.Core;

    public interface IExtensibleProject
    {
        string Name { get; }

        string Framework { get; }

        string ContentPath { get; }

        ImmutableList<NuGetFramework> SupportedPlatforms { get; set; }

        PackagePathResolver GetPathResolver();

        string GetInstallPath(PackageIdentity packageIdentity);

        void Install();

        void Update();

        void Uninstall();
    }
}
