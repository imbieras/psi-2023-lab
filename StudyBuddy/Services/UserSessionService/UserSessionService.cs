using StudyBuddy.Shared.Abstractions;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.Services.UserSessionService;

public class UserSessionService : IUserSessionService
{
    private readonly IHttpClientFactory _httpClientFactory;

    private UserId? _currentUserId;

    public UserSessionService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public UserId? GetCurrentUserId() => _currentUserId;

    public void SetCurrentUser(UserId userId) => _currentUserId = userId;

    public async Task<bool> AuthenticateUser(string username, string password)
    {
        var httpClient = _httpClientFactory.CreateClient("StudyBuddy.API");
        var response = await httpClient.GetAsync($"api/v1/user/by-username/{username}");

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
