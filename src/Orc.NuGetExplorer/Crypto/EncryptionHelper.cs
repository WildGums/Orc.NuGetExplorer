// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MD5Helper.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.NuGetExplorer.Crypto
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    internal static class EncryptionHelper
    {
        public static string GetMd5Hash(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return s;
            }

            var encoding = GetEncoding();
            var md5 = MD5.Create();

            var inputBytes = encoding.GetBytes(s);
            var hash = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();

            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        public static string Encrypt(string s, string key)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }

            var encoding = GetEncoding();
            var buffer = encoding.GetBytes(s);

            var crypto = CreateCryptoServiceProvider(encoding, key);

            var result = Convert.ToBase64String(crypto.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length));
            return result;
        }

        public static string Decrypt(string s, string key)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }

            var encoding = GetEncoding();

            var crypto = CreateCryptoServiceProvider(encoding, key);
            var buffer = Convert.FromBase64String(s);

            var result = encoding.GetString(crypto.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));
            return result;
        }

        private static Aes CreateCryptoServiceProvider(Encoding encoding, string key)
        {
            var aes = Aes.Create() ?? new AesCryptoServiceProvider();
            var md5 = new MD5CryptoServiceProvider();

            aes.Key = md5.ComputeHash(encoding.GetBytes(key));
            aes.GenerateIV();
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;

            return aes;
        }

        private static Encoding GetEncoding()
        {
            return Encoding.UTF8;
        }
    }
}
