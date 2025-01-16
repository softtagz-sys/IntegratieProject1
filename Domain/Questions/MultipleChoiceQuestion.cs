using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Options;

namespace BL.Domain.Questions;

public class MultipleChoiceQuestion : Question
{
    //public new ICollection<Option> Options { get; set; }
    
    public MultipleChoiceQuestion()
    {
    }

    public MultipleChoiceQuestion(int position, string text, ICollection<Option> options, Media media) : base(position, text, QuestionType.MultipleChoice, media)
    {
        Options = options;
    }
}