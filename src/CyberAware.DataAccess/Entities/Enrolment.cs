namespace CyberAware.DataAccess.Entities;

public class Enrolment
{
    public int EnrolmentID { get; set; }
    public int UserID { get; set; }
    public int ModuleID { get; set; }
    public int EnrolledBy { get; set; }
    public string? PathwayName { get; set; }
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Module Module { get; set; } = null!;
    public User Enroller { get; set; } = null!;
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
