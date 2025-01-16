using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Domain.Questions
{
    public class SingleChoiceQuestion : Question
    {
        //public new ICollection<Option> Options { get; set; }
        
        public SingleChoiceQuestion()
        {
        }

        public SingleChoiceQuestion(int position, string text, ICollection<Option> options, Media media) : base(position, text, QuestionType.SingleChoice, media)
        {
            Options = options;
        }

        
    }
}