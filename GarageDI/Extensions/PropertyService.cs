using System.Reflection;
using GarageDI.Attributes;
using GarageDI.Entities;

namespace GarageDI.Extensions;

/// <summary>
/// Extension methods on <see cref="Type"/> for retrieving filtered, ordered vehicle properties.
/// </summary>
internal static class PropertyService
{
    private static readonly Dictionary<Type, PropertyInfo[]> _propertyCache = new();

    /// <summary>
    /// Returns all public properties for <paramref name="type"/> that are not marked
    /// with <see cref="Exclude"/>, ordered by <see cref="Order"/> (default 100).
    /// Results are cached after the first call.
    /// </summary>
    public static PropertyInfo[] GetFilteredProperties(this Type type)
    {
        if (_propertyCache.TryGetValue(type, out var cached))
            return cached;

        var properties = type
            .GetProperties()
            .Where(p => !p.IsDefined(typeof(Exclude), true))
            .OrderBy(p => p.GetCustomAttribute<Order>()?.Value ?? 100)
            .ToArray();

        _propertyCache[type] = properties;
        return properties;
    }
}
