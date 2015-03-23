// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageOperationContextService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;

    internal class PackageOperationContextService : IPackageOperationContextService
    {
        #region Fields
        private static readonly object Sync = new object();
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
        public PackageOperationContext CurrentContext { get; private set; }
        #endregion

        #region Methods
        public event EventHandler<OperationContextEventArgs> OperationContextDisposing;

        public IDisposable UseOperationContext(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            return new DisposableToken<PackageOperationContext>(new PackageOperationContext {OperationType = operationType, Packages = packages, FileSystemContext = new TemporaryFileSystemContext()},
                token => ApplyOperationContext(token.Instance),
                token => CloseCurrentOperationContext(token.Instance));
        }

        private void ApplyOperationContext(PackageOperationContext context)
        {
            lock (Sync)
            {
                if (_rootContext == null)
                {
                    context.CatchedExceptions.Clear();

                    _rootContext = context;
                    CurrentContext = context;
                    _packageOperationNotificationService.NotifyOperationBatchStarting(context.OperationType, context.Packages);
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
                if (CurrentContext.Parent == null)
                {
                    OperationContextDisposing.SafeInvoke(this, new OperationContextEventArgs(context));
                    context.FileSystemContext.Dispose();

                    _packageOperationNotificationService.NotifyOperationBatchFinished(context.OperationType, context.Packages);
                    _rootContext = null;
                }

                CurrentContext = CurrentContext.Parent;
            }
        }
        #endregion
    }
}