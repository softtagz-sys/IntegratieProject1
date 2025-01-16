using System.ComponentModel.DataAnnotations;
using BL.Domain.Answers;

namespace BL.Domain;

public class Session
{
    [Key]
    public int Id { get; set; }
    public int FlowId { get; set; }
    public ICollection<Answer> Answers { get; set; }
    
    public Session() { }
    
    public Session(int flowId, ICollection<Answer> answers)
    {
        FlowId = flowId;
        Answers = answers;
    }
    
}