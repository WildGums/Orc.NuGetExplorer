namespace Orc.NuGetExplorer
{
    public interface IPackageSourceFactory
    {
        #region Methods
        IPackageSource CreatePackageSource(string source, string name, bool isEnabled, bool isOfficial);
        #endregion
    }
}
