using StudyBuddy.Abstractions;

namespace StudyBuddy.Services;

public class UserProfileFilterService
{
    public static List<IUser> FilterByBirthYear(int? startYear, int? endYear, IEnumerable<IUser> userList)
    {
        IEnumerable<IUser> userQuery = from user in userList
            where user.Traits.Birthdate.Year >= startYear && user.Traits.Birthdate.Year <= endYear
            select user;

        List<IUser> filteredUsers = userQuery.ToList();

        return filteredUsers;
    }
    public static List<IUser> FilterBySubject(string? subject, IEnumerable<IUser> userList)
    {
        IEnumerable<IUser> userQuery = from user in userList
            where user.Traits.Subject.Equals(subject)
            select user;

        List<IUser> filteredUsers = userQuery.ToList();

        return filteredUsers;
    }
}
