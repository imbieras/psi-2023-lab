﻿using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Managers
{
    public class FileManager
    {
        private readonly IUserManager _userManager;
        private readonly bool _loaded;
        private const string CsvDelimiter = ";";

        public FileManager(IUserManager userManager)
        {
            _userManager = userManager;

            if (_loaded)
            {
                LoadUsersFromCsv("test.csv");
                _loaded = true;
            }
        }

        public void LoadUsersFromCsv(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("The specified CSV file does not exist.");
                return;
            }

            try
            {
                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = CsvDelimiter,
                    PrepareHeaderForMatch = args => args.Header.ToLower()
                });
                var records = csv.GetRecords<UserCsvRecord>().ToList();

                foreach (var record in records)
                {
                    var userId = UserId.From(record.Id);
                    if (_userManager.GetUserById(userId) != null)
                    {
                        continue;
                    }

                    string username = record.Username;
                    var flags = Enum.TryParse(record.Flags, out UserFlags parsedFlags) ? parsedFlags : UserFlags.Registered;
                    var birthdate = record.Birthdate;
                    string subject = record.Subject;
                    string avatarPath = record.AvatarPath;
                    string description = record.Description;
                    var hobbies = new List<string>(record.Hobbies.Split(','));

                    var traits = new UserTraits(birthdate, subject, avatarPath, description, hobbies);

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
        public UserCsvRecord(Guid id, string username, string flags, string subject, string avatarPath, string description, string hobbies)
        {
            Id = id;
            Username = username;
            Flags = flags;
            Subject = subject;
            AvatarPath = avatarPath;
            Description = description;
            Hobbies = hobbies;
        }

        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Flags { get; set; }
        public DateTime Birthdate { get; set; }
        public string Subject { get; set; }
        public string AvatarPath { get; set; }
        public string Description { get; set; }
        public string Hobbies { get; set; }
    }
}
