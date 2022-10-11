namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using Catel;

    internal class PackageOperationContext : IPackageOperationContext, IUniqueIdentifyable
    {
        public PackageOperationContext(IPackageDetails[] packages, ITemporaryFileSystemContext fileSystemContext)
        {
            UniqueIdentifier = UniqueIdentifierHelper.GetUniqueIdentifier<PackageOperationContext>();
            Exceptions = new List<Exception>();
            FileSystemContext = fileSystemContext;
            Packages = packages;
        }

        public int UniqueIdentifier { get; }
        public IRepository? Repository { get; set; }
        public IPackageDetails[] Packages { get; set; }
        public PackageOperationType OperationType { get; set; }
        public IPackageOperationContext? Parent { get; set; }
        public IList<Exception>? Exceptions { get; private set; }
        public ITemporaryFileSystemContext FileSystemContext { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is not PackageOperationContext context)
            {
                return false;
            }

            return UniqueIdentifier.Equals(context.UniqueIdentifier);
        }

        public override int GetHashCode()
        {
            return UniqueIdentifier.GetHashCode();
        }
    }
}
