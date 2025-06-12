namespace Orc.NuGetExplorer.Tests.Windows
{
    using Catel.Configuration;
    using Moq;
    using NUnit.Framework;

    public class CredentialsPrompterFacts
    {
        [TestFixture]
        public class The_Encryption
        {
            [Test]
            public void Can_Store_And_Restore_Credentials()
            {
                // Arrange
                var targetName = "https://www.wildgums.com/nugetexplorer-test";
                var userName = "TestUser";
                var password = "TestPassword";

                // Act
                var configurationServiceMock = new Mock<IConfigurationService>();
                configurationServiceMock.Setup(x => x.GetValue(ConfigurationContainer.Roaming, Settings.NuGet.CredentialStorage, It.IsAny<object?>()))
                    .Returns(() => CredentialStoragePolicy.WindowsVaultConfigurationFallback);

                var credentialsPrompter = new Orc.NuGetExplorer.Windows.CredentialsPrompter(configurationServiceMock.Object, targetName);

                credentialsPrompter.WriteCredential(targetName, userName, password);

                configurationServiceMock.Verify(x => x.SetValue(It.IsAny<ConfigurationContainer>(), It.IsAny<string>(), It.IsAny<string?>()), Times.Never);

                var readCredentials = credentialsPrompter.ReadCredential(targetName, false);

                Assert.That(readCredentials.UserName, Is.EqualTo(userName));
                Assert.That(readCredentials.Password, Is.EqualTo(password));

                configurationServiceMock.Verify(x => x.GetValue(It.IsAny<ConfigurationContainer>(), It.IsAny<string>(), It.IsAny<string?>()), Times.Never);
            }
        }
    }
}
