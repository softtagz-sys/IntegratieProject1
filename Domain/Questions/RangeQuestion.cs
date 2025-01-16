namespace BL.Domain.Questions;

public class RangeQuestion : Question
{
    public int Min { get; set; }
    public int Max { get; set; }
    
    public RangeQuestion()
    {
    }
    
    public RangeQuestion(int position, string text, int min, int max, Media media) : base(position, text, QuestionType.Range, media)
    {
        Min = min;
        Max = max;
    }
}