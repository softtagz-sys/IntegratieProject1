using BL.Domain.Questions;

namespace IP_MVC.Models.Dto;

public class QuestionCreateDto
{
    public string Text { get; set; }
    public string Type { get; set; }
    public int FlowId { get; set; }
}