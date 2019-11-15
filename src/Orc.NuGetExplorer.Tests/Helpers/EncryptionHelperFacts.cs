// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Md5HelperFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !NET40

namespace Orc.NuGetExplorer.Tests
{
    using NUnit.Framework;
    using NuGetExplorer;

    [TestFixture]
    public class EncryptionHelperFacts
    {
        [TestCase("sadf32ASDF43", "some password")]
        [TestCase("very long string with spaces", "another password")]
        public void EncryptsAndDecryptsCorrectly(string input, string password)
        {
            var encrypted = EncryptionHelper.Encrypt(input, password);

            Assert.AreNotEqual(input, encrypted);

            var decrypted = EncryptionHelper.Decrypt(encrypted, password);
            
            Assert.AreEqual(input, decrypted);
        }
    }
}

#endif