namespace Orc.NuGetExplorer.Tests.Services
{
    using System;
    using Moq;
    using NuGet.Frameworks;

    public partial class UpdatePackagesLoaderServiceFacts
    {
        public static Mock<IDefaultNuGetFramework> CreateDefaultNuGetFrameworkMock()
        {
            var defaultNuGetFrameworkMock = new Mock<IDefaultNuGetFramework>();
            defaultNuGetFrameworkMock.Setup(x => x.GetHighest())
                .Returns(() =>
                {
                    return new[]
                    {
                            new NuGetFramework(".NETFramework", new Version(4, 7, 2)),
                            new NuGetFramework(".NETCoreApp", new Version(8, 0))
                    };
                });

            return defaultNuGetFrameworkMock;
        }

        public static Mock<IRepository> CreateLocalRepositoryMock()
        {
            var localRepositoryMock = new Mock<IRepository>();
            localRepositoryMock.Setup(x => x.Source)
                .Returns(() => string.Empty);
            localRepositoryMock.Setup(x => x.IsLocal)
                .Returns(() => true);

            return localRepositoryMock;
        }
    }
}
