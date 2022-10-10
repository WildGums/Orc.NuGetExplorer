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