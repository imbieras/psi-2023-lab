using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models
{
    public class UserTraits
    {
        public DateTime Birthdate { get; set; }

        public string Subject { get; set; }

        public string AvatarPath { get; set; }

        public string Description { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public UserTraits() { } // Parameterless constructor for EF

        public UserTraits(
            DateTime birthdate,
            string subject,
            string avatarPath,
            string description,
            Coordinates location
        )
        {
            Birthdate = birthdate;
            Subject = subject;
            AvatarPath = avatarPath;
            Description = description;
            Latitude = location.Value.Item1;
            Longitude = location.Value.Item2;
        }
    }
}
