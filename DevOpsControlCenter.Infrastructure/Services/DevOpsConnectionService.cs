using DevOpsControlCenter.Domain.Entities;
using DevOpsControlCenter.Infrastructure.Persistence;
using DevOpsControlCenter.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace DevOpsControlCenter.Infrastructure.Services;

public interface IDevOpsConnectionService
{
    Task<DevOpsConnection?> GetDefaultAsync();
    Task<bool> ExistsAsync();
    Task AddAsync(DevOpsConnection connection);
}

public class DevOpsConnectionService : IDevOpsConnectionService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public DevOpsConnectionService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<DevOpsConnection?> GetDefaultAsync()
    {
        await using var db = _contextFactory.CreateDbContext();
        var conn = await db.DevOpsConnections.AsNoTracking().FirstOrDefaultAsync();

        if (conn is not null)
        {
            conn.PersonalAccessToken = EncryptionHelper.Decrypt(conn.PersonalAccessToken);
        }

        return conn;
    }

    public async Task<bool> ExistsAsync()
    {
        await using var db = _contextFactory.CreateDbContext();
        return await db.DevOpsConnections.AnyAsync();
    }

    public async Task AddAsync(DevOpsConnection connection)
    {
        await using var db = _contextFactory.CreateDbContext();

        connection.PersonalAccessToken = EncryptionHelper.Encrypt(connection.PersonalAccessToken);

        db.DevOpsConnections.Add(connection);
        await db.SaveChangesAsync();
    }
}
