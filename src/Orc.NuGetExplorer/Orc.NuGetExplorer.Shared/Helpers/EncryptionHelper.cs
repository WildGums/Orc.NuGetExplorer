// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MD5Helper.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    internal static class EncryptionHelper
    {
        private static readonly byte[] IV = { 240, 3, 45, 29, 0, 76, 173, 59 };

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

        private static TripleDESCryptoServiceProvider CreateCryptoServiceProvider(Encoding encoding, string key)
        {
            var des = new TripleDESCryptoServiceProvider();
            var md5 = new MD5CryptoServiceProvider();

            des.Key = md5.ComputeHash(encoding.GetBytes(key));
            des.IV = IV;

            return des;
        }

        private static Encoding GetEncoding()
        {
            return Encoding.UTF8;
        }
    }
}