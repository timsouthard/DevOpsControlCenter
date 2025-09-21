using System.Text.Json.Serialization;

namespace DevOpsControlCenter.Web.Dtos;

public class ProjectListResponse
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("value")]
    public List<ProjectDto> Value { get; set; } = new();
}
