namespace StudyBuddy.Services;

public static class GenericFilterService<T>
{
    public static List<T> FilterByPredicate(IEnumerable<T> items, Func<T, bool> predicate) =>
        items.Where(predicate).ToList();
}
