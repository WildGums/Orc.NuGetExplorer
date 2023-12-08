namespace Orc.NuGetExplorer.Tests;

using NuGetExplorer;
using NUnit.Framework;

[TestFixture]
public class EncryptionHelperFacts
{
    [TestCase("sadf32ASDF43", "some password")]
    [TestCase("very long string with spaces", "another password")]
    public void EncryptsAndDecryptsCorrectly(string input, string password)
    {
        var encrypted = EncryptionHelper.Encrypt(input, password);

        Assert.That(encrypted, Is.Not.EqualTo(input));

        var decrypted = EncryptionHelper.Decrypt(encrypted, password);

        Assert.That(decrypted, Is.EqualTo(input));
    }
}
