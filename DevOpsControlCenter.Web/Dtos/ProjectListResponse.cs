using System.Text.Json.Serialization;

namespace DevOpsControlCenter.Web.Dtos;

/// <summary>
/// Represents the full response from the Azure DevOps
/// <c>_apis/projects</c> endpoint.
/// 
/// The API returns a JSON object containing:
/// - a <c>count</c> property (number of projects returned)
/// - a <c>value</c> array with one entry per project.
/// 
/// This DTO is used for deserializing the top-level response
/// before mapping the inner project entries to <see cref="ProjectDto"/>.
/// </summary>
public class ProjectListResponse
{
    /// <summary>
    /// Total number of projects included in the <c>value</c> array.
    /// Note: This may not represent all projects in the org if
    /// paging is applied by the API.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// Collection of Azure DevOps projects, each represented
    /// by a <see cref="ProjectDto"/>.
    /// </summary>
    [JsonPropertyName("value")]
    public List<ProjectDto> Value { get; set; } = new();
}
