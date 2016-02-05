// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationContextEventArgs.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
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