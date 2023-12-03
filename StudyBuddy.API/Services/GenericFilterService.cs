using System.ComponentModel.DataAnnotations;
using StudyBuddy.Shared.Abstractions;

namespace StudyBuddy.API.Services;

public delegate bool UserPredicate(IUser user);

public static class GenericFilterService<T> where T : class, IValidatableObject
{
    public static List<T> FilterByPredicate(IEnumerable<T> items, UserPredicate predicate) =>
        items.Where(item => predicate((IUser)item)).ToList();

    public static IEnumerable<T> FilterInvalidItems(IEnumerable<T> items)
    {
        foreach (T? item in items)
        {
            ValidationContext? validationContext = new ValidationContext(item);
            if (Validator.TryValidateObject(item, validationContext, null, true))
            {
                yield return item;
            }
        }
    }
}
