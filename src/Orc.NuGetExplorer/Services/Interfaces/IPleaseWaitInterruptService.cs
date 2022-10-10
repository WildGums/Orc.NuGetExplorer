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