namespace Orc.NuGetExplorer
{
    using System;
    using System.Threading;

    public class SynchronizationContextScopeManager
    {
        public static IDisposable OutOfContext()
        {
            var token = new SynchronizationDisabilityToken(SynchronizationContext.Current);

            SynchronizationContext.SetSynchronizationContext(null);

            return token;
        }

        struct SynchronizationDisabilityToken : IDisposable
        {
            private readonly SynchronizationContext _synchContext;

            public SynchronizationDisabilityToken(SynchronizationContext currentContext)
            {
                _synchContext = currentContext;
            }

            public void Dispose()
            {
                //return context;
                SynchronizationContext.SetSynchronizationContext(_synchContext);
            }
        }
    }
}
