namespace CyberAware.DataAccess.Entities;

public class Module
{
    public int ModuleID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CreatedBy { get; set; }
    public bool IsPublished { get; set; } = false;
    public int PassMark { get; set; } = 50;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User Creator { get; set; } = null!;
    public ICollection<ContentSection> ContentSections { get; set; } = new List<ContentSection>();
    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    public ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    public ICollection<ModuleVersion> ModuleVersions { get; set; } = new List<ModuleVersion>();
}
