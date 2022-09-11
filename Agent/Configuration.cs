using System.Reflection;
using System.Text.Json;

namespace InventoryManagement.Agent;

public class Configuration
{
    public Guid Guid { get; set; }
    public byte Version { get; set; }
    public string Hostname { get; set; }
}

public static class HandleConfiguration
{
    private const string ConfigurationFileName = "AgentConfiguration.json";
    private const byte ConfigurationVersion = 1;
    
    public static async void InitConfiguration()
    {
        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (File.Exists(Path.Join(workingDirectory, ConfigurationFileName))) return;
        
        var configuration = new Configuration
        {
            Version = ConfigurationVersion,
            Hostname = string.Empty,
            Guid = Guid.NewGuid()
        };

        var fileName = Path.Join(workingDirectory, ConfigurationFileName);
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
        
        if (!File.Exists(Path.Join(workingDirectory, ConfigurationFileName))) InitConfiguration();
        
        var fileName = Path.Join(workingDirectory, ConfigurationFileName);
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
        File.Delete(Path.Join(workingDirectory, ConfigurationFileName));

        var configPostMig = new Configuration
        {
            Version = newConfigVersion,
            Guid = configuration.Guid,
            Hostname = configuration.Hostname
        };

        var fileName = Path.Join(workingDirectory, ConfigurationFileName);
        await using var createStream = File.Create(fileName);
        await JsonSerializer.SerializeAsync(createStream, configPostMig,
            new JsonSerializerOptions {WriteIndented = true});
        await createStream.DisposeAsync();
        Console.WriteLine("Configuration migrated. Exiting... " +
                          "\nPlease check the configuration, confirm and restart the application.");
        Environment.Exit(2);
    }
}
