namespace IP_MVC.Models.Dto;

public class ProjectEditDto
{
    public int ProjectId { get; set; }
    public string AdminId { get; set; }
    public string NewName { get; set; }
    public string NewDescription { get; set; }
}