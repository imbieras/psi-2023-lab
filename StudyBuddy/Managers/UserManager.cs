using StudyBuddy.Abstractions;
using StudyBuddy.ValueObjects;
using StudyBuddy.Models;

namespace StudyBuddy.Managers;

public class UserManager : IUserManager
{
    private static readonly List<User?> _users = new();

    public UserId RegisterUser(string username, UserFlags flags, DateTime birthdate, string subject, string avatarFileName)
    {
        var userId = UserId.From(Guid.NewGuid());
        var newUser = new User(userId, username, flags, birthdate, subject, avatarFileName);
        _users.Add(newUser);
        Console.WriteLine("Added" +_users.Count());
        return userId;
    }

    public IUser? GetUserById(UserId userId)
    {
        return _users.FirstOrDefault(u => u?.Id == userId);
    }

    public IUser? GetUserByUsername(string username)
    {
        return _users.FirstOrDefault(u => u?.Name == username);
    }

    public void BanUser(UserId userId)
    {
        var user = _users.FirstOrDefault(u => u?.Id == userId);
        if (user != null)
        {
            user.Flags |= UserFlags.Banned;
        }
    }

   public List<User?> GetUserList() {
       // RegisterUser("lex",UserFlags.Admin,DateTime.Now,"Natural science","avatar1.jpg"); //Test User creation (Delete later)
       // RegisterUser("lex2", UserFlags.Registered, DateTime.Today, "KA", "avatar2.jpg"); //Test User creation (Delete Later)
        // RegisterUser("Tomashas Lavashas3", UserFlags.Registered, DateTime.Today, "KA", "avatar3.jpg"); //Test User creation (Delete Later)
        return _users;
    }

   public int GetListSize()
   {
       return _users.Count;
   }


}
