using GarageDI.DTOs;
using System.Diagnostics.CodeAnalysis;

namespace GarageDI.Garage;

/// <summary>
/// Handles garage operations such as parking, unparking, searching, and loading vehicles.
/// </summary>
internal class GarageHandler : IGarageHandler
{
    private IGarage<IVehicle> garage;
    private readonly IUtil util;
    private readonly IGaragePersistenceService persitiance;

    public GarageHandler(IGarage<IVehicle> garage, IUtil util, IGaragePersistenceService persitiance)
    {
        this.garage = garage;
        this.util = util;
        this.persitiance = persitiance;
    }

    /// <summary>
    /// Returns true if the garage is full.
    /// </summary>
    public bool IsGarageFull => garage.IsFull;

    /// <summary>
    /// Gets all vehicles information currently parked in the garage.
    /// </summary>
    /// <returns>List of strings of all parked vehicles Info.</returns>
    public IEnumerable<string> GetVehicleInfo()
    {
        return garage.Select(v => v.GetInfo()).ToList();
    }

    /// <summary>
    /// Gets the count of vehicles by type.
    /// </summary>
    /// <returns>List of vehicle counts by type.</returns>
    public List<VehicleCountDTO> GetByType()
    {
        return garage.GroupBy(v => v.GetType().Name)
                     .Select(v => new VehicleCountDTO(v.Key, v.Count()))
                     .ToList();
    }

    /// <summary>
    /// Uppmanar användaren att fylla i alla egenskaper för ett nytt fordon.
    /// Registreringsnumret valideras med den angivna funktionen — frågar om igen tills det godkänns.
    /// </summary>
    public IVehicle GetVehicle(VehicleTypeInfo typeInfo, Func<string, bool> regNoValidator)
    {
        var vehicle = typeInfo.CreateInstance();

        foreach (var prop in typeInfo.Properties)
        {
            switch (Type.GetTypeCode(prop.PropertyType))
            {
                case TypeCode.Int32:
                    prop.SetValue(vehicle, util.AskForInt(prop.GetDisplayText()));
                    break;
                case TypeCode.String:
                    if (prop.Name == nameof(IVehicle.RegNo))
                    {
                        string regNo;
                        do
                        {
                            regNo = util.AskForString(prop.GetDisplayText()).Trim();
                        } while (string.IsNullOrWhiteSpace(regNo) || !regNoValidator(regNo));

                        vehicle[prop.Name] = regNo;
                    }
                    else
                    {
                        vehicle[prop.Name] = util.AskForString(prop.GetDisplayText());
                    }
                    break;
            }
        }

        return vehicle;
    }

    /// <summary>
    /// Searches for vehicles matching the given type and property criteria.
    /// The user is prompted for each property; entering "X" skips that filter.
    /// <para>
    /// Imperativ stil: resultatet filtreras steg för steg i en foreach-loop.
    /// Jämför med den deklarativa varianten <see cref="SearchVehicleDeclarative"/>.
    /// </para>
    /// </summary>
    public IEnumerable<IVehicle> SearchVehicle(VehicleTypeInfo? typeInfo)
    {
        var properties = typeInfo is null
            ? typeof(Vehicle).GetFilteredProperties()
            : typeInfo.Properties;

        IEnumerable<IVehicle> result = typeInfo is null
            ? garage
            : garage.Where(v => v.GetType() == typeInfo.Type);

        foreach (var prop in properties)
        {
            var searchWord = util.AskForString(prop.GetDisplayText()).ToUpper();

            if (searchWord != "X")
                result = result.Where(v => v[prop.Name].ToString() == searchWord);
        }

        return result.ToList();
    }

   
    //public IEnumerable<IVehicle> SearchVehicle2(VehicleTypeInfo? typeInfo)
    //{
    //    var properties = typeInfo is null
    //        ? typeof(Vehicle).GetFilteredProperties()
    //        : typeInfo.Properties;

    //    var filters = properties
    //        .Select(p => (Property: p, SearchWord: util.AskForString(p.GetDisplayText()).ToUpper()))
    //        .Where(f => f.SearchWord != "X")
    //        .ToList();

    //    var result = typeInfo is null
    //        ? garage
    //        : garage.Where(v => v.GetType() == typeInfo.Type);

    //    return result
    //        .Where(v => filters.All(f => v[f.Property.Name].ToString() == f.SearchWord))
    //        .ToList();
    //}


    /// <summary>
    /// Parks a vehicle in the garage.
    /// </summary>
    /// <param name="v">The vehicle to park.</param>
    /// <returns>True if parked successfully, otherwise false.</returns>
    public bool Park(IVehicle v)
    {
        return garage.Park(v);
    }

    /// <summary>
    /// Gets a vehicle by registration number.
    /// </summary>
    /// <param name="regNo">The registration number.</param>
    /// <returns>The vehicle if found, otherwise null.</returns>
    public IVehicle? Get(string regNo)
    {
        return garage.FirstOrDefault(v => v.RegNo == regNo);
    }

    /// <summary>
    /// Unparks a vehicle by registration number.
    /// </summary>
    /// <param name="regNo">The registration number.</param>
    /// <returns>True if unparked successfully, otherwise false.</returns>
    public bool Leave(string regNo)
    {
        var match = Get(regNo);
        return match is not null && garage.Leave(match);
    }
    
    /// <summary>
    /// Loads a garage from persistent storage and populates it with vehicles.
    /// </summary>
    public void Load()
    {
        var dto = persitiance.LoadGarage(garage.Name);
        garage = garage.CreateNew(dto.Settings);
        dto.Vehicles.ForEach(v => garage.Park(v));
    }

}
