namespace GarageDI.Contracts
{
    /// <summary>
    /// Defines methods for persisting and loading garage data.
    /// </summary>
    public interface IGaragePersistenceService
    {
        /// <summary>
        /// Loads a garage from persistent storage.
        /// </summary>
        /// <param name="garageName">The name of the garage to load.</param>
        /// <returns>The loaded GarageDTO.</returns>
        GarageDTO LoadGarage(string garageName);
        /// <summary>
        /// Saves a garage to persistent storage.
        /// </summary>
        /// <param name="garageName">The name of the garage to save.</param>
        /// <param name="capacity">The capacity of the garage.</param>
        /// <param name="vehicles">The vehicles to save.</param>
        void SaveGarage(string garageName, int capacity, IEnumerable<IVehicle> vehicles);
    }
}

