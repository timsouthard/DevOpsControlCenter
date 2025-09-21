using System.Security.Cryptography;
using System.Text;

namespace DevOpsControlCenter.Infrastructure.Security;

public static class EncryptionHelper
{
    // You can load these from config/secrets instead of hardcoding
    private const string Passphrase = "DevOpsControLCenter-Passphrase";
    private static readonly byte[] Salt = Encoding.UTF8.GetBytes("DevOpsControlCenter-SaltValue");

    private static void GetKeyAndIv(out byte[] key, out byte[] iv)
    {
        // PBKDF2 with SHA256 to derive AES-256 key + IV
        using var pbkdf2 = new Rfc2898DeriveBytes(Passphrase, Salt, 100_000, HashAlgorithmName.SHA256);
        key = pbkdf2.GetBytes(32); // 256-bit key
        iv = pbkdf2.GetBytes(16);  // 128-bit IV
    }

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
            // Not Base64? → must be legacy plain text.
            return cipherText;
        }
        catch (CryptographicException)
        {
            // Wrong key/iv or corrupt data.
            return string.Empty;
        }
    }

}
