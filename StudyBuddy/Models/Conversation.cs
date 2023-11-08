using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models;

public class Conversation
{
    public Guid Id { get; set; }
    public UserId User1Id { get; set; }
    public UserId User2Id { get; set; }

    public Conversation(UserId user1Id, UserId user2Id)
    {
        User1Id = user1Id;
        User2Id = user2Id;
    }

    public Conversation()
    {
    }
}
