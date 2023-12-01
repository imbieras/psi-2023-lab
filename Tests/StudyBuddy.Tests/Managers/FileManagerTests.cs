using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using NSubstitute;
using StudyBuddy.API.Managers.FileManager;
using StudyBuddy.API.Services.UserService;
using StudyBuddy.Shared.Abstractions;
using StudyBuddy.Shared.Models;

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
            (string)"Test1_Test1",
            (string)"LCwo7cc*NALmtXq&F**PWu#",
            (string)"Registered",
            DateTime.Parse("2008-09-14"),
            (string)"Arts & Design",
            (string)"https://img.freepik.com/premium-photo/male-werewolf-with-head-wolf-dark-foggy-forest-generative-ai-illustration_118086-7239.jpg",
            (string)"Hello hehe",
            (string)"Photography"
            )
        });

        _sut.LoadUsersFromCsv(filePath);

        _userService.Received().RegisterUserAsync(
        Arg.Any<string>(),
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
            (string)"Test1_Test1",
            (string)"LCwo7cc*NALmtXq&F**PWu#",
            (string)"Registered",
            DateTime.Parse("2008-09-14"),
            (string)"Arts & Design",
            (string)"https://img.freepik.com/premium-photo/male-werewolf-with-head-wolf-dark-foggy-forest-generative-ai-illustration_118086-7239.jpg",
            (string)"Hello hehe",
            (string)"Photography"
            ),
            new(
            (string)"Test2_Test2",
            (string)"HQkA*NMvtnbwVz6iB#r^v7%",
            (string)"Registered",
            DateTime.Parse("2007-09-05"),
            (string)"Natural Sciences",
            (string)"https://cdnb.artstation.com/p/assets/images/images/063/783/949/large/antonio-j-manzanedo-werewolf-manzanedo-3.jpg?1686332788",
            (string)"aaaaaa",
            (string)"Gardening, Cooking"
            ),
            new(
            (string)"Test3_Test3",
            (string)"5RooF5o8b3YCvUgr$^p#FQZ",
            (string)"Registered",
            DateTime.Parse("2006-08-28"),
            (string)"Law & Legal Studies",
            (string)"https://cdn.images.express.co.uk/img/dynamic/80/590x/Werewolf-1056224.jpg?r=1544355660150",
            (string)"fskjdnfksjdnf",
            (string)"Dancing, Cooking"
            )
        });

        _sut.LoadUsersFromCsv(filePath);

        _userService.Received(3).RegisterUserAsync(
        Arg.Any<string>(),
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
