namespace Orc.NuGetExplorer.Windows
{
    using System;
    using Catel;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;

    /// <summary>
    /// Synchronous implementation of Catel IUIVisualizerService
    /// It used only for compatibility purposes
    /// to satisfy necessity of calling visualizer from implementations of NuGet Library's synchronous interfaces
    /// </summary>
    internal class SynchronousUIVisualizerService : UIVisualizerService, ISynchronousUiVisualizer
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public SynchronousUIVisualizerService(IViewLocator viewLocator, IDispatcherService dispatcherService) 
            : base(viewLocator, dispatcherService)
        {
        }

        public virtual bool? ShowDialog(string name, object data, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            EnsureViewIsRegistered(name);

            var windowTask = CreateWindowAsync(name, data, completedProc, true);
            var window = windowTask.Result;
            if (window is not null)
            {
                //aware this place
                //awaiting on this method in async implementation causes hardly avoidable deadlock
                //if it called from synchronous code
                var task = ShowWindowAsync(window, data, true);

                task.Wait();

                return task.Result;

            }

            return false;
        }

        public virtual bool? ShowDialog(IViewModel viewModel, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            Argument.IsNotNull("viewModel", viewModel);

            var viewModelType = viewModel.GetType();
            var viewModelTypeName = viewModelType.FullName;

            RegisterViewForViewModelIfRequired(viewModelType);

            return ShowDialog(viewModelTypeName, viewModel, completedProc);
        }
    }
}
