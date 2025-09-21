using Microsoft.AspNetCore.Components;

namespace DevOpsControlCenter.Web.Services;

/// <summary>
/// A lightweight wrapper around <see cref="HttpClient"/> that simplifies
/// making API requests within the Blazor application.
/// 
/// This service:
/// - Resolves relative URLs into absolute ones using the app's base URI.
/// - Provides strongly typed JSON POST functionality.
/// - Can be extended later with additional HTTP verbs (GET, PUT, DELETE, etc.).
/// 
/// Note: This client is scoped to the Blazor app and relies on dependency
/// injection of both <see cref="HttpClient"/> and <see cref="NavigationManager"/>.
/// </summary>
public class ApiClient
{
    private readonly HttpClient _http;
    private readonly NavigationManager _nav;

    /// <summary>
    /// Initializes the API client with the injected <see cref="HttpClient"/>
    /// and <see cref="NavigationManager"/>.
    /// </summary>
    public ApiClient(HttpClient http, NavigationManager nav)
    {
        _http = http;
        _nav = nav;
    }

    /// <summary>
    /// Sends a POST request with a JSON-serialized body to a relative URL
    /// within the application.
    /// 
    /// The <paramref name="relativeUrl"/> is combined with the app's
    /// <see cref="NavigationManager.BaseUri"/> to ensure the request
    /// is made against the correct server endpoint.
    /// </summary>
    /// <typeparam name="T">The type of the request body.</typeparam>
    /// <param name="relativeUrl">Relative path to the target API endpoint.</param>
    /// <param name="body">The request payload to serialize as JSON.</param>
    /// <returns>The raw <see cref="HttpResponseMessage"/> from the server.</returns>
    public async Task<HttpResponseMessage> PostAsync<T>(string relativeUrl, T body)
    {
        // Ensure the target URL is absolute (BaseUri + relative path)
        var url = new Uri(new Uri(_nav.BaseUri), relativeUrl);

        // Send POST with JSON body
        return await _http.PostAsJsonAsync(url, body);
    }
}
