namespace Orc.NuGetExplorer
{
    using Catel.Logging;

    public class NuGetToCatelLogTranslator : PackageManagerLogListenerBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public NuGetToCatelLogTranslator(INuGetLogListeningSevice nuGetLogListeningSevice)
            : base(nuGetLogListeningSevice)
        {
        }

        protected override void OnInfo(object? sender, NuGetLogRecordEventArgs e)
        {
            Log.Info(e.Message);
        }

        protected override void OnDebug(object? sender, NuGetLogRecordEventArgs e)
        {
            Log.Debug(e.Message);
        }

        protected override void OnWarning(object? sender, NuGetLogRecordEventArgs e)
        {
            Log.Warning(e.Message);
        }

        protected override void OnError(object? sender, NuGetLogRecordEventArgs e)
        {
            Log.Error(e.Message);
        }
    }
}
