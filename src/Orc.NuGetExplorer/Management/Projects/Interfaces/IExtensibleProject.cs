namespace Orc.NuGetExplorer
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

        bool CanIgnoreDependencies { get; }

        bool CanSupportSideBySide { get; }

        string GetInstallPath(PackageIdentity packageIdentity);
    }
}
