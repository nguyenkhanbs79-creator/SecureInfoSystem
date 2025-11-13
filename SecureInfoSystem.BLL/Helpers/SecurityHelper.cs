using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SecureInfoSystem.BLL.Helpers
{
    /// <summary>
    /// Provides reusable cryptographic helpers such as hashing and AES encryption.
    /// </summary>
    public static class SecurityHelper
    {
        private const string Passphrase = "SecureInfoSystemDemo-Key";
        private static readonly byte[] Salt = new byte[] { 0x10, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 };

        /// <summary>
        /// Hashes a password using SHA-256 and returns the Base64-encoded value.
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password must not be null or whitespace.", nameof(password));
            }

            using (var sha256 = SHA256.Create())
            {
                // TODO: Add salt for production scenarios.
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// Verifies the provided password against an existing hash.
        /// </summary>
        public static bool VerifyPassword(string password, string passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash))
            {
                return false;
            }

            string computedHash = HashPassword(password);
            return string.Equals(computedHash, passwordHash, StringComparison.Ordinal);
        }

        /// <summary>
        /// Encrypts the provided plaintext using AES and returns the Base64 cipher text.
        /// </summary>
        public static string EncryptString(string plainText)
        {
            if (plainText == null)
            {
                return null;
            }

            if (plainText.Length == 0)
            {
                return string.Empty;
            }

            using (var aes = CreateAes())
            using (var encryptor = aes.CreateEncryptor())
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                cryptoStream.FlushFinalBlock();

                byte[] cipherBytes = memoryStream.ToArray();
                return Convert.ToBase64String(cipherBytes);
            }
        }

        /// <summary>
        /// Decrypts the provided Base64 cipher text using AES and returns the plaintext.
        /// </summary>
        public static string DecryptString(string cipherTextBase64)
        {
            if (cipherTextBase64 == null)
            {
                return null;
            }

            if (cipherTextBase64.Length == 0)
            {
                return string.Empty;
            }

            byte[] cipherBytes = Convert.FromBase64String(cipherTextBase64);

            using (var aes = CreateAes())
            using (var decryptor = aes.CreateDecryptor())
            using (var memoryStream = new MemoryStream(cipherBytes))
            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            using (var reader = new StreamReader(cryptoStream, Encoding.UTF8))
            {
                try
                {
                    return reader.ReadToEnd();
                }
                catch (CryptographicException ex)
                {
                    throw new ApplicationException("Failed to decrypt the provided cipher text.", ex);
                }
            }
        }

        private static Aes CreateAes()
        {
            var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var keyDerivation = new Rfc2898DeriveBytes(Passphrase, Salt, 10000, HashAlgorithmName.SHA256))
            {
                aes.Key = keyDerivation.GetBytes(aes.KeySize / 8);
                aes.IV = keyDerivation.GetBytes(aes.BlockSize / 8);
            }

            return aes;
        }
    }
}
