using Newtonsoft.Json;

namespace GarageDI.Services
{
    /// <summary>
    /// Provides methods for saving and loading garage data to and from JSON files.
    /// </summary>
    public class GaragePersistenceService : IGaragePersistenceService
    {

        private readonly JsonSerializerSettings _jsonSettings;

        public GaragePersistenceService()
        {
            _jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented
            };
        }

        /// <summary>
        /// Saves the garage data to a JSON file.
        /// </summary>
        /// <param name="garageName">The name of the garage.</param>
        /// <param name="capacity">The capacity of the garage.</param>
        /// <param name="vehicles">The vehicles to save.</param>
        public void SaveGarage(string garageName, int capacity, IEnumerable<IVehicle> vehicles)
        {
            string filePath = GetFilePath(garageName);

            var garageDto = new GarageDTO
            {
                Capacity = capacity,
                Name = garageName,
                Vehicles = vehicles
            };

            try
            {
                using (StreamWriter streamWriter = File.CreateText(filePath))
                {
                    var serializer = JsonSerializer.Create(_jsonSettings);
                    serializer.Serialize(streamWriter, garageDto);

                }
                //För att visa hur man kan använda debug writeline under utveckling kompileras inte in in release mode!
                Debug.WriteLine($"JSON data successfully written to {garageName}.json");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error writing JSON to {garageName}.json: {ex.Message}");
            }

        }

        /// <summary>
        /// Loads the garage data from a JSON file.
        /// </summary>
        /// <param name="garageName">The name of the garage to load.</param>
        /// <returns>The loaded GarageDTO.</returns>
        public GarageDTO LoadGarage(string garageName)
        {
            string filePath = GetFilePath(garageName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file for the garage '{garageName}' does not exist.");
            }

            using (StreamReader streamReader = File.OpenText(filePath))
            {
                var serializer = JsonSerializer.Create(_jsonSettings);
                var garageDto = (GarageDTO?)serializer.Deserialize(streamReader, typeof(GarageDTO));
                ArgumentNullException.ThrowIfNull(garageDto, nameof(garageDto));

                garageDto.Settings.Validate();
           
                return garageDto;
            }
        }

        private string GetFilePath(string garageName)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{garageName}.json");
        }

    }
}

