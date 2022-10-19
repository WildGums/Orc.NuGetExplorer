namespace Orc.NuGetExplorer.Packaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Logging;
    using NuGet.Packaging;
    using NuGet.Protocol.Core.Types;

    internal class MultiVersionPackageSearchMetadataBuilder
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IEnumerable<IPackageSearchMetadata> _searchMetadatas;

        private MultiVersionPackageSearchMetadataBuilder(IEnumerable<IPackageSearchMetadata> searchMetadatas)
        {
            _searchMetadatas = searchMetadatas;
        }

        public static MultiVersionPackageSearchMetadataBuilder FromMetadatas(IEnumerable<IPackageSearchMetadata> searchMetadatas)
             => new(searchMetadatas);

        public IPackageSearchMetadata Build(string version)
        {
            var orderedMetadatas = _searchMetadatas.OrderByDescending(x => x.Identity.Version);

            var main = orderedMetadatas.FirstOrDefault(x => x.Identity.Version.OriginalVersion == version);
            if (main is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"'{nameof(main)}' cannot be null");
            }

            var versions = orderedMetadatas.ToList();
            versions.Remove(main);

            var clonedMetadata = Create(versions, main);

            return clonedMetadata;
        }

        public IPackageSearchMetadata Build()
        {
            var orderedMetadatas = _searchMetadatas.OrderByDescending(x => x.Identity.Version);

            var main = orderedMetadatas.FirstOrDefault();
            if (main is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"'{nameof(main)}' cannot be null");
            }

            var versions = orderedMetadatas.Skip(1).ToList();

            var clonedMetadata = Create(versions, main);

            return clonedMetadata;
        }

        private MultiVersionPackageSearchMetadata Create(IEnumerable<IPackageSearchMetadata> versions, IPackageSearchMetadata main)
        {
            var metadata = new MultiVersionPackageSearchMetadata(versions)
            {
                Authors = main.Authors,
                DependencySets = main.DependencySets ?? Enumerable.Empty<PackageDependencyGroup>(),
                Description = main.Description,
                DownloadCount = main.DownloadCount,
                IconUrl = main.IconUrl,
                Identity = main.Identity,
                LicenseUrl = main.LicenseUrl,
                Owners = main.Owners,
                ProjectUrl = main.ProjectUrl,
                Published = main.Published,
                ReportAbuseUrl = main.ReportAbuseUrl,
                RequireLicenseAcceptance = main.RequireLicenseAcceptance,
                Summary = main.Summary,
                Tags = main.Tags,
                Title = main.Title,
                IsListed = main.IsListed,
                PrefixReserved = main.PrefixReserved,
                LicenseMetadata = main.LicenseMetadata,
            };

            return metadata;
        }
    }
}
