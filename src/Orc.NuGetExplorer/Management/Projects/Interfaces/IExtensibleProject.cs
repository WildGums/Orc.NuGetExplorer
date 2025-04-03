namespace Orc.NuGetExplorer;

using System.Collections.Generic;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;

public interface IExtensibleProject
{
    string Name { get; }

    string Framework { get; }

    string ContentPath { get; }

    IReadOnlyList<NuGetFramework> SupportedPlatforms { get; set; }

    PackagePathResolver GetPathResolver();

    [ObsoleteEx(ReplacementTypeOrMember = "IgnoreMissingDependencies", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
    bool IgnoreDependencies { get; }

    bool IgnoreMissingDependencies { get; }

    bool SupportSideBySide { get; }

    bool NoCache { get; }

    string GetInstallPath(PackageIdentity packageIdentity);
}
