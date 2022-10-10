namespace Orc.NuGetExplorer
{
    using System;
    using Catel;

    public class NoVerboseHttpNuGetLogListeningService : INuGetLogListeningSevice
    {
        public void SendInfo(string message)
        {
            Argument.IsNotNullOrEmpty(() => message);

            if (message.StartsWith(Constants.Log.GetHttpRequestInfoPattern) || message.StartsWith(Constants.Log.SetHttpRequestInfoPattern)
               || message.StartsWith(Constants.Log.OkHttpRequestInfoPattern) || message.StartsWith(Constants.Log.NotFoundHttpRequestInfoPattern) || message.StartsWith(Constants.Log.CacheHttpRequestInfoPattern))
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
    }
}
