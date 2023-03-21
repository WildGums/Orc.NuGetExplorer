namespace Orc.NuGetExplorer;

using System.Collections.ObjectModel;
using Catel.Collections;

public class PackagesBatch
{
    public PackagesBatch()
    {
        PackageList = new FastObservableCollection<IPackageDetails>();
    }

    public ObservableCollection<IPackageDetails> PackageList { get; set; }
    public PackageOperationType OperationType { get; set; }
}