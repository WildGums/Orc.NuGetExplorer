// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer
{
    using NuGet;

    public static class IRepositoryExtensions
    {
        public static IPackageRepository ToNuGetRepository(this IRepository repository)
        {
            return ((Repository) repository).NuGetRepository;
        }
    }
}