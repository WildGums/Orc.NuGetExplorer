// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageOperationContextService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;

    internal interface IPackageOperationContextService
    {
        #region Properties
        IEnumerable<Exception> CatchedExceptions { get; }
        PackageOperationContext CurrentContext { get; }
        #endregion

        #region Methods
        IDisposable UseOperationContext(PackageOperationType operationType, params IPackageDetails[] packages);
        void AddCatchedException(Exception exception);
        #endregion
    }
}