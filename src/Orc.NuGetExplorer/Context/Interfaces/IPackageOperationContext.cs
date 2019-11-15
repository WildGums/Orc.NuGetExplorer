// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageOperationContext.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;

    public interface IPackageOperationContext
    {
        ITemporaryFileSystemContext FileSystemContext { get; set; }
        IList<Exception> Exceptions { get; }
        IPackageOperationContext Parent { get; set; }
        IRepository Repository { get; set; }
    }
}
