using Microsoft.AspNetCore.Identity;

namespace BL.Domain;

public class ProjectFacilitator
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    public string FacilitatorId { get; set; }
    public IdentityUser Facilitator { get; set; }
    
    public ProjectFacilitator(int projectId, string facilitatorId)
    {
        ProjectId = projectId;
        FacilitatorId = facilitatorId;
    }
    
    public ProjectFacilitator()
    {
    }
}