using StudyBuddy.Abstractions;
using StudyBuddy.Data.Repositories.UserRepository;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Services.UserService;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository) => _userRepository = userRepository;

    public async Task<IEnumerable<IUser>> GetAllUsersAsync() => await _userRepository.GetAllAsync();
    public async Task<IUser?> GetUserByIdAsync(UserId userId) => await _userRepository.GetByIdAsync(userId);

    public async Task<List<string>?> GetHobbiesById(UserId userId) => await _userRepository.GetHobbiesByIdAsync(userId);

    public async Task<IUser?> GetUserByUsernameAsync(string username)
    {
        IEnumerable<User> users = await _userRepository.GetAllAsync();
        return users.FirstOrDefault(u => u.Name == username);
    }

    public async Task BanUserAsync(UserId userId)
    {
        User? user = await _userRepository.GetByIdAsync(userId);
        if (user != null)
        {
            user.Flags |= UserFlags.Banned;
            await _userRepository.UpdateAsync(user);
        }
    }

    public async Task<UserId> RegisterUserAsync(string username, UserFlags flags, UserTraits traits,
        List<string> hobbies)
    {
        UserId userId = UserId.From(Guid.NewGuid());

        if (string.IsNullOrEmpty(traits.AvatarPath))
        {
            traits.AvatarPath = User.GenerateGravatarUrl(userId);
        }

        User newUser = new(userId, username, flags, traits, hobbies);
        await _userRepository.AddAsync(newUser);

        return userId;
    }


    public async Task AddHobbiesToUserAsync(User user, List<string> hobbies) =>
        await _userRepository.AddHobbiesToUserAsync(user, hobbies);


    // TODO: We can add a custom exception here
    public async Task AddHobbiesToUserAsync(UserId userId, List<string> hobbies)
    {
        User? user = await _userRepository.GetByIdAsync(userId);
        if (user != null)
        {
            user.Hobbies = hobbies;
            await _userRepository.AddAsync(user);
        }
    }

    public IUser? GetRandomUser()
    {
        IEnumerable<User> users = _userRepository.GetAllAsync().Result;
        Random random = new();
        IEnumerable<User> enumerable = users.ToList();
        int randomIndex = random.Next(0, enumerable.Count());
        return enumerable.ElementAt(randomIndex);
    }

    public async Task<IUser?> GetUltimateSeenUserAsync(UserId userId)
    {
        UserId latestId = await _userRepository.GetUltimateSeenUserAsync(userId);
        return await _userRepository.GetByIdAsync(latestId);
    }

    public async Task<IUser?> GetPenultimateSeenUserAsync(UserId userId)
    {
        UserId penultimateId = await _userRepository.GetPenultimateSeenUserAsync(userId);
        return await _userRepository.GetByIdAsync(penultimateId);
    }

    public async Task<bool> IsUserNotSeenAnyUserAsync(UserId userId)
    {
        return await _userRepository.IsUserNotSeenAnyUserAsync(userId);
    }

    public async Task<bool> IsUserSeenAsync(UserId userId, UserId otherUserId)
    {
        return await _userRepository.IsUserSeenAsync(userId, otherUserId);
    }

    public async Task UserSeenAsync(UserId userId, UserId otherUserId)
    {
        await _userRepository.UserSeenAsync(userId, otherUserId);
    }
}
