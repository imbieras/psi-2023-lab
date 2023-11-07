using StudyBuddy.Services.UserService;
using StudyBuddy.Services.UserSessionService;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Middlewares;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context, IUserSessionService userSessionService, IUserService userService)
    {
        Guid userIdGuid = default;
        if (!context.Request.Cookies.TryGetValue("UserId", out string? userIdString) ||
            !Guid.TryParse(userIdString, out userIdGuid))
        {
            // Clear the "UserId" cookie if it's not valid or doesn't exist
            context.Response.Cookies.Delete("UserId");
        }

        if (userSessionService.GetCurrentUserId() == null && await userService.GetUserByIdAsync(UserId.From(userIdGuid)) != null)
        {
            // Set the current user if it's not already set and the user exists
            userSessionService.SetCurrentUser(UserId.From(userIdGuid));
        }
        else
        {
            // Clear the "UserId" cookie if the user doesn't exist
            context.Response.Cookies.Delete("UserId");
        }

        await _next(context);
    }
}
