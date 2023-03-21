namespace Orc.NuGetExplorer.Example;

public class EchoService : IEchoService
{
    private PackageManagementEcho _echo;

    public PackageManagementEcho GetPackageManagementEcho()
    {
        _echo ??= new PackageManagementEcho();

        return _echo;
    }
}
