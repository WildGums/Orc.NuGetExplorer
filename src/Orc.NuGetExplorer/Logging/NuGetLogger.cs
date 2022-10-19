namespace Orc.NuGetExplorer.Loggers
{
    using System;
    using System.Threading.Tasks;
    using Catel.Logging;
    using NuGet.Common;

    public class NuGetLogger : ILogger
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly bool _verbose;
        private readonly INuGetLogListeningSevice _logListeningService;

        public NuGetLogger(bool verbose, INuGetLogListeningSevice logListeningService)
        {
            ArgumentNullException.ThrowIfNull(logListeningService);

            _logListeningService = logListeningService;
            _verbose = verbose;
        }

        public NuGetLogger(INuGetLogListeningSevice logListeningService) 
            : this(true, logListeningService)
        {

        }

        #region ILogger
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
            Log.Debug($"Send {message.Level} message to log listeners");
            ((ILogger)this).Log(message.Level, message.Message);
        }

        public async Task LogAsync(ILogMessage message)
        {
            Log.Debug($"Send {message.Level} message to log listeners");
            await LogAsync(message.Level, message.Message);
        }

        public async Task LogAsync(LogLevel level, string data)
        {
            var logginTask = Task.Run(() => ((ILogger)this).Log(level, data));
            await logginTask;
        }

        public void LogDebug(string data)
        {
            _logListeningService.SendDebug(data);
        }

        public void LogError(string data)
        {
            _logListeningService.SendError(data);
        }

        public void LogInformation(string data)
        {
            _logListeningService.SendInfo(data);
        }

        public void LogWarning(string data)
        {
            _logListeningService.SendWarning(data);
        }

        public void LogInformationSummary(string data)
        {
            _logListeningService.SendInfo(data);
        }

        public void LogMinimal(string data)
        {
            LogInformation(data);
        }

        public void LogVerbose(string data)
        {
            if (_verbose)
            {
                LogInformation(data);
            }
        }

        #endregion
    }
}
