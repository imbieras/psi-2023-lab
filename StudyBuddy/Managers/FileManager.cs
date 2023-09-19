using System.Globalization;
using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

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
                        if (_userManager.GetUserById(userId) != null)
                        {
                            continue; // Skip this user if ID is already in UserManager
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
                        _userManager.RegisterUser(username, flags, traits);
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
}
