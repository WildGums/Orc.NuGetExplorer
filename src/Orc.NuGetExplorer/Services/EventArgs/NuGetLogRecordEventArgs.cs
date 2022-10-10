namespace Orc.NuGetExplorer
{
    using System;
    using Catel;

    public class NuGetLogRecordEventArgs : EventArgs
    {
        #region Constructors
        public NuGetLogRecordEventArgs(string message)
        {
            Argument.IsNotNullOrEmpty(() => message);

            Message = message;
        }
        #endregion

        #region Properties
        public string Message { get; private set; }
        #endregion
    }
}