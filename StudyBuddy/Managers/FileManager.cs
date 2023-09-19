using System.Globalization;
using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;
using CsvHelper.Configuration;
using CsvHelper;

namespace StudyBuddy.Managers
{
    public class FileManager
    {
        private readonly IUserManager _userManager;
        static bool loaded = false;
        public FileManager(IUserManager userManager)
        {
            _userManager = userManager;

            if (!loaded)
            {
                LoadUsersFromCsv("test.csv");
                loaded = true;
            }

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
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";", // Set the delimiter to ';'
                }))
                {
                    var records = csv.GetRecords<UserCsvRecord>().ToList();

                    foreach (var record in records)
                    {
                        var userId = UserId.From(record.Id);

                        // Check if a user with the same ID already exists
                        if (_userManager.GetUserById(userId) != null)
                        {
                            continue; // Skip
                        }

                        var username = record.Username;
                        var flags = Enum.TryParse(record.Flags, out UserFlags parsedFlags) ? parsedFlags : UserFlags.Registered;
                        var birthdate = record.Birthdate;
                        var subject = record.Subject;
                        var avatarPath = record.AvatarPath;
                        var traits = new UserTraits(birthdate, subject, avatarPath);

                        _userManager.RegisterUser(username, flags, traits);
                    }

                    Console.WriteLine($"Loaded {records.Count} users from the CSV file.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading users from CSV file: {ex.Message}");
            }
        }


    }


    public class UserCsvRecord
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Flags { get; set; }

        public DateTime Birthdate { get; set; }

        public string Subject { get; set; }

        public string AvatarPath { get; set; }
    }


}
