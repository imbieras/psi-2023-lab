using StudyBuddy.Extensions;

namespace StudyBuddyTests.Extensions;

public class StringExtensionsTests
{
    [InlineData("John", "John's")]
    [InlineData("Johns", "Johns'")]
    [Theory]
    public void AddPossessiveSuffix_ShouldAddCorrectSuffix(string name, string expected)
    {
        string actual = name.AddPossessiveSuffix();

        Assert.Equal(actual, expected);
    }
}
