namespace Orc.NuGetExplorer
{
    public interface IRepository
    {
        #region Properties
        string Name { get; }
        string Source { get; }
        PackageOperationType OperationType { get; }

        bool IsLocal { get; }
        #endregion
    }
}
