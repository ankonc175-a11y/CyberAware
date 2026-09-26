namespace CyberAware.DataAccess.Entities;

public class Question
{
    public int QuestionID { get; set; }
    public int ModuleID { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string DifficultyLevel { get; set; } = "Medium";
    public string? QuestionExplanation { get; set; }

    public Module Module { get; set; } = null!;
    public ICollection<Option> Options { get; set; } = new List<Option>();
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}
