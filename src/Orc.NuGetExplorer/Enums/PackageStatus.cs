namespace Orc.NuGetExplorer.Enums;

public enum PackageStatus
{
    NotInstalled = -2,
    UpdateAvailable = -1,
    LastVersionInstalled = 0,
    Pending = 1
}