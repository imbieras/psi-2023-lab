using StudyBuddy.API.Services.ChatService;
using StudyBuddy.API.Services.MatchingService;
using StudyBuddy.API.Services.SchedulingService;
using StudyBuddy.API.Services.UserService;

namespace StudyBuddy.API.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService.UserService>();
        services.AddScoped<IMatchingService, MatchingService.MatchingService>();
        services.AddScoped<IChatService, ChatService.ChatService>();

        return services;
    }
}
