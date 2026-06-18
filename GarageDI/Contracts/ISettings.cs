/// <summary>
/// Defines the contract for application settings.
/// </summary>
namespace GarageDI.Contracts
{
    public interface ISettings
    {
        /// <summary>
        /// Gets or sets the maximum number of vehicles the garage can hold.
        /// </summary>
        int Capacity { get; set; }
        /// <summary>
        /// Gets or sets the name of the garage.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Validates the settings to ensure all required values are present and valid.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if any setting is invalid.</exception>
        void Validate();
    }
}