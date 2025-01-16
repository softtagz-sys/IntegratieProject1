namespace BL.Domain.Questions;

public class Option
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int? NextQuestionId { get; set; }
    
    public Option()
    {
    }

    public Option(string _text, int? nextQuestionId)
    {
        Text = _text;
        NextQuestionId = nextQuestionId;
        Console.WriteLine("Option created" + Text);
    }
}