namespace GarageDI.Contracts;
public interface IVehicleTypeFindService
{
    IReadOnlyList<VehicleTypeInfo> AvailableTypes { get; }
}