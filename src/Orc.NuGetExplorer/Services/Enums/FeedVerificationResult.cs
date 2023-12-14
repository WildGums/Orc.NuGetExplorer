namespace Orc.NuGetExplorer;

public enum FeedVerificationResult
{
    Unknown,
    Valid,
    AuthenticationRequired,
    AuthorizationRequired,
    Invalid
}