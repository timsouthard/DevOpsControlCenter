using DevOpsControlCenter.Domain.Entities;
using DevOpsControlCenter.Infrastructure.Persistence;
using DevOpsControlCenter.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace DevOpsControlCenter.Tests;

public class DevOpsConnectionServiceTests
{
    private async Task<DevOpsConnectionService> CreateServiceAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // isolated per test
            .Options;

        var factory = new PooledDbContextFactory<ApplicationDbContext>(options);
        return new DevOpsConnectionService(factory);
    }

    [Fact]
    public async Task AddAsync_SavesAndDecryptsConnection()
    {
        // Arrange
        var service = await CreateServiceAsync();

        var connection = new DevOpsConnection
        {
            Name = "Test Connection",
            Url = "https://dev.azure.com/clientsystems",
            PersonalAccessToken = "PAT123"
        };

        // Act
        await service.AddAsync(connection);
        var saved = await service.GetDefaultAsync();

        // Assert
        Assert.NotNull(saved);
        Assert.Equal("Test Connection", saved!.Name);
        Assert.Equal("PAT123", saved.PersonalAccessToken); // decrypted automatically
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrueAfterAddingConnection()
    {
        // Arrange
        var service = await CreateServiceAsync();

        // Act
        var before = await service.ExistsAsync();
        await service.AddAsync(new DevOpsConnection
        {
            Name = "Conn",
            Url = "https://dev.azure.com/x",
            PersonalAccessToken = "PAT"
        });
        var after = await service.ExistsAsync();

        // Assert
        Assert.False(before);
        Assert.True(after);
    }
}
