using InventoryManagement.Shared.Models;
using Microsoft.Win32;

namespace InventoryManagement.Agent;

public class ComputerInformation
{
    private readonly Configuration _configuration;

    public ComputerInformation(Configuration configuration)
    {
        _configuration = configuration;
    }

    public HostInformation GetComputerInformation()
    {
        var host = new HostInformation
        {
            Guid = _configuration.Guid,
            Debug = "testing",
            IsOnline = true,
            MachineName = Environment.MachineName,
            ComputerInformation = new Shared.Models.ComputerInformation
            {
                OsVersion = Environment.OSVersion.VersionString,
                SystemDirectory = Environment.SystemDirectory,
                ProcessorCount = Environment.ProcessorCount,
                DomainName = Environment.UserDomainName,
                UserName = Environment.UserName,
                Drives = GetDriveInformation(),
                Products = GetInstalledProducts()
            }
        };

        return host;
    }

    private List<Product> GetInstalledProducts()
    {
        var installedProducts = new List<Product>();
        const string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        using var key = Registry.LocalMachine.OpenSubKey(registryKey);
       
        foreach (var subKeyNames in key.GetSubKeyNames())
        {
            using var subKey = key.OpenSubKey(subKeyNames);
            
            if (subKey == null) continue;

            var product = new Product
            {
                Name = subKey.GetValue("DisplayName").ToString(),
                Version = subKey.GetValue("DisplayVersion") != null
                    ? subKey.GetValue("DisplayVersion").ToString()
                    : "N/A",
                Publisher = subKey.GetValue("Publisher") != null 
                    ? subKey.GetValue("Publisher").ToString() 
                    : "N/A",
                InstallLocation = subKey.OpenSubKey("InstallLocation") != null
                    ? subKey.OpenSubKey("InstallLocation").ToString()
                    : "N/A"
            };

            installedProducts.Add(product);
        }

        return installedProducts;
    }

    private List<Drive> GetDriveInformation()
    {
        var drives = new List<Drive>();

        foreach (var drive in DriveInfo.GetDrives())
        {
            try
            {
                drives.Add(new Drive
                {
                    Name = drive.Name,
                    DriveFormat = drive.DriveFormat,
                    DriveType = drive.DriveType.ToString(),
                    VolumeLabel = drive.VolumeLabel,
                    AvailableFreeSpace = drive.AvailableFreeSpace / (1024 * 1024 * 1024),
                    TotalSize = drive.TotalSize / (1024 * 1024 * 1024)
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return drives;
    }
}
