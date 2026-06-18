using GarageDI.UI;
using GarageDI.Services;

namespace GarageDI.Garage;

/// <summary>
/// Manages the main garage operations, user interface, and menu navigation.
/// </summary>
internal class GarageManager
{
    private readonly IUI _ui;
    private readonly IGarageHandler _handler;
    private readonly IUtil _util;
    private readonly IVehicleTypeFindService _typeFindService;
    private readonly Dictionary<int, Action> _menuOptions;
    private string _parkMenuOptions;

    public GarageManager(IUI ui, IGarageHandler handler, IUtil util, IVehicleTypeFindService typeFindService)
    {
        _ui = ui;
        _handler = handler;
        _util = util;
        _typeFindService = typeFindService;
        _menuOptions = GetMenuOptions();
        _parkMenuOptions = GetParkMenuOptions();
    }

    /// <summary>
    /// Entry point for running the main menu loop.
    /// </summary>
    internal void Run()
    {
        var menuItems = GetMainMenuItems();
        do
        {
            int selectedIndex = MenuNavigationHelper.ShowSelectionMenu(
                _ui,
                menuItems,
                0
            );
            if (_menuOptions.TryGetValue(selectedIndex, out Action? value))
            {
                value?.Invoke();
                Pause();
            }
        } while (true);
    }

    /// <summary>
    /// Loads a garage from persistent storage, optionally changing the current garage.
    /// </summary>
    private void Load()
    {
        var answer = _util.AskForString("Change the current garage? Y for yes, N for no");
        if (answer.Equals("Y", StringComparison.CurrentCultureIgnoreCase)) _handler.Load();
    }

    /// <summary>
    /// Searches for vehicles in the garage based on user criteria.
    /// </summary>
    private void Search()
    {
        _ui.Menu(isFull: false, GetParkMenuOptions(), "Search menu!" +
                                           "\nSkip search criteria with x" +
                                           "\nSelect 'All vehicles' to search across all types");
        _ui.Print("");

        VehicleTypeInfo? selectedVehicleType = ChooseVehicle(search: true);

        IEnumerable<IVehicle> vehicles = _handler.SearchVehicle(selectedVehicleType);
        PrintAll(vehicles);

    }

    private void PrintAll<T>(IEnumerable<T> vehicles)
    {
        if (!vehicles.Any()) _ui.Print("No result");
        else
        {
            foreach (var v in vehicles)
            {
                if (v is IVehicle vehicle)
                    _ui.Print(vehicle.GetInfo());
                else
                    _ui.Print(v?.ToString()!);
            }
        }
    }

    /// <summary>
    /// Unparks a vehicle by registration number.
    /// </summary>
    private void UnPark()
    {
        var regNo = _util.AskForString("Enter reg number").ToUpper();
        _ui.Print(_handler.Leave(regNo) ?
            $"Vehicle: [{regNo}] unparked" :
            $"Can´t find vehicle with registration number {regNo}");
    }

    /// <summary>
    /// Lists all parked vehicles information.
    /// </summary>
    private void ListParked()
    {
        PrintAll(_handler.GetVehicleInfo());
    }

    /// <summary>
    /// Lists the count of vehicles by type.
    /// </summary>
    private void ListByType()
    {
        _ui.Print("List by type");
        _handler.GetByType().ForEach(r => _ui.Print($"Type: {r.Name} Count: {r.Count}"));
    }

    /// <summary>
    /// Parks a new vehicle in the garage.
    /// </summary>
    private void Park()
    {
        _ui.Clear();
        _ui.Menu(_handler.IsGarageFull, GetParkMenuOptions(), "Park menu");
        if (_handler.IsGarageFull) return;

        VehicleTypeInfo? selectedType = ChooseVehicle(search: false);
        if (selectedType == null) throw new ArgumentNullException(nameof(selectedType)); //Should not be possible
        
        IVehicle vehicle = _handler.GetVehicle(selectedType, RegNoIsAvailable);

        _ui.Print(_handler.Park(vehicle) ?
            $"[{selectedType.Name}] with registration number:{vehicle.RegNo} parked" :
            $"Something failed");
    }

    /// <summary>
    /// Chooses a vehicle type from the menu using arrow key navigation.
    /// </summary>
    /// <param name="search">If true, allows selecting 'all vehicles'.</param>
    /// <returns>The selected vehicle type info, or null for 'all vehicles'.</returns>
    private VehicleTypeInfo? ChooseVehicle(bool search)
    {
        var vehicleTypes = _typeFindService.AvailableTypes;
        List<string> options = vehicleTypes.Select(t => t.Name).ToList();        
        
        if (search)
            options.Insert(0, "All vehicles");
        
        int selectedIndex = MenuNavigationHelper.ShowSelectionMenu(
            _ui,
            options.ToArray(),
            0
        );
        
        // If search can't choose All vehicles
        if (search && selectedIndex == 0)
            return null;
        
        // Adjust index in search mode  
        int vehicleTypeIndex = search ? selectedIndex - 1 : selectedIndex;
        return vehicleTypes[vehicleTypeIndex];
    }

    /// <summary>
    /// Fyller garaget med exempelfordon för demo/testning.
    /// </summary>
    private void Seed()
    {
        _handler.Park(new Buss() { RegNo = "AAA100", NrOfWheels = 12, Color = "RED",  Seats = 10 });
        _handler.Park(new Buss() { RegNo = "AAA200", NrOfWheels = 12, Color = "RED",  Seats = 11 });
        _handler.Park(new Buss() { RegNo = "AAA300", NrOfWheels = 12, Color = "BLUE", Seats = 12 });
        _handler.Park(new Buss() { RegNo = "BBB100", NrOfWheels = 12, Color = "BLUE", Seats = 13 });
        _handler.Park(new Buss() { RegNo = "BBB200", NrOfWheels = 12, Color = "RED",  Seats = 14 });
        _handler.Park(new Buss() { RegNo = "BBB300", NrOfWheels = 10, Color = "BLUE", Seats = 15 });
        _handler.Park(new Car()  { RegNo = "BBB400", NrOfWheels = 4, Color = "BLUE" });
        _handler.Park(new Car()  { RegNo = "BBB500", NrOfWheels = 6, Color = "RED" });
    }

    private bool RegNoIsAvailable(string regNo)
    {
        if (_handler.Get(regNo.ToUpper()) != null)
        {
            _ui.Print($"Reg number:{regNo} is already in the garage!");
            Pause(message: false);
            return false;
        }
        return true;
    }

    private string GetParkMenuOptions()
    {
        if (!string.IsNullOrEmpty(_parkMenuOptions))
            return _parkMenuOptions;

        return _parkMenuOptions = string.Join(Environment.NewLine,
         _typeFindService.AvailableTypes.Select(v => v.Name));
    }

    private string[] GetMainMenuItems()
    {
        return
        [
            $"Park",
            $"List Parked",
            $"List By Type",
            $"UnPark",
            $"Search",
            $"Seed Vehicles",
            $"Load",
            $"Quit"
        ];
    }

    private void Pause(bool message = true)
    {
        if (message) _ui.Pause();
        else _ui.Pause("");
    }

    private Dictionary<int, Action> GetMenuOptions()
    {
        return new Dictionary<int, Action>
        {
            {MenuConstants.Park, Park },
            {MenuConstants.ListAll, ListParked },
            {MenuConstants.ListByType, ListByType },
            {MenuConstants.Unpark, UnPark},
            {MenuConstants.Search, Search },
            {MenuConstants.Seed, Seed },
            {MenuConstants.Load, Load },
            {MenuConstants.Quit, () => Environment.Exit(0) }
        };
    }
}
