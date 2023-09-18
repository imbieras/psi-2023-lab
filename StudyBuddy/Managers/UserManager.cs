using System.Diagnostics;
using System.Formats.Asn1;
using System.Globalization;
using System.Text;
using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    public void WriteUsersToCsv(string filePath)
    {
        // Filter out null users and select necessary user information with traits
        var usersToExport = s_users.Where(u => u != null)
                                   .Select(u => new
                                   {
                                       Id = u.Id.ToString(),
                                       Username = u.Name,
                                       Flags = u.Flags.ToString(),
                                       Birthdate = u.Traits.Birthdate.ToString("yyyy-MM-dd"),
                                       Subject = u.Traits.Subject,
                                       AvatarPath = u.Traits.AvatarPath
                                   });

        // Create a CSV content string
        StringBuilder csvContent = new StringBuilder();
        csvContent.AppendLine("Id;Username;Flags;Birthdate;Subject;AvatarPath");

        foreach (var user in usersToExport)
        {
            csvContent.AppendLine($"{user.Id};{user.Username};{user.Flags};{user.Birthdate};{user.Subject};{user.AvatarPath}");
        }

        // Write the CSV content to the specified file
        File.WriteAllText(filePath, csvContent.ToString());
    }


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
            // Read all lines from the CSV file
            var csvLines = File.ReadAllLines(filePath);

            // Skip the header row
            var userDataLines = csvLines.Skip(1);

            // Parse and populate users from the CSV data
            foreach (var line in userDataLines)
            {
                var fields = line.Split(';');

                if (fields.Length >= 5) // Make sure there are at least 6 fields
                {
                    var userId = UserId.From(Guid.Parse(fields[0]));
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
