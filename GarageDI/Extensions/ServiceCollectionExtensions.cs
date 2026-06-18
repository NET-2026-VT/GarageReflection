using GarageDI.Services;

namespace GarageDI.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register garage services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all garage-related services for dependency injection.
    /// </summary>
    /// <param name="services">The service collection to add to.</param>
    public static void AddGarageServices(this IServiceCollection services)
    {
        services.AddSingleton<GarageManager>();
        services.AddSingleton<IGarage<IVehicle>, InMemoryGarage<IVehicle>>();
        services.AddSingleton<IGarageHandler, GarageHandler>();
        services.AddTransient<IUI, ConsoleUI>();
        services.AddSingleton<IUtil, Util>();
        services.AddSingleton<IVehicleTypeFindService, VehicleTypeFindService>();
        services.AddSingleton<IGaragePersistenceService, GaragePersistenceService>();
    }

    /// <summary>
    /// Gets and register Settings from configuration for dependency injection
    /// </summary>
    /// <param name="services">The service collection to add to.</param>
    /// <param name="configuration">Configuration to read out settings from appsettings.json</param>
    public static void GetGarageSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);

        Settings? settings = configuration.GetSection(ConfigHelpers.GetSettingsFromConfig).Get<Settings>();
        ArgumentNullException.ThrowIfNull(settings, nameof(settings));
        try
        {
            settings.Validate();
        }
        catch (ArgumentException ex)
        {
            Debug.WriteLine($"Configuration validation failed: {ex.Message}");
            throw;
        }

        services.AddSingleton<ISettings>(settings);
    }

}