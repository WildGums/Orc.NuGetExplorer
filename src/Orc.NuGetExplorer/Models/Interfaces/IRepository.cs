namespace Orc.NuGetExplorer;

public interface IRepository
{
    string Name { get; }
    string Source { get; }
    PackageOperationType OperationType { get; }

    bool IsLocal { get; }
}