namespace StudyBuddy.Services;

public static class GenericFilterService<T>
{
    public static List<T> FilterByPredicate(IEnumerable<T> items, Func<T, bool> predicate)
    {
        return items.Where(predicate).ToList();
    }
}
