namespace StudyBuddy.Extensions;

public static class StringExtensions
{
    public static string AddPossessiveSuffix(this string name)
    {
        string possessiveSuffix = name.EndsWith("s") ? "'" : "'s";
        return $"{name}{possessiveSuffix}";
    }
}
