namespace Orc.NuGetExplorer;

using System;

public interface IBusyIndicatorInterruptService
{
    IDisposable InterruptTemporarily();
}
