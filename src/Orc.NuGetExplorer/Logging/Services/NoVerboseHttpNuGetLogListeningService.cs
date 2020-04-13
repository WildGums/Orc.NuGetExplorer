namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Catel;

    public class NoVerboseHttpNuGetLogListeningService : INuGetLogListeningSevice
    {
        private static readonly string GetHttpRequestInfoPattern = "  GET https";
        private static readonly string SetHttpRequestInfoPattern = "  SET https";
        private static readonly string OkHttpRequestInfoPattern = "  OK https";
        private static readonly string NotFoundHttpRequestInfoPattern = "  NotFound https";
        private static readonly string CacheHttpRequestInfoPattern = "  CACHE https";

        #region Methods
        public void SendInfo(string message)
        {
            Argument.IsNotNullOrEmpty(() => message);

            if (message.StartsWith(GetHttpRequestInfoPattern) || message.StartsWith(SetHttpRequestInfoPattern)
               || message.StartsWith(OkHttpRequestInfoPattern) || message.StartsWith(NotFoundHttpRequestInfoPattern) || message.StartsWith(CacheHttpRequestInfoPattern))
            {
                SendDebug(message);
                return;
            }

            Info?.Invoke(this, new NuGetLogRecordEventArgs(message));
        }

        public void SendWarning(string message)
        {
            Argument.IsNotNullOrEmpty(() => message);

            Warning?.Invoke(this, new NuGetLogRecordEventArgs(message));
        }

        public void SendDebug(string message)
        {
            Argument.IsNotNullOrEmpty(() => message);

            Debug?.Invoke(this, new NuGetLogRecordEventArgs(message));
        }

        public void SendError(string message)
        {
            Argument.IsNotNullOrEmpty(() => message);

            Error?.Invoke(this, new NuGetLogRecordEventArgs(message));
        }

        public event EventHandler<NuGetLogRecordEventArgs> Info;
        public event EventHandler<NuGetLogRecordEventArgs> Warning;
        public event EventHandler<NuGetLogRecordEventArgs> Debug;
        public event EventHandler<NuGetLogRecordEventArgs> Error;
        #endregion
    }
}
