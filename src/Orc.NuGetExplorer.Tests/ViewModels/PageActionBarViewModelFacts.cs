namespace Orc.NuGetExplorer.Tests.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Collections;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Services;
    using Moq;
    using NUnit.Framework;
    using Orc.NuGetExplorer.Models;
    using Orc.NuGetExplorer.Tests.TestCases;
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
#pragma warning disable IDISP001 // Dispose created
                var serviceLocator = new ServiceLocator(ServiceLocator.Default);
#pragma warning restore IDISP001 // Dispose created

                // Resolve Catel services
                var commandManager = serviceLocator.ResolveType<ICommandManager>();
                var messageService = serviceLocator.ResolveType<IMessageService>();

                commandManager.CreateCommandWithGesture(typeof(Commands.Packages), nameof(Commands.Packages.BatchUpdate));

                // commandManager.RegisterAction(Commands.Packages.BatchUpdate, () => { });
                var testCommand = (ICompositeCommand)commandManager.GetCommand(Commands.Packages.BatchUpdate);

                var canExecuteRaised = false;

                testCommand.CanExecuteChanged += (sender, args) =>
                {
                    canExecuteRaised = true;
                };

                Assert.AreEqual(false, testCommand.CanExecute());

                var progressManager = new Mock<IProgressManager>().Object;
                var packageCommandService = new Mock<IPackageCommandService>().Object;
                var packageOperationContextService = new Mock<IPackageOperationContextService>().Object;


                var vm = new PageActionBarViewModel(new TestPage(), progressManager, packageCommandService, packageOperationContextService, messageService, commandManager);
                await vm.InitializeViewModelAsync();

                var vmCommand = vm.CheckAll;
                vmCommand.Execute();
                await vmCommand.Task;

                Assert.AreEqual(true, canExecuteRaised);
            }
        }

        public class TestPage : IManagerPage
        {
            public TestPage()
            {
                PackageItems = new FastObservableCollection<NuGetPackage>()
                {
                    FixtureNuGetPackageFactory.CreateFixturePackage("1.0.0", "WildGums"),
                };
                CanBatchUpdateOperations = true;
                CanBatchInstallOperations = true;
            }

            public FastObservableCollection<NuGetPackage> PackageItems { get; }
            public bool CanBatchUpdateOperations { get; }
            public bool CanBatchInstallOperations { get; }

            public void StartLoadingTimerOrInvalidateData()
            {
                throw new NotSupportedException();
            }
        }
    }
}
