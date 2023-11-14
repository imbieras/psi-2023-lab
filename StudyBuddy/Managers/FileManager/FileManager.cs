using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using StudyBuddy.Services.UserService;

namespace StudyBuddy.Managers.FileManager;

public class FileManager
{
    public const string CsvDelimiter = ";";
    private readonly IUserService _userService;

    public FileManager(IUserService userService) => _userService = userService;

    public void LoadUsersFromCsv(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("The specified CSV file does not exist.");
            return;
        }

        try
        {
            using StreamReader reader = new(filePath);
            using CsvReader csv = new(reader,
                new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = CsvDelimiter, PrepareHeaderForMatch = args => args.Header.ToLower()
                });

            List<UserCsvRecord> records = csv.GetRecords<UserCsvRecord>().ToList();

            foreach (UserCsvRecord? record in records)
            {
                string username = record.Username;
                UserFlags flags = Enum.TryParse(record.Flags, out UserFlags parsedFlags)
                    ? parsedFlags
                    : UserFlags.Registered;
                DateTime birthdate = record.Birthdate;
                string subject = record.Subject;
                string avatarPath = record.AvatarPath;
                string description = record.Description;
                List<string> hobbies = record.Hobbies.Split(',')
                    .Select(hobby => hobby.Trim())
                    .ToList();

                UserTraits traits = new()
                {
                    Birthdate = birthdate, Subject = subject, AvatarPath = avatarPath, Description = description
                };

                _userService.RegisterUserAsync(username, "password123",flags, traits, hobbies);
            }

            Console.WriteLine($"Loaded {records.Count} users from the CSV file.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading users from CSV file: {ex.Message}");
        }
    }
}

public record UserCsvRecord(
    string Username,
    string Flags,
    DateTime Birthdate,
    string Subject,
    string AvatarPath,
    string Description,
    string Hobbies
)
{
    public string Username { get; } = Username;

    public string Flags { get; } = Flags;

    public DateTime Birthdate { get; } = Birthdate;

    public string Subject { get; } = Subject;

    public string AvatarPath { get; } = AvatarPath;

    public string Description { get; } = Description;

    public string Hobbies { get; } = Hobbies;
}
