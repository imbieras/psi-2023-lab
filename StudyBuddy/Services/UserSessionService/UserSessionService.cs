using StudyBuddy.Shared.Abstractions;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.Services.UserSessionService;

public class UserSessionService : IUserSessionService
{
    private readonly HttpClient _httpClient;

    private UserId? _currentUserId;

    public UserSessionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public UserId? GetCurrentUserId() => _currentUserId;

    public void SetCurrentUser(UserId userId) => _currentUserId = userId;

    public async Task<bool> AuthenticateUser(string username, string password)
    {
        var response = await _httpClient.GetAsync($"api/v1/user/username/{username}");
        response.EnsureSuccessStatusCode();
        IUser? user = await response.Content.ReadFromJsonAsync<IUser>();
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return false;
        }
        SetCurrentUser(user.Id);
        return true;
    }
}
