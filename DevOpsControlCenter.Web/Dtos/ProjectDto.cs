using System.Text.Json.Serialization;

namespace DevOpsControlCenter.Web.Dtos;

/// <summary>
/// Represents a single Azure DevOps project as returned
/// by the Azure DevOps REST API.
///
/// This DTO is shaped to match the JSON payload returned by
/// the <c>_apis/projects</c> endpoint and is used in the
/// Web layer for display or transformation into domain models.
/// </summary>
public class ProjectDto
{
    /// <summary>
    /// Unique identifier (GUID) of the project in Azure DevOps.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable name of the project.
    /// Example: "Solution Center" or "SCV2".
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional project description provided in Azure DevOps.
    /// May be <c>null</c> if no description is set.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// API URL pointing to this project resource in Azure DevOps.
    /// This is not the same as the user-facing project URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Current state of the project.
    /// Typically "wellFormed" if active, or "deleted" if removed.
    /// </summary>
    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Incrementing revision number that tracks changes
    /// to the project definition in Azure DevOps.
    /// </summary>
    [JsonPropertyName("revision")]
    public int Revision { get; set; }

    /// <summary>
    /// Visibility of the project: "private" or "public".
    /// </summary>
    [JsonPropertyName("visibility")]
    public string Visibility { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp of the most recent update to the project metadata.
    /// </summary>
    [JsonPropertyName("lastUpdateTime")]
    public DateTime LastUpdateTime { get; set; }
}
