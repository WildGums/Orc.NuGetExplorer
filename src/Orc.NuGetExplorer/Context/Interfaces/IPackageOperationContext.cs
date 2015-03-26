// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageOperationContext.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;

    public interface IPackageOperationContext
    {
        ITemporaryFileSystemContext FileSystemContext { get; set; }
        IList<Exception> CatchedExceptions { get; }
        IPackageOperationContext Parent { get; set; }
        IRepository Repository { get; set; }
    }
}