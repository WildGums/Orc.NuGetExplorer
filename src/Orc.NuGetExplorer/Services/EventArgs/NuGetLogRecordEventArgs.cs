namespace Orc.NuGetExplorer
{
    using System;
    using Catel;

    public class NuGetLogRecordEventArgs : EventArgs
    {
        public NuGetLogRecordEventArgs(string message)
        {
            Argument.IsNotNullOrEmpty(() => message);

            Message = message;
        }

        public string Message { get; private set; }
    }
}
