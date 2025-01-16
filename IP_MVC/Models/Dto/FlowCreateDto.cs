namespace IP_MVC.Models.Dto;

public class FlowCreateDto
{
    public int NewProjectId { get; set; }
    public string NewName { get; set; }
    public DateTime NewStartDate { get; set; }
    public DateTime NewEndDate { get; set; }
    public string NewDescription { get; set; }
    public int? NewParentFlowId { get; set; }
}