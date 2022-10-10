namespace Orc.NuGetExplorer.Example
{
    using Models;

    public interface IEchoService
    {
        #region Methods
        PackageManagementEcho GetPackageManagementEcho();
        #endregion
    }
}