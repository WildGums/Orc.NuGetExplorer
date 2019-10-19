namespace Orc.NuGetExplorer.Loggers
{
    using Catel.Logging;
    using NuGet.Common;
    using System.Threading.Tasks;

    public class DebugLogger : ILogger
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private bool _verbose;

        public DebugLogger(bool verbose)
        {
            _verbose = verbose;
        }

        void ILogger.Log(LogLevel level, string data)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    LogDebug(data);
                    break;

                case LogLevel.Error:
                    LogError(data);
                    break;

                case LogLevel.Information:
                    LogInformation(data);
                    break;

                case LogLevel.Warning:
                    LogWarning(data);
                    break;

                case LogLevel.Minimal:
                    LogMinimal(data);
                    break;

                case LogLevel.Verbose:
                    LogVerbose(data);
                    break;
            }
        }

        void ILogger.Log(ILogMessage message)
        {
            ((ILogger)this).Log(message.Level, message.Message);
        }

        public async Task LogAsync(LogLevel level, string data)
        {
            var logginTask = Task.Run(() => ((ILogger)this).Log(level, data));
            await logginTask;
        }

        public async Task LogAsync(ILogMessage message)
        {
            await LogAsync(message.Level, message.Message);
        }

        public void LogDebug(string data)
        {
            Log.Debug(data);
        }

        public void LogError(string data)
        {
            Log.Error(data);
        }

        public void LogInformation(string data)
        {
            Log.Info(data);
        }

        public void LogInformationSummary(string data)
        {
            Log.Info(data);
        }

        public void LogMinimal(string data)
        {
            LogInformation(data);
        }

        public void LogVerbose(string data)
        {
            Log.Info(data);
        }

        public void LogWarning(string data)
        {
            Log.Warning(data);
        }
    }
}
