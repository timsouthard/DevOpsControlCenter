using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DevOpsControlCenter.Domain.Entities;
using DevOpsControlCenter.Infrastructure.Services;
using DevOpsControlCenter.Web.Dtos;

namespace DevOpsControlCenter.Web.Services;

/// <summary>
/// Defines the contract for interacting with Azure DevOps resources.
/// Currently focused on retrieving projects but can be extended
/// to support work items, builds, pipelines, etc.
/// </summary>
public interface IDevOpsService
{
    /// <summary>
    /// Retrieves a list of projects from the configured Azure DevOps organization.
    /// </summary>
    /// <returns>A list of <see cref="ProjectDto"/> objects, or an empty list if no connection exists.</returns>
    Task<List<ProjectDto>> GetProjectsAsync();
}

/// <summary>
/// Provides an implementation of <see cref="IDevOpsService"/> that connects
/// to Azure DevOps using a stored <see cref="DevOpsConnection"/> entity.
/// 
/// Responsibilities:
/// - Reads the default DevOps connection from the database.
/// - Configures HTTP requests with a Personal Access Token (PAT).
/// - Calls Azure DevOps REST APIs and maps the response into DTOs.
/// </summary>
public class DevOpsService : IDevOpsService
{
    private readonly IDevOpsConnectionService _connectionService;
    private readonly HttpClient _http;

    /// <summary>
    /// Initializes a new instance of the <see cref="DevOpsService"/> class.
    /// </summary>
    /// <param name="connectionService">Service for retrieving stored connection details.</param>
    /// <param name="factory">Factory used to create an <see cref="HttpClient"/> instance.</param>
    public DevOpsService(IDevOpsConnectionService connectionService, IHttpClientFactory factory)
    {
        _connectionService = connectionService;
        _http = factory.CreateClient();
    }

    /// <inheritdoc/>
    public async Task<List<ProjectDto>> GetProjectsAsync()
    {
        // Retrieve connection info (including decrypted PAT)
        var conn = await _connectionService.GetDefaultAsync();
        if (conn is null)
            return new List<ProjectDto>(); // no connection available, return empty list

        // Configure Azure DevOps PAT authentication
        // Format: "Basic :<PAT>" encoded as Base64
        var pat = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{conn.PersonalAccessToken}"));
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", pat);

        // Build the projects API endpoint URL
        var url = $"{conn.Url}/_apis/projects?api-version=7.1-preview.4";

        // Perform GET request
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        // Deserialize JSON response body into ProjectListResponse
        await using var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<ProjectListResponse>(stream);

        // Return list of projects or empty list
        return result?.Value ?? new List<ProjectDto>();
    }
}
