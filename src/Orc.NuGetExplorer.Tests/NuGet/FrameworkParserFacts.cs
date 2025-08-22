namespace Orc.NuGetExplorer.Tests
{
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using VerifyNUnit;

    public class FrameworkParserFacts
    {
        [TestFixture]
        public class The_ToSpecificPlatform_Method
        {
            [Test, Explicit]
            public async Task Returns_Correct_Minimum_Version_For_Windows()
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return;
                }

                //var nuGetFramework = new NuGet.Frameworks.NuGetFramework("net", new System.Version(8, 0),
                //    "windows", new System.Version(10, 0));
                var nuGetFramework = new NuGet.Frameworks.NuGetFramework(".NETCoreApp", new System.Version(8,0));
                var result = FrameworkParser.ToSpecificPlatform(nuGetFramework);

                await Verifier.Verify(result);
            }

            [Test]
            public async Task Returns_Correct_Minimum_Version_For_Windows_Auto()
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return;
                }

                //var nuGetFramework = new NuGet.Frameworks.NuGetFramework("net", new System.Version(8, 0),
                //    "windows", new System.Version(10, 0));
                var nuGetFramework = new NuGet.Frameworks.NuGetFramework(".NETCoreApp", new System.Version(8, 0));
                var result = FrameworkParser.ToSpecificPlatform(nuGetFramework);

                Assert.That(result.PlatformVersion, Is.EqualTo(System.Environment.OSVersion.Version));
            }
        }
    }
}
