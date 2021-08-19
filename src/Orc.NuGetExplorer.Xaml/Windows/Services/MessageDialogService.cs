namespace Orc.NuGetExplorer.Windows
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;
    using Catel.Services;

    internal class MessageDialogService : IMessageDialogService
    {
        private readonly IUIVisualizerService _uIVisualizerService;
        private readonly ITypeFactory _typeFactory;
        private readonly ISynchronousUiVisualizer _syncUiVisualizer;

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
            throw new NotImplementedException();
        }

        public T ShowDialog<T>(string title, string message, bool addCloseButton, params IDialogOption[] options)
        {
            throw new NotImplementedException();
        }
    }
}
