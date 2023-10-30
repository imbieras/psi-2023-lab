using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models;

public class UserHobby
{
    public UserHobby(UserId userId, User user, int hobbyId, Hobby hobby)
    {
        UserId = userId;
        User = user;
        HobbyId = hobbyId;
        Hobby = hobby;
    }

    public UserHobby()
    {
        throw new NotImplementedException();
    }

    public UserId UserId { get; set; }
    public User User { get; set; } // Navigation property

    public int HobbyId { get; set; }
    public Hobby Hobby { get; set; } // Navigation property
}
