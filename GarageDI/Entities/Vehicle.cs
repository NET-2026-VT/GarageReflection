using GarageDI.Attributes;
namespace GarageDI.Entities;

/// <summary>
/// Base class for all vehicle types in the garage system.
/// </summary>
public abstract class Vehicle : IVehicle
{
    private string _regNo;
    private string _color;
    private string? _info;

    [Exclude]
    public string Name { get; }

    public Vehicle()
    {
        Name = GetType().Name;
        _color = "Default";
        _regNo = "Default";
    }


    [Beautify("Registration number")]
    [Order(1)]
    /// <summary>
    /// Registreringsnummer. Sparas i versaler.
    /// Kastar <see cref="ArgumentException"/> om värdet är tomt eller bara innehåller blanksteg.
    /// </summary>
    public required string RegNo
    {
        get => _regNo;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Registreringsnummer får inte vara tomt.", nameof(RegNo));

            _regNo = value.ToUpper();
        }
    }


    /// <summary>
    /// Gets or sets the color of the vehicle.
    /// </summary>
    [Order(3)]
    public required string Color
    {
        get { return _color; }
        set { _color = value.ToUpper(); }
    }

    [Beautify("Number of wheels")]
    [Order(2)]
    public required int NrOfWheels { get; set; }

    /// <summary>
    /// Gets or sets a property value by name using reflection.
    /// </summary>
    [Exclude]
    public virtual object this[string name]
    {
        get
        {
            PropertyInfo? prop = GetType().GetProperty(name);

            if (prop != null)
                return prop.GetValue(this)!;
            else
                throw new ArgumentException($"Property '{name}' does not exist on type '{GetType().Name}'.", nameof(name));
        }
        set
        {
            PropertyInfo? prop = GetType().GetProperty(name);
            if (prop != null)
                prop.SetValue(this, value);
            else
                throw new ArgumentException($"Property '{name}' does not exist on type '{GetType().Name}'.", nameof(name));
        }
    }

    /// <summary>
    /// Gets a string representation of the vehicle's details.
    /// </summary>
    public virtual string GetInfo()
    {
        if (_info is not null) return _info;

        var builder = new StringBuilder().Append($"[{Name}]\t\t");

        Array.ForEach(GetType().GetFilteredProperties(),
                      p => builder.Append($" {p.GetDisplayText()}: {p.GetValue(this)}\t"));

        return _info = builder.ToString();
    }
}

