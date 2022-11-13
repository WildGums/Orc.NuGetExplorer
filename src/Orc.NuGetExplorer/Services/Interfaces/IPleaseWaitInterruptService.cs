namespace Orc.NuGetExplorer
{
    using System;

    public interface IPleaseWaitInterruptService
    {
        IDisposable InterruptTemporarily();
    }
}
