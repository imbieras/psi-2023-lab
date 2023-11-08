using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace StudyBuddy.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder UseValueConverterForType<T>(this ModelBuilder modelBuilder, ValueConverter converter) =>
        modelBuilder.UseValueConverterForType(typeof(T), converter);

    public static ModelBuilder UseValueConverterForType(this ModelBuilder modelBuilder, Type type,
        ValueConverter converter)
    {
        foreach (IMutableEntityType? entityType in modelBuilder.Model.GetEntityTypes())
        {
            IEnumerable<PropertyInfo>? properties =
                entityType.ClrType.GetProperties().Where(p => p.PropertyType == type);
            foreach (PropertyInfo? property in properties)
            {
                modelBuilder.Entity(entityType.Name).Property(property.Name)
                    .HasConversion(converter);
            }
        }

        return modelBuilder;
    }
}
