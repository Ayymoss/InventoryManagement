using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Shared.Models;

public class HostInformation
{
    public int Id { get; set; }
    public string Debug { get; set; }
    public Guid Guid { get; set; }
    public bool IsOnline { get; set; }
    public string MachineName { get; set; }
    
    public int ComputerInformationId { get; set; }
    [ForeignKey(nameof(ComputerInformationId))] public ComputerInformation ComputerInformation { get; set; }
}

public class ComputerInformation
{
    public int Id { get; set; }
    public string OsVersion { get; set; }
    public string SystemDirectory { get; set; }
    public int ProcessorCount { get; set; }
    public string DomainName { get; set; }
    public string UserName { get; set; }
    
    public int DriveId { get; set; }
    [ForeignKey(nameof(DriveId))] public List<Drive> Drives { get; set; }

    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))] public List<Product> Products { get; set; }
}

public class Drive
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string VolumeLabel { get; set; }
    public string DriveType { get; set; }
    public string DriveFormat { get; set; }
    public long TotalSize { get; set; }
    public long AvailableFreeSpace { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Version { get; set; }
    public string Publisher { get; set; }
    public string InstallLocation { get; set; }
}
