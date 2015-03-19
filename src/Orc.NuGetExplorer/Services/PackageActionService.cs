// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageActionService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Services;
    using NuGet;

    internal class PackageActionService : IPackageActionService
    {
        #region Fields
        private readonly IPackageRepository _localRepository;
        private readonly ILogger _logger;
        private readonly IPackageManager _packageManager;
        private readonly IPackageQueryService _packageQueryService;
        private readonly INestedOperationContextService _nestedOperationContextService;
        private readonly IPackageRepositoryService _packageRepositoryService;
        private readonly IPleaseWaitService _pleaseWaitService;
        #endregion

        #region Constructors
        public PackageActionService(IPleaseWaitService pleaseWaitService, IPackageManager packageManager,
            IPackageRepositoryService packageRepositoryService, ILogger logger, IPackageQueryService packageQueryService,
            INestedOperationContextService nestedOperationContextService)
        {
            Argument.IsNotNull(() => pleaseWaitService);
            Argument.IsNotNull(() => packageManager);
            Argument.IsNotNull(() => packageRepositoryService);
            Argument.IsNotNull(() => logger);
            Argument.IsNotNull(() => packageQueryService);
            Argument.IsNotNull(() => nestedOperationContextService);

            _pleaseWaitService = pleaseWaitService;
            _packageManager = packageManager;
            _packageRepositoryService = packageRepositoryService;
            _logger = logger;
            _packageQueryService = packageQueryService;
            _nestedOperationContextService = nestedOperationContextService;

            _localRepository = packageRepositoryService.LocalRepository;

            DependencyVersion = DependencyVersion.Lowest;
        }
        #endregion

        #region Properties
        public DependencyVersion DependencyVersion { get; set; }
        #endregion

        #region Methods
        public string GetActionName(PackageOperationType operationType)
        {
            return Enum.GetName(typeof (PackageOperationType), operationType);
        }

        public async Task Execute(PackageOperationType operationType, PackageDetails packageDetails, IPackageRepository sourceRepository = null, bool allowedPrerelease = false)
        {
            Argument.IsNotNull(() => packageDetails);

            await Task.Factory.StartNew(() =>
            {
                using (_pleaseWaitService.WaitingScope())
                {
                    switch (operationType)
                    {
                        case PackageOperationType.Uninstall:
                            UninstallPackage(packageDetails);
                            break;
                        case PackageOperationType.Install:
                            InstallPackage(packageDetails, allowedPrerelease, sourceRepository);
                            break;
                        case PackageOperationType.Update:
                            UpdatePackages(packageDetails, allowedPrerelease);
                            break;
                    }
                }
            });

            packageDetails.IsActionExecuted = null;
        }

        public bool CanExecute(PackageOperationType operationType, PackageDetails package)
        {
            if (package == null)
            {
                return false;
            }

            if (package.IsActionExecuted == null)
            {
                switch (operationType)
                {
                    case PackageOperationType.Install:
                        package.IsActionExecuted = !CanInstall(package);
                        break;

                    case PackageOperationType.Update:
                        package.IsActionExecuted = !CanUpdate(package);
                        break;

                    case PackageOperationType.Uninstall:
                        package.IsActionExecuted = !CanUninstall(package);
                        break;

                    default:
                        package.IsActionExecuted = null;
                        break;
                }
            }

            return !(package.IsActionExecuted ?? true);
        }

        public bool IsRefreshReqired(PackageOperationType operationType)
        {
            switch (operationType)
            {
                case PackageOperationType.Uninstall:
                    return true;
                case PackageOperationType.Install:
                    return false;
                case PackageOperationType.Update:
                    return true;
            }

            return false;
        }

        public string GetPluralActionName(PackageOperationType operationType)
        {
            return string.Format("{0} all", Enum.GetName(typeof (PackageOperationType), operationType));
        }

        private bool CanInstall(PackageDetails package)
        {
            Argument.IsNotNull(() => package);

            var count = _packageQueryService.CountPackages(_localRepository, package.Id);

            return count == 0;
        }

        private bool CanUpdate(PackageDetails package)
        {
            Argument.IsNotNull(() => package);

            var count = _packageQueryService.CountPackages(_localRepository, package);

            return count == 0;
        }

        private bool CanUninstall(PackageDetails package)
        {
            return true;
        }

        private void UninstallPackage(PackageDetails packageDetails)
        {
            Argument.IsNotNull(() => packageDetails);

            using (_nestedOperationContextService.OperationContext(PackageOperationType.Uninstall, packageDetails))
            {
                var dependentsResolver = new DependentsWalker(_localRepository, null);

                var walker = new UninstallWalker(_localRepository, dependentsResolver, null,
                    _logger, true, false);

                try
                {
                    var operations = walker.ResolveOperations(packageDetails.Package); // checking uninstall ability

                    _packageManager.UninstallPackage(packageDetails.Package, false, true);
                }
                catch (Exception exception)
                {
                    _logger.Log(MessageLevel.Error, exception.Message);
                    _nestedOperationContextService.AddCatchedException(exception);
                }
            }
        }

        private void InstallPackage(PackageDetails packageDetails, bool allowedPrerelease, IPackageRepository sourceRepository = null)
        {
            Argument.IsNotNull(() => packageDetails);

            if (sourceRepository == null)
            {
                sourceRepository = _packageRepositoryService.GetSourceAggregateRepository();
            }

            using (_nestedOperationContextService.OperationContext(PackageOperationType.Install, packageDetails))
            {
                var walker = new InstallWalker(_localRepository, sourceRepository, null, _logger, false, allowedPrerelease,
                    DependencyVersion);

                try
                {
                    var operations = walker.ResolveOperations(packageDetails.Package); // checking install ability

                    _packageManager.InstallPackage(packageDetails.Package, false, allowedPrerelease, false);
                }
                catch (Exception exception)
                {
                    _logger.Log(MessageLevel.Error, exception.Message);
                    _nestedOperationContextService.AddCatchedException(exception);
                }
            }
        }

        private void UpdatePackages(PackageDetails packageDetails, bool allowedPrerelease)
        {
            Argument.IsNotNull(() => packageDetails);

            using (_nestedOperationContextService.OperationContext(PackageOperationType.Update, packageDetails))
            {
                try
                {
                    _packageManager.UpdatePackage(packageDetails.Package, true, allowedPrerelease);
                }
                catch (Exception exception)
                {
                    _logger.Log(MessageLevel.Error, exception.Message);
                    _nestedOperationContextService.AddCatchedException(exception);
                }
            }
        }
        #endregion
    }
}