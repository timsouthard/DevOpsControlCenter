using DevOpsControlCenter.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevOpsControlCenter.Infrastructure.Persistence
{
    /// <summary>
    /// Primary Entity Framework Core DbContext for the application.
    /// 
    /// This context extends <see cref="IdentityDbContext{TUser}"/> to integrate
    /// ASP.NET Core Identity with custom application data. It provides access to
    /// both authentication/authorization tables (via Identity) and custom domain 
    /// entities defined by the application.
    /// 
    /// Responsibilities:
    /// - Manages persistence of Identity users, roles, claims, tokens, etc.
    /// - Provides DbSets for application-specific entities such as DevOps connections.
    /// - Configures entity relationships, schema, and migrations.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ApplicationDbContext"/>.
        /// </summary>
        /// <param name="options">
        /// The DbContext configuration options (typically supplied via dependency injection).
        /// </param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options) 
        {  
        }

        /// <summary>
        /// Represents the collection of <see cref="DevOpsConnection"/> records in the database.
        /// 
        /// This table stores configuration information for connecting to 
        /// external Azure DevOps instances (e.g., organization URLs and 
        /// personal access tokens). Only one record is typically needed 
        /// for most environments, but the schema allows multiple connections.
        /// </summary>
        public DbSet<DevOpsConnection> DevOpsConnections { get; set; } = null!;
    }
}
