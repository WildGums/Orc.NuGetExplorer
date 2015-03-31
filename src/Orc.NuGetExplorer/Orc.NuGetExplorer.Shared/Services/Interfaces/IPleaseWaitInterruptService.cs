// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPleaseWaitInterruptService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;

    public interface IPleaseWaitInterruptService
    {
        #region Methods
        IDisposable InterruptTemporarily();
        #endregion
    }
}