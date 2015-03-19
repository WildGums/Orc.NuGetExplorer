// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NestedOperationContextService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;

    internal class NestedOperationContextService : INestedOperationContextService
    {
        #region Fields
        private readonly IPackageOperationNotificationService _packageOperationNotificationService;
        private static readonly object Sync = new object();
        #endregion

        #region Constructors
        public NestedOperationContextService(IPackageOperationNotificationService packageOperationNotificationService)
        {
            Argument.IsNotNull(() => packageOperationNotificationService);

            _packageOperationNotificationService = packageOperationNotificationService;
        }
        #endregion

        private int _contextsCounter;
        private IList<Exception> _exceptions = new List<Exception>();
        #region Methods
        public IEnumerable<Exception> CatchesExceptions
        {
            get { return _exceptions.AsEnumerable(); }
        }

        public IDisposable OperationContext(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            return new DisposableToken(null,
                token => IncrementOperationContexCounter(operationType, packages),
                token => DecrementOperationContexCounter(operationType, packages));
        }

        public void AddCatchedException(Exception exception)
        {
            _exceptions.Add(exception);
        }

        private void IncrementOperationContexCounter(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            lock (Sync)
            {
                if (_contextsCounter == 0)
                {
                    _exceptions.Clear();
                    _packageOperationNotificationService.NotifyOperationBatchStarted(operationType, packages);
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
                    _packageOperationNotificationService.NotifyOperationBatchFinished(operationType, packages);
                }
            }
        }
        #endregion
    }
}