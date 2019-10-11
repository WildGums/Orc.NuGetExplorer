namespace Orc.NuGetExplorer.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface INuGetFeedVerificationService
    {
        Task<FeedVerificationResult> VerifyFeedAsync(string source, CancellationToken ct, bool authenticateIfRequired = true);

        //[ObsoleteEx]
        FeedVerificationResult VerifyFeed(string source, bool authenticateIfRequired = true);
    }
}
