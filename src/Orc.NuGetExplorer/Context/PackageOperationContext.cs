// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageOperationContext.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using Catel;

    internal class PackageOperationContext : IPackageOperationContext, IUniqueIdentifyable
    {
        #region Constructors
        public PackageOperationContext()
        {
            UniqueIdentifier = UniqueIdentifierHelper.GetUniqueIdentifier<PackageOperationContext>();
            CatchedExceptions = new List<Exception>();
        }
        #endregion

        #region Properties
        public int UniqueIdentifier { get; }
        public IRepository Repository { get; set; }
        public IPackageDetails[] Packages { get; set; }
        public PackageOperationType OperationType { get; set; }
        public IPackageOperationContext Parent { get; set; }
        public IList<Exception> CatchedExceptions { get; private set; }
        public ITemporaryFileSystemContext FileSystemContext { get; set; }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (!(obj is PackageOperationContext context))
            {
                return false;
            }

            return UniqueIdentifier.Equals(context.UniqueIdentifier);
        }

        public override int GetHashCode()
        {
            return UniqueIdentifier.GetHashCode();
        }
        #endregion
    }
}
