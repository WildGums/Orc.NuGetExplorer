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

            // Important notes on changing implementation: 
            // You need to keep IV value of aes provider the same per encryption/decryption operation
            // if you select CBC mode, otherwise it will fails with wrong offset
            aes.IV = new byte[] {
                0x2, 0x4, 0x8, 0x16,
                0x1, 0x3, 0x5, 0x7,
                0x11, 0x12, 0x13, 0x14,
                0x16, 0x16, 0x16, 0x16,
            };
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
