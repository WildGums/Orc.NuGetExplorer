namespace Orc.NuGetExplorer
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows.Threading;
    using Catel;

    public class SynchronizeInvoker : ISynchronizeInvoke
    {
        private readonly Dispatcher _dispatcher;

        public SynchronizeInvoker(Dispatcher dispatcher)
        {
            Argument.IsNotNull(() => dispatcher);
            _dispatcher = dispatcher;
        }

        public bool InvokeRequired => !_dispatcher.CheckAccess();

        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            return new DispatcherAsyncResult(
                _dispatcher.BeginInvoke(
                    method,
                    DispatcherPriority.Normal,
                    args));
        }

        public object EndInvoke(IAsyncResult result)
        {
            DispatcherAsyncResult dispatcherResult = result as DispatcherAsyncResult;
            dispatcherResult.Operation.Wait();
            return dispatcherResult.Operation.Result;
        }

        public object Invoke(Delegate method, object[] args)
        {
            return _dispatcher.Invoke(method, DispatcherPriority.Normal, args);
        }

        private class DispatcherAsyncResult : IAsyncResult
        {
            private readonly IAsyncResult _result;

            public DispatcherAsyncResult(DispatcherOperation operation)
            {
                Operation = operation;
                _result = operation.Task;
            }

            public DispatcherOperation Operation { get; }

            public bool IsCompleted => _result.IsCompleted;

            public WaitHandle AsyncWaitHandle => _result.AsyncWaitHandle;

            public object AsyncState => _result.AsyncState;

            public bool CompletedSynchronously => _result.CompletedSynchronously;
        }
    }
}
