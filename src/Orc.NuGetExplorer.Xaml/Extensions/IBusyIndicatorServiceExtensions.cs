namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using Catel.Services;

    public static class IBusyIndicatorServiceExtensions
    {
        public static IDisposable WaitingScope(this IBusyIndicatorService busyIndicatorService)
        {
            ArgumentNullException.ThrowIfNull(busyIndicatorService);

            return new DisposableToken<IBusyIndicatorService>(busyIndicatorService, token => token.Instance.Push(), token => token.Instance.Pop());
        }
    }
}
