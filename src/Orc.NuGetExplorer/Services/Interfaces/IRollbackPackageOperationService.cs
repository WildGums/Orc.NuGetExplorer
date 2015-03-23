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
        void PushRollbackAction(Action rollbackAction, PackageOperationContext context);
        void Rollback(PackageOperationContext context);
        void ClearRollbackActions(PackageOperationContext context);
    }
}