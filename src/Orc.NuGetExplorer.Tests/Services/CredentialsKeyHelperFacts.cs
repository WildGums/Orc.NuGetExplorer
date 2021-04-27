namespace Orc.NuGetExplorer.Tests.Services
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using NuGet.Configuration;
    using NuGet.Credentials;
    using NUnit.Framework;

    [TestFixture]
    public class CredentialsKeyHelperFacts
    {
        [TestCase("https://api.nuget.org/v3/index.json", "https://api.nuget.org/v3/")]
        [TestCase("https://extensions.wildgums.com/sub-url/nuget/index.json", "https://extensions.wildgums.com/sub-url/nuget/")]
        [TestCase("https://extensions.wildgums.com/another-sub-url/nuget/index.json", "https://extensions.wildgums.com/another-sub-url/nuget/")]
        public void ReturnsCorrectCacheKey(string url, string expectedUrlPart)
        {
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);

            var actual = ExplorerCredentialService.CredentialsKeyHelper.GetCacheKey(uri, NuGet.Configuration.CredentialRequestType.Forbidden, new DummyCredentialsProvider());
            var expected = $"dummy_{false}_{expectedUrlPart}";

            Assert.AreEqual(expected, actual);
        }
    }

    public class DummyCredentialsProvider : ICredentialProvider
    {
        public string Id => "dummy";

        public Task<CredentialResponse> GetAsync(Uri uri, IWebProxy proxy, CredentialRequestType type, string message, bool isRetry, bool nonInteractive, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
