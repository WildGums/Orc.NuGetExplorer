namespace Orc.NuGetExplorer;

using System.Threading.Tasks;

public interface INuGetConfigurationResetService
{
    Task ResetAsync();
}