using InventoryManagement.Shared.Models;
using Microsoft.Win32;
using Host = InventoryManagement.Shared.Models.Host;

namespace InventoryManagement.Agent;

public class ComputerInformation
{
    public Host GetComputerInformation()
    {
        var host = new Host
        {
            Guid = Guid.NewGuid(),
            IsOnline = true,
            MachineName = Environment.MachineName,
            ComputerInformation = new Shared.Models.ComputerInformation
            {
                OsVersion = Environment.OSVersion.VersionString,
                SystemDirectory = Environment.SystemDirectory,
                ProcessorCount = Environment.ProcessorCount,
                DomainName = Environment.UserDomainName,
                UserName = Environment.UserName,
                DriveProperties = GetDriveInformation(),
                InstalledProducts = GetInstalledProducts()
            }
        };

        return host;
    }

    private string[] GetInstalledProducts()
    {
        var installedProducts = Array.Empty<string>();
        const string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        using var key = Registry.LocalMachine.OpenSubKey(registryKey);
        foreach (var subKeyNames in key.GetSubKeyNames())
        {
            using var subKey = key.OpenSubKey(subKeyNames);
            var name = subKey.GetValue("DisplayName");
            if (name is null) continue;
            installedProducts = installedProducts.Append($"{name}").ToArray();
        }

        return installedProducts;
    }

    private List<DriveProperties> GetDriveInformation()
    {
        var drives = new List<DriveProperties>();

        foreach (var drive in DriveInfo.GetDrives())
        {
            try
            {
                drives.Add(new DriveProperties
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
