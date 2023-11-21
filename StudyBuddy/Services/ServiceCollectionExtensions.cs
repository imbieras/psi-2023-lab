using StudyBuddy.Services.ChatService;
using StudyBuddy.Services.MatchingService;
using StudyBuddy.Services.UserService;
using StudyBuddy.Services.UserSessionService;

namespace StudyBuddy.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService.UserService>();
        services.AddScoped<IUserSessionService, UserSessionService.UserSessionService>();
        services.AddScoped<IMatchingService, MatchingService.MatchingService>();
        services.AddScoped<IChatService, ChatService.ChatService>();

        return services;
    }
}
