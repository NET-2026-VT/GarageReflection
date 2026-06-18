namespace GarageDI.Entities;

/// <summary>
/// Represents a bus in the garage system.
/// </summary>
public class Buss : Vehicle
{
    /// <summary>
    /// Gets or sets the number of seats in the bus.
    /// </summary>
    public required int Seats { get; set; }
    
}
