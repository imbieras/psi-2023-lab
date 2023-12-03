using System.Text.RegularExpressions;
using Markdig;
using StudyBuddy.API.Data.Repositories.UserRepository;
using StudyBuddy.API.Models;
using StudyBuddy.Shared.Abstractions;
using StudyBuddy.Shared.DTOs;
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

    public async Task<UserId> RegisterUserAsync(ProfileDto profileDto)
    {
        IUser? user = await GetUserByUsernameAsync(profileDto.Username);
        if (user != null)
        {
            _logger.LogWarning("Username already taken: {username}", profileDto.Username);
            throw new UsernameAlreadyTakenException("Username already taken");
        }

        if (!UsernameRegex().IsMatch(profileDto.Username))
        {
            _logger.LogWarning("Invalid username format: {username}", profileDto.Username);
            throw new InvalidUsernameException("Invalid username format");
        }

        if (profileDto.Password != profileDto.ConfirmPassword)
        {
            _logger.LogWarning("Passwords do not match");
            throw new InvalidPasswordException("Passwords do not match");
        }

        if (!PasswordRegex().IsMatch(profileDto.Password))
        {
            _logger.LogWarning("Invalid password format");
            throw new InvalidPasswordException("Invalid password format");
        }

        UserId userId = UserId.From(Guid.NewGuid());

        UserTraits traits = CreateUserTraits(profileDto, ConvertMarkdownToHtml(profileDto.MarkdownContent));
        traits.AvatarPath = User.GenerateGravatarUrl(userId);

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(profileDto.Password);

        User newUser = new(userId, profileDto.Username, hashedPassword, UserFlags.Registered, traits,
            profileDto.Hobbies);
        await _userRepository.AddAsync(newUser);

        UserCounter.AddUser(newUser.Id);

        return userId;
    }


    private static string ConvertMarkdownToHtml(string? markdownContent) =>
        markdownContent != null ? Markdown.ToHtml(markdownContent) : string.Empty;

    private static UserTraits CreateUserTraits(ProfileDto profileDto, string htmlContent)
    {
        UserTraits traits = new()
        {
            Birthdate = DateTime.Parse(profileDto.Birthdate).ToUniversalTime(),
            Subject = profileDto.Subject,
            Description = htmlContent
        };

        if (!double.TryParse(profileDto.Longitude, out double longitude) ||
            !double.TryParse(profileDto.Latitude, out double latitude))
        {
            return traits;
        }

        traits.Longitude = longitude;
        traits.Latitude = latitude;

        return traits;
    }

    public async Task UpdateAsync(UserId userId, UpdateUserDto updateUserDto)
    {
        User? userToUpdate = await _userRepository.GetByIdAsync(userId);
        if (userToUpdate != null)
        {
            updateUserDto.MarkdownContent = ConvertMarkdownToHtml(updateUserDto.MarkdownContent);
            await _userRepository.UpdateAsync(userId, updateUserDto);
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
