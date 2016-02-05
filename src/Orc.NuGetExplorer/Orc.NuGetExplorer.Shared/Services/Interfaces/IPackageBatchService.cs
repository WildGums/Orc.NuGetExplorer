// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageBatchService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
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