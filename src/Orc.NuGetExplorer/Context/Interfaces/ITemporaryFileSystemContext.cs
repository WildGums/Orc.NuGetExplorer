// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITemporaryFileSystemContext.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
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