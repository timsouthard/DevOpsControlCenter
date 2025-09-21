namespace DevOpsControlCenter.Domain.Entities;

/// <summary>
/// Represents the configuration for connecting to an Azure DevOps organization.
/// 
/// This entity stores the essential information needed for DOCC (DevOps Control Center)
/// to authenticate and interact with the Azure DevOps REST APIs. Only one connection
/// is typically required, but the model supports multiple entries for flexibility.
/// </summary>
public class DevOpsConnection
{
    /// <summary>
    /// Primary key identifier for the DevOps connection record.
    /// </summary>
    public int DevOpsConnectionId { get; set; }

    /// <summary>
    /// A friendly display name for the connection.
    /// Example: "Client Systems DevOps" or "Internal Sandbox".
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The base URL of the Azure DevOps organization or collection.
    /// Example: https://dev.azure.com/clientsystems
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// The Personal Access Token (PAT) used for authentication.
    /// This value is encrypted before being persisted to the database
    /// and decrypted only when used at runtime.
    /// </summary>
    public string PersonalAccessToken { get; set; } = string.Empty;

    /// <summary>
    /// The UTC timestamp when this connection was created.
    /// Defaults to the time of entity instantiation.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The UTC timestamp of the most recent usage of this connection.
    /// Updated whenever the system successfully authenticates or performs
    /// an operation against Azure DevOps with this connection.
    /// </summary>
    public DateTime? LastUsedAt { get; set; }
}
