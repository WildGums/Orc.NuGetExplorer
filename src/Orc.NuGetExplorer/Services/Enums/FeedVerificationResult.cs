namespace Orc.NuGetExplorer
{
    using System.ComponentModel;

    public enum FeedVerificationResult
    {
        [Description("The feed isn't verified yet")]
        Unknown,
        [Description("The feed is verified and valid")]
        Valid,
        [Description("The feed requires authentication")]
        AuthenticationRequired,
        [Description("Access to feed denied")]
        AuthorizationRequired,
        [Description("The feed is verified but invalid")]
        Invalid
    }
}
