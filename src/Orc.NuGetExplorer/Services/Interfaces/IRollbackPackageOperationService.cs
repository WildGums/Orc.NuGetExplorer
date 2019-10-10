// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRollbackPackageOperationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer
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