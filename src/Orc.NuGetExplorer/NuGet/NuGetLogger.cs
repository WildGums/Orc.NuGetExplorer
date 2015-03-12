// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetLogger.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer
{
    using Catel;
    using NuGet;

    internal class NuGetLogger : ILogger
    {
        private readonly INuGetLogListeningSevice _logListeningSevice;

        public NuGetLogger(INuGetLogListeningSevice logListeningSevice)
        {
            Argument.IsNotNull(() => logListeningSevice);

            _logListeningSevice = logListeningSevice;
        }

        public FileConflictResolution ResolveFileConflict(string message)
        {
            return FileConflictResolution.IgnoreAll;
        }

        public void Log(MessageLevel level, string message, params object[] args)
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
        }        
    }
}