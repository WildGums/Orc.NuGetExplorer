namespace Orc.NuGetExplorer
{
    using NuGet.Configuration;

    [ObsoleteEx(ReplacementTypeOrMember = "NuGet.Configuration.PackageSource/NuGet.Protocol.Core.Types.SourceRepository", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
    public interface IRepository
    {
        #region Properties
        string Name { get; }
        string Source { get; }
        PackageSource PackageSource { get; set; }
        bool IsLocal { get; }
        #endregion
    }
}
