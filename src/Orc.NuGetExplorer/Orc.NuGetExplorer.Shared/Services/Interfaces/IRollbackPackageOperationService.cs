// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRollbackPackageOperationService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;

    internal interface IRollbackPackageOperationService
    {
        #region Methods
        void PushRollbackAction(Action rollbackAction, IPackageOperationContext context);
        void Rollback(IPackageOperationContext context);
        void ClearRollbackActions(IPackageOperationContext context);
        #endregion
    }
}