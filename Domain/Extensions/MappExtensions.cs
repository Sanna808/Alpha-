using System.Reflection;

namespace Domain.Extentions;

public static class MappExtensions
{
    public static TDestination MapTo<TDestination>(this object source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        TDestination destination = Activator.CreateInstance<TDestination>()!;

        var sourceProperties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationProperties = destination.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var destinationProperty in destinationProperties)
        {
            var sourceProperty = sourceProperties.FirstOrDefault(x => x.Name == destinationProperty.Name && x.PropertyType == destinationProperty.PropertyType);
            if (sourceProperty != null && destinationProperty.CanWrite)
            {
                var value = sourceProperty.GetValue(source);
                destinationProperty.SetValue(destination, value);
            }
        }
        return destination;
    }

    //public static void MapTo<TSource, TDestination>(this TSource source, TDestination destination)
    //{
    //    ArgumentNullException.ThrowIfNull(source, nameof(source));
    //    ArgumentNullException.ThrowIfNull(destination, nameof(destination));

    //    var sourceProperties = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    //    var destinationProperties = typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance);

    //    foreach (var destProp in destinationProperties)
    //    {
    //        var srcProp = sourceProperties.FirstOrDefault(x =>
    //            x.Name == destProp.Name && x.PropertyType == destProp.PropertyType);
    //        if (srcProp != null && destProp.CanWrite)
    //        {
    //            var value = srcProp.GetValue(source);
    //            destProp.SetValue(destination, value);
    //        }
    //    }
    //}
}
