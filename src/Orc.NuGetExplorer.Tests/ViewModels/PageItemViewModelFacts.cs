namespace Orc.NuGetExplorer.Tests.ViewModels
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;
    using Catel.MVVM;
    using Moq;
    using NuGet.Versioning;
    using NUnit.Framework;
    using Orc.NuGetExplorer.Models;
    using Orc.NuGetExplorer.Providers;
    using Orc.NuGetExplorer.Tests.TestCases;
    using Orc.NuGetExplorer.ViewModels;

    [TestFixture]
    internal class PageItemViewModelFacts
    {
        public class TheOnModelPropertyChangedMethod
        {
            [TestCase]
            public async Task InvalidatesPackagesBatchUpdateCommandAsync()
            {
#pragma warning disable IDISP001 // Dispose created
                var serviceLocator = new ServiceLocator(ServiceLocator.Default);
#pragma warning restore IDISP001 // Dispose created

                var commandManager = serviceLocator.ResolveType<ICommandManager>();
                commandManager.CreateCommandWithGesture(typeof(Commands.Packages), nameof(Commands.Packages.BatchUpdate));

                var testCommand = (ICompositeCommand)commandManager.GetCommand(Commands.Packages.BatchUpdate);

                var canExecuteRaised = false;

                testCommand.CanExecuteChanged += (sender, args) =>
                {
                    canExecuteRaised = true;
                };

                Assert.AreEqual(false, testCommand.CanExecute());

                var settingsProviderMock = new Mock<IModelProvider<ExplorerSettingsContainer>>();
                settingsProviderMock.Setup(x => x.Model).Returns(new ExplorerSettingsContainer());
                var settingsProvider = settingsProviderMock.Object;

                var model = FixtureNuGetPackageFactory.CreateFixturePackage("1.0.0", "WildGums");
                model.InstalledVersion = new NuGetVersion(model.Version);

                var vm = new PageItemViewModel(model, settingsProvider, commandManager);
                await vm.InitializeViewModelAsync();

                model.IsChecked = true;

                Assert.AreEqual(true, canExecuteRaised);
            }
        }
    }
}
