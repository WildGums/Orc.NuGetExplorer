// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageOperationContext.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;

    internal class PackageOperationContext : IPackageOperationContext
    {
        #region Fields
        private static int _contextCounter;
        #endregion

        #region Constructors
        public PackageOperationContext()
        {
            Id = _contextCounter++;
            CatchedExceptions = new List<Exception>();
        }
        #endregion

        #region Properties
        public int Id { get; private set; }
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
            var context = obj as PackageOperationContext;
            if (context == null)
            {
                return false;
            }

            return Id.Equals(context.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        #endregion
    }
}