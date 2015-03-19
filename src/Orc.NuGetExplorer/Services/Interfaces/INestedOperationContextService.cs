// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INestedOperationContextService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;

    internal interface INestedOperationContextService
    {
        #region Properties
        IEnumerable<Exception> CatchesExceptions { get; }
        #endregion

        #region Methods
        IDisposable OperationContext(PackageOperationType operationType, params IPackageDetails[] packages);
        void AddCatchedException(Exception exception);
        #endregion
    }
}