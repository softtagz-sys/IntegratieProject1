namespace BL.Domain.Questions;

public class OpenQuestion : Question
{
    public OpenQuestion()
    {
    }

    public OpenQuestion(int position, string text, Media media) : base(position, text, QuestionType.Open, media)
    {
    }
    
}