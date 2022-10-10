namespace Orc.NuGetExplorer
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface INuGetFeedVerificationService
    {
        #region Methods
        Task<FeedVerificationResult> VerifyFeedAsync(string source, bool authenticateIfRequired = true, CancellationToken cancellationToken = default);
        FeedVerificationResult VerifyFeed(string source, bool authenticateIfRequired = true);
        #endregion
    }
}
