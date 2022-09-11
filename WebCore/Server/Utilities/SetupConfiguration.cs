using System.Reflection;
using System.Text.Json;

namespace InventoryManagement.WebCore.Server.Utilities;

public class Configuration
{
    public byte Version  = 1;
    public DatabaseConfigurationProperties Database { get; set; }
}

public static class SetupConfiguration
{
    public static async void InitConfiguration()
    {
        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (File.Exists(Path.Join(workingDirectory, "GlobalConfiguration.json"))) return;

        var dbConfig = DatabaseConfiguration.PromptBuildConfig();
        
        var configuration = new Configuration
        {
            Version = 1,
            Database = dbConfig
        };

        var fileName = Path.Join(workingDirectory, "GlobalConfiguration.json");
        await using var createStream = File.Create(fileName);
        await JsonSerializer.SerializeAsync(createStream, configuration,
            new JsonSerializerOptions {WriteIndented = true});
        await createStream.DisposeAsync();
        Console.WriteLine("Configuration created. Exiting... " +
                          "\nPlease check the configuration, confirm and restart the application.");
        Environment.Exit(1);
    }

    public static Configuration ReadConfiguration()
    {
        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var fileName = Path.Join(workingDirectory, "GlobalConfiguration.json");
        var jsonString = File.ReadAllText(fileName);
        var configuration = JsonSerializer.Deserialize<Configuration>(jsonString);

        if (configuration == null)
        {
            Console.WriteLine("Configuration empty? Delete it for recreation.");
            Environment.Exit(-1);
        }

        var newConfigVersion = new Configuration().Version;

        if (newConfigVersion > configuration.Version)
        {
            MigrateConfiguration(configuration, newConfigVersion);
        }

        return configuration;
    }

    private static async void MigrateConfiguration(Configuration configuration, byte newConfigVersion)
    {
        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        File.Delete(Path.Join(workingDirectory, "GlobalConfiguration.json"));

        var configPostMig = new Configuration
        {
            Version = newConfigVersion,
            Database = configuration.Database
        };

        var fileName = Path.Join(workingDirectory, "GlobalConfiguration.json");
        await using var createStream = File.Create(fileName);
        await JsonSerializer.SerializeAsync(createStream, configPostMig,
            new JsonSerializerOptions {WriteIndented = true});
        await createStream.DisposeAsync();
        Console.WriteLine("Configuration migrated. Exiting... " +
                          "\nPlease check the configuration, confirm and restart the application.");
        Environment.Exit(2);
    }
}
