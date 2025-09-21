using DevOpsControlCenter.Infrastructure.Security;
using Xunit;

namespace DevOpsControlCenter.Tests;

public class EncryptionHelperTests
{
    [Fact]
    public void Encrypt_Then_Decrypt_ReturnsOriginal()
    {
        // Arrange
        var plainText = "SuperSecretValue123!";

        // Act
        var encrypted = EncryptionHelper.Encrypt(plainText);
        var decrypted = EncryptionHelper.Decrypt(encrypted);

        // Assert
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void Decrypt_InvalidCipher_ReturnsOriginalText()
    {
        // Arrange
        var badCipher = "not-a-valid-base64";

        // Act
        var decrypted = EncryptionHelper.Decrypt(badCipher);

        // Assert
        Assert.Equal(badCipher, decrypted);
    }


    [Fact]
    public void Encrypt_Blank_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, EncryptionHelper.Encrypt(""));
    }
}
