namespace CyberAware.DataAccess.Entities;

public class Answer
{
    public int AnswerID { get; set; }
    public int AttemptID { get; set; }
    public int QuestionID { get; set; }
    public int? SelectedOptionID { get; set; }

    public Attempt Attempt { get; set; } = null!;
    public Question Question { get; set; } = null!;
    public Option? SelectedOption { get; set; }
}
