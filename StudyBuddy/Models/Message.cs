using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models
{
    public class Message
    {
        public UserId SenderId { get; set; }
        public string Text { get; set; }

        public DateTime Time { get; set; }
    }
}
