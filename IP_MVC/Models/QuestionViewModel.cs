using BL.Domain.Answers;
using BL.Domain.Questions;

namespace IP_MVC.Models;

public class QuestionViewModel
{
    public Question Question { get; set; }
    public QuestionType QuestionType { get; set; }
    public Answer EarlierAnswer { get; set; }
}