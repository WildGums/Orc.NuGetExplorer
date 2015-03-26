// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetLogRecordEventArgs.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


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