using System.ComponentModel.DataAnnotations;

namespace StudyBuddy.API.Services;

public static class GenericFilterService<T> where T : class, IValidatableObject
{
    public static List<T> FilterByPredicate(IEnumerable<T> items, Func<T, bool> predicate) =>
        items.Where(predicate).ToList();

    public static IEnumerable<T> FilterInvalidItems(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            var validationContext = new ValidationContext(item);
            if (Validator.TryValidateObject(item, validationContext, null, true))
            {
                yield return item;
            }
        }
    }
}
