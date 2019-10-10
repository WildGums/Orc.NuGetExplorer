// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INuGetLogListeningSevice.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer
{
    using System;

    public interface INuGetLogListeningSevice
    {
        #region Methods
        void SendInfo(string message);
        void SendWarning(string message);
        void SendDebug(string message);
        void SendError(string message);
        event EventHandler<NuGetLogRecordEventArgs> Info;
        event EventHandler<NuGetLogRecordEventArgs> Warning;
        event EventHandler<NuGetLogRecordEventArgs> Debug;
        event EventHandler<NuGetLogRecordEventArgs> Error;
        #endregion
    }
}