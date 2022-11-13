namespace Orc.NuGetExplorer
{
    using System.Windows.Documents;

    internal interface IPackageDetailsService
    {
        FlowDocument PackageToFlowDocument(IPackageDetails package);
    }
}
