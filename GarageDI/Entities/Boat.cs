namespace GarageDI.Entities;

/// <summary>
/// Represents a boat in the garage system.
/// </summary>
public class Boat : Vehicle
{
    /// <summary>
    /// Gets or sets the length of the boat.
    /// </summary>
    public required int Length { get; set; }
   
}
