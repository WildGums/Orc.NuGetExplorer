namespace Orc.NuGetExplorer
{
    using System.Threading.Tasks;
    using System.Windows.Documents;
    using NuGet;

    public interface IPackageDetailsService
    {
        Task<FlowDocument> PackageToFlowDocument(IPackage package);
    }
}