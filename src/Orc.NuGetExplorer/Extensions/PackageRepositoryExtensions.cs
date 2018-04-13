// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageRepositoryExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Threading;
    using MethodTimer;
    using NuGet;

    internal static class PackageRepositoryExtensions
    {
        #region Methods
        [Time]
        public static IEnumerable<IPackage> FindAll(this IPackageRepository packageRepository, bool allowPrereleaseVersions,
            int skip = 0, int take = 10)
        {
            Argument.IsNotNull(() => packageRepository);

            return packageRepository.FindFiltered(string.Empty, allowPrereleaseVersions, skip, take);
        }

        [Time]
        public static IEnumerable<IPackage> FindFiltered(this IPackageRepository packageRepository, string filter, bool allowPrereleaseVersions,
            int skip = 0, int take = 10)
        {
            Argument.IsNotNull(() => packageRepository);

            var queryable = BuildQueryForSingleVersion(packageRepository, filter, allowPrereleaseVersions);
            var result = queryable.OrderByDescending(x => x.DownloadCount).Skip(skip).Take(take).ToList();
            return result;
        }

        [Time]
        public static IEnumerable<IPackage> FindPackageVersions(this IPackageRepository packageRepository, IPackage package, bool allowPrereleaseVersions,
            ref int skip, int minimalTake = 10)
        {
            Argument.IsNotNull(() => packageRepository);

            if (skip < 0)
            {
                return Enumerable.Empty<IPackage>();
            }

            var queryable = packageRepository.GetPackages().Where(x => Equals(x.Id, package.Id)).Skip(skip).Take(minimalTake);

            var result = new List<IPackage>(queryable.ToList());

            if (result.Count < minimalTake)
            {
                skip = -1;
            }
            else
            {
                skip += minimalTake;
            }

            if (!allowPrereleaseVersions && result.Any())
            {
                result = result.Where(x => x.IsReleaseVersion()).ToList();

                var count = result.Count;

                if (skip >= 0 && count < minimalTake)
                {
                    var additional = packageRepository.FindPackageVersions(package, false, ref skip, minimalTake).ToList();
                    result.AddRange(additional);
                }
            }

            return result;
        }

        public static IQueryable<IPackage> BuildQueryForSingleVersion(this IPackageRepository packageRepository, string filter, bool allowPrereleaseVersions)
        {
            Argument.IsNotNull(() => packageRepository);

            var queryable = packageRepository.GetPackages();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = filter.Trim();
                var localPackageRepository = packageRepository as LocalPackageRepository;
                if (localPackageRepository == null)
                {
                    queryable = queryable.Where(x => x.Title.ToUpper().Contains(filter.ToUpper()));
                }
                else
                {
                    queryable = queryable.Where(x => CultureInfo.InvariantCulture.CompareInfo.IndexOf(x.Id, filter, CompareOptions.IgnoreCase) != -1);
                }
            }

            if (allowPrereleaseVersions)
            {
                queryable = queryable.Where(x => x.IsAbsoluteLatestVersion);
            }
            else
            {
                queryable = queryable.Where(x => x.IsLatestVersion);
            }

            return queryable;
        }
        #endregion
    }
}