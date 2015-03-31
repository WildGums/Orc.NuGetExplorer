// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageDetailsService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Threading.Tasks;
    using System.Windows.Documents;

    internal interface IPackageDetailsService
    {
        #region Methods
        Task<FlowDocument> PackageToFlowDocument(IPackageDetails package);
        #endregion
    }
}