// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITemporaryFileSystemContext.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;

    public interface ITemporaryFileSystemContext : IDisposable
    {
        #region Properties
        string RootDirectory { get; }
        #endregion

        #region Methods
        string GetDirectory(string relativeDirectoryName);
        string GetFile(string relativeFilePath);
        #endregion
    }
}