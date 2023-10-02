using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using StudyBuddy.Abstractions;
using StudyBuddy.Models;

namespace StudyBuddy.Managers;

public class FileManager
{
    private readonly IUserManager _userManager;
    private const string CsvDelimiter = ";";

    public FileManager(IUserManager userManager) => _userManager = userManager;

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
            using CsvReader csv = new(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = CsvDelimiter, PrepareHeaderForMatch = args => args.Header.ToLower() });

            List<UserCsvRecord> records = csv.GetRecords<UserCsvRecord>().ToList();

            foreach (UserCsvRecord? record in records)
            {
                string username = record.Username;
                UserFlags flags = Enum.TryParse(record.Flags, out UserFlags parsedFlags) ? parsedFlags : UserFlags.Registered;
                DateTime birthdate = record.Birthdate;
                string subject = record.Subject;
                string avatarPath = record.AvatarPath;
                string description = record.Description;
                List<string> hobbies = new(record.Hobbies.Split(','));

                UserTraits traits = new()
                {
                    Birthdate = birthdate,
                    Subject = subject,
                    AvatarPath = avatarPath,
                    Description = description,
                    Hobbies = hobbies,
                    Location = "N/A"
                };

                _userManager.RegisterUser(username, flags, traits);
            }

            Console.WriteLine($"Loaded {records.Count} users from the CSV file.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading users from CSV file: {ex.Message}");
        }
    }
}

public class UserCsvRecord
{
    public UserCsvRecord(string username, string flags, string subject, string avatarPath, string description, string hobbies, DateTime birthdate)
    {
        Username = username;
        Flags = flags;
        Subject = subject;
        AvatarPath = avatarPath;
        Description = description;
        Hobbies = hobbies;
        Birthdate = birthdate;
    }

    public string Username { get; }

    public string Flags { get; }

    public DateTime Birthdate { get; }

    public string Subject { get; }

    public string AvatarPath { get; }

    public string Description { get; }

    public string Hobbies { get; }
}
