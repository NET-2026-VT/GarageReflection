namespace GarageDI;

/// <summary>
/// Handles application startup, dependency injection, and configuration.
/// </summary>
internal class StartUp
{
    /// <summary>
    /// Sets up dependency injection and runs the application.
    /// </summary>
    internal void SetUp()
    {
        ServiceCollection serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        try
        {
            serviceProvider.GetRequiredService<GarageManager>().Run();
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error with following message: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// Configures all required services.
    /// </summary>
    private void ConfigureServices(IServiceCollection services)
    {
        services.GetGarageSettings(GetConfig());
        services.AddGarageServices();
    }

    /// <summary>
    /// Loads the application configuration from appsettings.json.
    /// </summary>
    /// <returns>The application configuration root.</returns>
    private static IConfigurationRoot GetConfig() =>
                     new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile(ConfigHelpers.AppSettingsFileName, optional: true, reloadOnChange: true)
                        .Build();
}


