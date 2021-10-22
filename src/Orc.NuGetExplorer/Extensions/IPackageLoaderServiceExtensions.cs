﻿namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using NuGet.Protocol.Core.Types;
    using Orc.NuGetExplorer.Pagination;

    public static class IPackageLoaderServiceExtensions
    {
        public static async Task<IEnumerable<IPackageSearchMetadata>> LoadWithDefaultsAsync(this IPackageLoaderService packageLoaderService, string repository, CancellationToken token = default)
        {
            Argument.IsNotNull(() => packageLoaderService);

            var defaultFilter = new SearchFilter(true);
            var localPagination = new PageContinuation(0, new PackageSourceWrapper(repository));

            return await packageLoaderService.LoadAsync(string.Empty, localPagination, defaultFilter, token);
        }
    }
}