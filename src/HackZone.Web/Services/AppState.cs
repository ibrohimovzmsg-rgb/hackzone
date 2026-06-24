namespace HackZone.Web.Services;

public class AppState
{
    public string? Username { get; private set; }
    public string? AccessToken { get; private set; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);
    public List<string> Roles { get; private set; } = [];

    public event Action? OnChange;

    public void SetUser(string username, string accessToken, List<string> roles)
    {
        Username = username;
        AccessToken = accessToken;
        Roles = roles;
        OnChange?.Invoke();
    }

    public void Logout()
    {
        Username = null;
        AccessToken = null;
        Roles = [];
        OnChange?.Invoke();
    }

    public bool IsAdmin => Roles.Contains("Admin");
    public bool IsInstructor => Roles.Contains("Instructor");
}
