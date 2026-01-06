namespace Orc.NuGetExplorer.Tests;

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework;
using Orc.NuGetExplorer.Services;
using PublicApiGenerator;
using VerifyNUnit;

[TestFixture]
public class PublicApiFacts
{
    [Test, MethodImpl(MethodImplOptions.NoInlining)]
    public async Task Orc_NuGetExplorer_HasNoBreakingChanges_Async()
    {
        var assembly = typeof(DeferredPackageLoaderService).Assembly;

        await PublicApiApprover.ApprovePublicApiAsync(assembly);
    }

    [Test, MethodImpl(MethodImplOptions.NoInlining)]
    public async Task Orc_NuGetExplorer_Xaml_HasNoBreakingChanges_Async()
    {
        var assembly = typeof(XamlBusyIndicatorInterruptService).Assembly;

        await PublicApiApprover.ApprovePublicApiAsync(assembly);
    }
    internal static class PublicApiApprover
    {
        public static async Task ApprovePublicApiAsync(Assembly assembly)
        {
            var publicApi = ApiGenerator.GeneratePublicApi(assembly, new ApiGeneratorOptions());
            await Verifier.Verify(publicApi);
        }
    }
}
