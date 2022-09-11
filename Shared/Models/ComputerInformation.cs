namespace InventoryManagement.Shared.Models;

public class Host
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public bool IsOnline { get; set; }
    public string MachineName { get; set; }
    public ComputerInformation ComputerInformation { get; set; }
}

public class ComputerInformation
{
    public int Id { get; set; }
    public string OsVersion { get; set; }
    public string SystemDirectory { get; set; }
    public int ProcessorCount { get; set; }
    public string DomainName { get; set; }
    public string UserName { get; set; }
    public List<DriveProperties> DriveProperties { get; set; }
    public string[] InstalledProducts { get; set; }
}

public class DriveProperties
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string VolumeLabel { get; set; }
    public string DriveType { get; set; } 
    public string DriveFormat { get; set; }
    public long TotalSize { get; set; }
    public long AvailableFreeSpace { get; set; }
}
