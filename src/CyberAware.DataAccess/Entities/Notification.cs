namespace CyberAware.DataAccess.Entities;

public class Notification
{
    public int NotificationID { get; set; }
    public int UserID { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
