namespace GarageDI.Contracts;

/// <summary>
/// Defines the contract for handling garage operations.
/// </summary>
public interface IGarageHandler
{
    /// <summary>
    /// Gets a value indicating whether the garage is full.
    /// </summary>
    bool IsGarageFull { get; }

    /// <summary>
    /// Parks a vehicle in the garage.
    /// </summary>
    /// <param name="v">The vehicle to park.</param>
    /// <returns>True if the vehicle was parked successfully, false otherwise.</returns>
    bool Park(IVehicle v);

    /// <summary>
    /// Gets all vehicles currently parked in the garage.
    /// </summary>
    /// <returns>A list of all parked vehicles.</returns>
    IEnumerable<string> GetVehicleInfo();

    /// <summary>
    /// Gets the count of vehicles by type.
    /// </summary>
    /// <returns>A list of vehicle counts by type.</returns>
    List<VehicleCountDTO> GetByType();

    /// <summary>
    /// Uppmanar användaren att fylla i alla egenskaper för ett nytt fordon.
    /// </summary>
    /// <param name="typeInfo">Metadata om fordonstypen.</param>
    /// <param name="regNoValidator">Funktion som validerar registreringsnumret — returnerar true om det är giltigt.</param>
    IVehicle GetVehicle(VehicleTypeInfo typeInfo, Func<string, bool> regNoValidator);

    /// <summary>
    /// Searches for vehicles matching the given criteria.
    /// </summary>
    /// <param name="dto">The search criteria DTO.</param>
    /// <returns>An enumerable of matching vehicles.</returns>
    IEnumerable<IVehicle> SearchVehicle(VehicleTypeInfo? typeInfo);

    /// <summary>
    /// Gets a vehicle by its registration number.
    /// </summary>
    /// <param name="regNo">The registration number to search for.</param>
    /// <returns>The vehicle if found, null otherwise.</returns>
    IVehicle? Get(string regNo);

    /// <summary>
    /// Removes a vehicle from the garage by its registration number.
    /// </summary>
    /// <param name="regNo">The registration number of the vehicle to remove.</param>
    /// <returns>True if the vehicle was removed successfully, false otherwise.</returns>
    bool Leave(string regNo);

    /// <summary>
    /// Laddar ett garage från persistent lagring.
    /// </summary>
    void Load();
}
