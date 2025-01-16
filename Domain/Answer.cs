using System.ComponentModel.DataAnnotations;
using BL.Domain.Questions;

namespace BL.Domain.Answers
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public String AnswerTextPlayer1 { get; set; } 
        public String AnswerTextPlayer2 { get; set; } 
        public Session Session { get; set; }
        public Flow Flow { get; set; }

        public Answer()
        {
        }
    }
}