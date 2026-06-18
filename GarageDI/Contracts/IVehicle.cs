namespace GarageDI.Contracts;

/// <summary>
/// Defines the contract for all vehicle types in the garage system.
/// </summary>
public interface IVehicle
{
    /// <summary>
    /// Gets or sets the registration number of the vehicle.
    /// </summary>
    string RegNo { get; set; }
    /// <summary>
    /// Gets or sets the color of the vehicle.
    /// </summary>
    string Color { get; set; }
    /// <summary>
    /// Gets or sets a property value by name.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <returns>The value of the property.</returns>
    object this[string name] { get; set; }
    /// <summary>
    /// Gets a string representation of the vehicle's information.
    /// </summary>
    /// <returns>A formatted string containing the vehicle's details.</returns>
    string GetInfo();
}