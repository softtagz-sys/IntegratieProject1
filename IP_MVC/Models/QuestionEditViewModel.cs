using BL.Domain;
using BL.Domain.Questions;

namespace IP_MVC.Models;

public class QuestionEditViewModel
{
    public int Id { get; set; }
    public int Position { get; set; }
    public string Text { get; set; }
    public QuestionType Type { get; set; }
    public Media Media { get; set; }
    public int FlowId { get; set; }
    
    public List<string> Options { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }
    
    
}