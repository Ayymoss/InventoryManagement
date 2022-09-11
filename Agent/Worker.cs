using System.Reflection;
using System.Text.Json;
using System.Timers;
using InventoryManagement.Shared.Models;
using Microsoft.Win32;
using Timer = System.Timers.Timer;

namespace InventoryManagement.Agent;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private const string Name = "Inventory Management Worker";

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
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
    {
        var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "ComputerInformation.json");
        if (File.Exists(filePath)) File.Delete(filePath);

        var computerInformation = new ComputerInformation().GetComputerInformation();

        await using var createStream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(createStream, computerInformation, new JsonSerializerOptions {WriteIndented = true});
        await createStream.DisposeAsync();
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
