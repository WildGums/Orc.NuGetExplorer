namespace Orc.NuGetExplorer.Pagination
{
    using System;
    using NuGet.Protocol.Core.Types;
    using Orc.NuGetExplorer.Enums;
    using Orc.NuGetExplorer.Models;

    public class DeferToken
    {
        public DeferToken(MetadataOrigin loadType, NuGetPackage nuGetPackage)
        {
            LoadType = DetermineLoadBehavior(loadType);
            Package = nuGetPackage;
        }

        public Func<IPackageSearchMetadata>? PackageSelector { get; set; }

        /// <summary>
        /// Determines type of source which should be sought for state acquiring
        /// </summary>
        public MetadataOrigin LoadType { get; private set; }

        public NuGetPackage Package { get; set; }

        public IPackageSearchMetadata? Result { get; set; }

        public Action<PackageStatus>? UpdateAction { get; set; }

        private static MetadataOrigin DetermineLoadBehavior(MetadataOrigin page)
        {
            switch (page)
            {
                case MetadataOrigin.Browse: return MetadataOrigin.Installed;

                case MetadataOrigin.Installed: return MetadataOrigin.Browse;
            }

            return MetadataOrigin.Browse;
        }
    }
}
