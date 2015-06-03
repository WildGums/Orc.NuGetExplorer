// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackagesBatchServiceExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Threading;

    public static class IPackagesBatchServiceExtensions
    {
        #region Methods
        public static Task ShowPackagesBatchAsync(this IPackageBatchService packageBatchService, IEnumerable<IPackageDetails> packageDetails, PackageOperationType operationType)
        {
            Argument.IsNotNull(() => packageBatchService);

            return TaskHelper.Run(() => packageBatchService.ShowPackagesBatch(packageDetails, operationType));
        }
        #endregion
    }
}