namespace Orc.NuGetExplorer;

using System;
using Enums;

public class PackageModelStatusEventArgs : EventArgs
{
    public PackageModelStatusEventArgs(PackageStatus oldStatus, PackageStatus newStatus)
    {
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }

    public PackageStatus OldStatus { get; }

    public PackageStatus NewStatus { get; }
}
