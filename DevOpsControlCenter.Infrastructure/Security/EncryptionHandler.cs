using System.Security.Cryptography;
using System.Text;

namespace DevOpsControlCenter.Infrastructure.Security
{
    /// <summary>
    /// Provides helper methods for encrypting and decrypting sensitive data
    /// such as Personal Access Tokens (PATs).
    /// 
    /// This implementation uses AES-256 encryption with a key and IV derived
    /// deterministically from a passphrase and salt via PBKDF2 (Rfc2898).
    /// 
    /// Notes:
    /// - In production, the passphrase and salt should be stored securely
    ///   (e.g., Azure Key Vault, environment variables, or user secrets),
    ///   not hardcoded in source code.
    /// - The approach ensures that the same plaintext always produces the
    ///   same ciphertext, which simplifies storage but does not provide
    ///   semantic security. For higher security, a random IV should be
    ///   generated per encryption and stored alongside the ciphertext.
    /// </summary>
    public static class EncryptionHelper
    {
        /// <summary>
        /// A passphrase used for deriving the AES key and IV.
        /// This should be externalized to configuration in production.
        /// </summary>
        private const string Passphrase = "DevOpsControLCenter-Passphrase";

        /// <summary>
        /// Salt value used with PBKDF2 for key derivation.
        /// Must remain consistent for successful decryption.
        /// </summary>
        private static readonly byte[] Salt = Encoding.UTF8.GetBytes("DevOpsControlCenter-SaltValue");

        /// <summary>
        /// Derives a 256-bit AES key and a 128-bit IV using PBKDF2 with SHA-256.
        /// The same passphrase and salt will always generate the same values.
        /// </summary>
        /// <param name="key">The generated AES encryption key (32 bytes).</param>
        /// <param name="iv">The generated AES initialization vector (16 bytes).</param>
        private static void GetKeyAndIv(out byte[] key, out byte[] iv)
        {
            // 100,000 iterations of PBKDF2 → resistant to brute force
            using var pbkdf2 = new Rfc2898DeriveBytes(Passphrase, Salt, 100_000, HashAlgorithmName.SHA256);
            key = pbkdf2.GetBytes(32); // AES-256 key
            iv = pbkdf2.GetBytes(16);  // AES block size (128-bit IV)
        }

        /// <summary>
        /// Encrypts the given plaintext string using AES-256 with a derived key/IV.
        /// </summary>
        /// <param name="plainText">The string to encrypt.</param>
        /// <returns>Base64-encoded ciphertext, or an empty string if input is null/whitespace.</returns>
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                return string.Empty;

            GetKeyAndIv(out var key, out var iv);

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>
        /// Decrypts a previously encrypted string using AES-256 with the derived key/IV.
        /// 
        /// If the input is not valid Base64, the method assumes the value is legacy 
        /// plain text and returns it as-is. If the ciphertext cannot be decrypted 
        /// due to corruption or mismatched keys, an empty string is returned.
        /// </summary>
        /// <param name="cipherText">The Base64-encoded encrypted string.</param>
        /// <returns>The decrypted plaintext, legacy plain text, or empty string if invalid.</returns>
        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrWhiteSpace(cipherText))
                return string.Empty;

            try
            {
                GetKeyAndIv(out var key, out var iv);

                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;

                using var decryptor = aes.CreateDecryptor();
                var cipherBytes = Convert.FromBase64String(cipherText);
                var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (FormatException)
            {
                // Input was not Base64 → treat as legacy unencrypted string.
                return cipherText;
            }
            catch (CryptographicException)
            {
                // Decryption failed (wrong key/IV or corrupted ciphertext).
                return string.Empty;
            }
        }
    }
}
