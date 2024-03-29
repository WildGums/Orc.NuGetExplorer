﻿namespace Orc.NuGetExplorer;

using System;

public class OperationContextEventArgs : EventArgs
{
    public OperationContextEventArgs(IPackageOperationContext packageOperationContext)
    {
        PackageOperationContext = packageOperationContext;
    }

    public IPackageOperationContext PackageOperationContext { get; private set; }
}