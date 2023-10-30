using StudyBuddy.Data.Repositories;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Services.UserService;

using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using StudyBuddy.Data.Repositories;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IUser?> GetUserByIdAsync(UserId userId) => await _userRepository.GetByIdAsync(userId);

    public async Task<IUser?> GetUserByUsernameAsync(string username)
    {
        var users = await _userRepository.GetAllAsync();
        return users.FirstOrDefault(u => u.Name == username);
    }

    public async Task BanUserAsync(UserId userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user != null)
        {
            user.Flags |= UserFlags.Banned;
            await _userRepository.UpdateAsync(user);
        }
    }

    public async Task<UserId> RegisterUserAsync(string username, UserFlags flags, UserTraits traits)
    {
        UserId userId = UserId.From(Guid.NewGuid());

        if (string.IsNullOrEmpty(traits.AvatarPath))
        {
            traits.AvatarPath = User.GenerateGravatarUrl(userId);
        }

        User newUser = new(userId, username, flags, traits);
        await _userRepository.AddAsync(newUser);

        return userId;
    }

    public async Task<IEnumerable<IUser>> GetAllUsersAsync() => await _userRepository.GetAllAsync();

    public IUser? GetRandomUser(IUser user) => throw new NotImplementedException();
    public IUser? GetPreviousRandomProfile(IUser user) => throw new NotImplementedException();
    public IUser? GetCurrentRandomUser(IUser user) => throw new NotImplementedException();
    public bool IsUsedIndexesEmpty(IUser user) => throw new NotImplementedException();
}
