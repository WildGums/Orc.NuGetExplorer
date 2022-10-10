namespace Orc.NuGetExplorer
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