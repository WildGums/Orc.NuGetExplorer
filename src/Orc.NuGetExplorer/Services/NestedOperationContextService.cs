// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NestedOperationContextService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;

    internal class NestedOperationContextService : INestedOperationContextService
    {
        #region Fields
        private readonly INuGetPackageManagerNotifier _nuGetPackageManagerNotifier;
        private static readonly object Sync = new object();
        #endregion

        #region Constructors
        public NestedOperationContextService(INuGetPackageManagerNotifier nuGetPackageManagerNotifier)
        {
            Argument.IsNotNull(() => nuGetPackageManagerNotifier);

            _nuGetPackageManagerNotifier = nuGetPackageManagerNotifier;
        }
        #endregion

        private int _contextsCounter;

        #region Methods
        public IDisposable OperationContext(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            return new DisposableToken(null,
                token => IncrementOperationContexCounter(operationType, packages),
                token => DecrementOperationContexCounter(operationType, packages));
        }

        private void IncrementOperationContexCounter(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            lock (Sync)
            {
                if (_contextsCounter == 0)
                {
                    _nuGetPackageManagerNotifier.NotifyOperationBatchStarted(operationType, packages);
                }

                _contextsCounter++;
            }            
        }

        private void DecrementOperationContexCounter(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            lock (Sync)
            {
                _contextsCounter--;

                if (_contextsCounter == 0)
                {
                    _nuGetPackageManagerNotifier.NotifyOperationBatchFinished(operationType, packages);
                }
            }
        }
        #endregion
    }
}