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
        private readonly INuGetLogListeningSevice _logListeningSevice;
        #endregion

        #region Constructors
        public NuGetLogger(INuGetLogListeningSevice logListeningSevice, IDispatcherService dispatcherService)
        {
            Argument.IsNotNull(() => logListeningSevice);
            Argument.IsNotNull(() => dispatcherService);

            _logListeningSevice = logListeningSevice;
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
                        _logListeningSevice.SendDebug(string.Format(message, args));
                        break;
                    case MessageLevel.Info:
                        _logListeningSevice.SendInfo(string.Format(message, args));
                        break;
                    case MessageLevel.Error:
                        _logListeningSevice.SendError(string.Format(message, args));
                        break;
                    case MessageLevel.Warning:
                        _logListeningSevice.SendWarning(string.Format(message, args));
                        break;
                }
            });
        }
        #endregion
    }
}