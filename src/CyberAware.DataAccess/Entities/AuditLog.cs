namespace CyberAware.DataAccess.Entities;

public class AuditLog
{
    public int LogID { get; set; }
    public int UserID { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? TargetTable { get; set; }
    public int? TargetID { get; set; }
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
