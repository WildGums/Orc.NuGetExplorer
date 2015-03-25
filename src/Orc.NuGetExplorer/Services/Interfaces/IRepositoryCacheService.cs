// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryCacheService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using NuGet;

    internal interface IRepositoryCacheService
    {
        IRepository GetSerialisableRepository(string name, PackageOperationType operationType, IPackageRepository packageRepository);
        IPackageRepository GetNuGetRepository(IRepository repository);
    }
}