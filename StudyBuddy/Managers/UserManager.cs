using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Managers;

public class UserManager : IUserManager
{
    private static readonly List<User?> s_users = new();

    public IUser? GetUserById(UserId userId) => s_users.FirstOrDefault(u => u?.Id == userId);

    public IUser? GetUserByUsername(string username) => s_users.FirstOrDefault(u => u?.Name == username);

    public void BanUser(UserId userId)
    {
        User? user = s_users.FirstOrDefault(u => u?.Id == userId);
        if (user != null)
        {
            user.Flags |= UserFlags.Banned;
        }
    }

    public UserId RegisterUser(string username, UserFlags flags, UserTraits traits)
    {
        UserId userId = UserId.From(Guid.NewGuid());

        if (string.IsNullOrEmpty(traits.AvatarPath))
        {
            traits.AvatarPath = User.GenerateGravatarUrl(userId);
        }

        User newUser = new(userId, username, flags, traits);
        s_users.Add(newUser);

        return userId;
    }

    public List<IUser> GetAllUsers() => s_users.Where(u => u != null).Cast<IUser>().ToList();


    public IUser? GetRandomUser(IUser user)
    {
        Random random = new Random();
        int maxTries = s_users.Count() - user.UsedIndexes.Count;

        if (user == null || user.UsedIndexes == null || user.UsedIndexes.Count == s_users.Count())
            return null;

        for (int i = 0; i < maxTries; i++)
        {
            int currentIndex;

            do
            {
                currentIndex = random.Next(0, s_users.Count);
            }
            while (user.UsedIndexes.Contains(currentIndex));


            if (s_users[currentIndex].Id != user.Id )
            {
                user.UsedIndexes.Add(currentIndex);

                return s_users[currentIndex];
            }
        }

        return null;
    }

    public IUser? GetPreviousRandomProfile(IUser user)
    {
        if (user.UsedIndexes.Count >= 2)
        {
            int previousIndex = user.UsedIndexes[user.UsedIndexes.Count - 2];

            if (previousIndex >= 0 && previousIndex < s_users.Count)
            {
                return s_users[previousIndex];
            }
        }
        else if (user.UsedIndexes.Count == 1)
        {
            int lastIndex = user.UsedIndexes.Last();
            if (lastIndex >= 0 && lastIndex < s_users.Count)
            {
                return s_users[lastIndex];
            }
        }

        return null;
    }

    public IUser? GetCurrentRandomUser(IUser user)
    {
        return s_users[user.UsedIndexes.Last()];
    }

    public bool IsUsedIndexesEmpty(IUser user)
    {
        return (user.UsedIndexes.Count > 0) ? true : false;
    }

}
