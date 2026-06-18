using Newtonsoft.Json;

namespace GarageDI.DTOs;

/// <summary>
/// Data transfer object för garageinformation — används vid läsning/skrivning mot persistent lagring.
/// </summary>
public class GarageDTO
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public IEnumerable<IVehicle> Vehicles { get; set; } = [];

    /// <summary>
    /// Inställningar härledda från Name och Capacity — används när garaget laddas.
    /// </summary>
    [JsonIgnore]
    public ISettings Settings => new Settings { Name = Name, Capacity = Capacity };
}


