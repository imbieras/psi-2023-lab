using System.Text.Json;
using StudyBuddy.Services.UserSessionService;
using StudyBuddy.Shared.Abstractions;
using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.Middlewares;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpClientFactory _clientFactory;

    public AuthenticationMiddleware(RequestDelegate next, IHttpClientFactory clientFactory)
    {
        _next = next;
        _clientFactory = clientFactory;
    }

    public async Task Invoke(HttpContext context, IUserSessionService userSessionService)
    {
        Guid userIdGuid = default;
        if (!context.Request.Cookies.TryGetValue("UserId", out string? userIdString) ||
            !Guid.TryParse(userIdString, out userIdGuid))
        {
            // Clear the "UserId" cookie if it's not valid or doesn't exist
            context.Response.Cookies.Delete("UserId");
        }

        HttpClient? httpClient = _clientFactory.CreateClient("StudyBuddy.API");

        if (userSessionService.GetCurrentUserId() == null)
        {
            // Make a request to the API to get user details
            UserId userId = UserId.From(userIdGuid);

            HttpResponseMessage? response = await httpClient.GetAsync($"api/v1/user/{userId}");
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    IUser? currentUser = await response.Content.ReadFromJsonAsync<User>();
                    if (currentUser != null)
                    {
                        userSessionService.SetCurrentUser(currentUser.Id);
                    }
                }
                catch (JsonException)
                {
                    context.Response.Cookies.Delete("UserId");
                }
            }
            else
            {
                // Clear the "UserId" cookie if the user doesn't exist or there's an issue with the API call
                context.Response.Cookies.Delete("UserId");
            }
        }

        await _next(context);
    }
}
