// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPleaseWaitInterruptService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
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