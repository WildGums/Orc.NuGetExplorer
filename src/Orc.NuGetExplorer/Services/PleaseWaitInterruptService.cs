// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PleaseWaitInterruptService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;

    internal class PleaseWaitInterruptService : IPleaseWaitInterruptService
    {
        #region Fields
        #endregion

        #region Constructors
        public PleaseWaitInterruptService()
        {
        }
        #endregion

        #region Methods
        public IDisposable InterruptTemporarily()
        {
            return new DisposableToken(this, x => { }, x => { });
        }
        #endregion
    }
}