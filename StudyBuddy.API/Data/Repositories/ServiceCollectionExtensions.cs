using StudyBuddy.API.Data.Repositories.ChatRepository;
using StudyBuddy.API.Data.Repositories.MatchRepository;
using StudyBuddy.API.Data.Repositories.UserRepository;

namespace StudyBuddy.API.Data.Repositories;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository.UserRepository>();
        services.AddScoped<IMatchRepository, MatchRepository.MatchRepository>();
        services.AddScoped<IMatchRequestRepository, MatchRequestRepository>();
        services.AddScoped<IChatRepository, ChatRepository.ChatRepository>();

        return services;
    }
}
