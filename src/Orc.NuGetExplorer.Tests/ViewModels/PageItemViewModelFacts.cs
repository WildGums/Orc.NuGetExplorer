namespace Orc.NuGetExplorer.Tests.ViewModels;

using System.Threading.Tasks;
using Catel;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NuGet.Versioning;
using NUnit.Framework;
using Orc.NuGetExplorer.Providers;
using Orc.NuGetExplorer.ViewModels;

[TestFixture]
internal class PageItemViewModelFacts
{
    public class TheOnModelPropertyChangedMethod
    {
        [TestCase]
        public async Task InvalidatesPackagesBatchUpdateCommandAsync()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = serviceProvider.GetRequiredService<ICommandManager>();
            commandManager.CreateCommandWithGesture(serviceProvider, typeof(Commands.Packages), nameof(Commands.Packages.BatchUpdate));

            var testCommand = (ICompositeCommand)commandManager.GetCommand(Commands.Packages.BatchUpdate);

            var canExecuteRaised = false;

            testCommand.CanExecuteChanged += (sender, args) =>
            {
                canExecuteRaised = true;
            };

            Assert.That(testCommand.CanExecute(), Is.EqualTo(false));

            var settingsProviderMock = new Mock<IModelProvider<ExplorerSettingsContainer>>();
            settingsProviderMock.Setup(x => x.Model).Returns(new ExplorerSettingsContainer());
            var settingsProvider = settingsProviderMock.Object;

            var model = GlobalMocks.CreateMockPackage("1.0.0", "WildGums");
            model.InstalledVersion = new NuGetVersion(model.Version);

            var vm = new PageItemViewModel(model, settingsProvider, commandManager,
                serviceProvider.GetRequiredService<ILanguageService>(), serviceProvider);
            await vm.InitializeViewModelAsync();

            model.IsChecked = true;

            Assert.That(canExecuteRaised, Is.EqualTo(true));
        }
    }
}
