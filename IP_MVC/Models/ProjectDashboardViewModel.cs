using BL.Domain;

namespace IP_MVC.Models;

public class ProjectDashboardViewModel
{
    public IEnumerable<Project> AdminProjects { get; set; }
    public IEnumerable<Project> FacilitatorProjects { get; set; }
}