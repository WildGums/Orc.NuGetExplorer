using Orc.NuGetExplorer.Services;
using Orc.NuGetExplorer.Windows.Service;

namespace Orc.NuGetExplorer.Windows
{
    using Catel;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Services;
    using NuGetExplorer.Services;
    using NuGetExplorer.Windows.Service;
    using System.Threading.Tasks;

    public class MessageDialogService : IMessageDialogService
    {
        private readonly IUIVisualizerService _uIVisualizerService;

        private readonly ITypeFactory _typeFactory;

        private readonly ISynchronousUiVisualizer _syncUiVisualizer;

        static MessageDialogService()
        {
            var vmLocator = ServiceLocator.Default.ResolveType<IViewModelLocator>();

            vmLocator.Register<DialogHost, DialogHostViewModel>();
        }

        public MessageDialogService(IUIVisualizerService uIVisualizerService, ITypeFactory typeFactory, IMessageService messageService,
            ISynchronousUiVisualizer synchronousUiVisualizer)
        {
            Argument.IsNotNull(() => uIVisualizerService);
            Argument.IsNotNull(() => typeFactory);
            Argument.IsNotNull(() => messageService);
            Argument.IsNotNull(() => synchronousUiVisualizer);

            _uIVisualizerService = uIVisualizerService;
            _typeFactory = typeFactory;

            _syncUiVisualizer = synchronousUiVisualizer;
        }

        public async Task<T> ShowDialogAsync<T>(string title, string message, bool addCloseButton, params IDialogOption[] options)
        {
            var result = new DialogResult<T>();

            var vm = CreateDialogViewModel(result, title, message, addCloseButton, options);

            await _uIVisualizerService.ShowDialogAsync(vm);

            return result.Result;
        }

        public T ShowDialog<T>(string title, string message, bool addCloseButton, params IDialogOption[] options)
        {
            var result = new DialogResult<T>();

            var vm = CreateDialogViewModel(result, title, message, addCloseButton, options);

            _syncUiVisualizer.ShowDialog(vm);

            return result.Result;
        }

        private IViewModel CreateDialogViewModel(DialogResult result, string title, string message, bool addCloseButton, IDialogOption[] options)
        {
            var customize = new DialogCustomization(options, addCloseButton);

            return _typeFactory.CreateInstanceWithParametersAndAutoCompletion<DialogHostViewModel>(customize, result, title, message);
        }
    }
}
