namespace CyberAware.DataAccess.Entities;

public class Schedule
{
    public int ScheduleID { get; set; }
    public int ModuleID { get; set; }
    public int EnrolmentID { get; set; }
    public int FrequencyDays { get; set; }
    public DateTime NextDueDate { get; set; }

    public Module Module { get; set; } = null!;
    public Enrolment Enrolment { get; set; } = null!;
}
