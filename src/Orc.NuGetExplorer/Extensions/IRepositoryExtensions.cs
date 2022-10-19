namespace Orc.NuGetExplorer
{
    using System;
    using NuGet.Configuration;

    public static class IRepositoryExtensions
    {
        public static PackageSource ToPackageSource(this IRepository repository)
        {
            ArgumentNullException.ThrowIfNull(repository);

            return new PackageSource(repository.Source, repository.Name);
        }
    }
}
