namespace GarageDI.Garage;

public class InMemoryGarage<T> : IGarage<T> where T : IVehicle
{

    private readonly T[] _vehicles;
    private readonly IGaragePersistenceService _percistance;

    public string Name { get; }

    public int Capacity { get; init; }

    public int Count { get; private set; } = 0;

    public bool IsFull => Count >= Capacity;


    public InMemoryGarage(ISettings settings, IGaragePersistenceService percistance)
    {
        ArgumentNullException.ThrowIfNull(settings);

        Name = settings.Name;
        Capacity = Math.Max(2, settings.Capacity);
        _vehicles = new T[Capacity];
        this._percistance = percistance;
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var v in _vehicles)
        {
            if (v != null) yield return v;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    public bool Park(T vehicle)
    {
        if (IsFull) return false;

        if (_vehicles.Any(v => v != null && v.RegNo == vehicle.RegNo))
            return false;

        var index = Array.IndexOf(_vehicles, null);

        if (index != -1)
        {
            _vehicles[index] = vehicle;
            Count++;
            Save();
            return true;
        }

        return false;
    }

    private void Save()
    {
        var onlyParkedVehicles = _vehicles.Cast<IVehicle>().Where(v => v != null).ToList();
        _percistance.SaveGarage(Name, Capacity, onlyParkedVehicles);
    }

    public bool Leave(T vehicle)
    {
        bool result = false;
        if (vehicle is null) return result;

        var index = Array.IndexOf(_vehicles, vehicle);

        if (index != -1)
        {
            _vehicles[index] = default!;
            Count--;
            Save();
            result = true;
        }

        return result;
    }

    public IGarage<T> CreateNew(ISettings settings) => 
       new InMemoryGarage<T>(settings, _percistance);
    
}
