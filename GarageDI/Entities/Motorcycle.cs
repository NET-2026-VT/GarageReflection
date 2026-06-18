namespace GarageDI.Entities;

/// <summary>
/// Represents a motorcycle in the garage system.
/// </summary>
public class Motorcycle : Vehicle
{
    /// <summary>
    /// Gets or sets the cylinder volume of the motorcycle.
    /// </summary>
    [Beautify("Cylinder volume")]
    public required int CylinderVolume { get; set; }
}
