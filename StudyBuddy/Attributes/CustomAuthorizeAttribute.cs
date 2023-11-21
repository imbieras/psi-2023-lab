using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StudyBuddy.Services.UserSessionService;

namespace StudyBuddy.Attributes;

public class CustomAuthorizeAttribute : TypeFilterAttribute
{
    public CustomAuthorizeAttribute() : base(typeof(CustomAuthorizeFilter))
    {
    }

    private class CustomAuthorizeFilter : IAuthorizationFilter
    {
        private readonly IUserSessionService _userSessionService;

        public CustomAuthorizeFilter(IUserSessionService userSessionService)
        {
            _userSessionService = userSessionService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (_userSessionService.GetCurrentUserId() == null)
            {
                context.Result = new RedirectToActionResult("Login", "Profile", null);
            }
        }
    }
}
