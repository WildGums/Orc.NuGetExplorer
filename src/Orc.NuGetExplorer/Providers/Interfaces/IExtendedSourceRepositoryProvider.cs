namespace Orc.NuGetExplorer
{
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;

    public interface IExtendedSourceRepositoryProvider : ISourceRepositoryProvider
    {
        SourceRepository CreateLocalRepository(string source);
    }
}
