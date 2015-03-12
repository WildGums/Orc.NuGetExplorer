// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetLogListeningSevice.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;

    internal class NuGetLogListeningSevice : INuGetLogListeningSevice
    {
        #region Methods
        public void SendInfo(string message)
        {
            Info.SafeInvoke(this, new NuGetLogRecordEventArgs(message));
        }

        public void SendWarning(string message)
        {
            Warning.SafeInvoke(this, new NuGetLogRecordEventArgs(message));
        }

        public void SendDebug(string message)
        {
            Debug.SafeInvoke(this, new NuGetLogRecordEventArgs(message));
        }

        public void SendError(string message)
        {
            Error.SafeInvoke(this, new NuGetLogRecordEventArgs(message));
        }

        public event EventHandler<NuGetLogRecordEventArgs> Info;
        public event EventHandler<NuGetLogRecordEventArgs> Warning;
        public event EventHandler<NuGetLogRecordEventArgs> Debug;
        public event EventHandler<NuGetLogRecordEventArgs> Error;
        #endregion
    }
}