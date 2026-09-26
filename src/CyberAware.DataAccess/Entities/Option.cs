namespace CyberAware.DataAccess.Entities;

public class Option
{
    public int OptionID { get; set; }
    public int QuestionID { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; } = false;

    public Question Question { get; set; } = null!;
}
