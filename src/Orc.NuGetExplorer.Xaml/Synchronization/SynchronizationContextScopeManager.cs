namespace Orc.NuGetExplorer
{
    using System;
    using System.Threading;
    using Catel.Logging;

    public class SynchronizationContextScopeManager
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static IDisposable OutOfContext()
        {
            if (SynchronizationContext.Current is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Invalid synchronization context");
            }
            var token = new SynchronizationDisabilityToken(SynchronizationContext.Current);

            SynchronizationContext.SetSynchronizationContext(null);

            return token;
        }

        private struct SynchronizationDisabilityToken : IDisposable
        {
            private readonly SynchronizationContext _synchContext;

            public SynchronizationDisabilityToken(SynchronizationContext currentContext)
            {
                _synchContext = currentContext;
            }

            public void Dispose()
            {
                // Restore context;
                SynchronizationContext.SetSynchronizationContext(_synchContext);
            }
        }
    }
}
