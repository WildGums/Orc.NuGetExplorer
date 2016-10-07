// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageDetailsService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Windows.Documents;

    internal interface IPackageDetailsService
    {
        #region Methods
        FlowDocument PackageToFlowDocument(IPackageDetails package);
        #endregion
    }
}