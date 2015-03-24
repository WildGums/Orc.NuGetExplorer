// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationContextEventArgs.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;

    public class OperationContextEventArgs : EventArgs
    {
        #region Constructors
        public OperationContextEventArgs(PackageOperationContext packageOperationContext)
        {
            PackageOperationContext = packageOperationContext;
        }
        #endregion

        #region Properties
        public PackageOperationContext PackageOperationContext { get; private set; }
        #endregion
    }
}