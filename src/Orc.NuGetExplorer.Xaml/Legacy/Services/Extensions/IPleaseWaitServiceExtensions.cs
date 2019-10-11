// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPleaseWaitServiceExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer
{
    using System;
    using Catel;
    using Catel.Services;

    public static class IPleaseWaitServiceExtensions
    {
        #region Methods
        public static IDisposable WaitingScope(this IPleaseWaitService pleaseWaitService)
        {
            return new DisposableToken<IPleaseWaitService>(pleaseWaitService, token => token.Instance.Push(), token => token.Instance.Pop());
        }
        #endregion
    }
}