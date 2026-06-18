namespace GarageDI.Utils;

/// <summary>
/// Represents application settings for the garage.
/// </summary>
class Settings : ISettings
{
    /// <summary>
    /// The maximum number of vehicles the garage can hold.
    /// </summary>
    public int Capacity { get; set; }
    /// <summary>
    /// The name of the garage.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Validates the settings to ensure all required values are present and valid.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if any setting is invalid.</exception>
    public void Validate()
    {
        if (Capacity <= 0)
            throw new ArgumentException("Garage size must be greater than zero.");
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Garage name must be provided.");
    }
}

