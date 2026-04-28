namespace Orc.NuGetExplorer;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

public static class DispatchHelper
{
    private static readonly Dispatcher Dispatcher = Application.Current.Dispatcher;

    public static async Task DispatchIfNecessaryAsync(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (!Dispatcher.CheckAccess())
        {
            await Dispatcher.InvokeAsync(action);
        }
        else
        {
            action.Invoke();
        }
    }
}
