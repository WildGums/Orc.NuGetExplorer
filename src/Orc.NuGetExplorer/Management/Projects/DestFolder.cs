﻿namespace Orc.NuGetExplorer.Management;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Catel.Logging;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;

/// <summary>
/// Default project which represents "plugins" folder
/// </summary>
public class DestFolder : IExtensibleProject
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    private readonly PackagePathResolver _pathResolver;

    public DestFolder(string destinationFolder, IDefaultNuGetFramework defaultFramework)
    {
        ArgumentNullException.ThrowIfNull(destinationFolder);
        ArgumentNullException.ThrowIfNull(defaultFramework);

        var targetFramework = defaultFramework.GetHighest().First();
        Framework = targetFramework.DotNetFrameworkName;
        SupportedPlatforms = [FrameworkParser.ToSpecificPlatform(targetFramework)];

        Log.Info($"Current target framework for plugins set as '{Framework}'");

        // Note: commented part for testing correct package resolving to 4.X versions
        //var tfm472 = (defaultFramework as DefaultNuGetFramework).GetFirst();
        //Framework = tfm472.ToString();
        // SupportedPlatforms = ImmutableList.Create(FrameworkParser.ToSpecificPlatform(tfm472));

        // Default initialization
        SupportedPlatforms ??= ImmutableList.Create<NuGetFramework>();

        ContentPath = destinationFolder;
        _pathResolver = new PackagePathResolver(destinationFolder);
    }

    public string Name => "Plugins";

    public string Framework { get; private set; }

    public IReadOnlyList<NuGetFramework> SupportedPlatforms { get; set; }

    public string ContentPath { get; }

    [ObsoleteEx(ReplacementTypeOrMember = "IgnoreMissingDependencies", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
    public bool IgnoreDependencies { get { return IgnoreMissingDependencies; } }

    public bool IgnoreMissingDependencies { get; } = true;

    public bool SupportSideBySide { get; } = false;

    public bool NoCache { get; } = false;

    public PackagePathResolver GetPathResolver()
    {
        return _pathResolver;
    }

    public string GetInstallPath(PackageIdentity packageIdentity)
    {
        return _pathResolver.GetInstallPath(packageIdentity);
    }

    public override string ToString()
    {
        return Name;
    }
}
