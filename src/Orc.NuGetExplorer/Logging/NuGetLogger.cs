// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetLogger.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;
    using Catel.Services;
    using NuGet;

    internal class NuGetLogger : ILogger
    {
        #region Fields
        private readonly IDispatcherService _dispatcherService;
        private readonly INuGetLogListeningSevice _logListeningService;
        #endregion

        #region Constructors
        public NuGetLogger(INuGetLogListeningSevice logListeningService, IDispatcherService dispatcherService)
        {
            Argument.IsNotNull(() => logListeningService);
            Argument.IsNotNull(() => dispatcherService);

            _logListeningService = logListeningService;
            _dispatcherService = dispatcherService;
        }
        #endregion

        #region Methods
        public FileConflictResolution ResolveFileConflict(string message)
        {
            return FileConflictResolution.IgnoreAll;
        }

        public void Log(MessageLevel level, string message, params object[] args)
        {
            _dispatcherService.Invoke(() =>
            {
                switch (level)
                {
                    case MessageLevel.Debug:
                        _logListeningService.SendDebug(string.Format(message, args));
                        break;

                    case MessageLevel.Info:
                        _logListeningService.SendInfo(string.Format(message, args));
                        break;

                    case MessageLevel.Error:
                        _logListeningService.SendError(string.Format(message, args));
                        break;

                    case MessageLevel.Warning:
                        _logListeningService.SendWarning(string.Format(message, args));
                        break;
                }
            });
        }
        #endregion
    }
}