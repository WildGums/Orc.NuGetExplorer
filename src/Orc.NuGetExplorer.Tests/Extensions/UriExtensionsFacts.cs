namespace Orc.NuGetExplorer.Tests
{
    using System;
    using NUnit.Framework;
    using Orc.NuGetExplorer;

    [TestFixture]
    public class UriExtensionsFacts
    {
        public class The_DecodeLocalUri_Method
        {
            [TestCase(
                "file:///C:/Users/test/AppData/Roaming/WildGums/Orc.NuGetExplorer example/plugins/Newtonsoft.Json.13.0.2-beta2/Newtonsoft.Json.13.0.2-beta2.nupkg#packageIcon.png",
                "C:\\Users\\test\\AppData\\Roaming\\WildGums\\Orc.NuGetExplorer example\\plugins\\Newtonsoft.Json.13.0.2-beta2\\packageIcon.png")]
            public void CanHandleAbsoluteUriWithWhitespaces(string absoluteUri, string expectedPath)
            {
                var uri = new Uri(absoluteUri);
                var parsedUri = uri.GetLocalUriForFragment();

                var actualPath = parsedUri.LocalPath;

                Assert.That(actualPath, Is.EqualTo(expectedPath));
            }
        }
    }
}
