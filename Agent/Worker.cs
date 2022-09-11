using System.Net.Http.Json;
using System.Timers;
using Timer = System.Timers.Timer;

namespace InventoryManagement.Agent;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly Configuration _configuration;
    private const string Name = "Inventory Management Worker";

    public Worker(ILogger<Worker> logger, Configuration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new Timer();
        timer.Elapsed += GetSystemInformation;
        timer.Interval = 10_000;
        timer.AutoReset = true;
        timer.Enabled = true;
    }

    private async void GetSystemInformation(object? source, ElapsedEventArgs e)
    //private async void GetSystemInformation()
    {
        var computerInformation = new ComputerInformation(_configuration).GetComputerInformation();

        var client = new HttpClient();
        var response =
            await client.PostAsJsonAsync($"http://{_configuration.Hostname}:5000/api/Host", computerInformation);
        Console.WriteLine($"Server Response: {await response.Content.ReadAsStringAsync()}");
        response.EnsureSuccessStatusCode();
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{Name} starting...");
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{Name} stopping...");
        return base.StopAsync(cancellationToken);
    }
}
