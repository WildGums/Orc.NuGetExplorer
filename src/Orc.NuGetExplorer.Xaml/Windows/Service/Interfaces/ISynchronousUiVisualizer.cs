namespace Orc.NuGetExplorer.Windows
{
    using System;
    using Catel.MVVM;
    using Catel.Services;

    internal interface ISynchronousUiVisualizer
    {
        bool? ShowDialog(IViewModel viewModel, EventHandler<UICompletedEventArgs>? completedProc = null);
    }
}
