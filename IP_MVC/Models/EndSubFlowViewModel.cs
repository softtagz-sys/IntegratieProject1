using BL.Domain.Answers;
using BL.Domain.Questions;

namespace IP_MVC.Models;

public class EndSubFlowViewModel
{
    public List<Question> Questions { get; set; }
    public List<Answer> Answers { get; set; }
    public int FlowId { get; set; }
}