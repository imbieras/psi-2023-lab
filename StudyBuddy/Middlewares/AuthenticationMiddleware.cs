using StudyBuddy.Abstractions;
using StudyBuddy.Managers.UserManager;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Middlewares;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context, IUserService userService, IUserManager userManager)
    {
        Guid userIdGuid = default;
        if (!context.Request.Cookies.TryGetValue("UserId", out string? userIdString) || !Guid.TryParse(userIdString, out userIdGuid))
        {
            // Clear the "UserId" cookie if it's not valid or doesn't exist
            context.Response.Cookies.Delete("UserId");
        }

        if (userService.GetCurrentUserId() == null && userManager.GetUserById(UserId.From(userIdGuid)) != null)
        {
            // Set the current user if it's not already set and the user exists
            userService.SetCurrentUser(UserId.From(userIdGuid));
        }
        else
        {
            // Clear the "UserId" cookie if the user doesn't exist
            context.Response.Cookies.Delete("UserId");
        }

        await _next(context);
    }
}
