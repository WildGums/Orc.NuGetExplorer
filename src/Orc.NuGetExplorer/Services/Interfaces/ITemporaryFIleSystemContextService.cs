// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITemporaryFIleSystemContextService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;

    public interface ITemporaryFIleSystemContextService
    {
        #region Properties
        ITemporaryFileSystemContext Context { get; }
        #endregion

        #region Methods
        IDisposable UseTemporaryFIleSystemContext();
        #endregion
    }
}