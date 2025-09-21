using Microsoft.AspNetCore.Components;

namespace DevOpsControlCenter.Web.Services;

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly NavigationManager _nav;

    public ApiClient(HttpClient http, NavigationManager nav)
    {
        _http = http;
        _nav = nav;
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string relativeUrl, T body)
    {
        var url = new Uri(new Uri(_nav.BaseUri), relativeUrl);
        return await _http.PostAsJsonAsync(url, body);
    }
}
