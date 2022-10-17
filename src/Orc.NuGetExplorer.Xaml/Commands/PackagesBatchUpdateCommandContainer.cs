namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using Orc.NuGetExplorer.Management;
    using Orc.NuGetExplorer.Packaging;
    using Orc.NuGetExplorer.Windows;

    internal class PackagesBatchUpdateCommandContainer : CommandContainerBase<IManagerPage>
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IProgressManager _progressManager;
        private readonly IMessageService _messageService;
        private readonly INuGetPackageManager _projectManager;
        private readonly IExtensibleProjectLocator _projectLocator;
        private readonly IPackageCommandService _packageCommandService;
        private readonly IPackageOperationContextService _packageOperationContextService;

        public PackagesBatchUpdateCommandContainer(ICommandManager commandManager, IProgressManager progressManager, IMessageService messageService, INuGetPackageManager projectManager, 
            IExtensibleProjectLocator projectLocator, IPackageCommandService packageCommandService, IPackageOperationContextService packageOperationContextService)
            : base(Commands.Packages.BatchUpdate, commandManager)
        {
            _progressManager = progressManager;
            _messageService = messageService;
            _projectManager = projectManager;
            _projectLocator = projectLocator;
            _packageCommandService = packageCommandService;
            _packageOperationContextService = packageOperationContextService;
        }

        protected override bool CanExecute(IManagerPage? parameter)
        {
            if (parameter is null)
            {
                return false;
            }

            return parameter.PackageItems.Any(x => x.IsChecked);
        }

        protected async override Task ExecuteAsync(IManagerPage? parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter);

            var sourcePage = parameter;
            var parentVM = (IViewModel)parameter;

            try
            {
                _progressManager.ShowBar(parentVM);

                var batchedPackages = sourcePage.PackageItems.Where(x => x.IsChecked).ToList();

                if (batchedPackages.Any(x => x.ValidationContext?.HasErrors ?? false))
                {
                    await _messageService.ShowErrorAsync("One or more package(s) cannot be updated due to validation errors", "Can't update packages");
                    return;
                }

                var projects = _projectLocator.GetAllExtensibleProjects()
                            .Where(x => _projectLocator.IsEnabled(x)).ToList();

                using (var cts = new CancellationTokenSource())
                {
                    var updatePackageList = new List<IPackageDetails>();

                    foreach (var package in batchedPackages)
                    {
                        var targetProjects = new List<IExtensibleProject>();

                        foreach (var project in projects)
                        {
                            if (!await _projectManager.IsPackageInstalledAsync(project, package.Identity, cts.Token))
                            {
                                targetProjects.Add(project);
                            }
                        }

                        var targetVersion = (await package.LoadVersionsAsync() ?? package.Versions)?.FirstOrDefault();

                        if (targetVersion is null)
                        {
                            Log.Warning("Cannot perform upgrade because of 'Target version' is null");
                            return;
                        }


                        var updatePackageDetails = PackageDetailsFactory.Create(PackageOperationType.Update, package.GetMetadata(), targetVersion, null);
                        updatePackageList.Add(updatePackageDetails);
                    }

                    using (var operationContext = _packageOperationContextService.UseOperationContext(PackageOperationType.Update, updatePackageList.ToArray()))
                    {
                        foreach (var updatePackageDetails in updatePackageList)
                        {
                            await _packageCommandService.ExecuteUpdateAsync(updatePackageDetails, operationContext, cts.Token);
                        }
                    }
                }

                await Task.Delay(200);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error when updating package");
            }
            finally
            {
                _progressManager.HideBar(parentVM);

                sourcePage.StartLoadingTimerOrInvalidateData();
            }
        }
    }
}
