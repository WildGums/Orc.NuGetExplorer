namespace Orc.NuGetExplorer.Example
{
    using Models;

    public class EchoService : IEchoService
    {
        #region Fields
        private PackageManagementEcho _echo;
        #endregion

        #region Methods
        public PackageManagementEcho GetPackageManagementEcho()
        {
            if (_echo is null)
            {
                _echo = new PackageManagementEcho();
            }

            return _echo;
        }
        #endregion
    }
}
