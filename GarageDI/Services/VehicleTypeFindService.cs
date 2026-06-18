using System.Reflection;

namespace GarageDI.Services;


public class VehicleTypeFindService : IVehicleTypeFindService
{
    private readonly IReadOnlyList<VehicleTypeInfo> _vehicleTypes = DiscoverVehicleTypes();

    /// <summary>
    /// Gets all available vehicle types discovered in the assembly.
    /// </summary>
    public IReadOnlyList<VehicleTypeInfo> AvailableTypes => _vehicleTypes;

    /// <summary>
    /// Discovers all concrete vehicle types that implement IVehicle.
    /// </summary>
    private static List<VehicleTypeInfo> DiscoverVehicleTypes()
    {
        var vehicleType = typeof(IVehicle);
        var assembly = vehicleType.Assembly;

        return assembly.GetTypes()
            .Where(t => t.IsClass &&
                       !t.IsAbstract &&
                       vehicleType.IsAssignableFrom(t))
            .Select(t => new VehicleTypeInfo(t))
            .OrderBy(vt => vt.Name)
            .ToList();
    }
}
