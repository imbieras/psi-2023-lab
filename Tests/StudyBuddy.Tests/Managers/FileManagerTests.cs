using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using NSubstitute;
using StudyBuddy.Abstractions;
using StudyBuddy.Managers.FileManager;
using StudyBuddy.Models;
using StudyBuddy.Services.UserService;

namespace StudyBuddyTests.Managers;

public class FileManagerTests
{
    private readonly IUserService _userService;
    private readonly FileManager _sut;

    public FileManagerTests()
    {
        _userService = Substitute.For<IUserService>();
        _sut = new FileManager(_userService);
    }

    [Fact]
    public void LoadUsersFromCsv_SingleUserFileWithMockData_Loads_SingleUser()
    {
        const string filePath = "single_user_mock_test.csv";
        CreateCsvFile(filePath, new List<UserCsvRecord>
        {
            new(
            "Test1_Test1",
            "Registered",
            DateTime.Parse("2008-09-14"),
            "Arts & Design",
            "https://img.freepik.com/premium-photo/male-werewolf-with-head-wolf-dark-foggy-forest-generative-ai-illustration_118086-7239.jpg",
            "Hello hehe",
            "Photography"
            )
        });

        _sut.LoadUsersFromCsv(filePath);

        _userService.Received().RegisterUserAsync(
        Arg.Any<string>(),
        Arg.Any<UserFlags>(),
        Arg.Any<UserTraits>(),
        Arg.Any<List<string>>()
        );

        File.Delete(filePath);
    }

    [Fact]
    public void LoadUsersFromCsv_MultipleUsersFileWithMockData_Loads_MultipleUsers()
    {
        const string filePath = "multiple_users_mock_test.csv";
        CreateCsvFile(filePath, new List<UserCsvRecord>
        {
            new(
            "Test1_Test1",
            "Registered",
            DateTime.Parse("2008-09-14"),
            "Arts & Design",
            "https://img.freepik.com/premium-photo/male-werewolf-with-head-wolf-dark-foggy-forest-generative-ai-illustration_118086-7239.jpg",
            "Hello hehe",
            "Photography"
            ),
            new(
            "Test2_Test2",
            "Registered",
            DateTime.Parse("2007-09-05"),
            "Natural Sciences",
            "https://cdnb.artstation.com/p/assets/images/images/063/783/949/large/antonio-j-manzanedo-werewolf-manzanedo-3.jpg?1686332788",
            "aaaaaa",
            "Gardening, Cooking"
            ),
            new(
            "Test3_Test3",
            "Registered",
            DateTime.Parse("2006-08-28"),
            "Law & Legal Studies",
            "https://cdn.images.express.co.uk/img/dynamic/80/590x/Werewolf-1056224.jpg?r=1544355660150",
            "fskjdnfksjdnf",
            "Dancing, Cooking"
            )
        });

        _sut.LoadUsersFromCsv(filePath);

        _userService.Received(3).RegisterUserAsync(
        Arg.Any<string>(),
        Arg.Any<UserFlags>(),
        Arg.Any<UserTraits>(),
        Arg.Any<List<string>>()
        );

        File.Delete(filePath);
    }

    [Fact]
    public void LoadUsersFromCsv_NonExistentFile_DoesNotLoad_Users()
    {
        const string filePath = "nonexistent_test.csv";

        _sut.LoadUsersFromCsv(filePath);

        _userService.DidNotReceive().RegisterUserAsync(
        Arg.Any<string>(),
        Arg.Any<UserFlags>(),
        Arg.Any<UserTraits>(),
        Arg.Any<List<string>>()
        );
    }

    private static void CreateCsvFile(string filePath, IEnumerable<UserCsvRecord> records)
    {
        using StreamWriter writer = new(filePath);
        using CsvWriter csv = new(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = FileManager.CsvDelimiter, PrepareHeaderForMatch = args => args.Header.ToLower() });
        csv.WriteRecords(records);
    }
}
