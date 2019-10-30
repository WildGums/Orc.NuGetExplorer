namespace Orc.NuGetExplorer.Windows
{
    using System;
    using Catel.MVVM;
    using Catel.Services;

    public interface ISynchronousUiVisualizer
    {
        bool? ShowDialog(IViewModel viewModel, EventHandler<UICompletedEventArgs> completedProc = null);
    }
}
