namespace Orc.NuGetExplorer;

using System.Collections.ObjectModel;
using Catel.Collections;

public class PackagesBatch
{
    public PackagesBatch()
    {
        PackageList = new System.Collections.ObjectModel.ObservableCollection<IPackageDetails>();
    }

    public System.Collections.ObjectModel.ObservableCollection<IPackageDetails> PackageList { get; set; }
    public PackageOperationType OperationType { get; set; }
}
