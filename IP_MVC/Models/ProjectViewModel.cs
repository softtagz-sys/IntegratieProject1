using BL.Domain;
using Microsoft.AspNetCore.Identity;

namespace IP_MVC.Models;

public class ProjectViewModel
{
    public Project Project { get; set; }
    public IdentityUser Owner { get; set; }
}