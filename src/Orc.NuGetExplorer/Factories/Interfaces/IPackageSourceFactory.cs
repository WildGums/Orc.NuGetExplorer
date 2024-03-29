﻿namespace Orc.NuGetExplorer;

public interface IPackageSourceFactory
{
    IPackageSource CreatePackageSource(string source, string name, bool isEnabled, bool isOfficial);
}