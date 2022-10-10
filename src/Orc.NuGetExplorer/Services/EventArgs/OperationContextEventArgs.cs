﻿namespace Orc.NuGetExplorer
{
    using System;
    using Catel;

    public class OperationContextEventArgs : EventArgs
    {
        #region Constructors
        public OperationContextEventArgs(IPackageOperationContext packageOperationContext)
        {
            Argument.IsNotNull(() => packageOperationContext);

            PackageOperationContext = packageOperationContext;
        }
        #endregion

        #region Properties
        public IPackageOperationContext PackageOperationContext { get; private set; }
        #endregion
    }
}