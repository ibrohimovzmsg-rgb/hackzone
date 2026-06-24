using Blazored.LocalStorage;

namespace HackZone.Web.Services;

public class AuthService(ApiService api, AppState state, ILocalStorageService storage)
{
    private const string AccessKey = "hz_access";
    private const string RefreshKey = "hz_refresh";
    private const string UserKey = "hz_user";

    public async Task<(bool Success, string? Error)> LoginAsync(string email, string password)
    {
        var (ok, data, err) = await api.PostAsync<ApiAuthResponse>("api/auth/login", new { email, password });
        if (!ok || data is null) return (false, err);
        await PersistAndApply(data);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(string username, string email, string password)
    {
        var (ok, data, err) = await api.PostAsync<ApiAuthResponse>("api/auth/register", new { username, email, password });
        if (!ok || data is null) return (false, err);
        await PersistAndApply(data);
        return (true, null);
    }

    public async Task LogoutAsync()
    {
        await storage.RemoveItemAsync(AccessKey);
        await storage.RemoveItemAsync(RefreshKey);
        await storage.RemoveItemAsync(UserKey);
        state.Logout();
    }

    public async Task TryRestoreSessionAsync()
    {
        var access = await storage.GetItemAsStringAsync(AccessKey);
        var user = await storage.GetItemAsync<StoredUser>(UserKey);
        if (access is not null && user is not null)
            state.SetUser(user.Username, access, user.Roles);
    }

    private async Task PersistAndApply(ApiAuthResponse data)
    {
        var roles = data.User.Roles?.ToList() ?? [];
        await storage.SetItemAsStringAsync(AccessKey, data.Token.AccessToken);
        await storage.SetItemAsStringAsync(RefreshKey, data.Token.RefreshToken);
        await storage.SetItemAsync(UserKey, new StoredUser(data.User.Username, roles));
        state.SetUser(data.User.Username, data.Token.AccessToken, roles);
    }
}

public record ApiAuthResponse(ApiTokenResponse Token, ApiUserResponse User);
public record ApiTokenResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);
public record ApiUserResponse(Guid Id, string Username, string Email, string? DisplayName, int ReputationPoints, bool EmailConfirmed, string[]? Roles);
public record StoredUser(string Username, List<string> Roles);
