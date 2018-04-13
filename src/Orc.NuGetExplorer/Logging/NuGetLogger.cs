// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetLogger.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;
    using NuGet;

    internal class NuGetLogger : ILogger
    {
        #region Fields
        private readonly INuGetLogListeningSevice _logListeningService;
        #endregion

        #region Constructors
        public NuGetLogger(INuGetLogListeningSevice logListeningService)
        {
            Argument.IsNotNull(() => logListeningService);

            _logListeningService = logListeningService;
        }
        #endregion

        #region Methods
        public FileConflictResolution ResolveFileConflict(string message)
        {
            return FileConflictResolution.IgnoreAll;
        }

        public void Log(MessageLevel level, string message, params object[] args)
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
        }
        #endregion
    }
}