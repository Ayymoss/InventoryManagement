namespace InventoryManagement.Agent;

public static class Agent
{
    
    // TODO: Fix Null Reference Exception
    /*
     *      at InventoryManagement.Agent.ComputerInformation.GetInstalledProducts() in D:\Users\Amos\RiderProjects\_PrimaryProjects\InventoryManagement\Agent\ComputerInformation.cs:line 50
     *      at InventoryManagement.Agent.ComputerInformation.GetComputerInformation() in D:\Users\Amos\RiderProjects\_PrimaryProjects\InventoryManagement\Agent\ComputerInformation.cs:line 17
     *      at InventoryManagement.Agent.Worker.GetSystemInformation(Object source, ElapsedEventArgs e) in D:\Users\Amos\RiderProjects\_PrimaryProjects\InventoryManagement\Agent\Worker.cs:line 31
     */
    public static async Task Main(string[] args)
    {
        var configuration = HandleConfiguration.ReadConfiguration();

        var host = Host.CreateDefaultBuilder(args)
            .UseWindowsService(options => { options.ServiceName = "Inventory Management Agent"; })
            .ConfigureServices(services =>
            {
                services.AddSingleton(configuration);
                services.AddHostedService<Worker>();
            })
            .Build();
        
        await host.RunAsync();
    }
}
