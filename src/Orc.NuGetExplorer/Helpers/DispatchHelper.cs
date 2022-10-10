namespace Orc.NuGetExplorer
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;

    public static class DispatchHelper
    {
        private static readonly Dispatcher Dispatcher = Application.Current.Dispatcher;

        public static async Task DispatchIfNecessaryAsync(Action action)
        {
            if (!Dispatcher.CheckAccess())
            {
                await Dispatcher.InvokeAsync(action);
            }
            else
            {
                action.Invoke();
            }
        }

#if NET40
       /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <returns>The task representing the action.</returns>
        private static Task InvokeAsync(this Dispatcher dispatcher, Action action)
        {
            var tcs = new TaskCompletionSource<bool>();

            var dispatcherOperation = dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }), null);

            dispatcherOperation.Completed += (sender, e) => tcs.SetResult(true);
            dispatcherOperation.Aborted += (sender, e) => tcs.SetCanceled();

            return tcs.Task;
        }
#endif
    }
}
