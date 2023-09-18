using System.Globalization;
using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;


namespace StudyBuddy.Managers;

public class UserManager : IUserManager
{
    private static readonly List<User?> s_users = new();

    public IUser? GetUserById(UserId userId) => s_users.FirstOrDefault(u => u?.Id == userId);

    public IUser? GetUserByUsername(string username) => s_users.FirstOrDefault(u => u?.Name == username);

    public void BanUser(UserId userId)
    {
        User? user = s_users.FirstOrDefault(u => u?.Id == userId);
        if (user != null)
        {
            user.Flags |= UserFlags.Banned;
        }
    }

    public UserId RegisterUser(string username, UserFlags flags, UserTraits traits)
    {
        UserId userId = UserId.From(Guid.NewGuid());

        if (string.IsNullOrEmpty(traits.AvatarPath))
        {
            traits.AvatarPath = User.GenerateGravatarUrl(userId);
        }

        User newUser = new(userId, username, flags, traits);
        s_users.Add(newUser);

        return userId;
    }

    public List<IUser> GetAllUsers() => s_users.Where(u => u != null).Cast<IUser>().ToList();


    public void LoadUsersFromCsv(string filePath)
    {
        // Check if the file exists
        if (!File.Exists(filePath))
        {
            Console.WriteLine("The specified CSV file does not exist.");
            return;
        }

        try
        {
            var csvLines = File.ReadAllLines(filePath);

            // Skip the header row
            var userDataLines = csvLines.Skip(1);

            // Parse and populate users from the CSV data
            foreach (var line in userDataLines)
            {
                var fields = line.Split(';');

                if (fields.Length >= 6) // Make sure there are at least 6 fields
                {
                    var userId = UserId.From(Guid.Parse(fields[0]));

                    // Check if a user with the same ID already exists
                    if (s_users.Any(u => u?.Id == userId))
                    {
                        continue; // Skip this user if ID is already in the list
                    }

                    var username = fields[1];
                    var flags = UserFlags.Registered;

                    if (Enum.TryParse(fields[2], out UserFlags parsedFlags))
                    {
                        flags = parsedFlags;
                    }

                    var birthdate = DateTime.ParseExact(fields[3], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    var subject = fields[4];
                    var avatarPath = fields[5];

                    var traits = new UserTraits(birthdate, subject, avatarPath);
                    var newUser = new User(userId, username, flags, traits);
                    s_users.Add(newUser);
                }
            }

            Console.WriteLine($"Loaded {userDataLines.Count()} users from the CSV file.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading users from CSV file: {ex.Message}");
        }
    }

}
