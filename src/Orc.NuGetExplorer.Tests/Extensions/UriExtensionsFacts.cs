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
                "file:///C:/Users/test/AppData/Roaming/Orc.NuGetExplorer example/plugins/Test.1.0.0-alpha0/Test.1.0.0-alpha0.nupkg#packageIcon.png",
                "C:\\Users\\test\\AppData\\Roaming\\Orc.NuGetExplorer example\\plugins\\Test.1.0.0-alpha0\\packageIcon.png")]
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
