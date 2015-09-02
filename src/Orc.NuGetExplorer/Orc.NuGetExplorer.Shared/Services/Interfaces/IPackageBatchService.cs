// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageBatchService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    public interface IPackageBatchService
    {
        #region Methods
        void ShowPackagesBatch(IEnumerable<IPackageDetails> packageDetails, PackageOperationType operationType);
        #endregion
    }
}