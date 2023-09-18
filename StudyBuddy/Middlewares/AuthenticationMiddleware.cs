using StudyBuddy.Abstractions;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Middlewares;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Cookies.TryGetValue("UserId", out string? userIdString))
        {
            if (Guid.TryParse(userIdString, out Guid userIdGuid))
            {
                IUserService? userService = context.RequestServices.GetService<IUserService>();
                userService?.SetCurrentUser(UserId.From(userIdGuid));
            }
        }

        await _next(context);
    }
}
