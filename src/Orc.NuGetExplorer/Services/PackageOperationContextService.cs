// --------------------------------------------------------------------------------------------------------------------
// <copyright file="packageOperationContextService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;

    internal class PackageOperationContextService : IPackageOperationContextService
    {
        #region Fields
        private static readonly object Sync = new object();
        private readonly IList<Exception> _exceptions = new List<Exception>();
        private readonly IPackageOperationNotificationService _packageOperationNotificationService;
        private PackageOperationContext _rootContext;
        #endregion

        #region Constructors
        public PackageOperationContextService(IPackageOperationNotificationService packageOperationNotificationService)
        {
            Argument.IsNotNull(() => packageOperationNotificationService);

            _packageOperationNotificationService = packageOperationNotificationService;
        }
        #endregion

        #region Properties
        public IEnumerable<Exception> CatchedExceptions
        {
            get { return _exceptions.AsEnumerable(); }
        }

        public PackageOperationContext CurrentContext { get; private set; }
        #endregion

        #region Methods
        public IDisposable UseOperationContext(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            return new DisposableToken<PackageOperationContext>(new PackageOperationContext { OperationType = operationType, Packages = packages }, 
                token => ApplyOperationContext(token.Instance),
                token => CloseCurrentOperationContext(token.Instance));
        }

        public void AddCatchedException(Exception exception)
        {
            _exceptions.Add(exception);
        }

        private void ApplyOperationContext(PackageOperationContext context)
        {
            lock (Sync)
            {
                if (_rootContext == null)
                {
                    _exceptions.Clear();

                    _rootContext = new PackageOperationContext();
                    CurrentContext = _rootContext;
                    _packageOperationNotificationService.NotifyOperationBatchStarted(context.OperationType, context.Packages);
                }
                else
                {
                    context.Parent = CurrentContext;
                    CurrentContext = context;
                }
            }
        }

        private void CloseCurrentOperationContext(PackageOperationContext context)
        {
            lock (Sync)
            {
                CurrentContext = CurrentContext.Parent;

                if (CurrentContext == null)
                {
                    _packageOperationNotificationService.NotifyOperationBatchFinished(context.OperationType, context.Packages);
                    _rootContext = null;
                }
            }
        }
        #endregion
    }
}