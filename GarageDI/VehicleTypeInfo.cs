namespace GarageDI;

/// <summary>
/// Represents metadata about a vehicle type discovered through reflection.
/// Owns the type, its display name, the filtered/ordered properties, and a factory
/// for creating instances – so callers never need to re-reflect on the same type.
/// </summary>
public class VehicleTypeInfo
{
    private readonly Func<IVehicle> _factory;

    public string Name { get; }
    public Type Type { get; }

    /// <summary>
    /// The filtered, ordered properties for this vehicle type.
    /// Use this instead of calling <see cref="PropertyService.GetProperties"/> directly.
    /// </summary>
    public PropertyInfo[] Properties { get; }

    public VehicleTypeInfo(Type type)
    {
        Type = type;
        Name = type.Name;
        _factory = () => (IVehicle)Activator.CreateInstance(type)!;
        Properties = type.GetFilteredProperties();
    }

    public IVehicle CreateInstance() => _factory();
}
