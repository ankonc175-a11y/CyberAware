namespace CyberAware.DataAccess.Entities;

public class Attempt
{
    public int AttemptID { get; set; }
    public int UserID { get; set; }
    public int ModuleID { get; set; }
    public double Score { get; set; }
    public bool Passed { get; set; }
    public DateTime AttemptDate { get; set; } = DateTime.UtcNow;
    public int DurationSeconds { get; set; }

    public User User { get; set; } = null!;
    public Module Module { get; set; } = null!;
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}
