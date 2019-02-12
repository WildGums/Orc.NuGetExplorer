// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageOperationContextService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using Catel.IoC;

    internal class PackageOperationContextService : IPackageOperationContextService
    {
        #region Fields
        private readonly object _lockObject = new object();
        private readonly IPackageOperationNotificationService _packageOperationNotificationService;
        private readonly ITypeFactory _typeFactory;
        private PackageOperationContext _rootContext;
        #endregion

        #region Constructors
        public PackageOperationContextService(IPackageOperationNotificationService packageOperationNotificationService, ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => packageOperationNotificationService);
            Argument.IsNotNull(() => typeFactory);

            _packageOperationNotificationService = packageOperationNotificationService;
            _typeFactory = typeFactory;
        }
        #endregion

        #region Properties
        public IPackageOperationContext CurrentContext { get; private set; }
        #endregion

        #region Methods
        public event EventHandler<OperationContextEventArgs> OperationContextDisposing;

        public IDisposable UseOperationContext(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            var context = _typeFactory.CreateInstance<TemporaryFileSystemContext>();
            return new DisposableToken<PackageOperationContext>(new PackageOperationContext {OperationType = operationType, Packages = packages, FileSystemContext = context},
                token => ApplyOperationContext(token.Instance),
                token => CloseCurrentOperationContext(token.Instance));
        }

        private void ApplyOperationContext(PackageOperationContext context)
        {
            Argument.IsNotNull(() => context);

            lock (_lockObject)
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
            Argument.IsNotNull(() => context);

            lock (_lockObject)
            {
                if (CurrentContext.Parent == null)
                {
                    OperationContextDisposing?.Invoke(this, new OperationContextEventArgs(context));
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
