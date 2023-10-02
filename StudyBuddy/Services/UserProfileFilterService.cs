using StudyBuddy.Abstractions;

public class UserProfileFilterService
{
    private readonly IUserManager _userManager;

    public UserProfileFilterService(IUserManager userManager)
    {
        _userManager = userManager;
    }

    public List<IUser> FilterByBirthYear(int? startYear, int? endYear, List<IUser> userList)
    {
        var userQuery = from user in userList
                        where user.Traits.Birthdate.Year >= startYear && user.Traits.Birthdate.Year <= endYear
                        select user;

        var filteredUsers = userQuery.ToList();

        return filteredUsers;
    }
    public List<IUser> FilterBySubject(string? subject, List<IUser> userList)
    {
        var userQuery = from user in userList
                        where user.Traits.Subject.Equals(subject)
                        select user;

        var filteredUsers = userQuery.ToList();

        return filteredUsers;
    }

}
