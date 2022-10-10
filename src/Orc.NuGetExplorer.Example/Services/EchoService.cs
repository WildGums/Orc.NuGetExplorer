namespace Orc.NuGetExplorer.Example
{
    using Models;

    public class EchoService : IEchoService
    {
        private PackageManagementEcho _echo;

        public PackageManagementEcho GetPackageManagementEcho()
        {
            _echo ??= new PackageManagementEcho();

            return _echo;
        }
    }
}
