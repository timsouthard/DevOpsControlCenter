using DevOpsControlCenter.Domain.Entities;
using DevOpsControlCenter.Infrastructure.Persistence;
using DevOpsControlCenter.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace DevOpsControlCenter.Infrastructure.Services
{
    /// <summary>
    /// Defines the contract for managing DevOps connection settings.
    /// </summary>
    public interface IDevOpsConnectionService
    {
        /// <summary>
        /// Retrieves the default DevOps connection, if one exists.
        /// The stored Personal Access Token (PAT) is automatically decrypted before returning.
        /// </summary>
        /// <returns>
        /// A <see cref="DevOpsConnection"/> instance with decrypted credentials,
        /// or <c>null</c> if no connection is configured.
        /// </returns>
        Task<DevOpsConnection?> GetDefaultAsync();

        /// <summary>
        /// Checks whether at least one DevOps connection has been stored.
        /// </summary>
        /// <returns><c>true</c> if a connection exists; otherwise <c>false</c>.</returns>
        Task<bool> ExistsAsync();

        /// <summary>
        /// Adds a new DevOps connection to the database.
        /// The Personal Access Token is encrypted before being persisted.
        /// </summary>
        /// <param name="connection">The connection details to add.</param>
        Task AddAsync(DevOpsConnection connection);
    }

    /// <summary>
    /// Implements <see cref="IDevOpsConnectionService"/> using EF Core.
    /// 
    /// Connections are persisted in the <see cref="ApplicationDbContext"/> and
    /// Personal Access Tokens (PATs) are encrypted before storage.
    /// 
    /// Uses <see cref="IDbContextFactory{TContext}"/> to ensure a new
    /// <see cref="ApplicationDbContext"/> instance per operation,
    /// avoiding concurrency issues with long-lived DbContext instances.
    /// </summary>
    public class DevOpsConnectionService : IDevOpsConnectionService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="DevOpsConnectionService"/>.
        /// </summary>
        /// <param name="contextFactory">Factory for creating <see cref="ApplicationDbContext"/> instances.</param>
        public DevOpsConnectionService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <inheritdoc/>
        public async Task<DevOpsConnection?> GetDefaultAsync()
        {
            await using var db = _contextFactory.CreateDbContext();
            var conn = await db.DevOpsConnections.AsNoTracking().FirstOrDefaultAsync();

            if (conn is not null)
            {
                // Decrypt the PAT before returning it to consumers
                conn.PersonalAccessToken = EncryptionHelper.Decrypt(conn.PersonalAccessToken);
            }

            return conn;
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync()
        {
            await using var db = _contextFactory.CreateDbContext();
            return await db.DevOpsConnections.AnyAsync();
        }

        /// <inheritdoc/>
        public async Task AddAsync(DevOpsConnection connection)
        {
            await using var db = _contextFactory.CreateDbContext();

            // Always encrypt PAT before saving to the database
            connection.PersonalAccessToken = EncryptionHelper.Encrypt(connection.PersonalAccessToken);

            db.DevOpsConnections.Add(connection);
            await db.SaveChangesAsync();
        }
    }
}
