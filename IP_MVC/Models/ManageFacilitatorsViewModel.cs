namespace IP_MVC.Models;

public class ManageFacilitatorsViewModel
{
    public IEnumerable<Microsoft.AspNetCore.Identity.IdentityUser> Facilitators { get; set; }
    public int ProjectId { get; set; }
}