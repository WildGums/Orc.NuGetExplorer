namespace Orc.NuGetExplorer.Tests.ViewModels;

using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using Catel;
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
        [Apartment(ApartmentState.STA)]
        public async Task Checkbox_Selects_And_Deselects_All_Packages_Async()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();
            using var serviceProvider = serviceCollection.BuildServiceProvider();
            var commandManager = serviceProvider.GetRequiredService<ICommandManager>();
            commandManager.CreateCommandWithGesture(serviceProvider, typeof(Commands.Packages), nameof(Commands.Packages.BatchUpdate));

            var page = new TestPage();
            page.PackageItems.Add(GlobalMocks.CreateMockPackage("2.0.0", "Catel"));
            page.PackageItems.Add(GlobalMocks.CreateMockPackage("3.0.0", "Orchestra"));
            var vm = new PageActionBarViewModel(page, new Mock<IProgressManager>().Object,
                new Mock<IPackageCommandService>().Object, new Mock<IPackageOperationContextService>().Object,
                serviceProvider.GetRequiredService<IMessageService>(), commandManager,
                serviceProvider.GetRequiredService<ILanguageService>(), serviceProvider);
            await vm.InitializeViewModelAsync();
            var checkbox = new CheckBox 
            { 
                DataContext = vm, 
                Command = vm.CheckAll
            };

            checkbox.SetBinding(CheckBox.IsCheckedProperty, new Binding(nameof(vm.IsCheckAll)));
            var onClick = typeof(CheckBox).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic);

            onClick.Invoke(checkbox, null);

            await vm.CheckAll.Task;

            Assert.That(page.PackageItems.All(package => package.IsChecked), Is.True);

            page.PackageItems.Add(GlobalMocks.CreateMockPackage("4.0.0", "NextPage"));

            Assert.That(page.PackageItems.All(package => package.IsChecked), Is.True);

            onClick.Invoke(checkbox, null);

            await vm.CheckAll.Task;

            Assert.That(page.PackageItems.All(package => !package.IsChecked), Is.True);

            page.PackageItems.Add(GlobalMocks.CreateMockPackage("5.0.0", "UnselectedNextPage"));

            Assert.That(page.PackageItems.All(package => !package.IsChecked), Is.True);
            await vm.CloseViewModelAsync(false);
        }

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
                packageOperationContextService, messageService, commandManager,
                serviceProvider.GetRequiredService<ILanguageService>(), serviceProvider);
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
