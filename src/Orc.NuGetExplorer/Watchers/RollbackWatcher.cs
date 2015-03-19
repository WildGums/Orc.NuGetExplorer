// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RollbackWatcher.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;

    public sealed class RollbackWatcher : PackageManagerWatcherBase
    {
        private readonly IPackageOperationContextService _packageOperationContextService;
        private readonly IRollbackPackageOperationService _rollbackPackageOperationService;

        #region Constructors
        internal RollbackWatcher(IPackageOperationNotificationService packageOperationNotificationService, IPackageOperationContextService packageOperationContextService,
            IRollbackPackageOperationService rollbackPackageOperationService) 
            : base(packageOperationNotificationService)
        {
            Argument.IsNotNull(() => packageOperationContextService);
            Argument.IsNotNull(() => rollbackPackageOperationService);

            _packageOperationContextService = packageOperationContextService;
            _rollbackPackageOperationService = rollbackPackageOperationService;
        }
        #endregion

        protected override void OnOperationStarted(object sender, PackageOperationEventArgs e)
        {
            base.OnOperationStarted(sender, e);
        }

        protected override void OnOperationFinished(object sender, PackageOperationEventArgs e)
        {
            base.OnOperationFinished(sender, e);
        }
    }
}