using BL.Domain;
using BL.Domain.Questions;

namespace IP_MVC.Models;

public class FlowEditViewModel
{
    public Flow Flow { get; set; }
    public IEnumerable<Flow> SubFlows { get; set; }
    public IEnumerable<Question> Questions { get; set; }
    
    public int Position { get; set; }
}