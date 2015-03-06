namespace Orc.NuGetExplorer
{
    public interface IPackageSource
    {
        bool IsEnabled { get; set; }
        bool IsMachineWide { get; set; }
        bool IsOfficial { get; set; }
        bool IsPasswordClearText { get; set; }
        string Name { get; }
        string Password { get; set; }
        string Source { get; }
        string UserName { get; set; }
    }
}