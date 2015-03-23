// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemporaryFIleSystemContextService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;

    public class TemporaryFIleSystemContextService : ITemporaryFIleSystemContextService
    {
        #region Properties
        public ITemporaryFileSystemContext Context { get; private set; }
        #endregion

        #region Methods
        public IDisposable UseTemporaryFIleSystemContext()
        {
            return new DisposableToken<ITemporaryFileSystemContext>(new TemporaryFileSystemContext(), token => { }, token => { });
        }
        #endregion
    }
}