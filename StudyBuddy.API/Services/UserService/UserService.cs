using System.Text.RegularExpressions;
using StudyBuddy.API.Data.Repositories.UserRepository;
using StudyBuddy.API.Models;
using StudyBuddy.Shared.Abstractions;
using StudyBuddy.Shared.Exceptions;
using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.API.Services.UserService;

public partial class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<IUser>> GetAllUsersAsync() => await _userRepository.GetAllAsync();
    public async Task<IUser?> GetUserByIdAsync(UserId userId) => await _userRepository.GetByIdAsync(userId);

    public async Task<List<string>?> GetHobbiesById(UserId userId) => await _userRepository.GetHobbiesByIdAsync(userId);

    public async Task<IUser?> GetUserByUsernameAsync(string username)
    {
        IEnumerable<User> users = await _userRepository.GetAllAsync();
        return users.FirstOrDefault(u => u.Username == username);
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

    public async Task<UserId> RegisterUserAsync(
        string username,
        string password,
        UserFlags flags,
        UserTraits traits,
        List<string> hobbies
    )
    {
        if (!UsernameRegex().IsMatch(username))
        {
            _logger.LogWarning("Invalid username format: {username}", username);
            throw new InvalidUsernameException("Invalid username format");
        }

        if (!PasswordRegex().IsMatch(password))
        {
            _logger.LogWarning("Invalid password format");
            throw new InvalidPasswordException("Invalid password format");
        }

        UserId userId = UserId.From(Guid.NewGuid());

        traits.AvatarPath = User.GenerateGravatarUrl(userId);

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        User newUser = new(userId, username, hashedPassword, flags, traits, hobbies);
        await _userRepository.AddAsync(newUser);

        UserCounter.AddUser(newUser.Id);

        return userId;
    }

    public async Task UpdateAsync(IUser user)
    {
        User? userToUpdate = await _userRepository.GetByIdAsync(user.Id);
        if (userToUpdate != null)
        {
            await _userRepository.UpdateAsync(userToUpdate);
        }
    }

    public async Task<IUser?> GetRandomUserAsync()
    {
        IEnumerable<User> users = await _userRepository.GetAllAsync();

        IEnumerable<User> enumerable = users.ToList();
        if (!enumerable.Any())
        {
            return null;
        }

        Random random = new();
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

    public async Task<bool> IsUserNotSeenAnyUserAsync(UserId userId) =>
        await _userRepository.IsUserNotSeenAnyUserAsync(userId);

    public async Task<bool> IsUserSeenAsync(UserId userId, UserId otherUserId) =>
        await _userRepository.IsUserSeenAsync(userId, otherUserId);

    public async Task UserSeenAsync(UserId userId, UserId otherUserId) =>
        await _userRepository.UserSeenAsync(userId, otherUserId);

    [GeneratedRegex("^[A-Za-z0-9]+([A-Za-z0-9]*|[._-]?[A-Za-z0-9]+)*$")]
    private static partial Regex UsernameRegex();

    [GeneratedRegex(".{8,}")]
    private static partial Regex PasswordRegex();
}