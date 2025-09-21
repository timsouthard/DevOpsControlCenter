using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DevOpsControlCenter.Domain.Entities;
using DevOpsControlCenter.Infrastructure.Services;
using DevOpsControlCenter.Web.Dtos;

namespace DevOpsControlCenter.Web.Services;

public interface IDevOpsService
{
    Task<List<ProjectDto>> GetProjectsAsync();
}

public class DevOpsService : IDevOpsService
{
    private readonly IDevOpsConnectionService _connectionService;
    private readonly HttpClient _http;

    public DevOpsService(IDevOpsConnectionService connectionService, IHttpClientFactory factory)
    {
        _connectionService = connectionService;
        _http = factory.CreateClient();
    }

    public async Task<List<ProjectDto>> GetProjectsAsync()
    {
        var conn = await _connectionService.GetDefaultAsync();
        if (conn is null)
            return new List<ProjectDto>();

        // Configure PAT auth
        var pat = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{conn.PersonalAccessToken}"));
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", pat);

        // Call Azure DevOps REST API
        var url = $"{conn.Url}/_apis/projects?api-version=7.1-preview.4";
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var x = await response.Content.ReadAsStringAsync();

        using var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<ProjectListResponse>(stream);

        return result?.Value ?? new List<ProjectDto>();
    }
}
