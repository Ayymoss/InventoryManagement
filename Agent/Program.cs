namespace InventoryManagement.Agent;

public static class Agent
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .UseWindowsService(options => { options.ServiceName = "Inventory Management Agent"; })
            .ConfigureServices(services => { services.AddHostedService<Worker>(); })
            .Build();

        await host.RunAsync();
    }
}
