namespace Orc.NuGetExplorer.Windows
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using Catel;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Reflection;
    using Catel.Services;

    /// <summary>
    /// Synchronous implementation of Catel IUIVisualizerService
    /// It used only for compatibility purposes
    /// to satisfy necessity of calling visualizer from implementations of NuGet Library's synchronous interfaces
    /// </summary>
    public class SynchronousUIVisualizerService : UIVisualizerService, ISynchronousUiVisualizer
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IViewLocator _viewLocator;

        public SynchronousUIVisualizerService(IViewLocator viewLocator) : base(viewLocator)
        {
            _viewLocator = viewLocator;
        }

        public virtual bool? ShowDialog(string name, object data, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            EnsureViewIsRegistered(name);

            var window = CreateWindow(name, data, completedProc, true);

            if (window != null)
            {
                //aware this place
                //awaiting on this method in async implementation causes hardly avoidable deadlock
                //if it called from synchronous code
                var task = ShowWindow(window, data, true);

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

        protected virtual Task<bool?> ShowWindow(FrameworkElement window, object data, bool showModal)
        {
            // Note: no async/await because we use a TaskCompletionSource
            var tcs = new TaskCompletionSource<bool?>();

            HandleCloseSubscription(window, data, (sender, args) => tcs.TrySetResult(args.Result), showModal);

            var showMethodInfo = showModal ? window.GetType().GetMethodEx("ShowDialog") : window.GetType().GetMethodEx("Show");

            if (showModal && showMethodInfo is null)
            {
                Log.Warning("Method 'ShowDialog' not found on '{0}', falling back to 'Show'", window.GetType().Name);

                showMethodInfo = window.GetType().GetMethodEx("Show");
            }

            if (showMethodInfo is null)
            {
                var exception = Log.ErrorAndCreateException<NotSupportedException>($"Methods 'Show' or 'ShowDialog' not found on '{window.GetType().Name}', cannot show the window");
                tcs.SetException(exception);
            }
            else
            {
                // ORCOMP-337: Always invoke with priority Input.
                window.Dispatcher.Invoke(() =>
                {
                    // Safety net to prevent crashes when this is the main window
                    try
                    {
                        showMethodInfo.Invoke(window, null);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"An error occurred while showing window '{window.GetType().GetSafeFullName(true)}'");
                        tcs.TrySetResult(null);
                    }
                }, DispatcherPriority.Input);
            }

            return tcs.Task;
        }
    }
}
