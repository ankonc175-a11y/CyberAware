namespace CyberAware.DataAccess.Entities;

public class ModuleVersion
{
    public int VersionID { get; set; }
    public int ModuleID { get; set; }
    public int VersionNumber { get; set; }
    public string ContentSnapshot { get; set; } = string.Empty;
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Module Module { get; set; } = null!;
    public User Creator { get; set; } = null!;
}
