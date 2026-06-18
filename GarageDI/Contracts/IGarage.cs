namespace GarageDI.Contracts;

/// <summary>
/// Defines the contract for a generic garage that can store vehicles.
/// </summary>
/// <typeparam name="T">The type of vehicle to store in the garage.</typeparam>
public interface IGarage<T> : IEnumerable<T> where T : IVehicle
{
    /// <summary>
    /// Gets a value indicating whether the garage is full.
    /// </summary>
    bool IsFull { get; }
    /// <summary>
    /// Gets the name of the garage.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Parks a vehicle in the garage.
    /// </summary>
    /// <param name="vehicle">The vehicle to park.</param>
    /// <returns>True if the vehicle was parked successfully, false otherwise.</returns>
    bool Park(T vehicle);
    /// <summary>
    /// Removes a vehicle from the garage.
    /// </summary>
    /// <param name="vehicle">The vehicle to remove.</param>
    /// <returns>True if the vehicle was removed successfully, false otherwise.</returns>
    bool Leave(T vehicle);
    IGarage<T> CreateNew(ISettings settings);
}
