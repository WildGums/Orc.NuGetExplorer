namespace Orc.NuGetExplorer;

using System;
using System.Threading;
using Catel.Logging;
using Microsoft.Extensions.Logging;

public class SynchronizationContextScopeManager
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(SynchronizationContextScopeManager));

    public static IDisposable OutOfContext()
    {
        if (SynchronizationContext.Current is null)
        {
            throw Logger.LogErrorAndCreateException<InvalidOperationException>("Invalid synchronization context");
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
