// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INuGetLogListeningSevice.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using NuGet;

    public interface INuGetLogListeningSevice
    {
        void SendInfo(string message);
        void SendWarning(string message);
        void SendDebug(string message);
        void SendError(string message);

        event EventHandler<NuGetLogRecordEventArgs> Info;
        event EventHandler<NuGetLogRecordEventArgs> Warning;
        event EventHandler<NuGetLogRecordEventArgs> Debug;
        event EventHandler<NuGetLogRecordEventArgs> Error;
    }


}