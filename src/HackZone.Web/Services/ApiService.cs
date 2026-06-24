using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HackZone.Web.Services;

public class ApiService(IHttpClientFactory factory, AppState appState)
{
    private HttpClient CreateClient()
    {
        var client = factory.CreateClient("HackZoneApi");
        if (appState.IsAuthenticated)
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", appState.AccessToken);
        return client;
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        try { return await CreateClient().GetFromJsonAsync<T>(url); }
        catch { return default; }
    }

    public async Task<(bool Success, T? Data, string? Error)> PostAsync<T>(string url, object body)
    {
        try
        {
            var resp = await CreateClient().PostAsJsonAsync(url, body);
            if (resp.IsSuccessStatusCode)
                return (true, await resp.Content.ReadFromJsonAsync<T>(), null);
            var err = await resp.Content.ReadFromJsonAsync<ErrorResponse>();
            return (false, default, err?.Error ?? resp.ReasonPhrase);
        }
        catch (Exception ex) { return (false, default, ex.Message); }
    }

    public async Task<(bool Success, string? Error)> PostAsync(string url, object body)
    {
        try
        {
            var resp = await CreateClient().PostAsJsonAsync(url, body);
            if (resp.IsSuccessStatusCode) return (true, null);
            var err = await resp.Content.ReadFromJsonAsync<ErrorResponse>();
            return (false, err?.Error ?? resp.ReasonPhrase);
        }
        catch (Exception ex) { return (false, ex.Message); }
    }
}

public record ErrorResponse(string Error);
