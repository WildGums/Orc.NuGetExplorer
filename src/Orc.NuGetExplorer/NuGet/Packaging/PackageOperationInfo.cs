namespace Orc.NuGetExplorer.Packaging;

public class PackageOperationInfo
{
    public PackageOperationInfo(string operationPath, PackageOperationType operationType, IPackageDetails package)
    {
        OperationPath = operationPath;
        OperationType = operationType;
        Package = package;
    }

    public string OperationPath { get; }

    public PackageOperationType OperationType { get; }

    public IPackageDetails Package { get; }
}