namespace Orc.NuGetExplorer
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    internal static class EncryptionHelper
    {
        // ReSharper disable once InconsistentNaming
        private static readonly byte[] IV = new byte[16] { 210, 10, 56, 110, 98, 189, 66, 77, 83, 120, 86, 44, 67, 111, 98, 66 };

        public static string GetMd5Hash(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return s;
            }

            var encoding = GetEncoding();

            using (var md5 = MD5.Create())
            {
                var inputBytes = encoding.GetBytes(s);
                var hash = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();

                for (var i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }

        public static string Encrypt(string s, string key)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }

            var encoding = GetEncoding();
            var buffer = encoding.GetBytes(s);

            using (var crypto = CreateCryptoServiceProvider(encoding, key))
            {
                using (var encryptor = crypto.CreateEncryptor())
                {
                    var result = Convert.ToBase64String(encryptor.TransformFinalBlock(buffer, 0, buffer.Length));
                    return result;
                }
            }
        }

        public static string Decrypt(string s, string key)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }

            var encoding = GetEncoding();

            using (var crypto = CreateCryptoServiceProvider(encoding, key))
            {
                var buffer = Convert.FromBase64String(s);

                using (var decryptor = crypto.CreateDecryptor())
                {
                    var result = encoding.GetString(decryptor.TransformFinalBlock(buffer, 0, buffer.Length));
                    return result;
                }
            }
        }

        private static Aes CreateCryptoServiceProvider(Encoding encoding, string key)
        {
            var aes = Aes.Create();

            using (var md5 = MD5.Create())
            {
                aes.Key = md5.ComputeHash(encoding.GetBytes(key));
            }

            aes.IV = IV;
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
