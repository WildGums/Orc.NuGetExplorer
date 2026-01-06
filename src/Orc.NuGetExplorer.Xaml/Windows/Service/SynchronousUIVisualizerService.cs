namespace Orc.NuGetExplorer.Windows;

using System;
using Catel.Logging;
using Catel.MVVM;
using Catel.MVVM.Views;
using Catel.Services;
using Microsoft.Extensions.Logging;

/// <summary>
/// Synchronous implementation of Catel IUIVisualizerService
/// It used only for compatibility purposes
/// to satisfy necessity of calling visualizer from implementations of NuGet Library's synchronous interfaces
/// </summary>
internal class SynchronousUIVisualizerService : UIVisualizerService, ISynchronousUiVisualizer
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(SynchronousUIVisualizerService));

    public SynchronousUIVisualizerService(ILogger<UIVisualizerService> logger, 
        IViewFactory viewFactory, IViewLocator viewLocator, IDispatcherService dispatcherService,
        IViewModelFactory viewModelFactory)
        : base(logger, viewLocator, viewFactory, dispatcherService, viewModelFactory)
    {
    }

    public virtual bool? ShowDialog(string name, object data, EventHandler<UICompletedEventArgs>? completedProc = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw Logger.LogErrorAndCreateException<ArgumentException>($"'{nameof(name)}' parameter is incorrect");
        }

        EnsureViewIsRegistered(name);

        var context = new UIVisualizerContext
        {
            Name = name,
            Data = data,
            CompletedCallback = completedProc,
            IsModal = true
        };

        var windowTask = CreateWindowAsync(context);
        var window = windowTask.Result;
        if (window is not null)
        {
            // aware this place
            // awaiting on this method in async implementation causes hardly avoidable deadlock
            // if it called from synchronous code
            var task = ShowWindowAsync(window, context);

            task.Wait();

            return task.Result.DialogResult;

        }

        return false;
    }

    public virtual bool? ShowDialog(IViewModel viewModel, EventHandler<UICompletedEventArgs>? completedProc = null)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        var viewModelType = viewModel.GetType();
        var viewModelTypeName = viewModelType.FullName;

        RegisterViewForViewModelIfRequired(viewModelType);

        return ShowDialog(viewModelTypeName ?? string.Empty, viewModel, completedProc);
    }
}
