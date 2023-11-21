using StudyBuddy.Data.Repositories.ChatRepository;
using StudyBuddy.Data.Repositories.MatchRepository;
using StudyBuddy.Data.Repositories.UserRepository;

namespace StudyBuddy.Data.Repositories;

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
