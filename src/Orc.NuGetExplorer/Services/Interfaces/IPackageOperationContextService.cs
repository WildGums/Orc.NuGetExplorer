// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageOperationContextService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;

    public interface IPackageOperationContextService
    {
        #region Properties
        IPackageOperationContext CurrentContext { get; }
        #endregion

        #region Methods
        event EventHandler<OperationContextEventArgs> OperationContextDisposing;
        IDisposable UseOperationContext(PackageOperationType operationType, params IPackageDetails[] packages);
        #endregion
    }
}