using Microsoft.AspNetCore.Identity;

namespace BL.Domain.Questions;

public class Question
{
    public int Id { get; set; }
    public int Position { get; set; }
    public string Text { get; set; }
    public QuestionType Type { get; set; }
    public Media Media { get; set; }
    public int FlowId { get; set; }
    public virtual ICollection<Option> Options { get; set; }

    public Question()
    {
        Options = new List<Option>();
    }

    public Question(int position, string text, QuestionType type, Media media)
    {
        Position = position;
        Text = text;
        Type = type;
        Media = media;
    }
}
