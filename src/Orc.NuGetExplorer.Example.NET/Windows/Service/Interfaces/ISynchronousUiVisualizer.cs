namespace Orc.NuGetExplorer.Windows.Service
{
    using Catel.MVVM;
    using Catel.Services;
    using System;

    public interface ISynchronousUiVisualizer
    {
        bool? ShowDialog(IViewModel viewModel, EventHandler<UICompletedEventArgs> completedProc = null);
    }
}
