// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITemporaryFIleSystemContextService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer
{
    using System;

    internal interface ITemporaryFIleSystemContextService
    {
        #region Properties
        ITemporaryFileSystemContext Context { get; }
        #endregion

        #region Methods
        IDisposable UseTemporaryFIleSystemContext();
        #endregion
    }
}