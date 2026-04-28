namespace Orc.NuGetExplorer.Tests.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Catel;
using Catel.Collections;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Orc.NuGetExplorer.ViewModels;
using Orc.NuGetExplorer.Windows;

[TestFixture]
internal class PageActionBarViewModelFacts
{
    public class TheCheckAllExecuteAsyncMethod
    {
        [TestCase]
        public async Task InvalidatesPackagesBatchUpdateCommandAsync()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            // Resolve Catel services
            var commandManager = serviceProvider.GetRequiredService<ICommandManager>();
            var messageService = serviceProvider.GetRequiredService<IMessageService>();

            commandManager.CreateCommandWithGesture(serviceProvider, typeof(Commands.Packages), nameof(Commands.Packages.BatchUpdate));

            // commandManager.RegisterAction(Commands.Packages.BatchUpdate, () => { });
            var testCommand = (ICompositeCommand)commandManager.GetCommand(Commands.Packages.BatchUpdate);

            var canExecuteRaised = false;

            testCommand.CanExecuteChanged += (sender, args) =>
            {
                canExecuteRaised = true;
            };

            Assert.That(testCommand.CanExecute(), Is.EqualTo(false));

            var progressManager = new Mock<IProgressManager>().Object;
            var packageCommandService = new Mock<IPackageCommandService>().Object;
            var packageOperationContextService = new Mock<IPackageOperationContextService>().Object;


            var vm = new PageActionBarViewModel(new TestPage(), progressManager, packageCommandService, 
                packageOperationContextService, messageService, commandManager, serviceProvider);
            await vm.InitializeViewModelAsync();

            var vmCommand = vm.CheckAll;
            vmCommand.Execute();
            await vmCommand.Task;

            Assert.That(canExecuteRaised, Is.EqualTo(true));
        }
    }

    public class TestPage : IManagerPage
    {
        public TestPage()
        {
            PackageItems = new System.Collections.ObjectModel.ObservableCollection<NuGetPackage>()
            {
                GlobalMocks.CreateMockPackage("1.0.0", "WildGums"),
            };
            CanBatchUpdateOperations = true;
            CanBatchInstallOperations = true;
        }

        public System.Collections.ObjectModel.ObservableCollection<NuGetPackage> PackageItems { get; }
        public bool CanBatchUpdateOperations { get; }
        public bool CanBatchInstallOperations { get; }

        public void StartLoadingTimerOrInvalidateData()
        {
            throw new NotSupportedException();
        }
    }
}
