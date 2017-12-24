// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApiPackageRegistry.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.NuGetExplorer
{
    public interface IApiPackageRegistry
    {
        void Register(string packageName, string version);

        bool IsRegistered(string packageName);

        void Validate(IPackageDetails package);
    }
}