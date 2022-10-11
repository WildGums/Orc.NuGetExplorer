namespace Orc.NuGetExplorer
{
    using Catel;

    public abstract class PackageManagerLogListenerBase
    {
        protected PackageManagerLogListenerBase(INuGetLogListeningSevice nuGetLogListeningSevice)
        {
            Argument.IsNotNull(() => nuGetLogListeningSevice);

            nuGetLogListeningSevice.Error += OnError;
            nuGetLogListeningSevice.Info += OnInfo;
            nuGetLogListeningSevice.Debug += OnDebug;
            nuGetLogListeningSevice.Warning += OnWarning;
        }

        protected virtual void OnWarning(object? sender, NuGetLogRecordEventArgs e)
        {
        }

        protected virtual void OnDebug(object? sender, NuGetLogRecordEventArgs e)
        {
        }

        protected virtual void OnInfo(object? sender, NuGetLogRecordEventArgs e)
        {
        }

        protected virtual void OnError(object? sender, NuGetLogRecordEventArgs e)
        {
        }
    }
}
