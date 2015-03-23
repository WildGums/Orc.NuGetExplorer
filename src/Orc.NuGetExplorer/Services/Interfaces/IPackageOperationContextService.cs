// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageOperationContextService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;

    internal interface IPackageOperationContextService
    {
        #region Properties
        PackageOperationContext CurrentContext { get; }
        #endregion

        #region Methods
        event EventHandler<OperationContextEventArgs> OperationContextDisposing;
        IDisposable UseOperationContext(PackageOperationType operationType, params IPackageDetails[] packages);
        #endregion
    }
}